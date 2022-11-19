
using NUnit.Framework;
using System;
using UnityEngine;

public class MVC
{
    [Test]
    public void CreateEmptyComponent()
    {

        ModelBase model = new EmptyModel(new EmptyData());
        ViewBase view = new GameObject().AddComponent(typeof(EmptyView)) as ViewBase;

        IController controller = new Controller<ViewBase, ModelBase>(ControllerType.instance, model, view);

        Assert.IsNotNull(controller);

    }

}