using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combatant : MonoBehaviour
{
    public enum Alignment { Allies, Enemies, Neutral }
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

    private ref float GetResource(Resource resource)
    {
        switch (resource)
        {
            case Resource.Health:
                return ref health;
            case Resource.Mana:
                return ref mana;
            case Resource.Aether:
                return ref aether;
            case Resource.Readiness:
            default:
                return ref readiness;
        }
    }

    public bool CanPay(Cost[] costs)
    {
        foreach (Cost cost in costs)
            if (GetResource(cost.resource) < cost.amountValue)
                return false;

        return true;
    }

    public bool Pay(Cost[] costs)
    {
        if (!CanPay(costs))
            return false;

        foreach (Cost cost in costs)
            Pay(cost);

        return true;
    }
    public bool CanPay(Cost cost)
    {
        return GetResource(cost.resource) >= cost.amountValue;
    }

    public bool Pay(Cost cost)
    {
        return Pay(ref GetResource(cost.resource), cost.amountValue, cost.amountPercent);
    }

    private bool Pay(ref float resource, float amountValue, float amountPercent)
    {
        if (resource < amountValue)
            return false;

        resource -= amountValue;
        resource *= amountPercent;
        
        return true;
    }

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
            abilitySlot.Cooldown();
        }
    }
}
