using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Constants for the game
/// </summary>
namespace Constants
{
    public static class Values
    {
        public static readonly Vector2Int InvalidVector2Int = Vector2Int.one * -1;
    }

    public static class Events
    {
        public static readonly List<string> All = new List<string> { 
            AddUnit, 
            ShowUnitInfo, 
            PerformOperation,
        };
        public const string AddUnit = "unit.add";
        public const string ShowUnitInfo = "unit.info";
        public const string PerformOperation = "unit.operation";
    }
    public static class Operations
    {
        public static readonly List<string> All = new List<string> {
            SpawnAgent, 
            Move,
            DealDamage,
        };
        public const string SpawnAgent = "operation.spawn.agent";
        public const string Move = "operation.move";
        public const string DealDamage = "operation.damage";
    }
    public static class Effects
    {
        public static readonly List<string> All = new List<string> {
            BoostAttackDamage,
            BoostAttackSpeed,
            BoostMovementSpeed,
            EngageEnemy,
            DealDamage,
            ProtectiveTower,
            ProtectiveWall,
        };
        public const string BoostAttackDamage = "effect.boost.attack.damage";
        public const string BoostAttackSpeed = "effect.boost.attack.speed";
        public const string BoostMovementSpeed = "effect.boost.movement.speed";
        public const string EngageEnemy = "effect.engage.enemy";
        public const string DealDamage = "effect.damage";
        public const string ProtectiveTower = "effect.tower";
        public const string ProtectiveWall = "effect.wall";
    }
    public static class Controllers
    {
        public static readonly List<string> All = new List<string> {
            GameBoardController,
            InfoController,
            ProductionController,
        };
        public const string GameBoardController = "GameBoardController";
        public const string InfoController = "InfoController";
        public const string ProductionController = "ProductionController";
    }
}