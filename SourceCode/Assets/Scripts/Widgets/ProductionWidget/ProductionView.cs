using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using mehmetsrl.Utils.Pool;
using UnityEngine.Events;

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


    #region InitialSetup
    protected override void OnCreate()
    {
        Model = new ProductionModel(gameData);
        contentRectTransform = productionScrollView.content.GetComponent<RectTransform>();
    }

    private void Start()
    {
        Init();
    }

    void Init()
    {
        RectTransform productionSlotRT = productionSlotPrefab.GetComponent<RectTransform>();

        colNum = (int)(contentRectTransform.rect.width / productionSlotRT.rect.width);

        rowNum = (int)(contentRectTransform.rect.height / productionSlotRT.rect.height);

        rowElementPool = new ObjectPool<RowElement>(CreateRow, OnGetRowElement, OnReleaseRowElement, OnDestroyRowElement, true, rowNum,rowNum*2);

        for (int i = 0; i < rowNum; i++)
        {
            rowElementPool.Get();
        }
        contentRectTransform.GetComponent<ScrollContent>().StartScrollContent();
    }
    #endregion


    #region RowSlotOperations
    private void OnRowElementClicked(UnitData unitData)
    {
        Controller.InitUnit(unitData);
    }

    private void OnDestroyRowElement(RowElement rowElement)
    {

    }

    private void OnReleaseRowElement(RowElement rowElement)
    {
        foreach (var productionSlot in rowElement.productionSlots)
        {
            productionSlot.Clear();
        }
    }
    int unitIndex = 0;
    private void OnGetRowElement(RowElement rowElement)
    {

        foreach (var productionSlot in rowElement.productionSlots)
        {
            productionSlot.index = unitIndex;
            UnitData unitData = Controller.AvailableUnits[unitIndex];
            productionSlot.image.sprite = unitData.propertyData.image;
            productionSlot.OnClick += () => OnRowElementClicked(unitData);
            unitIndex++;
            if (unitIndex >= Controller.AvailableUnits.Length)
                unitIndex = 0;
        }
    }

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



    public void OnRequireNewSlot()
    {
        rowElementPool.Get();
    }
    public void OnReplacement(Transform trans)
    {
        //If row number cannot hold whole production units add new rows
        if (productionScrollView.content.childCount * colNum < Controller.AvailableUnits.Length)
        {
            rowElementPool.Get();
            contentRectTransform.GetComponent<ScrollContent>().StartScrollContent();
        }
    }


    public override void UpdateView()
    {

    }
}
