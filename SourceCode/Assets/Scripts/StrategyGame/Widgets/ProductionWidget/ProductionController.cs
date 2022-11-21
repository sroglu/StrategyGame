using System.Collections.Generic;
using mehmetsrl.MVC.core;
public class ProductionController : Controller<ProductionView, ProductionModel>
{

    #region Properties
    UnitData[] buildingUnits;
    #endregion

    #region Accesors
    public bool CanProduceUnit { get; private set; }
    public UnitData[] BuildingUnits
    {
        get
        {
            if (buildingUnits == null)
            {
                List<UnitData> availableUnitList = new List<UnitData>();
                foreach (var unit in Model.CurrentData.units)
                {
                    if (unit.type == UnitType.building)
                    {
                        availableUnitList.Add(unit);
                    }
                }
                buildingUnits = availableUnitList.ToArray();
            }
            return buildingUnits;
        }
    }
    #endregion

    public ProductionController(ProductionModel model, ProductionView view) : base(ControllerType.instance, model, view) { }
    protected override void OnCreate() { CanProduceUnit = true; }

    /// <summary>
    /// Initialize new unit based on unit data user selected on view.
    /// </summary>
    /// <param name="unitData">Unit data</param>
    public void InitUnit(UnitData unitData)
    {
        //Check user alredy initialize a unit.
        if (!CanProduceUnit) return;
        CanProduceUnit = false;

        //Create new unit by instance manager
        var initiatedBuildingUnitController = InstanceManager.Instance.CreateBuilding(unitData);

        //Add unit to gameboard by user inputs
        View.StartCoroutine(initiatedBuildingUnitController.AddUnitToGameBoard((_) => CanProduceUnit = true));
    }
}
