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
    public UnitData soldierData;


    [SerializeField]
    GridLayoutGroup playground;
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
        playgroundRT = playground.GetComponent<RectTransform>();
        feedbackImageRT=feedbackImage.GetComponent<RectTransform>();

        boardData.playgroundArea = rectTransform.rect.size;

        Model = new GameBoardModel(boardData);


        
        playground.cellSize = Model.CurrentData.tileSize;

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

    void FixedUpdate()
    {
        if (Pointer.current.delta.ReadValue().magnitude > 0)
        {
            Controller.OnMoveOnBoard(CalculateRelativePos(Pointer.current.position.ReadValue()));
        }

    }

    Vector2 CalculateRelativePos(Vector2 pointerPos)
    {
        Vector2 relativeClickPos = (Vector2)transform.InverseTransformPoint(pointerPos + new Vector2(rectTransform.rect.width / 2, rectTransform.rect.height / 2));
        relativeClickPos = new Vector2(relativeClickPos.x, rectTransform.rect.height - relativeClickPos.y);
        return relativeClickPos;
    }


    public override void UpdateView()
    {

    }
}
