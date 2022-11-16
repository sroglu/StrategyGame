using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionController : Controller<ProductionView, ProductionModel>
{

    UnitData[] availableUnits;
    public bool CanProduceUnit { get; private set; }

    public UnitData[] AvailableUnits
    {
        get
        {
            if (availableUnits == null)
            {
                List<UnitData> availableUnitList = new List<UnitData>();
                foreach (var unit in Model.CurrentData.units)
                {
                    if (unit.spawnFrom == UnitType.none)
                    {
                        availableUnitList.Add(unit);
                    }
                }
                availableUnits = availableUnitList.ToArray();
            }
            return availableUnits;
        }
    }


    public ProductionController(ProductionModel model, ProductionView view) : base(ControllerType.instance, model, view)
    {
    }
    protected override void OnCreate()
    {
        CanProduceUnit = true;
    }
    public void InitUnit(UnitData unitData)
    {
        if (!CanProduceUnit) return;

        BuildingUnitController buildingUnitController = new BuildingUnitController(new UnitModel(unitData));

        buildingUnitController.SetSnapToPointer(true, 
            () => CanProduceUnit = !buildingUnitController.SnapToPointer);
        CanProduceUnit = !buildingUnitController.SnapToPointer;

        Redirect(Constants.Events.AddUnit, Constants.Controllers.GameBoardController, new Events.UnitEventArgs(buildingUnitController));
    }


}
