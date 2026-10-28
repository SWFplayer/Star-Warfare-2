using System;
using UnityEngine;

public class BattlePickup
{
	private enum EPickUpColumn
	{
		ID = 0,
		SKILL_INFO_ID = 1,
		TYPE = 2
	}

	public enum EPickupType
	{
		PICKUP = 0,
		PRESENT = 1
	}

	private SkillGroup mSkillGroup;

	private byte id;

	private byte rowId;

	private short skillId;

	private Player mPlayer;

	private GameObject effectObj;

	private Timer effectTimer;

	private EPickupType mType;

	public void Init(byte id, Player player)
	{
		this.id = id;
		mPlayer = player;
		rowId = (byte)(id - 1);
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PICK_UP, DataSheetName.PICK_UP);
		skillId = gameDataTable.GetShort(rowId, 1);
		mType = (EPickupType)(int)Enum.Parse(typeof(EPickupType), gameDataTable.GetString(rowId, 2));
		LoadConfig();
		InitEffect();
	}

	public static EPickupType GetPickupType(int id)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PICK_UP, DataSheetName.PICK_UP);
		int row = id - 1;
		return (EPickupType)(int)Enum.Parse(typeof(EPickupType), gameDataTable.GetString(row, 2));
	}

	public void LoadConfig()
	{
		mSkillGroup = new SkillGroup();
		mSkillGroup.source = "pick up " + id;
		mSkillGroup.InitSkillInfo(skillId);
	}

	public void TakeOff()
	{
		mSkillGroup.UninitFromPlayerSkill();
		if (effectObj != null)
		{
			effectObj.SetActive(false);
		}
	}

	public void TakeOn(PlayerSkill playerSkill)
	{
		mSkillGroup.InitToPlayerSkill(playerSkill);
		foreach (AdvancedSkill advancedSkill in mSkillGroup.GetAdvancedSkills())
		{
			advancedSkill.Active();
		}
		if (effectObj != null)
		{
			effectObj.SetActive(true);
			effectTimer.Do();
		}
	}

	public void SharePickup(PlayerSkill playerSkill, float shareRate)
	{
		mSkillGroup.InitToPlayerSkill(playerSkill);
		foreach (AdvancedSkill advancedSkill in mSkillGroup.GetAdvancedSkills())
		{
			if (advancedSkill.GetExternalId() == 3001 || advancedSkill.GetExternalId() == 3002)
			{
				float num = advancedSkill.GetSpecialSkillValues()[0];
				float num2 = advancedSkill.GetSpecialSkillValues()[1];
				advancedSkill.SetSpecialSkillValue("[" + num + "," + num2 * shareRate + "]");
			}
			else
			{
				float num3 = advancedSkill.GetOffTriggerValues()[0];
				advancedSkill.SetOffTriggerValues((num3 * shareRate).ToString(), string.Empty);
			}
			advancedSkill.Active();
		}
		SetEffectTimer();
		if (effectObj != null)
		{
			effectObj.SetActive(true);
			effectTimer.Do();
		}
	}

	public void CheckTimer()
	{
		if (effectObj != null && effectTimer != null && effectTimer.Ready())
		{
			effectObj.SetActive(false);
		}
	}

	private void InitEffect()
	{
		if (mPlayer != null && !mPlayer.IsBoss)
		{
			GameObject gameObject = null;
			switch (id)
			{
			case 3:
				gameObject = Resources.Load("SW2_Effect/ENERGY CLIP") as GameObject;
				effectObj = UnityEngine.Object.Instantiate(gameObject, mPlayer.GetTransform().Find(BoneName.Weapon).transform.position, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform().Find(BoneName.Weapon).transform;
				break;
			case 4:
				gameObject = Resources.Load("SW2_Effect/HYPER CLIP") as GameObject;
				effectObj = UnityEngine.Object.Instantiate(gameObject, mPlayer.GetTransform().Find(BoneName.Weapon).transform.position, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform().Find(BoneName.Weapon).transform;
				break;
			case 5:
				gameObject = Resources.Load("SW2_Effect/BULLET SHIELD") as GameObject;
				effectObj = UnityEngine.Object.Instantiate(gameObject, mPlayer.GetTransform().position + Vector3.up * 0.8f, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform();
				break;
			case 6:
				gameObject = Resources.Load("SW2_Effect/ENERGY SHIELD") as GameObject;
				effectObj = UnityEngine.Object.Instantiate(gameObject, mPlayer.GetTransform().position + Vector3.up * 0.8f, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform();
				break;
			case 7:
				gameObject = Resources.Load("SW2_Effect/Explosive Shield") as GameObject;
				effectObj = UnityEngine.Object.Instantiate(gameObject, mPlayer.GetTransform().position + Vector3.up * 0.8f, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform();
				break;
			case 8:
				gameObject = Resources.Load("SW2_Effect/Melee Shield") as GameObject;
				effectObj = UnityEngine.Object.Instantiate(gameObject, mPlayer.GetTransform().position + Vector3.up * 0.8f, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform();
				break;
			case 9:
				gameObject = Resources.Load("SW2_Effect/COUNTER SHIELD") as GameObject;
				effectObj = UnityEngine.Object.Instantiate(gameObject, mPlayer.GetTransform().position + Vector3.up * 0.8f, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform();
				break;
			case 10:
				gameObject = Resources.Load("SW2_Effect/Vampire") as GameObject;
				effectObj = UnityEngine.Object.Instantiate(gameObject, mPlayer.GetTransform().Find(BoneName.Weapon).transform.position, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform().Find(BoneName.Weapon).transform;
				break;
			case 11:
				gameObject = Resources.Load("SW2_Effect/Explosion") as GameObject;
				effectObj = UnityEngine.Object.Instantiate(gameObject, mPlayer.GetTransform().position + Vector3.up * 0.8f, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform();
				break;
			case 12:
				gameObject = Resources.Load("SW2_Effect/Rush") as GameObject;
				effectObj = UnityEngine.Object.Instantiate(gameObject, mPlayer.GetTransform().position, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform();
				break;
			case 13:
				gameObject = Resources.Load("SW2_Effect/Wizard") as GameObject;
				effectObj = UnityEngine.Object.Instantiate(gameObject, mPlayer.GetTransform().position + Vector3.up * 0.8f, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform();
				break;
			case 14:
				gameObject = Resources.Load("SW2_Effect/Rage") as GameObject;
				effectObj = UnityEngine.Object.Instantiate(gameObject, mPlayer.GetTransform().Find(BoneName.Weapon).transform.position, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform().Find(BoneName.Weapon).transform;
				break;
			case 15:
				gameObject = Resources.Load("SW2_Effect/Pickup15") as GameObject;
				effectObj = UnityEngine.Object.Instantiate(gameObject, mPlayer.GetTransform().position, Quaternion.identity) as GameObject;
				effectObj.transform.parent = mPlayer.GetTransform();
				break;
			}
			if (effectObj != null && effectObj.activeSelf)
			{
				effectObj.SetActive(false);
				SetEffectTimer();
			}
		}
	}

	private void SetEffectTimer()
	{
		if (!(effectObj != null))
		{
			return;
		}
		effectTimer = new Timer();
		foreach (AdvancedSkill advancedSkill in mSkillGroup.GetAdvancedSkills())
		{
			if (advancedSkill.GetOffTriggerType() == AdvancedSkill.EOffTriggerType.WAIT_TIME)
			{
				float num = advancedSkill.GetOffTriggerTime();
				if (mPlayer.GetSkills().HasSkill(SkillsType.PICKUP_DURING_INCREASE))
				{
					num += num * mPlayer.GetSkills().GetSkill(SkillsType.PICKUP_DURING_INCREASE);
				}
				effectTimer.SetTimer(num, false);
			}
		}
	}
}
