using System.Collections.Generic;
using UnityEngine;

public class MissionDestination : Mission
{
	private MissionDestinationCheckScript mLastPointScript;

	public MissionDestinationCheckScript mCurrentPointScript;

	private List<byte> mIdList;

	private List<MissionDestinationCheckScript> mScriptList;

	private int mTotalNumber;

	private int mArrivedNumber;

	private GameObject instructObject;

	public override void LoadConfig(int i)
	{
		base.LoadConfig(i);
		GameDataTable dataTable = GetDataTable();
		mIdList = new List<byte>();
		for (int j = 0; j < dataTable.Count; j++)
		{
			int num = dataTable.GetInt(j, 0);
			if (num != base.ID)
			{
				continue;
			}
			int num2 = dataTable.GetInt(j, 1);
			for (int k = 0; k < num2; k++)
			{
				byte item = (byte)dataTable.GetInt(j, 2 + k);
				if (!mIdList.Contains(item))
				{
					mIdList.Add(item);
				}
			}
			break;
		}
	}

	public override void LevelStart()
	{
		base.LevelStart();
		mProcess = EMissionProcess.NORMAL;
		mTotalNumber = 0;
		mArrivedNumber = 0;
		Dictionary<byte, GameObject> dictionary = new Dictionary<byte, GameObject>();
		GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.MISSION_DESTINATION);
		GameObject gameObject = null;
		mScriptList = new List<MissionDestinationCheckScript>();
		GameObject[] array2 = array;
		foreach (GameObject gameObject2 in array2)
		{
			int result = 0;
			string text = gameObject2.name.Replace(MissionNamePrefix.DESTINATION, string.Empty);
			if (int.TryParse(text, out result))
			{
				MissionManager.SetMissionGameObjectState(gameObject2, false);
				if (!dictionary.ContainsKey((byte)result))
				{
					dictionary.Add((byte)result, gameObject2);
				}
				else
				{
					Debug.LogError("Duplicated Destination ID = " + result);
				}
			}
			else
			{
				Debug.LogError("Failed to parse id. p = " + text);
			}
		}
		for (int j = 0; j < mIdList.Count; j++)
		{
			byte key = mIdList[j];
			if (dictionary.ContainsKey(key))
			{
				MissionDestinationCheckScript component = dictionary[key].GetComponent<MissionDestinationCheckScript>();
				component.Id = mTotalNumber;
				component.SetMissionDestination(this);
				mTotalNumber++;
				if (gameObject == null)
				{
					MissionManager.SetMissionGameObjectState(dictionary[key], true);
					AddRadarPoint(component);
					mCurrentPointScript = component;
				}
				else
				{
					MissionDestinationCheckScript component2 = gameObject.GetComponent<MissionDestinationCheckScript>();
					if (null != component2)
					{
						component2.Next = dictionary[key];
					}
				}
				if (j == mIdList.Count - 1)
				{
					mLastPointScript = component;
				}
				gameObject = dictionary[mIdList[j]];
			}
			else
			{
				Debug.LogError("Failed to find Destination GameObject ID = " + mIdList[j]);
			}
		}
		mMissionRefreshInterval = (float)base.TimeLimitation / (float)mTotalNumber;
		instructObject = null;
		if (GameApp.GetInstance().GetUserState().GetSelectedStageIndexGeneral() == 0 && GameApp.GetInstance().GetUserState().GetSelectedSubStageIndexGeneral() == 1 && GameApp.GetInstance().GetUserState().GetSelectedDifficultMode() == EDifficultMode.NORMAL)
		{
			Object original = Resources.Load(EffectName.MISSION_INSTRUCTION);
			LocalPlayer localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			instructObject = Object.Instantiate(original, localPlayer.GetPosition() + Vector3.up * 2.5f, Quaternion.identity) as GameObject;
			instructObject.transform.parent = localPlayer.GetTransform();
			localPlayer.InstructionObject = instructObject.transform;
			localPlayer.InstructionTarget = mCurrentPointScript.transform;
			instructObject.SetActive(false);
		}
	}

	public override EMissionProcess CheckIfComplete()
	{
		if (mProcess == EMissionProcess.NORMAL)
		{
			if (mLastPointScript != null && mLastPointScript.Arrived)
			{
				mProcess = EMissionProcess.SUCCESS;
				if (instructObject != null)
				{
					Object.Destroy(instructObject);
				}
			}
			else if (instructObject != null)
			{
				GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
					.InstructionTarget = mCurrentPointScript.transform;
			}
		}
		return mProcess;
	}

	public override float GetProgressPercentage()
	{
		return (float)mArrivedNumber / (float)mTotalNumber;
	}

	public override string GetProgressText()
	{
		return base.SummaryText + " [e4ce00](" + mArrivedNumber + " / " + mTotalNumber + ")[-]";
	}

	public void IncreaseArrivedNumber()
	{
		mArrivedNumber++;
	}

	public void AddRadarPoint(MissionDestinationCheckScript missionDestinationCheckScript)
	{
		GameApp.GetInstance().GetGameWorld().AddRadarPoint(TagName.MISSION_DESTINATION, "Destination" + missionDestinationCheckScript.Id, missionDestinationCheckScript.gameObject.transform, true);
	}

	public void RemoveRadarPoint(MissionDestinationCheckScript missionDestinationCheckScript)
	{
		GameApp.GetInstance().GetGameWorld().RemoveRadarPointByTransform(missionDestinationCheckScript.transform);
	}

	public override void ReFreshMissionTarget()
	{
		base.ReFreshMissionTarget();
		if (UIRadar.mInstance != null && MissionDetailsHUD.m_Instance != null)
		{
			UIRadar.mInstance.RefreshTargetEffect("Destination" + mCurrentPointScript.Id, mCurrentPointScript.gameObject.transform.position);
			MissionDetailsHUD.m_Instance.RefreshMission();
		}
		if (instructObject != null)
		{
			instructObject.SetActive(true);
		}
	}
}
