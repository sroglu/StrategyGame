using mehmetsrl.MVC.core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UnitView : View<UnitModel>
{
    #region EditorBindings
    [SerializeField]
    Image image;
    #endregion

    protected override void OnCreate()
    {
        if(image==null)
            image=GetComponent<Image>();
    }
    protected override void OnInit()
    {
        image.sprite = Model.CurrentData.propertyData.image;
    }
    public override void UpdateView()
    {
    }
    /// <summary>
    /// Place unit view on specified position under specified parent
    /// </summary>
    /// <param name="parent">Parent</param>
    /// <param name="position">Relative position</param>
    /// <param name="anchoredMin">AnchoredMinimum</param>
    /// <param name="anchoredMax">anchoredMaximum</param>
    public void Place(Transform parent, Vector2 position, Vector2 anchoredMin, Vector2 anchoredMax)
    {
        rectTransform.parent = parent;
        rectTransform.anchorMin = anchoredMin;
        rectTransform.anchorMax = anchoredMax;
        rectTransform.anchoredPosition = position;
    }
    /// <summary>
    /// Simple method for folluwing pointer
    /// </summary>
    public void FollowPointer()
    {
        rectTransform.anchoredPosition = Pointer.current.position.ReadValue();
    }

}
