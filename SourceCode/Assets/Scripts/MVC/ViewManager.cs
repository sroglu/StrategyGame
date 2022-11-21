using mehmetsrl.MVC.core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace mehmetsrl.MVC
{
    /// <summary>
    /// Class that links the views to MVC core
    /// It links predefined page views and initializes instance views
    /// Creates custom UI events
    /// </summary>
    public class ViewManager : MonoBehaviour
    {
        //Singleton
        private static ViewManager _instance;
        public static ViewManager Instance
        {
            get { return _instance; }
            private set
            {
                if (_instance == null)
                    _instance = value;
                else
                    Destroy(value);
            }
        }
        #region EditorBindings
        [Header("Views")]
        [SerializeField]
        private ViewBase[] pageViews;
        private ViewBase currentPageView;

        [Header("UI Prefabs")]
        [SerializeField]
        private ViewBase[] instancePrefabs;

        [Header("DOM Actions")]
        [SerializeField]
        public UnityEvent<ViewBase.CustomActionEventType, InputAction.CallbackContext, GameObject> OnInputOnView;
        #endregion

        #region Properties
        private readonly Stack<ViewBase> history = new Stack<ViewBase>();
        private Inputs Inputs;
        #endregion

        #region UI Events
        /// <summary>
        /// Custom Actions
        /// </summary>
        List<RaycastResult> raycastResult = new List<RaycastResult>();
        private void OnViewCustomAction(ViewBase.CustomActionEventType viewAction, InputAction.CallbackContext evArgs)
        {
            PointerEventData pData = new PointerEventData(EventSystem.current)
            {
                position = Pointer.current.position.ReadValue(),
                pointerId = -1,
            };
            raycastResult.Clear();
            EventSystem.current.RaycastAll(pData, raycastResult);

            if (raycastResult.Count > 0)
            {
                ViewBase.ActionPerformed(viewAction, evArgs, raycastResult[0].gameObject);
                OnInputOnView?.Invoke(viewAction, evArgs, raycastResult[0].gameObject);
            }
        }

        #endregion

        private void Awake()
        {
            Instance = this;

            Inputs = new Inputs();
            Inputs.UI.Hold.performed += (evArgs) => OnViewCustomAction(ViewBase.CustomActionEventType.hold, evArgs);
        }
        void OnEnable()
        {
            Inputs.Enable();
        }
        void OnDisable()
        {
            Inputs.Disable();
        }

        private void OnDestroy()
        {
            Inputs.Dispose();
        }

        #region ViewMethods
        /// <summary>
        /// Creates new view for required MVC component
        /// </summary>
        /// <typeparam name="T"> View class </typeparam>
        /// <returns></returns>
        public T CreateInstanceView<T>() where T : ViewBase
        {
            if (typeof(T) == typeof(EmptyView))
                return new GameObject().AddComponent(typeof(EmptyView)) as T;

            foreach (var instancePrefab in instancePrefabs)
            {
                if (instancePrefab is T tInstancePrefab)
                {
                    return GameObject.Instantiate(tInstancePrefab);
                }
            }
            return null;
        }
        /// <summary>
        /// Returns already designed page views
        /// </summary>
        /// <typeparam name="T"> View class </typeparam>
        /// <returns></returns>
        public static T GetPageView<T>() where T : ViewBase
        {
            for (int i = 0; i < Instance.pageViews.Length; i++)
            {
                if (Instance.pageViews[i] is T tPageView)
                    return tPageView;
            }
            return null;
        }

        /// <summary>
        /// Facede for showing the designed page views
        /// </summary>
        /// <typeparam name="T"> View class </typeparam>
        /// <param name="remember"></param>
        public static void ShowPageView<T>(bool remember = true) where T : ViewBase
        {
            for (int i = 0; i < Instance.pageViews.Length; i++)
            {
                if (Instance.pageViews[i] is T)
                {
                    if (Instance.currentPageView != null)
                    {
                        if (remember)
                            Instance.history.Push(Instance.currentPageView);
                        Instance.currentPageView.Hide();
                    }
                    Instance.pageViews[i].Show();
                    Instance.currentPageView = Instance.pageViews[i];
                }
            }
        }
        /// <summary>
        /// Shows spesific page view.
        /// </summary>
        /// <param name="view"></param>
        /// <param name="remember"></param>
        public static void ShowPageView(ViewBase view, bool remember = true)
        {
            if (Instance.currentPageView != null)
            {
                if (remember)
                    Instance.history.Push(Instance.currentPageView);
                Instance.currentPageView.Hide();
            }
            view.Show();
            Instance.currentPageView = view;
        }
        /// <summary>
        /// Shows last page view at history
        /// </summary>
        public static void ShowLastPageView()
        {
            if (Instance.history.Count > 0)
            {
                ShowPageView(Instance.history.Pop(), false);
            }
        }

        #endregion
    }

}