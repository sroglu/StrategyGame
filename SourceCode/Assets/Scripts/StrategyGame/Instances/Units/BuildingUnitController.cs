
/// <summary>
/// Controller for buildings in the game
/// </summary>
public class BuildingUnitController : UnitController
{
    public BuildingUnitController(UnitModel model) : base(model)
    {
    }

    public override void PerformOperation(Events.OperationEvent e)
    {
        switch (e.operation.command)
        {
            case Constants.Operations.SpawnAgent:

                uint spawnAgentLevel = (uint)e.operation.amount - 1;

                var agent = InstanceManager.Instance.CreateAgent(Model.CurrentData.spawns[spawnAgentLevel]);

                Redirect(Constants.Events.AddUnit, Constants.Controllers.GameBoardController, new Events.AddRandomUnitEventArgs(agent));

                break;
        }
    }
}
