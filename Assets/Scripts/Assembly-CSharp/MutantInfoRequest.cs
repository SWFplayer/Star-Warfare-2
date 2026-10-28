public class MutantInfoRequest : Request
{
	protected float playerBossTimeToWin;

	protected int playerChannelId;

	public MutantInfoRequest(float _playerBossTimeToWin, int _playerChannelId)
	{
		requestID = 259;
		playerBossTimeToWin = _playerBossTimeToWin;
		playerChannelId = _playerChannelId;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(8);
		bytesBuffer.AddFloat(playerBossTimeToWin);
		bytesBuffer.AddInt(playerChannelId);
		return bytesBuffer;
	}
}
