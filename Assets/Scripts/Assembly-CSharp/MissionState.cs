using UnityEngine;

public class MissionState
{
	public static string MissionStateVersion = "104";

	protected EDifficultMode difficultMode;

	protected MissionStateGroup[][] missionStatusArray;

	protected byte mUnlockedStageId;

	protected byte mUnlockedSubStageId;

	public MissionState(EDifficultMode mode)
	{
		difficultMode = mode;
		missionStatusArray = new MissionStateGroup[Global.TOTAL_STAGE][];
		for (int i = 0; i < Global.TOTAL_STAGE; i++)
		{
			missionStatusArray[i] = new MissionStateGroup[Global.TOTAL_SUB_STAGE];
			for (int j = 0; j < Global.TOTAL_SUB_STAGE; j++)
			{
				missionStatusArray[i][j] = new MissionStateGroup();
				missionStatusArray[i][j].StatusArray = new bool[Global.MISSION_SIZE];
				missionStatusArray[i][j].VersionStatusArray = new string[Global.MISSION_SIZE];
			}
		}
	}

	public void Print()
	{
		Debug.Log("Print Mission!!!!!!");
		for (int i = 0; i < Global.MISSION_SIZE; i++)
		{
			Debug.Log("m/value: " + i + "/" + missionStatusArray[0][1][i]);
		}
	}

	public void Init()
	{
		mUnlockedStageId = 0;
		mUnlockedSubStageId = 0;
		if (difficultMode == EDifficultMode.NORMAL)
		{
			mUnlockedSubStageId = 1;
		}
		for (int i = 0; i < Global.TOTAL_STAGE; i++)
		{
			for (int j = 0; j < Global.TOTAL_SUB_STAGE; j++)
			{
				for (int k = 0; k < Global.MISSION_SIZE; k++)
				{
					missionStatusArray[i][j][k] = false;
				}
			}
		}
	}

	public bool GetMissionStatus(int i, int j, int k)
	{
		return missionStatusArray[i][j][k];
	}

	public void SetMissionStatus(int i, int j, int k, bool status)
	{
		missionStatusArray[i][j][k] = status;
		if (status)
		{
			missionStatusArray[i][j].SetVersion(k, MissionStateVersion);
		}
	}

	public string GetMissionVersionStatus(int i, int j, int k)
	{
		return missionStatusArray[i][j].GetVersion(k);
	}

	public void SetMissionVersionStatus(int i, int j, int k, string version)
	{
		missionStatusArray[i][j].SetVersion(k, version);
	}

	public MissionStateGroup[][] GetMissionStatusArray()
	{
		return missionStatusArray;
	}

	public void SetMissionStatusArray(MissionStateGroup[][] missions)
	{
		for (int i = 0; i < missions.Length; i++)
		{
			for (int j = 0; j < missions[0].Length; j++)
			{
				for (int k = 0; k < missions[0][0].Length; k++)
				{
					missionStatusArray[i][j][k] = missions[i][j][k];
				}
			}
		}
	}

	public void SetUnlockedStageId(byte stage)
	{
		mUnlockedStageId = stage;
	}

	public byte GetUnlockedStageId()
	{
		return mUnlockedStageId;
	}

	public void SetUnlockedSubStageId(byte subStage)
	{
		mUnlockedSubStageId = subStage;
	}

	public byte GetUnlockedSubStageId()
	{
		return mUnlockedSubStageId;
	}
}
