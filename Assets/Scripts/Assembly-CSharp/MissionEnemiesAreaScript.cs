using System.Collections.Generic;
using UnityEngine;

public class MissionEnemiesAreaScript : MonoBehaviour
{
	public int Distance = 10;

	private Timer mCheckTimer = new Timer();

	private Robot NPC;

	private void Start()
	{
		mCheckTimer.SetTimer(0.5f, false);
	}

	private void Update()
	{
		if (!mCheckTimer.Ready())
		{
			return;
		}
		mCheckTimer.Do();
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null)
		{
			return;
		}
		if (NPC == null)
		{
			List<Robot> robotList = gameWorld.GetRobotList();
			foreach (Robot item in robotList)
			{
				if (item.GetRobotType() == ERobotType.DEFENDER)
				{
					NPC = item;
				}
			}
		}
		if (NPC != null && null != NPC.GetTransform())
		{
			NPC.StayInEnemiesArea = (((NPC.GetTransform().position - base.transform.position).sqrMagnitude < (float)(Distance * Distance)) ? true : false);
		}
		Player localPlayer = gameWorld.GetLocalPlayer();
		if (localPlayer != null && null != localPlayer.GetTransform())
		{
			localPlayer.StayInEnemiesArea = (((localPlayer.GetTransform().position - base.transform.position).sqrMagnitude < (float)(Distance * Distance)) ? true : false);
		}
	}
}
