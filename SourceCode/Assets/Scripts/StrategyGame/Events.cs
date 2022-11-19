using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{

    public class UnitEvent : EventArgs
    {
        public UnitController unit;
        public UnitEvent(UnitController unit)
        {
            this.unit = unit;
        }
    }
    public class AddUnitEventArgs : UnitEvent
    {
        public enum UnitAddMethod
        {
            Spawn_AtPos,
            Spawn_AtRandomPos,
            Spawn_AtRandomPosAroundUnit,
            SpawnByPositionSelection

        }

        public UnitAddMethod method;
        public Vector2 position;
        public AddUnitEventArgs(UnitController unit,Vector2 position):base(unit)
        {
            method = UnitAddMethod.Spawn_AtPos;
            this.position = position;
        }

        public AddUnitEventArgs(UnitController unit) : base(unit)
        {
            this.method = UnitAddMethod.SpawnByPositionSelection;
            this.position = Vector2.one * -1;
        }
    }
    public class AddRandomUnitEventArgs : AddUnitEventArgs
    {
        public AddRandomUnitEventArgs(UnitController unit, bool spawnArounUnit=true):base(unit)
        {
            this.method = spawnArounUnit ? UnitAddMethod.Spawn_AtRandomPosAroundUnit : UnitAddMethod.Spawn_AtRandomPos;
        }
    }



    public class OperationEvent : UnitEvent
    {
        public Operation operation;
        //public Vector2 position;

        public OperationEvent(UnitController unit, Operation operation) : base(unit)
        {
            this.operation = operation;
            //this.position = Vector2.one * -1;
        }

        //public OperationEvent(UnitController unit, Operation operation, Vector2 position) : base(unit)
        //{
        //    this.operation = operation;
        //    this.position = position;
        //}
    }

    public class MoveOperationEvent : OperationEvent
    {
        public Vector2 position;

        public MoveOperationEvent(UnitController unit, Operation operation, Vector2 position) : base(unit, operation)
        {
            this.position = position;
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
