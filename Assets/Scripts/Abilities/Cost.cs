using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cost : IDescribable
{
    public string description;
    public Resource resource;
    public float amountValue;
    public float amountPercent;

    public string GetDescription() { return description; }
}
