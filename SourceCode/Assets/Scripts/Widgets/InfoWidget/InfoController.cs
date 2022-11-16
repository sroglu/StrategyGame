using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoController : Controller<InfoView, InfoModel>
{
    public InfoController(InfoModel model, InfoView view) : base(ControllerType.instance, model, view)
    {
    }

    protected override void OnActionRedirected(IController source, string actionName, EventArgs data)
    {
        if(actionName==Constants.Events.ShowUnitInfo)
        {
            Events.UnitEventArgs unitEventArgs = data as Events.UnitEventArgs;
            if (unitEventArgs != null && unitEventArgs.unitController != null)
            {
                unitEventArgs.unitController.ShowInfoOn(this);
            }
            else
            {
                Show(View.emptyPropertyData);
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
        Redirect(op.operation,new Events.OperationEventArgs(op));
    }
}
