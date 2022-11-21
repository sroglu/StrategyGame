using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace mehmetsrl.MVC.core
{
    /// <summary>
    /// Interface for views
    /// A view should initialized with a controller.
    /// A view should have update method.
    /// A view should have hide/show methods.
    /// </summary>
    public interface IView
    {
        void Init(IController controller);
        void UpdateView();
        void Hide();
        void Show();
    }

    /// <summary>
    /// Base class for views with some common implementations.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public abstract class ViewBase : MonoBehaviour, IView, IDisposable, IPointerEnterHandler, IPointerExitHandler
    {
        #region Definitions
        public enum ViewState
        {
            visible,
            invisible
        }
        public enum CustomActionEventType
        {
            hold,
        }
        #endregion

        #region Properties
        bool inAnim = false;
        RectTransform _rectTransform;
        #endregion

        #region Accesors
        public ViewState State { get { if (gameObject.activeInHierarchy) return ViewState.visible; return ViewState.invisible; } }
        public RectTransform rectTransform { get { if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>(); return _rectTransform; } }
        public bool IsInAnim { get { return inAnim; } }
        public bool IsPointerOn { get; private set; }
        public bool IsOpen { get { return gameObject.activeInHierarchy; } }
        #endregion

        #region UtilityFunctions
        /// <summary>
        /// Makes the view invisible.
        /// </summary>
        public void Hide() { gameObject.SetActive(false); OnStateChanged(State); StateChanged?.Invoke(State); }
        /// <summary>
        /// Makes the view visible.
        /// </summary>
        public void Show() { gameObject.SetActive(true); OnStateChanged(State); StateChanged?.Invoke(State); }
        /// <summary>
        /// Handles the pointer events for OnHover template function
        /// </summary>
        public void OnPointerEnter(PointerEventData eventData) { IsPointerOn = true; }
        /// <summary>
        /// Handles the pointer events for OnHover template function
        /// </summary>
        public void OnPointerExit(PointerEventData eventData) { IsPointerOn = false; }
        /// <summary>
        /// Dentroy the instance.
        /// </summary>
        public void DestroyInstance() { Destroy(gameObject); }
        #endregion

        /// <summary>
        /// Event pointer for state changes.
        /// </summary>
        public UnityAction<ViewState> StateChanged;
        /// <summary>
        /// Custom action pointer for custom events
        /// </summary>
        public static UnityAction<CustomActionEventType, InputAction.CallbackContext, GameObject> ActionPerformed;

        protected virtual void Awake()
        {
            ActionPerformed += (CustomActionEventType actionType, InputAction.CallbackContext evArgs, GameObject targetObj) => { if (IsPointerOn) OnCustomInputAction(actionType, evArgs, targetObj); };
            gameObject.layer = LayerMask.NameToLayer("View");
            OnCreate();
        }
        
        void FixedUpdate()
        {
            if (IsPointerOn)
            {
                OnHover(Pointer.current.position.ReadValue());
            }
        }

        protected void OnDestroy()
        {
            OnRemove();
            ActionPerformed = null;
        }
        /// <summary>
        /// Dispose method destroys the instance.
        /// </summary>
        public void Dispose() { DestroyInstance(); }

        /// <summary>
        /// Template functions for child classes
        /// These classes are trigerred in base class.
        /// Implementations rely on the childs.
        /// </summary>
        #region TemplateFunctions
        protected virtual void OnCreate() { }
        protected virtual void OnRemove() { }
        protected virtual void OnDestroyInstance() { }
        protected virtual void OnStateChanged(ViewState state) { }
        protected virtual void OnHover(Vector2 cursorPosition) { }
        protected virtual void OnCustomInputAction(CustomActionEventType actionType, InputAction.CallbackContext evArgs, GameObject targetObj) { }

        //Public Methods
        public virtual void Init(IController controller) { }
        public abstract void UpdateView();
        #endregion

    }

    /// <summary>
    /// Generic view class
    /// It implements the relation with controller and model
    /// </summary>
    /// <typeparam name="M"> Model </typeparam>
    public abstract class View<M> : ViewBase where M : ModelBase
    {

        public IController Controller;
        public M Model;

        public bool IsInitiated { get { return Model != null; } }

        public override sealed void Init(IController controller)
        {
            Controller = controller;

            if (Model == null)
                Model = (M)Controller.GetModel();

            OnInit();
            UpdateView();
        }

        protected virtual void OnInit() { }

        protected sealed override void OnDestroyInstance()
        {
            Controller.Dispose();
        }
    }
}