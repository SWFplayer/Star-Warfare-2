using UnityEngine;

public class HookHitPlayerResponse : Response
{
	protected short x;

	protected short y;

	protected short z;

	protected int playerId;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		x = bytesBuffer.ReadShort();
		y = bytesBuffer.ReadShort();
		z = bytesBuffer.ReadShort();
		playerId = bytesBuffer.ReadInt();
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null)
		{
			Player player = null;
			player = ((gameWorld.GetLocalPlayer() == null || gameWorld.GetLocalPlayer().GetChannelID() != playerId) ? ((Player)gameWorld.GetRemotePlayerByUserID(playerId)) : ((Player)gameWorld.GetLocalPlayer()));
			if (player != null)
			{
				HookForceScript hookForceScript = player.GetObject().AddComponent<HookForceScript>();
				Vector3 pos = new Vector3((float)x / 10f, (float)y / 10f, (float)z / 10f);
				hookForceScript.Init(pos, player);
			}
		}
	}
}
