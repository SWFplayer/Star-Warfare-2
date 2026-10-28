using System.Collections.Generic;
using UnityEngine;

public class PVPSelfStatisticPageScript : MonoBehaviour
{
	private const int RESULT_STATE_INIT = 0;

	private const int RESULT_STATE_TOTAL_EXP = 1;

	private const int RESULT_STATE_FINAL = 10;

	public UILabel PlayerName;

	public UISprite RankIcon;

	public UISprite RankIconEffect;

	public UILabel RankLevel;

	public UISprite EXPForward;

	public GameObject StatisticList;

	public GameObject StatisticItemTemplate;

	public float StatisticSpace = 65f;

	public UnlockListPageScript UnlockPage;

	public UITexture Head;

	public UISprite HeadFrame;

	public GameObject NoScoreTip;

	public GameObject AvatarFrame;

	private GameObject mModel;

	private Vector3 m_Rotation = new Vector3(0f, 128f, 0f);

	private Vector3 m_Pos;

	private Vector3 m_Scale = Vector3.one * 0.7f;

	private string mAnimationName;

	private int mCurrentEXP;

	private int mNewEXP;

	private int mDeltaEXP;

	private float mEXPGrowingTime = 5f;

	private int mCurrentRank;

	private bool mHasLevelUp;

	private UserState mUserState;

	private LocalPlayer mPlayer;

	private bool mInited;

	private int mState = -1;

	public static string[] PrivateInfoLocId;

	private void Awake()
	{
		mPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		mUserState = GameApp.GetInstance().GetUserState();
		mCurrentRank = mUserState.GetRank(mUserState.GetExp()).rankID;
		mCurrentEXP = mUserState.GetExp();
		mNewEXP = mCurrentEXP;
		mDeltaEXP = 0;
		PlayerName.text = mPlayer.GetName();
		SetEXP(mCurrentEXP, false);
		RankIcon.spriteName = Rank.GetRankIconName(mCurrentRank);
		RankLevel.text = Rank.GetRankLevelString(mCurrentRank);
		mInited = false;
		UnlockPage.gameObject.SetActive(false);
		int num = 0;
		for (int i = 0; i < 32; i++)
		{
			int num2 = mPlayer.VSStatistics.expList[i];
			if (num2 > 0)
			{
				num += num2;
			}
		}
		mUserState.AddExp(num);
		GameApp.GetInstance().Save();
		short[] array = new short[6];
		short[] avatar = mUserState.GetAvatar();
		for (int j = 0; j < avatar.Length; j++)
		{
			array[j] = avatar[j];
		}
		Weapon weapon = mUserState.GetBattleWeapons()[0];
		array[5] = (short)weapon.GunID;
		ChangeAvatar(array, false);
	}

	public void SetEXP(int exp, bool changeStateWhenFinish)
	{
		float num = 0f;
		List<Rank> rankList = mUserState.GetRankList();
		mCurrentEXP = exp;
		mCurrentEXP = Mathf.Min(mCurrentEXP, mNewEXP);
		int rankID = mUserState.GetRank(mCurrentEXP).rankID;
		if (rankID > mCurrentRank)
		{
			mCurrentRank = rankID;
			UpdateRankIconEffect();
			mHasLevelUp = true;
			UnlockPage.OpenByLevelUp(mCurrentRank);
		}
		if (mCurrentRank >= rankList.Count)
		{
			num = 1f;
		}
		else
		{
			int exp2 = rankList[mCurrentRank - 1].exp;
			int exp3 = rankList[mCurrentRank].exp;
			num = (float)(mCurrentEXP - exp2) / (float)(exp3 - exp2);
		}
		EXPForward.fillAmount = num;
		if (changeStateWhenFinish && mCurrentEXP >= mNewEXP)
		{
			mState = 10;
			if (!mUserState.DontMindIfRegisted() && mNewEXP >= rankList[4].exp)
			{
				UIMsgBox.instance.ShowMessage(PVPResultPageUI.mInstance, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_4"), 8, 23);
			}
		}
	}

	private void GrowingEXP(int exp, float deltaTime)
	{
		int exp2 = mCurrentEXP + Mathf.CeilToInt((float)exp * deltaTime);
		SetEXP(exp2, true);
	}

	private void UpdateRankIconEffect()
	{
		RankIcon.spriteName = Rank.GetRankIconName(mCurrentRank);
		RankLevel.text = Rank.GetRankLevelString(mCurrentRank);
		RankIconEffect.spriteName = RankIcon.spriteName;
		RankIconEffect.GetComponent<TweenAlpha>().Reset();
		RankIconEffect.GetComponent<TweenAlpha>().PlayForward();
		RankIconEffect.GetComponent<TweenScale>().Reset();
		RankIconEffect.GetComponent<TweenScale>().PlayForward();
	}

	private void CreateStatistics()
	{
		int num = 0;
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PVP_EXP, DataSheetName.PVP_EXP);
		PrivateInfoLocId = new string[32];
		for (int i = 0; i < 32; i++)
		{
			PrivateInfoLocId[i] = gameDataTable.GetString(i, 1);
		}
		for (int j = 0; j < 32; j++)
		{
			int num2 = mPlayer.VSStatistics.expList[j];
			if (num2 > 0)
			{
				GameObject gameObject = Object.Instantiate(StatisticItemTemplate) as GameObject;
				gameObject.transform.parent = StatisticList.transform;
				gameObject.transform.localPosition = new Vector3(0f, (float)(-num) * StatisticSpace, 0f);
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				PVPSelfStatisticItem component = gameObject.GetComponent<PVPSelfStatisticItem>();
				component.Title.text = LocalizationManager.GetInstance().GetString(PrivateInfoLocId[j]);
				component.Value.text = num2.ToString();
				mNewEXP += num2;
				num++;
			}
		}
		if (num == 0)
		{
			NoScoreTip.SetActive(true);
		}
		List<Rank> rankList = mUserState.GetRankList();
		if (mUserState.DontMindIfRegisted())
		{
			mNewEXP = Mathf.Min(mNewEXP, rankList[rankList.Count - 1].exp);
		}
		else
		{
			mNewEXP = Mathf.Min(mNewEXP, rankList[4].exp);
		}
		mDeltaEXP = Mathf.CeilToInt((float)(mNewEXP - mCurrentEXP) / mEXPGrowingTime);
		mState = 1;
		mInited = true;
		GameWorld.RefreshHeadFrame(HeadFrame);
	}

