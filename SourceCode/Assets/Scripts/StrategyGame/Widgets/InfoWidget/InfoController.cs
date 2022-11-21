using mehmetsrl.MVC.core;
using System;
public class InfoController : Controller<InfoView, InfoModel>
{
    #region Properties
    UnitController activeUnit;
    #endregion
    public InfoController(InfoModel model, InfoView view) : base(ControllerType.instance, model, view) { }

    #region UtilityFunctions

    /// <summary>
    /// Handles responsible events by overriding.
    /// </summary>
    protected override void OnActionRedirected(IController source, string actionName, EventArgs data)
    {
        if(actionName==Constants.Events.ShowUnitInfo)
        {
            //Shows unit propert datas on view.
            Events.GetInfoUnitEventArgs unitEventArgs = data as Events.GetInfoUnitEventArgs;
            if (unitEventArgs != null && unitEventArgs.unitController != null)
            {
                activeUnit = unitEventArgs.unitController;
                activeUnit?.ShowInfoOn(this);
            }
            else
            {
                Show(View.emptyPropertyData);
                activeUnit=null;
            }
        }
    }
    /// <summary>
    /// Shows property data on view.
    /// </summary>
    /// <param name="infoData">Property data</param>
    public void Show(PropertyData infoData)
    {
        Model.Update(infoData);
        View.UpdateView();
    }

    /// <summary>
    /// Perform operation of active unit.
    /// </summary>
    /// <param name="op"></param>
    public void PerformOperation(Operation op)
    {
        if(activeUnit==null) return;
        var e = new Events.OperationEvent(activeUnit,op);
        activeUnit?.PerformOperation(e);
    }
    #endregion
}
