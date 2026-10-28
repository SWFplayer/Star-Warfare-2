using System;
using System.Collections.Generic;
using UnityEngine;

public class IapScript : MonoBehaviour, UIMsgListener
{
	public enum IapType
	{
		Package = 0,
		Mithril = 1,
		Gold = 2,
		PVP = 3,
		Nothing = 4
	}

	public enum IapPackageType
	{
		RookiePack = 0,
		ExpertPack = 1,
		ColorPack = 2,
		LevelUnlockPack = 3,
		CopperChest = 4,
		SilverChest = 5,
		GoldChest = 6
	}

	public static IapScript mInstance;

	public UISprite ItemPageSprite;

	public UISprite MithrilPageSprite;

	public UISprite GoldPageSprite;

	public GameObject GoodsList;

	public float GoodsSpace;

	public float PacketSpace;

	public GameObject GoodsTemplate;

	public GameObject PacketsTemplate;

	public UIGrid GoodsBars;

	public GameObject BarTemplate;

	public GameObject ExitButton;

	public GameObject PointLeft;

	public GameObject PointLeftMiddle;

	public GameObject PointMiddle;

	public GameObject PointRightMiddle;

	public GameObject PointRight;

	public Vector3 StartPosition = new Vector3(440f, 0f, 0f);

	public GameObject RestoreButton;

	public GameObject RestoreHighlight;

	public GameObject MaterialChestOkButton;

	public GameObject MaterialChestObj;

	public GameObject MaterialTemplate;

	public GameObject MaterialPanel;

	public GameObject AllMaterialListObj;

	public UIGrid AllMaterialList;

	public GameObject AllMaterialItemTemplate;

	public GameObject ComfirmBuyMaterial;

	public GameObject CloseBuyMaterial;

	private List<SwVoidMaterialInfo> materialInfoList = new List<SwVoidMaterialInfo>();

	private List<SwVoidMaterialInfo> randomMaterialChestInfo = new List<SwVoidMaterialInfo>();

	[HideInInspector]
	public SwVoidMaterialInfo lastSelected;

	public GameObject allMaterialLineTwo;

	public List<GameObject> materialAddObjectList;

	public List<GameObject> materialAmountObjectList;

	public UILabel MaterialIapChestPrice;

	public IapGoodsScript FreeMithrilGoods;

	[HideInInspector]
	public List<UIDailyRewardInfo> materialChestInfo = new List<UIDailyRewardInfo>();

	[HideInInspector]
	public List<IapGoodsScript> m_Goods = new List<IapGoodsScript>();

	[HideInInspector]
	public List<UISprite> m_Bars = new List<UISprite>();

	protected UICenterOnChild mCenterOnChild;

	private float mCheckCenterChildTime = 0.015f;

	private float mLastCheckTime;

	private IapGoodsScript mLastCenterGoods;

	protected float XLeft;

	protected float XLeftMiddle;

	protected float XMiddle;

	protected float XRightMiddle;

	protected float XRight;

	private bool needUpdate = true;

	private bool mIsWaitingIAPResult;

	private float mWaitingIAPResultTimeOutLimit = 120f;

	private float mWaitingIAPResultStartTime;

	private bool mIsWaitingIAPInfo;

	private float mWaitingIAPInfoTimeOutLimit = 15f;

	private float mWaitingIAPInfoStartTime;

	private IapGoodsScript buyingIapGoods;

	public IapType CurrentPage { get; set; }

	public bool IsGotoUnlockIAP { get; set; }

	public bool IsGotoMaterialChest { get; set; }

	public bool IsGotoColorPack { get; set; }

	public bool IsGotoEquipmentSuitPackage { get; set; }

	public IapPackageType IsGotoAllMaterialList { get; set; }

