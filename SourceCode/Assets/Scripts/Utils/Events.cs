using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class AddUnitEventArgs : EventArgs
    {
        public enum UnitAddMethod
        {
            Spawn_AtPos,
            Spawn_AtRandomPos,
            Spawn_AtRandomPosAroundUnit,
            SpawnByPositionSelection

        }

        public UnitAddMethod method;
        public UnitController unit;
        public Vector2Int position;
        public AddUnitEventArgs(UnitController unitController, Vector2Int position)
        {
            method = UnitAddMethod.Spawn_AtPos;
            this.unit = unitController;
            this.position = position;
        }

        public AddUnitEventArgs(UnitController unitController)
        {
            this.method = UnitAddMethod.SpawnByPositionSelection;
            this.unit = unitController;
            this.position = Vector2Int.one * -1;
        }
    }
    public class AddRandomUnitEventArgs : AddUnitEventArgs
    {
        public AddRandomUnitEventArgs(UnitController unitController,bool spawnArounUnit=true) : base(unitController)
        {
            this.method = spawnArounUnit ?UnitAddMethod.Spawn_AtRandomPosAroundUnit:UnitAddMethod.Spawn_AtRandomPos;
            this.position = Vector2Int.one * -1;
        }
    }





    public class GetInfoUnitEventArgs : EventArgs
    {
        public UnitController unitController; 
        public GetInfoUnitEventArgs(UnitController unitController)
        {
            this.unitController = unitController;
        }
    }


}
