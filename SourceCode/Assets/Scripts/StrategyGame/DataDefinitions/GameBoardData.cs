using System;
using UnityEngine;

/// <summary>
/// Gamgeboard data
/// </summary>
[System.Serializable]
public class GameBoardData : ICloneable
{
    public Vector2 tileSize = new Vector2(32, 32);
    public Vector2 playgroundArea=Vector2.zero;

    public Vector2Int tileNum
    {
        get
        {
            return new Vector2Int(
                (int)(playgroundArea.x / tileSize.x),
                (int)(playgroundArea.y / tileSize.y));
        }
    }
    public object Clone()
    {
        return MemberwiseClone();
    }
}
