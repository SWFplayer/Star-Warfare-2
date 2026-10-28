using System;
using System.Collections.Generic;
using UnityEngine;

public class InterMissionScript : MonoBehaviour, UIMsgListener
{
	public Camera SceneCamera;

	public float CameraRotateDuration = 1.5f;

	public UICamera ShareUICamera;

	public Transform ShopCameraPosition;

	public Transform MissionCameraPosition;

	public Transform CustomCameraPosition;

	public GameObject AvatarPosition;

	public GameObject ShopPosition;

	public GameObject MissionPosition;

	public Transform ShopPositionInit;

	public Transform MissionPositionInit;

	public Transform AvatarLookingMissionPosition;

	public Transform AvatarLookingCustomPosition;

	public Transform AvatarLookingShopPosition;

	public Transform AvatarShowBagPosition;

	public GameObject ShopButton;

	public GameObject MissionButton;

	public GameObject CustomButton;

	public UISprite ShopPageIcon;

	public UISprite ShopPageBackIcon;

	public UISprite MissionPageIcon;

	public UISprite MissionPageBackIcon;

	public UISprite CustomPageIcon;

	public UISprite CustomPageBackIcon;

	public GameObject AvatarFrame;

	public CustomizePlayerStatus StatusObj;

	public Transform StatusPosition_Customize;

	public Transform StatusPosition_Shop;

	public GameObject ExitButton;

	public GameObject FreeMithrilButton;

	private float RandomAnimationTime = 10f;

	private static InterMissionScript mInstance;

	private GameObject mModel;

	private GameObject mWeapon;

	private GameObject mWeapon2;

	private List<float> mAnimTimeList = new List<float>();

	protected bool enableFly;

	protected string idleAnimationName;

	protected float lastRandomAnimationTime;

	protected bool justCreateNewAvatar;

	protected Weapon currentWeapon;

	protected Vector3 scale = Vector3.one;

	protected float lastMotionTime;

	protected Vector3 m_Rotation = new Vector3(0f, 128f, 0f);

	protected Vector3 m_Pos;

	protected ShopUIScript ShopUI;

	protected MissionSelectUIScript MissionUI;

	protected CustomizeUIScript CustomUI;

	private bool mReadyToShowWeapon;

	private bool mReadyToShowEquip;

	private float mWaitForShowWeaponTime = 1f;

	public SwVoid mSwVoid;

	private GameObject mBowSwordAnim;

	private int mBowSwordState;

	public InterMissionPageType CurrentPage { get; set; }

	public static InterMissionScript GetInstance()
	{
		return mInstance;
	}

