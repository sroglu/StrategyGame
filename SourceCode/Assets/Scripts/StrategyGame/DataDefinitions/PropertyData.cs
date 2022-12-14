using System;
using UnityEngine;

[System.Serializable]
public class Operation
{
    public string name;
    public Sprite image;
    public string command;
    public float amount;
}
[System.Serializable]
public class Effect
{
    public string name;
    public string effect;
    public float amount;
}

/// <summary>
/// Property data
/// </summary>
[System.Serializable]
public class PropertyData : ICloneable
{

    public string title,description;
    public UnityEngine.Sprite image;

    public Operation[] operations;
    public Effect[] effects;

    public object Clone()
    {
        return MemberwiseClone();
    }
}
