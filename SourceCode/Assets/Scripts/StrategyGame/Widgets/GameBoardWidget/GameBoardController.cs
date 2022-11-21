using System;
using UnityEngine;
using System.Collections.Generic;
using mehmetsrl.Algorithms.DataStructures;
using UnityEngine.Events;
using mehmetsrl.MVC.core;
public partial class GameBoardController : Controller<GameBoardView, GameBoardModel>
{
    #region Properties
    Tile_Vector2Int<UnitController>[,] gameBoard;
    UnitController activeUnit;
    RectInt projectionArea;
    #endregion
    public GameBoardController(GameBoardModel model,GameBoardView view) : base(ControllerType.instance, model, view)
    {
    }
    protected override void OnCreate()
    {
        CreateGameBoard();
    }

    #region EventHandlers
    /// <summary>
    /// Handles right click on gameboard
    /// </summary>
    /// <param name="viewRelativePosition">Pointer position relative to the component</param>
    public void OnRightClickOnBoard(Vector2 viewRelativePosition)
    {
        //Get the board coord to process further.
        Vector2Int boardCoord = GetBoardCoordsFromRelativePosition(viewRelativePosition);
        
        //If right click event queue is not empty. Perform right click action.
        if (actionsWaitingRightClickEvent.Count > 0)
        {
            actionsWaitingRightClickEvent.Dequeue()?.Invoke(boardCoord);
        }
        else if (activeUnit != null)
        {
            //Perform default action for active unit
            activeUnit.PerformDefaultOperation();
        }
    }
    /// <summary>
    /// Handles left click on gameboard
    /// </summary>
    /// <param name="viewRelativePosition">Pointer position relative to the component</param>
    public void OnClickOnBoard(Vector2 viewRelativePosition)
    {
        //Get the board coord to process further.
        Vector2Int boardCoord = GetBoardCoordsFromRelativePosition(viewRelativePosition);

        //If right left event queue is not empty. Perform left click action.
        if (actionsWaitingLeftClickEvent.Count > 0)
        {
            actionsWaitingLeftClickEvent.Dequeue()?.Invoke(boardCoord);
        }
        else
        {
            //Select an active unit
            activeUnit = GetUnitAtCoord(boardCoord);
            OpenInfo(activeUnit);
        }
    }

    #endregion


    #region GameflowFunctions
    /// <summary>
    /// Facede for redirect open info command to the info widget
    /// </summary>
    /// <param name="unit"></param>
    void OpenInfo(UnitController unit)
    {
        Redirect(Constants.Events.ShowUnitInfo, Constants.Controllers.InfoController, new Events.GetInfoUnitEventArgs(unit));
    }
    /// <summary>
    /// Handles responsible events by overriding.
    /// </summary>
    protected override void OnActionRedirected(IController source, string actionName, EventArgs data)
    {
        switch(actionName)
        {
            case Constants.Events.AddUnit:
                HandleAddUnit(source,data as Events.AddUnitEventArgs);
                break;
            case Constants.Events.PerformOperation:
                HandleUnitOperations(data as Events.OperationEvent);
                break;
        }
    }

    #endregion


    #region GameboardEvents
    Queue<UnityAction<Vector2Int>> actionsWaitingLeftClickEvent = new Queue<UnityAction<Vector2Int>>();
    Queue<UnityAction<Vector2Int>> actionsWaitingRightClickEvent = new Queue<UnityAction<Vector2Int>>();
    /// <summary>
    /// Add unit event handle function.
    /// </summary>
    void HandleAddUnit(IController source, Events.AddUnitEventArgs e)
    {
        if (e == null) return;
        //Setting size uf the unit by using gameboard data
        if (e.unit != null)
        {
            SetUnitPixelSize(e.unit);
        }
        switch (e.method)
        {
            case Events.AddUnitEventArgs.UnitAddMethod.SpawnByPositionSelection:
                //Assign projection area to give feedback.
                projectionArea = new RectInt(Vector2Int.zero, e.unit.SizeByUnit);
                //Show feedback
                View.StartCoroutine(ShowFeedbackOnGameBoard());

                //Wait for user input and place the unit
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
                                //If cannot place the unit abondon
                                e.unit.Abandon();
                            }
                            //Clear projection area
                            projectionArea = new RectInt(Vector2Int.zero, Vector2Int.zero);
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
                    //Check available area for the unit than place it
                    if (GetAvailableAreaOnBoard(out positionToPlace, e.unit.SizeByUnit, 20))
                    {
                        TryPlaceUnit(e.unit, positionToPlace);
                    }
                    else
                    {
                        //If cannot place the unit abondon
                        e.unit.Abandon();
                    }
                }
                break;
            case Events.AddUnitEventArgs.UnitAddMethod.Spawn_AtRandomPosAroundUnit:
                //Get random position around spawner unit.
                var spawnerUnit = source as UnitController;
                Vector2Int randomPosAroundUnit = GetRandomPositionAround(new RectInt(spawnerUnit.PositionByUnit, spawnerUnit.SizeByUnit));
                //If there is an available position place it.
                if (randomPosAroundUnit != InvalidPosition)
                {
                    PlaceUnit(e.unit, randomPosAroundUnit);
                }
                else
                {
                    //If not abondon the unit
                    e.unit.Abandon();
                }
                break;
        }
    }
    /// <summary>
    /// Unit operation event handle function.
    /// </summary>
    void HandleUnitOperations(Events.OperationEvent e)
    {
        if (e == null) return;

        switch(e.operation.command)
        {
            case Constants.Operations.Move:
                var moveEvent = e as Events.MoveOperationEvent;
                if (moveEvent != null)
                {
                    //If move event is already defined. Directly move the unit to specified position
                    //Pointer position should be processed before call MoveUnit since it takes gameboard coord.
                    Vector2Int moveTargetCoord = GetBoardCoordsFromRelativePosition(View.CalculateRelativePos(moveEvent.position));
                    View.StartCoroutine(MoveUnit(e.unit, moveTargetCoord));
                }
                else
                {
                    //If move event is not specified. Wait for user input
                    actionsWaitingLeftClickEvent.Enqueue(
                        new UnityAction<Vector2Int>(
                        (Vector2Int targetCoord) =>
                            View.StartCoroutine(MoveUnit(e.unit, targetCoord))
                        ));
                }
                break;
        }

    }
    #endregion
}
