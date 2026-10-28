using System.Collections.Generic;

public class PlayerUploadStatisticsRequest : Request
{
	protected int killCash;

	protected int pickupCash;

	protected int pickupEnegy;

	protected int comboCash;

	protected int bossCash;

	protected int bossMithril;

	protected byte remotePlayerCount;

	protected List<int> remotePlayerIds;

	public PlayerUploadStatisticsRequest(int kc, int pc, int pe, int cc, int bc, int bm)
	{
		requestID = 122;
		killCash = kc;
		pickupCash = pc;
		pickupEnegy = pe;
		comboCash = cc;
		bossCash = bc;
		bossMithril = bm;
		remotePlayerCount = 0;
		if (!GameApp.GetInstance().GetGameMode().IsCoopMode())
		{
			return;
		}
		remotePlayerIds = new List<int>();
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null)
		{
			return;
		}
		foreach (RemotePlayer remotePlayer in gameWorld.GetRemotePlayers())
		{
			if (remotePlayer != null)
			{
				remotePlayerCount++;
				remotePlayerIds.Add(remotePlayer.GetChannelID());
			}
		}
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(25 + 4 * remotePlayerCount);
		bytesBuffer.AddInt(killCash);
		bytesBuffer.AddInt(pickupCash);
		bytesBuffer.AddInt(pickupEnegy);
		bytesBuffer.AddInt(comboCash);
		bytesBuffer.AddInt(bossCash);
		bytesBuffer.AddInt(bossMithril);
		bytesBuffer.AddByte(remotePlayerCount);
		for (int i = 0; i < remotePlayerCount; i++)
		{
			bytesBuffer.AddInt(remotePlayerIds[i]);
		}
		return bytesBuffer;
	}
}
