using System;
using UnityEngine;
using mehmetsrl.Algorithms.Graph;
using System.Collections.Generic;
using mehmetsrl.Algorithms.DataStructures;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public partial class GameBoardController : Controller<GameBoardView, GameBoardModel>
{
    Tile_Vector2Int<UnitController>[,] gameBoard;
    public GameBoardController(GameBoardModel model,GameBoardView view) : base(ControllerType.instance, model, view)
    {
    }
    protected override void OnCreate()
    {
        CreateGameBoard();
    }

    UnitController activeUnit;
    RectInt projectionArea;

    public void OnRightClickOnBoard(Vector2 viewRelativePosition)
    {
        Vector2Int boardCoord = GetBoardCoordsFromRelativePosition(viewRelativePosition);
        if (actionsWaitingRightClickEvent.Count > 0)
        {
            actionsWaitingRightClickEvent.Dequeue()?.Invoke(boardCoord);
        }
        else if (activeUnit != null)
        {
            activeUnit.PerformDefaultOperation();
        }
    }

    public void OnClickOnBoard(Vector2 viewRelativePosition)
    {
        Vector2Int boardCoord = GetBoardCoordsFromRelativePosition(viewRelativePosition);

        if (actionsWaitingLeftClickEvent.Count > 0)
        {
            actionsWaitingLeftClickEvent.Dequeue()?.Invoke(boardCoord);
        }
        else
        {
            activeUnit = GetUnitAtCoord(boardCoord);
            OpenInfo(activeUnit);
        }
    }

    void OpenInfo(UnitController unit)
    {
        Redirect(Constants.Events.ShowUnitInfo, Constants.Controllers.InfoController, new Events.GetInfoUnitEventArgs(unit));
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

    Queue<UnityAction<Vector2Int>> actionsWaitingLeftClickEvent = new Queue<UnityAction<Vector2Int>>();
    Queue<UnityAction<Vector2Int>> actionsWaitingRightClickEvent = new Queue<UnityAction<Vector2Int>>();

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
                projectionArea = new RectInt(Vector2Int.zero,e.unit.SizeByUnit);
                View.StartCoroutine(ShowFeedbackOnGameBoard());

                actionsWaitingLeftClickEvent.Enqueue(
                        new UnityAction<Vector2Int>(
                        (Vector2Int targetCoord) => 
                        {
                            if (TryPlaceUnit(e.unit, targetCoord))
                            {
                                OpenInfo(e.unit);
                            }
                            else
                            {
                                e.unit.Abandon();
                            }
                            projectionArea = new RectInt(Vector2Int.zero,Vector2Int.zero);
                        }
                        ));
                break;
            case Events.AddUnitEventArgs.UnitAddMethod.Spawn_AtPos:
                TryPlaceUnit(e.unit, GetBoardCoordsFromRelativePosition(View.CalculateRelativePos(e.position)));
                break;
            case Events.AddUnitEventArgs.UnitAddMethod.Spawn_AtRandomPos:
                if (Tile_Vector2Int<UnitController>.OccupiedTile > 0)
                {
                    Vector2Int positionToPlace;
                    if (GetAvailableAreaOnBoard(out positionToPlace, e.unit.SizeByUnit, 20))
                    {
                        TryPlaceUnit(e.unit, positionToPlace);
                    }
                    else
                    {
                        e.unit.Abandon();
                    }
                }
                break;
            case Events.AddUnitEventArgs.UnitAddMethod.Spawn_AtRandomPosAroundUnit:
                var spawnerUnit = source as UnitController;
                var spawnArea = GetTilePositionsAroundUnit(spawnerUnit);
                if (spawnArea.Count > 0)
                {
                    TryPlaceUnit(e.unit, spawnArea[UnityEngine.Random.Range(0, spawnArea.Count)]);
                }
                else
                {
                    e.unit.Abandon();
                }
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
                    Vector2Int moveTargetCoord = GetBoardCoordsFromRelativePosition(View.CalculateRelativePos(moveEvent.position));
                    View.StartCoroutine(MoveUnit(e.unit, moveTargetCoord));
                }
                else
                {
                    actionsWaitingLeftClickEvent.Enqueue(
                        new UnityAction<Vector2Int>(
                        (Vector2Int targetCoord) =>
                            View.StartCoroutine(MoveUnit(e.unit, targetCoord))
                        ));
                }
                break;
        }

    }


    IEnumerator ShowFeedbackOnGameBoard()
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


    #endregion
}
