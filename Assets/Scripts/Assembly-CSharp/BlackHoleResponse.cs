using UnityEngine;

public class BlackHoleResponse : Response
{
	private int ownerID;

	private BlackHole blackhole;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		blackhole = new BlackHole();
		blackhole.x = (float)bytesBuffer.ReadShort() / 10f;
		blackhole.z = (float)bytesBuffer.ReadShort() / 10f;
		blackhole.radius = (int)bytesBuffer.ReadByte();
		blackhole.speed = (int)bytesBuffer.ReadByte();
		ownerID = bytesBuffer.ReadInt();
		blackhole.BlackHoleDuringTime = (int)bytesBuffer.ReadByte();
		blackhole.isAttract = bytesBuffer.ReadByte() == 1;
	}

	public override void ProcessLogic()
	{
		Debug.Log("BlackHole!");
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null)
		{
			return;
		}
		blackhole.owner = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(ownerID);
		if (blackhole.owner == null || (gameWorld.State != GameState.Playing && GameApp.GetInstance().GetGameMode().IsVSMode()))
		{
			return;
		}
		LocalPlayer localPlayer = gameWorld.GetLocalPlayer();
		blackhole.BlackHoleStartTime = Time.time;
		if (localPlayer.InPlayingState())
		{
			if (localPlayer.Team != blackhole.owner.Team && GameApp.GetInstance().GetGameMode().IsTeamMode())
			{
				localPlayer.AddBlackHole(blackhole);
			}
			else if (!GameApp.GetInstance().GetGameMode().IsTeamMode() && GameApp.GetInstance().GetGameMode().IsVSMode())
			{
				localPlayer.AddBlackHole(blackhole);
			}
		}
	}
}
