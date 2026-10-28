using System.Collections.Generic;
using UnityEngine;

public class PlayerSkill
{
	public enum ELastSkillType
	{
		NONE = 0,
		SWVOID = 1,
		ACTIVE_SKILL = 2
	}

	protected Player player;

	protected List<Skill> skills;

	protected List<AdvancedSkill> advancedSkillList;

	protected Dictionary<AdvancedSkill.EOnTriggerType, List<AdvancedSkill>> OnTriggerCheckList;

	protected Dictionary<AdvancedSkill.EOffTriggerType, List<AdvancedSkill>> OffTriggerCheckList;

	protected List<PlayerSpecialSkill> specialSkillList = new List<PlayerSpecialSkill>();

	protected List<LiteSpecialSkill> liteSpecialSkillList = new List<LiteSpecialSkill>();

	protected Dictionary<int, AdvancedSkill> mOtherPlayerSkillList = new Dictionary<int, AdvancedSkill>();

	private float HpAutoRecoverTime;

	private ESwVoidStateID lastSwVoidState = ESwVoidStateID.IDLE;

	private int lastActiveSkillId;

	private ELastSkillType lastSkillType;

	public bool needSendServerPlayerSkillCheck;

	private int LastTriggerEnemyKills;

	public GameUnit LastTriggerEnemy;

	private float mMovingTime;

	private float mStandTime;

	private Vector3 mLastPlayerPosition = Vector3.zero;

	private Vector3 mLastStandPlayerPosition = Vector3.zero;

	private bool mIsUpdateIsMovingEnable;

	private Timer mEnemyNearTimer = new Timer(0.2f);

	public bool IsRobot;

	public NetworkManager NetworkManager;

	public void init(OnlineCustomizeData currentCustomize, UserState userState)
	{
		Clear();
		CreateSkills();
		byte[] array = new byte[4];
		byte[] array2 = new byte[4];
		for (int i = 0; i < currentCustomize.mAvatarIdArray.Length; i++)
		{
			Armor armor = userState.GetArmor(i, currentCustomize.mAvatarIdArray[i]);
			armor.TakeOn(this);
			if (i < array.Length)
			{
				array[i] = armor.SetElement;
			}
			if (i < array2.Length)
			{
				array2[i] = armor.SuitId;
			}
		}
		short[] array3 = null;
		array3 = currentCustomize.mItemIdArray;
		if (array3 != null)
		{
			for (int j = 0; j < array3.Length; j++)
			{
				Item itemById = userState.GetItemById(array3[j]);
				if (itemById != null && itemById.GetCount() > 0)
				{
					itemById.TakeOn(this);
				}
			}
		}
		if (GameApp.GetInstance().IsInGameScene())
		{
			List<Item> flaskItemList = userState.GetFlaskItemList();
			foreach (Item item in flaskItemList)
			{
				item.TakeOn(this);
			}
		}
		if (currentCustomize.mPvpSkillIdArray != null)
		{
			for (int k = 0; k < currentCustomize.mPvpSkillIdArray.Length; k++)
			{
				PvpSkill perkById = userState.GetPvpSkillManager().GetPerkById(currentCustomize.mPvpSkillIdArray[k]);
				if (perkById != null)
				{
					perkById.TakeOn(this);
				}
			}
		}
		if (currentCustomize.mAwardSkillIdArray != null)
		{
			for (int l = 0; l < currentCustomize.mAwardSkillIdArray.Length; l++)
			{
				PvpSkill killStreakById = userState.GetPvpSkillManager().GetKillStreakById(currentCustomize.mAwardSkillIdArray[l]);
				if (killStreakById != null)
				{
					killStreakById.TakeOn(this);
				}
			}
		}
		if (currentCustomize.IsPvp)
		{
			int prestige = userState.GetOnlinePlayer().GetPrestige();
			for (int m = 0; m <= prestige; m++)
			{
				PrestigeSkill prestigeSkillById = userState.GetPvpSkillManager().GetPrestigeSkillById(m);
				if (prestigeSkillById != null && prestigeSkillById.GetHasSkill())
				{
					prestigeSkillById.TakeOn(this);
				}
			}
		}
		int mSwVoidId = currentCustomize.mSwVoidId;
		if (mSwVoidId != 0 && userState.swVoidList.ContainsKey(mSwVoidId))
		{
			userState.swVoidList[mSwVoidId].TakeOn(this);
		}
		userState.mCurrentArmorSet.SetElementSets(array, array2);
		userState.mCurrentArmorSet.InitArmorSet(this);
		needSendServerPlayerSkillCheck = false;
		lastActiveSkillId = 0;
		lastSwVoidState = ESwVoidStateID.IDLE;
		lastSkillType = ELastSkillType.NONE;
		LastTriggerEnemyKills = 0;
		PlayerEquipmentTrigger(userState);
	}

