using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New GameData", menuName = "GameData")]
public class GameData : ScriptableObject, ICloneable
{

    public UnitData[] units;


    public object Clone()
    {
        return MemberwiseClone();
    }
}
