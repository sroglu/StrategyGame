using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UnitController : Controller<UnitView, UnitModel>
{
    public UnityAction<Vector2Int> OnPlaced;

    public Vector2Int PositionByUnit { get; private set; }
    public bool SnapToPointer { get; private set; }

    public Vector2Int SizeByUnit { get { return Model.CurrentData.dimensions; } }
    public Vector2 SizeByPixel { get { return View.rectTransform.sizeDelta; } private set { View.rectTransform.sizeDelta=value; } }


    public UnitController(UnitModel model) : base(ControllerType.instance, model)
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
    protected void GameViewConfig()
    {
        View.transform.parent = GameManager.Instance.CurrentPage.GetView().transform;
        View.rectTransform.anchorMin = Vector2.zero;
        View.rectTransform.anchorMax = Vector2.zero;
    }
    public void PlaceTo(RectTransform parent,Vector2Int positionByUnit, Vector2 position)
    {
        PlaceTo(parent, positionByUnit, position, Vector2.zero, Vector2.zero);
    }
    public void PlaceTo(RectTransform parent, Vector2Int positionByUnit, Vector2 position,Vector2 anchoredMin,Vector2 anchoredMax)
    {
        View.rectTransform.parent = parent;
        View.rectTransform.anchorMin = anchoredMin;
        View.rectTransform.anchorMax = anchoredMax;
        View.rectTransform.anchoredPosition = position;
        PositionByUnit = positionByUnit;
        SnapToPointer = false;
        OnPlaced?.Invoke(PositionByUnit);
    }

    public virtual void PerformOperation(Operation op) { }
    public virtual void UnityUpdate(){ }


    protected override void OnDestroy()
    {
        this.snapEndCallback = null;
    }

}
