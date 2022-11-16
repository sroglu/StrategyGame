using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyView : ViewBase
{

    protected override void OnCreate()
    {
        gameObject.name = "Empty";
    }
    public override sealed void UpdateView()
    {

    }
}