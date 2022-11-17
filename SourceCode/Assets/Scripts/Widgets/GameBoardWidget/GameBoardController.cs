using System;
using UnityEngine;
using mehmetsrl.Algorithms.Graph;
using System.Collections.Generic;
using mehmetsrl.Algorithms.DataStructures;
using System.Collections;

public class GameBoardController : Controller<GameBoardView, GameBoardModel>
{
    Tile_Vector2Int<UnitController>[,] gameBoard;
    uint occupiedTile =0;
    uint TotalTile { get { return (uint)(gameBoard.GetLength(0) * gameBoard.GetLength(1)); } }
    public GameBoardController(GameBoardModel model,GameBoardView view) : base(ControllerType.instance, model, view)
    {
    }
    protected override void OnCreate()
    {
        gameBoard = new Tile_Vector2Int<UnitController>[Model.CurrentData.tileNum.x, Model.CurrentData.tileNum.y];
        for (int x = 0; x < gameBoard.GetLength(0); x++)
        {
            for (int y = 0; y < gameBoard.GetLength(1); y++)
            {
                gameBoard[x, y] = new Tile_Vector2Int<UnitController>(null,new Vector2Int(x,y));
            }

        }
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
        //Calculate upper bounds
        var upperBound = pivot + size;

        for (int i = 0; i < size.x; i++)
        {
            for (int j = 0; j < size.y; j++)
            {
                //Check candidate building within the bounds
                if (pivot.x >= 0 && pivot.y >= 0 && upperBound.x <= gameBoard.GetLength(0) && upperBound.y <= gameBoard.GetLength(1))
                {
                    if (GetUnitAtCoord(new Vector2Int(pivot.x + i, pivot.y + j)) != null)
                        return false;
                }
                else
                    return false;
            }
        }
        return true;
    }
    public UnitController GetUnitAtCoord(Vector2Int coord)
    {
        if(coord.x>=0&& coord.x<Model.CurrentData.tileNum.x && coord.y >= 0 && coord.y < Model.CurrentData.tileNum.y)
            return gameBoard[coord.x, coord.y].Holder;
        return null;
    }
    void OccupyUnitTilesOnBoard(UnitController unitToPlace, Vector2Int coords)
    {
        for (int i = 0; i < unitToPlace.SizeByUnit.x; i++)
            for (int j = 0; j < unitToPlace.SizeByUnit.y; j++)
            {
                Vector2Int setCoords = new Vector2Int(coords.x + i, coords.y + j);
                gameBoard[setCoords.x, setCoords.y].Holder=unitToPlace;
                occupiedTile++;
            }

    }
    public bool PlaceUnit(UnitController unit, Vector2Int coords)
    {
        //Debug.Log("Try place unit at "+coords);

        //If candidate building does not intersect with other instances place it.
        if (CheckAreaIsAvailable(coords, unit.SizeByUnit))
        {
            OccupyUnitTilesOnBoard(unit, coords);
            unit.PlaceTo(View.rectTransform, coords,GetBoardRelativePositionFromCoords(coords), Vector2.up, Vector2.up);
            return true;
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

    List<Vector2Int> GetTilePositionsAroundUnit(UnitController unit)
    {
        var result=new List<Vector2Int>();

        for (int x = unit.PositionByUnit.x - 1; x < unit.PositionByUnit.x + unit.SizeByUnit.x+1;)
        {
            for (int y = unit.PositionByUnit.y - 1; y < unit.PositionByUnit.y + unit.SizeByUnit.y+1;y++)
            {
                Vector2Int candidateTileCoords = new Vector2Int(x, y);
                if (CheckAreaIsAvailable(candidateTileCoords, Vector2Int.one))
                {
                    result.Add(candidateTileCoords);
                }

                if (x >= unit.PositionByUnit.x && x <= unit.PositionByUnit.x + unit.SizeByUnit.x &&
                    y >= unit.PositionByUnit.y && y <= unit.PositionByUnit.y + unit.SizeByUnit.y)
                {
                    x += unit.PositionByUnit.x;
                }
                else
                    x++;
            }
        }

        return result;
    }

    #endregion



    UnitController unitToBePlaced;
    public void OnClickOnBoard(Vector2 viewRelativePosition)
    {
        Vector2Int boardCoord = GetBoardCoordsFromRelativePosition(viewRelativePosition);

        if (unitToBePlaced != null)
        {
            if (PlaceUnit(unitToBePlaced, boardCoord))
            {
                OpenInfo(unitToBePlaced);
                unitToBePlaced = null;
                View.EndFeedback();
            }
        }
        else
        {
            UnitController unit = GetUnitAtCoord(boardCoord);
            OpenInfo(unit);
        }
    }

    public void OnMoveOnBoard(Vector2 viewRelativePosition)
    {
        Vector2Int boardCoord = GetBoardCoordsFromRelativePosition(viewRelativePosition);
        if (unitToBePlaced != null)
        {
            Rect areaOfInterest = new Rect(GetBoardRelativePositionFromCoords(boardCoord), GetPxelSizeFromUnitSize(unitToBePlaced.SizeByUnit));
            if (CheckAreaIsAvailable(boardCoord, unitToBePlaced.SizeByUnit))
            {
                View.FeedbackOnArea(areaOfInterest,Color.green);
            }
            else
            {
                View.FeedbackOnArea(areaOfInterest, Color.red);
            }
        }
    }

    public Path<Tile_Vector2Int<UnitController>> FindShortestPath(Tile_Vector2Int<UnitController> startPoint, Tile_Vector2Int<UnitController> destination)
    {
        Path<Tile_Vector2Int<UnitController>> path;
        //AStar Shortest Path
        var graph = new GraphDataStruct_Vector2IntBoard<UnitController>(ref gameBoard);
        path = AStar.FindPath(graph, startPoint, destination, DistanceFunction, HeuristicFunction);

        //Dijkstra Shortest Path
        //path = Dijkstra.FindPath(graph, startPoint, destination, DistanceFunction);

        return path;
    }

    private double DistanceFunction(Tile_Vector2Int<UnitController> tile1, Tile_Vector2Int<UnitController> tile2)
    {
        //For diagonal neigbours all distances should be equal.
        return 1;
    }

    private double HeuristicFunction(Tile_Vector2Int<UnitController> tile1, Tile_Vector2Int<UnitController> tile2)
    {
        return Vector2Int.Distance(tile1.position, tile2.position);
    }

    void OpenInfo(UnitController unit)
    {
        Redirect(Constants.Events.ShowUnitInfo, Constants.Controllers.InfoController, new Events.GetInfoUnitEventArgs(unit));
    }




    protected override void OnActionRedirected(IController source, string actionName, EventArgs data)
    {
        if (actionName == Constants.Events.AddUnit)
        {
            var unitEventArgs = data as Events.AddUnitEventArgs;
            if (unitEventArgs?.unit != null)
            {
                SetUnitPixelSize(unitEventArgs.unit);
            }

            switch (unitEventArgs.Method)
            {
                case Events.AddUnitEventArgs.UnitAddMethod.SpawnByPositionSelection:
                    unitToBePlaced = unitEventArgs.unit;
                    break;
                case Events.AddUnitEventArgs.UnitAddMethod.Spawn_AtPos:
                    PlaceUnit(unitEventArgs.unit, unitEventArgs.position);
                    break;
                case Events.AddUnitEventArgs.UnitAddMethod.Spawn_AtRandomPos:
                    if(Tile_Vector2Int<UnitController>.OccupiedTile>0)
                    {
                        Vector2Int positionToPlace;
                        GetAvailableAreaOnBoard(out positionToPlace,unitEventArgs.unit.SizeByUnit,20);
                        PlaceUnit(unitEventArgs.unit, positionToPlace);
                    }
                    break;
                case Events.AddUnitEventArgs.UnitAddMethod.Spawn_AtRandomPosAroundUnit:
                    var spawnerUnit= source as UnitController;
                    var spawnArea = GetTilePositionsAroundUnit(spawnerUnit);
                    PlaceUnit(unitEventArgs.unit, spawnArea[UnityEngine.Random.Range(0,spawnArea.Count)]);

                    break;
            }
        }
        //else 
        //if(actionName == Constants.Operations.SpawnSoldier)
        //{
        //    //TODO: Builder tasarlanacak (ViewManager?)
        //    Events.OperationEventArgs operationEventArgs = data as Events.OperationEventArgs;

        //    SoldierUnitController soldierController = new SoldierUnitController(new UnitModel(View.soldierData));
        //    SetUnitPixelSize(soldierController);

        //    Vector2Int candidateSpawnPoint;
        //    do
        //    {
        //        if (occupiedTile >= TotalTile)
        //        {
        //            Debug.LogError("Lack of space! "+ occupiedTile+ "  "+TotalTile);
        //        }
        //        candidateSpawnPoint = new Vector2Int(UnityEngine.Random.Range(0, Model.CurrentData.tileNum.x), UnityEngine.Random.Range(0, Model.CurrentData.tileNum.y));
        //    }
        //    while ((occupiedTile < TotalTile) && !PlaceUnit(soldierController, candidateSpawnPoint));


        //}
    }


    //TODO:kdTree may solve this
    bool GetAvailableAreaOnBoard(out Vector2Int result, Vector2Int dimension,uint ttl)
    {
        Vector2Int candidateSpawnPoint = new Vector2Int(UnityEngine.Random.Range(0, Model.CurrentData.tileNum.x), UnityEngine.Random.Range(0, Model.CurrentData.tileNum.y));
        if (CheckAreaIsAvailable(candidateSpawnPoint, dimension))
        {
            result = candidateSpawnPoint;
            return true;
        }
        ttl--;
        if (ttl > 0)
            return GetAvailableAreaOnBoard(out result,dimension, ttl);
        result = Vector2Int.zero;
        return false;
    }



}
