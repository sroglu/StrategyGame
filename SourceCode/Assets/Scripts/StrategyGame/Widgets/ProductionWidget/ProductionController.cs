using System.Collections.Generic;
using mehmetsrl.MVC.core;
public class ProductionController : Controller<ProductionView, ProductionModel>
{

    UnitData[] availableUnits;
    public bool CanProduceUnit { get; private set; }

    public UnitData[] BuildableUnits
    {
        get
        {
            if (availableUnits == null)
            {
                List<UnitData> availableUnitList = new List<UnitData>();
                foreach (var unit in Model.CurrentData.units)
                {
                    if (unit.type == UnitType.building)
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
        CanProduceUnit = false;

        var initiatedBuildingUnitController = InstanceManager.Instance.CreateBuilding(unitData);

        View.StartCoroutine(initiatedBuildingUnitController.AddUnitToGameBoard(() => CanProduceUnit = true));
    }
}
