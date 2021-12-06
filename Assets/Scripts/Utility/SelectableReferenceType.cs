using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public class SelectableReferenceType : PropertyAttribute
{
    private Type type;
    public Type Type => type;
    public SelectableReferenceType(Type type)
    {
        this.type = type;
    }
}