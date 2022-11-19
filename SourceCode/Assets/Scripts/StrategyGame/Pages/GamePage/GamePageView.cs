using mehmetsrl.Utils.DesignPatterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GamePageView : View<GamePageModel>
{

    #region EditorBindings
    [Header("Widget Views")]
    public GameBoardView gameBoardView;
    public ProductionView productionView;
    public InfoView infoView;

    #endregion


    #region Accesors
    new GamePageController Controller { get { return base.Controller as GamePageController; } }

    #endregion

    public override void UpdateView()
    {

    }
}
