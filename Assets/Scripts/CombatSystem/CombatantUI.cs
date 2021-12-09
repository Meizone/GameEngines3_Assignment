using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatantUI : MonoBehaviour
{
    [SerializeField]
    private SelectionArrow selectionArrow;
    [SerializeField]
    private ResourceBar readinessBar;
    [SerializeField]
    private ResourceBar healthBar;
    [SerializeField]
    private ResourceBar manaBar;
    private Combatant combatant;
    private BattleUI battleUI;
    private Settings settings;

    public void Init(BattleUI battleUI, Settings settings, Combatant combatant)
    {
        SetSettings(settings);
        SetCombatant(combatant);
        SetBattleUI(battleUI);
    }

    public void SetCombatant(Combatant value)
    {
        combatant = value;
        selectionArrow.state = battleUI.DetermineSelectionState(combatant);
    }

    public void SetSettings(Settings value)
    {
        settings = value;

        selectionArrow.activeSelectionColour = settings.activeSelectionColour;
        selectionArrow.inactiveSelectionColour = settings.inactiveSelectionColour;
        selectionArrow.allySelectionColour = settings.allySelectionColour;
        selectionArrow.enemySelectionColour = settings.enemySelectionColour;
        readinessBar.Color = settings.readinessResourceColour;
        healthBar.Color = settings.healthResourceColour;
        manaBar.Color = settings.manaResourceColour;
    }

    public void SetBattleUI(BattleUI value)
    {
        if (battleUI != null)
            Deregister();

        battleUI = value;
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
        battleUI.onTurnStarted += OnTurnStarted;
        battleUI.onTurnEnded += OnTurnEnded;
        battleUI.onTargettingStarted += OnTargettingStarted;
        battleUI.onAbilityActivated += OnAbilityActivated;
        battleUI.onResourceChanged += OnResourceChanged;
    }

    private void Deregister()
    {
        battleUI.onTurnStarted -= OnTurnStarted;
        battleUI.onTurnEnded -= OnTurnEnded;
        battleUI.onTargettingStarted -= OnTargettingStarted;
        battleUI.onAbilityActivated -= OnAbilityActivated;
        battleUI.onResourceChanged -= OnResourceChanged;
    }

    private void OnTurnStarted(Combatant combatant, uint turn, uint turnInBattle)
    {
        if (this.combatant == combatant)
            selectionArrow.state = SelectionArrow.State.Active;
    }

    private void OnTurnEnded(Combatant combatant, uint turn, uint turnInBattle)
    {
        selectionArrow.state = SelectionArrow.State.Inactive;
    }

    private void OnTargettingStarted(AbilityData ability)
    {
        battleUI.DetermineSelectionState(ability.combatant);
    }

    private void OnAbilityActivated(AbilityData activatedAbility, Combatant target)
    {
        selectionArrow.state = SelectionArrow.State.Inactive;
    }

    private void OnResourceChanged(Combatant combatant, Resource.Type resource, Resource.Value change, Resource.Value final)
    {
        switch (resource)
        {
            case Resource.Type.Health:
                healthBar.value = final.percent;
                break;
            case Resource.Type.Mana:
                manaBar.value = final.percent;
                break;
            case Resource.Type.Readiness:
                readinessBar.value = final.percent;
                break;
            case Resource.Type.Aether:
            default:
                break;
        }
    }
}
