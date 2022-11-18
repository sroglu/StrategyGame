using Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AgentUnitController : UnitController
{
    public AgentUnitController(UnitModel model) : base(model)
    {

    }

    protected override OperationEvent GetDefaultOperation()
    {
        //Move operation is the default operation for agent units
        return new MoveOperationEvent(this, Model.CurrentData.propertyData.operations[0], Pointer.current.position.ReadValue());
    }
}
