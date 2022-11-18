using System;
using UnityEngine;
using mehmetsrl.Algorithms.Graph;
using System.Collections.Generic;
using mehmetsrl.Algorithms.DataStructures;
using System.Collections;

public partial class GameBoardController : Controller<GameBoardView, GameBoardModel>
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

    UnitController activeUnit,unitToBePlaced,candidateUnitToMove;

    public void OnRightClickOnBoard(Vector2 viewRelativePosition)
    {
        if (unitToBePlaced != null)
        {
            unitToBePlaced.Abandon();
            unitToBePlaced = null;
        }
        else if (activeUnit != null)
        {
            activeUnit.PerformDefaultOperation();
        }
    }

    public void OnClickOnBoard(Vector2 viewRelativePosition)
    {
        Vector2Int boardCoord = GetBoardCoordsFromRelativePosition(viewRelativePosition);

        if (unitToBePlaced != null)
        {
            if (TryPlaceUnit(unitToBePlaced, boardCoord))
            {
                OpenInfo(unitToBePlaced);
                unitToBePlaced = null;
                View.EndFeedback();
            }
        }else if (candidateUnitToMove != null)
        {
            View.StartCoroutine(MoveUnit(candidateUnitToMove, boardCoord));
        }
        else
        {
            activeUnit = GetUnitAtCoord(boardCoord);
            OpenInfo(activeUnit);
        }
    }

    public void OnMoveOnBoard(Vector2 viewRelativePosition)
    {
        Vector2Int boardCoord = GetBoardCoordsFromRelativePosition(viewRelativePosition);
        if (unitToBePlaced != null)
        {
            Rect areaOfInterest = new Rect(GetBoardRelativePositionFromCoords(boardCoord), GetPxelSizeFromUnitSize(unitToBePlaced.SizeByUnit));
            if (CheckAreaIsAvailable(new RectInt(boardCoord, unitToBePlaced.SizeByUnit)))
            {
                View.FeedbackOnArea(areaOfInterest,Color.green);
            }
            else
            {
                View.FeedbackOnArea(areaOfInterest, Color.red);
            }
        }
    }

    protected override void OnActionRedirected(IController source, string actionName, EventArgs data)
    {
        switch(actionName)
        {
            case Constants.Events.AddUnit:
                AddUnit(source,data as Events.AddUnitEventArgs);
                break;
            case Constants.Events.PerformOperation:
                HandleUnitOperations(data as Events.OperationEvent);
                break;
        }

    }


    #region GameboardEvents
    void AddUnit(IController source, Events.AddUnitEventArgs e)
    {
        if (e == null) return;
        if (e.unit != null)
        {
            View.AddInstanceToPlayground(e.unit.View.rectTransform);
            SetUnitPixelSize(e.unit);
        }
        switch (e.method)
        {
            case Events.AddUnitEventArgs.UnitAddMethod.SpawnByPositionSelection:
                unitToBePlaced = e.unit;
                break;
            case Events.AddUnitEventArgs.UnitAddMethod.Spawn_AtPos:
                TryPlaceUnit(e.unit, GetBoardCoordsFromRelativePosition(View.CalculateRelativePos(e.position)));
                break;
            case Events.AddUnitEventArgs.UnitAddMethod.Spawn_AtRandomPos:
                if (Tile_Vector2Int<UnitController>.OccupiedTile > 0)
                {
                    Vector2Int positionToPlace;
                    if (GetAvailableAreaOnBoard(out positionToPlace, e.unit.SizeByUnit, 20))
                        TryPlaceUnit(e.unit, positionToPlace);
                    else
                        e.unit.Abandon();
                }
                break;
            case Events.AddUnitEventArgs.UnitAddMethod.Spawn_AtRandomPosAroundUnit:
                var spawnerUnit = source as UnitController;
                var spawnArea = GetTilePositionsAroundUnit(spawnerUnit);
                if (spawnArea.Count > 0)
                    TryPlaceUnit(e.unit, spawnArea[UnityEngine.Random.Range(0, spawnArea.Count)]);
                else
                    e.unit.Abandon();
                break;
        }
    }


    public void HandleUnitOperations(Events.OperationEvent e)
    {
        if (e == null) return;

        switch(e.operation.command)
        {
            case Constants.Operations.Move:
                var moveEvent = e as Events.MoveOperationEvent;
                if (moveEvent != null)
                {
                    Vector2Int moveTargetCorrd = GetBoardCoordsFromRelativePosition(View.CalculateRelativePos(moveEvent.position));
                    View.StartCoroutine(MoveUnit(e.unit, moveTargetCorrd));
                }
                else
                {
                    candidateUnitToMove = e.unit;
                }
                break;
        }



    }

    IEnumerator MoveUnit(UnitController unit, Vector2Int targetCoord)
    {
        var path = FindShortestPath(new Tile_Vector2Int<UnitController>(unit,unit.PositionByUnit), new Tile_Vector2Int<UnitController>(null, targetCoord));

        while (path.Count>0)
        {
            var nextTile = path.Dequeue();
            if (nextTile.Holder != null)
            {
                path = FindShortestPath(new Tile_Vector2Int<UnitController>(unit, unit.PositionByUnit), new Tile_Vector2Int<UnitController>(null, targetCoord));
                nextTile = path.Dequeue();
            }

            //Debug.Log(nextTile);

            RemoveUnit(new RectInt(candidateUnitToMove.PositionByUnit, candidateUnitToMove.SizeByUnit));
            PlaceUnit(candidateUnitToMove,nextTile.position);
            yield return new WaitForFixedUpdate();
        }

        yield return null;


    }

    #endregion


}
