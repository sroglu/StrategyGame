using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoController : Controller<InfoView, InfoModel>
{
    public InfoController(InfoModel model, InfoView view) : base(ControllerType.instance, model, view)
    {
    }

    UnitController activeUnit;

    protected override void OnActionRedirected(IController source, string actionName, EventArgs data)
    {
        if(actionName==Constants.Events.ShowUnitInfo)
        {
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

    public void Show(PropertyData infoData)
    {
        Model.Update(infoData);
        View.UpdateView();
    }

    public void PerformOperation(Operation op)
    {
        var e = new Events.OperationEvent(activeUnit,op);
        activeUnit?.PerformOperation(e);
    }
}