	public void Clear()
	{
		if (advancedSkillList != null)
		{
			foreach (AdvancedSkill advancedSkill in advancedSkillList)
			{
				advancedSkill.Inactive();
			}
		}
		advancedSkillList = new List<AdvancedSkill>();
		OnTriggerCheckList = new Dictionary<AdvancedSkill.EOnTriggerType, List<AdvancedSkill>>();
		OffTriggerCheckList = new Dictionary<AdvancedSkill.EOffTriggerType, List<AdvancedSkill>>();
	}

	public void CreateSkills()
	{
		skills = new List<Skill>();
		for (int i = 0; i < 104; i++)
		{
			Skill skill = new Skill();
			skill.skillType = (SkillsType)i;
			skill.data = 0f;
			skills.Add(skill);
		}
	}

	public void AddSkill(Skill skill)
	{
		if (skill.skillType != SkillsType.SKILL_COUNT)
		{
			if (Skill.isPercent(skill.skillType))
			{
				skills[(int)skill.skillType].data += skill.data / 100f;
			}
			else
			{
				skills[(int)skill.skillType].data += skill.data;
			}
			CheckSkillModified(skill, true);
		}
	}

	public void AddSkills(List<Skill> skills)
	{
		foreach (Skill skill in skills)
		{
			AddSkill(skill);
		}
	}

	public void RemoveSkill(Skill skill)
	{
		if (skill.skillType != SkillsType.SKILL_COUNT)
		{
			if (Skill.isPercent(skill.skillType))
			{
				skills[(int)skill.skillType].data -= skill.data / 100f;
			}
			else
			{
				skills[(int)skill.skillType].data -= skill.data;
			}
			if ((double)skills[(int)skill.skillType].data < 1E-05 && (double)skills[(int)skill.skillType].data > -1E-05)
			{
				skills[(int)skill.skillType].data = 0f;
			}
			CheckSkillModified(skill, false);
		}
	}

	public void RemoveSkills(List<Skill> skills)
	{
		foreach (Skill skill in skills)
		{
			RemoveSkill(skill);
		}
	}

	public float GetSkill(SkillsType sType)
	{
		return (sType != SkillsType.SKILL_COUNT && sType != SkillsType.SKILL_NONE) ? skills[(int)sType].data : 0f;
	}

	public bool HasSkill(SkillsType sType)
	{
		return skills[(int)sType].data != 0f;
	}

	public override string ToString()
	{
		string text = string.Empty;
		for (int i = 0; i < 104; i++)
		{
			string text2 = text;
			text = string.Concat(text2, skills[i].skillType, ":", skills[i].data, "   ");
		}
		return text;
	}

	public string GetInfoString()
	{
		string text = "player skill : ";
		for (int i = 0; i < 104; i++)
		{
			if (skills[i].data != 0f)
			{
				string text2 = text;
				text = string.Concat(text2, "[ ", skills[i].skillType, " : ", skills[i].data, " ] ");
			}
		}
		text += "\n";
		foreach (AdvancedSkill advancedSkill in advancedSkillList)
		{
			text += "< ";
			text += advancedSkill.GetInfoString();
			text += " > \n";
		}
		return text;
	}

	public void AddAdvancedSkill(AdvancedSkill newSkill)
	{
		advancedSkillList.Add(newSkill);
		if (OnTriggerCheckList.ContainsKey(newSkill.GetOnTriggerType()))
		{
			OnTriggerCheckList[newSkill.GetOnTriggerType()].Add(newSkill);
		}
		else
		{
			OnTriggerCheckList.Add(newSkill.GetOnTriggerType(), new List<AdvancedSkill>());
			OnTriggerCheckList[newSkill.GetOnTriggerType()].Add(newSkill);
		}
		if (OffTriggerCheckList.ContainsKey(newSkill.GetOffTriggerType()))
		{
			OffTriggerCheckList[newSkill.GetOffTriggerType()].Add(newSkill);
		}
		else
		{
			OffTriggerCheckList.Add(newSkill.GetOffTriggerType(), new List<AdvancedSkill>());
			OffTriggerCheckList[newSkill.GetOffTriggerType()].Add(newSkill);
		}
		if (newSkill.GetOnTriggerType() == AdvancedSkill.EOnTriggerType.ALWAYS)
		{
			newSkill.Active();
		}
	}

