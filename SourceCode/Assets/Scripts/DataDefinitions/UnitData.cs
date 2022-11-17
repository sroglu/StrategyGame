using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitType
{
    none,
    barrack,
    powerPlant,
    soldier
}

[CreateAssetMenu(fileName = "New Unit", menuName = "Unit")]
public class UnitData : ScriptableObject,ICloneable
{
    public string unitName="New Unit";
    public Vector2Int dimensions = new Vector2Int(1,1);

    public UnitType type;
    public UnitType spawnFrom;
    public UnitData[] spawns;

    public PropertyData propertyData;

    public object Clone()
    {
        return MemberwiseClone();
    }
}