	private void Awake()
	{
		mInstance = this;
		GameApp.GetInstance().GetUserState().LoadCurrentCustomize(true, 0);
		GameApp.GetInstance().GetGameMode().TypeOfNetwork = NetworkType.Single;
		GameApp.GetInstance().GetGameMode().ModePlay = Mode.CamPain;
		GameApp.GetInstance().ClearGameWorld();
		AvatarFrame.transform.position = AvatarPosition.transform.position;
		CreateDefaultAvatar(false);
		if (CustomUI == null)
		{
			GameObject original = Resources.Load("UI/Menu/Bag/Prefab/CustomizeUI") as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original, new Vector3(-5f, -5f, 10f), Quaternion.identity) as GameObject;
			CustomUI = gameObject.GetComponent<CustomizeUIScript>();
		}
		if (!TutorialManager.GetInstance().GetTutorialWatched(TutorialTriggerType.SHOP_TUTORIAL))
		{
			TutorialManager.GetInstance().Trigger(TutorialTriggerType.SHOP_TUTORIAL);
		}
		else if (!TutorialManager.GetInstance().GetTutorialWatched(TutorialTriggerType.SUIT_SET_TUTORIAL))
		{
			TutorialManager.GetInstance().Trigger(TutorialTriggerType.SUIT_SET_TUTORIAL);
		}
		else if (!TutorialManager.GetInstance().GetTutorialWatched(TutorialTriggerType.MISSION_TUTORIAL))
		{
			TutorialManager.GetInstance().Trigger(TutorialTriggerType.MISSION_TUTORIAL);
		}
		else if (GameApp.GetInstance().GetUserState().GetSelectedStageIndex() == 0 && GameApp.GetInstance().GetUserState().GetSelectedSubStageIndex() == Global.BOSS_SUB_STAGE_ID && !TutorialManager.GetInstance().GetTutorialWatched(TutorialTriggerType.ENDLESS_TUTORIAL))
		{
			TutorialManager.GetInstance().Trigger(TutorialTriggerType.ENDLESS_TUTORIAL);
		}
		else if (!TutorialManager.GetInstance().GetTutorialWatched(TutorialTriggerType.VOID_TUTORIAL))
		{
			TutorialManager.GetInstance().Trigger(TutorialTriggerType.VOID_TUTORIAL);
		}
		else if (GameApp.GetInstance().GetUserState().GetSelectedDifficultMode() == EDifficultMode.HEROIC)
		{
			TutorialManager.GetInstance().Trigger(TutorialTriggerType.HEROIC_MISSION);
		}
		CurrentPage = InterMissionPageType.PageCount;
		ChangePage(InterMissionPageType.Mission);
		UIEventListener uIEventListener = UIEventListener.Get(ShopButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener2 = UIEventListener.Get(MissionButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener3 = UIEventListener.Get(CustomButton);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener4 = UIEventListener.Get(ExitButton);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener5 = UIEventListener.Get(FreeMithrilButton);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		GameObject gameObject2 = GameObject.Find(AudioName.MENU_MUSIC_NAME);
		if (gameObject2 == null)
		{
			GameObject original2 = Resources.Load(AudioName.BGM_MENU_SINGLE) as GameObject;
			gameObject2 = UnityEngine.Object.Instantiate(original2, new Vector3(-0.5f, 1f, 0f), Quaternion.identity) as GameObject;
			gameObject2.name = AudioName.MENU_MUSIC_NAME;
			UnityEngine.Object.DontDestroyOnLoad(gameObject2);
			gameObject2.AddComponent<MenuMusicScript>();
		}
		float num = (float)Screen.width / (float)Screen.height;
		if (num > 1.3333334f)
		{
			SceneCamera.fieldOfView = 60f;
		}
		else
		{
			SceneCamera.fieldOfView = 66f;
		}
		spawnMySwVoidInterMission();
		short[] array = new short[6];
		short[] avatar = GameApp.GetInstance().GetUserState().GetAvatar();
		for (int i = 0; i < avatar.Length; i++)
		{
			array[i] = avatar[i];
		}
		Weapon weapon = GameApp.GetInstance().GetUserState().GetBattleWeapons()[0];
		array[5] = (short)weapon.GetGunID();
		UserState userState = GameApp.GetInstance().GetUserState();
		userState.CheckPackagePushData();
		userState.GetCurrentCustomize().mAvatarColorArray[0].mCurrentEquipID = (byte)array[0];
		userState.GetCurrentCustomize().mAvatarColorArray[1].mCurrentEquipID = (byte)array[1];
		userState.GetCurrentCustomize().mAvatarColorArray[2].mCurrentEquipID = (byte)array[2];
		userState.GetCurrentCustomize().mAvatarColorArray[3].mCurrentEquipID = (byte)array[3];
		UIAtlasManager.UpdateMatOutOfGame();
		userState.GetServerTime();
	}

	private void Start()
	{
	}

	private void OnDestroy()
	{
		mInstance = null;
		Resources.UnloadAsset(UIAtlasManager.Mat_OutOfGame);
		GameApp.GetInstance().GetUserState().SaveCurrentCustomize(true, 0);
		GameApp.GetInstance().Save();
	}

	private void Update()
	{
		if (currentWeapon != null && currentWeapon.GetWeaponType() == WeaponType.BowSword)
		{
			UpdateAvatarBowSword();
		}
		else if (currentWeapon != null && currentWeapon.GetWeaponType() == WeaponType.Sword)
		{
			UpdateAvatarSword();
		}
		else
		{
			UpdateAvatar();
		}
		if (mSwVoid != null)
		{
			mSwVoid.Loop(Time.deltaTime);
		}
	}

	public void UpdateAvatar()
	{
		if (mModel == null)
		{
			return;
		}
		if (mAnimTimeList.Count != 0 && AnimationPlayed(idleAnimationName, mAnimTimeList[0]) && mAnimTimeList[0] <= mAnimTimeList[1])
		{
			currentWeapon.CreateTrajectoryUI(mModel, mWeapon, mWeapon2);
			currentWeapon.PlayFireAnim();
			if (currentWeapon.GetWeaponType() == WeaponType.Fist || currentWeapon.GetWeaponType() == WeaponType.Myth370Fist || currentWeapon.GetWeaponType() == WeaponType.QuadrupleFist || currentWeapon.GetWeaponType() == WeaponType.RailGun)
			{
				List<float> list2;
				List<float> list = (list2 = mAnimTimeList);
				int index2;
				int index = (index2 = 0);
				float num = list2[index2];
				list[index] = num + 0.5f;
			}
			else
			{
				List<float> list4;
				List<float> list3 = (list4 = mAnimTimeList);
				int index2;
				int index3 = (index2 = 0);
				float num = list4[index2];
				list3[index3] = num + currentWeapon.AttackFrequency;
			}
		}
		if (mAnimTimeList.Count != 0 && AnimationPlayed(idleAnimationName, mAnimTimeList[1]))
		{
			currentWeapon.PlayIdleAnim();
			currentWeapon.StopSoundForUI(mModel);
		}
		if (justCreateNewAvatar)
		{
			UpdateAnimation();
			justCreateNewAvatar = false;
		}
		if (currentWeapon != null && currentWeapon.GetWeaponType() == WeaponType.TwinRPG)
		{
			return;
		}
		GameObject gameObject = mModel;
		if (mReadyToShowWeapon)
		{
			if (Time.time - lastMotionTime > mWaitForShowWeaponTime)
			{
				currentWeapon.PlayIdleAnim();
				idleAnimationName = AnimationString.Shop + "_" + currentWeapon.GetAnimationSuffix() + "_" + AnimationString.Show1;
				lastMotionTime = Time.time;
				gameObject.GetComponent<Animation>()[idleAnimationName].wrapMode = WrapMode.Once;
				mReadyToShowWeapon = false;
				if (gameObject.GetComponent<Animation>()[idleAnimationName].time == 0f)
				{
					mAnimTimeList = WeaponShowManager.GetInstance().GetTimeList(currentWeapon, 1);
				}
				gameObject.GetComponent<Animation>().CrossFade(idleAnimationName, 0.6f);
			}
		}
		else if (mReadyToShowEquip)
		{
			if (Time.time - lastMotionTime > mWaitForShowWeaponTime)
			{
				currentWeapon.PlayIdleAnim();
				idleAnimationName = AnimationString.Shop + "_" + currentWeapon.GetAnimationSuffix() + "_" + AnimationString.Show2;
				lastMotionTime = Time.time;
				gameObject.GetComponent<Animation>()[idleAnimationName].wrapMode = WrapMode.Once;
				mReadyToShowEquip = false;
				if (gameObject.GetComponent<Animation>()[idleAnimationName].time == 0f)
				{
					mAnimTimeList = WeaponShowManager.GetInstance().GetTimeList(currentWeapon, 2);
				}
				gameObject.GetComponent<Animation>().CrossFade(idleAnimationName, 0.6f);
			}
		}
		else if (Time.time - lastRandomAnimationTime > RandomAnimationTime && Time.time - lastMotionTime > gameObject.GetComponent<Animation>()[idleAnimationName].length && CurrentPage != InterMissionPageType.Mission)
		{
			currentWeapon.PlayIdleAnim();
			string text = AnimationString.Show1;
			int showNum = 1;
			if (UnityEngine.Random.Range(0, 100) < 50)
			{
				text = AnimationString.Show2;
				showNum = 2;
			}
			idleAnimationName = AnimationString.Shop + "_" + currentWeapon.GetAnimationSuffix() + "_" + text;
			mAnimTimeList = WeaponShowManager.GetInstance().GetTimeList(currentWeapon, showNum);
			gameObject.GetComponent<Animation>().CrossFade(idleAnimationName, 0.6f);
			lastRandomAnimationTime = Time.time;
			lastMotionTime = Time.time;
		}
		if (Time.time - lastMotionTime > gameObject.GetComponent<Animation>()[idleAnimationName].clip.length)
		{
			idleAnimationName = AnimationString.Shop + "_" + currentWeapon.GetAnimationSuffix() + "_" + AnimationString.Idle;
			mAnimTimeList = WeaponShowManager.GetInstance().GetTimeList(currentWeapon, 0);
			gameObject.GetComponent<Animation>().CrossFade(idleAnimationName, 0.2f);
		}
	}

	public void UpdateAvatarSword()
	{
		if (mModel == null)
		{
			return;
		}
		if (mAnimTimeList.Count != 0 && AnimationPlayed(idleAnimationName, mAnimTimeList[0]) && mAnimTimeList[0] <= mAnimTimeList[1])
		{
			currentWeapon.PlayFireAnim();
			List<float> list2;
			List<float> list = (list2 = mAnimTimeList);
			int index2;
			int index = (index2 = 0);
			float num = list2[index2];
			list[index] = num + 10f;
		}
		if (mAnimTimeList.Count != 0 && AnimationPlayed(idleAnimationName, mAnimTimeList[1]))
		{
			currentWeapon.PlayIdleAnim();
			currentWeapon.StopSoundForUI(mModel);
		}
		if (justCreateNewAvatar)
		{
			UpdateAnimation();
			justCreateNewAvatar = false;
		}
		GameObject gameObject = mModel;
		if (mReadyToShowWeapon || mReadyToShowEquip)
		{
			if (Time.time - lastMotionTime > mWaitForShowWeaponTime)
			{
				currentWeapon.PlayIdleAnim();
				idleAnimationName = AnimationString.Shop + "_" + currentWeapon.GetAnimationSuffix() + "_" + AnimationString.Sword_Attack;
				lastMotionTime = Time.time;
				gameObject.GetComponent<Animation>()[idleAnimationName].wrapMode = WrapMode.Once;
				mReadyToShowWeapon = false;
				mReadyToShowEquip = false;
				if (gameObject.GetComponent<Animation>()[idleAnimationName].time == 0f)
				{
					mAnimTimeList = WeaponShowManager.GetInstance().GetTimeList(currentWeapon, 1);
				}
				gameObject.GetComponent<Animation>().CrossFade(idleAnimationName, 0.6f);
			}
		}
		else if (Time.time - lastRandomAnimationTime > RandomAnimationTime && Time.time - lastMotionTime > gameObject.GetComponent<Animation>()[idleAnimationName].length && CurrentPage != InterMissionPageType.Mission)
		{
			currentWeapon.PlayIdleAnim();
			string sword_Attack = AnimationString.Sword_Attack;
			int showNum = 1;
			idleAnimationName = AnimationString.Shop + "_" + currentWeapon.GetAnimationSuffix() + "_" + sword_Attack;
			mAnimTimeList = WeaponShowManager.GetInstance().GetTimeList(currentWeapon, showNum);
			gameObject.GetComponent<Animation>().CrossFade(idleAnimationName, 0.6f);
			lastRandomAnimationTime = Time.time;
			lastMotionTime = Time.time;
		}
		if (Time.time - lastMotionTime > gameObject.GetComponent<Animation>()[idleAnimationName].clip.length)
		{
			idleAnimationName = AnimationString.Shop + "_" + currentWeapon.GetAnimationSuffix() + "_" + AnimationString.Idle;
			mAnimTimeList = WeaponShowManager.GetInstance().GetTimeList(currentWeapon, 0);
			gameObject.GetComponent<Animation>().CrossFade(idleAnimationName, 0.2f);
		}
	}

	public void UpdateAvatarBowSword()
	{
		if (mModel == null)
		{
			return;
		}
		if (mAnimTimeList.Count != 0 && AnimationPlayed(idleAnimationName, mAnimTimeList[0]) && mAnimTimeList[0] <= mAnimTimeList[1])
		{
			currentWeapon.CreateTrajectoryUI(mModel, mWeapon, mWeapon2);
			currentWeapon.PlayFireAnim();
			List<float> list2;
			List<float> list = (list2 = mAnimTimeList);
			int index2;
			int index = (index2 = 0);
			float num = list2[index2];
			list[index] = num + currentWeapon.AttackFrequency;
		}
		if (mAnimTimeList.Count != 0 && AnimationPlayed(idleAnimationName, mAnimTimeList[1]))
		{
			currentWeapon.PlayIdleAnim();
		}
		if (justCreateNewAvatar)
		{
			UpdateAnimation();
			justCreateNewAvatar = false;
		}
		GameObject gameObject = mModel;
		if (mReadyToShowWeapon || mReadyToShowEquip)
		{
			if (Time.time - lastMotionTime > mWaitForShowWeaponTime)
			{
				idleAnimationName = AnimationString.Shop + "_" + currentWeapon.GetAnimationSuffix() + "_" + AnimationString.Sword_Attack;
				mBowSwordState = 1;
				lastMotionTime = Time.time;
				gameObject.GetComponent<Animation>()[idleAnimationName].wrapMode = WrapMode.Once;
				mReadyToShowWeapon = false;
				mAnimTimeList = WeaponShowManager.GetInstance().GetTimeList(currentWeapon, 1);
				gameObject.GetComponent<Animation>().CrossFade(idleAnimationName, 0.3f);
			}
		}
		else
		{
			bool flag = false;
			flag = ((mBowSwordState != 1 && mBowSwordState != 3) ? (Time.time - lastMotionTime > 8f) : (Time.time - lastMotionTime > 4f));
			if (flag && Time.time - lastMotionTime > gameObject.GetComponent<Animation>()[idleAnimationName].clip.length)
			{
				if (CurrentPage != InterMissionPageType.Mission)
				{
					if (mBowSwordState == 1)
					{
						idleAnimationName = AnimationString.Shop + "_" + currentWeapon.GetAnimationSuffix() + "_" + AnimationString.Show1;
						mBowSwordState++;
						mBowSwordAnim.SetActive(true);
						mBowSwordAnim.transform.localPosition = Vector3.zero;
						mBowSwordAnim.GetComponent<Animation>().CrossFade(AnimationString.Show1, 0f);
						mBowSwordAnim.GetComponent<Animation>()[AnimationString.Show1].wrapMode = WrapMode.ClampForever;
						mWeapon.SetActive(false);
					}
					else if (mBowSwordState == 2)
					{
						idleAnimationName = AnimationString.Shop + "_" + currentWeapon.GetAnimationSuffix() + "_" + AnimationString.Sword_Attack;
						mBowSwordState++;
					}
					else if (mBowSwordState == 3)
					{
						idleAnimationName = AnimationString.Shop + "_LaserBow_" + AnimationString.Show2;
						mBowSwordState++;
						mBowSwordAnim.SetActive(true);
						mBowSwordAnim.transform.localPosition = Vector3.zero;
						mBowSwordAnim.GetComponent<Animation>().CrossFade(AnimationString.Show2, 0f);
						mBowSwordAnim.GetComponent<Animation>()[AnimationString.Show1].wrapMode = WrapMode.ClampForever;
						mWeapon2.SetActive(false);
					}
					else if (mBowSwordState == 4 || mBowSwordState == 0)
					{
						idleAnimationName = AnimationString.Shop + "_" + currentWeapon.GetAnimationSuffix() + "_" + AnimationString.Sword_Attack;
						mAnimTimeList = WeaponShowManager.GetInstance().GetTimeList(currentWeapon, 1);
						mBowSwordState = 1;
					}
					gameObject.GetComponent<Animation>().CrossFade(idleAnimationName, 0.3f);
				}
				lastMotionTime = Time.time;
			}
		}
		if (Time.time - lastMotionTime > gameObject.GetComponent<Animation>()[idleAnimationName].clip.length)
		{
			if (mBowSwordState == 2)
			{
				BowSword bowSword = currentWeapon as BowSword;
				bowSword.SetInnerState(BowSword.EInnerState.ESword);
				mWeapon.SetActive(false);
				mBowSwordAnim.SetActive(false);
				mWeapon2.SetActive(true);
				bowSword.UIShowTail();
			}
			else if (mBowSwordState == 4)
			{
				BowSword bowSword2 = currentWeapon as BowSword;
				bowSword2.SetInnerState(BowSword.EInnerState.EBow);
				mWeapon2.SetActive(false);
				mBowSwordAnim.SetActive(false);
				mWeapon.SetActive(true);
			}
			idleAnimationName = AnimationString.Shop + "_" + currentWeapon.GetAnimationSuffix() + "_" + AnimationString.Idle;
			mAnimTimeList = WeaponShowManager.GetInstance().GetTimeList(currentWeapon, 0);
			gameObject.GetComponent<Animation>().CrossFade(idleAnimationName, 0.2f);
		}
	}

	public void UpdateAnimation()
	{
		if (!(mModel == null))
		{
			GameObject gameObject = mModel;
			mAnimTimeList = WeaponShowManager.GetInstance().GetTimeList(currentWeapon, 0);
			gameObject.GetComponent<Animation>().CrossFade(idleAnimationName, 0.2f);
		}
	}

	public void ConfirmEquipAnimation(bool isChangeWeapon)
	{
		if (!(mModel == null))
		{
			GameObject gameObject = mModel;
			if (currentWeapon.GetWeaponType() == WeaponType.BowSword)
			{
				BowSword bowSword = currentWeapon as BowSword;
				bowSword.SetInnerState(BowSword.EInnerState.EBow);
				mWeapon2.SetActive(false);
				mBowSwordAnim.SetActive(false);
				mWeapon.SetActive(true);
				idleAnimationName = AnimationString.Shop + "_" + currentWeapon.GetAnimationSuffix() + "_attack";
				mBowSwordState = 1;
				lastMotionTime = Time.time;
				mAnimTimeList = WeaponShowManager.GetInstance().GetTimeList(currentWeapon, 1);
				gameObject.GetComponent<Animation>().CrossFade(idleAnimationName, 0.3f);
			}
			else if (isChangeWeapon)
			{
				mReadyToShowWeapon = true;
			}
			else
			{
				mReadyToShowEquip = true;
			}
		}
	}

	public void ChangeAvatar(short[] avatar, bool isChangeWeapon)
	{
		bool flag = isChangeWeapon || mWeapon == null;
		if (!flag)
		{
			if (mWeapon != null)
			{
				mWeapon.transform.parent = null;
				mWeapon.transform.localScale = Vector3.one;
			}
			if (mWeapon2 != null)
			{
				mWeapon2.transform.parent = null;
				mWeapon2.transform.localScale = Vector3.one;
			}
			if (mBowSwordAnim != null)
			{
				mBowSwordAnim.transform.parent = null;
				mBowSwordAnim.transform.localScale = Vector3.one;
			}
		}
		ClearModels();
		Armor armor = GameApp.GetInstance().GetUserState().GetArmor(4, avatar[4]);
		enableFly = ((armor.GetSkill(SkillsType.FLY) > 0f) ? true : false);
		currentWeapon = GameApp.GetInstance().GetUserState().GetWeapons()[avatar[5]];
		GameObject gameObject = AvatarBuilder.GetInstance().CreateUIAvatar(avatar);
		gameObject.transform.rotation = Quaternion.Euler(m_Rotation);
		Transform parent = gameObject.transform.Find(BoneName.Weapon);
		if (currentWeapon.GetWeaponType() == WeaponType.BowSword)
		{
			parent = gameObject.transform.Find(BoneName.WeaponL);
		}
		if (flag)
		{
			string path = Global.WEAPON_PATH + "/" + Global.QUALITY_LEVEL_1 + "/" + Global.WEAPON_PREFIX + string.Format("{0:D2}", avatar[5]);
			UnityEngine.Object original = Resources.Load(path);
			mWeapon = (GameObject)UnityEngine.Object.Instantiate(original);
			mWeapon.AddComponent<MaterialChanger>();
			mWeapon.GetComponent<MaterialChanger>().SetMaterialInfo();
		}
		mWeapon.transform.parent = parent;
		mWeapon.transform.localPosition = Vector3.zero;
		mWeapon.transform.localRotation = Quaternion.identity;
		if (currentWeapon.GetWeaponType() == WeaponType.DoublePistol || currentWeapon.GetWeaponType() == WeaponType.ChargeDoublePistol)
		{
			if (flag)
			{
				string path2 = Global.WEAPON_PATH + "/" + Global.QUALITY_LEVEL_1 + "/" + Global.WEAPON_PREFIX + string.Format("{0:D2}", avatar[5]);
				UnityEngine.Object original2 = Resources.Load(path2);
				mWeapon2 = (GameObject)UnityEngine.Object.Instantiate(original2);
				mWeapon2.AddComponent<MaterialChanger>();
				mWeapon2.GetComponent<MaterialChanger>().SetMaterialInfo();
			}
			Transform parent2 = gameObject.transform.Find(BoneName.WeaponL);
			mWeapon2.transform.parent = parent2;
			mWeapon2.transform.localPosition = Vector3.zero;
			mWeapon2.transform.localRotation = Quaternion.identity;
			currentWeapon.UIInit(mWeapon, mWeapon2);
		}
		else if (currentWeapon.GetWeaponType() == WeaponType.BowSword)
		{
			if (flag)
			{
				mBowSwordAnim = Pool.sAllocate("Character/TempWeapon/BowSwordAnim");
				mBowSwordAnim.SetActive(false);
				mWeapon2 = Pool.sAllocate(Global.WEAPON_PATH + "/" + Global.QUALITY_LEVEL_1 + "/Sword");
				mWeapon2.SetActive(false);
				mBowSwordState = 0;
				BowSword bowSword = currentWeapon as BowSword;
				bowSword.SetInnerState(BowSword.EInnerState.EBow);
				mWeapon.SetActive(true);
				mWeapon2.SetActive(false);
				mBowSwordAnim.SetActive(false);
			}
			mBowSwordAnim.transform.parent = gameObject.transform;
			mBowSwordAnim.transform.localPosition = Vector3.zero;
			Transform parent3 = gameObject.transform.Find(BoneName.Weapon);
			mWeapon2.transform.parent = parent3;
			mWeapon2.transform.localPosition = Vector3.zero;
			mWeapon2.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
			currentWeapon.UIInit(mWeapon, mWeapon2);
		}
		else
		{
			currentWeapon.UIInit(mWeapon, null);
		}
		gameObject.transform.localScale = scale;
		SetGameObject(gameObject);
		idleAnimationName = AnimationString.Shop + "_" + currentWeapon.GetAnimationSuffix() + "_" + AnimationString.Idle;
		gameObject.GetComponent<Animation>()[idleAnimationName].wrapMode = WrapMode.Loop;
		gameObject.GetComponent<Animation>().Play(idleAnimationName);
		SetModel(gameObject);
		gameObject.transform.parent = AvatarFrame.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.Euler(m_Rotation);
		gameObject.transform.localScale = scale;
		justCreateNewAvatar = true;
	}

	public void ClearModels()
	{
		if (mModel != null)
		{
			GameObject gameObject = EffectPool.GetInstance().GetEffectPool(EffectPoolType.SHOP_GLOW).CreateObject(mModel.transform.position, mModel.transform.forward, Quaternion.identity);
			gameObject.transform.localScale = Vector3.one * 0.5f;
			NGUITools.SetLayer(gameObject, PhysicsLayer.DEFAULT);
			UnityEngine.Object.Destroy(mModel);
			mModel = null;
		}
	}

	public void SetModel(GameObject obj)
	{
		mModel = obj;
		mModel.transform.position = m_Pos;
	}

	public GameObject GetModel()
	{
		return mModel;
	}

	public void AddMixingTransformAnimation(GameObject playerObj, string weaponSuffix)
	{
	}

	public void SetLowerBodyAnimation(GameObject playerObj, string weaponSuffix)
	{
		if (playerObj.GetComponent<Animation>()[AnimationString.Shop + "_" + weaponSuffix + "_" + AnimationString.Idle] != null)
		{
			playerObj.GetComponent<Animation>()[AnimationString.Shop + "_" + weaponSuffix + "_" + AnimationString.Idle].wrapMode = WrapMode.Loop;
		}
	}

	public void SetGameObject(GameObject playerObj)
	{
		SetLowerBodyAnimation(playerObj, "assault");
		SetLowerBodyAnimation(playerObj, "RPG");
		SetLowerBodyAnimation(playerObj, "shotgun");
		SetLowerBodyAnimation(playerObj, "fist");
	}

	public void CreateDefaultAvatar(bool changeWeapon)
	{
		short[] avatar = GameApp.GetInstance().GetUserState().GetAvatar();
		short[] array = new short[6];
		for (int i = 0; i < avatar.Length; i++)
		{
			array[i] = avatar[i];
		}
		Weapon weapon = GameApp.GetInstance().GetUserState().GetBattleWeapons()[0];
		array[5] = (short)weapon.GunID;
		ChangeAvatar(array, changeWeapon);
	}

	public void ChangePage(InterMissionPageType page)
	{
		if (page == CurrentPage)
		{
			return;
		}
		switch (CurrentPage)
		{
		case InterMissionPageType.Shop:
			ShopPageIcon.spriteName = "shop_n";
			ShopPageBackIcon.spriteName = "btn04_n";
			if (ShopUI != null)
			{
				TweenPosition component = ShopUI.GetComponent<TweenPosition>();
				component.from = ShopPositionInit.position;
				component.to = ShopPosition.transform.position;
				component.PlayReverse();
			}
			ReturnCurrentEquip();
			break;
		case InterMissionPageType.Mission:
			MissionPageIcon.spriteName = "quest_n";
			MissionPageBackIcon.spriteName = "btn04_n";
			if (MissionUI != null)
			{
				TweenPosition component2 = MissionUI.GetComponent<TweenPosition>();
				component2.from = MissionPositionInit.position;
				component2.to = MissionPosition.transform.position;
				component2.PlayReverse();
				MissionUI.SetMoneyStatusObjActive(false);
			}
			break;
		case InterMissionPageType.Custom:
			CustomPageIcon.spriteName = "player_n";
			CustomPageBackIcon.spriteName = "btn04_n";
			if (CustomUI != null)
			{
				CustomUI.Hide();
			}
			break;
		}
		switch (page)
		{
		case InterMissionPageType.Shop:
		{
			ShopPageIcon.spriteName = "shop_u";
			ShopPageBackIcon.spriteName = "btn04_u";
			if (ShopUI == null)
			{
				GameObject original2 = Resources.Load("UI/Menu/Shop/Prefab/ShopUI") as GameObject;
				GameObject gameObject2 = UnityEngine.Object.Instantiate(original2, ShopPositionInit.position, ShopPositionInit.rotation) as GameObject;
				ShopUI = gameObject2.GetComponent<ShopUIScript>();
			}
			else
			{
				ShopUI.ItemList.ShowEquips(ShopUI.CurrentEquipTag);
			}
			TweenTransform.Begin(SceneCamera.gameObject, CameraRotateDuration, SceneCamera.transform, ShopCameraPosition);
			StatusObj.gameObject.SetActive(true);
			TweenTransform.Begin(StatusObj.gameObject, CameraRotateDuration / 2f, StatusPosition_Customize, StatusPosition_Shop);
			if (ShopUI != null && ShopUI.CurrentEquipTag == 4)
			{
				TweenRotation.Begin(AvatarFrame, CameraRotateDuration / 2f, AvatarShowBagPosition.localRotation);
			}
			else
			{
				TweenRotation.Begin(AvatarFrame, CameraRotateDuration / 2f, AvatarLookingShopPosition.localRotation);
			}
			TweenPosition component3 = ShopUI.GetComponent<TweenPosition>();
			component3.from = ShopPositionInit.position;
			component3.to = ShopPosition.transform.position;
			component3.PlayForward();
			FreeMithrilButton.SetActive(true);
			if (GameApp.GetInstance().AppStatus != 1 || !GameApp.GetInstance().isAdInitFinished)
			{
				FreeMithrilButton.SetActive(false);
			}
			if (ShopUI != null && ShopUI.SuitState != null)
			{
				ShopUI.SuitState.RefreshFromEquiped();
			}
			break;
		}
		case InterMissionPageType.Mission:
		{
			MissionPageIcon.spriteName = "quest_u";
			MissionPageBackIcon.spriteName = "btn04_u";
			if (MissionUI == null)
			{
				GameObject original3 = Resources.Load("UI/Menu/MissionUI/Prefab/MissionSelectUI") as GameObject;
				GameObject gameObject3 = UnityEngine.Object.Instantiate(original3, MissionPositionInit.position, MissionPositionInit.rotation) as GameObject;
				MissionUI = gameObject3.GetComponent<MissionSelectUIScript>();
			}
			TweenTransform.Begin(SceneCamera.gameObject, CameraRotateDuration / 2f, SceneCamera.transform, MissionCameraPosition);
			StatusObj.gameObject.SetActive(false);
			TweenRotation.Begin(AvatarFrame, CameraRotateDuration, AvatarLookingMissionPosition.localRotation);
			TweenPosition component4 = MissionUI.GetComponent<TweenPosition>();
			component4.from = MissionPositionInit.position;
			component4.to = MissionPosition.transform.position;
			component4.PlayForward();
			MissionUI.SetMoneyStatusObjActive(true);
			FreeMithrilButton.SetActive(false);
			break;
		}
		case InterMissionPageType.Custom:
			CustomPageIcon.spriteName = "player_u";
			CustomPageBackIcon.spriteName = "btn04_u";
			if (CustomUI == null)
			{
				GameObject original = Resources.Load("UI/Menu/Bag/Prefab/CustomizeUI") as GameObject;
				GameObject gameObject = UnityEngine.Object.Instantiate(original, new Vector3(-5f, -5f, 10f), Quaternion.identity) as GameObject;
				CustomUI = gameObject.GetComponent<CustomizeUIScript>();
			}
			TweenTransform.Begin(SceneCamera.gameObject, CameraRotateDuration, SceneCamera.transform, CustomCameraPosition);
			StatusObj.gameObject.SetActive(true);
			TweenTransform.Begin(StatusObj.gameObject, CameraRotateDuration / 2f, StatusPosition_Shop, StatusPosition_Customize);
			TweenRotation.Begin(AvatarFrame, CameraRotateDuration / 2f, AvatarLookingCustomPosition.localRotation);
			CustomUI.Show();
			CustomUI.CheckWeaponUpgradePush();
			FreeMithrilButton.SetActive(false);
			break;
		}
		CurrentPage = page;
	}

	public void ChangeToMissionPage()
	{
		ChangePage(InterMissionPageType.Mission);
	}

	public void ChangeToShopPage()
	{
		ChangePage(InterMissionPageType.Shop);
	}

	private void OnClickButton(GameObject go)
	{
		if (go == ShopButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_SELECT);
			ChangePage(InterMissionPageType.Shop);
		}
		else if (go == MissionButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_SELECT);
			ChangePage(InterMissionPageType.Mission);
			TutorialManager.GetInstance().Trigger(TutorialTriggerType.MISSION_TUTORIAL);
			if (GameApp.GetInstance().GetUserState().mReadyToShowRate)
			{
				UIMsgBox.instance.ShowReview(this);
				GameApp.GetInstance().GetUserState().mReadyToShowRate = false;
			}
		}
		else if (go == CustomButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_SELECT);
			ChangePage(InterMissionPageType.Custom);
			TutorialManager.GetInstance().Trigger(TutorialTriggerType.COSTUMIZE_TUTORIAL);
		}
		else if (go == ExitButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);

            #if UNITY_EDITOR

                        Debug.Log("Editor, No Need To Restart The Game");
                        LoadLobbyOrMainMenu();
            #else
                
                                        if (Application.platform == RuntimePlatform.WindowsPlayer)
                                        {
                                            Debug.Log("Windows Build Freeze Bug Prevention");
            
                                            GameApp.GetInstance().Save(); 

                                            try
                                            {
                       
                                                string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            
                                                System.Diagnostics.Process.Start(exePath);
                                            }
                                            catch (System.Exception ex)
                                            {
                                                Debug.LogError("Failed to restart game: " + ex.Message);
                                            }
            
                                            Application.Quit();
                                        }
                                        else
                                        {
                    
                                            Debug.Log("Not On Windows Build");
                                            LoadLobbyOrMainMenu();
                                        }
            #endif

            Application.LoadLevel("MainMenu");
		}
		else if (go == FreeMithrilButton)
		{
			GameObject original = Resources.Load("UI/Menu/Ads/Prefab/AdsUI") as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		}
	}

    private void LoadLobbyOrMainMenu()
    {
        Application.LoadLevel("MainMenu");
    }


    public void ShowAvatarBag(bool isShowBag)
	{
		if (isShowBag)
		{
			TweenRotation.Begin(AvatarFrame, CameraRotateDuration / 2f, AvatarShowBagPosition.localRotation);
			return;
		}
		switch (CurrentPage)
		{
		case InterMissionPageType.Custom:
			TweenRotation.Begin(AvatarFrame, CameraRotateDuration / 2f, AvatarLookingCustomPosition.localRotation);
			break;
		case InterMissionPageType.Shop:
			TweenRotation.Begin(AvatarFrame, CameraRotateDuration / 2f, AvatarLookingShopPosition.localRotation);
			break;
		case InterMissionPageType.Mission:
			break;
		}
	}

	public bool AnimationPlayed(string name, float second)
	{
		if (mModel.GetComponent<Animation>()[name] == null)
		{
			return false;
		}
		if (mModel.GetComponent<Animation>()[name].time >= second)
		{
			return true;
		}
		return false;
	}

	public bool AnimationEntirePlayed(string name)
	{
		if (mModel.GetComponent<Animation>()[name] == null)
		{
			return false;
		}
		return mModel.GetComponent<Animation>()[name].time >= mModel.GetComponent<Animation>()[name].length;
	}

	public void ReturnCurrentEquip()
	{
		short[] avatar = GameApp.GetInstance().GetUserState().GetAvatar();
		short[] array = new short[6];
		for (int i = 0; i < avatar.Length; i++)
		{
			array[i] = avatar[i];
		}
		if (CustomUI != null && CustomUI.GetTargetWeaponID() != 0)
		{
			array[5] = (short)CustomUI.GetTargetWeaponID();
		}
		else
		{
			array[5] = (short)GameApp.GetInstance().GetUserState().GetBattleWeapons()[0].GunID;
		}
		ChangeAvatar(array, true);
		StatusObj.SetStatus(array, array);
		if (ShopUI != null)
		{
			ShopUI.ItemList.ResetAvatar(array);
		}
		if (CustomUI != null)
		{
			CustomUI.SelectItemPage.ResetAvatar(array);
		}
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 6)
		{
			UserState userState = GameApp.GetInstance().GetUserState();
			switch (buttonId)
			{
			case UIMsg.UIMsgButton.Ok:
				userState.ResetPlayTimesAfterIgnoreRating(false);
				UIMsgBox.instance.CloseMessage();
				break;
			case UIMsg.UIMsgButton.Cancel:
				userState.ResetPlayTimesAfterIgnoreRating(true);
				UIMsgBox.instance.CloseMessage();
				break;
			case UIMsg.UIMsgButton.Never:
				userState.ResetPlayTimesAfterIgnoreRating(false);
				UIMsgBox.instance.CloseMessage();
				break;
			}
		}
	}

	public void spawnMySwVoidInterMission()
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		SwVoidSavedata swVoidSavedata = null;
		if (userState.SwVoidIdInUsing != 0)
		{
			if (userState.swVoidList.ContainsKey(userState.SwVoidIdInUsing))
			{
				swVoidSavedata = userState.swVoidList[userState.SwVoidIdInUsing];
			}
		}
		else
		{
			swVoidSavedata = null;
		}
		if (swVoidSavedata != null)
		{
			SpawnSwVoidForUI(swVoidSavedata);
		}
		else if (mSwVoid != null)
		{
			mSwVoid.DestroyForUI();
			mSwVoid = null;
		}
	}

	public void SpawnSwVoidForUI(SwVoidSavedata swVoidSavedata)
	{
		ESwVoidType type = swVoidSavedata.GetType();
		GameObject gameObject = null;
		SwVoid swVoid;
		switch (type)
		{
		case ESwVoidType.SOLDIER:
		{
			swVoid = new SwVoidSoldier();
			GameObject original = (swVoidSavedata.IsHighLevel() ? (Resources.Load("Character/SwVoid/L1/V1_Nirvana_02") as GameObject) : (Resources.Load("Character/SwVoid/L1/V1_Nirvana_01") as GameObject));
			gameObject = UnityEngine.Object.Instantiate(original, mModel.transform.position, Quaternion.Euler(270f, 125f, 0f)) as GameObject;
			gameObject.transform.Find(swVoid.GetCenter()).gameObject.AddComponent<Light>().color = new Color32(223, byte.MaxValue, byte.MaxValue, byte.MaxValue);
			break;
		}
		case ESwVoidType.MEDIC:
		{
			swVoid = new SwVoidMedic();
			GameObject original = (swVoidSavedata.IsHighLevel() ? (Resources.Load("Character/SwVoid/L1/V3_Hygeio_02") as GameObject) : (Resources.Load("Character/SwVoid/L1/V3_Hygeio_01") as GameObject));
			gameObject = UnityEngine.Object.Instantiate(original, mModel.transform.position, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
			gameObject.transform.Find(swVoid.GetCenter()).gameObject.AddComponent<Light>().color = new Color32(135, byte.MaxValue, 0, byte.MaxValue);
			break;
		}
		case ESwVoidType.SPY:
		{
			swVoid = new SwVoidSpy();
			GameObject original = Resources.Load("Character/SwVoid/L1/V1_NirvanaSpy") as GameObject;
			gameObject = UnityEngine.Object.Instantiate(original, mModel.transform.position, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
			break;
		}
		case ESwVoidType.SCOUT:
		{
			swVoid = new SwVoidScout();
			GameObject original = (swVoidSavedata.IsHighLevel() ? (Resources.Load("Character/SwVoid/L1/V2_Twilight_02") as GameObject) : (Resources.Load("Character/SwVoid/L1/V2_Twilight_01") as GameObject));
			gameObject = UnityEngine.Object.Instantiate(original, mModel.transform.position, Quaternion.Euler(0f, 180f, 0f)) as GameObject;
			gameObject.transform.Find(swVoid.GetCenter()).gameObject.AddComponent<Light>().color = new Color32(0, 65, byte.MaxValue, byte.MaxValue);
			break;
		}
		case ESwVoidType.HEAVY:
		{
			swVoid = new SwVoidHeavy();
			GameObject original = (swVoidSavedata.IsHighLevel() ? (Resources.Load("Character/SwVoid/L1/V5_Catalyst_02") as GameObject) : (Resources.Load("Character/SwVoid/L1/V5_Catalyst_01") as GameObject));
			gameObject = UnityEngine.Object.Instantiate(original, mModel.transform.position, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
			gameObject.transform.Find(swVoid.GetCenter()).gameObject.AddComponent<Light>().color = new Color32(244, 24, 248, byte.MaxValue);
			break;
		}
		case ESwVoidType.DEMOMAN:
		{
			swVoid = new SwVoidDemoman();
			GameObject original = (swVoidSavedata.IsHighLevel() ? (Resources.Load("Character/SwVoid/L1/V4_Collapsar_02") as GameObject) : (Resources.Load("Character/SwVoid/L1/V4_Collapsar_01") as GameObject));
			gameObject = UnityEngine.Object.Instantiate(original, mModel.transform.position, Quaternion.Euler(0f, 300f, 0f)) as GameObject;
			gameObject.transform.Find(swVoid.GetCenter()).gameObject.AddComponent<Light>().color = new Color32(167, 0, 0, byte.MaxValue);
			break;
		}
		default:
		{
			swVoid = new SwVoidSoldier();
			GameObject original = Resources.Load("_Test/SwVoid0") as GameObject;
			gameObject = UnityEngine.Object.Instantiate(original, mModel.transform.position, Quaternion.Euler(0f, 0f, 0f)) as GameObject;
			break;
		}
		}
		gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
		if (mSwVoid != null)
		{
			mSwVoid.DestroyForUI();
		}
		mSwVoid = swVoid;
		swVoid.SetSavedata(swVoidSavedata);
		swVoid.InitForUI(gameObject);
	}

	public void RefreshAfterIap()
	{
		ReturnCurrentEquip();
		if (CustomUI != null)
		{
			CustomUI.Refresh();
		}
	}

	public void ShowPackagePush(bool isExpert)
	{
		GameObject original = Resources.Load("UI/Menu/IAP/Prefab/PackagePushUI") as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		if (PackagePushScript.mInstance != null)
		{
			PackagePushScript.mInstance.Init(isExpert);
		}
	}

	public void ShowTrialWeaponBuy()
	{
		GameObject original = Resources.Load("UI/Menu/IAP/Prefab/WeaponTrialPushBuy") as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
	}
}
