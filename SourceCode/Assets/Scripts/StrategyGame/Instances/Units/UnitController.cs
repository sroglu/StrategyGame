using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UnitController : Controller<UnitView, UnitModel>
{
    public Vector2Int PositionByUnit { get; private set; }
    public bool Placed { get; private set; }

    public Vector2Int SizeByUnit { get { return Model.CurrentData.dimensions; } }
    public Vector2 SizeByPixel { get { return View.rectTransform.sizeDelta; } private set { View.rectTransform.sizeDelta=value; } }

    public bool HasOperation(Operation operation)
    {
        foreach (var op in Model.CurrentData.propertyData.operations)
        {
            if(operation.command==op.command)
                return true;
        }
        return false;
    }
    public bool HasEffect(Effect effect)
    {
        foreach (var ef in Model.CurrentData.propertyData.effects)
        {
            if (effect.effect == ef.effect)
                return true;
        }
        return false;
    }

    public UnitController(UnitModel model,UnitView view=null) : base(ControllerType.instance, model, view)
    {
    }
    public void SetPixelSize(Vector2 size)
    {
        SizeByPixel = size;
    }
    internal void ShowInfoOn(InfoController infoController)
    {
        infoController.Show(Model.CurrentData.propertyData);
    }
    public void PlaceTo(RectTransform parent,Vector2Int positionByUnit, Vector2 position)
    {
        PlaceTo(parent, positionByUnit, position, Vector2.zero, Vector2.zero);
    }
    public void PlaceTo(RectTransform parent, Vector2Int positionByUnit, Vector2 position,Vector2 anchoredMin,Vector2 anchoredMax)
    {
        View.Place(parent, position, anchoredMin, anchoredMax);
        PositionByUnit = positionByUnit;
        Placed = true;
    }

    public virtual void PerformDefaultOperation()
    {
        Events.OperationEvent e = GetDefaultOperation();
        if (e != null) {
            PerformOperation(e);
        }
    }
    bool disposed=false;
    public void Abandon()
    {
        disposed = true;
        Dispose();
    }

    public IEnumerator AddUnitToGameBoard(Action callback)
    {
        Events.AddUnitEventArgs e = new Events.AddUnitEventArgs(this);
        Redirect(Constants.Events.AddUnit, Constants.Controllers.GameBoardController, e);
        Placed = false;
        do
        {
            View.FollowCursor();
            yield return new WaitForFixedUpdate();
        }
        while (!Placed && !disposed);
        callback?.Invoke();
    }

    public virtual void PerformOperation(Events.OperationEvent e) {
        if (HasOperation(e.operation))
            Redirect(Constants.Events.PerformOperation, Constants.Controllers.GameBoardController, e);
    }

    public Events.OperationEvent GetOperation(uint index)
    {
        if (index < Model.CurrentData.propertyData.operations.Length)
            return new Events.OperationEvent(this, Model.CurrentData.propertyData.operations[index]);
        return null;
    }
    protected virtual Events.OperationEvent GetDefaultOperation() { return null; }

}
