public class TwinRPG : RPG
{
	protected override EffectPoolType GetRocketEffect()
	{
		return EffectPoolType.W15_ROCKET;
	}

	public override void GunOn()
	{
		base.GunOn();
		PlayIdleAnimGunOn();
	}
}
