using UnityEngine;

internal class PlayerFlashedResponse : Response
{
	private float alpha;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		alpha = (float)(int)bytesBuffer.ReadByte() / 100f;
	}

	public override void ProcessLogic()
	{
		if (GameApp.GetInstance().GetGameWorld() != null)
		{
			Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if (localPlayer != null && localPlayer.InPlayingState())
			{
				Camera.main.gameObject.GetComponentInChildren<ScreenFlashScript>().Flash(alpha);
			}
		}
	}
}
