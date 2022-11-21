using mehmetsrl.MVC.core;
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