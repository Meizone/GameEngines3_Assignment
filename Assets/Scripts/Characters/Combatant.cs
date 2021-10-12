using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combatant : MonoBehaviour
{
    public Alignment alignment;

    // Resources
    public float health;
    public float mana;
    public float readiness;
    public float aether;

    // Stats
    public float speed;
    public float initiative;

    // Abilities
    public AbilitySlot[] abilities;

    private void Start()
    {
        LoadFromSave();
    }

    private void LoadFromSave()
    {
        Debug.LogFormat("Message: <color=red>{0}</color>", "LoadFromSave not yet implemented.");
        health = 100;
        mana = 100;
        readiness = 0;
        aether = 0;
        speed = 50;
        initiative = 50;
    }

    public void StartBattle()
    {
        readiness = initiative;
    }

    public void UseAbility(Ability ability)
    {
        ability.Activate(this);
    }

    public void UpdateReadiness(float speedScale)
    {
        readiness += speed * Time.deltaTime * speedScale;
    }

    public void Activate()
    {
        foreach (AbilitySlot abilitySlot in abilities)
        {
            abilitySlot.Update();
        }
    }
}
