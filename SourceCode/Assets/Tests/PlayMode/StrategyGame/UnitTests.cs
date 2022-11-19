using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

public class UnitTests
{
    UnitController CreateUnit(UnitData unitData)
    {
        var unitView = new GameObject("UnitView", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).AddComponent<UnitView>();
        return new UnitController(new UnitModel(unitData), unitView);
    }


    [Test]
    public void CreateUnit()
    {
        var unitDataObjs = Resources.LoadAll("Data/Units", typeof(UnitData));

        foreach (var uObj in unitDataObjs)
        {
            var unitData = uObj as UnitData;

            if (unitData == null)
            {
                Assert.Fail();
            }

            if(CreateUnit(unitData)==null)
                Assert.Fail();
        }
        Assert.Pass();
    }


    /*
    Canvas CreateCanvas()
    {
        return new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
    }


    GameBoardController CreateGameboard(ref Canvas canvas, uint size)
    {
        var gameboardData = new GameBoardData();
        gameboardData.tileSize = new Vector2(32, 32);
        gameboardData.playgroundArea = new Vector2(gameboardData.tileSize.x, gameboardData.tileSize.y) * size;
        var gameboardView = new GameObject("Gameboard", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image)).AddComponent<GameBoardView>();

        gameboardView.transform.parent = canvas.transform;
        var gridLayoutGroup = new GameObject("GridLayoutGroup", typeof(RectTransform)).AddComponent<GridLayoutGroup>();
        var grid = new GameObject("Grid", typeof(RectTransform)).AddComponent<Grid>();
        var image = new GameObject("Image", typeof(RectTransform)).AddComponent<Image>();

        gridLayoutGroup.transform.parent=canvas.transform;
        grid.transform.parent=canvas.transform;
        image.transform.parent=canvas.transform;

        gameboardView.SetEditorBindings(
            gameboardData,
            gridLayoutGroup,
            grid,
            image
            );

        return new GameBoardController(gameboardView.Model, gameboardView);
    }



    [UnityTest]
    public IEnumerator PlaceUnitOnGameBoard()
    {
        var canvas = CreateCanvas();

        var unitDataObjs = Resources.LoadAll("Data/Units", typeof(UnitData));

        foreach (var uObj in unitDataObjs)
        {
            var unitData = uObj as UnitData;

            if (unitData == null)
            {
                Assert.Fail();
            }
            else
            {
                var unit = CreateUnit(unitData);
                if (unit == null)
                {
                    Assert.Fail();
                }
                else
                {
                    var gamaboarController = CreateGameboard(ref canvas, 10);
                    gamaboarController.CreateGameBoard();
                    var result = gamaboarController.TryPlaceUnit(unit, Vector2Int.zero);

                    if (!result)
                        Assert.Fail();


                }
            }
        }

        Assert.Pass();

        yield return null;
    }
    */
}