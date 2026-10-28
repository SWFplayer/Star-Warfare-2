using System.Collections.Generic;
using UnityEngine;

public class MissionProtectionNPC : Mission
{
	private List<byte> mIdList;

	private List<Robot> mProtectedItemList;

	private float mIdleTime;

	private float mFireTime;

	private int mMaxHP;

	private float mDamageScale;

	public override void LoadConfig(int i)
	{
		base.LoadConfig(i);
		mIdleTime = 0f;
		mFireTime = 0f;
		mMaxHP = 0;
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
		dataTable = GetRobotTable();
		for (int l = 0; l < dataTable.Count; l++)
		{
			int num3 = dataTable.GetInt(l, 0);
			if (num3 == base.ID)
			{
				mIdleTime = dataTable.GetFloat(l, 1);
				mFireTime = dataTable.GetFloat(l, 2);
				mMaxHP = dataTable.GetInt(l, 3);
				mDamageScale = dataTable.GetFloat(l, 4);
			}
		}
	}

	public override void LevelStart()
	{
		base.LevelStart();
		mProcess = EMissionProcess.NORMAL;
		mProtectedItemList = new List<Robot>();
		Dictionary<byte, GameObject> dictionary = new Dictionary<byte, GameObject>();
		GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.MISSION_PROTECTED_ITEM);
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			int result = 0;
			string text = gameObject.name.Replace(MissionNamePrefix.PROTECTED_TIME, string.Empty);
			if (int.TryParse(text, out result))
			{
				if (!dictionary.ContainsKey((byte)result))
				{
					dictionary.Add((byte)result, gameObject);
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
				MissionProtectionScript component = dictionary[key].GetComponent<MissionProtectionScript>();
				if (null != component)
				{
					Robot robot = GameApp.GetInstance().GetGameWorld().CreateRobot(ERobotType.PROTECTED_NPC, dictionary[key].transform.position, dictionary[key].transform.rotation);
					robot.OwnerMission = this;
					robot.Name = dictionary[key].name;
					robot.Speed = component.Speed;
					robot.SpeedValues = component.SpeedValues;
					robot.CanMove = true;
					robot.MaxHp = mMaxHP;
					robot.Hp = robot.MaxHp;
					robot.PathPoints = component.PathPoints;
					robot.IsLeader = false;
					robot.NPCIdleDuration = component.WaitTime;
					robot.AttackIdleDuration = mIdleTime;
					robot.AttackDuration = mFireTime;
					robot.SetWeaponDamage(mDamageScale);
					GameApp.GetInstance().GetGameWorld().AddProtectedNPC(robot);
					mProtectedItemList.Add(robot);
					GameApp.GetInstance().GetGameWorld().AddRadarPoint(TagName.MISSION_PROTECTED_ITEM, "PortectedItem" + j, robot.GetTransform());
				}
				else
				{
					Debug.LogError("Failed to find MissionProtectionScript in " + dictionary[key].name);
				}
			}
			else
			{
				Debug.LogError("Failed to find Destination GameObject ID = " + mIdList[j]);
			}
		}
		if (mProtectedItemList.Count > 0)
		{
			mProtectedItemList[0].IsLeader = true;
		}
	}

	public override EMissionProcess CheckIfComplete()
	{
		if (mProcess == EMissionProcess.NORMAL)
		{
			bool flag = true;
			foreach (Robot mProtectedItem in mProtectedItemList)
			{
				if (!mProtectedItem.Arrived)
				{
					flag = false;
				}
			}
			if (flag)
			{
				mProcess = EMissionProcess.SUCCESS;
			}
		}
		if (mProcess == EMissionProcess.NORMAL)
		{
			foreach (Robot mProtectedItem2 in mProtectedItemList)
			{
				if (mProtectedItem2.Destroyed)
				{
					Debug.Log("FAIL");
					mProcess = EMissionProcess.FAIL;
				}
			}
		}
		return mProcess;
	}

	public override float GetProgressPercentage()
	{
		return mProtectedItemList[0].GetCurrentIndexPercent();
	}
}