	private void Update()
	{
		int num = mState;
		if (num == 1 && !UnlockPage.gameObject.activeSelf)
		{
			GrowingEXP(mDeltaEXP, RealTime.deltaTime);
		}
		SocialManager.UpdateHead(mPlayer.SocialPlayer, Head);
	}

	public void OpenPage()
	{
		base.gameObject.SetActive(true);
		if (!mInited)
		{
			CreateStatistics();
		}
		else
		{
			SetEXP(mUserState.GetExp(), true);
		}
	}

	public void ChangeAvatar(short[] avatar, bool isChangeWeapon)
	{
		ClearModels();
		Armor armor = GameApp.GetInstance().GetUserState().GetArmor(4, avatar[4]);
		bool flag = ((armor.GetSkill(SkillsType.FLY) > 0f) ? true : false);
		GameObject gameObject = AvatarBuilder.GetInstance().CreateUIAvatar(avatar);
		gameObject.transform.rotation = Quaternion.Euler(m_Rotation);
		Weapon weapon = GameApp.GetInstance().GetUserState().GetWeapons()[avatar[5]];
		Transform parent = gameObject.transform.Find(BoneName.Weapon);
		if (weapon.GetWeaponType() == WeaponType.BowSword)
		{
			parent = gameObject.transform.Find(BoneName.WeaponL);
		}
		string path = Global.WEAPON_PATH + "/" + Global.QUALITY_LEVEL_1 + "/" + Global.WEAPON_PREFIX + string.Format("{0:D2}", avatar[5]);
		Object original = Resources.Load(path);
		GameObject gameObject2 = (GameObject)Object.Instantiate(original);
		gameObject2.transform.parent = parent;
		gameObject2.transform.localPosition = Vector3.zero;
		gameObject2.transform.localRotation = Quaternion.identity;
		gameObject2.AddComponent<MaterialChanger>();
		gameObject2.GetComponent<MaterialChanger>().SetMaterialInfo();
		if (weapon.GetWeaponType() != WeaponType.DoublePistol && weapon.GetWeaponType() != WeaponType.ChargeDoublePistol)
		{
			weapon.UIInit(gameObject2, null);
		}
		else
		{
			Transform parent2 = gameObject.transform.Find(BoneName.WeaponL);
			GameObject gameObject3 = (GameObject)Object.Instantiate(original);
			gameObject3.transform.parent = parent2;
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.localRotation = Quaternion.identity;
			gameObject3.AddComponent<MaterialChanger>();
			gameObject3.GetComponent<MaterialChanger>().SetMaterialInfo();
			weapon.UIInit(gameObject2, gameObject3);
		}
		gameObject.transform.localScale = m_Scale;
		SetGameObject(gameObject);
		mAnimationName = AnimationString.Shop + "_" + weapon.GetAnimationSuffix() + "_" + AnimationString.Idle;
		gameObject.GetComponent<Animation>()[mAnimationName].wrapMode = WrapMode.Loop;
		gameObject.GetComponent<Animation>().Play(mAnimationName);
		SetModel(gameObject);
		gameObject.transform.parent = AvatarFrame.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.Euler(m_Rotation);
		gameObject.transform.localScale = m_Scale;
	}

	public void SetGameObject(GameObject playerObj)
	{
		SetLowerBodyAnimation(playerObj, "assault");
		SetLowerBodyAnimation(playerObj, "RPG");
		SetLowerBodyAnimation(playerObj, "shotgun");
		SetLowerBodyAnimation(playerObj, "fist");
	}

	public void SetLowerBodyAnimation(GameObject playerObj, string weaponSuffix)
	{
		if (playerObj.GetComponent<Animation>()[AnimationString.Shop + "_" + weaponSuffix + "_" + AnimationString.Idle] != null)
		{
			playerObj.GetComponent<Animation>()[AnimationString.Shop + "_" + weaponSuffix + "_" + AnimationString.Idle].wrapMode = WrapMode.Loop;
		}
	}

	public void ClearModels()
	{
		if (mModel != null)
		{
			Object.Destroy(mModel);
			mModel = null;
		}
	}

	public void HideModels()
	{
		if (mModel != null)
		{
			mModel.SetActive(false);
		}
	}

	public void ShowModels()
	{
		if (mModel != null)
		{
			mModel.SetActive(true);
			mModel.GetComponent<Animation>().Play(mAnimationName);
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
}
