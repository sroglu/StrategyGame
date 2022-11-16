using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public interface IView
{
    void Init(IController controller);
    //void AssignModel(IModel model);
    void UpdateView();
    void Hide();
    void Show();
}

[RequireComponent(typeof(RectTransform))]
public abstract class ViewBase : MonoBehaviour, IView, IPointerEnterHandler, IPointerExitHandler
{
    public enum ViewState
    {
        visible,
        invisible
    }
    public enum CustomActionEventType
    {
        hold,
    }

    public UnityAction<ViewState> StateChanged;
    public static UnityAction<CustomActionEventType, InputAction.CallbackContext, GameObject> ActionPerformed;


    RectTransform _rectTransform;
    public RectTransform rectTransform { get { if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>(); return _rectTransform; } }
    public bool IsOpen { get { return gameObject.activeInHierarchy; } }

    bool inAnim = false;
    public bool IsInAnim { get { return inAnim; } }
    public bool IsPointerOn { get; private set; }
    public void SetAnim(bool inAnim) { this.inAnim = inAnim; }


    public Vector2 GetAnchoredPosition(Vector2 anchor)
    {
        //return new Vector2(
        //    ((anchor - rectTransform.pivot).x * rectTransform.rect.width /*+ rectTransform.position.x*/) / Screen.width+rectTransform.position.normalized.x,
        //    ((anchor - rectTransform.pivot).y * rectTransform.rect.height /*+ rectTransform.position.y*/) / Screen.height + rectTransform.position.normalized.y);
        return new Vector2(
            ((anchor - rectTransform.pivot).x * rectTransform.rect.width + rectTransform.position.x) / Screen.width,
            ((anchor - rectTransform.pivot).y * rectTransform.rect.height + rectTransform.position.y) / Screen.height);
    }


    protected virtual void Awake()
    {
        ActionPerformed += (CustomActionEventType actionType, InputAction.CallbackContext evArgs, GameObject targetObj) => { if (IsPointerOn) OnCustomInputAction(actionType, evArgs, targetObj); };
        gameObject.layer = LayerMask.NameToLayer("View");
        OnCreate();
    }

    protected void OnDestroy()
    {
        OnRemove();
        ActionPerformed = null;
    }

    public ViewState State { get { if (gameObject.activeInHierarchy) return ViewState.visible; return ViewState.invisible; } }

    protected virtual void OnCreate() { }
    protected virtual void OnRemove() { }
    protected virtual void OnStateChanged(ViewState state) { }
    protected virtual void OnCustomInputAction(CustomActionEventType actionType, InputAction.CallbackContext evArgs, GameObject targetObj) { }

    //Public Methods
    public virtual void Init(IController controller) { }
    public abstract void UpdateView();



    public void Hide()
    {
        gameObject.SetActive(false); OnStateChanged(State); StateChanged?.Invoke(State);
    }
    public void Show()
    {
        gameObject.SetActive(true); OnStateChanged(State); StateChanged?.Invoke(State);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsPointerOn = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsPointerOn = false;
    }
}


public abstract class View<M> : ViewBase where M : ModelBase
{

    public IController Controller;
    public M Model;

    public bool IsInitiated { get { return Model!=null; } }

    public override sealed void Init(IController controller)
    {
        Controller = controller;

        if(Model==null)
            Model = (M)Controller.GetModel();

        OnInit();
        UpdateView();
    }

    protected virtual void OnInit(){}
}
