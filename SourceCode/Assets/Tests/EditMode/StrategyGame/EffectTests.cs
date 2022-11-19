using NUnit.Framework;

public class EffectTests
{
    [Test]
    public void Effect_BoostAttackDamage()
    {
        Assert.AreEqual(Constants.Effects.BoostAttackDamage, "effect.boost.attack.damage");
    }
    [Test]
    public void Effect_BoostAttackSpeed()
    {
        Assert.AreEqual(Constants.Effects.BoostAttackSpeed, "effect.boost.attack.speed");
    }
    [Test]
    public void Effect_BoostMovementSpeed()
    {
        Assert.AreEqual(Constants.Effects.BoostMovementSpeed, "effect.boost.movement.speed");
    }
    [Test]
    public void Effect_EngageEnemy()
    {
        Assert.AreEqual(Constants.Effects.EngageEnemy, "effect.engage.enemy");
    }
    [Test]
    public void Effect_DealDamage()
    {
        Assert.AreEqual(Constants.Effects.DealDamage, "effect.damage");
    }
    [Test]
    public void Effect_ProtectiveTower()
    {
        Assert.AreEqual(Constants.Effects.ProtectiveTower, "effect.tower");
    }
    [Test]
    public void Effect_ProtectiveWall()
    {
        Assert.AreEqual(Constants.Effects.ProtectiveWall, "effect.wall");
    }
}
