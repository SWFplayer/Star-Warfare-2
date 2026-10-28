public class SendPointInfoResponse : Response
{
	protected byte pointCount;

	protected bool[] isNeutral;

	protected byte[] mControllerID;

	protected float[] mBalance;

	protected float mTeamBlueScore;

	protected float mTeamRedScore;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		pointCount = bytesBuffer.ReadByte();
		isNeutral = new bool[pointCount];
		mControllerID = new byte[pointCount];
		mBalance = new float[pointCount];
		for (int i = 0; i < pointCount; i++)
		{
			isNeutral[i] = bytesBuffer.ReadBool();
			mControllerID[i] = bytesBuffer.ReadByte();
			mBalance[i] = bytesBuffer.ReadFloat();
		}
		mTeamBlueScore = (float)bytesBuffer.ReadInt() * 1f;
		mTeamRedScore = (float)bytesBuffer.ReadInt() * 1f;
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || (GameApp.GetInstance().GetGameMode().ModePlay != Mode.VS_KOTH && GameApp.GetInstance().GetGameMode().ModePlay != Mode.Domination))
		{
			return;
		}
		VsModeLogic vsBattleLogic = gameWorld.VsBattleLogic;
		if (vsBattleLogic != null)
		{
			for (int i = 0; i < pointCount; i++)
			{
				vsBattleLogic.ChangeMainPointController((byte)i, (TeamName)mControllerID[i], isNeutral[i], mBalance[i]);
			}
			vsBattleLogic.SetInfo(mTeamBlueScore, mTeamRedScore);
		}
	}
}
