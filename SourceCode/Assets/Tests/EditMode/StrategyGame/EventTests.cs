using NUnit.Framework;

public class EventTests
{
    [Test]
    public void Event_AddUnit()
    {
        Assert.AreEqual(Constants.Events.AddUnit, "unit.add");
    }
    [Test]
    public void Event_ShowUnitInfo()
    {

        Assert.AreEqual(Constants.Events.ShowUnitInfo,"unit.info");
    }
    [Test]
    public void Event_PerformOperation()
    {

        Assert.AreEqual(Constants.Events.PerformOperation, "unit.operation");
    }
}
