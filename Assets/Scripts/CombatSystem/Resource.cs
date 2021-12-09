using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Resource
{
    public delegate void ValueChangedEventHandler(float value, float percent);
    public event ValueChangedEventHandler onValueChanged;
    public struct Value
    {
        public float amount;
        public float percent;
        public Value(float amount, float percent) { this.amount = amount; this.percent = percent; }
        public static Value operator +(Value a, Value b) => new Value(a.amount + b.amount, a.percent + b.percent);
    }

    public enum Query { Value, Percent }
    public enum Type { Health, Mana, Readiness, Aether }
    [SerializeField] private float _current;
    [SerializeField] private float _min;
    [SerializeField] private float _max;
    public float current { get { return _current; } }
    public float percent { get { return 100.0f * ((_current - _min) / (_max - _min)); } }
    public float range { get { return _max - _min; } }

    public Resource(float min, float max, float current)
    {
        _min = min;
        _max = max;
        _current = current;
    }

    public float Get(Query query)
    {
        return query == 0 ? current : percent;
    }

    public Value Get()
    {
        return new Value(current, percent);
    }

    public bool CanPay(Payment.Direction direction, Payment.Type type, float amount)
    {
        return Evaluate(direction, type, amount) > _min;
    }

    public void Set(Payment.Type type, float amount)
    {
        _current = _min;
        Pay(Payment.Direction.Credit, type, amount);
    }

    /// <summary>
    /// Change the value of the resource by the amount specified.
    /// </summary>
    /// <returns>The amount of change: the new value minus the old value.</returns>
    public Value Pay(Payment.Direction direction, Payment.Type type, float amount)
    {
        Value change = new Value(current, percent);
        _current = Evaluate(direction, type, amount);
        change.amount = _current - change.amount;
        change.percent = percent - change.percent;
        onValueChanged?.Invoke(current, percent);
        return change;
    }

    /// <summary>
    /// Evaluate the amount that the resource would become if the payment were made.
    /// </summary>
    private float Evaluate(Payment.Direction direction, Payment.Type type, float amount)
    {
        if (direction == Payment.Direction.None)
            return _current;
        else if (direction == Payment.Direction.Debit)
            amount *= -1.0f;

        switch (type)
        {
            case Payment.Type.Value:
                return _current + amount;
            case Payment.Type.PercentOfCurrent:
                amount *= 0.01f;
                return _min + amount * percent * range;
            case Payment.Type.PercentOfTotal:
                amount *= 0.01f;
                Debug.Log(amount);
                return _current + amount * (_max - _min);
            default:
                Debug.LogErrorFormat("<color=red>Attempting to process payment of unknown type.</color>");
                return _min - 1;
        }
    }
}
