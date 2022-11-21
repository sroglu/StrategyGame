using UnityEngine;
using mehmetsrl.MVC.core;

public class InstanceManager : MonoBehaviour
{
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

    public void ClearAllInstances()
    {
        //Clear All Instances Created
        foreach (var instance in GetComponentsInChildren<ViewBase>())
        {
            instance.DestroyInstance();
        }
    }
    public UnitController CreateBuilding(UnitData unitData)
    {
        return new BuildingUnitController(new UnitModel(unitData));
    }

    public UnitController CreateAgent(UnitData unitData)
    {
        return new AgentUnitController(new UnitModel(unitData));
    }
}
