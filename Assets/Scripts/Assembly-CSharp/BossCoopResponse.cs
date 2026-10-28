public class BossCoopResponse : Response
{
	public BytesBuffer mData;

	public override void ReadData(byte[] data)
	{
		mData = new BytesBuffer(data);
	}

	public override void ProcessLogic()
	{
		GameApp.GetInstance().GetGameWorld().GetBossCoop()
			.DoResponse(mData);
	}
}
