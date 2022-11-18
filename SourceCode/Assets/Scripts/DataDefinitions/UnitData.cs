using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    none,
    building,
    agent
}

[CreateAssetMenu(fileName = "New Unit", menuName = "Unit")]
public class UnitData : ScriptableObject,ICloneable
{
    public string unitName="New Unit";
    public Vector2Int dimensions = new Vector2Int(1,1);

    public UnitType type;
    public UnitData[] spawns;

    public PropertyData propertyData;

    public object Clone()
    {
        return MemberwiseClone();
    }
}