	public void RemoveAdvancedSkill(AdvancedSkill skillToRemove)
	{
		advancedSkillList.Remove(skillToRemove);
		if (OnTriggerCheckList.ContainsKey(skillToRemove.GetOnTriggerType()) && OnTriggerCheckList[skillToRemove.GetOnTriggerType()].Contains(skillToRemove))
		{
			OnTriggerCheckList[skillToRemove.GetOnTriggerType()].Remove(skillToRemove);
		}
		if (OffTriggerCheckList.ContainsKey(skillToRemove.GetOffTriggerType()) && OffTriggerCheckList[skillToRemove.GetOffTriggerType()].Contains(skillToRemove))
		{
			OffTriggerCheckList[skillToRemove.GetOffTriggerType()].Remove(skillToRemove);
		}
		skillToRemove.Remove();
	}

	public void AddLiteSpecialSkill(LiteSpecialSkill skill)
	{
		if (!liteSpecialSkillList.Contains(skill))
		{
			liteSpecialSkillList.Add(skill);
		}
	}

	public void RemoveLiteSpecialSkill(LiteSpecialSkill skill)
	{
		if (liteSpecialSkillList.Contains(skill))
		{
			liteSpecialSkillList.Remove(skill);
		}
	}

	public float GetDamageBoothDependsOnState(GameUnit unit)
	{
		float num = 0f;
		foreach (LiteSpecialSkill liteSpecialSkill in liteSpecialSkillList)
		{
			switch (liteSpecialSkill.type)
			{
			case AdvancedSkill.ESpecialSkillType.EXECUTIONER:
				if ((float)unit.Hp / (float)unit.MaxHp * 100f <= liteSpecialSkill.parameter1)
				{
					num += liteSpecialSkill.parameter2;
				}
				break;
			case AdvancedSkill.ESpecialSkillType.MODE_PLAYER_STATE:
				if (unit.GameUnitType == EGameUnitType.ENEMY)
				{
					Enemy enemy = unit as Enemy;
					if (enemy.inShield())
					{
						num += liteSpecialSkill.parameter2;
						Debug.Log(num);
					}
				}
				break;
			}
		}
		return num / 100f;
	}

	public int GetSpecialSkillDamage(AdvancedSkill.ESpecialSkillType type)
	{
		LiteSpecialSkill liteSpecialSkill = null;
		foreach (LiteSpecialSkill liteSpecialSkill2 in liteSpecialSkillList)
		{
			if (liteSpecialSkill2.type == type)
			{
				liteSpecialSkill = liteSpecialSkill2;
			}
		}
		if (liteSpecialSkill != null)
		{
			switch (liteSpecialSkill.type)
			{
			case AdvancedSkill.ESpecialSkillType.MINE:
				return (int)(player.GetCurrentDps() * liteSpecialSkill.parameter1);
			case AdvancedSkill.ESpecialSkillType.IMPACT_WAVE:
				return (int)(player.GetCurrentDps() * liteSpecialSkill.parameter1);
			case AdvancedSkill.ESpecialSkillType.HOOK:
				return (int)(player.GetCurrentDps() * liteSpecialSkill.parameter1);
			default:
				return 0;
			}
		}
		return 0;
	}

