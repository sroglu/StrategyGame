using System.Collections;
using System.Collections.Generic;
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

    private void Update()
    {
        Controller.UnityUpdate();
    }

}
