using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Payment : IDescribable
{
    public enum Direction { Debit = -1, None = 0, Credit = 1 }
    public enum Type { Value, PercentOfCurrent, PercentOfTotal }

    public Resource.Type resource;
    public Direction direction;

    public Type type;

    [SerializeField, Min(0)]
    private float _amount;
    public float amount {
        get { return Mathf.Abs(_amount); }
        set { if (value < 0) direction = (Direction)((int)direction * -1.0f);
            _amount = Mathf.Abs(value); } }

    public string GetDescription()
    {
        Debug.LogFormat("<color=yellow>Cost descriptions not yet implemented.</color>");
        return "";
    }
}
