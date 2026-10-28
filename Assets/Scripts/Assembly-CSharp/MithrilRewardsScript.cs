using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MithrilRewardsScript : MonoBehaviour
{
	public enum IAPFrom
	{
		NORMAL = 0,
		PACKAGE_PUSH = 1
	}

	public static IAPFrom mIapFrom;

	public static MithrilRewardsScript Instance;

	public static bool showDownloadIcon;

	public GameObject UIStateMgr;

	public bool DebugIap;

	private string mCurrentProductId;

	public List<string> mProductIdList;

	private List<IapData> mIapDataList = new List<IapData>();

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Instance = this;
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.IAP_LIST, DataSheetName.IAP_MITRHIL);
		for (int i = 0; i < gameDataTable.Count - 1; i++)
		{
			IapData iapData = new IapData();
			iapData.Type = IapScript.IapType.Mithril;
			iapData.Index = gameDataTable.GetInt(i, 0);
			iapData.Amount = gameDataTable.GetInt(i, 3);
			iapData.ProductID = gameDataTable.GetString(i, 4).ToLower();
			mIapDataList.Add(iapData);
		}
		gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.IAP_LIST, DataSheetName.IAP_GOLD);
		for (int j = 0; j < gameDataTable.Count - 1; j++)
		{
			IapData iapData2 = new IapData();
			iapData2.Type = IapScript.IapType.Gold;
			iapData2.Index = gameDataTable.GetInt(j, 0);
			iapData2.Amount = gameDataTable.GetInt(j, 3);
			iapData2.ProductID = gameDataTable.GetString(j, 4).ToLower();
		}
		gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.IAP_LIST, DataSheetName.IAP_PACKAGE);
		for (int k = 0; k < gameDataTable.Count; k++)
		{
			IapPackageData iapPackageData = new IapPackageData();
			iapPackageData.Type = IapScript.IapType.Package;
			iapPackageData.Index = k;
			iapPackageData.PackageType = (IapScript.IapPackageType)(int)Enum.Parse(typeof(IapScript.IapPackageType), gameDataTable.GetString(k, 3), true);
			iapPackageData.HeadID = gameDataTable.GetInt(k, 4);
			iapPackageData.ChestID = gameDataTable.GetInt(k, 5);
			iapPackageData.HandID = gameDataTable.GetInt(k, 6);
			iapPackageData.FootID = gameDataTable.GetInt(k, 7);
			iapPackageData.BagID1 = gameDataTable.GetInt(k, 8);
			iapPackageData.BagID2 = gameDataTable.GetInt(k, 9);
			iapPackageData.GunID1 = gameDataTable.GetInt(k, 10);
			iapPackageData.GunID2 = gameDataTable.GetInt(k, 11);
			iapPackageData.GunID3 = gameDataTable.GetInt(k, 12);
			iapPackageData.ITEMID1 = gameDataTable.GetInt(k, 13);
			iapPackageData.ITEMID2 = gameDataTable.GetInt(k, 14);
			iapPackageData.ITEMID3 = gameDataTable.GetInt(k, 15);
			iapPackageData.COLOR = gameDataTable.GetInt(k, 16);
			iapPackageData.ProductID = gameDataTable.GetString(k, 17).ToLower();
			mIapDataList.Add(iapPackageData);
		}
		gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.IAP_LIST, DataSheetName.IAP_PVP);
		for (int l = 0; l < gameDataTable.Count; l++)
		{
			IapData iapData3 = new IapData();
			iapData3.Type = IapScript.IapType.PVP;
			iapData3.Index = gameDataTable.GetInt(l, 0);
			iapData3.Amount = gameDataTable.GetInt(l, 3);
			iapData3.ProductID = gameDataTable.GetString(l, 4).ToLower();
			mIapDataList.Add(iapData3);
		}
	}

	public void GETUDID(string udid)
	{
		GameApp.GetInstance().UUID = udid;
	}

	public void RewardMithril(string msg)
	{
		int value = int.Parse(msg);
		value = Mathf.Clamp(value, 0, 300);
		if (GameApp.GetInstance().isGameStart())
		{
			GameApp.GetInstance().GetUserState().AddMithril(value);
			GameApp.GetInstance().GetUserState().AnalysisManager.AddADMithrilAnalysis(value);
			GameApp.GetInstance().Save();
		}
		else
		{
			GameApp.GetInstance().SetMithrilFromTapjoy(value);
		}
	}

	public void RewardCash(string msg)
	{
		int value = int.Parse(msg);
		value = Mathf.Clamp(value, 0, 3001);
		if (value > 0 && value <= 3000)
		{
			GameApp.GetInstance().GetUserState().AddCash(value);
			GameApp.GetInstance().GetUserState().AnalysisManager.AddADCashAnalysis(value);
			GameApp.GetInstance().Save();
		}
	}

	public void SetRewardStatus(string msg)
	{
		int value = int.Parse(msg);
		value = Mathf.Clamp(value, 0, 3);
		GameApp.GetInstance().Save();
	}

	public void ShowDownloadIcon()
	{
		showDownloadIcon = true;
	}

	public void HideDownloadIcon()
	{
		showDownloadIcon = false;
	}

	public void GetReward()
	{
		GameApp.GetInstance().httpRequestSent = false;
		GameApp.GetInstance().StartHttpRequestThread();
	}

	public void StartIAPValidation(string base64)
	{
		switch ((int)AndroidConstant.version)
		{
		case 0:
			StartCoroutine(GetIAPValidation(base64));
			break;
		case 1:
			Debug.Log("start Aws Validate, mCurrentProductId=" + mCurrentProductId);
			IAP.provideContent();
			mProductIdList.Add(mCurrentProductId);
			break;
		}
	}

	public void SetIAPIdentity(string id)
	{
		mCurrentProductId = id;
		Debug.Log("SetIAPIdentity, mCurrentProductId=" + mCurrentProductId);
	}

	public void SetIAPLocalizedPrice(string price)
	{
		foreach (IapData mIapData in mIapDataList)
		{
			if (mIapData.ProductID == mCurrentProductId)
			{
				mIapData.LocalizedPrice = price;
				Debug.Log("......" + mCurrentProductId + "  " + price);
			}
		}
	}

	public string GetIAPLocalizedPrice(string productID)
	{
		foreach (IapData mIapData in mIapDataList)
		{
			if (mIapData.ProductID == productID)
			{
				return mIapData.LocalizedPrice;
			}
		}
		Debug.LogError("No localizedPrice for " + productID);
		return null;
	}

	public bool IsLocalizedPriceInitialized(IapScript.IapType type)
	{
		foreach (IapData mIapData in mIapDataList)
		{
			if (mIapData.Type == type && mIapData.LocalizedPrice == string.Empty)
			{
				return false;
			}
		}
		return true;
	}

	private IEnumerator GetIAPValidation(string base64)
	{
		string id = mCurrentProductId;
		float startTime = Time.time;
		bool timeOut = false;
		WWW iapWWW = null;
		byte[] encodeBytes = Encoding.UTF8.GetBytes(base64);
		string base64EncodedText = Convert.ToBase64String(encodeBytes);
		string base64UrlEncodedText = base64EncodedText.Replace("=", string.Empty).Replace('+', '-').Replace('/', '_');
		switch ((int)AndroidConstant.version)
		{
		case 0:
			iapWWW = new WWW("http://sw1.freyrgames.com:7671/IapStatServer/VerifyAgPay?receipt=" + base64UrlEncodedText + "&appcode=ag006&udid=" + GameApp.GetInstance().UUID);
			break;
		case 1:
			Debug.Log("start Aws Validate");
			iapWWW = new WWW("http://sw1.freyrgames.com:7671/IapStatServer/VerifyAwsPay?receipt=" + base64UrlEncodedText + "&appcode=kg006&udid=" + GameApp.GetInstance().UUID);
			break;
		}
		while (!iapWWW.isDone)
		{
			if (Time.time - startTime > 40f)
			{
				timeOut = true;
				iapWWW.Dispose();
				break;
			}
			yield return new WaitForSeconds(0.5f);
		}
		if (timeOut)
		{
			Debug.Log("...MithrilRewards: http timeout");
			IAP.userCanceled();
		}
		else if (iapWWW.text.Equals("succ"))
		{
			Debug.Log("iap validation succ!");
			Debug.Log("...MithrilRewards: ProvideContent");
			IAP.provideContent();
			mProductIdList.Add(id);
		}
		else
		{
			Debug.Log("iap validation fail!");
			IAP.userCanceled();
		}
	}

	private void Update()
	{
		if (DebugIap)
		{
			DebugIap = false;
			mProductIdList.Add("com.ifreyr.starwarfare2.RookiePack");
		}
		if (mProductIdList.Count <= 0)
		{
			return;
		}
		foreach (string mProductId in mProductIdList)
		{
			IapData iapData = GetIapData(mProductId);
			if (iapData != null)
			{
				ProvideContent(iapData);
			}
		}
		mProductIdList.Clear();
		GameApp.GetInstance().Save();
	}

	public void ProvidePac(string s)
	{
		Debug.Log("ProvidePac = " + s);
		UserState userState = GameApp.GetInstance().GetUserState();
		IapData iapData = GetIapData(s);
		if (iapData.Type != IapScript.IapType.Package)
		{
			return;
		}
		IapPackageData iapPackageData = iapData as IapPackageData;
		if (iapPackageData.PackageType == IapScript.IapPackageType.RookiePack)
		{
			if (!userState.GetRookiePackBought())
			{
				Debug.Log("GetRookiePackBought");
				UnlockArmor(iapPackageData.HeadID);
				UnlockArmor(iapPackageData.ChestID);
				UnlockArmor(iapPackageData.HandID);
				UnlockArmor(iapPackageData.FootID);
				UnlockArmor(iapPackageData.BagID1);
				UnlockArmor(iapPackageData.BagID2);
				UnlockWeapon(iapPackageData.GunID1);
				UnlockWeapon(iapPackageData.GunID2);
				UnlockWeapon(iapPackageData.GunID3);
				userState.SetBattleWeaponID(0, 1);
				if (!userState.GetRookiePackBought())
				{
					if (iapPackageData.ITEMID1 != 0)
					{
						userState.GetItems()[iapPackageData.ITEMID1].AddCount(20);
					}
					if (iapPackageData.ITEMID2 != 0)
					{
						userState.GetItems()[iapPackageData.ITEMID2].AddCount(20);
					}
					if (iapPackageData.ITEMID3 != 0)
					{
						userState.GetItems()[iapPackageData.ITEMID3].AddCount(20);
					}
				}
				userState.SetRookiePackBought(true);
				if (InterMissionScript.GetInstance() != null)
				{
					InterMissionScript.GetInstance().RefreshAfterIap();
				}
				if (LoadoutUIScript.Instance != null && LoadoutUIScript.Instance.LoadoutPage.IsCoopTabSelected())
				{
					LoadoutUIScript.Instance.LoadoutPage.RefreshLoadout();
					if (CustomizeUIScript.mInstance != null)
					{
						CustomizeUIScript.mInstance.Refresh();
					}
				}
			}
		}
		else if (iapPackageData.PackageType == IapScript.IapPackageType.ExpertPack)
		{
			if (!userState.GetExpertPackBought())
			{
				Debug.Log("GetExpertPackBought");
				UnlockArmor(iapPackageData.HeadID);
				UnlockArmor(iapPackageData.ChestID);
				UnlockArmor(iapPackageData.HandID);
				UnlockArmor(iapPackageData.FootID);
				UnlockArmor(iapPackageData.BagID1);
				UnlockArmor(iapPackageData.BagID2);
				UnlockWeapon(iapPackageData.GunID1);
				UnlockWeapon(iapPackageData.GunID2);
				UnlockWeapon(iapPackageData.GunID3);
				userState.SetBattleWeaponID(0, 1);
				if (!userState.GetExpertPackBought())
				{
					if (iapPackageData.ITEMID1 != 0)
					{
						userState.GetItems()[iapPackageData.ITEMID1].AddCount(20);
					}
					if (iapPackageData.ITEMID2 != 0)
					{
						userState.GetItems()[iapPackageData.ITEMID2].AddCount(20);
					}
					if (iapPackageData.ITEMID3 != 0)
					{
						userState.GetItems()[iapPackageData.ITEMID3].AddCount(20);
					}
				}
				userState.SetExpertPackBought(true);
				if (InterMissionScript.GetInstance() != null)
				{
					InterMissionScript.GetInstance().RefreshAfterIap();
				}
				if (LoadoutUIScript.Instance != null && LoadoutUIScript.Instance.LoadoutPage.IsCoopTabSelected())
				{
					LoadoutUIScript.Instance.LoadoutPage.RefreshLoadout();
					if (CustomizeUIScript.mInstance != null)
					{
						CustomizeUIScript.mInstance.Refresh();
					}
				}
			}
		}
		else if (iapPackageData.PackageType == IapScript.IapPackageType.ColorPack)
		{
			if (!userState.GetMultiColorUnlocked())
			{
				userState.SetMultiColorUnlocked(true);
			}
		}
		else if (iapPackageData.PackageType == IapScript.IapPackageType.LevelUnlockPack && !userState.GetUnlockPackBought())
		{
			GameApp.GetInstance().GetUserState().UnlockAllLevel_Weapons_Equips();
			GameApp.GetInstance().GetUserState().SetUnlockPackBought(true);
			if (MissionSelectUIScript.m_Instance != null)
			{
				foreach (MapPanelScript mapPanel in MissionSelectUIScript.m_Instance.m_MapPanels)
				{
					if (mapPanel.m_MapID >= Global.TOTAL_ACTIVATED_STAGE)
					{
						continue;
					}
					mapPanel.CancelLevelButtons();
					foreach (LevelButtonScript levelButton in mapPanel.m_LevelButtons)
					{
						levelButton.Init(mapPanel.m_MapID, levelButton.m_LevelID, mapPanel);
					}
					mapPanel.ResetMissionInfos(mapPanel.m_MapID, 1);
				}
			}
			if (ShopUIScript.GetInstance() != null)
			{
				ShopUIScript.GetInstance().RefreshLockIcon();
			}
		}
		userState.AnalysisManager.IncreasePackageBuy(iapData.Index);
		GameApp.GetInstance().Save();
	}

	public static void ProvideContent(IapData data)
	{
		UserState userState = GameApp.GetInstance().GetUserState();
		if (data.Type == IapScript.IapType.Gold)
		{
			userState.AddCash(data.Amount);
			userState.AnalysisManager.AddGoldBuyNum(data.Index, GameAnalysisManager.currentAnalysisVersion);
			if (GameApp.GetInstance().IsConnectedToServer())
			{
				SyncCashRequest request = new SyncCashRequest();
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
		else if (data.Type == IapScript.IapType.Mithril)
		{
			userState.AddMithril(data.Amount);
			userState.AnalysisManager.AddMithrilBuyNum(data.Index, GameAnalysisManager.currentAnalysisVersion);
			if (GameApp.GetInstance().IsConnectedToServer())
			{
				SyncMithrilRequest request2 = new SyncMithrilRequest();
				GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
			}
		}
		else if (data.Type == IapScript.IapType.Package)
		{
			IapPackageData iapPackageData = data as IapPackageData;
			if (iapPackageData.PackageType == IapScript.IapPackageType.RookiePack)
			{
				if (userState.GetTrialWeaponBuyID() == iapPackageData.GunID1 && userState.GetTrialWeaponBuySeconds() > 0f)
				{
					userState.ClearTrialWeaponBuyInfo();
				}
				if (userState.GetTrialWeaponBuyID() == iapPackageData.GunID2 && userState.GetTrialWeaponBuySeconds() > 0f)
				{
					userState.ClearTrialWeaponBuyInfo();
				}
				if (userState.GetTrialWeaponBuyID() == iapPackageData.GunID3 && userState.GetTrialWeaponBuySeconds() > 0f)
				{
					userState.ClearTrialWeaponBuyInfo();
				}
				UnlockArmor(iapPackageData.HeadID);
				UnlockArmor(iapPackageData.ChestID);
				UnlockArmor(iapPackageData.HandID);
				UnlockArmor(iapPackageData.FootID);
				UnlockArmor(iapPackageData.BagID1);
				UnlockArmor(iapPackageData.BagID2);
				UnlockWeapon(iapPackageData.GunID1);
				UnlockWeapon(iapPackageData.GunID2);
				UnlockWeapon(iapPackageData.GunID3);
				if (!userState.GetRookiePackBought())
				{
					if (iapPackageData.ITEMID1 != 0)
					{
						userState.GetItems()[iapPackageData.ITEMID1].AddCount(20);
					}
					if (iapPackageData.ITEMID2 != 0)
					{
						userState.GetItems()[iapPackageData.ITEMID2].AddCount(20);
					}
					if (iapPackageData.ITEMID3 != 0)
					{
						userState.GetItems()[iapPackageData.ITEMID3].AddCount(20);
					}
				}
				userState.SetRookiePackBought(true);
				if (InterMissionScript.GetInstance() != null)
				{
					InterMissionScript.GetInstance().RefreshAfterIap();
				}
				if (LoadoutUIScript.Instance != null && LoadoutUIScript.Instance.LoadoutPage.IsCoopTabSelected())
				{
					LoadoutUIScript.Instance.LoadoutPage.RefreshLoadout();
					if (CustomizeUIScript.mInstance != null)
					{
						CustomizeUIScript.mInstance.Refresh();
					}
				}
			}
			else if (iapPackageData.PackageType == IapScript.IapPackageType.ExpertPack)
			{
				if (userState.GetTrialWeaponBuyID() == iapPackageData.GunID1 && userState.GetTrialWeaponBuySeconds() > 0f)
				{
					userState.ClearTrialWeaponBuyInfo();
				}
				if (userState.GetTrialWeaponBuyID() == iapPackageData.GunID2 && userState.GetTrialWeaponBuySeconds() > 0f)
				{
					userState.ClearTrialWeaponBuyInfo();
				}
				if (userState.GetTrialWeaponBuyID() == iapPackageData.GunID3 && userState.GetTrialWeaponBuySeconds() > 0f)
				{
					userState.ClearTrialWeaponBuyInfo();
				}
				UnlockArmor(iapPackageData.HeadID);
				UnlockArmor(iapPackageData.ChestID);
				UnlockArmor(iapPackageData.HandID);
				UnlockArmor(iapPackageData.FootID);
				UnlockArmor(iapPackageData.BagID1);
				UnlockArmor(iapPackageData.BagID2);
				UnlockWeapon(iapPackageData.GunID1);
				UnlockWeapon(iapPackageData.GunID2);
				UnlockWeapon(iapPackageData.GunID3);
				if (!userState.GetExpertPackBought())
				{
					if (iapPackageData.ITEMID1 != 0)
					{
						userState.GetItems()[iapPackageData.ITEMID1].AddCount(20);
					}
					if (iapPackageData.ITEMID2 != 0)
					{
						userState.GetItems()[iapPackageData.ITEMID2].AddCount(20);
					}
					if (iapPackageData.ITEMID3 != 0)
					{
						userState.GetItems()[iapPackageData.ITEMID3].AddCount(20);
					}
				}
				userState.SetExpertPackBought(true);
				if (InterMissionScript.GetInstance() != null)
				{
					InterMissionScript.GetInstance().RefreshAfterIap();
				}
				if (LoadoutUIScript.Instance != null && LoadoutUIScript.Instance.LoadoutPage.IsCoopTabSelected())
				{
					LoadoutUIScript.Instance.LoadoutPage.RefreshLoadout();
					if (CustomizeUIScript.mInstance != null)
					{
						CustomizeUIScript.mInstance.Refresh();
					}
				}
			}
			else if (iapPackageData.PackageType == IapScript.IapPackageType.ColorPack)
			{
				userState.SetMultiColorUnlocked(true);
			}
			else if (iapPackageData.PackageType == IapScript.IapPackageType.LevelUnlockPack)
			{
				GameApp.GetInstance().GetUserState().UnlockAllLevel_Weapons_Equips();
				GameApp.GetInstance().GetUserState().SetUnlockPackBought(true);
				if (MissionSelectUIScript.m_Instance != null)
				{
					foreach (MapPanelScript mapPanel in MissionSelectUIScript.m_Instance.m_MapPanels)
					{
						if (mapPanel.m_MapID >= Global.TOTAL_ACTIVATED_STAGE)
						{
							continue;
						}
						mapPanel.CancelLevelButtons();
						foreach (LevelButtonScript levelButton in mapPanel.m_LevelButtons)
						{
							levelButton.Init(mapPanel.m_MapID, levelButton.m_LevelID, mapPanel);
						}
						mapPanel.ResetMissionInfos(mapPanel.m_MapID, 1);
					}
				}
				if (ShopUIScript.GetInstance() != null)
				{
					ShopUIScript.GetInstance().RefreshLockIcon();
				}
			}
			else if (iapPackageData.PackageType == IapScript.IapPackageType.CopperChest || iapPackageData.PackageType == IapScript.IapPackageType.SilverChest || iapPackageData.PackageType == IapScript.IapPackageType.GoldChest)
			{
				Debug.Log("购买成功： " + iapPackageData.PackageType);
				SwVoidDataManager.SendIapChestRequest(iapPackageData.PackageType);
			}
			userState.AnalysisManager.IncreasePackageBuy(data.Index);
		}
		else if (data.Type == IapScript.IapType.PVP)
		{
			if (GameApp.GetInstance().IsConnectedToServer())
			{
				PvpExpIapRequest request3 = new PvpExpIapRequest();
				GameApp.GetInstance().GetNetworkManager().SendRequest(request3);
			}
			else
			{
				SwPvpExpIapWWWRequest request4 = new SwPvpExpIapWWWRequest();
				GameApp.GetInstance().GetWWWManager().SendRequest(request4);
			}
			userState.AnalysisManager.IncreasePackageBuy(7);
		}
		if (mIapFrom == IAPFrom.NORMAL && IapScript.mInstance != null)
		{
			IapScript.mInstance.IAPSuccess(data);
		}
		else if (mIapFrom == IAPFrom.PACKAGE_PUSH && PackagePushScript.mInstance != null)
		{
			PackagePushScript.mInstance.IAPSuccess(data);
		}
		GameApp.GetInstance().Save();
	}

	private IapData GetIapData(string name)
	{
		foreach (IapData mIapData in mIapDataList)
		{
			if (mIapData.ProductID == name)
			{
				return mIapData;
			}
		}
		Debug.LogError("could not find iapdata.");
		return null;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			Debug.Log("Game will Pause!");
			Resources.UnloadUnusedAssets();
			GC.Collect();
		}
	}

	private static void UnlockArmor(int ID)
	{
		if (ID == 0)
		{
			return;
		}
		UserState userState = GameApp.GetInstance().GetUserState();
		Armor armorByUniqueID = userState.GetArmorByUniqueID(ID);
		if (armorByUniqueID != null)
		{
			if (armorByUniqueID.Level == 0 || armorByUniqueID.Level == 15)
			{
				armorByUniqueID.IrregularSetLevel(1, Global.PACKAGE_UNLOCK_VERSION_STRING);
			}
			if (!(LoadoutUIScript.Instance != null) || LoadoutUIScript.Instance.LoadoutPage.IsCoopTabSelected())
			{
				BodyType armorType = armorByUniqueID.GetArmorType();
				short armorID = armorByUniqueID.GetArmorID();
				userState.SetAvatar(armorType, armorID);
			}
		}
	}

	private static void UnlockWeapon(int ID)
	{
		if (ID == 0)
		{
			return;
		}
		UserState userState = GameApp.GetInstance().GetUserState();
		Weapon weapon = userState.GetWeapons()[ID];
		if (weapon != null)
		{
			if (weapon.Level == 0 || weapon.Level == 15)
			{
				weapon.IrregularSetLevel(1, Global.PACKAGE_UNLOCK_VERSION_STRING);
			}
			if (!(LoadoutUIScript.Instance != null) || LoadoutUIScript.Instance.LoadoutPage.IsCoopTabSelected())
			{
				userState.SetBattleWeaponID(0, weapon.GetGunID());
			}
		}
	}
}
