using System;
using System.Collections;
using UnityEngine;
using mehmetsrl.MVC.core;

/// <summary>
/// Base controller for units.
/// </summary>
public class UnitController : Controller<UnitView, UnitModel>
{
    #region Accesors
    //Unit placed or not
    public bool Placed { get; private set; }
    //Unit position by board unit
    public Vector2Int PositionByUnit { get; private set; }
    //Unit size by board unit
    public Vector2Int SizeByUnit { get { return Model.CurrentData.dimensions; } }
    //Unit size by pure pixel
    public Vector2 SizeByPixel { get { return View.rectTransform.sizeDelta; } private set { View.rectTransform.sizeDelta=value; } }
    #endregion

    #region Properties
    bool abandoned = false;
    #endregion

    public UnitController(UnitModel model,UnitView view=null) : base(ControllerType.instance, model, view)
    {
    }

    #region UtilityFunctions
    /// <summary>
    /// Checks whether unit has the operation given.
    /// </summary>
    /// <param name="operation">Operation</param>
    /// <returns>Whether or not unit has operation</returns>
    public bool HasOperation(Operation operation)
    {
        foreach (var op in Model.CurrentData.propertyData.operations)
        {
            if(operation.command==op.command)
                return true;
        }
        return false;
    }
    /// <summary>
    /// Checks whether unit has the effect given.
    /// </summary>
    /// <param name="effect">Effect</param>
    /// <returns>Whether or not unit has effect</returns>
    public bool HasEffect(Effect effect)
    {
        foreach (var ef in Model.CurrentData.propertyData.effects)
        {
            if (effect.effect == ef.effect)
                return true;
        }
        return false;
    }
    /// <summary>
    /// Set unit size.
    /// </summary>
    /// <param name="size">Size of the unit</param>
    public void SetPixelSize(Vector2 size) { SizeByPixel = size; }
    internal void ShowInfoOn(InfoController infoController) { infoController.Show(Model.CurrentData.propertyData); }
    /// <summary>
    /// Overload function for placing unit
    /// </summary>
    /// <param name="parent">Parent</param>
    /// <param name="positionByUnit">Gameboard position</param>
    /// <param name="position">Relative position</param>
    public void PlaceTo(Transform parent,Vector2Int positionByUnit, Vector2 position)
    {
        PlaceTo(parent, positionByUnit, position, Vector2.zero, Vector2.zero);
    }
    /// <summary>
    /// Place unit on specified position under specified parent
    /// </summary>
    /// <param name="parent">Parent</param>
    /// <param name="positionByUnit">Gameboard position</param>
    /// <param name="position">Relative position</param>
    /// <param name="anchoredMin">AnchoredMinimum</param>
    /// <param name="anchoredMax">anchoredMaximum</param>
    public void PlaceTo(Transform parent, Vector2Int positionByUnit, Vector2 position,Vector2 anchoredMin,Vector2 anchoredMax)
    {
        View.Place(parent, position, anchoredMin, anchoredMax);
        PositionByUnit = positionByUnit;
        Placed = true;
    }
    /// <summary>
    /// Default operation method
    /// Can be overridden by childs for special cases
    /// </summary>
    public virtual void PerformDefaultOperation()
    {
        Events.OperationEvent e = GetDefaultOperation();
        if (e != null) {
            PerformOperation(e);
        }
    }

    /// <summary>
    /// Trigger when unit failed to be placed.
    /// Unit will be destroyed.
    /// </summary>
    /// <returns>Returns whether the unit is succesfully abondened.</returns>
    public bool Abandon()
    {
        if (Placed)
            return false;
        abandoned = true;
        Dispose();
        return true;
    }

    /// <summary>
    /// Creates and redirects add unit action to gameboard controller for this unit.
    /// </summary>
    /// <param name="callback">Returns unit is placed on gameboard or not when unit add action ended.</param>
    /// <returns></returns>
    public IEnumerator AddUnitToGameBoard(Action<bool> callback)
    {
        Events.AddUnitEventArgs e = new Events.AddUnitEventArgs(this);
        Redirect(Constants.Events.AddUnit, Constants.Controllers.GameBoardController, e);
        Placed = false;
        do
        {
            View.FollowPointer();
            yield return new WaitForFixedUpdate();
        }
        while (!Placed && !abandoned);
        callback?.Invoke(Placed);
    }
    /// <summary>
    /// Redirects operation events to gameboard controller if the unit has given operation.
    /// </summary>
    /// <param name="e"></param>
    public virtual void PerformOperation(Events.OperationEvent e) {
        if (HasOperation(e.operation))
            Redirect(Constants.Events.PerformOperation, Constants.Controllers.GameBoardController, e);
    }
    /// <summary>
    /// Get unit operation by given index
    /// </summary>
    /// <param name="index">Operation array index</param>
    /// <returns>Operation</returns>
    public Events.OperationEvent GetOperation(uint index)
    {
        if (index < Model.CurrentData.propertyData.operations.Length)
            return new Events.OperationEvent(this, Model.CurrentData.propertyData.operations[index]);
        return null;
    }
    #endregion

    #region Overridables
    protected virtual Events.OperationEvent GetDefaultOperation() { return null; }
    #endregion
}
