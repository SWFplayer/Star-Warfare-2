using UnityEngine;

internal class PlayerHitPlayerResponse : Response
{
	protected int playerID;

	protected int m_hp;

	protected bool block;

	protected bool criticalDamage;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		playerID = bytesBuffer.ReadInt();
		m_hp = bytesBuffer.ReadInt();
		block = bytesBuffer.ReadBool();
		criticalDamage = bytesBuffer.ReadBool();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || gameWorld.State != GameState.Playing)
		{
			return;
		}
		Player localPlayer = gameWorld.GetLocalPlayer();
		if (localPlayer == null)
		{
			return;
		}
		int channelID = Lobby.GetInstance().GetChannelID();
		if (playerID == channelID)
		{
			if (block)
			{
				if (!localPlayer.IsBoss)
				{
					localPlayer.ShowDefent();
				}
			}
			else
			{
				localPlayer.UnderAttackSetHP(m_hp);
			}
			return;
		}
		RemotePlayer remotePlayerByUserID = gameWorld.GetRemotePlayerByUserID(playerID);
		if (remotePlayerByUserID == null)
		{
			return;
		}
		if (block)
		{
			if (!remotePlayerByUserID.IsBoss)
			{
				remotePlayerByUserID.ShowDefent();
			}
			return;
		}
		remotePlayerByUserID.UnderAttackSetHP(m_hp);
		if (criticalDamage)
		{
			remotePlayerByUserID.CreateDeadBlood();
		}
		else if (remotePlayerByUserID.GetLastBloodEffectTimer().Ready())
		{
			gameWorld.GetHitBloodPool().CreateObject(remotePlayerByUserID.GetTransform().position + Vector3.up * 1.5f, Vector3.zero, Quaternion.identity);
			remotePlayerByUserID.GetLastBloodEffectTimer().Do();
		}
	}

	public override void ProcessRobotLogic(UIRobot robot)
	{
	}
}