	private void Awake()
	{
		mInstance = this;
		mCenterOnChild = GoodsList.GetComponent<UICenterOnChild>();
		mCenterOnChild.onFinished = DragFinished;
		CurrentPage = IapType.Nothing;
		UIEventListener uIEventListener = UIEventListener.Get(ItemPageSprite.transform.parent.gameObject);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener2 = UIEventListener.Get(MithrilPageSprite.transform.parent.gameObject);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener3 = UIEventListener.Get(GoldPageSprite.transform.parent.gameObject);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener4 = UIEventListener.Get(ExitButton);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener5 = UIEventListener.Get(RestoreButton);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener6 = UIEventListener.Get(MaterialChestOkButton);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener7 = UIEventListener.Get(RestoreButton);
		uIEventListener7.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uIEventListener7.onPress, new UIEventListener.BoolDelegate(OnButtonPress));
		UIEventListener uIEventListener8 = UIEventListener.Get(CloseBuyMaterial);
		uIEventListener8.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener8.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		UIEventListener uIEventListener9 = UIEventListener.Get(ComfirmBuyMaterial);
		uIEventListener9.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener9.onClick, new UIEventListener.VoidDelegate(OnClickButton));
		IsGotoUnlockIAP = false;
		int version = (int)AndroidConstant.version;
		if (version != 0 && version == 1)
		{
			RestoreButton.gameObject.SetActive(false);
		}
	}

	private void Start()
	{
		XLeft = PointLeft.transform.position.x;
		XLeftMiddle = PointLeftMiddle.transform.position.x;
		XMiddle = PointMiddle.transform.position.x;
		XRightMiddle = PointRightMiddle.transform.position.x;
		XRight = PointRight.transform.position.x;
		mLastCheckTime = Time.time;
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	public void OnClickButton(GameObject go)
	{
		if (CurrentPage != IapType.Package && go == ItemPageSprite.transform.parent.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			ChangePage(IapType.Package);
		}
		else if (CurrentPage != IapType.Mithril && go == MithrilPageSprite.transform.parent.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			ChangePage(IapType.Mithril);
		}
		else if (CurrentPage != IapType.Gold && go == GoldPageSprite.transform.parent.gameObject)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			ChangePage(IapType.Gold);
		}
		else if (go == ExitButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			base.gameObject.SetActive(false);
			if (SwVoidMenuScript.mInstance != null && SwVoidMenuScript.mInstance.gameObject.activeSelf)
			{
				UIMsgBox.instance.ShowMessage(UIMsgBox.instance, string.Empty, 9, 53);
				SwVoidMenuScript.TryShow();
			}
			if (ShopUIScript.GetInstance() != null && ShopUIScript.GetInstance().gameObject.activeSelf)
			{
				ShopUIScript.GetInstance().ItemList.RefreshAllGoods();
			}
		}
		else if (go == RestoreButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			Debug.Log("Start Restore Iap.");
			mInstance.WaitIAPResultStart(null);
			if (Global.ENABLE_IAP)
			{
				if (GameApp.GetInstance().IsConnectedToInternet())
				{
					IAP.NowRestoreProduct();
					return;
				}
				mInstance.WaitIAPResultEnd();
				UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_6"));
			}
		}
		else if (go == MaterialChestOkButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			MaterialChestObj.SetActive(false);
		}
		else if (go == CloseBuyMaterial)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			HideAllMaterialPage();
		}
		else if (go == ComfirmBuyMaterial)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			CorfirmBuyMaterial();
		}
	}

	public void OnButtonPress(GameObject go, bool isPress)
	{
		if (go == RestoreButton)
		{
			RestoreHighlight.SetActive(isPress);
		}
	}

	public void OpenPage(IapType page)
	{
		base.gameObject.SetActive(true);
		ChangePage(page);
		if (!GameApp.GetInstance().IsConnectedToInternet())
		{
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_MESSEGE_6"), 1, 91);
		}
		else
		{
			if (!Global.ENABLE_IAP || Application.platform != RuntimePlatform.IPhonePlayer)
			{
				return;
			}
			if (!MithrilRewardsScript.Instance.IsLocalizedPriceInitialized(IapType.Mithril) || !MithrilRewardsScript.Instance.IsLocalizedPriceInitialized(IapType.Package))
			{
				if (!MithrilRewardsScript.Instance.IsLocalizedPriceInitialized(IapType.Package))
				{
					IAP.GetProductInfo(4);
				}
				else if (!MithrilRewardsScript.Instance.IsLocalizedPriceInitialized(IapType.Mithril))
				{
					IAP.GetProductInfo(1);
				}
				WaitIAPInfoStart();
			}
			else
			{
				ShowLocalizedPrice();
			}
		}
	}

	public void ChangePage(IapType page)
	{
		if (page == CurrentPage)
		{
			return;
		}
		FreeMithrilGoods.gameObject.SetActive(false);
		FreeMithrilGoods.transform.localPosition = new Vector3(-1f * GoodsSpace, 0f, 0f);
		foreach (IapGoodsScript good in m_Goods)
		{
			if (good != FreeMithrilGoods)
			{
				UnityEngine.Object.DestroyImmediate(good.gameObject);
			}
		}
		foreach (UISprite bar in m_Bars)
		{
			UnityEngine.Object.DestroyImmediate(bar.gameObject);
		}
		m_Goods.Clear();
		m_Bars.Clear();
		mLastCenterGoods = null;
		switch (CurrentPage)
		{
		case IapType.Package:
			ItemPageSprite.spriteName = "type_n";
			break;
		case IapType.Mithril:
			MithrilPageSprite.spriteName = "type_n";
			break;
		case IapType.Gold:
			GoldPageSprite.spriteName = "type_n";
			break;
		}
		string sheetName = string.Empty;
		switch (page)
		{
		case IapType.Package:
			ItemPageSprite.spriteName = "type_u";
			sheetName = DataSheetName.IAP_PACKAGE;
			break;
		case IapType.Mithril:
			MithrilPageSprite.spriteName = "type_u";
			sheetName = DataSheetName.IAP_MITRHIL;
			break;
		case IapType.Gold:
			GoldPageSprite.spriteName = "type_u";
			sheetName = DataSheetName.IAP_GOLD;
			break;
		}
		CurrentPage = page;
		needUpdate = true;
		if (CurrentPage != IapType.Package && CurrentPage != IapType.Nothing)
		{
			GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.IAP_LIST, sheetName);
			if (CurrentPage == IapType.Mithril && GameApp.GetInstance().AppStatus == 1 && GameApp.GetInstance().isAdInitFinished)
			{
				FreeMithrilGoods.gameObject.SetActive(true);
				FreeMithrilGoods.mIndex = 0;
				m_Goods.Add(FreeMithrilGoods);
			}
			for (int i = 0; i < gameDataTable.Count - 1; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(GoodsTemplate) as GameObject;
				gameObject.transform.parent = GoodsList.transform;
				gameObject.transform.localPosition = new Vector3((float)i * GoodsSpace, 0f, 0f);
				gameObject.transform.localRotation = Quaternion.identity;
				gameObject.transform.localScale = Vector3.one;
				IapGoodsScript component = gameObject.GetComponent<IapGoodsScript>();
				float mPrice = gameDataTable.GetFloat(i, 1);
				bool isDollar = gameDataTable.GetString(i, 2) == "dollor";
				int num = gameDataTable.GetInt(i, 3);
				string productID = gameDataTable.GetString(i, 4);
				string price = ((CurrentPage != IapType.Gold && Global.ENABLE_IAP && Application.platform == RuntimePlatform.IPhonePlayer) ? MithrilRewardsScript.Instance.GetIAPLocalizedPrice(productID) : ((CurrentPage == IapType.Gold || Application.platform != RuntimePlatform.Android) ? mPrice.ToString() : ("$" + mPrice)));
				component.Init(i, isDollar, price, num, productID);
				if (CurrentPage == IapType.Mithril)
				{
					switch (num)
					{
					case 100:
						if (component.BonusObject != null)
						{
							component.BonusObject.SetActive(false);
						}
						break;
					case 240:
						if (component.BonusObject != null && component.BonusBackground != null && component.BonusLabel != null)
						{
							component.BonusBackground.spriteName = "redline";
							component.BonusLabel.text = LocalizationManager.GetInstance().GetString("LOC_IAP_DISCOUNT_1");
						}
						break;
					case 720:
						if (component.BonusObject != null && component.BonusBackground != null && component.BonusLabel != null)
						{
							component.BonusBackground.spriteName = "redline";
							component.BonusLabel.text = LocalizationManager.GetInstance().GetString("LOC_IAP_DISCOUNT_2");
						}
						break;
					case 1800:
						if (component.BonusObject != null && component.BonusBackground != null && component.BonusLabel != null)
						{
							component.BonusBackground.spriteName = "redline1";
							component.BonusLabel.text = "[fffc00]" + LocalizationManager.GetInstance().GetString("LOC_IAP_DISCOUNT_3") + "[-]";
						}
						break;
					case 6600:
						if (component.BonusObject != null && component.BonusBackground != null && component.BonusLabel != null)
						{
							component.BonusBackground.spriteName = "redline";
							component.BonusLabel.text = LocalizationManager.GetInstance().GetString("LOC_IAP_DISCOUNT_4");
						}
						break;
					case 15000:
						if (component.BonusObject != null && component.BonusBackground != null && component.BonusLabel != null)
						{
							component.BonusBackground.spriteName = "redline";
							component.BonusLabel.text = LocalizationManager.GetInstance().GetString("LOC_IAP_DISCOUNT_5");
						}
						break;
					case 36000:
						if (component.BonusObject != null && component.BonusBackground != null && component.BonusLabel != null)
						{
							component.BonusBackground.spriteName = "redline1";
							component.BonusLabel.text = "[fffc00]" + LocalizationManager.GetInstance().GetString("LOC_IAP_DISCOUNT_6") + "[-]";
						}
						break;
					}
				}
				else if (CurrentPage == IapType.Gold)
				{
					component.mPrice = mPrice;
					switch (num)
					{
					case 100000:
						if (component.BonusObject != null)
						{
							component.BonusObject.SetActive(false);
						}
						break;
					case 550000:
						if (component.BonusObject != null && component.BonusBackground != null && component.BonusLabel != null)
						{
							component.BonusBackground.spriteName = "redline";
							component.BonusLabel.text = LocalizationManager.GetInstance().GetString("LOC_GOLD_DISCOUNT_1");
						}
						break;
					case 1200000:
						if (component.BonusObject != null && component.BonusBackground != null && component.BonusLabel != null)
						{
							component.BonusBackground.spriteName = "redline";
							component.BonusLabel.text = LocalizationManager.GetInstance().GetString("LOC_GOLD_DISCOUNT_2");
						}
						break;
					case 7000000:
						if (component.BonusObject != null && component.BonusBackground != null && component.BonusLabel != null)
						{
							component.BonusBackground.spriteName = "redline1";
							component.BonusLabel.text = "[fffc00]" + LocalizationManager.GetInstance().GetString("LOC_GOLD_DISCOUNT_3") + "[-]";
						}
						break;
					case 16000000:
						if (component.BonusObject != null && component.BonusBackground != null && component.BonusLabel != null)
						{
							component.BonusBackground.spriteName = "redline";
							component.BonusLabel.text = LocalizationManager.GetInstance().GetString("LOC_GOLD_DISCOUNT_4");
						}
						break;
					case 27000000:
						if (component.BonusObject != null && component.BonusBackground != null && component.BonusLabel != null)
						{
							component.BonusBackground.spriteName = "redline";
							component.BonusLabel.text = LocalizationManager.GetInstance().GetString("LOC_GOLD_DISCOUNT_5");
						}
						break;
					case 100000000:
						if (component.BonusObject != null && component.BonusBackground != null && component.BonusLabel != null)
						{
							component.BonusBackground.spriteName = "redline1";
							component.BonusLabel.text = "[fffc00]" + LocalizationManager.GetInstance().GetString("LOC_GOLD_DISCOUNT_6") + "[-]";
						}
						break;
					}
				}
				m_Goods.Add(component);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(BarTemplate) as GameObject;
				gameObject2.transform.parent = GoodsBars.transform;
				gameObject2.transform.localPosition = Vector3.zero;
				gameObject2.transform.localRotation = Quaternion.identity;
				gameObject2.transform.localScale = Vector3.one;
				m_Bars.Add(gameObject2.GetComponent<UISprite>());
			}
		}
		else if (CurrentPage == IapType.Package)
		{
			if (IsGotoMaterialChest)
			{
				UserState userState = GameApp.GetInstance().GetUserState();
				IapPackageType isGotoAllMaterialList = IapPackageType.RookiePack;
				if (!SwVoidDataManager.IsAllOrNoWishInGroup(3))
				{
					isGotoAllMaterialList = IapPackageType.GoldChest;
				}
				else if (!SwVoidDataManager.IsAllOrNoWishInGroup(1))
				{
					isGotoAllMaterialList = IapPackageType.SilverChest;
				}
				else if (!SwVoidDataManager.IsAllOrNoWishInGroup(0) || !SwVoidDataManager.IsAllOrNoWishInGroup(2))
				{
					isGotoAllMaterialList = IapPackageType.CopperChest;
				}
				IsGotoAllMaterialList = isGotoAllMaterialList;
			}
			float num2 = 0f;
			GameDataTable gameDataTable2 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.IAP_LIST, sheetName);
			for (int j = 0; j < gameDataTable2.Count; j++)
			{
				float num3 = gameDataTable2.GetFloat(j, 0);
				bool isDollar2 = gameDataTable2.GetString(j, 1) == "dollor";
				IapPackageData iapPackageData = new IapPackageData();
				iapPackageData.Type = IapType.Package;
				iapPackageData.Index = j;
				iapPackageData.PackageType = (IapPackageType)(int)Enum.Parse(typeof(IapPackageType), gameDataTable2.GetString(j, 3), true);
				iapPackageData.HeadID = gameDataTable2.GetInt(j, 4);
				iapPackageData.ChestID = gameDataTable2.GetInt(j, 5);
				iapPackageData.HandID = gameDataTable2.GetInt(j, 6);
				iapPackageData.FootID = gameDataTable2.GetInt(j, 7);
				iapPackageData.BagID1 = gameDataTable2.GetInt(j, 8);
				iapPackageData.BagID2 = gameDataTable2.GetInt(j, 9);
				iapPackageData.GunID1 = gameDataTable2.GetInt(j, 10);
				iapPackageData.GunID2 = gameDataTable2.GetInt(j, 11);
				iapPackageData.GunID3 = gameDataTable2.GetInt(j, 12);
				iapPackageData.ITEMID1 = gameDataTable2.GetInt(j, 13);
				iapPackageData.ITEMID2 = gameDataTable2.GetInt(j, 14);
				iapPackageData.ITEMID3 = gameDataTable2.GetInt(j, 15);
				iapPackageData.COLOR = gameDataTable2.GetInt(j, 16);
				iapPackageData.ProductID = gameDataTable2.GetString(j, 17).ToLower();
				bool flag = true;
				if (AndroidConstant.version == AndroidConstant.Version.Kindle && iapPackageData.PackageType == IapPackageType.ColorPack && GameApp.GetInstance().IapColorPackStatus == 0)
				{
					flag = false;
					num2 = 0f - PacketSpace;
				}
				GameObject gameObject3 = UnityEngine.Object.Instantiate(PacketsTemplate) as GameObject;
				gameObject3.transform.parent = GoodsList.transform;
				gameObject3.transform.localPosition = new Vector3((float)j * PacketSpace + num2, 0f, 0f);
				gameObject3.transform.localRotation = Quaternion.identity;
				gameObject3.transform.localScale = Vector3.one;
				IapGoodsScript component2 = gameObject3.GetComponent<IapGoodsScript>();
				string price2 = ((Global.ENABLE_IAP && Application.platform == RuntimePlatform.IPhonePlayer) ? MithrilRewardsScript.Instance.GetIAPLocalizedPrice(iapPackageData.ProductID) : ((Application.platform != RuntimePlatform.Android) ? num3.ToString() : ("$" + num3)));
				component2.mPackageData = iapPackageData;
				component2.Init(j, isDollar2, price2, gameDataTable2.GetString(j, 2), iapPackageData.ProductID);
				m_Goods.Add(component2);
				GameObject gameObject4 = UnityEngine.Object.Instantiate(BarTemplate) as GameObject;
				gameObject4.transform.parent = GoodsBars.transform;
				gameObject4.transform.localPosition = Vector3.zero;
				gameObject4.transform.localRotation = Quaternion.identity;
				gameObject4.transform.localScale = Vector3.one;
				m_Bars.Add(gameObject4.GetComponent<UISprite>());
				if (!flag)
				{
					gameObject3.SetActive(false);
					gameObject4.SetActive(false);
				}
			}
		}
		if (m_Bars.Count > 0)
		{
			Vector3 localPosition = GoodsBars.transform.localPosition;
			float num4 = GoodsBars.cellWidth * (float)(m_Bars.Count - 1) + (float)m_Bars[m_Bars.Count - 1].width;
			GoodsBars.transform.localPosition = new Vector3((0f - num4) / 2f, localPosition.y, localPosition.z);
			GoodsBars.Reposition();
		}
		GoodsList.GetComponent<UIDraggablePanel>().ResetPosition();
		Vector3 vector = StartPosition;
		if (IsGotoUnlockIAP)
		{
			vector = new Vector3(-800f, 0f, 0f);
			IsGotoUnlockIAP = false;
		}
		if (IsGotoMaterialChest)
		{
			UserState userState2 = GameApp.GetInstance().GetUserState();
			IapPackageType iapPackageType = IapPackageType.RookiePack;
			if (!SwVoidDataManager.IsAllOrNoWishInGroup(3))
			{
				iapPackageType = IapPackageType.GoldChest;
			}
			else if (!SwVoidDataManager.IsAllOrNoWishInGroup(1))
			{
				iapPackageType = IapPackageType.SilverChest;
			}
			else if (!SwVoidDataManager.IsAllOrNoWishInGroup(0) || !SwVoidDataManager.IsAllOrNoWishInGroup(2))
			{
				iapPackageType = IapPackageType.CopperChest;
			}
			switch (iapPackageType)
			{
			case IapPackageType.CopperChest:
				vector = new Vector3(-1200f, 0f, 0f);
				break;
			case IapPackageType.SilverChest:
				vector = new Vector3(-1600f, 0f, 0f);
				break;
			case IapPackageType.GoldChest:
				vector = new Vector3(-2000f, 0f, 0f);
				break;
			default:
				vector = new Vector3(-2000f, 0f, 0f);
				break;
			}
			IsGotoMaterialChest = false;
		}
		if (IsGotoColorPack)
		{
			vector = new Vector3(-400f, 0f, 0f);
			IsGotoColorPack = false;
		}
		if (IsGotoEquipmentSuitPackage)
		{
			vector = new Vector3(0f, 0f, 0f);
			IsGotoEquipmentSuitPackage = false;
		}
		if (page == IapType.Mithril && GameApp.GetInstance().AppStatus == 1 && GameApp.GetInstance().isAdInitFinished)
		{
			vector /= 2f;
		}
		for (int k = 0; k < GoodsList.transform.childCount; k++)
		{
			GoodsList.transform.GetChild(k).localPosition += vector;
		}
		mCenterOnChild.Recenter();
	}

	public IapGoodsScript GetCenterGoods()
	{
		return mLastCenterGoods;
	}

	private void Update()
	{
		if (mIsWaitingIAPInfo)
		{
			if (Time.time - mWaitingIAPInfoStartTime >= mWaitingIAPInfoTimeOutLimit)
			{
				WaitIAPInfoEnd();
				UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_27"));
				base.gameObject.SetActive(false);
			}
			if (MithrilRewardsScript.Instance.IsLocalizedPriceInitialized(IapType.Mithril) && MithrilRewardsScript.Instance.IsLocalizedPriceInitialized(IapType.Package))
			{
				WaitIAPInfoEnd();
				ShowLocalizedPrice();
			}
		}
		if (mIsWaitingIAPResult)
		{
			if (Time.time - mWaitingIAPResultStartTime >= mWaitingIAPResultTimeOutLimit)
			{
				IAPFailed();
			}
			else
			{
				switch (IAP.purchaseStatus(null))
				{
				case 1:
				case 10:
					mInstance.WaitIAPResultEnd();
					break;
				default:
					mInstance.IAPFailed();
					break;
				case 0:
					break;
				}
			}
		}
		if (needUpdate)
		{
			ArrangeGoods();
		}
		if (!(Time.time - mLastCheckTime > mCheckCenterChildTime))
		{
			return;
		}
		mLastCheckTime = Time.time;
		if (!(mCenterOnChild.centeredObject != null) || !mCenterOnChild.centeredObject.activeSelf)
		{
			return;
		}
		IapGoodsScript component = mCenterOnChild.centeredObject.GetComponent<IapGoodsScript>();
		if (component != null && component != mLastCenterGoods)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_DRAG);
			if (mLastCenterGoods != null)
			{
				mLastCenterGoods.UnSelect();
				m_Bars[mLastCenterGoods.mIndex].spriteName = "move_n";
			}
			mLastCenterGoods = component;
			mLastCenterGoods.Select();
			m_Bars[mLastCenterGoods.mIndex].spriteName = "move_u";
		}
	}

	public void ArrangeGoods()
	{
		for (int i = 0; i < m_Goods.Count; i++)
		{
			float x = m_Goods[i].transform.position.x;
			float num = 0f;
			float num2 = 0f;
			float num3 = 1f;
			float num4 = 1f;
			float num5 = 1f;
			float num6 = 1f;
			bool flag = false;
			if (XLeft <= x && x <= XLeftMiddle)
			{
				num = XLeft;
				num2 = XLeftMiddle;
				num3 = 0.7f;
				num4 = 0.85f;
				num5 = 0.3f;
				num6 = 0.5f;
			}
			else if (XLeftMiddle < x && x <= XMiddle)
			{
				num = XLeftMiddle;
				num2 = XMiddle;
				num3 = 0.85f;
				num4 = 1f;
				num5 = 0.5f;
				num6 = 1f;
			}
			else if (XMiddle < x && x <= XRightMiddle)
			{
				num = XMiddle;
				num2 = XRightMiddle;
				num3 = 0.85f;
				num4 = 1f;
				flag = true;
				num5 = 0.5f;
				num6 = 1f;
			}
			else if (XRightMiddle < x && x <= XRight)
			{
				num = XRightMiddle;
				num2 = XRight;
				num3 = 0.7f;
				num4 = 0.85f;
				flag = true;
				num5 = 0.3f;
				num6 = 0.5f;
			}
			else
			{
				num = x;
				num2 = x + 1f;
				num3 = 0.7f;
				num4 = 0.7f;
				num5 = 0.3f;
				num6 = 0.3f;
			}
			float t = ((!flag) ? ((x - num) / (num2 - num)) : (1f - (x - num) / (num2 - num)));
			float num7 = Mathf.Lerp(num3, num4, t);
			float alpha = Mathf.Lerp(num5, num6, t);
			m_Goods[i].transform.localScale = Vector3.one * num7;
			m_Goods[i].SetAlpha(alpha);
		}
	}

	public void SetNeedUpdate()
	{
		needUpdate = true;
	}

	public void DragFinished()
	{
		needUpdate = false;
	}

	public void VoidServerCheckSuccess()
	{
		if (buyingIapGoods != null)
		{
			MithrilRewardsScript.mIapFrom = MithrilRewardsScript.IAPFrom.NORMAL;
			mIsWaitingIAPResult = true;
			mWaitingIAPResultStartTime = Time.time;
			if (Global.ENABLE_IAP)
			{
				if (GameApp.GetInstance().IsConnectedToInternet())
				{
					IAP.NowPurchaseProduct(buyingIapGoods.GetProductID(), "1");
					return;
				}
				WaitIAPResultEnd();
				UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_6"));
			}
			else
			{
				buyingIapGoods.StartCoroutine("ProvideContent");
			}
		}
		else
		{
			IAPFailed();
		}
	}

	public void WaitIAPInfoStart()
	{
		UIMsgBox.instance.ShowMessage(UIMsgBox.instance, LocalizationManager.GetInstance().GetString("LOC_IAP_PRICECHECK"), 7);
		mIsWaitingIAPInfo = true;
		mWaitingIAPInfoStartTime = Time.time;
	}

	public void WaitIAPInfoEnd()
	{
		UIMsgBox.instance.CloseMessage();
		mIsWaitingIAPInfo = false;
	}

	public void WaitIAPResultStart(IapGoodsScript buyingIap)
	{
		buyingIapGoods = buyingIap;
		UIMsgBox.instance.ShowMessage(UIMsgBox.instance, string.Empty, 9, 93);
		mIsWaitingIAPResult = true;
		mWaitingIAPResultStartTime = Time.time;
	}

	public void WaitPackageCheckStart(IapGoodsScript buyingIap)
	{
		buyingIapGoods = buyingIap;
		UIMsgBox.instance.ShowMessage(UIMsgBox.instance, string.Empty, 9);
	}

	public void WaitIAPResultEnd()
	{
		UIMsgBox.instance.CloseMessage();
		mIsWaitingIAPResult = false;
		buyingIapGoods = null;
	}

	public void IAPFailedInVoidServer(int errorCode)
	{
		WaitIAPResultEnd();
		UIMsgBox.instance.ShowErrorCodeMessage(errorCode);
	}

	public void IAPFailed()
	{
		WaitIAPResultEnd();
		UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_IAP_TIPS_4"));
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
	}

	public void IAPSuccess(IapData data)
	{
		if (!GameApp.GetInstance().GetUserState().GetFirstIapSuccess())
		{
			SwFirstIapSuccessRequest request = new SwFirstIapSuccessRequest();
			GameApp.GetInstance().GetWWWManager().SendRequest(request);
		}
		WaitIAPResultEnd();
		UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_IAP_TIPS_5"));
		if (CurrentPage == IapType.Package && data.Type == IapType.Package)
		{
			mInstance.m_Goods[data.Index].ShowOwned();
		}
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_BUY);
	}

	public void ShowRewardInfo(Dictionary<short, short> materialList)
	{
		MaterialChestObj.SetActive(true);
		MaterialPanel.transform.localPosition = new Vector3(-300f, 100f, 0f);
		if (materialChestInfo != null)
		{
			foreach (UIDailyRewardInfo item in materialChestInfo)
			{
				item.gameObject.SetActive(false);
				UnityEngine.Object.DestroyImmediate(item.gameObject);
			}
		}
		materialChestInfo = new List<UIDailyRewardInfo>();
		int count = materialList.Count;
		int num = 0;
		int num2 = 150;
		int num3 = 150;
		foreach (KeyValuePair<short, short> material in materialList)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(MaterialTemplate) as GameObject;
			gameObject.SetActive(true);
			gameObject.transform.parent = MaterialPanel.transform;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			UIDailyRewardInfo component = gameObject.GetComponent<UIDailyRewardInfo>();
			component.Init(3, material.Key, material.Value);
			materialChestInfo.Add(component);
		}
		MaterialPanel.GetComponent<UIGrid>().Reposition();
		MaterialPanel.GetComponent<UIPanel>().clipRange = new Vector4(300f, -100f, 750f, 350f);
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 91 && buttonId == UIMsg.UIMsgButton.Ok)
		{
			base.gameObject.SetActive(false);
			UIMsgBox.instance.CloseMessage();
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
		}
	}

	public void ShowLocalizedPrice()
	{
		foreach (IapGoodsScript good in m_Goods)
		{
			good.RefreshPrice();
		}
		if (MaterialIapChestPrice.gameObject.activeSelf && buyingIapGoods != null)
		{
			MaterialIapChestPrice.text = buyingIapGoods.priceString;
		}
	}

	public void CorfirmBuyMaterial()
	{
		AllMaterialListObj.SetActive(false);
		if (!GameApp.GetInstance().GetUserState().HasLoginOnline())
		{
			SwVoidOnlineProcess.NotRegistedMessage();
			return;
		}
		WaitPackageCheckStart(buyingIapGoods);
		IAP.sPurchaseStatus = 0;
		SwVoidDataManager.SendCheckServerRequest();
	}

	public void HideAllMaterialPage()
	{
		AllMaterialListObj.SetActive(false);
	}

	public void ShowAllMaterialPage(IapGoodsScript selectedIap)
	{
		buyingIapGoods = selectedIap;
		AllMaterialListObj.SetActive(true);
		materialInfoList.Clear();
		randomMaterialChestInfo.Clear();
		while (AllMaterialList.transform.childCount != 0)
		{
			UnityEngine.Object.DestroyImmediate(AllMaterialList.transform.GetChild(0).gameObject);
		}
		UserState userState = GameApp.GetInstance().GetUserState();
		Dictionary<short, short> swVoidMaterialList = userState.swVoidMaterialList;
		IapPackageType packageType = buyingIapGoods.mPackageData.PackageType;
		AddRandomMaterialIconToShow(0);
		AddMaterialToShow(1, userState.GetMaterialNumber(1));
		AddMaterialToShow(2, userState.GetMaterialNumber(2));
		AddMaterialToShow(3, userState.GetMaterialNumber(3));
		AddMaterialToShow(4, userState.GetMaterialNumber(4));
		AddMaterialToShow(5, userState.GetMaterialNumber(5));
		AddRandomMaterialIconToShow(2);
		AddMaterialToShow(201, userState.GetMaterialNumber(201));
		AddMaterialToShow(202, userState.GetMaterialNumber(202));
		AddMaterialToShow(203, userState.GetMaterialNumber(203));
		AddMaterialToShow(204, userState.GetMaterialNumber(204));
		AddMaterialToShow(205, userState.GetMaterialNumber(205));
		materialAmountObjectList[1].GetComponent<UILabel>().text = SwVoidDataManager.GetIapAmount(packageType, 0);
		if (packageType > IapPackageType.CopperChest)
		{
			AddRandomMaterialIconToShow(1);
			AddMaterialToShow(101, userState.GetMaterialNumber(101));
			AddMaterialToShow(102, userState.GetMaterialNumber(102));
			AddMaterialToShow(103, userState.GetMaterialNumber(103));
			AddMaterialToShow(104, userState.GetMaterialNumber(104));
			AddMaterialToShow(105, userState.GetMaterialNumber(105));
			allMaterialLineTwo.SetActive(true);
			materialAddObjectList[2].SetActive(true);
			materialAmountObjectList[2].GetComponent<UILabel>().text = SwVoidDataManager.GetIapAmount(packageType, 1);
			materialAmountObjectList[2].SetActive(true);
		}
		else
		{
			allMaterialLineTwo.SetActive(false);
			materialAddObjectList[2].SetActive(false);
			materialAmountObjectList[2].SetActive(false);
		}
		if (packageType > IapPackageType.SilverChest)
		{
			AddRandomMaterialIconToShow(3);
			AddMaterialToShow(301, userState.GetMaterialNumber(301));
			AddMaterialToShow(302, userState.GetMaterialNumber(302));
			AddMaterialToShow(303, userState.GetMaterialNumber(303));
			AddMaterialToShow(304, userState.GetMaterialNumber(304));
			AddMaterialToShow(305, userState.GetMaterialNumber(305));
			materialAddObjectList[3].SetActive(true);
			materialAmountObjectList[3].GetComponent<UILabel>().text = SwVoidDataManager.GetIapAmount(packageType, 3);
			materialAmountObjectList[3].SetActive(true);
		}
		else
		{
			materialAddObjectList[3].SetActive(false);
			materialAmountObjectList[3].SetActive(false);
		}
		AllMaterialList.Reposition();
		MaterialIapChestPrice.text = buyingIapGoods.priceString;
	}

	private void AddMaterialToShow(short id, short amount)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(AllMaterialItemTemplate) as GameObject;
		gameObject.transform.parent = AllMaterialList.transform;
		gameObject.transform.localPosition = new Vector3(0f, 0f, -2f);
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		gameObject.SetActive(true);
		SwVoidMaterialInfo component = gameObject.GetComponent<SwVoidMaterialInfo>();
		materialInfoList.Add(component);
		component.info.text = amount.ToString();
		component.Icon.spriteName = SwVoidDataManager.GetMaterialIconName(id);
		component.Rarity.spriteName = SwVoidDataManager.GetMaterialRarityName(id);
		component.materialId = id;
		component.CheckWish();
	}

	private void AddRandomMaterialIconToShow(short id)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(AllMaterialItemTemplate) as GameObject;
		gameObject.transform.parent = AllMaterialList.transform;
		gameObject.transform.localPosition = new Vector3(0f, 0f, -2f);
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		gameObject.SetActive(true);
		SwVoidMaterialInfo component = gameObject.GetComponent<SwVoidMaterialInfo>();
		randomMaterialChestInfo.Add(component);
		component.info.gameObject.SetActive(false);
		component.m_ClickType = SwVoidMaterialInfo.EClickType.IAP_RANDOM;
		component.materialId = id;
		component.SetRandomIapIcon();
		component.SetRandomIapIconList();
		component.CheckWish();
	}

	public void RecheckWishIcon(int rareId, bool isActive)
	{
		foreach (SwVoidMaterialInfo materialInfo in materialInfoList)
		{
			if (SwVoidDataManager.GetMaterialGroup((SwVoidDataManager.ESwVoidMaterialType)materialInfo.materialId) == rareId)
			{
				materialInfo.SetMask(isActive);
			}
		}
	}

	public void RecheckWishIcon(short materialId)
	{
		SwVoidMaterialInfo swVoidMaterialInfo = null;
		int materialGroup = SwVoidDataManager.GetMaterialGroup((SwVoidDataManager.ESwVoidMaterialType)materialId);
		bool flag = !SwVoidDataManager.IsAllOrNoWishInGroup(materialGroup);
		foreach (SwVoidMaterialInfo item in randomMaterialChestInfo)
		{
			if (materialGroup == item.materialId)
			{
				swVoidMaterialInfo = item;
			}
		}
		if (swVoidMaterialInfo != null)
		{
			swVoidMaterialInfo.SetMask(flag);
		}
		if (!flag)
		{
			RecheckWishIcon(materialGroup, true);
		}
	}
}
