using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using mehmetsrl.Algorithms.DataStructures;
using mehmetsrl.Algorithms.Graph;
using System.Linq;
using UnityEngine.InputSystem;
using mehmetsrl.MVC.core;

/// <summary>
/// Partial class to organize GameboarController utility methods
/// </summary>
public partial class GameBoardController : Controller<GameBoardView, GameBoardModel>
{
    #region Accesors
    public Vector2Int InvalidPosition { get { return Constants.Values.InvalidVector2Int; } }
    #endregion


    #region UtilityFunctions

    //Board Functions
    //Base Functions
    /// <summary>
    /// Creates gameboard.
    /// </summary>
    public void CreateGameBoard()
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
    /// <summary>
    /// Converts component relative position to gameboard position.
    /// </summary>
    /// <param name="pointerRelativePosition"></param>
    /// <returns></returns>
    public Vector2Int GetBoardCoordsFromRelativePosition(Vector2 pointerRelativePosition)
    {
        return new Vector2Int((int)(pointerRelativePosition.x / Model.CurrentData.tileSize.x), (int)(pointerRelativePosition.y / Model.CurrentData.tileSize.y));
    }
    /// <summary>
    /// Converts gameboard position to component relative position.
    /// </summary>
    /// <param name="coords">Gameboard coord</param>
    /// <returns></returns>
    public Vector2 GetBoardRelativePositionFromCoords(Vector2Int coords)
    {
        return new Vector2(coords.x * Model.CurrentData.tileSize.x, -coords.y * Model.CurrentData.tileSize.y);
    }
    /// <summary>
    /// Converts gameboard size to pure pixel size.
    /// </summary>
    /// <param name="size">Gameboar size</param>
    /// <returns></returns>
    public Vector2 GetPxelSizeFromUnitSize(Vector2Int size)
    {
        return new Vector2(size.x * Model.CurrentData.tileSize.x, size.y * Model.CurrentData.tileSize.y);
    }
    /// <summary>
    /// Checks whather or not the specified area is available on the gameboard.
    /// </summary>
    /// <param name="area">Query area</param>
    /// <returns></returns>
    public bool CheckAreaIsAvailable(RectInt area)
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
    /// <summary>
    /// Detects areas availe around by given area and range.
    /// </summary>
    /// <param name="area">Query area</param>
    /// <param name="range">Quer range</param>
    /// <returns></returns>
    public List<Vector2Int> GetTilePositionsAroundArea(RectInt area, uint range = 1)
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
    /// <summary>
    /// Shows feedback on the view.
    /// </summary>
    public IEnumerator ShowFeedbackOnGameBoard()
    {
        Vector2 relativePos;
        Rect areaOfInterest;
        while (projectionArea.size != Vector2Int.zero)
        {
            relativePos = View.CalculateRelativePos(Pointer.current.position.ReadValue());
            projectionArea.position = GetBoardCoordsFromRelativePosition(relativePos);
            areaOfInterest = new Rect(GetBoardRelativePositionFromCoords(projectionArea.position), GetPxelSizeFromUnitSize(projectionArea.size));

            if (CheckAreaIsAvailable(projectionArea))
            {
                View.FeedbackOnArea(areaOfInterest, Color.green);
            }
            else
            {
                View.FeedbackOnArea(areaOfInterest, Color.red);
            }

            yield return new WaitForFixedUpdate();
        }
        View.EndFeedback();
        yield return null;

    }

