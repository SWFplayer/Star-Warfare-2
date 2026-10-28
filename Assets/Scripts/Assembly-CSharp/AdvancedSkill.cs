using System.Collections.Generic;
using UnityEngine;

public class AdvancedSkill
{
	public enum EOnTriggerType
	{
		ALWAYS = 0,
		ACTIVE_SKILL = 1,
		AFTER_ATTACK = 2,
		AFTER_BE_HIT = 3,
		WAIT_TIME = 4,
		PICK_UP = 5,
		INPUT_CONTROLLER_CHANGE = 6,
		HIT_BY_DAMAGE_TYPE = 7,
		KILL_STREAK = 8,
		IS_MOVING = 9,
		USE_ITEM = 10,
		IS_STAND = 11,
		HAS_ENEMY_AROUND = 12,
		OTHER_PLAYER = 13,
		REBIRTH = 14,
		DEATH = 15,
		HIT_BY_WEAPON_TYPE = 16,
		ALLY_DEAD = 17,
		TRIPLE_DEAD = 18,
		PLAYER_IN_POINT = 19,
		CAPTURE_FLAG = 20,
		EQUIPMENT_TRIGGER = 21,
		HP_RATE = 22,
		ACTIVE_SKILL_USED = 23,
		PLAYER_BOSS_ACTIVE_BUFF = 24,
		KILL_ENEMIES = 25,
		OPEN_SUPER_SHIELD = 26,
		ALL_SKILL_NOT_COOLDOWN = 27,
		PLAYER_BE_HIT_DEBUFF = 28,
		USE_A_ITEM = 29
	}

	public enum EOffTriggerType
	{
		NEVER = 0,
		AFTER_ACTIVE_SKILL = 1,
		AFTER_ATTACK = 2,
		AFTER_BE_HIT = 3,
		WAIT_TIME = 4,
		ATTER_ATTACK_AND_BE_HIT = 5,
		INPUT_CONTROLLER_CHANGE = 6,
		HIT_BY_DAMAGE_TYPE = 7,
		KILL_OTHERS = 8,
		IS_MOVING = 9,
		IS_STAND = 10,
		PLAYER_OUT_POINT = 11,
		HP_RATE = 12,
		HIT_BY_WEAPON_TYPE = 13,
		SKILL_COOLDOWN = 14
	}

	public enum ESpecialSkillType
	{
		NONE = 0,
		RESTORE_HP = 1,
		TIME_CONTROLLER_AND_ENEMY_STUN = 2,
		AFFECT_TEAM_PLAYERS = 3,
		AREA_CONTINUE_DAMAGE = 4,
		TIME_TO_STACK_WITH_LIMITED = 5,
		EXECUTIONER = 6,
		AFFECT_ENEMY_PLAYERS = 7,
		RANDOM_PICKUP = 8,
		REVENGE_RADAR = 9,
		GENERATE_VEHICLE = 10,
		AOE_BUFF = 11,
		PLACE_HOLDER2 = 12,
		MISSILE_ATTACK = 13,
		PLACE_HOLDER3 = 14,
		KILL_IN_NEXT_BLOW = 15,
		KILL_ALL = 16,
		REVENGE_BOMB = 17,
		AFFECT_SPECIFIC_PLAYER = 18,
		MODE_PLAYER_STATE = 19,
		ACTIVE_LAST_SKILL = 20,
		GENERATE_PRESENT = 21,
		FLASH_REFILL = 22,
		MINE = 23,
		IMPACT_WAVE = 24,
		ADD_SHIELD = 25,
		BORROWED_TIME = 26,
		HOOK = 27,
		HIT_DONW = 28,
		FLASH_BLADE = 29,
		CD_REDUCE = 30,
		USE_ITEM_SPEED_UP = 31,
		SUPER_SHIELD = 32
	}

	public enum EEquipmentTriggerType
	{
		ONLY_ONE_ENERGY_WEAPON = 0
	}

	public enum EMissleType
	{
		MISSLE = 0,
		RANDOM_MISSLE = 1,
		TRIPLE_MISSLE = 2,
		SLOW_MISSLE = 3
	}

	public enum EBuffState
	{
		NONE = 0,
		BUFF = 1,
		DEBUFF = 2
	}

	protected bool isActived;

	protected int mExternalId;

	protected string mIconName;

	protected string mBuffDesc;

	protected EBuffState mBuffState;

	protected EOnTriggerType onTriggerType;

	protected EOffTriggerType offTriggerType;

	protected float[] onTriggerValues = new float[2];

	protected float[] offTriggerValues = new float[2];

	protected List<Skill> skills = new List<Skill>();

	protected PlayerSkill mPlayerSkill;

	private float lastActiveTime;

	private float lastInactiveTime;

	private ESpecialSkillType mSpecialSkillType;

	private float[] mSpecialSkillValues;

	private string mSpecialSkillValueString;

