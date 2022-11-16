using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class GameBoardController : Controller<GameBoardView, GameBoardModel>
{
    UnitController[,] gameBoard;
    uint occupiedTile =0;
    uint TotalTile { get { return (uint)(gameBoard.GetLength(0) * gameBoard.GetLength(1)); } }
    public GameBoardController(GameBoardModel model,GameBoardView view) : base(ControllerType.instance, model, view)
    {
    }
    protected override void OnCreate()
    {
        gameBoard = new UnitController[Model.CurrentData.tileNum.x, Model.CurrentData.tileNum.y];
    }

    #region UtilityFunctions

    //Board Functions
    public Vector2Int GetBoardCoordsFromRelativePosition(Vector2 pointerRelativePosition)
    {
        return new Vector2Int((int)(pointerRelativePosition.x / Model.CurrentData.tileSize.x), (int)(pointerRelativePosition.y / Model.CurrentData.tileSize.y));
    }
    public Vector2 GetBoardRelativePositionFromCoords(Vector2Int coords)
    {
        return new Vector2(coords.x * Model.CurrentData.tileSize.x, -coords.y * Model.CurrentData.tileSize.y);
    }
    public Vector2 GetPxelSizeFromUnitSize(Vector2Int size)
    {
        return new Vector2(size.x * Model.CurrentData.tileSize.x, size.y * Model.CurrentData.tileSize.y);
    }
    bool CheckAreaIsAvailable(Vector2Int pivot, Vector2Int size)
    {
        for (int i = 0; i < size.x; i++)
            for (int j = 0; j < size.y; j++)
                if (GetUnitAtCoord(new Vector2Int(pivot.x + i, pivot.y + j)) != null)
                    return false;
        return true;
    }
    public UnitController GetUnitAtCoord(Vector2Int coord)
    {
        if(coord.x>=0&& coord.x<Model.CurrentData.tileNum.x && coord.y >= 0 && coord.y < Model.CurrentData.tileNum.y)
            return gameBoard[coord.x, coord.y];
        return null;
    }
    void OccupyUnitTilesOnBoard(UnitController unitToPlace, Vector2Int coords)
    {
        for (int i = 0; i < unitToPlace.SizeByUnit.x; i++)
            for (int j = 0; j < unitToPlace.SizeByUnit.y; j++)
            {
                gameBoard[coords.x + i, coords.y + j] = unitToPlace;
                occupiedTile++;
            }

    }
    public bool PlaceUnit(UnitController unit, Vector2Int coords)
    {
        //Debug.Log("Try place unit at "+coords);
        //Calculate upper bounds
        Vector2Int upperBound = unit.SizeByUnit + coords;
        //Check candidate building within the bounds
        if (coords.x >= 0 && coords.y >= 0 &&
            upperBound.x <= Model.CurrentData.tileNum.x && upperBound.y <= Model.CurrentData.tileNum.y)
        {
            //If candidate building does not intersect with other instances place it.
            if (CheckAreaIsAvailable(coords, unit.SizeByUnit))
            {
                OccupyUnitTilesOnBoard(unit, coords);
                unit.PlaceTo(View.rectTransform, GetBoardRelativePositionFromCoords(coords), Vector2.up, Vector2.up);
                return true;
            }
        }

        return false;
    }
    public void SetUnitPixelSize(UnitController unit)
    {
        unit.SetPixelSize(
                    new Vector2(
                        unit.SizeByUnit.x * Model.CurrentData.tileSize.x,
                        unit.SizeByUnit.y * Model.CurrentData.tileSize.y));
    }

    #endregion



    BuildingUnitController currentBuildingUnit;
    public void OnClickOnBoard(Vector2 viewRelativePosition)
    {
        Vector2Int boardCoord = GetBoardCoordsFromRelativePosition(viewRelativePosition);

        if (currentBuildingUnit!=null)
        {
            if (PlaceUnit(currentBuildingUnit, boardCoord))
            {
                currentBuildingUnit.SetSnapToPointer(false);
                OpenInfo(currentBuildingUnit);
                currentBuildingUnit = null;
                View.EndFeedback();
            }
        }
        else
        {
            UnitController unit = GetUnitAtCoord(boardCoord);
            //if (unit != null)
            {
                OpenInfo(unit);
            }
        }
    }

    public void OnMoveOnBoard(Vector2 viewRelativePosition)
    {
        Vector2Int boardCoord = GetBoardCoordsFromRelativePosition(viewRelativePosition);
        if (currentBuildingUnit != null)
        {
            Rect areaOfInterest = new Rect(GetBoardRelativePositionFromCoords(boardCoord), GetPxelSizeFromUnitSize(currentBuildingUnit.SizeByUnit));
            if (CheckAreaIsAvailable(boardCoord, currentBuildingUnit.SizeByUnit))
            {
                View.FeedbackOnArea(areaOfInterest,Color.green);
            }
            else
            {
                View.FeedbackOnArea(areaOfInterest, Color.red);
            }
        }
    }




    void OpenInfo(UnitController unit)
    {
        Redirect(Constants.Events.ShowUnitInfo, Constants.Controllers.InfoController, new Events.UnitEventArgs(unit));
    }

    protected override void OnActionRedirected(IController source, string actionName, EventArgs data)
    {
        if (actionName == Constants.Events.AddUnit)
        {
            Events.UnitEventArgs unitEventArgs = data as Events.UnitEventArgs;
            currentBuildingUnit = unitEventArgs.unitController as BuildingUnitController;
            if (currentBuildingUnit != null)
            {
                SetUnitPixelSize(currentBuildingUnit);
            }
        }
        else if(actionName == Constants.Operations.SpawnSoldier)
        {
            //TODO: Builder tasarlanacak (ViewManager?)
            Events.OperationEventArgs operationEventArgs = data as Events.OperationEventArgs;

            SoldierUnitController soldierController = new SoldierUnitController(new UnitModel(View.soldierData));
            SetUnitPixelSize(soldierController);

            Vector2Int candidateSpawnPoint;
            do
            {
                if (occupiedTile >= TotalTile)
                {
                    Debug.LogError("Lack of space! "+ occupiedTile+ "  "+TotalTile);
                }
                candidateSpawnPoint = new Vector2Int(UnityEngine.Random.Range(0, Model.CurrentData.tileNum.x), UnityEngine.Random.Range(0, Model.CurrentData.tileNum.y));
            }
            while ((occupiedTile < TotalTile) && !PlaceUnit(soldierController, candidateSpawnPoint)) ;


        }
    }


}
