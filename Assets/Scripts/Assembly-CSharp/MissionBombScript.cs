using System.Collections.Generic;
using UnityEngine;

public class MissionBombScript : MonoBehaviour
{
	public int Distance = 2;

	public float Time = 3f;

	public List<MissionEnemyBaseScript> BaseScript = new List<MissionEnemyBaseScript>();

	private Timer mCheckTimer = new Timer();

	private bool mStay;

	private float mStayTime;

	private MissionBaseDestruction mMissionBaseDestruction;

	private void Start()
	{
		mCheckTimer.SetTimer(0.5f, false);
		mStay = false;
		mStayTime = 0f;
	}

	private void Update()
	{
		if (!mCheckTimer.Ready())
		{
			return;
		}
		mCheckTimer.Do();
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || !(null != MissionDetailsHUD.m_Instance))
		{
			return;
		}
		Player localPlayer = gameWorld.GetLocalPlayer();
		if (localPlayer != null && localPlayer.InPlayingState() && null != localPlayer.GetTransform() && (localPlayer.GetTransform().position - base.transform.position).sqrMagnitude < (float)(Distance * Distance))
		{
			if (mStay)
			{
				mStayTime += mCheckTimer.GetInterval();
				MissionDetailsHUD.m_Instance.SetProgressBar(mStayTime / Time);
			}
			else
			{
				mStay = true;
				MissionDetailsHUD.m_Instance.SetProgressBarActive(true);
				MissionDetailsHUD.m_Instance.SetProgressBar(0f);
			}
			if (mStayTime > Time)
			{
				if (BaseScript[mMissionBaseDestruction.GetBaseNumber()] != null)
				{
					MissionManager.SetMissionGameObjectState(BaseScript[mMissionBaseDestruction.GetBaseNumber()].gameObject, true);
					mMissionBaseDestruction.AddMissionEnemyBasePoint(BaseScript[mMissionBaseDestruction.GetBaseNumber()].gameObject);
					mMissionBaseDestruction.mEffectPosition = BaseScript[mMissionBaseDestruction.GetBaseNumber()].transform.position;
				}
				MissionManager.SetMissionGameObjectState(base.gameObject, false);
				GameApp.GetInstance().GetGameWorld().RemoveRadarPointByTransform(base.gameObject.transform);
				MissionDetailsHUD.m_Instance.SetProgressBarActive(false);
				mMissionBaseDestruction.IncreaseBombNumber();
				mMissionBaseDestruction.TriggerAction();
				MissionManager.GetInstance().SetLastRefreshTime();
			}
		}
		else
		{
			mStay = false;
			mStayTime = 0f;
			if (MissionDetailsHUD.m_Instance != null)
			{
				MissionDetailsHUD.m_Instance.SetProgressBarActive(false);
			}
		}
	}

	public void SetMissionBaseDestruction(MissionBaseDestruction missionBaseDestruction)
	{
		mMissionBaseDestruction = missionBaseDestruction;
	}
}
