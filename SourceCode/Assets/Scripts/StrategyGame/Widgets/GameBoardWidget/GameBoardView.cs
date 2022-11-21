using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using mehmetsrl.MVC.core;
public class GameBoardView : View<GameBoardModel>, IPointerClickHandler
{
    #region EditorBindings

    [Header("GameData")]
    [SerializeField]
    GameBoardData boardData;

    [SerializeField]
    GridLayoutGroup gameBoard;
    [SerializeField]
    Grid tilePrefab;
    [SerializeField]
    Image feedbackImage;
    #endregion


    #region Properties
    RectTransform playgroundRT;
    RectTransform feedbackImageRT;
    #endregion

    #region Accesors
    new GameBoardController Controller { get { return base.Controller as GameBoardController; } }

    #endregion

    public void SetEditorBindings(GameBoardData boardData, GridLayoutGroup gameBoard, Grid tilePrefab, Image feedbackImage)
    {
        this.boardData = boardData;
        this.gameBoard = gameBoard;
        this.tilePrefab = tilePrefab;
        this.feedbackImage = feedbackImage;
    }


    protected override void OnCreate()
    {
        //RectTransform of the playground could be needed later.
        playgroundRT = gameBoard.GetComponent<RectTransform>();
        feedbackImageRT=feedbackImage.GetComponent<RectTransform>();

        boardData.playgroundArea = rectTransform.rect.size;

        Model = new GameBoardModel(boardData);


        
        gameBoard.cellSize = Model.CurrentData.tileSize;

        for (int i = 0; i < Model.CurrentData.tileNum.x* Model.CurrentData.tileNum.y; i++)
        {
            GameObject.Instantiate(tilePrefab, playgroundRT);
        }
    }

    public void FeedbackOnArea(Rect area, Color color)
    {
        feedbackImageRT.anchoredPosition = area.position;
        feedbackImageRT.sizeDelta = area.size;

        feedbackImage.color = new Color(color.r, color.g, color.b, 0.5f);
        feedbackImage.gameObject.SetActive(true);
    }

    public void EndFeedback()
    {
        feedbackImage.gameObject.SetActive(false);

    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button==PointerEventData.InputButton.Left)
            Controller.OnClickOnBoard(CalculateRelativePos(eventData.position));
        else if(eventData.button==PointerEventData.InputButton.Right)
            Controller.OnRightClickOnBoard(CalculateRelativePos(eventData.position));
    }

    public Vector2 CalculateRelativePos(Vector2 pointerPos)
    {
        Vector2 relativePos = (Vector2)transform.InverseTransformPoint(pointerPos + new Vector2(rectTransform.rect.width / 2, rectTransform.rect.height / 2));
        relativePos = new Vector2(relativePos.x, rectTransform.rect.height - relativePos.y);
        return relativePos;
    }

    public void AddInstanceToPlayground(RectTransform instanceTransform)
    {
        instanceTransform.parent = InstanceManager.Instance.transform;
    }

    public override void UpdateView()
    {

    }

}
