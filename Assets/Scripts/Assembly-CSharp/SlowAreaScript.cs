using System.Collections.Generic;
using UnityEngine;

public class SlowAreaScript : MonoBehaviour
{
	public Player owner;

	public int SkillId;

	public float radius;

	public void Init()
	{
		InvokeRepeating("SendSlowRequest", 0f, 1f);
	}

	private void SendSlowRequest()
	{
		if (GameApp.GetInstance().GetGameWorld() == null)
		{
			return;
		}
		Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (owner != localPlayer)
		{
			return;
		}
		List<int> list = new List<int>();
		List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
		foreach (RemotePlayer item in remotePlayers)
		{
			if (item != null && item.InPlayingState() && !item.IsSameTeam(localPlayer) && Vector3.Distance(base.transform.position, item.GetPosition()) < radius)
			{
				list.Add(item.GetChannelID());
			}
		}
		PlayerBuffRequest request = new PlayerBuffRequest(list, SkillId);
		GameApp.GetInstance().GetNetworkManager().SendRequest(request);
	}
}
