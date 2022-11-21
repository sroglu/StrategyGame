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

        #region Properties
        protected ControllerType ControllerType = ControllerType.instance;

        #endregion
        protected ControllerBase(ControllerType controllerType)
        {
            ControllerType = controllerType;
            RedirectToAction += OnRedirectToAction;
        }

        #region UtilityFunctions    
        /// <summary>
        /// Getter function for model.
        /// </summary>
        /// <returns>Model</returns>
        public abstract IModel GetModel();
        /// <summary>
        /// Getter function for view.
        /// </summary>
        /// <returns>View</returns>
        public abstract ViewBase GetView();
        /// <summary>
        /// Redirect an event to all controllers
        /// If controller has implementation proccess it
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="data">Additional data</param>
        protected void Redirect(string actionName, EventArgs data = null)
        {
            RedirectToAction(this, actionName, null, data);
        }

        /// <summary>
        /// Redirect an event to specified controller
        /// Controller should not be an instance type controller
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Target controller that handle the event</param>
        /// <param name="data">Additional data</param>
        protected void Redirect(string actionName, string controllerName, EventArgs data = null)
        {
            RedirectToAction(this, actionName, controllerName, data);
        }

        /// <summary>
        /// Distributes redirect calls.
        /// If controller name is null it triggers for all controllers
        /// </summary>
        /// <param name="source">Source controller that redirects event</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Target controller that handle the event</param>
        /// <param name="data">Additional data</param>
        void OnRedirectToAction(IController source, string actionName, string controllerName, EventArgs data)
        {
            if (controllerName == null || controllerName == GetType().ToString())
            {
                OnActionRedirected(source, actionName, data);
            }
        }
        #endregion

        #region Overridables
        /// <summary>
        /// Handle function for redirected events
        /// All controllers implement the events they responsible.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="actionName"></param>
        /// <param name="data"></param>
        protected virtual void OnActionRedirected(IController source, string actionName, EventArgs data) { }
        public abstract void Dispose();
        #endregion
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