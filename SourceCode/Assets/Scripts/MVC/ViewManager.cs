using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Canvas))]
public class ViewManager : MonoBehaviour
{
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

    private readonly Stack<ViewBase> history = new Stack<ViewBase>();

    private Inputs Inputs;
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


    #region ViewMethods
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

    public static T GetPageView<T>() where T : ViewBase
    {
        for (int i = 0; i < Instance.pageViews.Length; i++)
        {
            if (Instance.pageViews[i] is T tPageView)
                return tPageView;
        }
        return null;
    }

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

    public static void ShowLastPageView()
    {
        if (Instance.history.Count > 0)
        {
            ShowPageView(Instance.history.Pop(), false);
        }
    }

    #endregion


    #region BasicUiOps

    /// <summary>
    /// DragDrop
    /// </summary>
    Vector2 prevPointerPos = Vector2.zero;
    public void DragViewPointerDown(ViewBase view)
    {
        prevPointerPos = Pointer.current.position.ReadValue();
    }
    public void DragView(ViewBase view)
    {
        Vector2 pointerPos = Pointer.current.position.ReadValue();
        if (prevPointerPos != Vector2.zero)
            view.transform.position += (Vector3)(pointerPos - prevPointerPos);
        prevPointerPos = pointerPos;
    }


    #endregion



}
