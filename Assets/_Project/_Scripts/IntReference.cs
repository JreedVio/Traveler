using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IntReference
{
    public int Value;

    public IntReference(int value_)
    {
        Value = value_;
    }
    
    public IntReference()
    {
        Value = 0;
    }
}