	public void Loop()
	{
		foreach (PlayerSpecialSkill specialSkill in specialSkillList)
		{
			specialSkill.Loop();
		}
		if (OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.WAIT_TIME))
		{
			foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.WAIT_TIME])
			{
				item.CheckWaitTimeOnTrigger();
			}
		}
		if (OffTriggerCheckList.ContainsKey(AdvancedSkill.EOffTriggerType.WAIT_TIME))
		{
			foreach (AdvancedSkill item2 in OffTriggerCheckList[AdvancedSkill.EOffTriggerType.WAIT_TIME])
			{
				item2.CheckWaitTimeOffTrigger();
			}
		}
		CheckAllSkillCoolDown();
		if (player != null && player.IsLocal())
		{
			UpdateIsMoving();
		}
	}

	public void InitSkillButton(List<StateSkill> stateSkillList, int startIndex)
	{
		int num = startIndex;
		if (!OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.ACTIVE_SKILL))
		{
			return;
		}
		foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.ACTIVE_SKILL])
		{
			if (num < stateSkillList.Count)
			{
				StateSkill stateSkill = stateSkillList[num];
				stateSkill.gameObject.SetActive(true);
				stateSkill.SetAdvancedSkill(item);
				stateSkill.SetSkillType(StateSkill.EStateSkillType.ADVANCED_SKILL);
				stateSkill.SetCoolDownTime(item.GetOnTriggerValues()[0]);
				stateSkill.SetEffectTime(item.GetOffTriggerValues()[0]);
				num++;
			}
		}
	}

	public void EnableKillStreak(int killStreakId)
	{
		PvpSkill killStreakById = GameApp.GetInstance().GetUserState().GetPvpSkillManager()
			.GetKillStreakById(killStreakId);
		if (killStreakById == null || !OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.KILL_STREAK))
		{
			return;
		}
		foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.KILL_STREAK])
		{
			if (item.GetExternalId() == killStreakById.GetExternalID() && null != HUDBase.instance && null != HUDBase.instance.m_KillStreakHUDManager)
			{
				HUDBase.instance.m_KillStreakHUDManager.AddSkill(killStreakId, item);
			}
		}
	}

	public void CheckAllSkillCoolDown()
	{
		if (!(HUDBase.instance != null))
		{
			return;
		}
		if (HUDBase.instance.m_SkillHUDManager.IsAllSkillNotCoolDown())
		{
			if (!OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.ALL_SKILL_NOT_COOLDOWN))
			{
				return;
			}
			{
				foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.ALL_SKILL_NOT_COOLDOWN])
				{
					if (!item.IsActived())
					{
						item.Active();
						TrySendPlayerSkillCheck(true, 27, item.GetExternalId());
					}
				}
				return;
			}
		}
		if (!OffTriggerCheckList.ContainsKey(AdvancedSkill.EOffTriggerType.SKILL_COOLDOWN))
		{
			return;
		}
		foreach (AdvancedSkill item2 in OffTriggerCheckList[AdvancedSkill.EOffTriggerType.SKILL_COOLDOWN])
		{
			if (item2.IsActived())
			{
				item2.Inactive();
				TrySendPlayerSkillCheck(false, 14, item2.GetExternalId());
			}
		}
	}

	public void TriggerSkillFromOtherPlayer(int skillId)
	{
		bool flag = false;
		if (OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.OTHER_PLAYER))
		{
			foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.OTHER_PLAYER])
			{
				if (item.GetExternalId() == skillId)
				{
					item.Active();
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			SkillGroup skillGroup = new SkillGroup();
			skillGroup.source = "Other player: " + skillId;
			skillGroup.InitSkillInfo(skillId);
			skillGroup.InitToPlayerSkill(this);
			if (skillGroup.GetAdvancedSkills().Count > 0)
			{
				skillGroup.GetFirstAdvancedSkill().Active();
			}
		}
	}

	public void TriggerOffSkillFromOtherPlayer(int skillId)
	{
		if (!OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.OTHER_PLAYER))
		{
			return;
		}
		foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.OTHER_PLAYER])
		{
			if (item.GetExternalId() == skillId)
			{
				item.Inactive();
				break;
			}
		}
	}

	public void PlayerAttack(Weapon weapon)
	{
		if (OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.AFTER_ATTACK))
		{
			foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.AFTER_ATTACK])
			{
				if (item.GetOnTriggerValues()[0].Equals((float)weapon.mSkillType))
				{
					item.Active();
					TrySendPlayerSkillCheck(true, 2, item.GetExternalId());
				}
			}
		}
		if (OffTriggerCheckList.ContainsKey(AdvancedSkill.EOffTriggerType.AFTER_ATTACK))
		{
			foreach (AdvancedSkill item2 in OffTriggerCheckList[AdvancedSkill.EOffTriggerType.AFTER_ATTACK])
			{
				item2.Inactive();
				TrySendPlayerSkillCheck(false, 2, item2.GetExternalId());
			}
		}
		if (!OffTriggerCheckList.ContainsKey(AdvancedSkill.EOffTriggerType.ATTER_ATTACK_AND_BE_HIT))
		{
			return;
		}
		foreach (AdvancedSkill item3 in OffTriggerCheckList[AdvancedSkill.EOffTriggerType.ATTER_ATTACK_AND_BE_HIT])
		{
			item3.Inactive();
			TrySendPlayerSkillCheck(false, 5, item3.GetExternalId());
		}
	}

	public bool PlayerKillAmountOfEnemy(GameUnit enemy)
	{
		if (OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.KILL_ENEMIES))
		{
			foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.KILL_ENEMIES])
			{
				if (GameApp.GetInstance().GetGameMode().IsVSMode())
				{
					if ((float)(player.Kills - LastTriggerEnemyKills) >= item.GetOnTriggerValues()[1])
					{
						LastTriggerEnemyKills = player.Kills;
						LastTriggerEnemy = enemy;
						item.Active();
						TrySendPlayerSkillCheck(true, 25, item.GetExternalId());
						return true;
					}
				}
				else if ((float)(player.Kills - LastTriggerEnemyKills) >= item.GetOnTriggerValues()[0])
				{
					LastTriggerEnemyKills = player.Kills;
					LastTriggerEnemy = enemy;
					item.Active();
					TrySendPlayerSkillCheck(true, 25, item.GetExternalId());
					return true;
				}
			}
		}
		return false;
	}

	public int GetLastTriggeredEnemyAmount()
	{
		return LastTriggerEnemyKills;
	}

	public void PlayerHitTheTargetByDamageType(DamageProperty dp)
	{
		if (OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.HIT_BY_DAMAGE_TYPE))
		{
			foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.HIT_BY_DAMAGE_TYPE])
			{
				if ((float)(int)(byte)dp.damageType == item.GetOnTriggerValues()[0] || item.GetOnTriggerValues()[0].Equals(0f))
				{
					item.Active();
				}
			}
		}
		if (!OffTriggerCheckList.ContainsKey(AdvancedSkill.EOffTriggerType.HIT_BY_DAMAGE_TYPE))
		{
			return;
		}
		foreach (AdvancedSkill item2 in OffTriggerCheckList[AdvancedSkill.EOffTriggerType.HIT_BY_DAMAGE_TYPE])
		{
			if ((float)(int)(byte)dp.damageType == item2.GetOffTriggerValues()[0] || item2.GetOffTriggerValues()[0].Equals(0f))
			{
				item2.Inactive();
			}
		}
	}

	public void PlayerBeHit()
	{
		if (OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.AFTER_BE_HIT))
		{
			foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.AFTER_BE_HIT])
			{
				item.Active();
				TrySendPlayerSkillCheck(true, 3, item.GetExternalId());
			}
		}
		if (OffTriggerCheckList.ContainsKey(AdvancedSkill.EOffTriggerType.AFTER_BE_HIT))
		{
			foreach (AdvancedSkill item2 in OffTriggerCheckList[AdvancedSkill.EOffTriggerType.AFTER_BE_HIT])
			{
				item2.Inactive();
				TrySendPlayerSkillCheck(false, 3, item2.GetExternalId());
			}
		}
		if (!OffTriggerCheckList.ContainsKey(AdvancedSkill.EOffTriggerType.ATTER_ATTACK_AND_BE_HIT))
		{
			return;
		}
		foreach (AdvancedSkill item3 in OffTriggerCheckList[AdvancedSkill.EOffTriggerType.ATTER_ATTACK_AND_BE_HIT])
		{
			item3.Inactive();
			TrySendPlayerSkillCheck(false, 5, item3.GetExternalId());
		}
	}

	public void PlayerInputControllerChange(int changeType)
	{
		if (OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.INPUT_CONTROLLER_CHANGE))
		{
			foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.INPUT_CONTROLLER_CHANGE])
			{
				if (item.GetOnTriggerValues()[0].Equals(changeType))
				{
					item.Active();
					TrySendPlayerSkillCheck(true, 6, item.GetExternalId());
				}
			}
		}
		if (!OffTriggerCheckList.ContainsKey(AdvancedSkill.EOffTriggerType.INPUT_CONTROLLER_CHANGE))
		{
			return;
		}
		foreach (AdvancedSkill item2 in OffTriggerCheckList[AdvancedSkill.EOffTriggerType.INPUT_CONTROLLER_CHANGE])
		{
			if (item2.GetOffTriggerValues()[0].Equals(changeType))
			{
				item2.Inactive();
				TrySendPlayerSkillCheck(false, 6, item2.GetExternalId());
			}
		}
	}

	public void PlayerUseActiveSkill()
	{
		if (!OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.ACTIVE_SKILL_USED))
		{
			return;
		}
		foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.ACTIVE_SKILL_USED])
		{
			item.Active();
		}
	}

	public void UpdateIsMoving()
	{
		if (!mIsUpdateIsMovingEnable)
		{
			if (player == null || !(player.GetObject() != null))
			{
				return;
			}
			mLastStandPlayerPosition = (mLastPlayerPosition = player.GetPosition());
			mIsUpdateIsMovingEnable = true;
		}
		if (player.InPlayingState())
		{
			if (Vector3.SqrMagnitude(player.GetPosition() - mLastPlayerPosition) > 0f)
			{
				mMovingTime += Time.deltaTime;
				mStandTime = 0f;
			}
			else
			{
				mStandTime += Time.deltaTime;
				mMovingTime = 0f;
			}
			mLastPlayerPosition = player.GetPosition();
		}
		if (OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.IS_MOVING))
		{
			foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.IS_MOVING])
			{
				if (!item.IsActived() && mMovingTime > item.GetOnTriggerValues()[0])
				{
					item.Active();
					TrySendPlayerSkillCheck(true, 9, item.GetExternalId());
				}
			}
		}
		if (OffTriggerCheckList.ContainsKey(AdvancedSkill.EOffTriggerType.IS_MOVING))
		{
			foreach (AdvancedSkill item2 in OffTriggerCheckList[AdvancedSkill.EOffTriggerType.IS_MOVING])
			{
				if (item2.IsActived() && mMovingTime > item2.GetOffTriggerValues()[0])
				{
					item2.Inactive();
					TrySendPlayerSkillCheck(false, 9, item2.GetExternalId());
				}
			}
		}
		if (OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.IS_STAND))
		{
			foreach (AdvancedSkill item3 in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.IS_STAND])
			{
				if (!item3.IsActived() && mStandTime > item3.GetOnTriggerValues()[0])
				{
					item3.Active();
					TrySendPlayerSkillCheck(true, 11, item3.GetExternalId());
				}
			}
		}
		if (!OffTriggerCheckList.ContainsKey(AdvancedSkill.EOffTriggerType.IS_STAND))
		{
			return;
		}
		foreach (AdvancedSkill item4 in OffTriggerCheckList[AdvancedSkill.EOffTriggerType.IS_STAND])
		{
			if (item4.IsActived() && mStandTime > item4.GetOffTriggerValues()[0])
			{
				item4.Inactive();
				TrySendPlayerSkillCheck(false, 10, item4.GetExternalId());
			}
		}
	}

	public void UpdateEnemyNearPerk()
	{
		if (!OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.HAS_ENEMY_AROUND) || !mEnemyNearTimer.Ready())
		{
			return;
		}
		mEnemyNearTimer.Do();
		int num = 0;
		List<RemotePlayer> remotePlayers = GameApp.GetInstance().GetGameWorld().GetRemotePlayers();
		if (remotePlayers.Count > 0)
		{
			for (int i = 0; i < remotePlayers.Count; i++)
			{
				RemotePlayer remotePlayer = remotePlayers[i];
				if (!player.IsSameTeam(remotePlayer) && Vector3.Distance(player.GetPosition(), remotePlayer.GetPosition()) <= 10f && remotePlayer.InPlayingState())
				{
					num++;
				}
			}
		}
		foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.HAS_ENEMY_AROUND])
		{
			if ((float)num >= item.GetOnTriggerValues()[0])
			{
				item.Active();
				TrySendPlayerSkillCheck(true, 12, item.GetExternalId());
			}
		}
	}

	public void PlayerRebirth()
	{
		if (!OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.REBIRTH))
		{
			return;
		}
		foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.REBIRTH])
		{
			item.Active();
			TrySendPlayerSkillCheck(true, 14, item.GetExternalId());
		}
	}

	public void PlayerDead()
	{
		if (!OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.DEATH))
		{
			return;
		}
		foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.DEATH])
		{
			item.Active();
		}
	}

	public void AllyDead()
	{
		if (!OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.ALLY_DEAD))
		{
			return;
		}
		foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.ALLY_DEAD])
		{
			item.Active();
			TrySendPlayerSkillCheck(true, 17, item.GetExternalId());
		}
	}

	public void PlayerKillOthers()
	{
		if (!OffTriggerCheckList.ContainsKey(AdvancedSkill.EOffTriggerType.KILL_OTHERS))
		{
			return;
		}
		foreach (AdvancedSkill item in OffTriggerCheckList[AdvancedSkill.EOffTriggerType.KILL_OTHERS])
		{
			item.Inactive();
			TrySendPlayerSkillCheck(false, 8, item.GetExternalId());
		}
	}

	public void TripleDead()
	{
		if (!OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.TRIPLE_DEAD))
		{
			return;
		}
		foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.TRIPLE_DEAD])
		{
			item.Active();
			TrySendPlayerSkillCheck(true, 18, item.GetExternalId());
		}
	}

	public void PlayerInPoint()
	{
		if (!OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.PLAYER_IN_POINT))
		{
			return;
		}
		foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.PLAYER_IN_POINT])
		{
			item.Active();
		}
	}

	public void PlayerOutPoint()
	{
		if (!OffTriggerCheckList.ContainsKey(AdvancedSkill.EOffTriggerType.PLAYER_OUT_POINT))
		{
			return;
		}
		foreach (AdvancedSkill item in OffTriggerCheckList[AdvancedSkill.EOffTriggerType.PLAYER_OUT_POINT])
		{
			item.Inactive();
		}
	}

	public void PlayerCaptureFlag()
	{
		if (!OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.CAPTURE_FLAG))
		{
			return;
		}
		foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.CAPTURE_FLAG])
		{
			item.Active();
		}
	}

	public void PlayerEquipmentTrigger(UserState state)
	{
		bool flag = false;
		if (state.GetBattleWeapons().Count == 1 && state.GetBattleWeapons()[0].GetDamageType() == PlayerDamageType.ENERGY)
		{
			flag = true;
		}
		if (!OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.EQUIPMENT_TRIGGER))
		{
			return;
		}
		foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.EQUIPMENT_TRIGGER])
		{
			if (item.GetOnTriggerValues()[0] == 0f && flag)
			{
				item.Active();
			}
		}
	}

	public void PlayerHpRateTrigger(float HpRate)
	{
		if (OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.HP_RATE))
		{
			foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.HP_RATE])
			{
				if (item.GetOnTriggerValues()[0] == 0f && HpRate < item.GetOnTriggerValues()[1] / 100f)
				{
					item.Active();
				}
				else if (item.GetOnTriggerValues()[0] == 1f && HpRate >= item.GetOnTriggerValues()[1] / 100f)
				{
					item.Active();
				}
			}
		}
		if (!OffTriggerCheckList.ContainsKey(AdvancedSkill.EOffTriggerType.HP_RATE))
		{
			return;
		}
		foreach (AdvancedSkill item2 in OffTriggerCheckList[AdvancedSkill.EOffTriggerType.HP_RATE])
		{
			if (item2.GetOffTriggerValues()[0] == 0f && HpRate < item2.GetOffTriggerValues()[1] / 100f)
			{
				item2.Inactive();
			}
			else if (item2.GetOffTriggerValues()[0] == 1f && HpRate >= item2.GetOffTriggerValues()[1] / 100f)
			{
				item2.Inactive();
			}
		}
	}

	public void playerTryOpenSuperShield()
	{
		if (!OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.OPEN_SUPER_SHIELD))
		{
			return;
		}
		foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.OPEN_SUPER_SHIELD])
		{
			if (!item.IsActived())
			{
				item.Active();
			}
		}
	}

	public void SetPlayer(Player player)
	{
		needSendServerPlayerSkillCheck = false;
		this.player = player;
		if (player.IsLocal() && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			needSendServerPlayerSkillCheck = true;
		}
	}

	public Player GetPlayer()
	{
		return player;
	}

	private void CheckSkillModified(Skill skill, bool isAdding)
	{
		switch (skill.skillType)
		{
		case SkillsType.SHOOT_SPEED:
			if (player != null)
			{
				player.ResetAllWeaponAttackFrenquency();
			}
			break;
		case SkillsType.HP_UP_RATE:
			if (player != null)
			{
				player.RecalculateMaxHp(isAdding, skill.data / 100f);
			}
			break;
		}
	}

	public void AddSpecialSkill(PlayerSpecialSkill playerSpecialSkill)
	{
		if (!specialSkillList.Contains(playerSpecialSkill))
		{
			specialSkillList.Add(playerSpecialSkill);
		}
	}

	public void RemoveSpecialSkill(PlayerSpecialSkill playerSpecialSkill)
	{
		if (specialSkillList.Contains(playerSpecialSkill))
		{
			playerSpecialSkill.OnInactive();
			specialSkillList.Remove(playerSpecialSkill);
		}
	}

	public void PlayerDeadClear()
	{
		foreach (AdvancedSkill advancedSkill in advancedSkillList)
		{
			if (advancedSkill.GetOnTriggerType() != AdvancedSkill.EOnTriggerType.ALWAYS && advancedSkill.GetOffTriggerType() != AdvancedSkill.EOffTriggerType.NEVER)
			{
				advancedSkill.Inactive();
			}
		}
		player.mBattleItemManager.PlayerDeadClear();
		player.mBattlePickupManager.PlayerDeadClear();
	}

	public void PlayerUseAItem()
	{
		if (!OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.USE_A_ITEM))
		{
			return;
		}
		foreach (AdvancedSkill item in OnTriggerCheckList[AdvancedSkill.EOnTriggerType.USE_A_ITEM])
		{
			item.Active();
			item.AddStack();
		}
	}

	public void TrySendPlayerSkillCheck(bool isOnCheck, byte triggerType, int skillId)
	{
		if (needSendServerPlayerSkillCheck)
		{
			PlayerSkillCheckRequest request = new PlayerSkillCheckRequest(isOnCheck, triggerType, skillId);
			if (IsRobot)
			{
				NetworkManager.SendRequest(request);
			}
			else
			{
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
	}

	public float GetHealValue()
	{
		if (OnTriggerCheckList.ContainsKey(AdvancedSkill.EOnTriggerType.HP_RATE))
		{
			using (List<AdvancedSkill>.Enumerator enumerator = OnTriggerCheckList[AdvancedSkill.EOnTriggerType.HP_RATE].GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					AdvancedSkill current = enumerator.Current;
					return current.GetOnTriggerValues()[1] / 100f;
				}
			}
		}
		return 0f;
	}

	public void SetLastActiveSkill(int Id)
	{
		lastActiveSkillId = Id;
		lastSkillType = ELastSkillType.ACTIVE_SKILL;
	}

	public void SetLastActiveSkill(ESwVoidStateID state)
	{
		lastSwVoidState = state;
		lastSkillType = ELastSkillType.SWVOID;
	}

	public ELastSkillType GetLastActiveSkillType()
	{
		return lastSkillType;
	}

	public void ActiveLastSkill()
	{
		if (lastSkillType == ELastSkillType.ACTIVE_SKILL)
		{
			AdvancedSkill advancedSkill = GetAdvancedSkill(lastActiveSkillId);
			if (advancedSkill == null || lastActiveSkillId == 4011)
			{
				return;
			}
			Debug.Log("Active Last Skill " + lastActiveSkillId);
			if (player.mVehicle != null && (lastActiveSkillId == 4004 || lastActiveSkillId == 4008 || lastActiveSkillId == 4014 || lastActiveSkillId == 4015))
			{
				player.mVehicle.ResetDeadTime();
				return;
			}
			if (lastActiveSkillId > 5000 && lastActiveSkillId < 6000)
			{
				Item itemBySkill = player.GetUserState().GetItemBySkill(lastActiveSkillId);
				if (itemBySkill != null)
				{
					player.ReuseItem(itemBySkill.GetItemID());
				}
				return;
			}
			advancedSkill.Active();
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				TrySendPlayerSkillCheck(true, (byte)advancedSkill.GetOnTriggerType(), lastActiveSkillId);
			}
			if (advancedSkill.GetOnTriggerType() == AdvancedSkill.EOnTriggerType.KILL_STREAK)
			{
				UserState userState = GameApp.GetInstance().GetUserState();
				PvpSkill killStreakByExternalId = userState.GetPvpSkillManager().GetKillStreakByExternalId(lastActiveSkillId);
				KillStreakUsedRequest request = new KillStreakUsedRequest(killStreakByExternalId.GetId());
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
		else if (lastSkillType == ELastSkillType.SWVOID && player.mSwVoid != null && (lastSwVoidState == ESwVoidStateID.HEAVY_SHIELD || lastSwVoidState == ESwVoidStateID.DEMOMAN_EXPLODE || lastSwVoidState == ESwVoidStateID.SCOUT_DASH || lastSwVoidState == ESwVoidStateID.MEDIC_HEAL))
		{
			player.mSwVoid.ActiveSkill();
		}
	}

	public AdvancedSkill GetAdvancedSkill(int ID)
	{
		foreach (AdvancedSkill advancedSkill in advancedSkillList)
		{
			if (advancedSkill.GetExternalId() == ID)
			{
				return advancedSkill;
			}
		}
		return null;
	}

	public List<AdvancedSkill> GetAdvancedSkills()
	{
		return advancedSkillList;
	}

	public bool GetLastSkillCanActive()
	{
		AdvancedSkill advancedSkill = GetAdvancedSkill(lastActiveSkillId);
		if (advancedSkill == null || !advancedSkill.CanActive())
		{
			return false;
		}
		return true;
	}
}