	private PlayerSpecialSkill playerSpecialSkill;

	public string source = string.Empty;

	public void Active()
	{
		if (!isActived)
		{
			isActived = true;
			mPlayerSkill.AddSkills(skills);
			SpecialSkillActive();
			if (mExternalId == 0 || mExternalId == 8003 || mExternalId != 7008)
			{
			}
			PlayVFX();
		}
		lastActiveTime = Time.time;
		if (onTriggerType == EOnTriggerType.KILL_STREAK || onTriggerType == EOnTriggerType.USE_ITEM)
		{
			mPlayerSkill.SetLastActiveSkill(mExternalId);
		}
	}

	public void ActiveWithoutRepeat()
	{
		if (!isActived)
		{
			isActived = true;
			mPlayerSkill.AddSkills(skills);
			SpecialSkillActive();
			if (mExternalId == 0 || mExternalId == 8003 || mExternalId != 7008)
			{
			}
			PlayVFX();
		}
		lastActiveTime = Time.time;
	}

	public void Inactive()
	{
		if (isActived)
		{
			isActived = false;
			mPlayerSkill.RemoveSkills(skills);
			SpecialSkillInactive();
			DestroyVFX();
		}
		lastInactiveTime = Time.time;
	}

	public void CheckWaitTimeOnTrigger()
	{
		if (!isActived && onTriggerType == EOnTriggerType.WAIT_TIME && Time.time - lastInactiveTime > onTriggerValues[0])
		{
			Active();
		}
	}

	public void CheckWaitTimeOffTrigger()
	{
		if (isActived && offTriggerType == EOffTriggerType.WAIT_TIME)
		{
			float num = offTriggerValues[0];
			if (mBuffState == EBuffState.DEBUFF && mPlayerSkill.HasSkill(SkillsType.DEBUFF_DURING_INCREASE))
			{
				num += num * mPlayerSkill.GetSkill(SkillsType.DEBUFF_DURING_INCREASE);
			}
			if (mExternalId > 3000 && mExternalId < 4000 && mPlayerSkill.HasSkill(SkillsType.PICKUP_DURING_INCREASE))
			{
				num += num * mPlayerSkill.GetSkill(SkillsType.PICKUP_DURING_INCREASE);
			}
			if (Time.time - lastActiveTime > num)
			{
				Inactive();
			}
		}
	}

	public float ActivedTimePercentage()
	{
		if (isActived && offTriggerType == EOffTriggerType.WAIT_TIME)
		{
			float a = (Time.time - lastActiveTime) / offTriggerValues[0];
			return Mathf.Min(a, 1f);
		}
		return 0f;
	}

	public void InitToPlayerSkill(PlayerSkill playerSkill)
	{
		if (playerSkill == null)
		{
			Debug.LogError("playerSkill should not be null");
		}
		mPlayerSkill = playerSkill;
		playerSkill.AddAdvancedSkill(this);
	}

	public void UninitFromPlayerSkill()
	{
		if (mPlayerSkill != null)
		{
			mPlayerSkill.RemoveAdvancedSkill(this);
			mPlayerSkill = null;
		}
	}

	public void Remove()
	{
		Inactive();
	}

	public int GetExternalId()
	{
		return mExternalId;
	}

	public void SetExternalId(int id)
	{
		mExternalId = id;
	}

	public string GetIconName()
	{
		return mIconName;
	}

	public void SetIconName(string name)
	{
		mIconName = name;
	}

	public void SetBuffState(EBuffState buffState)
	{
		mBuffState = buffState;
	}

	public EBuffState GetBuffState()
	{
		return mBuffState;
	}

	public void SetBuffDesc(string desc)
	{
		mBuffDesc = desc;
	}

	public string GetBuffDesc()
	{
		return LocalizationManager.GetInstance().GetString(mBuffDesc);
	}

	public EOnTriggerType GetOnTriggerType()
	{
		return onTriggerType;
	}

	public float[] GetOnTriggerValues()
	{
		return onTriggerValues;
	}

	public EOffTriggerType GetOffTriggerType()
	{
		return offTriggerType;
	}

	public float[] GetOffTriggerValues()
	{
		return offTriggerValues;
	}

	public void AddSkill(Skill skill)
	{
		skills.Add(skill);
	}

	public void AddSkill(SkillsType type, float val)
	{
		Skill skill = new Skill();
		skill.skillType = type;
		skill.data = val;
		skills.Add(skill);
	}

	public List<Skill> GetSkills()
	{
		return skills;
	}

	public float GetSkill(SkillsType skillType)
	{
		foreach (Skill skill in skills)
		{
			if (skill.skillType == skillType)
			{
				return skill.data;
			}
		}
		return 0f;
	}

	public string GetInfoString()
	{
		string text = source;
		text += " : ";
		foreach (Skill skill in skills)
		{
			text += skill.skillType;
			text += " ";
			text += skill.data;
			text += ";";
		}
		return text;
	}

