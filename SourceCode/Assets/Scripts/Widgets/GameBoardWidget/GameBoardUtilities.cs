using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mehmetsrl.Algorithms.DataStructures;
using mehmetsrl.Algorithms.Graph;
using System.Linq;
public partial class GameBoardController : Controller<GameBoardView, GameBoardModel>
{
    #region UtilityFunctions

    //Board Functions
    //Generic Functions
    void CreateGameBoard()
    {
        gameBoard = new Tile_Vector2Int<UnitController>[Model.CurrentData.tileNum.x, Model.CurrentData.tileNum.y];
        for (int x = 0; x < gameBoard.GetLength(0); x++)
        {
            for (int y = 0; y < gameBoard.GetLength(1); y++)
            {
                gameBoard[x, y] = new Tile_Vector2Int<UnitController>(null, new Vector2Int(x, y));
            }

        }
    }
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
    bool CheckAreaIsAvailable(RectInt area)
    {
        //Calculate upper bounds
        //Check candidate building within the bounds
        if (area.x >= 0 && area.y >= 0 && (area.x + area.width) <= gameBoard.GetLength(0) && area.y + area.height <= gameBoard.GetLength(1))
        {
            for (int x = area.x; x < area.x + area.width; x++)
            {
                for (int y = area.y; y < area.y + area.height; y++)
                {
                    if (gameBoard[x, y].Holder != null)
                        return false;
                }
            }
            return true;
        }
        return false;
    }
    List<Vector2Int> GetTilePositionsAroundArea(RectInt area, uint range = 1)
    {
        var result = new List<Vector2Int>();

        for (int x = (int)(area.x - range); x < area.x + area.width + range; x++)
        {
            for (int y = (int)(area.y - range); y < area.y + area.height + range; y++)
            {
                //Skip area that spawner unit placed
                if (x >= area.x && x < area.x + area.width &&
                    y >= area.y && y < area.y + area.height)
                {
                    continue;
                }

                Vector2Int candidateTileCoords = new Vector2Int(x, y);
                if (CheckAreaIsAvailable(new RectInt(candidateTileCoords, Vector2Int.one)))
                {
                    result.Add(candidateTileCoords);
                }
            }
        }

        return result;
    }

