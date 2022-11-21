using UnityEngine;
using UnityEngine.UI;
using mehmetsrl.Utils.DesignPatterns;
using mehmetsrl.MVC.core;

/// <summary>
/// Row class for item list
/// </summary>
class RowElement
{
    public Transform row;
    public ProductionSlot[] productionSlots;
}

public class ProductionView : View<ProductionModel>
{
    #region EditorBindings
    [SerializeField]
    GameData gameData;

    [SerializeField]
    ProductionSlot productionSlotPrefab;
    [SerializeField]
    GridLayoutGroup productionSlotRowPrefab;


    [SerializeField]
    ScrollRect productionScrollView;

    #endregion

    #region Properties
    RectTransform contentRectTransform;


    float productionButtonSize=100f;
    RectTransform productionSlotRT;
    int colNum,rowNum;

    //RowElement[] productionSlotRowArr;
    ObjectPool<RowElement> rowElementPool;
    #endregion

    #region Accesors
    new ProductionController Controller { get { return base.Controller as ProductionController; } }
    #endregion

    protected override void OnCreate()
    {
        Model = new ProductionModel(gameData);
        ProcessViewElements();
    }
    private void Start() { CalculateRowsAndColomns(); }

    #region UtilityFunctions

    /// <summary>
    /// Get UI element components
    /// </summary>
    void ProcessViewElements()
    {
        contentRectTransform = productionScrollView.content.GetComponent<RectTransform>();
    }

    /// <summary>
    /// Calculates how many rows and colomn needed.
    /// </summary>
    void CalculateRowsAndColomns()
    {
        RectTransform productionSlotRT = productionSlotPrefab.GetComponent<RectTransform>();

        colNum = (int)(contentRectTransform.rect.width / productionSlotRT.rect.width);

        rowNum = (int)(contentRectTransform.rect.height / productionSlotRT.rect.height);

        rowElementPool = new ObjectPool<RowElement>(CreateRow, OnGetRowElement, OnReleaseRowElement, OnDestroyRowElement, true, rowNum, rowNum * 2);

        for (int i = 0; i < rowNum; i++)
        {
            rowElementPool.Get();
        }
        contentRectTransform.GetComponent<ScrollContent>().StartScrollContent();
    }

    #region RowSlotOperations
    /// <summary>
    /// Action for slot item click.
    /// Initializes new unit through controller.
    /// </summary>
    /// <param name="unitData"></param>
    private void OnRowElementClicked(UnitData unitData)
    {
        Controller.InitUnit(unitData);
    }
    private void OnDestroyRowElement(RowElement rowElement) { }
    /// <summary>
    /// Called when object will go back to the object pool.
    /// </summary>
    /// <param name="rowElement"></param>
    private void OnReleaseRowElement(RowElement rowElement)
    {
        foreach (var productionSlot in rowElement.productionSlots)
        {
            productionSlot.Clear();
        }
    }
    int unitIndex = 0;
    /// <summary>
    /// Function when row element comes back from object pool.
    /// Image and action variables are assgned for each row slot.
    /// To list all available elements, unitIndex variable is needed.
    /// </summary>
    /// <param name="rowElement">Row</param>
    private void OnGetRowElement(RowElement rowElement)
    {
        foreach (var productionSlot in rowElement.productionSlots)
        {
            productionSlot.index = unitIndex;
            UnitData unitData = Controller.BuildingUnits[unitIndex];
            productionSlot.image.sprite = unitData.propertyData.image;
            productionSlot.OnClick += () => OnRowElementClicked(unitData);

            //For next iteration update the unit idex.
            unitIndex++;
            if (unitIndex >= Controller.BuildingUnits.Length)
                unitIndex = 0;
        }
    }
    /// <summary>
    /// Row element creation method.
    /// Instantiates new row with RowElement class.
    /// </summary>
    /// <returns>Row element</returns>
    private RowElement CreateRow()
    {
        RowElement rowElement= new RowElement();
        rowElement.row = GameObject.Instantiate(productionSlotRowPrefab, contentRectTransform).transform;
        rowElement.productionSlots = new ProductionSlot[colNum];

        for (int i = 0; i < rowElement.productionSlots.Length; i++)
        {
            rowElement.productionSlots[i] = GameObject.Instantiate(productionSlotPrefab, rowElement.row.transform);
        }

        return rowElement;
    }
    #endregion

    #region EditorMethods
    /// <summary>
    /// Called when new row required by infinite scroll view.
    /// </summary>
    public void OnRequireNewRow()
    {
        rowElementPool.Get();
    }
    /// <summary>
    /// Triggered when items replaced in the infinite view.
    /// </summary>
    /// <param name="trans">Replaced transform</param>
    public void OnReplacement(Transform trans)
    {
        //If row number cannot hold whole production units add new rows
        if (productionScrollView.content.childCount * colNum < Controller.BuildingUnits.Length)
        {
            rowElementPool.Get();
            contentRectTransform.GetComponent<ScrollContent>().StartScrollContent();
        }
    }
    #endregion

    #endregion

    public override void UpdateView() { }
}
