using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
        Controller.OnClickOnBoard(CalculateRelativePos(eventData.position));
    }
    protected override void OnHover(Vector2 cursorPosition)
    {
        Controller.OnMoveOnBoard(CalculateRelativePos(cursorPosition));
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
