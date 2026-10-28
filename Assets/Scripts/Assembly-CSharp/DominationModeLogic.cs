using UnityEngine;

public class DominationModeLogic : VsModeLogic
{
	public static float pointBaseSpeed = 5f;

	private bool[] isNeutrals;

	private TeamName[] mPointControllers;

	private PointScript[] mPoints;

	private Timer addScoreTimer;

	private bool findSuccess;

	public byte PointCount { get; set; }

	public override void Init()
	{
		addScoreTimer = new Timer(1f, true);
		PointCount = 3;
		Scores = new float[2];
		isNeutrals = new bool[PointCount];
		for (int i = 0; i < PointCount; i++)
		{
			isNeutrals[i] = true;
		}
		mPointControllers = new TeamName[PointCount];
		for (int j = 0; j < PointCount; j++)
		{
			mPointControllers[j] = TeamName.Blue;
		}
		mPoints = new PointScript[PointCount];
		FindPoints();
	}

	public override void SetPointController(byte pointID, TeamName teamName, bool isNeutral = false)
	{
		if (pointID >= 0 && pointID < PointCount)
		{
			mPointControllers[pointID] = teamName;
			if (mPoints[pointID] != null)
			{
				isNeutrals[pointID] = isNeutral;
				mPoints[pointID].mBalance = 0f;
				mPoints[pointID].isNeutral = isNeutral;
				mPoints[pointID].ClearCatchPointMenber();
				mPoints[pointID].SetController(teamName);
				mPoints[pointID].SetTimeRun();
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
		for (int i = 0; i < PointCount; i++)
		{
			if (!isNeutrals[i])
			{
				Scores[(int)mPointControllers[i]] += pointBaseSpeed;
				GameApp.GetInstance().GetGameWorld().BattleInfo.TeamScores[(int)mPointControllers[i]] = GetScore((byte)mPointControllers[i]);
				if (Scores[(int)mPointControllers[i]] > 4000f)
				{
					Scores[(int)mPointControllers[i]] = 4000f;
					StopTime();
				}
			}
		}
	}

	public override void ClearAll()
	{
		for (int i = 0; i < PointCount; i++)
		{
			isNeutrals[i] = true;
		}
		mPointControllers = new TeamName[PointCount];
		for (int j = 0; j < PointCount; j++)
		{
			mPointControllers[j] = TeamName.Blue;
		}
		mPoints = new PointScript[PointCount];
		for (int k = 0; k < PointCount; k++)
		{
			if (mPoints[k] != null)
			{
				mPoints[k].mBalance = 0f;
				mPoints[k].isNeutral = true;
			}
		}
		Scores[0] = 0f;
		Scores[1] = 0f;
		isTimeRun = true;
	}

	public void FindPoints()
	{
		GameObject gameObject = GameObject.Find("__KingOfTheHill");
		if (mPoints[0] != null && findSuccess)
		{
			return;
		}
		for (int i = 0; i < PointCount; i++)
		{
			GameObject gameObject2 = gameObject.transform.Find("Cube" + (i + 1)).gameObject;
			if (gameObject2 != null)
			{
				Debug.Log("has point");
				findSuccess = true;
				mPoints[i] = gameObject2.GetComponent<PointScript>();
				mPoints[i].enabled = true;
				mPoints[i].SetModeLogic(this);
				mPoints[i].Id = (byte)i;
				gameObject.transform.Find("Point" + (i + 1)).gameObject.GetComponent<Renderer>().enabled = true;
				GameApp.GetInstance().GetGameWorld().AddRadarPoint(TagName.PVP_DOMINATION_POINT, "Point" + i, mPoints[i].transform);
				continue;
			}
			Debug.Log("No Point");
			findSuccess = false;
			break;
		}
	}

	public override void SendBattleInfoToNewPlayer(int newPlayerId)
	{
		SendPointInfoRequest request = new SendPointInfoRequest(PointCount, newPlayerId, isNeutrals, mPointControllers, GetScoreInFloat(0), GetScoreInFloat(1), mPoints);
		GameApp.GetInstance().GetNetworkManager().SendRequest(request);
	}

	public override void ChangeMainPointController(byte id, TeamName team, bool neutral, float balance)
	{
		if (PointCount == 0)
		{
			FindPoints();
		}
		if (id >= 0 && id < PointCount)
		{
			mPointControllers[id] = team;
			isNeutrals[id] = neutral;
			if (mPoints[id] == null)
			{
				FindPoints();
			}
			if (mPoints[id] != null)
			{
				mPoints[id].mBalance = balance;
				mPoints[id].isNeutral = neutral;
				mPoints[id].SetController(team);
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

	public PointScript GetPoint(int index)
	{
		if (index < mPoints.Length)
		{
			return mPoints[index];
		}
		return null;
	}

	public override byte GetPointCountForecast(TeamName team, int pointId, TeamName newTeam)
	{
		byte b = 0;
		for (int i = 0; i < PointCount; i++)
		{
			if (i == pointId)
			{
				if (newTeam == team)
				{
					b++;
				}
			}
			else if (!isNeutrals[i] && team == mPointControllers[i])
			{
				b++;
			}
		}
		return b;
	}
}
