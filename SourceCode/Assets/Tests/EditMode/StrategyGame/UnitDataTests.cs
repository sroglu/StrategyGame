using NUnit.Framework;
using UnityEngine;

public class UnitDataTests
{

    [Test]
    public void UnitDataOperationTest()
    {
        //var unitObjs = AssetDatabase.LoadAllAssetsAtPath("Assets/Resources/Data/Units");
        var unitObjs = Resources.LoadAll("Data/Units", typeof(UnitData));


        foreach (var uObj in unitObjs)
        {
            UnitData unitData = uObj as UnitData;

            if (unitData != null)
            {
                foreach (var unitDataOperation in unitData.propertyData.operations)
                {
                    if (!Constants.Operations.All.Contains(unitDataOperation.command))
                    {
                        Assert.Fail("Failed at command: "+ unitDataOperation.command+" of "+ unitData, unitData);
                    }
                }
            }
        }

        Assert.Pass();
    }




    [Test]
    public void UnitDataEffectTest()
    {
        //var unitObjs = AssetDatabase.LoadAllAssetsAtPath("Assets/Resources/Data/Units");
        var unitObjs = Resources.LoadAll("Data/Units", typeof(UnitData));


        foreach (var uObj in unitObjs)
        {
            UnitData unitData = uObj as UnitData;

            if (unitData != null)
            {
                foreach (var unitDataEffect in unitData.propertyData.effects)
                {
                    if (!Constants.Effects.All.Contains(unitDataEffect.effect))
                    {
                        Assert.Fail("Failed at effect: " + unitDataEffect.effect + " of " + unitData, unitData);
                    }
                }
            }
        }

        Assert.Pass();
    }



}