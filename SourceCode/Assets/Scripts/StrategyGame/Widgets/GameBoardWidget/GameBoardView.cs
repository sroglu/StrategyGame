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

    protected override void OnCreate()
    {
        boardData.playgroundArea = rectTransform.rect.size;
        Model = new GameBoardModel(boardData);
        ProcessViewElements();
    }
    public override void UpdateView() { }


    #region EventHandlers

    /// <summary>
    /// Handles click events on view.
    /// </summary>
    /// <param name="eventData"> Pointer event of Unity</param>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            Controller.OnClickOnBoard(CalculateRelativePos(eventData.position));
        else if (eventData.button == PointerEventData.InputButton.Right)
            Controller.OnRightClickOnBoard(CalculateRelativePos(eventData.position));
    }
    #endregion

    #region UtilityFunctions

    /// <summary>
    /// Claculates relative position by given viewport position.
    /// </summary>
    /// <param name="pointerPos">Ponter position</param>
    /// <returns>View relative position</returns>
    public Vector2 CalculateRelativePos(Vector2 pointerPos)
    {
        Vector2 relativePos = (Vector2)transform.InverseTransformPoint(pointerPos + new Vector2(rectTransform.rect.width / 2, rectTransform.rect.height / 2));
        relativePos = new Vector2(relativePos.x, rectTransform.rect.height - relativePos.y);
        return relativePos;
    }

    /// <summary>
    /// Calculates UI object sizes and instantiate tiles.
    /// </summary>
    void ProcessViewElements()
    {
        playgroundRT = gameBoard.GetComponent<RectTransform>();
        feedbackImageRT = feedbackImage.GetComponent<RectTransform>();

        gameBoard.cellSize = Model.CurrentData.tileSize;

        for (int i = 0; i < Model.CurrentData.tileNum.x * Model.CurrentData.tileNum.y; i++)
        {
            GameObject.Instantiate(tilePrefab, playgroundRT);
        }
    }

    /// <summary>
    /// Shows feedback by using unity UI elements.
    /// </summary>
    /// <param name="area">Feedback area</param>
    /// <param name="color">Color of the feedback area</param>
    public void FeedbackOnArea(Rect area, Color color)
    {
        feedbackImageRT.anchoredPosition = area.position;
        feedbackImageRT.sizeDelta = area.size;

        feedbackImage.color = new Color(color.r, color.g, color.b, 0.5f);
        feedbackImage.gameObject.SetActive(true);
    }

    /// <summary>
    /// Finilize feedback
    /// </summary>
    public void EndFeedback()
    {
        feedbackImage.gameObject.SetActive(false);
    }
    #endregion


}
