using System.Collections.Generic;
using UnityEngine;

public class MainScript : MonoBehaviour
{
	protected NetworkManager networkMgr;

	public bool bInit;

	private void Start()
	{
		bInit = false;
	}

	public void Init()
	{
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			networkMgr = GameApp.GetInstance().GetNetworkManager();
		}
		GameApp.GetInstance().CreateGameWorld();
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
		}
		bInit = true;
	}

	private void Update()
	{
		if (bInit)
		{
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				TimeManager.GetInstance().Loop();
				networkMgr.ProcessReceivedPackets();
			}
			GameApp.GetInstance().GetGameWorld().Loop(Time.deltaTime);
		}
	}

	private void LateUpdate()
	{
		if (bInit)
		{
			GameApp.GetInstance().GetGameWorld().LateLoop(Time.deltaTime);
		}
	}

	private void OnDestroy()
	{
		if (GameApp.GetInstance().GetGameWorld() == null || GameApp.GetInstance().GetGameWorld().State == GameState.SwitchBossLevel)
		{
			return;
		}
		if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer() != null)
		{
			Object.Destroy(GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.GetObject());
		}
		List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
		if (remotePlayers == null)
		{
			return;
		}
		foreach (RemotePlayer item in remotePlayers)
		{
			if (item != null)
			{
				Object.Destroy(item.GetObject());
			}
		}
	}
}
