using UnityEngine;

public class MissionPowerScript : MonoBehaviour
{
	public int Distance = 2;

	public float Time = 3f;

	private Timer mCheckTimer = new Timer();

	private bool mStay;

	private float mStayTime;

	private MissionChargeDestroy mMission;

	public int Id { get; set; }

	private void Start()
	{
		mCheckTimer.SetTimer(0.5f, false);
		mStay = false;
		mStayTime = 0f;
	}

	private void Update()
	{
		if (MissionDetailsHUD.m_Instance == null || !mCheckTimer.Ready())
		{
			return;
		}
		mCheckTimer.Do();
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null)
		{
			return;
		}
		Player localPlayer = gameWorld.GetLocalPlayer();
		if (localPlayer != null && null != localPlayer.GetTransform() && (localPlayer.GetTransform().position - base.transform.position).sqrMagnitude < (float)(Distance * Distance))
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
				localPlayer.isPowerful = true;
				mMission.AddMissionDestructiblePoint();
				GameApp.GetInstance().GetGameWorld().RemoveRadarPointByKey(RadarPointKeyName.POWER_CHARGER);
				MissionDetailsHUD.m_Instance.SetProgressBarActive(false);
				MissionManager.SetMissionGameObjectState(base.gameObject, false);
				mMission.TriggerAction();
			}
		}
		else
		{
			mStay = false;
			mStayTime = 0f;
			MissionDetailsHUD.m_Instance.SetProgressBarActive(false);
		}
	}

	public void SetMission(MissionChargeDestroy mission)
	{
		mMission = mission;
	}
}
