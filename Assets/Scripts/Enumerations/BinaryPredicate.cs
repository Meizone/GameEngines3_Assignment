using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum BinaryPredicate
{
    EqualTo = (1 << 0),
    LessThan = (1 << 1), 
    GreaterThan = (1 << 2),
}
