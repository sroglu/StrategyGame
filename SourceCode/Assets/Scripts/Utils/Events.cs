using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    public class UnitEventArgs : EventArgs
    {
        public UnitController unitController;
        public UnitEventArgs(UnitController unitController)
        {
            this.unitController = unitController;
        }
    }
    public class OperationEventArgs : EventArgs
    {
        public Operation operation;
        public OperationEventArgs(Operation operation)
        {
            this.operation = operation;
        }
    }
}
