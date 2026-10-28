public class WeaponAdjuster
{
	public float mOffsetStand;

	public float mOffsetRun;

	public float mAngleHStand;

	public float mAngleHRun;

	public float mAngleVOffset;

	public float mAngleVSclae = 1f;

	public WeaponAdjuster(float offsetStand, float offsetRun)
	{
		mOffsetStand = offsetStand;
		mOffsetRun = offsetRun;
	}

	public WeaponAdjuster(float offsetStand, float offsetRun, float angleHStand, float angleHRun, float angleVOffset, float angleVScale)
	{
		mOffsetStand = offsetStand;
		mOffsetRun = offsetRun;
		mAngleHStand = angleHStand;
		mAngleHRun = angleHRun;
		mAngleVOffset = angleVOffset;
		mAngleVSclae = angleVScale;
	}
}
