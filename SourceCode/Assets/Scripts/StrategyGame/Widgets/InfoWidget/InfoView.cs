using mehmetsrl.MVC.core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoView : View<InfoModel>
{
    #region EditorBindings
    [SerializeField]
    public PropertyData emptyPropertyData;

    public Text title, description;
    public Image image;
    public Text effectTitle,operationTitle;
    public ScrollRect operationListScroll;
    public ScrollRect effectListScroll;


    [Header("Widget Views")]
    public float listItemAspectRatio = 5f;
    public OperationButton operationButtonPrefab;
    public Text effectTextPrefab;

    #endregion

    #region Accesors
    new InfoController Controller { get { return base.Controller as InfoController; } }

    #endregion

    #region Properties
    GridLayoutGroup opContentGrid;
    RectTransform opContentRT;
    GridLayoutGroup efContentGrid;
    RectTransform efContentRT;
    string effectTitleText = "Effects";
    string operationTitleText = "Operations";    
    
    List<OperationButton> opButtonsList;
    List<Text> efTextList;
    #endregion

    protected override void OnCreate()
    {
        Model = new InfoModel(emptyPropertyData);
        ProcessViewElements();
    }

    /// <summary>
    /// Overrides base function for this specified view.
    /// </summary>
    public override void UpdateView()
    {
        //Model related
        if (Model == null)
        {
            SetEmpty();
            return;
        }

        //Fill view elements
        title.text = Model.CurrentData.title;
        description.text = Model.CurrentData.description;
        if (Model.CurrentData.image)
        {
            image.color = Color.white;
            image.sprite = Model.CurrentData.image;
        }
        else
        {
            image.color = Color.clear;
        }

        FillOperations();
        FillEffects();
    }

    #region UtilityFunctions
    /// <summary>
    /// Get UI element components
    /// </summary>
    void ProcessViewElements()
    {
        opContentGrid = operationListScroll.content.GetComponent<GridLayoutGroup>();
        opContentRT = operationListScroll.content.GetComponent<RectTransform>();
        efContentGrid = effectListScroll.content.GetComponent<GridLayoutGroup>();
        efContentRT = effectListScroll.content.GetComponent<RectTransform>();
    }

    /// <summary>
    /// Fills operation list
    /// </summary>
    void FillOperations()
    {
        opContentGrid.cellSize = new Vector2(opContentRT.rect.width, opContentRT.rect.width / listItemAspectRatio);

        ClearOperationList();

        opButtonsList = new List<OperationButton>();

        foreach (var operation in Model.CurrentData.operations)
        {
            var operationButton = GameObject.Instantiate(operationButtonPrefab, operationListScroll.content);
            operationButton.mButton.onClick.AddListener(() => Controller.PerformOperation(operation));
            operationButton.mButtonText.text = operation.name;

            opButtonsList.Add(operationButton);
        }
        operationTitle.text = opButtonsList.Count > 0 ? operationTitleText : "";
        opContentRT.sizeDelta = new Vector2(0, Model.CurrentData.operations.Length * opContentGrid.cellSize.y);
    }

    /// <summary>
    /// Fills effect list
    /// </summary>
    void FillEffects()
    {
        efContentGrid.cellSize = new Vector2(efContentRT.rect.width, efContentRT.rect.width / listItemAspectRatio);
        ClearEffectList(); 
        efTextList = new List<Text>();

        foreach (var effect in Model.CurrentData.effects)
        {
            var effectText = GameObject.Instantiate(effectTextPrefab, effectListScroll.content);
            effectText.text = effect.name;

            efTextList.Add(effectText);
        }

        effectTitle.text = efTextList.Count > 0 ? effectTitleText : "";
        efContentRT.sizeDelta = new Vector2(0, Model.CurrentData.effects.Length * efContentGrid.cellSize.y);
    }

    /// <summary>
    /// Clear operation list
    /// </summary>
    void ClearOperationList()
    {
        if (opButtonsList != null)
        {
            foreach (var operationButton in opButtonsList)
            {
                operationButton.mButton.onClick.RemoveAllListeners();
                Destroy(operationButton.gameObject);
            }
            opButtonsList.Clear();
        }
    }
    /// <summary>
    /// Clear effect list
    /// </summary>
    void ClearEffectList()
    {
        if (efTextList != null)
        {
            foreach (var efText in efTextList)
            {
                Destroy(efText.gameObject);
            }
            efTextList.Clear();
        }
    }

    /// <summary>
    /// Clear all view elements
    /// </summary>
    void SetEmpty()
    {
        title.text = "";
        description.text = "";
        effectTitle.text = "";
        operationTitle.text = "";
        ClearOperationList();
        ClearEffectList();
    }

    #endregion


}
