public class MissionItemDestruction : Mission
{
	public override void LoadConfig(int i)
	{
		base.LoadConfig(i);
	}

	public override void LevelStart()
	{
		base.LevelStart();
		mProcess = EMissionProcess.NORMAL;
	}

	public override EMissionProcess CheckIfComplete()
	{
		return mProcess;
	}
}
