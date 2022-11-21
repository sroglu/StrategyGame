using mehmetsrl.MVC.core;
using UnityEngine;

public class GamePageView : View<GamePageModel>
{
    #region EditorBindings
    [Header("Widget Views")]
    public GameBoardView gameBoardView;
    public ProductionView productionView;
    public InfoView infoView;
    #endregion

    public override void UpdateView() { }
}
