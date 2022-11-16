using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ControllerType
{
    page,
    instance
}


public interface IController : IDisposable
{
    IModel GetModel();
    ViewBase GetView();
}

public abstract class ControllerBase : IController
{
    static Action<IController,string, string, EventArgs> RedirectToAction;

    protected ControllerType ControllerType = ControllerType.instance;

    protected ControllerBase(ControllerType controllerType)
    {
        ControllerType = controllerType;
        RedirectToAction += OnRedirectToAction;
    }
    protected void Redirect(string actionName, EventArgs data = null)
    {
        RedirectToAction(this, actionName, null, data);
    }
    protected void Redirect(string actionName, string controllerName, EventArgs data =null)
    {
        RedirectToAction(this, actionName, controllerName, data);
    }
    void OnRedirectToAction(IController source, string actionName, string controllerName, EventArgs data)
    {
        if (controllerName == null)
        {
            OnActionRedirected(source, actionName, data);
        }
        else
        {
            if (controllerName == GetType().ToString())
                OnActionRedirected(source, actionName, data);
        }
    }


    public abstract IModel GetModel();
    public abstract ViewBase GetView();
    public abstract void Dispose();
    protected virtual void OnActionRedirected(IController source, string actionName, EventArgs data) { }
}

public class Controller<V, M> : ControllerBase where V : ViewBase where M : IModel
{
    public static M Box(object model) { return (M)model; }
    V pageView{get {return ViewManager.GetPageView<V>(); }}
    V _instanceView;
    V instanceView
    {
        get { 
            if (_instanceView == null) 
                _instanceView = ViewManager.Instance.CreateInstanceView<V>(); 
            return _instanceView; 
        }
    }
    V view;

    public V View
    {
        get
        {
            if (view == null)
            {
                switch (ControllerType)
                {
                    case ControllerType.page:
                        view = pageView;
                        break;
                    case ControllerType.instance:
                        view = instanceView;
                        break;
                    default:
                        view = pageView;
                        break;
                }
            }
            return view;
        }
        private set
        {
            view = value;
        }
    }
    protected M Model { get; private set; }

    public Controller(ControllerType controllerType, M model, V view = null):base(controllerType)
    {
        Model = model;
        View = view;

        View.Init(this);
        OnCreate();

        if (ControllerType == ControllerType.page)
            View.Hide();
    }

    public override void Dispose()
    {
        OnDestroy();
        Model.Dispose();
    }

    protected virtual void OnCreate() { }
    protected virtual void OnDestroy() { }

    public override sealed IModel GetModel()
    {
        return Model;
    }

    public override sealed ViewBase GetView()
    {
        return View;
    }

}
