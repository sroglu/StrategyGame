using System;
using UnityEngine.InputSystem;

public class BuildingUnitController : UnitController
{
    public BuildingUnitController(UnitModel model) : base(model)
    {
    }

    Action snapEndCallback;
    public bool SnapToPointer { get; private set; }
    protected override void OnDestroy()
    {
        this.snapEndCallback = null;
    }

    public void SetSnapToPointer(bool snap = true, Action snapEndCallback = null)
    {
        SnapToPointer = snap;

        if (snap)
        {
            this.snapEndCallback = snapEndCallback;
            GameViewConfig();
        }
        else
        {
            this.snapEndCallback?.Invoke();
            this.snapEndCallback = null;
        }
    }

    public override void UnityUpdate()
    {
        if (SnapToPointer)
        {
            View.rectTransform.anchoredPosition = Pointer.current.position.ReadValue();
        }
    }
}
