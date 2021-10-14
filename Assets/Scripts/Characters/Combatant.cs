using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Combatant : MonoBehaviour
{
    public delegate void TurnStartedEvent();
    private event TurnStartedEvent OnTurnStarted;

    private uint _faction;
    public uint faction { get { return _faction; } }

    // Resources
    [SerializeField] private Resource _health;
    [SerializeField] private Resource _mana;
    [SerializeField] private Resource _readiness;
    [SerializeField] private Resource _aether;
    public float health { get { return _health.current; } }
    public float mana { get { return _mana.current; } }
    public float readiness { get { return _readiness.current; } }
    public float aether { get { return _aether.current; } }

    // Stats
    private float _speed;
    private float _initiative;

    // Abilities
    private AbilityData[] _abilities;

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
            Debug.Log(changes[(int)cost.resource].amount);
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

    private void Start()
    {
        _health = new Resource(0, 100, 100);
        _mana = new Resource(0, 100, 100);
        _readiness = new Resource(0, 100, 0);
        _aether = new Resource(0, 100, 0);
        LoadFromSave();
    }

    private void LoadFromSave()
    {
        Debug.LogFormat("Message: <color=red>{0}</color>", "LoadFromSave not yet implemented.");
        _health.Set(Payment.Type.PercentOfTotal, 100.0f);
        _mana.Set(Payment.Type.PercentOfTotal, 75.0f);
        _readiness.Set(Payment.Type.PercentOfTotal, 50.0f);
        Debug.Log("aether");
        _aether.Set(Payment.Type.PercentOfTotal, 25.0f);
        _speed = 50;
        _initiative = 50;
        _turn = 0;
    }

    public void StartBattle(BattleManager battle)
    {
        this._battle = battle;
        _readiness.Set(Payment.Type.PercentOfTotal, 50.0f);
        _turn = 0;

        foreach (AbilityData ability in _abilities)
            ability.Register(OnTurnStarted);
    }

    public void UpdateReadiness(float speedScale)
    {
        _readiness.Pay(Payment.Direction.Credit, Payment.Type.Value, _speed * Time.deltaTime * speedScale);
    }

    public void StartTurn()
    {
        ++_turn;
        OnTurnStarted?.Invoke();
    }
}
