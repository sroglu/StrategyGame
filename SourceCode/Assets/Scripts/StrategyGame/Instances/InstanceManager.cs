using UnityEngine;
using mehmetsrl.MVC.core;

/// <summary>
/// Class for managing instances on the scene
/// </summary>
public class InstanceManager : MonoBehaviour
{
    //Singleton
    private static InstanceManager _instance;
    public static InstanceManager Instance
    {
        get { return _instance; }
        private set
        {
            if (_instance == null)
                _instance = value;
            else
                Destroy(value);
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Clear all instances under management of this manager.
    /// Some instances might be moved to other transforms.
    /// Those instances should managed by their responsible managers or controllers.
    /// </summary>
    public void ClearAllInstances()
    {
        foreach (var instance in GetComponentsInChildren<ViewBase>())
        {
            instance.DestroyInstance();
        }
    }

    /// <summary>
    /// Builder method for buildings
    /// </summary>
    /// <param name="unitData">Unit data</param>
    /// <returns>Building unit</returns>
    public UnitController CreateBuilding(UnitData unitData)
    {
        var unit = new BuildingUnitController(new UnitModel(unitData));
        unit.View.transform.parent = transform;
        return unit;
    }

    /// <summary>
    /// Builder method for agents
    /// </summary>
    /// <param name="unitData">Unit data</param>
    /// <returns>Agent unit</returns>
    public UnitController CreateAgent(UnitData unitData)
    {
        var unit = new AgentUnitController(new UnitModel(unitData));
        unit.View.transform.parent = transform;
        return unit;
    }
}
