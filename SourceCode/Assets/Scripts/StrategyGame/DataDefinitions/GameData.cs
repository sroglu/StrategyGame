using System;
using UnityEngine;

/// <summary>
/// Game data
/// </summary>
[CreateAssetMenu(fileName = "New GameData", menuName = "GameData")]
public class GameData : ScriptableObject, ICloneable
{

    public UnitData[] units;


    public object Clone()
    {
        return MemberwiseClone();
    }
}
