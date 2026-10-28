public class MissionEmpty : Mission
{
	public override void LoadConfig(int i)
	{
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
