using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constants
{
    public static class Events
    {
        public const string AddUnit = "unit.add";
        public const string ShowUnitInfo = "unit.info";
    }

    public static class Operations
    {
        public const string SpawnSoldier = "operation.spawn.soldier";
        public const string DealDamage = "operation.damage";
    }
    public static class Effects
    {
        public const string BoostAttackDamage = "effect.boost.attack.damage";
        public const string BoostAttackSpeed = "effect.boost.attack.speed";
        public const string BoostMovementSpeed = "effect.boost.movement.speed";
        public const string EngageEnemy = "effect.engage.enemy";
        public const string DealDamage = "effect.damage";


        public const string ProtectiveTower = "protective.tower";
        public const string ProtectiveWall = "protective.wall";
    }

    public static class Controllers
    {
        public const string GameBoardController = "GameBoardController";
        public const string InfoController = "InfoController";
        public const string ProductionController = "ProductionController";
    }
}