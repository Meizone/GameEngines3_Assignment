using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Combatant : MonoBehaviour
{
    public delegate void TurnStartedEvent();
    public delegate void ResourceChangedEvent(Resource.Type type, float value, float percent);
    public event TurnStartedEvent onTurnStarted;
    public event ResourceChangedEvent onResourceValueChanged;

    [SerializeField] private uint _faction;
    public uint faction { get { return _faction; } }
    public bool isPlayerControlled = false;

    // Resources
    [SerializeField] public Transform uiOffset;
    private Collider2D selectionBox;
    [SerializeField] private Resource _aether = new Resource(0, 100, 0);
    [SerializeField] private Resource _readiness = new Resource(0, 100, 0);
    [SerializeField] private Resource _health = new Resource(0, 100, 100);
    [SerializeField] private Resource _mana = new Resource(0, 100, 100);
    public float aether { get { return _aether.current; } }
    public float readiness { get { return _readiness.current; } }
    public float health { get { return _health.current; } }
    public float mana { get { return _mana.current; } }
    public Resource[] Resources { get { return new Resource[] { _aether, _readiness, _health, _mana }; } }

    // Stats
    [SerializeField] private float _speed;
    [SerializeField, Range(0, 100)] private float _initiative;

    // Abilities
    [SerializeField]
    private Ability[] abilityTypes;
    private LinkedList<AbilityData> _abilities;
    public LinkedList<AbilityData> Abilities { get { return _abilities; } }

    private BattleManager _battle;
    public BattleManager battle { get { return _battle; } }

    private uint _turn;
    public uint turn { get { return _turn; } }

    #region Resources
    private Resource _GetResource(Resource.Type resource)
    {
        switch (resource)
        {
            case Resource.Type.Health:
                return _health;
            case Resource.Type.Mana:
                return _mana;
            case Resource.Type.Readiness:
                return _readiness;
            case Resource.Type.Aether:
                return _aether;
            default:
                return null;
        }
    }

    public float GetResource(Resource.Type resource, Resource.Query query = Resource.Query.Value)
    {
        return _GetResource(resource).Get(query);
    }

    #endregion
    #region Payments
    public bool CanPay(Payment[] costs)
    {
        foreach (Payment cost in costs)
            if (!CanPay(cost))
                return false;

        return true;
    }

    public bool CanPay(Payment payment)
    {
        return _GetResource(payment.resource).CanPay(payment.direction, payment.type, payment.amount);
    }

    public void Pay(Payment[] costs)
    {
        Resource.Value[] changes = new Resource.Value[Enum.GetValues(typeof(Resource.Type)).Length];
        foreach (Payment cost in costs)
        {
            changes[(int)cost.resource] += _GetResource(cost.resource).Pay(cost.direction, cost.type, cost.amount);
        }
        for (int i = 0; i < changes.Length; i++)
        {
            if (changes[i].amount != 0)
                _battle.EventResourceChanged(this, (Resource.Type)i, changes[i], _GetResource((Resource.Type)i).Get());
        }
    }

    public void Pay(Payment cost)
    {
        Resource.Value change = _GetResource(cost.resource).Pay(cost.direction, cost.type, cost.amount);
        _battle.EventResourceChanged(this, cost.resource, change, _GetResource(cost.resource).Get());
    }
    #endregion

    private void Awake()
    {
        selectionBox = GetComponent<Collider2D>();

        _aether = new Resource(0, 100, 0);
        _readiness = new Resource(0, 100, 0);
        _health = new Resource(0, 100, 100);
        _mana = new Resource(0, 100, 100);

        _abilities = new LinkedList<AbilityData>();
        foreach (Ability ability in abilityTypes)
        {
            AddAbility(ability);
        }
    }

    private void OnEnable()
    {
        _readiness.onValueChanged += OnReadinessValueChanged;
        _aether.onValueChanged += OnAetherValueChanged;
        _health.onValueChanged += OnHealthValueChanged;
        _mana.onValueChanged += OnManaValueChanged;
    }

    private void OnDisable()
    {
        _readiness.onValueChanged -= OnReadinessValueChanged;
        _health.onValueChanged -= OnHealthValueChanged;
        _mana.onValueChanged -= OnManaValueChanged;
    }

    private void OnReadinessValueChanged(float value, float percent)
    {
        onResourceValueChanged?.Invoke(Resource.Type.Readiness, value, percent);
    }

    private void OnAetherValueChanged(float value, float percent)
    {
        onResourceValueChanged?.Invoke(Resource.Type.Aether, value, percent);
    }

    private void OnManaValueChanged(float value, float percent)
    {
        onResourceValueChanged?.Invoke(Resource.Type.Mana, value, percent);
    }

    private void OnHealthValueChanged(float value, float percent)
    {
        onResourceValueChanged?.Invoke(Resource.Type.Health, value, percent);
    }

    public void AddAbility(Ability ability)
    {
        if (_abilities == null)
            _abilities = new LinkedList<AbilityData>();
        _abilities.AddLast(new AbilityData(this, ability));
    }

    public void StartBattle(BattleManager battle)
    {
        _battle = battle;
        _readiness.Set(Payment.Type.PercentOfTotal, _initiative);
        _turn = 0;

        foreach (AbilityData ability in _abilities)
            ability.Register(onTurnStarted);
    }

    public void SetTargettable(bool value)
    {
        Debug.Log(gameObject.name + "has selectable set to" + value + "!");
        selectionBox.enabled = value;
    }

    private void OnMouseDown()
    {
        Debug.Log("Mouse down!");
        if (!selectionBox.enabled)
            return;

    }

    private void Update()
    {
        if (Mouse.current.leftButton.IsActuated())
        {
            //Debug.Log(Time.time+ "actuated!");
            //Debug.Log(Time.time+ "mp" + Mouse.current.position.ReadValue());
            //Vector2 mp = Mouse.current.position.ReadValue();
            //Vector3 mp3 = new Vector3(mp.x, mp.y, 20);
            //Debug.Log(Time.time+ "wp" + Camera.current.ScreenToWorldPoint(mp3));
        
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), Vector2.zero, 100);
            foreach (RaycastHit2D hit in hits)
            {
                Debug.Log("hit!");
        
                if (hit.collider.gameObject == gameObject)
                {
                    Debug.Log("Mouse down!" + hit.collider.gameObject.name);
                }
            }
        }
    }

    public void UpdateReadiness(float speedScale)
    {
        Resource.Value change = _readiness.Pay(Payment.Direction.Credit, Payment.Type.Value, _speed * Time.deltaTime * speedScale);
        if (change.amount != 0)
            _battle.EventResourceChanged(this, Resource.Type.Readiness, change, _readiness.value);

    }

    public void StartTurn()
    {
        ++_turn;
        onTurnStarted?.Invoke();
    }

    public void ChooseRandomAbility()
    {
        LinkedList<AbilityData> activatedAbilities = new LinkedList<AbilityData>();
        foreach (AbilityData ability in _abilities)
        {
            if (ability.isPayable)
                activatedAbilities.AddLast(ability);
        }

        int i = UnityEngine.Random.Range(0, activatedAbilities.Count - 1);
        _battle.ChooseRandomTarget(activatedAbilities.ElementAt(i));
    }

    private void OnDestroy()
    {
        Debug.LogError(gameObject.name + " was destroyed prematurely!");
    }
}
