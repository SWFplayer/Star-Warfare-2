using System;
using System.Collections.Generic;
using UnityEngine;

public class UnlockListPageScript : MonoBehaviour
{
	public GameObject RankObj;

	public UISprite RankIcon;

	public UILabel RankName;

	public UILabel RankLevel;

	public GameObject NewEquipTable;

	public GameObject ConfirmButton;

	public GameObject GoCustomButton;

	public UIGrid UnlockList;

	public GameObject UnlockSkillTemplate;

	public GameObject UnlockWeaponTemplate;

	public GameObject UnlockEquipTemplate;

	public GameObject EffectObj;

	public UITweener[] TweenEffects;

	[HideInInspector]
	public static bool IsGoToCustomize;

	private List<PvpSkill> UnlockedSkills = new List<PvpSkill>();

	private List<Weapon> UnlockedWeapons = new List<Weapon>();

	private List<Armor> UnlockedEquips = new List<Armor>();

	private bool mIsForCampain;

	private void Awake()
	{
		RankObj.SetActive(false);
		EffectObj.SetActive(false);
		UIEventListener uIEventListener = UIEventListener.Get(ConfirmButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClicked));
		if (GoCustomButton != null)
		{
			UIEventListener uIEventListener2 = UIEventListener.Get(GoCustomButton);
			uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClicked));
		}
		IsGoToCustomize = false;
	}

	public void OpenByLevelUp(int currentPVPLevel)
	{
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_EQUIPPED);
		mIsForCampain = false;
		base.gameObject.SetActive(true);
		int num = 0;
		UserState userState = GameApp.GetInstance().GetUserState();
		Rank rank = userState.GetRankList()[currentPVPLevel - 1];
		RankObj.SetActive(true);
		RankIcon.spriteName = Rank.GetRankIconName(currentPVPLevel);
		RankLevel.text = Rank.GetRankLevelString(currentPVPLevel);
		RankName.text = LocalizationManager.GetInstance().GetString(rank.rankName);
		if (NewEquipTable != null)
		{
			NewEquipTable.SetActive(false);
		}
		while (UnlockList.transform.childCount > 0)
		{
			UnityEngine.Object.DestroyImmediate(UnlockList.transform.GetChild(0).gameObject);
		}
		UnlockedSkills.Clear();
		PvpSkillManager pvpSkillManager = userState.GetPvpSkillManager();
		foreach (Perk item in pvpSkillManager.GetAllPerk())
		{
			if (item.GetUnlockLevel() == currentPVPLevel)
			{
				UnlockedSkills.Add(item);
				num++;
			}
		}
		foreach (KillStreak item2 in pvpSkillManager.GetAllKillStreak())
		{
			if (item2.GetUnlockLevel() == currentPVPLevel)
			{
				UnlockedSkills.Add(item2);
				num++;
			}
		}
		UnlockedWeapons.Clear();
		foreach (Weapon value in userState.GetWeapons().Values)
		{
			if (value.UnlockPVPLevel == currentPVPLevel)
			{
				UnlockedWeapons.Add(value);
				num++;
			}
		}
		UnlockedEquips.Clear();
		for (int i = 0; i < 5; i++)
		{
			foreach (Armor item3 in userState.GetArmor()[i])
			{
				if (item3.UnlockPVPLevel == currentPVPLevel)
				{
					UnlockedEquips.Add(item3);
					num++;
				}
			}
		}
		if (num > 0)
		{
			GenerateUnlockedContents();
		}
		else
		{
			OnButtonClicked(ConfirmButton);
		}
	}

	public void OpenByStageClear(EDifficultMode diff, byte stage, byte subStage)
	{
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_EQUIPPED);
		mIsForCampain = true;
		base.gameObject.SetActive(true);
		int num = 0;
		UserState userState = GameApp.GetInstance().GetUserState();
		RankObj.SetActive(false);
		if (NewEquipTable != null)
		{
			NewEquipTable.SetActive(true);
		}
		while (UnlockList.transform.childCount > 0)
		{
			UnityEngine.Object.DestroyImmediate(UnlockList.transform.GetChild(0).gameObject);
		}
		UnlockedSkills.Clear();
		UnlockedWeapons.Clear();
		foreach (Weapon value in userState.GetWeapons().Values)
		{
			LevelID unlockLevel = value.UnlockLevel;
			if (unlockLevel.Difficult == diff && unlockLevel.StageID == stage && unlockLevel.SubStageID == subStage)
			{
				UnlockedWeapons.Add(value);
				num++;
			}
		}
		UnlockedEquips.Clear();
		for (int i = 0; i < 5; i++)
		{
			foreach (Armor item in userState.GetArmor()[i])
			{
				LevelID unlockLevel2 = item.UnlockLevel;
				if (unlockLevel2.Difficult == diff && unlockLevel2.StageID == stage && unlockLevel2.SubStageID == subStage)
				{
					UnlockedEquips.Add(item);
					num++;
				}
			}
		}
		if (num > 0)
		{
			GenerateUnlockedContents();
		}
		else
		{
			OnButtonClicked(ConfirmButton);
		}
	}

	private void GenerateUnlockedContents()
	{
		for (int i = 0; i < UnlockedSkills.Count; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(UnlockSkillTemplate) as GameObject;
			gameObject.name += i;
			gameObject.transform.parent = UnlockList.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			string typeString = string.Empty;
			if (UnlockedSkills[i] is Perk)
			{
				typeString = LocalizationManager.GetInstance().GetString("LOC_PERK");
			}
			else if (UnlockedSkills[i] is KillStreak)
			{
				typeString = LocalizationManager.GetInstance().GetString("LOC_KILL_STREAK");
			}
			gameObject.GetComponent<UnlockContentScript>().Init(UnlockedSkills[i].GetIconName(), typeString, UnlockedSkills[i].GetName());
		}
		for (int j = 0; j < UnlockedWeapons.Count; j++)
		{
			GameObject gameObject2 = UnityEngine.Object.Instantiate(UnlockWeaponTemplate) as GameObject;
			gameObject2.name += j;
			gameObject2.transform.parent = UnlockList.transform;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject2.transform.localRotation = Quaternion.identity;
			gameObject2.transform.localScale = Vector3.one;
			string typeString2 = LocalizationManager.GetInstance().GetString("LOC_STORE_WEAPON");
			gameObject2.GetComponent<UnlockContentScript>().Init(UIConstant.EQUIPS_ICON_STRING_PREFIX[5] + UnlockedWeapons[j].GunID, typeString2, UnlockedWeapons[j].Info);
		}
		for (int k = 0; k < UnlockedEquips.Count; k++)
		{
			GameObject gameObject3 = UnityEngine.Object.Instantiate(UnlockEquipTemplate) as GameObject;
			gameObject3.name += k;
			gameObject3.transform.parent = UnlockList.transform;
			gameObject3.transform.localPosition = Vector3.zero;
			gameObject3.transform.localRotation = Quaternion.identity;
			gameObject3.transform.localScale = Vector3.one;
			string typeString3 = string.Empty;
			switch (UnlockedEquips[k].GetArmorType())
			{
			case BodyType.Head:
				typeString3 = LocalizationManager.GetInstance().GetString("LOC_STORE_SHIELD");
				break;
			case BodyType.Body:
				typeString3 = LocalizationManager.GetInstance().GetString("LOC_STORE_SHIELD");
				break;
			case BodyType.Arm:
				typeString3 = LocalizationManager.GetInstance().GetString("LOC_STORE_SHIELD");
				break;
			case BodyType.Foot:
				typeString3 = LocalizationManager.GetInstance().GetString("LOC_STORE_SHIELD");
				break;
			case BodyType.Bag:
				typeString3 = LocalizationManager.GetInstance().GetString("LOC_STORE_BAG");
				break;
			}
			gameObject3.GetComponent<UnlockContentScript>().Init(UIConstant.EQUIPS_ICON_STRING_PREFIX[(int)UnlockedEquips[k].GetArmorType()] + UnlockedEquips[k].GetArmorID(), typeString3, UnlockedEquips[k].Name);
		}
		UnlockList.Reposition();
		EffectObj.SetActive(true);
		UITweener[] tweenEffects = TweenEffects;
		foreach (UITweener uITweener in tweenEffects)
		{
			uITweener.Reset();
			uITweener.PlayForward();
		}
	}

	private void OnButtonClicked(GameObject go)
	{
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		if (go == ConfirmButton)
		{
			if (mIsForCampain)
			{
				if (ResultPageUI.mInstance != null)
				{
					ResultPageUI.mInstance.Exit();
				}
			}
			else
			{
				base.gameObject.SetActive(false);
				EffectObj.SetActive(false);
			}
		}
		else
		{
			if (!(go == GoCustomButton))
			{
				return;
			}
			IsGoToCustomize = true;
			if (mIsForCampain)
			{
				if (ResultPageUI.mInstance != null)
				{
					ResultPageUI.mInstance.Exit();
				}
			}
			else if (PVPResultPageUI.mInstance != null)
			{
				PVPResultPageUI.mInstance.Exit();
			}
		}
	}
}
