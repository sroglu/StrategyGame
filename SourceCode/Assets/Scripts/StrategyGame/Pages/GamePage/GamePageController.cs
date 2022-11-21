using mehmetsrl.MVC.core;

/// <summary>
/// Game page controller.
/// It consist of 3 mvc components called widgets.
/// </summary>
public class GamePageController : Controller<GamePageView, GamePageModel>
{
    GameBoardController boardWidget;
    ProductionController productionWidget;
    InfoController infoWidget;
    public GamePageController(GamePageModel model) : base(ControllerType.page , model)
    {

    }
    protected override void OnCreate()
    {

        boardWidget = new GameBoardController(View.gameBoardView.Model, View.gameBoardView);
        productionWidget = new ProductionController(View.productionView.Model, View.productionView);
        infoWidget = new InfoController(View.infoView.Model, View.infoView);

        InstanceManager.Instance.ClearAllInstances();
    }

}