    //Unit Functions Interface could be used
    /// <summary>
    /// Finds unit controller by given coord.
    /// </summary>
    /// <param name="coord">Gameboard coord</param>
    /// <returns></returns>
    public UnitController GetUnitAtCoord(Vector2Int coord)
    {
        if (coord.x >= 0 && coord.x < gameBoard.GetLength(0) && coord.y >= 0 && coord.y < gameBoard.GetLength(1))
            return gameBoard[coord.x, coord.y].Holder;
        return null;
    }
    /// <summary>
    /// Fills gameboard tiles with given unit.
    /// </summary>
    /// <param name="unitToPlace">Unit</param>
    /// <param name="coords">Gameboard coord</param>
    public void OccupyUnitTilesOnBoard(UnitController unitToPlace, Vector2Int coords)
    {
        for (int i = 0; i < unitToPlace.SizeByUnit.x; i++)
            for (int j = 0; j < unitToPlace.SizeByUnit.y; j++)
            {
                Vector2Int setCoords = new Vector2Int(coords.x + i, coords.y + j);
                gameBoard[setCoords.x, setCoords.y].Holder = unitToPlace;
            }

    }
    /// <summary>
    /// Querys gameboard area and place the unit.
    /// </summary>
    /// <param name="unit">Unit</param>
    /// <param name="coords">Gameboard coord</param>
    /// <returns></returns>
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
    /// <summary>
    /// Querys gameboard area and remove the unit.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
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
    /// <summary>
    /// Places unit without querying.
    /// </summary>
    /// <param name="unit">Unit</param>
    /// <param name="coords">Gameboard coord</param>
    public void PlaceUnit(UnitController unit, Vector2Int coords)
    {
        OccupyUnitTilesOnBoard(unit, coords);
        unit.PlaceTo(View.rectTransform, coords, GetBoardRelativePositionFromCoords(coords), Vector2.up, Vector2.up);
    }
    /// <summary>
    /// Removes unit without querying.
    /// </summary>
    /// <param name="areaToRemove">Gameboard coord</param>
    public void RemoveUnit(RectInt areaToRemove)
    {
        for (int x = areaToRemove.x; x < areaToRemove.x + areaToRemove.width; x++)
        {
            for (int y = areaToRemove.y; y < areaToRemove.y + areaToRemove.height; y++)
            {
                gameBoard[x, y].Holder = null;
            }
        }
    }
    /// <summary>
    /// Sets unit size by using gameboard data.
    /// </summary>
    /// <param name="unit">Unit</param>
    public void SetUnitPixelSize(UnitController unit)
    {
        unit.SetPixelSize(
                    new Vector2(
                        unit.SizeByUnit.x * Model.CurrentData.tileSize.x,
                        unit.SizeByUnit.y * Model.CurrentData.tileSize.y));
    }
    /// <summary>
    /// Moves unit to given coord.
    /// </summary>
    /// <param name="unit">Unit to move</param>
    /// <param name="targetCoord">Gameboard coord to move</param>
    /// <returns></returns>
    IEnumerator MoveUnit(UnitController unit, Vector2Int targetCoord)
    {
        var path = FindShortestPath(new Tile_Vector2Int<UnitController>(unit, unit.PositionByUnit), new Tile_Vector2Int<UnitController>(null, targetCoord));
        while (path!=null && path.Count > 0)
        {
            var nextTile = path.Dequeue();
            if (nextTile.Holder != null)
            {
                path = FindShortestPath(new Tile_Vector2Int<UnitController>(unit, unit.PositionByUnit), new Tile_Vector2Int<UnitController>(null, targetCoord));
                if (path == null) yield return null;
                nextTile = path.Dequeue();
            }

            RemoveUnit(new RectInt(unit.PositionByUnit, unit.SizeByUnit));
            PlaceUnit(unit, nextTile.position);
            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    //TODO:kdTree may solve GetAvailableAreaOnBoard issue
    /// <summary>
    /// Recursive function that Tries to find available position.
    /// To control ttl is used.
    /// kdTree might be used to solve this.
    /// </summary>
    /// <param name="result">Out available place</param>
    /// <param name="dimension">Query dimension</param>
    /// <param name="ttl">Time to live/ How many times try to find suitable place.</param>
    /// <returns></returns>
    public bool GetAvailableAreaOnBoard(out Vector2Int result, Vector2Int dimension, uint ttl)
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
    /// <summary>
    /// Checks gameboard tile left or not.
    /// </summary>
    /// <returns>That are tiles available or not</returns>
    public bool IsThereAnyTileLeft()
    {
        return Tile_Vector2Int<UnitController>.OccupiedTile < gameBoard.GetLength(0) * gameBoard.GetLength(1);
    }
    /// <summary>
    /// Finds random position in the given area.
    /// </summary>
    /// <param name="area">Query area</param>
    /// <returns>Random gameboard coord</returns>
    public Vector2Int GetRandomPositionAround(RectInt area)
    {
        while (IsThereAnyTileLeft())
        {
            var spawnArea = GetTilePositionsAroundArea(area);
            if(spawnArea.Count>0)
            {
                return spawnArea[Random.Range(0, spawnArea.Count)];
            }
            area.position -= Vector2Int.one;
            area.size += Vector2Int.one * 2;
        }
        return InvalidPosition;
    }

    //Path Finding
    /// <summary>
    /// Finds shortest path by given start and end points on the gameboard.
    /// </summary>
    /// <param name="startPoint">Start gameboard coord</param>
    /// <param name="destination">Destination gameboard coord</param>
    /// <returns>Shortest path queue</returns>
    public Queue<Tile_Vector2Int<UnitController>> FindShortestPath(Tile_Vector2Int<UnitController> startPoint, Tile_Vector2Int<UnitController> destination)
    {
        Path<Tile_Vector2Int<UnitController>> path;
        //AStar Shortest Path
        var graph = new GraphDataStruct_Vector2IntBoard<UnitController>(ref gameBoard);
        path = AStar.FindPath(graph, startPoint, destination, DistanceFunction, HeuristicFunction);

        //Dijkstra Shortest Path
        //path = Dijkstra.FindPath(graph, startPoint, destination, DistanceFunction);

        if (path == null) return null;
        return new Queue<Tile_Vector2Int<UnitController>>(path.Reverse());
    }

    /// <summary>
    /// Calculates distance between neighbours in graph structure.
    /// </summary>
    /// <param name="tile1">Query tile</param>
    /// <param name="tile2">Neigbour tile</param>
    /// <returns>Distance</returns>
    private double DistanceFunction(Tile_Vector2Int<UnitController> tile1, Tile_Vector2Int<UnitController> tile2)
    {
        //For diagonal neigbours all distances should be equal.
        return 1;
    }

    /// <summary>
    /// Calculates heuristic distance between query tile and destination tile.
    /// </summary>
    /// <param name="tile1">Query tile</param>
    /// <param name="tile2">Destination tile</param>
    /// <returns>Distance</returns>
    private double HeuristicFunction(Tile_Vector2Int<UnitController> tile1, Tile_Vector2Int<UnitController> tile2)
    {
        return Vector2Int.Distance(tile1.position, tile2.position);
    }

    #endregion

}
