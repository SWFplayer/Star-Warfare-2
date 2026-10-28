using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon
{
	public enum ESkillType
	{
		NONE = 0,
		ASSAULT = 1,
		SHOTGUN = 2,
		RPG = 3,
		SWORD = 4,
		SNIPER = 5,
		PISTOL = 6
	}

	public enum EGunParamName
	{
		NAME = 0,
		ID = 1,
		DAMAGE = 2,
		ATTACK_FREQUENCY = 3,
		RANGE = 4,
		BOMB_RANGE = 5,
		SHOTGUN_RANGE = 6,
		SHOTGUN_BULLET_COUNT = 7,
		BULLET_SPEED = 8,
		SPLASH_DAMAGE = 9,
		SPLASH_DURATION = 10,
		SPEED_DRAG = 11,
		ENEGY_CONSUME = 12,
		PRICE = 13,
		MITHRIL = 14,
		WEAPON_TYPE = 15,
		DESCRIPTION = 16,
		UNLOCK_LEVEL = 17,
		UNLOCK_DIFFCULTY = 18,
		UNLOCK_STAGE = 19,
		UNLOCK_SUB_STAGE = 20,
		AIM_SPRITE_STRING = 21,
		DISPLAYER_ORDER = 22,
		DAMAGE_TYPE = 23,
		PVP_SCALE = 24,
		MUT_SCALE = 25,
		BOOTH_TYPE = 26,
		HIT_TRIGGERED_SKILL = 27
	}

	public static string WeaponDefaultVersion = "100";

	public static string WeaponPriceVersion = "104";

	protected bool attacked;

	protected float lastAttackTime;

	protected GameObject hitParticles;

	protected GameObject projectile;

	protected Camera cameraComponent;

	protected Transform cameraTransform;

	protected ThirdPersonStandardCameraScript gameCamera;

	protected Transform gunfire;

	protected Transform mRotatePivot;

	protected Transform mRotatePivotL;

	protected GameObject gun;

	protected GameObject gunfireObj;

	protected Transform weaponBoneTrans;

	protected AudioSource shootAudio;

	protected GameConfig gConfig;

	protected GameWorld gameWorld;

	public Player player;

	protected Vector3 aimTarget;

	protected int gunID = 1;

	protected int rowID = 1;

	protected bool isCDing;

	protected float hitForce;

	protected float range;

	protected float bombRange;

	protected float bulletSpeed;

	protected int maxCapacity;

	protected int maxGunLoad;

	protected int capacity;

	protected int bulletCount;

	protected float maxDeflection;

	protected Vector2 deflection;

	protected float damage;

	protected float damageInit;

	protected float splashDamage;

	protected float splashDamageInit;

	protected float splashDuration;

	protected float bombRangeInit;

	protected float reticleWidth;

	protected float mAttackFrenquency;

	protected float attackFrenquencyInit;

	protected float accuracy;

	protected int enegyConsume = 5;

	protected float speedDrag;

	protected Vector3 lastHitPosition;

	private string _price;

	private string _mithril;

	protected string description;

	protected byte unlockPVPLevel;

	protected LevelID unlockLevel;

	protected byte displayOrder;

	protected PlayerDamageType damageType;

	protected byte level;

	protected bool canUseInPVP;

	protected Vector3 center;

	protected Timer playSoundTimer = new Timer();

	protected byte trialLevel;

	protected bool waitForAttackAnimationStop = true;

	public Renderer[] gunRenderers;

	public ESkillType mSkillType;

	protected static Dictionary<ESkillType, SkillsType> sWeaponBoothDic;

	protected static Dictionary<ESkillType, SkillsType> sWeaponAccDic;

	private static Dictionary<int, WeaponConfig> sWeaponConfigDic = null;

	private static object syncLock = new object();

	protected int mHitTriggeredSkillToOthers;

	protected int mHitTriggeredSkillToSelf;

	protected int staticSkill;

	protected string[] BuyVer = new string[Global.MAX_LEVEL_WEAPONW];

	protected virtual float attackFrenquency
	{
		get
		{
			return mAttackFrenquency;
		}
		set
		{
			mAttackFrenquency = value;
		}
	}

	public float SpeedDrag
	{
		get
		{
			return speedDrag;
		}
		set
		{
			speedDrag = value;
		}
	}

	protected int price
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_price, "shuliang");
		}
		set
		{
			_price = AntiCracking.CryptBufferStr(value, "shuliang");
		}
	}

	protected int mithril
	{
		get
		{
			return AntiCracking.DecryptBufferStr(_mithril, "shuliang");
		}
		set
		{
			_mithril = AntiCracking.CryptBufferStr(value, "shuliang");
		}
	}

	public int DamageLevel { get; set; }

	public int FrequencyLevel { get; set; }

	public int AccuracyLevel { get; set; }

	public bool IsSelectedForBattle { get; set; }

	public WeaponAdjuster Adjuster { get; set; }

	public string FireSoundName { get; set; }

	public string ExplodeSoundName { get; set; }

	public string AimSpriteString { get; set; }

	public int GunID
	{
		get
		{
			return gunID;
		}
		set
		{
			gunID = value;
		}
	}

	public int RowID
	{
		get
		{
			return rowID;
		}
		set
		{
			rowID = value;
		}
	}

	public byte DisplayOrder
	{
		get
		{
			return displayOrder;
		}
	}

	public string Description
	{
		get
		{
			return description;
		}
	}

	public Transform GunFire
	{
		get
		{
			return gunfire;
		}
	}

	public Transform RotatePivot
	{
		get
		{
			return mRotatePivot;
		}
	}

	public Transform RotatePivotL
	{
		get
		{
			return mRotatePivotL;
		}
	}

	public float Dps
	{
		get
		{
			return Damage / AttackFrequency;
		}
		set
		{
			Damage = value * AttackFrequency;
		}
	}

	public string Info
	{
		get
		{
			return LocalizationManager.GetInstance().GetString(Name);
		}
	}

	public string Name { get; set; }

	public int Price
	{
		get
		{
			if (GameApp.GetInstance().GetUserState().GetTrialWeaponBuyID() == GunID && GameApp.GetInstance().GetUserState().GetTrialWeaponBuySeconds() > 0f)
			{
				return Mathf.CeilToInt((float)price * Global.TRIAL_WEAPON_SALE_RATE);
			}
			return price;
		}
	}

	public int Mithril
	{
		get
		{
			if (GameApp.GetInstance().GetUserState().GetTrialWeaponBuyID() == GunID && GameApp.GetInstance().GetUserState().GetTrialWeaponBuySeconds() > 0f)
			{
				return Mathf.CeilToInt((float)mithril * Global.TRIAL_WEAPON_SALE_RATE);
			}
			return mithril;
		}
	}

	public byte Level
	{
		get
		{
			return level;
		}
		set
		{
			level = value;
			if (value == 1)
			{
				BuyVer[0] = WeaponPriceVersion;
				GameApp.GetInstance().GetUserState().SetLastestBuyWeaponID((short)GunID);
			}
		}
	}

	public byte TrialLevel
	{
		get
		{
			return trialLevel;
		}
		set
		{
			trialLevel = value;
		}
	}

	public bool CanUseInPVP
	{
		get
		{
			return canUseInPVP;
		}
		set
		{
			canUseInPVP = value;
		}
	}

	public float Accuracy
	{
		get
		{
			return accuracy;
		}
		set
		{
			accuracy = value;
		}
	}

	public GameObject Gun
	{
		get
		{
			return gun;
		}
		set
		{
			gun = value;
		}
	}

	public Vector3 Center
	{
		get
		{
			return center;
		}
		set
		{
			center = value;
		}
	}

	public virtual SkillsType BoothType
	{
		get
		{
			return sWeaponBoothDic[mSkillType];
		}
	}

	public virtual SkillsType AccType
	{
		get
		{
			return sWeaponAccDic[mSkillType];
		}
	}

	public int MaxGunLoad
	{
		get
		{
			return maxGunLoad;
		}
	}

	public float DamageInit
	{
		get
		{
			return damageInit;
		}
	}

	public float Damage
	{
		get
		{
			return damage;
		}
		set
		{
			damage = value;
		}
	}

	public float AttackFrequency
	{
		get
		{
			return attackFrenquency;
		}
		set
		{
			attackFrenquency = value;
		}
	}

	public float AttackFrenquencyInit
	{
		get
		{
			return attackFrenquencyInit;
		}
	}

	public int EnegyConsume
	{
		get
		{
			return enegyConsume;
		}
		set
		{
			enegyConsume = value;
		}
	}

	public virtual int BulletCount
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public int Capacity
	{
		get
		{
			return capacity;
		}
	}

	public Vector2 Deflection
	{
		get
		{
			return deflection;
		}
	}

	public byte UnlockPVPLevel
	{
		get
		{
			return unlockPVPLevel;
		}
	}

	public LevelID UnlockLevel
	{
		get
		{
			return unlockLevel;
		}
	}

	public Weapon()
	{
	}

	public abstract void Attack(float deltaTime);

	public virtual void StopFire()
	{
	}

	public abstract WeaponType GetWeaponType();

	public bool GetWaitForAttackAnimationStop()
	{
		return waitForAttackAnimationStop;
	}

	public virtual bool IsTypeOfLoopShootingWeapon()
	{
		return true;
	}

	public virtual EAimType GetIdleAimType()
	{
		return EAimType.NONE;
	}

	public virtual EAimType GetAttackAimType()
	{
		return EAimType.NONE;
	}

	public virtual void IdleZoom(float deltaTime)
	{
		GameApp.GetInstance().GetGameWorld().GetCamera()
			.ZoomOut(deltaTime);
	}

	public virtual void AttackZoom(float deltaTime)
	{
		GameApp.GetInstance().GetGameWorld().GetCamera()
			.ZoomIn(deltaTime);
	}

	public virtual void UpdateAttackState(float deltaTime)
	{
		bool flag = ((player.GetSkills().GetSkill(SkillsType.FLY) > 0f) ? true : false);
		InputController inputController = player.inputController;
		WrapMode mode = WrapMode.Loop;
		if (!IsTypeOfLoopShootingWeapon())
		{
			mode = WrapMode.ClampForever;
		}
		if (CoolDown())
		{
			if (player.IsLocal())
			{
				player.Attack();
			}
			else
			{
				CreateTrajectory();
			}
			if (inputController.inputInfo.IsMoving())
			{
				if (flag)
				{
					player.PlayAnimation(AnimationString.GetFlyAniString(inputController.inputInfo.dir), WrapMode.Loop);
					player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack, mode);
				}
				else
				{
					player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
					player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack, mode);
				}
			}
			else if (flag)
			{
				player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
				player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack, mode);
			}
			else
			{
				player.PlayAnimationAllLayers(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack, mode);
			}
		}
		else if (!IsTypeOfLoopShootingWeapon())
		{
			if (inputController.inputInfo.IsMoving())
			{
				if (flag)
				{
					if (player.IsPlayingAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack))
					{
						if (!inputController.inputInfo.fire)
						{
							player.ChangeToIdleState();
						}
					}
					else
					{
						player.PlayAnimation(AnimationString.GetFlyAniString(inputController.inputInfo.dir), WrapMode.Loop);
					}
					if (player.IsPlayingAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack) && player.AnimationPlayed(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack, 1f))
					{
						player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
					}
				}
				else
				{
					player.PlayAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
					if (player.IsPlayingAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack) && player.AnimationPlayed(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack, 1f))
					{
						player.PlayAnimationAllLayers(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
					}
				}
			}
			else if (flag)
			{
				if (player.IsPlayingAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack) && player.AnimationPlayed(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack, 1f))
				{
					player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
					player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
				}
			}
			else if (player.IsPlayingAnimation(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack) && player.AnimationPlayed(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack, 1f))
			{
				player.PlayAnimationAllLayers(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
			}
		}
		if (!inputController.inputInfo.fire && (!GetWaitForAttackAnimationStop() || (!player.IsPlayingAnimationNotFinished(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack) && !player.IsPlayingAnimationNotFinished(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack) && !player.IsPlayingAnimationNotFinished(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack) && !player.IsPlayingAnimationNotFinished(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack))))
		{
			player.ChangeToIdleState();
			if (flag)
			{
				player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
				player.PlayAnimation(AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
			}
			else
			{
				player.PlayAnimationAllLayers(AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
			}
		}
	}

	public virtual void UpdateIdleState(float deltaTime)
	{
		InputController inputController = player.inputController;
		bool flag = player.GetSkills().GetSkill(SkillsType.FLY) > 0f;
		StopFire();
		AutoDestructEffect();
		StopSound();
		PlayIdleAnim();
		if (player.IsLocal())
		{
			if (flag)
			{
				GameApp.GetInstance().GetGameWorld().GetCamera()
					.SetPivotOffset(Vector3.up * 0.25f);
			}
			IdleZoom(deltaTime);
			player.Move(inputController.inputInfo.moveDirection);
		}
		if (inputController.inputInfo.IsMoving())
		{
			if (flag)
			{
				player.PlayAnimation(AnimationString.GetFlyAniString(inputController.inputInfo.dir), WrapMode.Loop);
				player.PlayAnimation(AnimationString.Fly + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
				AudioManager.GetInstance().StopSound("float_idle");
				AudioManager.GetInstance().PlaySoundSingleAt("_Old/_Audio/float_move", player.GetTransform().position);
			}
			else
			{
				player.PlayAnimation(AnimationString.Stand + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
				AudioManager.GetInstance().PlaySoundSingleLoop(AudioName.PLAYER_FOOTSTEP, player.GetObject(), player.GetTransform());
			}
		}
		else if (flag)
		{
			player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
			player.PlayAnimation(AnimationString.Fly + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
			AudioManager.GetInstance().StopSound("float_move");
			AudioManager.GetInstance().PlaySoundSingleAt("_Old/_Audio/float_idle", player.GetTransform().position);
		}
		else
		{
			player.PlayAnimation(AnimationString.Stand + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
			AudioManager.GetInstance().StopSound(AudioName.PLAYER_FOOTSTEP, player.GetObject());
		}
		if (inputController.inputInfo.fire)
		{
			player.SetState(Player.ATTACK_STATE);
		}
	}

	public virtual void EnableGunObject(bool bEnable)
	{
		if (!bEnable)
		{
			gun.SetActiveRecursively(false);
		}
		else
		{
			gun.SetActiveRecursively(true);
		}
	}

	public void SetLevel(byte value)
	{
		level = value;
	}

	public void IrregularSetLevel(byte value, string VerString)
	{
		level = value;
		BuyVer[value - 1] = VerString;
	}

	public abstract string GetAnimationSuffix();

	public abstract string GetAnimationSuffixAlter();

	public virtual void DoLogic()
	{
	}

	public virtual void LoadConfig(byte ownedState)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.WEAPON);
		if (gameDataTable != null)
		{
			Name = gameDataTable.GetString(rowID, 0);
			damageInit = gameDataTable.GetFloat(rowID, 2);
			attackFrenquency = gameDataTable.GetFloat(rowID, 3);
			range = gameDataTable.GetFloat(rowID, 4);
			bombRange = gameDataTable.GetFloat(rowID, 5);
			reticleWidth = gameDataTable.GetFloat(rowID, 6);
			bulletCount = gameDataTable.GetInt(rowID, 7);
			bulletSpeed = gameDataTable.GetFloat(rowID, 8);
			splashDamage = gameDataTable.GetFloat(rowID, 9);
			splashDuration = gameDataTable.GetFloat(rowID, 10);
			speedDrag = gameDataTable.GetFloat(rowID, 11);
			enegyConsume = gameDataTable.GetInt(rowID, 12);
			price = gameDataTable.GetInt(rowID, 13);
			mithril = gameDataTable.GetInt(rowID, 14);
			description = gameDataTable.GetString(rowID, 16);
			unlockPVPLevel = (byte)gameDataTable.GetInt(rowID, 17);
			unlockLevel.Difficult = (EDifficultMode)gameDataTable.GetInt(rowID, 18);
			unlockLevel.StageID = (byte)gameDataTable.GetInt(rowID, 19);
			unlockLevel.SubStageID = (byte)gameDataTable.GetInt(rowID, 20);
			AimSpriteString = gameDataTable.GetString(rowID, 21);
			displayOrder = (byte)gameDataTable.GetInt(rowID, 22);
			damageType = (PlayerDamageType)gameDataTable.GetInt(rowID, 23);
			mSkillType = (ESkillType)gameDataTable.GetInt(rowID, 26);
			string str = gameDataTable.GetString(rowID, 27);
			int[] array = new int[2];
			List<string> list = SwJson.ToJsonArray(str);
			mHitTriggeredSkillToOthers = int.Parse(list[0].ToString());
			mHitTriggeredSkillToSelf = int.Parse(list[1].ToString());
			staticSkill = int.Parse(list[2].ToString());
			damage = damageInit;
			attackFrenquencyInit = attackFrenquency;
			bombRangeInit = bombRange;
			splashDamageInit = splashDamage;
			Adjuster = WeaponDirection.adjusters[rowID];
			if (ownedState != 15)
			{
				for (int i = 0; i < ownedState; i++)
				{
					Upgrade();
				}
			}
		}
		if (sWeaponBoothDic == null)
		{
			sWeaponBoothDic = new Dictionary<ESkillType, SkillsType>();
			sWeaponAccDic = new Dictionary<ESkillType, SkillsType>();
			sWeaponBoothDic[ESkillType.NONE] = SkillsType.SKILL_NONE;
			sWeaponBoothDic[ESkillType.ASSAULT] = SkillsType.ASSAULT_BOOTH;
			sWeaponBoothDic[ESkillType.SHOTGUN] = SkillsType.SHOTGUN_BOOTH;
			sWeaponBoothDic[ESkillType.RPG] = SkillsType.RPG_BOOTH;
			sWeaponBoothDic[ESkillType.SWORD] = SkillsType.SWORD_BOOTH;
			sWeaponBoothDic[ESkillType.SNIPER] = SkillsType.SNIPER_BOOTH;
			sWeaponBoothDic[ESkillType.PISTOL] = SkillsType.PISTOL_BOOTH;
			sWeaponAccDic[ESkillType.NONE] = SkillsType.SKILL_NONE;
			sWeaponAccDic[ESkillType.ASSAULT] = SkillsType.ASSAULT_ACC;
			sWeaponAccDic[ESkillType.SHOTGUN] = SkillsType.SHOTGUN_ACC;
			sWeaponAccDic[ESkillType.RPG] = SkillsType.RPG_ACC;
			sWeaponAccDic[ESkillType.SWORD] = SkillsType.SWORD_ACC;
			sWeaponAccDic[ESkillType.SNIPER] = SkillsType.SNIPER_ACC;
			sWeaponAccDic[ESkillType.PISTOL] = SkillsType.PISTOL_ACC;
		}
	}

	protected virtual void CreateGun()
	{
		string path = Global.WEAPON_PATH + "/" + GetPrefabLevel() + "/" + Global.WEAPON_PREFIX + string.Format("{0:D2}", gunID);
		Object original = Resources.Load(path);
		gun = (GameObject)Object.Instantiate(original);
		GetRenderers();
	}

	public void CreateGunForUI()
	{
		string path = Global.WEAPON_PATH + "/" + Global.QUALITY_LEVEL_1 + "/" + Global.WEAPON_PREFIX + string.Format("{0:D2}", gunID);
		Object original = Resources.Load(path);
		gun = Object.Instantiate(original) as GameObject;
		GetRenderers();
	}

	public float GetSpeedDrag()
	{
		return speedDrag;
	}

	public int GetGunID()
	{
		return gunID;
	}

	public void Upgrade()
	{
		WeaponUpgrade weaponUpgrade = GameApp.GetInstance().GetUserState().weaponUpgrade;
		damage += (float)weaponUpgrade.GetDamage(level) * damageInit / 100f;
		BuyVer[level] = WeaponPriceVersion;
		level++;
		DamageLevel++;
		if (SelectItemPageScript.mInstance != null)
		{
			NotificationManager.GetInstance().SendLocalNotificationForEquip();
		}
		if (level == 8)
		{
			GameApp.GetInstance().GetUserState().Achievement.CheckAchievement_WeaponMaxLevel();
		}
	}

	public void UpgradeByPlan(int newLevel)
	{
		WeaponUpgrade weaponUpgrade = GameApp.GetInstance().GetUserState().weaponUpgrade;
		damage += (float)weaponUpgrade.GetDamage(level) * damageInit / 100f;
		BuyVer[level] = Global.UPGRATE_PUSH_VERSION_STRING + "0" + newLevel;
		level++;
		DamageLevel++;
		if (SelectItemPageScript.mInstance != null)
		{
			NotificationManager.GetInstance().SendLocalNotificationForEquip();
		}
		if (level == 8)
		{
			GameApp.GetInstance().GetUserState().Achievement.CheckAchievement_WeaponMaxLevel();
		}
	}

	public void UpgradeForTrial()
	{
		WeaponUpgrade weaponUpgrade = GameApp.GetInstance().GetUserState().weaponUpgrade;
		damage += (float)weaponUpgrade.GetDamage(trialLevel) * damageInit / 100f;
		trialLevel++;
		DamageLevel++;
	}

	public float GetNextLevelDamage()
	{
		WeaponUpgrade weaponUpgrade = GameApp.GetInstance().GetUserState().weaponUpgrade;
		return damage + damageInit * (float)weaponUpgrade.GetDamage(level);
	}

	public int GetDamageUpgradePrice()
	{
		WeaponUpgrade weaponUpgrade = GameApp.GetInstance().GetUserState().GetWeaponUpgrade();
		int num = level;
		float num2 = price;
		float num3 = (float)weaponUpgrade.GetPrice(num) / 100f;
		float num4 = num2;
		num4 *= num3;
		return (int)num4 / 100 * 100;
	}

	public int GetDamageUpgradePrice(int currentLevel)
	{
		WeaponUpgrade weaponUpgrade = GameApp.GetInstance().GetUserState().GetWeaponUpgrade();
		float num = price;
		float num2 = (float)weaponUpgrade.GetPrice(currentLevel) / 100f;
		float num3 = num;
		num3 *= num2;
		return (int)num3 / 100 * 100;
	}

	public int GetUpgradePriceToLevel(int fromLevel, int toLevel)
	{
		int num = 0;
		for (int i = fromLevel; i < toLevel; i++)
		{
			num += GetDamageUpgradePrice(i);
		}
		return num;
	}

	public void InitInBossStage()
	{
		gameWorld = GameApp.GetInstance().GetGameWorld();
		gameCamera = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
		cameraComponent = gameCamera.GetComponent<Camera>();
		cameraTransform = gameCamera.CameraTransform;
	}

	public virtual void Init(Player player)
	{
		gameWorld = GameApp.GetInstance().GetGameWorld();
		gameCamera = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
		cameraComponent = gameCamera.GetComponent<Camera>();
		cameraTransform = gameCamera.CameraTransform;
		this.player = player;
		hitForce = 0f;
		weaponBoneTrans = player.GetTransform().Find(BoneName.Weapon);
		CreateGun();
		gun.transform.parent = weaponBoneTrans;
		gun.transform.localPosition = Vector3.zero;
		gun.transform.localRotation = Quaternion.identity;
		shootAudio = gun.GetComponent<AudioSource>();
		if (shootAudio == null)
		{
		}
		gunfire = GetGunFire();
		GameObject gameObject = new GameObject("RotatePivot");
		mRotatePivot = gameObject.transform;
		mRotatePivot.parent = gunfire;
		mRotatePivot.localPosition = new Vector3(0f, 0f, 0f - Adjuster.mOffsetStand);
		mRotatePivot.localRotation = Quaternion.identity;
		GunOff();
		AttackFrequency = AttackFrenquencyInit / (1f + GetAccBuff());
		SetWeaponShootSpeed();
	}

	public virtual void UIInit(GameObject weaponObj, GameObject weaponObjL)
	{
	}

	public virtual void DamageBoothByArmor()
	{
		WeaponUpgrade weaponUpgrade = GameApp.GetInstance().GetUserState().GetWeaponUpgrade();
		damage = damageInit;
		splashDamage = splashDamageInit;
		bombRange = bombRangeInit;
		attackFrenquency = attackFrenquencyInit;
		for (int i = 0; i < level; i++)
		{
			damage += (float)weaponUpgrade.GetDamage(i) * damageInit / 100f;
		}
	}

	public virtual float SimpleDamage()
	{
		WeaponUpgrade weaponUpgrade = GameApp.GetInstance().GetUserState().GetWeaponUpgrade();
		float num = damageInit;
		if (level != 0 && level != 15)
		{
			for (int i = 0; i < level; i++)
			{
				num += (float)weaponUpgrade.GetDamage(i) * damageInit / 100f;
			}
		}
		return num;
	}

	public float GetDamageByLevel(int level)
	{
		WeaponUpgrade weaponUpgrade = GameApp.GetInstance().GetUserState().GetWeaponUpgrade();
		float num = damageInit;
		if (level != 0 && level != 15)
		{
			for (int i = 0; i < level; i++)
			{
				num += (float)weaponUpgrade.GetDamage(i) * damageInit / 100f;
			}
		}
		return num;
	}

	public virtual void Loop(float deltaTime)
	{
	}

	public virtual void AutoDestructEffect()
	{
	}

	public virtual void AutoAim(float deltaTime)
	{
	}

	public virtual void PlayIdleAnim()
	{
	}

	public virtual void PlayFireAnim()
	{
	}

	public virtual void StopGunAnim()
	{
	}

	public virtual void PlaySound()
	{
	}

	public virtual void StopSound()
	{
	}

	public virtual void StopSoundForUI(GameObject Avatar)
	{
	}

	public virtual void GetBullet()
	{
		capacity += maxGunLoad;
		capacity = Mathf.Clamp(capacity, 0, maxCapacity);
	}

	public void AddBullets(int num)
	{
		BulletCount += num;
		BulletCount = Mathf.Clamp(BulletCount, 0, 9999);
	}

	public virtual void MaxBullet()
	{
		BulletCount = maxGunLoad;
	}

	public virtual void PlayBlankShootSound()
	{
		AudioManager.GetInstance().PlaySoundSingle("_Old/_Audio/blank/blank_shot01");
	}

	public virtual void CreateTrajectory()
	{
		lastAttackTime = Time.time;
	}

	public virtual void CreateTrajectoryUI(GameObject avatar, GameObject gunObj, GameObject gunObjL)
	{
	}

	public virtual bool CoolDown()
	{
		if (Time.time - lastAttackTime > attackFrenquency)
		{
			return true;
		}
		return false;
	}

	public void SetWeaponShootSpeed()
	{
		if (IsTypeOfLoopShootingWeapon() || player == null || player.GetObject() == null || player.GetObject().GetComponent<Animation>() == null)
		{
			return;
		}
		if (player.GetObject().GetComponent<Animation>()[AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack] != null)
		{
			player.GetObject().GetComponent<Animation>()[AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack].speed = player.GetObject().GetComponent<Animation>()[AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack].clip.length / Mathf.Max(attackFrenquency - 0.3f, 0.1f);
		}
		if (player.GetObject().GetComponent<Animation>()[AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack] != null)
		{
			player.GetObject().GetComponent<Animation>()[AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack].speed = player.GetObject().GetComponent<Animation>()[AnimationString.Fly + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack].clip.length / Mathf.Max(attackFrenquency - 0.3f, 0.1f);
		}
		if (player.GetObject().GetComponent<Animation>()[AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack] != null)
		{
			player.GetObject().GetComponent<Animation>()[AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack].speed = player.GetObject().GetComponent<Animation>()[AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.Attack].clip.length / Mathf.Max(attackFrenquency - 0.3f, 0.1f);
		}
		if (player.GetObject().GetComponent<Animation>()[AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack] != null)
		{
			player.GetObject().GetComponent<Animation>()[AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack].speed = player.GetObject().GetComponent<Animation>()[AnimationString.Stand + "_" + GetAnimationSuffix() + "_" + AnimationString.RunAttack].clip.length / Mathf.Max(attackFrenquency - 0.3f, 0.1f);
		}
		if (GetWeaponType() == WeaponType.BowSword)
		{
			if (player.GetObject().GetComponent<Animation>()[AnimationString.Fly + "_LaserSword_" + AnimationString.Attack] != null)
			{
				player.GetObject().GetComponent<Animation>()[AnimationString.Fly + "_LaserSword_" + AnimationString.Attack].speed = player.GetObject().GetComponent<Animation>()[AnimationString.Fly + "_LaserSword_" + AnimationString.Attack].clip.length / 0.90000004f;
			}
			if (player.GetObject().GetComponent<Animation>()[AnimationString.Fly + "_LaserSword_" + AnimationString.RunAttack] != null)
			{
				player.GetObject().GetComponent<Animation>()[AnimationString.Fly + "_LaserSword_" + AnimationString.RunAttack].speed = player.GetObject().GetComponent<Animation>()[AnimationString.Fly + "_LaserSword_" + AnimationString.RunAttack].clip.length / 0.90000004f;
			}
			if (player.GetObject().GetComponent<Animation>()[AnimationString.Stand + "_LaserSword_" + AnimationString.Attack] != null)
			{
				player.GetObject().GetComponent<Animation>()[AnimationString.Stand + "_LaserSword_" + AnimationString.Attack].speed = player.GetObject().GetComponent<Animation>()[AnimationString.Stand + "_LaserSword_" + AnimationString.Attack].clip.length / 0.90000004f;
			}
			if (player.GetObject().GetComponent<Animation>()[AnimationString.Stand + "_LaserSword_" + AnimationString.RunAttack] != null)
			{
				player.GetObject().GetComponent<Animation>()[AnimationString.Stand + "_LaserSword_" + AnimationString.RunAttack].speed = player.GetObject().GetComponent<Animation>()[AnimationString.Stand + "_LaserSword_" + AnimationString.RunAttack].clip.length / 0.90000004f;
			}
			if (player.GetObject().GetComponent<Animation>()[AnimationString.Fly + "_LaserSword_" + AnimationString.Attack + "01"] != null)
			{
				player.GetObject().GetComponent<Animation>()[AnimationString.Fly + "_LaserSword_" + AnimationString.Attack + "01"].speed = player.GetObject().GetComponent<Animation>()[AnimationString.Fly + "_LaserSword_" + AnimationString.Attack + "01"].clip.length / 0.90000004f;
			}
			if (player.GetObject().GetComponent<Animation>()[AnimationString.Fly + "_LaserSword_" + AnimationString.RunAttack + "01"] != null)
			{
				player.GetObject().GetComponent<Animation>()[AnimationString.Fly + "_LaserSword_" + AnimationString.RunAttack + "01"].speed = player.GetObject().GetComponent<Animation>()[AnimationString.Fly + "_LaserSword_" + AnimationString.RunAttack + "01"].clip.length / 0.90000004f;
			}
			if (player.GetObject().GetComponent<Animation>()[AnimationString.Stand + "_LaserSword_" + AnimationString.Attack + "01"] != null)
			{
				player.GetObject().GetComponent<Animation>()[AnimationString.Stand + "_LaserSword_" + AnimationString.Attack + "01"].speed = player.GetObject().GetComponent<Animation>()[AnimationString.Stand + "_LaserSword_" + AnimationString.Attack + "01"].clip.length / 0.90000004f;
			}
			if (player.GetObject().GetComponent<Animation>()[AnimationString.Stand + "_LaserSword_" + AnimationString.RunAttack + "01"] != null)
			{
				player.GetObject().GetComponent<Animation>()[AnimationString.Stand + "_LaserSword_" + AnimationString.RunAttack + "01"].speed = player.GetObject().GetComponent<Animation>()[AnimationString.Stand + "_LaserSword_" + AnimationString.RunAttack + "01"].clip.length / 0.90000004f;
			}
		}
	}

	public virtual void GunOn()
	{
		if (gun != null)
		{
			Renderer[] array = gunRenderers;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = true;
			}
		}
		if (GetStaticSkillId() != 0)
		{
			player.TriggerSkillFromOtherPlayer(GetStaticSkillId());
		}
		SetWeaponShootSpeed();
	}

	public virtual void GunOff()
	{
		if (gun != null)
		{
			Renderer[] array = gunRenderers;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = false;
			}
		}
		if (GetStaticSkillId() != 0)
		{
			player.TriggerOffSkillFromOtherPlayer(GetStaticSkillId());
		}
		StopFire();
		StopSound();
	}

	public static string GetAnimationName(WeaponType type)
	{
		string result = string.Empty;
		switch (type)
		{
		case WeaponType.AssaultRifle:
		case WeaponType.LaserRifle:
			result = "assault";
			break;
		case WeaponType.RPG:
		case WeaponType.TwinRPG:
		case WeaponType.TrackingRPG:
		case WeaponType.SplitRPG:
			result = "RPG";
			break;
		case WeaponType.ShotGun:
		case WeaponType.LaserShotGun:
			result = "shotgun";
			break;
		case WeaponType.DoublePistol:
		case WeaponType.ChargeDoublePistol:
			result = "DoubleP";
			break;
		case WeaponType.RailGun:
			result = "RailGun";
			break;
		case WeaponType.MachineGun:
			result = "jiatelin";
			break;
		case WeaponType.Sniper:
		case WeaponType.UltraSniper:
			result = "Sniper";
			break;
		case WeaponType.Fist:
		case WeaponType.QuadrupleFist:
		case WeaponType.Myth370Fist:
			result = "fist";
			break;
		default:
			Debug.LogError("No fit WeaponType");
			break;
		}
		return result;
	}

	public static MissionWeaponType GetMissionWeaponType(WeaponType type)
	{
		MissionWeaponType result = MissionWeaponType.NO_GUN;
		switch (type)
		{
		case WeaponType.AssaultRifle:
		case WeaponType.MachineGun:
			result = MissionWeaponType.ASSAULT_RIFLE;
			break;
		case WeaponType.ShotGun:
		case WeaponType.LaserShotGun:
			result = MissionWeaponType.SHOTGUN;
			break;
		case WeaponType.RPG:
		case WeaponType.TwinRPG:
		case WeaponType.TrackingRPG:
		case WeaponType.SplitRPG:
			result = MissionWeaponType.RPG;
			break;
		case WeaponType.LaserRifle:
			result = MissionWeaponType.LASER_RIFLE;
			break;
		case WeaponType.BowSword:
			result = MissionWeaponType.BOW_SWORD;
			break;
		case WeaponType.Fist:
		case WeaponType.QuadrupleFist:
		case WeaponType.Myth370Fist:
			result = MissionWeaponType.FIST;
			break;
		case WeaponType.Sniper:
		case WeaponType.UltraSniper:
		case WeaponType.UltraSniper_Exp:
			result = MissionWeaponType.SNIPER;
			break;
		case WeaponType.RailGun:
			result = MissionWeaponType.RAIL_BLASTER;
			break;
		}
		return result;
	}

	public virtual void ResetDamage(Weapon oldWeapon = null, float damageRate = 1f, float hitRate = 0f)
	{
	}

	protected Ray GetWeaponRay(Transform gunFireTrasform, out Vector3 direction)
	{
		if (player.GameUnitType == EGameUnitType.LOCAL_PLAYER)
		{
			Camera main = Camera.main;
			Transform transform = main.transform;
			ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
			Vector3 vector = default(Vector3);
			bool flag = false;
			if (HUDBase.instance != null && HUDBase.instance.m_Aim.HasAimAssisted)
			{
				flag = true;
				vector = main.ViewportToWorldPoint(HUDBase.instance.m_Aim.AssistedAimPositionInViewport);
			}
			if (!flag)
			{
				vector = main.ScreenToWorldPoint(new Vector3(component.ReticlePosition.x, (float)Screen.height - component.ReticlePosition.y, 0.1f));
			}
			Vector3 normalized = (vector - transform.position).normalized;
			Ray result = new Ray(transform.position + normalized * 1.8f, normalized);
			aimTarget = transform.position + (vector - transform.position).normalized * 80f;
			direction = (aimTarget - gunFireTrasform.position).normalized;
			return result;
		}
		if (player.GameUnitType == EGameUnitType.ROBOT)
		{
			Robot robot = player as Robot;
			direction = robot.AimDirection;
			if (direction.Equals(Vector3.zero))
			{
				direction = player.GetTransform().forward;
			}
			aimTarget = gunFireTrasform.position + direction.normalized * 80f;
			return new Ray(gunFireTrasform.position, direction);
		}
		Debug.LogError("game unit type is wrong: " + player.GameUnitType);
		direction = Vector3.zero;
		return default(Ray);
	}

	protected Ray GetShotGunRay(Transform gunFireTrasform)
	{
		int num = (int)((float)Screen.width * 0.5f * reticleWidth);
		int num2 = (int)((float)Screen.height * 0.5f * reticleWidth);
		int num3 = Random.Range(0, num * 2 + 1) - num;
		int num4 = Random.Range(0, num2 * 2 + 1) - num2;
		Vector3 vector = default(Vector3);
		bool flag = false;
		if (HUDBase.instance != null && HUDBase.instance.m_Aim.HasAimAssisted)
		{
			flag = true;
		}
		if (flag)
		{
			Vector3 vector2 = cameraComponent.ViewportToScreenPoint(HUDBase.instance.m_Aim.AssistedAimPositionInViewport);
			vector = cameraComponent.ScreenToWorldPoint(new Vector3(vector2.x + (float)num3, (float)Screen.height - vector2.y + (float)num4, 0.1f));
		}
		else
		{
			vector = cameraComponent.ScreenToWorldPoint(new Vector3(gameCamera.ReticlePosition.x + (float)num3, (float)Screen.height - gameCamera.ReticlePosition.y + (float)num4, 0.1f));
		}
		Vector3 normalized = (vector - cameraTransform.position).normalized;
		Ray result = default(Ray);
		if (player.GameUnitType == EGameUnitType.LOCAL_PLAYER)
		{
			result = new Ray(cameraTransform.position + normalized * 1.8f, normalized);
		}
		else if (player.GameUnitType == EGameUnitType.ROBOT)
		{
			Robot robot = player as Robot;
			Vector3 vector3 = Vector3.Cross(robot.AimDirection, robot.GetTransform().right);
			Matrix4x4 localToWorldMatrix = player.GetTransform().localToWorldMatrix;
			localToWorldMatrix.SetColumn(1, vector3);
			localToWorldMatrix.SetColumn(2, robot.AimDirection);
			Vector3 v = cameraTransform.InverseTransformDirection(normalized);
			normalized = localToWorldMatrix.MultiplyVector(v);
			result = new Ray(gunFireTrasform.position, normalized);
		}
		return result;
	}

	public virtual PlayerDamageType GetDamageType()
	{
		return damageType;
	}

	public float GetLastAttackTime()
	{
		return lastAttackTime;
	}

	public void SetLastAttackTime(float shareLastAttackTime)
	{
		if (shareLastAttackTime > lastAttackTime)
		{
			lastAttackTime = shareLastAttackTime;
		}
	}

	protected Transform GetGunFire()
	{
		return GetGunFire(gun);
	}

	protected Transform GetGunFire(GameObject gunObj)
	{
		return gunObj.transform.Find("Point01");
	}

	protected virtual void GetRenderers()
	{
		gunRenderers = gun.GetComponentsInChildren<Renderer>();
	}

	public virtual DamageProperty CreateDamageProperty()
	{
		DamageProperty damageProperty = new DamageProperty(player, (int)damage);
		damageProperty.criticalAttack = false;
		if (player.IsLocal())
		{
			damageProperty.isLocal = true;
		}
		else
		{
			damageProperty.isLocal = false;
		}
		damageProperty.wType = GetWeaponType();
		damageProperty.damageType = GetDamageType();
		damageProperty.buff += player.GetSkills().GetSkill(SkillsType.ALL_DAMAGE);
		if (damageType != PlayerDamageType.NORMAL)
		{
			SkillsType sType = (SkillsType)(damageType - 1 + 53);
			damageProperty.buff += player.GetSkills().GetSkill(sType);
		}
		if (BoothType != SkillsType.SKILL_NONE)
		{
			damageProperty.buff += player.GetSkills().GetSkill(BoothType);
		}
		damageProperty.weaponid = gunID;
		damageProperty.buff += player.GetSuperShieldBooth();
		return damageProperty;
	}

	public float GetFinalDamageInUI(PlayerSkill skills)
	{
		float num = SimpleDamage();
		float num2 = 0f;
		num2 += skills.GetSkill(SkillsType.ALL_DAMAGE);
		if (damageType != PlayerDamageType.NORMAL)
		{
			SkillsType sType = (SkillsType)(damageType - 1 + 53);
			num2 += skills.GetSkill(sType);
		}
		num2 += skills.GetSkill(SkillsType.FINAL_DAMAGE);
		return Mathf.CeilToInt(num * (1f + num2));
	}

	public float GetFinalDamageInUI(TempPlayerSkill skills)
	{
		float num = SimpleDamage();
		float num2 = 0f;
		num2 += skills.GetSkill(SkillsType.ALL_DAMAGE);
		if (damageType != PlayerDamageType.NORMAL)
		{
			SkillsType sType = (SkillsType)(damageType - 1 + 53);
			num2 += skills.GetSkill(sType);
		}
		num2 += skills.GetSkill(SkillsType.FINAL_DAMAGE);
		return Mathf.CeilToInt(num * (1f + num2));
	}

	public virtual bool GetInputFireStatus()
	{
		return true;
	}

	public virtual void StartAttackState()
	{
	}

	public virtual void EndAttackState()
	{
	}

	public virtual void StartIdleState()
	{
	}

	public virtual void EndIdleState()
	{
	}

	public float GetAccBuff()
	{
		return player.GetSkills().GetSkill(SkillsType.SHOOT_SPEED) + player.GetSkills().GetSkill(AccType);
	}

	public virtual float GetBombRange()
	{
		return (bombRange + ((mSkillType != ESkillType.RPG) ? 0f : player.GetSkills().GetSkill(SkillsType.RPG_RADIUS_INCREASE))) * (1f + player.GetSkills().GetSkill(SkillsType.RPG_RADIUS_PERCENT_INCREASE));
	}

	public static WeaponConfig sGetWeaponConfigById(int weaponId)
	{
		lock (syncLock)
		{
			if (sWeaponConfigDic == null)
			{
				sWeaponConfigDic = new Dictionary<int, WeaponConfig>();
				GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.WEAPON);
				if (gameDataTable != null)
				{
					for (int i = 0; i < gameDataTable.Count - 1; i++)
					{
						WeaponConfig weaponConfig = new WeaponConfig();
						weaponConfig.mWeaponType = (WeaponType)gameDataTable.GetInt(i, 15);
						weaponConfig.mExplodeRadius = gameDataTable.GetFloat(i, 5);
						weaponConfig.mBulletSpeed = gameDataTable.GetFloat(i, 8);
						int key = gameDataTable.GetInt(i, 1);
						sWeaponConfigDic.Add(key, weaponConfig);
					}
				}
			}
			if (sWeaponConfigDic != null && sWeaponConfigDic.ContainsKey(weaponId))
			{
				return sWeaponConfigDic[weaponId];
			}
			return new WeaponConfig();
		}
	}

	protected void PlayIdleAnimGunOn()
	{
		InputController inputController = player.inputController;
		bool flag = player.GetSkills().GetSkill(SkillsType.FLY) > 0f;
		if (inputController != null && inputController.inputInfo.IsMoving())
		{
			if (flag)
			{
				player.PlayAnimation(AnimationString.GetFlyAniString(inputController.inputInfo.dir), WrapMode.Loop);
				player.PlayAnimation(AnimationString.Fly + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
			}
			else
			{
				player.PlayAnimation(AnimationString.Stand + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
			}
		}
		else if (flag)
		{
			player.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
			player.PlayAnimation(AnimationString.Fly + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
		}
		else
		{
			player.PlayAnimation(AnimationString.Stand + "_" + player.GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
		}
	}

	public string GetPrefabLevel()
	{
		string result = Global.QUALITY_LEVEL_1;
		if ((GameApp.GetInstance().GetGameMode().IsVSMode() && player.GameUnitType == EGameUnitType.REMOTE_PLAYER && GameApp.GetInstance().GetUserState().mGraphicQuality.mCapbility <= DeviceCaps.Capbility.Middle) || GameApp.GetInstance().GetUserState().mGraphicQuality.mCapbility <= DeviceCaps.Capbility.Low)
		{
			result = "L3";
		}
		return result;
	}

	public int GetHitTriggeredSkillToOthers()
	{
		return mHitTriggeredSkillToOthers;
	}

	public int GetHitTriggeredSkillToSelf()
	{
		return mHitTriggeredSkillToSelf;
	}

	public int GetStaticSkillId()
	{
		return staticSkill;
	}

	public string[] GetBuyVer()
	{
		return BuyVer;
	}

	public void SetBuyVer(string[] buyVer)
	{
		BuyVer = buyVer;
	}

	public bool EquippedUnlockPatch()
	{
		if (level == 0 || level == 15)
		{
			byte b = 0;
			string[] buyVer = BuyVer;
			foreach (string text in buyVer)
			{
				if (text.Equals(WeaponDefaultVersion))
				{
					break;
				}
				b++;
			}
			if (b > 0)
			{
				level = b;
				return true;
			}
		}
		return false;
	}

	public virtual void CheatCheck()
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.WEAPON);
		float num = gameDataTable.GetFloat(rowID, 3);
		byte b = 0;
		if (AttackFrenquencyInit < num)
		{
			b++;
		}
		if (player != null && player.GetSkills() != null)
		{
			float accBuff = GetAccBuff();
			if (accBuff > 2.25f)
			{
				b += 2;
			}
		}
		if (player != null)
		{
			if (attackFrenquency * 1.05f < num / (GetAccBuff() + 1f))
			{
				b += 4;
			}
		}
		else if (attackFrenquency < num)
		{
			b += 4;
		}
		if (b != 0 && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			CheatCheckRequest request = new CheatCheckRequest(CheatType.WEAPON_SPEED_RATE, attackFrenquency, b, (byte)gunID);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
		float obj = gameDataTable.GetFloat(rowID, 11);
		if (!GetSpeedDrag().Equals(obj) && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			CheatCheckRequest request2 = new CheatCheckRequest(CheatType.PLAYER_SPEED, GetSpeedDrag(), 0, (byte)gunID);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
		}
	}

	public void setParam(float _range, float _shotRange, int _bullecCount, float _bulletSpeed)
	{
		bombRange = (bombRangeInit = _range);
		reticleWidth = _shotRange;
		bulletCount = _bullecCount;
		bulletSpeed = _bulletSpeed;
	}

	public virtual void CheatCheckParam()
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.WEAPON);
		if (gameDataTable == null)
		{
			return;
		}
		if (bombRange > 0f)
		{
			float obj = gameDataTable.GetFloat(rowID, 5);
			if (!bombRangeInit.Equals(obj))
			{
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
				{
					CheatCheckRequest request = new CheatCheckRequest(CheatType.BOMB_RANGE, bombRangeInit, 0, (byte)gunID);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
			}
			else if (!bombRange.Equals(obj) && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				CheatCheckRequest request2 = new CheatCheckRequest(CheatType.BOMB_RANGE, bombRange, 0, (byte)gunID);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
			}
		}
		if (GetWeaponType() == WeaponType.ShotGun || GetWeaponType() == WeaponType.LaserShotGun)
		{
			float obj2 = gameDataTable.GetFloat(rowID, 6);
			if (!reticleWidth.Equals(obj2) && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				CheatCheckRequest request3 = new CheatCheckRequest(CheatType.SHOT_GUN_RANGE, reticleWidth, 0, (byte)gunID);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request3);
			}
			int num = gameDataTable.GetInt(rowID, 7);
			if (bulletCount != num && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				CheatCheckRequest request4 = new CheatCheckRequest(CheatType.BULLET_COUNT, bulletCount, 0, (byte)gunID);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request4);
			}
		}
		if (GetWeaponType() == WeaponType.RPG || GetWeaponType() == WeaponType.TwinRPG || GetWeaponType() == WeaponType.TrackingRPG || GetWeaponType() == WeaponType.SplitRPG || GetWeaponType() == WeaponType.BowSword || GetWeaponType() == WeaponType.TrackingRPG || GetWeaponType() == WeaponType.Fist || GetWeaponType() == WeaponType.Myth370Fist || GetWeaponType() == WeaponType.QuadrupleFist)
		{
			float obj3 = gameDataTable.GetFloat(rowID, 8);
			if (!bulletSpeed.Equals(obj3) && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				CheatCheckRequest request5 = new CheatCheckRequest(CheatType.BULLET_SPEED, bulletSpeed, 0, (byte)gunID);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request5);
			}
		}
	}

	public void ChangeToTrialWeapon()
	{
		trialLevel = 1;
		damage = damageInit;
		for (int i = 1; i < Global.TRIAL_WEAPON_LEVEL; i++)
		{
			UpgradeForTrial();
		}
	}

	public void ReturnFromTrialWeapon()
	{
		level = 15;
		damage = damageInit;
	}

	public void ReturnFromTrialBuyWeapon()
	{
		if (level == 15)
		{
			level = 0;
			damage = damageInit;
		}
	}

	public virtual void ClearEffect()
	{
	}
}
