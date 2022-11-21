using mehmetsrl.MVC.core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UnitView : View<UnitModel>
{
    #region Accesors
    new UnitController Controller { get { return base.Controller as UnitController; } }

    #endregion

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

    public void Place(RectTransform parent, Vector2 position, Vector2 anchoredMin, Vector2 anchoredMax)
    {
        rectTransform.parent = parent;
        rectTransform.anchorMin = anchoredMin;
        rectTransform.anchorMax = anchoredMax;
        rectTransform.anchoredPosition = position;
    }

    public void FollowCursor()
    {
        rectTransform.anchoredPosition = Pointer.current.position.ReadValue();
    }

}
