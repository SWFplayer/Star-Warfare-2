using UnityEngine;

public class KOTHModeLogic : VsModeLogic
{
	public static float pointBaseSpeed = 5f;

	private bool isNeutral;

	private TeamName mPointController;

	private PointScript mMainPoint;

	private Timer addScoreTimer;

	private bool findSuccess;

	public override void Init()
	{
		addScoreTimer = new Timer(1f, true);
		Scores = new float[2];
		isNeutral = true;
		FindMainPoint();
	}

	public override void SetPointController(byte pointID, TeamName teamName, bool isNeutral = false)
	{
		if (pointID == 0)
		{
			mPointController = teamName;
			if (mMainPoint != null)
			{
				this.isNeutral = isNeutral;
				mMainPoint.mBalance = 0f;
				mMainPoint.isNeutral = false;
				mMainPoint.ClearCatchPointMenber();
				mMainPoint.SetController(mPointController);
				mMainPoint.SetTimeRun();
			}
		}
	}

	public override void Update()
	{
		if (!isTimeRun || !addScoreTimer.Ready())
		{
			return;
		}
		addScoreTimer.Do();
		if (!isNeutral)
		{
			Scores[(int)mPointController] += pointBaseSpeed;
			GameApp.GetInstance().GetGameWorld().BattleInfo.TeamScores[(int)mPointController] = GetScore((byte)mPointController);
			if (Scores[(int)mPointController] > 1500f)
			{
				Scores[(int)mPointController] = 1500f;
				StopTime();
			}
		}
	}

	private void FindMainPoint()
	{
		GameObject gameObject = GameObject.Find("__KingOfTheHill");
		GameObject gameObject2 = gameObject.transform.Find("Cube").gameObject;
		if (!(mMainPoint != null) || !findSuccess)
		{
			if (gameObject2 != null)
			{
				Debug.Log("has point");
				findSuccess = true;
				mMainPoint = gameObject2.GetComponent<PointScript>();
				mMainPoint.enabled = true;
				mMainPoint.SetModeLogic(this);
				gameObject.transform.Find("Point").gameObject.GetComponent<Renderer>().enabled = true;
				GameApp.GetInstance().GetGameWorld().AddRadarPoint(TagName.PVP_DOMINATION_POINT, "Point0", mMainPoint.transform);
			}
			else
			{
				Debug.Log("No point");
				findSuccess = false;
			}
		}
	}

	public override void ChangeMainPointController(byte id, TeamName team, bool neutral, float balance)
	{
		if (id == 0)
		{
			mPointController = team;
			isNeutral = neutral;
			if (mMainPoint == null)
			{
				FindMainPoint();
			}
			if (mMainPoint != null)
			{
				mMainPoint.mBalance = balance;
				mMainPoint.isNeutral = isNeutral;
				mMainPoint.SetController(team);
			}
		}
	}

	public override void SetInfo(float blueScore, float redScore)
	{
		Scores[0] = blueScore;
		Scores[1] = redScore;
		GameApp.GetInstance().GetGameWorld().BattleInfo.TeamScores[0] = GetScore(0);
		GameApp.GetInstance().GetGameWorld().BattleInfo.TeamScores[1] = GetScore(1);
	}

	public override void ClearAll()
	{
		isNeutral = true;
		mMainPoint.mBalance = 0f;
		mMainPoint.isNeutral = true;
		Scores[0] = 0f;
		Scores[1] = 0f;
		isTimeRun = true;
	}

	public override void Restart()
	{
		base.Restart();
		isNeutral = true;
	}

	public override void SendBattleInfoToNewPlayer(int newPlayerId)
	{
		SendPointInfoRequest request = new SendPointInfoRequest(newPlayerId, isNeutral, mPointController, GetScoreInFloat(0), GetScoreInFloat(1), mMainPoint.mBalance);
		GameApp.GetInstance().GetNetworkManager().SendRequest(request);
	}

	public PointScript GetPoint()
	{
		return mMainPoint;
	}

	public override byte GetPointCountForecast(TeamName team, int pointId, TeamName newTeam)
	{
		if (team == newTeam)
		{
			return 1;
		}
		return 0;
	}
}
