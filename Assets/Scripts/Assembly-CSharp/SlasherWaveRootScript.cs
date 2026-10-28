public class SlasherWaveRootScript : BaseLaserScript
{
	public SlasherWaveScript[] Fires;

	public new float KnockSpeed;

	public HitCheck mHitCheck = new HitCheck();

	private void Start()
	{
		mHitCheck.mTransform = base.gameObject.transform;
	}
}