    //Unit Functions
    public UnitController GetUnitAtCoord(Vector2Int coord)
    {
        if (coord.x >= 0 && coord.x < gameBoard.GetLength(0) && coord.y >= 0 && coord.y < gameBoard.GetLength(1))
            return gameBoard[coord.x, coord.y].Holder;
        return null;
    }
    void OccupyUnitTilesOnBoard(UnitController unitToPlace, Vector2Int coords)
    {
        for (int i = 0; i < unitToPlace.SizeByUnit.x; i++)
            for (int j = 0; j < unitToPlace.SizeByUnit.y; j++)
            {
                Vector2Int setCoords = new Vector2Int(coords.x + i, coords.y + j);
                gameBoard[setCoords.x, setCoords.y].Holder = unitToPlace;
            }

    }
    public bool TryPlaceUnit(UnitController unit, Vector2Int coords)
    {
        //If candidate building does not intersect with other instances place it.
        if (CheckAreaIsAvailable(new RectInt(coords, unit.SizeByUnit)))
        {
            PlaceUnit(unit,coords);
            return true;
        }
        return false;
    }
    public bool TryRemoveUnit(UnitController unit)
    {
        RectInt areaToRemove = new RectInt(unit.PositionByUnit, unit.SizeByUnit);

        for (int x = areaToRemove.x; x < areaToRemove.x + areaToRemove.width; x++)
            for (int y = areaToRemove.y; y < areaToRemove.y + areaToRemove.height; y++) {
                {
                    if (gameBoard[x, y].Holder != unit)
                        return false;
                } 
            }
        RemoveUnit(areaToRemove);
        return true;
    }
    void PlaceUnit(UnitController unit, Vector2Int coords)
    {
        OccupyUnitTilesOnBoard(unit, coords);
        unit.PlaceTo(View.rectTransform, coords, GetBoardRelativePositionFromCoords(coords), Vector2.up, Vector2.up);
    }
    void RemoveUnit(RectInt areaToRemove)
    {
        for (int x = areaToRemove.x; x < areaToRemove.x + areaToRemove.width; x++)
        {
            for (int y = areaToRemove.y; y < areaToRemove.y + areaToRemove.height; y++)
            {
                gameBoard[x, y].Holder = null;
            }
        }
    }
    public void SetUnitPixelSize(UnitController unit)
    {
        unit.SetPixelSize(
                    new Vector2(
                        unit.SizeByUnit.x * Model.CurrentData.tileSize.x,
                        unit.SizeByUnit.y * Model.CurrentData.tileSize.y));
    }
    IEnumerator MoveUnit(UnitController unit, Vector2Int targetCoord)
    {
        var path = FindShortestPath(new Tile_Vector2Int<UnitController>(unit, unit.PositionByUnit), new Tile_Vector2Int<UnitController>(null, targetCoord));

        while (path.Count > 0)
        {
            var nextTile = path.Dequeue();
            if (nextTile.Holder != null)
            {
                path = FindShortestPath(new Tile_Vector2Int<UnitController>(unit, unit.PositionByUnit), new Tile_Vector2Int<UnitController>(null, targetCoord));
                nextTile = path.Dequeue();
            }

            RemoveUnit(new RectInt(unit.PositionByUnit, unit.SizeByUnit));
            PlaceUnit(unit, nextTile.position);
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    //TODO:kdTree may solve GetAvailableAreaOnBoard issue
    bool GetAvailableAreaOnBoard(out Vector2Int result, Vector2Int dimension, uint ttl)
    {
        Vector2Int candidateSpawnPoint = new Vector2Int(UnityEngine.Random.Range(0, gameBoard.GetLength(0)), UnityEngine.Random.Range(0, gameBoard.GetLength(1)));
        if (CheckAreaIsAvailable(new RectInt(candidateSpawnPoint, dimension)))
        {
            result = candidateSpawnPoint;
            return true;
        }
        ttl--;
        if (ttl > 0)
            return GetAvailableAreaOnBoard(out result, dimension, ttl);
        result = Vector2Int.zero;
        return false;
    }

    List<Vector2Int> GetTilePositionsAroundUnit(UnitController unit, uint range = 1)
    {
        var result = new List<Vector2Int>();

        for (int x = (int)(unit.PositionByUnit.x - range); x < unit.PositionByUnit.x + unit.SizeByUnit.x + range; x++)
        {
            for (int y = (int)(unit.PositionByUnit.y - range); y < unit.PositionByUnit.y + unit.SizeByUnit.y + range; y++)
            {
                //Skip area that spawner unit placed
                if (x >= unit.PositionByUnit.x && x < unit.PositionByUnit.x + unit.SizeByUnit.x &&
                    y >= unit.PositionByUnit.y && y < unit.PositionByUnit.y + unit.SizeByUnit.y)
                {
                    continue;
                }

                Vector2Int candidateTileCoords = new Vector2Int(x, y);
                if (CheckAreaIsAvailable(new RectInt(candidateTileCoords, Vector2Int.one)))
                {
                    result.Add(candidateTileCoords);
                }
            }
        }
        return result;
    }

    //Path Finding
    public Queue<Tile_Vector2Int<UnitController>> FindShortestPath(Tile_Vector2Int<UnitController> startPoint, Tile_Vector2Int<UnitController> destination)
    {
        Path<Tile_Vector2Int<UnitController>> path;
        //AStar Shortest Path
        var graph = new GraphDataStruct_Vector2IntBoard<UnitController>(ref gameBoard);
        path = AStar.FindPath(graph, startPoint, destination, DistanceFunction, HeuristicFunction);

        //Dijkstra Shortest Path
        //path = Dijkstra.FindPath(graph, startPoint, destination, DistanceFunction);
        return new Queue<Tile_Vector2Int<UnitController>>(path.Reverse());
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

    #endregion

}