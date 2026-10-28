using UnityEngine;

public class MissionDestinationCheckScript : MonoBehaviour
{
	public int Distance = 5;

	public float Time = 1f;

	public GameObject Next;

	public bool NeedShowProcessBar;

	private bool mArrived;

	private Timer mCheckTimer = new Timer();

	private bool mStay;

	private float mStayTime;

	private MissionDestination mMissionDestination;

	public bool Arrived
	{
		get
		{
			return mArrived;
		}
	}

	public int Id { get; set; }

	private void Start()
	{
		mCheckTimer.SetTimer(0.1f, false);
		mArrived = false;
		mStay = false;
		mStayTime = 0f;
	}

	private void Update()
	{
		if (mArrived || !mCheckTimer.Ready())
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
			if (!(mStayTime > Time))
			{
				return;
			}
			mArrived = true;
			mMissionDestination.IncreaseArrivedNumber();
			mMissionDestination.TriggerAction();
			MissionManager.GetInstance().SetLastRefreshTime();
			if (Next != null)
			{
				MissionManager.SetMissionGameObjectState(Next, true);
				Next.SetActive(true);
				MissionDestinationCheckScript component = Next.GetComponent<MissionDestinationCheckScript>();
				if (component != null)
				{
					mMissionDestination.AddRadarPoint(component);
					mMissionDestination.mCurrentPointScript = component;
				}
				mMissionDestination.RemoveRadarPoint(this);
			}
			MissionManager.SetMissionGameObjectState(base.gameObject, false);
			if (MissionDetailsHUD.m_Instance != null)
			{
				MissionDetailsHUD.m_Instance.SetProgressBarActive(false);
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

	public void SetMissionDestination(MissionDestination missionDestination)
	{
		mMissionDestination = missionDestination;
	}
}
