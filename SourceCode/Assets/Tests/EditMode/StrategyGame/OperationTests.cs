using NUnit.Framework;

public class OperationTests
{
    [Test]
    public void Operation_SpawnAgent()
    {
        Assert.AreEqual(Constants.Operations.SpawnAgent, "operation.spawn.agent");
    }
    [Test]
    public void Operation_Move()
    {
        Assert.AreEqual(Constants.Operations.Move, "operation.move");
    }
    [Test]
    public void Operation_DealDamage()
    {
        Assert.AreEqual(Constants.Operations.DealDamage, "operation.damage");
    }
}
