using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingUnitController : UnitController
{
    public BuildingUnitController(UnitModel model) : base(model)
    {
    }

    public override void PerformOperation(Operation op)
    {
        switch (op.operation)
        {
            case Constants.Operations.SpawnSoldier:

                SoldierUnitController soldierController = new SoldierUnitController(new UnitModel(Model.CurrentData.spawns[(int)op.amount]));

                Redirect(Constants.Events.AddUnit, Constants.Controllers.GameBoardController, new Events.AddRandomUnitEventArgs(this));

                break;
        }
    }
}
