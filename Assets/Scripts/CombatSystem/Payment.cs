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

    public string Description { get { Debug.LogFormat("<color=yellow>Descriptions have not yet been implemented for Payments.</color>"); return ""; } }

    public Payment(Resource.Type resource, Direction direction, Type type, float amount)
    {
        this.resource = resource;
        this.direction = direction;
        if (amount < 0)
            this.direction = (Direction)((int)direction * -1.0f);
        this.type = type;
        _amount = Mathf.Abs(amount);
    }

    public Payment(Resource.Type resource, float amount) : this(resource, Direction.Debit, Type.Value, amount) { }
}