	public void SetOnTriggerType(int type)
	{
		onTriggerType = (EOnTriggerType)type;
	}

	public void SetOffTriggerType(int type)
	{
		offTriggerType = (EOffTriggerType)type;
	}

	public void SetOnTriggerValues(string valueOne, string valueTwo)
	{
		if (valueOne != string.Empty)
		{
			onTriggerValues[0] = float.Parse(valueOne);
		}
		if (valueTwo != string.Empty)
		{
			onTriggerValues[1] = float.Parse(valueTwo);
		}
	}

	public void SetOffTriggerValues(string valueOne, string valueTwo)
	{
		if (valueOne != string.Empty)
		{
			offTriggerValues[0] = float.Parse(valueOne);
		}
		if (valueTwo != string.Empty)
		{
			offTriggerValues[1] = float.Parse(valueTwo);
		}
	}

	public float GetOffTriggerTime()
	{
		return offTriggerValues[0];
	}

	public void SetSpecialSkillType(string specialSkillType)
	{
		if (!(specialSkillType == string.Empty))
		{
			int num = int.Parse(specialSkillType);
			mSpecialSkillType = (ESpecialSkillType)num;
		}
	}

	public void SetSpecialSkillValue(string valueString)
	{
		mSpecialSkillValueString = valueString;
		switch (mSpecialSkillType)
		{
		case ESpecialSkillType.TIME_CONTROLLER_AND_ENEMY_STUN:
		case ESpecialSkillType.AREA_CONTINUE_DAMAGE:
		case ESpecialSkillType.RANDOM_PICKUP:
		case ESpecialSkillType.REVENGE_RADAR:
		case ESpecialSkillType.AOE_BUFF:
		case ESpecialSkillType.MISSILE_ATTACK:
		case ESpecialSkillType.IMPACT_WAVE:
		case ESpecialSkillType.HOOK:
		case ESpecialSkillType.USE_ITEM_SPEED_UP:
			mSpecialSkillValues = new float[1];
			mSpecialSkillValues[0] = float.Parse(valueString);
			break;
		case ESpecialSkillType.RESTORE_HP:
		case ESpecialSkillType.TIME_TO_STACK_WITH_LIMITED:
		case ESpecialSkillType.EXECUTIONER:
		case ESpecialSkillType.REVENGE_BOMB:
		case ESpecialSkillType.MODE_PLAYER_STATE:
		case ESpecialSkillType.MINE:
		case ESpecialSkillType.ADD_SHIELD:
		case ESpecialSkillType.SUPER_SHIELD:
		{
			mSpecialSkillValues = new float[2];
			List<string> list = SwJson.ToJsonArray(mSpecialSkillValueString);
			for (int i = 0; i < mSpecialSkillValues.Length; i++)
			{
				mSpecialSkillValues[i] = float.Parse(list[i].ToString());
			}
			break;
		}
		case ESpecialSkillType.AFFECT_TEAM_PLAYERS:
		case ESpecialSkillType.AFFECT_ENEMY_PLAYERS:
			mSpecialSkillValues = new float[1];
			mSpecialSkillValues[0] = float.Parse(valueString);
			break;
		case ESpecialSkillType.GENERATE_VEHICLE:
		case ESpecialSkillType.PLACE_HOLDER2:
		case ESpecialSkillType.PLACE_HOLDER3:
		case ESpecialSkillType.KILL_IN_NEXT_BLOW:
		case ESpecialSkillType.KILL_ALL:
		case ESpecialSkillType.AFFECT_SPECIFIC_PLAYER:
		case ESpecialSkillType.ACTIVE_LAST_SKILL:
		case ESpecialSkillType.GENERATE_PRESENT:
		case ESpecialSkillType.FLASH_REFILL:
		case ESpecialSkillType.BORROWED_TIME:
		case ESpecialSkillType.HIT_DONW:
		case ESpecialSkillType.FLASH_BLADE:
		case ESpecialSkillType.CD_REDUCE:
			break;
		}
	}

