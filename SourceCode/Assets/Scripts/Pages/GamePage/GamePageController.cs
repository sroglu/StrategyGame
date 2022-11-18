using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePageController : Controller<GamePageView, GamePageModel>
{

    GameBoardController boardController;
    ProductionController productionController;
    InfoController infoController;


    public GamePageController(GamePageModel model) : base(ControllerType.page , model)
    {

    }

    protected override void OnCreate()
    {

        boardController = new GameBoardController(View.gameBoardView.Model, View.gameBoardView);
        productionController = new ProductionController(View.productionView.Model, View.productionView);
        infoController = new InfoController(View.infoView.Model, View.infoView);

        InstanceManager.Instance.ClearAllInstances();

    }


}
