using System;

namespace mehmetsrl.MVC.core
{
    /// <summary>
    /// Controllers can be a page or an instance.
    /// </summary>
    public enum ControllerType
    {
        page,
        instance
    }

    /// <summary>
    /// Interface for controllers.
    /// A controller should has a model and a view.
    /// </summary>
    public interface IController : IDisposable
    {
        IModel GetModel();
        ViewBase GetView();
    }

    /// <summary>
    /// Base controller class with some common implementations
    /// It also describes functionalities of a controller
    /// </summary>
    public abstract class ControllerBase : IController
    {
        static Action<IController, string, string, EventArgs> RedirectToAction;

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
        protected void Redirect(string actionName, string controllerName, EventArgs data = null)
        {
            RedirectToAction(this, actionName, controllerName, data);
        }
        void OnRedirectToAction(IController source, string actionName, string controllerName, EventArgs data)
        {
            if (controllerName == null || controllerName == GetType().ToString())
            {
                OnActionRedirected(source, actionName, data);
            }
        }


        public abstract IModel GetModel();
        public abstract ViewBase GetView();
        public abstract void Dispose();
        protected virtual void OnActionRedirected(IController source, string actionName, EventArgs data) { }
    }

    /// <summary>
    /// Generic controller class
    /// It implements the relation with view and model
    /// </summary>
    /// <typeparam name="V"> View </typeparam>
    /// <typeparam name="M"> Model </typeparam>
    public class Controller<V, M> : ControllerBase where V : ViewBase where M : IModel
    {
        public static M Box(object model) { return (M)model; }
        V pageView { get { return ViewManager.GetPageView<V>(); } }
        V _instanceView;
        V instanceView
        {
            get
            {
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

        public Controller(ControllerType controllerType, M model, V view = null) : base(controllerType)
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
            View.Dispose();
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
}