using System.Collections.Generic;
using UnityEngine;

public class MissionKillEnemiesArea : Mission
{
	private bool mHasArea;

	private List<byte> mAreaId;

	private List<int> mTargetAmount;

	private bool mHasNPC;

	private Robot NPC;

	private float mIdleTime;

	private float mFireTime;

	private int mMaxHP;

	private int mArrived;

	private Dictionary<byte, GameObject> mAreaList;

	private int TotalAmount;

	private int mRecentKillTotalAmount;

	private int mActionNum;

	private float mDamageScale;

	public override void LoadConfig(int i)
	{
		base.LoadConfig(i);
		GameDataTable dataTable = GetDataTable();
		mHasArea = false;
		mAreaId = new List<byte>();
		mTargetAmount = new List<int>();
		mHasNPC = false;
		mIdleTime = 0f;
		mFireTime = 0f;
		mMaxHP = 0;
		TotalAmount = 0;
		mActionNum = 0;
		for (int j = 0; j < dataTable.Count; j++)
		{
			int num = dataTable.GetInt(j, 0);
			if (num != base.ID)
			{
				continue;
			}
			mHasArea = dataTable.GetBoolean(j, 1);
			mHasNPC = dataTable.GetBoolean(j, 2);
			if (mHasArea)
			{
				int num2 = dataTable.GetInt(j, 3);
				for (int k = 0; k < num2; k++)
				{
					mAreaId.Add((byte)dataTable.GetInt(j, 4 + k * 2));
					mTargetAmount.Add(dataTable.GetInt(j, 5 + k * 2));
					TotalAmount += dataTable.GetInt(j, 5 + k * 2);
				}
			}
			else
			{
				mTargetAmount.Add(dataTable.GetInt(j, 5));
				TotalAmount += dataTable.GetInt(j, 5);
			}
			mActionNum = dataTable.GetInt(j, 10);
			break;
		}
		if (!mHasNPC)
		{
			return;
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
		mAreaList = new Dictionary<byte, GameObject>();
		mArrived = 0;
		mRecentKillTotalAmount = 0;
		mProcess = EMissionProcess.NORMAL;
		if (mHasArea)
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.MISSION_ENEMY_AREA);
			GameObject[] array2 = array;
			foreach (GameObject gameObject in array2)
			{
				int result = 0;
				string text = gameObject.name.Replace(MissionNamePrefix.ENEMY_AREA, string.Empty);
				if (int.TryParse(text, out result))
				{
					if (mAreaId.Contains((byte)result))
					{
						mAreaList.Add((byte)result, gameObject);
					}
					else
					{
						MissionManager.SetMissionGameObjectState(gameObject, false);
					}
				}
				else
				{
					Debug.LogError("Failed to parse id. area = " + text);
				}
			}
			MissionManager.SetMissionGameObjectState(mAreaList[mAreaId[0]], true);
			GameApp.GetInstance().GetGameWorld().AddRadarPoint(TagName.MISSION_ENEMY_AREA, "KillEnemiesArea" + mAreaId[0], mAreaList[mAreaId[0]].transform);
			GameApp.GetInstance().GetGameWorld().AddRadarPoint(TagName.MISSION_PROTECTED_ITEM, "KillEnemiesProtection" + mAreaId[0], mAreaList[mAreaId[0]].transform);
			MissionEnemiesAreaScript component = mAreaList[mAreaId[0]].GetComponent<MissionEnemiesAreaScript>();
			if (mHasNPC)
			{
				NPC = GameApp.GetInstance().GetGameWorld().CreateRobot(ERobotType.DEFENDER);
				NPC.AttackIdleDuration = mIdleTime;
				NPC.AttackDuration = mFireTime;
				NPC.MaxHp = mMaxHP;
				NPC.Hp = NPC.MaxHp;
				NPC.ProtectRadius = component.Distance;
				NPC.MissionObject = mAreaList[mAreaId[0]];
				NPC.SetWeaponDamage(mDamageScale);
			}
			return;
		}
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null)
		{
			Player localPlayer = gameWorld.GetLocalPlayer();
			if (localPlayer != null)
			{
				localPlayer.StayInEnemiesArea = true;
			}
			if (NPC != null)
			{
				NPC.StayInEnemiesArea = true;
			}
		}
	}

	public override EMissionProcess CheckIfComplete()
	{
		if (mTargetAmount.Count == 0)
		{
			Debug.Log(string.Concat(base.Type, " cannot read data from xls."));
			return EMissionProcess.NORMAL;
		}
		if (mProcess == EMissionProcess.NORMAL)
		{
			if (GetKilledCount() - mRecentKillTotalAmount >= mTargetAmount[mArrived])
			{
				mArrived++;
				if (mArrived >= mAreaId.Count)
				{
					mArrived = mAreaId.Count - 1;
					mProcess = EMissionProcess.SUCCESS;
				}
				else
				{
					mRecentKillTotalAmount = GetKilledCount();
					MoveToNextArea();
				}
			}
			else if (GetKilledCount() >= mActionCounter * TotalAmount / mActionNum)
			{
				TriggerAction();
			}
			if (mHasNPC && NPC.Destroyed)
			{
				mProcess = EMissionProcess.FAIL;
			}
		}
		return mProcess;
	}

	public override float GetProgressPercentage()
	{
		if (mIsBossAppeared)
		{
			return 1f;
		}
		int num = 0;
		num = ((mAreaList.Count != 0) ? mTargetAmount[mArrived] : mTargetAmount[0]);
		return (float)(GetKilledCount() - mRecentKillTotalAmount) / (float)num;
	}

	public override string GetProgressText()
	{
		if (mIsBossAppeared)
		{
			return LocalizationManager.GetInstance().GetString("LOC_MISSOION_SPECIAL_BOSS");
		}
		int num = 0;
		num = ((mAreaList.Count != 0) ? mTargetAmount[mArrived] : mTargetAmount[0]);
		return base.SummaryText + " [e4ce00](" + (GetKilledCount() - mRecentKillTotalAmount) + " / " + num + ")[-]";
	}

	private int GetKilledCount()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null)
		{
			Player localPlayer = gameWorld.GetLocalPlayer();
			if (localPlayer != null)
			{
				if (mHasArea)
				{
					return localPlayer.KillsInArea;
				}
				return localPlayer.Kills;
			}
		}
		return 0;
	}

	private void MoveToNextArea()
	{
		MissionManager.SetMissionGameObjectState(mAreaList[mAreaId[mArrived - 1]], false);
		MissionManager.SetMissionGameObjectState(mAreaList[mAreaId[mArrived]], true);
		GameApp.GetInstance().GetGameWorld().RemoveRadarPointByKey("KillEnemiesArea" + mAreaId[mArrived - 1]);
		GameApp.GetInstance().GetGameWorld().RemoveRadarPointByKey("KillEnemiesProtection" + mAreaId[mArrived - 1]);
		GameApp.GetInstance().GetGameWorld().AddRadarPoint(TagName.MISSION_ENEMY_AREA, "KillEnemiesArea" + mAreaId[mArrived], mAreaList[mAreaId[mArrived]].transform);
		GameApp.GetInstance().GetGameWorld().AddRadarPoint(TagName.MISSION_PROTECTED_ITEM, "KillEnemiesProtection" + mAreaId[mArrived], mAreaList[mAreaId[mArrived]].transform);
	}
}
