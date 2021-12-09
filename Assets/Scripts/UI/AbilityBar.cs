using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBar : MonoBehaviour
{
    [SerializeField]
    private RectTransform abilityBar;
    [SerializeField]
    private AbilityButton abilityButtonPrefab;
    [SerializeField]
    private Queue<AbilityButton> abPool = new Queue<AbilityButton>();
    private LinkedList<AbilityButton> activeButtons = new LinkedList<AbilityButton>();
    private BattleManager battle;
    public void SetBattle(BattleManager battle)
    {
        if (this.battle != null)
        {
            Deregister();
        }
        this.battle = battle;
        Register();
    }

    private void OnEnable()
    {
        Register();
    }

    private void OnDisable()
    {
        Deregister();
    }

    private void Register()
    {
        if (battle == null)
            return;

        battle.onTurnStarted += OnTurnStarted;
        battle.onTurnEnded += OnTurnEnded;
        battle.onTargettingStarted += OnTargettingStarted;
        battle.onTargettingCancelled += OnTargettingCancelled;
        battle.onAbilityActivated += OnAbilityActivated;
    }

    private void Deregister()
    {
        if (battle == null)
            return;

        battle.onTurnStarted -= OnTurnStarted;
        battle.onTurnEnded -= OnTurnEnded;
        battle.onTargettingStarted -= OnTargettingStarted;
        battle.onTargettingCancelled -= OnTargettingCancelled;
        battle.onAbilityActivated -= OnAbilityActivated;
    }

    private void SetCombatant(Combatant combatant)
    {
        foreach (AbilityButton button in activeButtons)
        {
            ReturnAbilityButton(button);
        }

        if (combatant == null)
            return;

        foreach (AbilityData ability in combatant.Abilities)
        {
            AbilityButton ab =  GetAbilityButton();
            ab.Assign(ability);
        }
    }

    private AbilityButton GetAbilityButton()
    {
        if (abPool.Count < 1)
            abPool.Enqueue(Instantiate(abilityButtonPrefab, abilityBar));

        AbilityButton abilityButton = abPool.Dequeue();
        activeButtons.AddLast(abilityButton);
        abilityButton.gameObject.SetActive(true);
        abilityButton.ForceInactive = false;
        abilityButton.transform.SetAsLastSibling();
        return abilityButton;
    }

    private void ReturnAbilityButton(AbilityButton abilityButton, bool removeFromActiveList = true)
    {
        Debug.Log("ReturnAbilityButton");
        abilityButton.gameObject.SetActive(false);
        abilityButton.ForceInactive = false;
        if (removeFromActiveList)
            activeButtons.Remove(abilityButton);
        abPool.Enqueue(abilityButton);
    }

    private void OnTurnStarted(Combatant combatant, uint turn, uint turnInBattle)
    {
        SetCombatant(combatant);
    }

    private void OnTurnEnded(Combatant combatant, uint turn, uint turnInBattle)
    {
        Debug.Log("AbilityBar.OnTurnEnded");
        SetCombatant(null);
    }

    private void OnTargettingStarted(AbilityData ability)
    {
        foreach (AbilityButton abilityButton in activeButtons)
        {
            abilityButton.ForceInactive = true;
        }
    }

    private void OnTargettingCancelled(AbilityData ability)
    {
        foreach (AbilityButton abilityButton in activeButtons)
        {
            abilityButton.ForceInactive = false;
        }
    }

    private void OnAbilityActivated(AbilityData activatedAbility, Combatant target)
    {
        foreach (AbilityButton abilityButton in activeButtons)
        {
            ReturnAbilityButton(abilityButton, false);
        }
        activeButtons.Clear();
    }
}
