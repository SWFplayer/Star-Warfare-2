using System.Collections.Generic;

public class SwVoidGetRewardAfterGameRequest : Request
{
	private int mSwVoidId;

	private EDifficultMode mMode;

	private byte mStageId;

	private byte mSubStageId;

	private int mTotalKill;

	private bool mIsWin;

	private byte mBoss1Type;

	private byte mBoss2Type;

	private byte remotePlayerCount;

	private List<int> remotePlayerIds;

	public SwVoidGetRewardAfterGameRequest(int swVoidId, EDifficultMode mode, int totalKill, byte stageId, byte subStageId, bool isWin)
	{
		requestID = 65;
		mSwVoidId = swVoidId;
		mMode = mode;
		mStageId = stageId;
		mSubStageId = subStageId;
		mTotalKill = totalKill;
		mIsWin = isWin;
		mBoss1Type = GameApp.GetInstance().GetUserState().GetBoss1ByteType();
		mBoss2Type = GameApp.GetInstance().GetUserState().GetBoss2ByteType();
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
		BytesBuffer bytesBuffer = new BytesBuffer(15 + 4 * remotePlayerCount);
		bytesBuffer.AddInt(mSwVoidId);
		bytesBuffer.AddByte((byte)mMode);
		bytesBuffer.AddByte(mStageId);
		bytesBuffer.AddByte(mSubStageId);
		bytesBuffer.AddInt(mTotalKill);
		bytesBuffer.AddBool(mIsWin);
		bytesBuffer.AddByte(mBoss1Type);
		bytesBuffer.AddByte(mBoss2Type);
		bytesBuffer.AddByte(remotePlayerCount);
		for (int i = 0; i < remotePlayerCount; i++)
		{
			bytesBuffer.AddInt(remotePlayerIds[i]);
		}
		return bytesBuffer;
	}
}