	public void SpecialSkillActive()
	{
		switch (mSpecialSkillType)
		{
		case ESpecialSkillType.ADD_SHIELD:
		{
			if (!GameApp.GetInstance().GetGameMode().IsSingle())
			{
				break;
			}
			Player player2 = mPlayerSkill.GetPlayer();
			if (player2 != null)
			{
				if (mSpecialSkillValues[0] == 0f)
				{
					player2.AddShield(0f, mSpecialSkillValues[1] / 100f);
				}
				else if (mSpecialSkillValues[0] == 1f)
				{
					int num = (int)mSpecialSkillValues[1];
					player2.AddShield(num, 0f);
				}
			}
			break;
		}
		case ESpecialSkillType.BORROWED_TIME:
			if (GameApp.GetInstance().GetGameMode().IsSingle() && mPlayerSkill.GetPlayer() != null && mPlayerSkill.GetPlayer().IsLocal())
			{
				playerSpecialSkill = new PlayerSkillBorrowedTime();
				((PlayerSkillBorrowedTime)playerSpecialSkill).Init();
				mPlayerSkill.AddSpecialSkill(playerSpecialSkill);
			}
			break;
		case ESpecialSkillType.RESTORE_HP:
		{
			if (!GameApp.GetInstance().GetGameMode().IsSingle())
			{
				break;
			}
			Player player4 = mPlayerSkill.GetPlayer();
			if (player4 == null)
			{
				break;
			}
			if (mSpecialSkillValues[0] == 0f)
			{
				player4.RecoverRateHp(mSpecialSkillValues[1] / 100f);
			}
			else if (mSpecialSkillValues[0] == 1f)
			{
				int num2 = (int)mSpecialSkillValues[1];
				if (GameApp.GetInstance().GetGameMode().IsVSMode())
				{
					num2 = (int)VSMath.GetStaticRecoverInVS(num2);
				}
				player4.RecoverHP(num2);
			}
			break;
		}
		case ESpecialSkillType.AREA_CONTINUE_DAMAGE:
			if (mPlayerSkill.GetPlayer().IsLocal())
			{
				playerSpecialSkill = new PlayerSkillRangeFire();
				((PlayerSkillRangeFire)playerSpecialSkill).player = mPlayerSkill.GetPlayer();
				((PlayerSkillRangeFire)playerSpecialSkill).damagePerSecond = mPlayerSkill.GetPlayer().GetCurrentDps() * 0.01f * mSpecialSkillValues[0];
				((PlayerSkillRangeFire)playerSpecialSkill).rangeSqr = 100f;
				mPlayerSkill.AddSpecialSkill(playerSpecialSkill);
			}
			break;
		case ESpecialSkillType.TIME_TO_STACK_WITH_LIMITED:
			playerSpecialSkill = new PlayerSkillTimeStackWithLimited();
			((PlayerSkillTimeStackWithLimited)playerSpecialSkill).Init(this, mPlayerSkill, mSpecialSkillValues[0], (int)mSpecialSkillValues[1]);
			mPlayerSkill.AddSpecialSkill(playerSpecialSkill);
			break;
		case ESpecialSkillType.EXECUTIONER:
			playerSpecialSkill = new LiteSpecialSkill();
			((LiteSpecialSkill)playerSpecialSkill).parameter1 = mSpecialSkillValues[0];
			((LiteSpecialSkill)playerSpecialSkill).parameter2 = mSpecialSkillValues[1];
			((LiteSpecialSkill)playerSpecialSkill).type = ESpecialSkillType.EXECUTIONER;
			mPlayerSkill.AddLiteSpecialSkill((LiteSpecialSkill)playerSpecialSkill);
			break;
		case ESpecialSkillType.TIME_CONTROLLER_AND_ENEMY_STUN:
			if (mPlayerSkill.GetPlayer().IsLocal())
			{
				playerSpecialSkill = new PlayerSkillTimeController();
				((PlayerSkillTimeController)playerSpecialSkill).Init();
				((PlayerSkillTimeController)playerSpecialSkill).player = mPlayerSkill.GetPlayer();
				((PlayerSkillTimeController)playerSpecialSkill).SkillId = (int)mSpecialSkillValues[0];
				((PlayerSkillTimeController)playerSpecialSkill).Stun();
			}
			break;
		case ESpecialSkillType.AFFECT_TEAM_PLAYERS:
			if (GameApp.GetInstance().GetGameMode().IsSingle())
			{
				GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
					.TriggerSkillFromOtherPlayer((int)mSpecialSkillValues[0]);
			}
			break;
		case ESpecialSkillType.REVENGE_BOMB:
		{
			Player player7 = mPlayerSkill.GetPlayer();
			if (!player7.IsLocal())
			{
				break;
			}
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				if (mSpecialSkillValues[0] == 9f)
				{
					SpawnObjectRequest request = new SpawnObjectRequest((byte)mSpecialSkillValues[0], player7.GetPosition(), (short)mSpecialSkillValues[1]);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
				else
				{
					SpawnObjectRequest request2 = new SpawnObjectRequest((byte)mSpecialSkillValues[0], player7.GetPosition());
					GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
				}
				break;
			}
			ESpawnObjectType eSpawnObjectType = (ESpawnObjectType)mSpecialSkillValues[0];
			Vector3 position = player7.GetPosition();
			GameObject gameObject = null;
			if (eSpawnObjectType == ESpawnObjectType.FLASH)
			{
				Object original = Resources.Load(EffectName.SKILL_REVENGE_FALSH);
				position += new Vector3(0f, 1f, 0f);
				gameObject = Object.Instantiate(original, position, Quaternion.identity) as GameObject;
				GeneratedBomb component = gameObject.GetComponent<GeneratedBomb>();
				component.owner = player7;
			}
			break;
		}
		case ESpecialSkillType.MINE:
		{
			Player player11 = mPlayerSkill.GetPlayer();
			if (player11.IsLocal())
			{
				playerSpecialSkill = new LiteSpecialSkill();
				((LiteSpecialSkill)playerSpecialSkill).parameter1 = mSpecialSkillValues[0];
				((LiteSpecialSkill)playerSpecialSkill).parameter2 = mSpecialSkillValues[1];
				((LiteSpecialSkill)playerSpecialSkill).type = ESpecialSkillType.MINE;
				mPlayerSkill.AddLiteSpecialSkill((LiteSpecialSkill)playerSpecialSkill);
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
				{
					SpawnObjectRequest request3 = new SpawnObjectRequest(7, player11.GetPosition());
					GameApp.GetInstance().GetNetworkManager().SendRequest(request3);
					break;
				}
				Vector3 position2 = player11.GetPosition();
				GameObject gameObject2 = null;
				gameObject2 = EffectPool.GetInstance().GetEffectPool(EffectPoolType.MINE).CreateObject(position2 + Vector3.up * 0.2f, Vector3.zero, Quaternion.identity);
				Mine component2 = gameObject2.GetComponent<Mine>();
				component2.owner = player11;
				component2.Init();
			}
			break;
		}
		case ESpecialSkillType.IMPACT_WAVE:
		{
			Player player12 = mPlayerSkill.GetPlayer();
			if (player12.IsLocal())
			{
				playerSpecialSkill = new LiteSpecialSkill();
				((LiteSpecialSkill)playerSpecialSkill).parameter1 = mSpecialSkillValues[0];
				((LiteSpecialSkill)playerSpecialSkill).type = ESpecialSkillType.IMPACT_WAVE;
				mPlayerSkill.AddLiteSpecialSkill((LiteSpecialSkill)playerSpecialSkill);
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
				{
					float eX = 0f - GameApp.GetInstance().GetGameWorld().GetCamera()
						.AngelV;
					float angelH = GameApp.GetInstance().GetGameWorld().GetCamera()
						.AngelH;
					SpawnObjectRequest request4 = new SpawnObjectRequest(8, player12.GetPosition(), eX, angelH);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request4);
					break;
				}
				Vector3 position3 = player12.GetPosition();
				GameObject gameObject3 = null;
				Object original2 = Resources.Load(EffectName.IMPACT_WAVE);
				position3 += new Vector3(0f, 1f, 0f);
				gameObject3 = Object.Instantiate(original2, position3, player12.GetTransform().rotation) as GameObject;
				float x = 0f - GameApp.GetInstance().GetGameWorld().GetCamera()
					.AngelV;
				float angelH2 = GameApp.GetInstance().GetGameWorld().GetCamera()
					.AngelH;
				gameObject3.transform.rotation = Quaternion.Euler(x, angelH2, 0f);
				ImpactWave component3 = gameObject3.GetComponent<ImpactWave>();
				component3.owner = player12;
			}
			break;
		}
		case ESpecialSkillType.HOOK:
		{
			Player player15 = mPlayerSkill.GetPlayer();
			if (!player15.IsLocal())
			{
				break;
			}
			playerSpecialSkill = new LiteSpecialSkill();
			((LiteSpecialSkill)playerSpecialSkill).parameter1 = mSpecialSkillValues[0];
			((LiteSpecialSkill)playerSpecialSkill).type = ESpecialSkillType.HOOK;
			mPlayerSkill.AddLiteSpecialSkill((LiteSpecialSkill)playerSpecialSkill);
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				int num3 = 0;
				GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
				if (gameWorld != null)
				{
					if (HUDBase.instance != null)
					{
						GameUnit assistedTarget = HUDBase.instance.m_Aim.GetAssistedTarget();
						if (assistedTarget != null && assistedTarget.GameUnitType == EGameUnitType.REMOTE_PLAYER)
						{
							num3 = (assistedTarget as Player).GetChannelID();
						}
					}
					if (num3 == 0)
					{
						float frontDistance = 100f;
						foreach (RemotePlayer remotePlayer in gameWorld.GetRemotePlayers())
						{
							if (remotePlayer != null && !remotePlayer.IsSameTeam(player15) && remotePlayer.InPlayingState() && GameWorld.CheckEnemyInRange(player15, remotePlayer.GetPosition(), frontDistance, 20f) && remotePlayer.UnitCanBeHitByRay(player15.GetPosition() + new Vector3(0f, 1f, 0f), 100f))
							{
								frontDistance = Vector3.Distance(remotePlayer.GetPosition(), player15.GetPosition());
								num3 = remotePlayer.GetChannelID();
							}
						}
					}
				}
				float eX2 = 0f - GameApp.GetInstance().GetGameWorld().GetCamera()
					.AngelV;
				float angelH3 = GameApp.GetInstance().GetGameWorld().GetCamera()
					.AngelH;
				SpawnObjectRequest request6 = new SpawnObjectRequest(10, player15.GetPosition(), eX2, angelH3, num3);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request6);
			}
			else
			{
				Vector3 position4 = player15.GetPosition();
				GameObject gameObject4 = null;
				Object original3 = Resources.Load(EffectName.HOOK);
				position4 += new Vector3(0f, 1f, 0f);
				gameObject4 = Object.Instantiate(original3, position4, player15.GetTransform().rotation) as GameObject;
				HookScript component4 = gameObject4.GetComponent<HookScript>();
				component4.owner = player15;
				gameObject4.transform.forward = component4.GetWeaponDir(position4);
			}
			break;
		}
		case ESpecialSkillType.MODE_PLAYER_STATE:
			playerSpecialSkill = new LiteSpecialSkill();
			((LiteSpecialSkill)playerSpecialSkill).parameter1 = mSpecialSkillValues[0];
			((LiteSpecialSkill)playerSpecialSkill).parameter2 = mSpecialSkillValues[1];
			((LiteSpecialSkill)playerSpecialSkill).type = ESpecialSkillType.MODE_PLAYER_STATE;
			mPlayerSkill.AddLiteSpecialSkill((LiteSpecialSkill)playerSpecialSkill);
			break;
		case ESpecialSkillType.MISSILE_ATTACK:
		{
			Player player6 = mPlayerSkill.GetPlayer();
			if (player6.IsLocal() && HUDBase.instance != null)
			{
				if (mSpecialSkillValues[0] == 0f)
				{
					player6.GetUserState().Achievement.SetMissleLaunchTime();
					player6.GetUserState().Achievement.missle1count++;
					((HUDOnline)HUDBase.instance).ActiveBombPage(10f, EMissleType.MISSLE);
				}
				else if (mSpecialSkillValues[0] == 1f)
				{
					player6.GetUserState().Achievement.missle2count++;
					((HUDOnline)HUDBase.instance).ActiveBombPage(30f, EMissleType.RANDOM_MISSLE);
				}
				else if (mSpecialSkillValues[0] == 2f)
				{
					player6.GetUserState().Achievement.missle3count++;
					((HUDOnline)HUDBase.instance).ActiveBombPage(10f, EMissleType.TRIPLE_MISSLE);
				}
			}
			break;
		}
		case ESpecialSkillType.AOE_BUFF:
		{
			Player player17 = mPlayerSkill.GetPlayer();
			if (player17.IsLocal() && HUDBase.instance != null)
			{
				((HUDOnline)HUDBase.instance).ActiveBombPage(20f, EMissleType.SLOW_MISSLE, (short)mSpecialSkillValues[0]);
			}
			break;
		}
		case ESpecialSkillType.USE_ITEM_SPEED_UP:
		{
			Player player16 = mPlayerSkill.GetPlayer();
			if (player16.IsLocal())
			{
				playerSpecialSkill = new PlayerSkillUseItemSpeedUp();
				((PlayerSkillUseItemSpeedUp)playerSpecialSkill).Init(this, mPlayerSkill, mSpecialSkillValues[0]);
				mPlayerSkill.AddSpecialSkill(playerSpecialSkill);
			}
			break;
		}
		case ESpecialSkillType.REVENGE_RADAR:
		{
			Player player14 = mPlayerSkill.GetPlayer();
			if (player14.IsLocal())
			{
				LocalPlayer localPlayer = player14 as LocalPlayer;
				if (localPlayer != null)
				{
					localPlayer.TriggerRevengeRadar(mSpecialSkillValues[0]);
				}
			}
			break;
		}
		case ESpecialSkillType.KILL_ALL:
		{
			Player player13 = mPlayerSkill.GetPlayer();
			if (player13.IsLocal())
			{
				SpawnObjectRequest request5 = new SpawnObjectRequest(3, player13.GetPosition());
				GameApp.GetInstance().GetNetworkManager().SendRequest(request5);
				player13.GetUserState().Achievement.CheckAchievemnet_Nuclear();
			}
			break;
		}
		case ESpecialSkillType.RANDOM_PICKUP:
		{
			Player player10 = mPlayerSkill.GetPlayer();
			if (player10.IsLocal())
			{
				player10.GetUserState().Achievement.ResetPickupIDList();
				if (null != RandomPickUpManagerScript.instance)
				{
					RandomPickUpManagerScript.instance.CreateNRandomItems((int)mSpecialSkillValues[0] - 1);
				}
				GameApp.GetInstance().GetGameWorld().CreateARandomItemNearPlayer(BattlePickup.EPickupType.PICKUP);
				player10.GetUserState().Achievement.CheckAchievement_4Pickup();
			}
			break;
		}
		case ESpecialSkillType.AFFECT_ENEMY_PLAYERS:
		{
			Player player9 = mPlayerSkill.GetPlayer();
			if (player9.IsLocal() && mSpecialSkillValues[0] == 7003f)
			{
				mPlayerSkill.GetPlayer().GetUserState().Achievement.CheckAchievement_RadarExpert();
			}
			break;
		}
		case ESpecialSkillType.ACTIVE_LAST_SKILL:
		{
			Player player8 = mPlayerSkill.GetPlayer();
			if (player8.IsLocal())
			{
				mPlayerSkill.ActiveLastSkill();
			}
			break;
		}
		case ESpecialSkillType.GENERATE_PRESENT:
		{
			Player player5 = mPlayerSkill.GetPlayer();
			if (player5.IsLocal() && player5.GetSkills().LastTriggerEnemy != null)
			{
				GameApp.GetInstance().GetGameWorld().CreatePresentNearPlayer(player5.GetSkills().LastTriggerEnemy.GetPosition());
			}
			break;
		}
		case ESpecialSkillType.FLASH_REFILL:
		{
			Player player3 = mPlayerSkill.GetPlayer();
			if (player3.IsLocal())
			{
				Camera.main.gameObject.GetComponentInChildren<ScreenFlashScript>().FlashSeconds(1.5f, FlashRefill);
			}
			break;
		}
		case ESpecialSkillType.SUPER_SHIELD:
		{
			Player player = mPlayerSkill.GetPlayer();
			if (player.IsLocal())
			{
				player.OpenSuperShield(mSpecialSkillValues[0], mSpecialSkillValues[1]);
			}
			break;
		}
		case ESpecialSkillType.GENERATE_VEHICLE:
		case ESpecialSkillType.PLACE_HOLDER2:
		case ESpecialSkillType.PLACE_HOLDER3:
		case ESpecialSkillType.KILL_IN_NEXT_BLOW:
		case ESpecialSkillType.AFFECT_SPECIFIC_PLAYER:
		case ESpecialSkillType.HIT_DONW:
		case ESpecialSkillType.FLASH_BLADE:
		case ESpecialSkillType.CD_REDUCE:
			break;
		}
	}

	private void FlashRefill()
	{
		Player player = mPlayerSkill.GetPlayer();
		UserState userState = GameApp.GetInstance().GetUserState();
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			int num;
			do
			{
				num = Random.Range(1, 16);
			}
			while (num == 9 || num == 4 || num == 7 || num == 11 || num == 13);
			PvpSkill killStreakById = userState.GetPvpSkillManager().GetKillStreakById(num);
			if (player.GetSkills().GetAdvancedSkill(killStreakById.GetExternalID()) == null)
			{
				SkillGroup skillGroup = new SkillGroup();
				skillGroup.InitSkillInfo(killStreakById.GetExternalID());
				skillGroup.InitToPlayerSkill(player.GetSkills());
			}
			if (player.GetSkills().GetAdvancedSkill(killStreakById.GetExternalID()).CanActive())
			{
				player.GetSkills().GetAdvancedSkill(killStreakById.GetExternalID()).ActiveWithoutRepeat();
				KillStreakUsedRequest request = new KillStreakUsedRequest(killStreakById.GetId(), true);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
		else
		{
			int num2 = Random.Range(1, 15);
			GameWorld.TryCreatePickUp(LootType.PICKUP, player.GetPosition(), num2, 0f);
		}
	}

	public void SpecialSkillInactive()
	{
		switch (mSpecialSkillType)
		{
		case ESpecialSkillType.AREA_CONTINUE_DAMAGE:
			mPlayerSkill.RemoveSpecialSkill(playerSpecialSkill);
			break;
		case ESpecialSkillType.BORROWED_TIME:
			if (GameApp.GetInstance().GetGameMode().IsSingle())
			{
				mPlayerSkill.RemoveSpecialSkill(playerSpecialSkill);
			}
			break;
		case ESpecialSkillType.TIME_TO_STACK_WITH_LIMITED:
			((PlayerSkillTimeStackWithLimited)playerSpecialSkill).Uninit();
			mPlayerSkill.RemoveSpecialSkill(playerSpecialSkill);
			break;
		case ESpecialSkillType.EXECUTIONER:
		case ESpecialSkillType.MODE_PLAYER_STATE:
			mPlayerSkill.RemoveLiteSpecialSkill((LiteSpecialSkill)playerSpecialSkill);
			break;
		case ESpecialSkillType.HOOK:
			if (mPlayerSkill.GetPlayer() != null)
			{
				mPlayerSkill.GetPlayer().ClearSuperShieldDamageBooth();
			}
			break;
		}
	}

	public bool IsActived()
	{
		return isActived;
	}

	public bool CanActive()
	{
		Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (!localPlayer.InPlayingState())
		{
			return false;
		}
		if (mSpecialSkillType == ESpecialSkillType.GENERATE_VEHICLE)
		{
			Ray ray = new Ray(localPlayer.GetPosition() + Vector3.up, localPlayer.GetTransform().forward);
			int layerMask = (1 << PhysicsLayer.WALL_AGAINST_PALYER) | (1 << PhysicsLayer.TRANSPARENT_WALL);
			if (Physics.Raycast(ray, 4f, layerMask))
			{
				UserStateHUD.InfoBoxHUD infoBox = UserStateHUD.GetInstance().InfoBox;
				infoBox.SetWarning(LocalizationManager.GetInstance().GetString("LOC_VEHICLE_FAILRLS"));
				return false;
			}
			localPlayer.GetUserState().Achievement.AddCanonCount();
			localPlayer.GetUserState().Achievement.CheckAchievement_EnemyPointCanon();
			return true;
		}
		return true;
	}

	public void PlayVFX()
	{
		if (GameApp.GetInstance().GetGameWorld() != null && GameApp.GetInstance().GetGameWorld().VFXDictionary.ContainsKey(mExternalId))
		{
			if (GameApp.GetInstance().GetGameMode().IsSingle())
			{
				GameApp.GetInstance().GetGameWorld().PlayVFXOnPlayer(mPlayerSkill.GetPlayer(), (short)mExternalId);
			}
			else if (GameApp.GetInstance().GetGameMode().IsCoopMode() && mPlayerSkill.GetPlayer() != null && mPlayerSkill.GetPlayer().IsLocal() && onTriggerType == EOnTriggerType.OPEN_SUPER_SHIELD)
			{
				GameApp.GetInstance().GetGameWorld().PlayVFXOnPlayer(mPlayerSkill.GetPlayer(), (short)mExternalId);
				PlayerPlayVFXRequest request = new PlayerPlayVFXRequest((short)mExternalId, true);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
	}

	public void DestroyVFX()
	{
		if (GameApp.GetInstance().GetGameWorld() != null && GameApp.GetInstance().GetGameWorld().VFXDictionary.ContainsKey(mExternalId))
		{
			if (GameApp.GetInstance().GetGameMode().IsSingle())
			{
				GameApp.GetInstance().GetGameWorld().StopVFXOnPlayer(mPlayerSkill.GetPlayer(), (short)mExternalId);
			}
			else if (GameApp.GetInstance().GetGameMode().IsCoopMode() && mPlayerSkill.GetPlayer() != null && mPlayerSkill.GetPlayer().IsLocal() && onTriggerType == EOnTriggerType.OPEN_SUPER_SHIELD)
			{
				GameApp.GetInstance().GetGameWorld().StopVFXOnPlayer(mPlayerSkill.GetPlayer(), (short)mExternalId);
				PlayerPlayVFXRequest request = new PlayerPlayVFXRequest((short)mExternalId, false);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
	}

	public bool IsHpRestore()
	{
		return mSpecialSkillType == ESpecialSkillType.RESTORE_HP;
	}

	public float[] GetSpecialSkillValues()
	{
		return mSpecialSkillValues;
	}

	public bool IsStaticForTempSkillUI()
	{
		if (onTriggerType == EOnTriggerType.ALWAYS && offTriggerType == EOffTriggerType.NEVER)
		{
			return true;
		}
		if (onTriggerType == EOnTriggerType.EQUIPMENT_TRIGGER)
		{
			bool flag = false;
			if (GameApp.GetInstance().GetUserState().GetBattleWeapons()
				.Count == 1 && GameApp.GetInstance().GetUserState().GetBattleWeapons()[0].GetDamageType() == PlayerDamageType.ENERGY)
			{
				flag = true;
			}
			if (flag && GetOnTriggerValues()[0] == 0f)
			{
				return true;
			}
			return false;
		}
		return false;
	}

	public int GetStackTimes()
	{
		if (mSpecialSkillType == ESpecialSkillType.TIME_TO_STACK_WITH_LIMITED)
		{
			PlayerSkillTimeStackWithLimited playerSkillTimeStackWithLimited = playerSpecialSkill as PlayerSkillTimeStackWithLimited;
			if (playerSkillTimeStackWithLimited != null)
			{
				return playerSkillTimeStackWithLimited.GetStackTimes();
			}
		}
		else if (mSpecialSkillType == ESpecialSkillType.USE_ITEM_SPEED_UP)
		{
			PlayerSkillUseItemSpeedUp playerSkillUseItemSpeedUp = playerSpecialSkill as PlayerSkillUseItemSpeedUp;
			if (playerSkillUseItemSpeedUp != null)
			{
				return playerSkillUseItemSpeedUp.GetStackTimes();
			}
		}
		return 0;
	}

	public void AddStack()
	{
		if (mSpecialSkillType == ESpecialSkillType.USE_ITEM_SPEED_UP)
		{
			PlayerSkillUseItemSpeedUp playerSkillUseItemSpeedUp = playerSpecialSkill as PlayerSkillUseItemSpeedUp;
			if (playerSkillUseItemSpeedUp != null)
			{
				playerSkillUseItemSpeedUp.AddStack();
			}
		}
	}
}
