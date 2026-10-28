using System;
using System.Collections.Generic;
using UnityEngine;

public class SwVoidMenuScript : MonoBehaviour
{
	public static SwVoidMenuScript mInstance;

	public GameObject exitButton;

	public GameObject upgradeButton;

	public GameObject equipButton;

	public GameObject resolveButton;

	public GameObject passiveSkillButtonOne;

	public GameObject passiveSkillButtonTwo;

	public GameObject addMaterialTestButton;

	public GameObject tabMake;

	public GameObject tabVoid;

	public GameObject upgradeSlotComfirm;

	public GameObject upgradeSlotNotEnoughMithril;

	public GameObject upgradeSlotCancle;

	public GameObject upgradeSlotObject;

	public GameObject passiveSkillDescription;

	public GameObject materialDescription;

	public UILabel materialName;

	public UISprite materialIcon;

	public UILabel materialDes;

	public SwVoidMaterialInfo lastSwVoidMaterialInfo;

	public float swVoidSpace;

	public GameObject swVoidIconTemplate;

	public GameObject swVoidIconList;

	public UISprite passiveSkillButtonSelectedOne;

	public UISprite passiveSkillButtonSelectedTwo;

	public UILabel materialGroupText;

	public UILabel voidCount;

	public GameObject voidCountBack;

	public UILabel upgradeSlotCost;

	[HideInInspector]
	public List<SwVoidIconScript> m_SwVoidIcons = new List<SwVoidIconScript>();

	private bool needUpdate = true;

	private UserState mUserState;

	private int templateCount;

	protected UICenterOnChild mCenterOnChild;

	private float mCheckCenterChildTime = 0.015f;

	private float mLastCheckTime;

	public GameObject PointLeft;

	public GameObject PointLeftMiddle;

	public GameObject PointMiddle;

	public GameObject PointRightMiddle;

	public GameObject PointRight;

	public GameObject OnlineWait;

	private SwVoidIconScript mLastCenterIcon;

	protected float XLeft;

	protected float XLeftMiddle;

	protected float XMiddle;

	protected float XRightMiddle;

	protected float XRight;

	private int mSelectedIndex;

	private int mIconEquipIndex;

	public GameObject MaterialChestOkButton;

	public GameObject MaterialChestObj;

	public GameObject MaterialTemplate;

	public GameObject MaterialPanel;

	[HideInInspector]
	public List<UIDailyRewardInfo> materialChestInfo = new List<UIDailyRewardInfo>();

	public SwVoidMaterialInfo[] materialNeedObjs;

	public UILabel m_SwVoidName;

	public UILabel m_Description;

	public UILabel m_AttributeOne;

	public UILabel m_AttributeTwo;

	public UILabel m_NextAttributeOne;

	public UILabel m_NextAttributeTwo;

	public GameObject ShowAllMaterialButton;

	public UISprite ShowAllBackground;

	public UILabel AllMaterialCount;

	public UISprite AllMaterialNewTag;

	public GameObject AllMaterialListObj;

	public GameObject AllMaterialCloseButton;

	public UIGrid AllMaterialList;

	public GameObject AllMaterialItemTemplate;

	private bool mIsAllMaterialVisible;

	public UILabel materialNameInAllMaterial;

	public UILabel materialDesInAllMaterial;

	public bool isWaitForEnterSwVoidStore { get; set; }

	public ESwVoidMenuTabState mMenuState { get; set; }

	public SwVoidMaterialInfo lastSelectedInAllMaterial { get; set; }

	private void Awake()
	{
		mInstance = this;
		templateCount = 5;
		mUserState = GameApp.GetInstance().GetUserState();
		mSelectedIndex = 0;
		mIconEquipIndex = -1;
		UIEventListener uIEventListener = UIEventListener.Get(exitButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(upgradeButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(equipButton);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener4 = UIEventListener.Get(resolveButton);
		uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener5 = UIEventListener.Get(passiveSkillButtonOne);
		uIEventListener5.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener5.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener6 = UIEventListener.Get(passiveSkillButtonTwo);
		uIEventListener6.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener6.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener7 = UIEventListener.Get(addMaterialTestButton);
		uIEventListener7.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener7.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener8 = UIEventListener.Get(tabMake);
		uIEventListener8.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener8.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener9 = UIEventListener.Get(tabVoid);
		uIEventListener9.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener9.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener10 = UIEventListener.Get(voidCountBack);
		uIEventListener10.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener10.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener11 = UIEventListener.Get(upgradeSlotComfirm);
		uIEventListener11.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener11.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener12 = UIEventListener.Get(upgradeSlotCancle);
		uIEventListener12.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener12.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener13 = UIEventListener.Get(MaterialChestOkButton);
		uIEventListener13.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener13.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener14 = UIEventListener.Get(ShowAllMaterialButton);
		uIEventListener14.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener14.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		UIEventListener uIEventListener15 = UIEventListener.Get(AllMaterialCloseButton);
		uIEventListener15.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener15.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
		Init(true, ESwVoidMenuTabState.MAKE_TAB);
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

	private void Update()
	{
		if (OnlineWait.activeSelf)
		{
			return;
		}
		if (needUpdate)
		{
			ArrangeIcons();
			ClearPassiveSkillDescription();
			ClearSwVoidMaterialInfo();
		}
		if (!(Time.time - mLastCheckTime > mCheckCenterChildTime))
		{
			return;
		}
		mLastCheckTime = Time.time;
		if (!(mCenterOnChild.centeredObject != null))
		{
			return;
		}
		SwVoidIconScript component = mCenterOnChild.centeredObject.GetComponent<SwVoidIconScript>();
		if (component != null && component != mLastCenterIcon)
		{
			if (mLastCenterIcon != null)
			{
				mLastCenterIcon.UnSelect();
			}
			mLastCenterIcon = component;
			mLastCenterIcon.Select();
			mSelectedIndex = component.mIndex;
		}
	}

	public static void TryShow()
	{
		GetSwVoidDataFromServer(false);
	}

	public void Show()
	{
		mInstance.gameObject.SetActive(true);
	}

	public void Init(bool isFirstTime, ESwVoidMenuTabState tabState)
	{
		mMenuState = tabState;
		mCenterOnChild = swVoidIconList.GetComponent<UICenterOnChild>();
		mCenterOnChild.onFinished = DragFinished;
		int num = 0;
		switch (mMenuState)
		{
		case ESwVoidMenuTabState.MAKE_TAB:
			tabMake.GetComponent<UISprite>().spriteName = "type_u";
			tabVoid.GetComponent<UISprite>().spriteName = "type_n";
			templateCount = Global.VOID_TYPE_NUM;
			break;
		case ESwVoidMenuTabState.VOID_TAB:
			tabVoid.GetComponent<UISprite>().spriteName = "type_u";
			tabMake.GetComponent<UISprite>().spriteName = "type_n";
			num = (templateCount = mUserState.swVoidList.Count);
			break;
		}
		int[] array = new int[templateCount];
		int num2 = 0;
		if (mMenuState == ESwVoidMenuTabState.VOID_TAB)
		{
			foreach (int key in mUserState.swVoidList.Keys)
			{
				int num3 = (array[num2] = key);
				if (mUserState.SwVoidIdInUsing != 0 && num3 == mUserState.SwVoidIdInUsing)
				{
					if (isFirstTime)
					{
						mSelectedIndex = num2;
					}
					mIconEquipIndex = num2;
				}
				num2++;
			}
		}
		for (int i = 0; i < templateCount; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(swVoidIconTemplate) as GameObject;
			gameObject.SetActive(true);
			gameObject.transform.parent = swVoidIconList.transform;
			gameObject.transform.localPosition = new Vector3((float)(i - mSelectedIndex) * swVoidSpace, 0f, 0f);
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			SwVoidIconScript component = gameObject.GetComponent<SwVoidIconScript>();
			m_SwVoidIcons.Add(component);
			if (i < num)
			{
				component.Init(i, mUserState.swVoidList[array[i]], array[i]);
			}
			else
			{
				component.Init(i, (ESwVoidType)(i - num));
			}
		}
		mCenterOnChild.Recenter();
		if (tabState == ESwVoidMenuTabState.MAKE_TAB)
		{
			voidCount.text = string.Empty;
			voidCountBack.gameObject.SetActive(false);
		}
		else
		{
			voidCount.text = templateCount.ToString() + "/" + mUserState.swvoidCountLimit;
			voidCountBack.gameObject.SetActive(true);
		}
		if (templateCount == 0)
		{
			upgradeButton.SetActive(false);
			equipButton.SetActive(false);
			resolveButton.SetActive(false);
			passiveSkillButtonOne.SetActive(false);
			passiveSkillButtonTwo.SetActive(false);
			SwVoidMaterialInfo[] array2 = materialNeedObjs;
			foreach (SwVoidMaterialInfo swVoidMaterialInfo in array2)
			{
				swVoidMaterialInfo.gameObject.SetActive(false);
			}
			m_Description.text = string.Empty;
			m_AttributeOne.text = string.Empty;
			m_AttributeTwo.text = string.Empty;
			m_SwVoidName.text = string.Empty;
			m_NextAttributeOne.text = string.Empty;
			m_NextAttributeTwo.text = string.Empty;
		}
		AllMaterialCount.text = SwVoidDataManager.CalcAllMaterialCount().ToString();
		AllMaterialNewTag.gameObject.SetActive(SwVoidDataManager.HasNewMaterial());
	}

	public void OnButtonClick(GameObject go)
	{
		if (go == exitButton)
		{
			ClearPassiveSkillDescription();
			base.gameObject.SetActive(false);
			if (InterMissionScript.GetInstance() != null)
			{
				InterMissionScript.GetInstance().spawnMySwVoidInterMission();
			}
			if (CustomizeUIScript.mInstance != null)
			{
				CustomizeUIScript.mInstance.RefreshSwVoidIcon();
			}
		}
		else if (go == upgradeButton)
		{
			ClearPassiveSkillDescription();
			mLastCenterIcon.ClickUpgradeButton();
		}
		else if (go == equipButton)
		{
			ClearPassiveSkillDescription();
			if (mUserState.SwVoidIdInUsing == mLastCenterIcon.swVoidIdOfPlayer && mUserState.SwVoidIdInUsing != 0)
			{
				mUserState.ChangeEquipedSwVoid(0);
				mLastCenterIcon.Equip(false);
			}
			else
			{
				mUserState.ChangeEquipedSwVoid(mLastCenterIcon.swVoidIdOfPlayer);
				if (mIconEquipIndex != -1 && mUserState.SwVoidIdInUsing != 0)
				{
					m_SwVoidIcons[mIconEquipIndex].Equip(false);
				}
				mLastCenterIcon.Equip();
				mIconEquipIndex = mSelectedIndex;
			}
		}
		else if (go == resolveButton)
		{
			ClearPassiveSkillDescription();
			mLastCenterIcon.TryResolve();
		}
		else if (go == passiveSkillButtonOne)
		{
			bool activeSelf = passiveSkillDescription.activeSelf;
			if (!activeSelf)
			{
				mLastCenterIcon.ShowPassiveDescription(0);
				passiveSkillButtonSelectedOne.gameObject.SetActive(true);
			}
			else if (passiveSkillButtonSelectedTwo.gameObject.activeSelf)
			{
				mLastCenterIcon.ShowPassiveDescription(0);
				passiveSkillButtonSelectedOne.gameObject.SetActive(true);
				passiveSkillButtonSelectedTwo.gameObject.SetActive(false);
			}
			else
			{
				passiveSkillDescription.SetActive(!activeSelf);
				passiveSkillButtonSelectedOne.gameObject.SetActive(false);
			}
		}
		else if (go == passiveSkillButtonTwo)
		{
			bool activeSelf2 = passiveSkillDescription.activeSelf;
			if (!activeSelf2)
			{
				mLastCenterIcon.ShowPassiveDescription(1);
				passiveSkillButtonSelectedTwo.gameObject.SetActive(true);
			}
			else if (passiveSkillButtonSelectedOne.gameObject.activeSelf)
			{
				mLastCenterIcon.ShowPassiveDescription(1);
				passiveSkillButtonSelectedTwo.gameObject.SetActive(true);
				passiveSkillButtonSelectedOne.gameObject.SetActive(false);
			}
			else
			{
				passiveSkillDescription.SetActive(!activeSelf2);
				passiveSkillButtonSelectedTwo.gameObject.SetActive(false);
			}
		}
		else if (go == addMaterialTestButton)
		{
			if (IapScript.mInstance != null)
			{
				IapScript.mInstance.OpenPage(IapScript.IapType.Nothing);
				IapScript.mInstance.IsGotoMaterialChest = true;
				IapScript.mInstance.OpenPage(IapScript.IapType.Package);
			}
			ClearPassiveSkillDescription();
		}
		else if (go == tabVoid)
		{
			if (mIconEquipIndex == -1)
			{
				SetFocusIndex(0);
			}
			else
			{
				SetFocusIndex(mIconEquipIndex);
			}
			Refresh(ESwVoidMenuTabState.VOID_TAB);
			ClearPassiveSkillDescription();
		}
		else if (go == tabMake)
		{
			SetFocusIndex(0);
			Refresh(ESwVoidMenuTabState.MAKE_TAB);
			ClearPassiveSkillDescription();
		}
		else if (go == voidCountBack)
		{
			int num = SwVoidDataManager.CostOfBuyNewSlot(mUserState.swvoidCountLimit + 1);
			if (num == 0)
			{
				upgradeSlotObject.SetActive(false);
			}
			else
			{
				upgradeSlotObject.SetActive(true);
				upgradeSlotCost.text = num.ToString();
				if (mUserState.GetMithril() >= num)
				{
					upgradeSlotNotEnoughMithril.SetActive(false);
					upgradeSlotComfirm.SetActive(true);
				}
				else
				{
					upgradeSlotComfirm.SetActive(false);
					upgradeSlotNotEnoughMithril.SetActive(true);
				}
			}
			ClearPassiveSkillDescription();
		}
		else if (go == upgradeSlotComfirm)
		{
			SwVoidDataManager.SendAddSlotRequest();
			ClearPassiveSkillDescription();
		}
		else if (go == upgradeSlotCancle)
		{
			upgradeSlotObject.SetActive(false);
			ClearPassiveSkillDescription();
		}
		else if (go == MaterialChestOkButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			MaterialChestObj.SetActive(false);
			ClearPassiveSkillDescription();
		}
		else if (go == ShowAllMaterialButton || go == AllMaterialCloseButton)
		{
			mIsAllMaterialVisible = !mIsAllMaterialVisible;
			if (mIsAllMaterialVisible)
			{
				ShowAllBackground.spriteName = "btn_color_u";
				ShowAllMaterialPage();
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			}
			else
			{
				ShowAllBackground.spriteName = "itemother";
				AllMaterialListObj.SetActive(false);
				AllMaterialNewTag.gameObject.SetActive(SwVoidDataManager.HasNewMaterial());
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
			}
		}
		ClearSwVoidMaterialInfo();
	}

	public void Refresh(ESwVoidMenuTabState tabState)
	{
		foreach (SwVoidIconScript swVoidIcon in m_SwVoidIcons)
		{
			UnityEngine.Object.DestroyImmediate(swVoidIcon.gameObject);
		}
		m_SwVoidIcons.Clear();
		mLastCenterIcon = null;
		swVoidIconList.transform.localPosition = new Vector3(0f, 40f, 0f);
		Vector4 vector = default(Vector4);
		vector = swVoidIconList.GetComponent<UIPanel>().clipRange;
		vector.x = 0f;
		swVoidIconList.GetComponent<UIPanel>().clipRange = vector;
		Init(false, tabState);
		ArrangeIcons();
	}

	public void SetNeedUpdate()
	{
		needUpdate = true;
	}

	public void DragFinished()
	{
		needUpdate = false;
	}

	public void SetFocusIndex(int newIndex)
	{
		mSelectedIndex = newIndex;
	}

	public void ArrangeIcons()
	{
		for (int i = 0; i < m_SwVoidIcons.Count; i++)
		{
			float x = m_SwVoidIcons[i].transform.position.x;
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
			m_SwVoidIcons[i].transform.localScale = Vector3.one * num7;
			m_SwVoidIcons[i].SetAlpha(alpha);
		}
	}

	public void ClearPassiveSkillDescription()
	{
		passiveSkillButtonSelectedTwo.gameObject.SetActive(false);
		passiveSkillButtonSelectedOne.gameObject.SetActive(false);
		passiveSkillDescription.SetActive(false);
	}

	public static void GetSwVoidDataFromServer(bool skipAddFirst)
	{
		if (GameApp.GetInstance().IsConnectedToInternet())
		{
			UserState userState = GameApp.GetInstance().GetUserState();
			bool flag = false;
			if (!userState.GetHasGetFirstBossVoidMaterials() && userState.GetNormalMissionState().GetUnlockedStageId() >= 1)
			{
				flag = true;
			}
			if (skipAddFirst)
			{
				flag = false;
			}
			if (userState.HasLoginOnline())
			{
				if (GameApp.GetInstance().IsConnectedToServer())
				{
					if (flag)
					{
						SwVoidFirstBossStaticMaterialsRequest request = new SwVoidFirstBossStaticMaterialsRequest();
						GameApp.GetInstance().GetNetworkManager().SendRequest(request);
					}
					else
					{
						SwVoidGetDataRequest request2 = new SwVoidGetDataRequest();
						GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
					}
				}
				else if (flag)
				{
					SwVoidGetFirstBossStaticMaterialsWWWRequest request3 = new SwVoidGetFirstBossStaticMaterialsWWWRequest();
					GameApp.GetInstance().GetWWWManager().SendRequest(request3);
				}
				else
				{
					SwVoidGetDataWWWRequest request4 = new SwVoidGetDataWWWRequest();
					GameApp.GetInstance().GetWWWManager().SendRequest(request4);
				}
				TutorialManager.GetInstance().Trigger(TutorialTriggerType.VOID_COMPOSE);
			}
			else
			{
				SwVoidOnlineProcess.NotRegistedMessage();
			}
		}
		else
		{
			UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_6"));
		}
	}

	public void WaitOnlineInfo()
	{
		OnlineWait.SetActive(true);
	}

	public void ReceiveOnlineInfo()
	{
		OnlineWait.SetActive(false);
	}

	public void ReceiveUpgradeInfo(short[] upgradeValue)
	{
		mLastCenterIcon.ShowUpgradeInfo(upgradeValue);
	}

	public void ReceiveConfirmUpgrade(SwVoidResponseData newSwVoidInfo)
	{
		mLastCenterIcon.ReceiveConfirmUpgrade();
	}

	public void ReceiveResolveInfo(int swVoidId)
	{
		mLastCenterIcon.receiveResolveInfo();
		mIconEquipIndex = -1;
	}

	public SwVoidIconScript GetLastCenterIcon()
	{
		return mLastCenterIcon;
	}

	public void ResetSwVoidSaveDateInEachIcon()
	{
		foreach (SwVoidIconScript swVoidIcon in m_SwVoidIcons)
		{
			swVoidIcon.ResetSwVoidSaveData();
			swVoidIcon.CheckEquipState();
		}
		AllMaterialCount.text = SwVoidDataManager.CalcAllMaterialCount().ToString();
		AllMaterialNewTag.gameObject.SetActive(SwVoidDataManager.HasNewMaterial());
	}

	public void ShowSwVoidMaterialInfo(short materialId, SwVoidMaterialInfo info)
	{
		ClearPassiveSkillDescription();
		if (lastSwVoidMaterialInfo != null)
		{
			lastSwVoidMaterialInfo.isShowInfo = false;
			lastSwVoidMaterialInfo.selected.SetActive(false);
		}
		lastSwVoidMaterialInfo = info;
		materialDescription.SetActive(true);
		Debug.Log(materialId);
		materialName.text = SwVoidDataManager.GetMaterialName(materialId);
		materialIcon.spriteName = SwVoidDataManager.GetMaterialIconName(materialId);
		materialDes.text = SwVoidDataManager.GetMaterialDes(materialId);
	}

	public void ShowSwVoidMaterialInfoInAllMaterial(short materialId, SwVoidMaterialInfo info)
	{
		ClearSwVoidMaterialInfoInAllMaterial();
		if (lastSelectedInAllMaterial != null)
		{
			lastSelectedInAllMaterial.isShowInfo = false;
			lastSelectedInAllMaterial.selected.SetActive(false);
		}
		lastSelectedInAllMaterial = info;
		materialNameInAllMaterial.text = SwVoidDataManager.GetMaterialName(materialId);
		materialDesInAllMaterial.text = SwVoidDataManager.GetMaterialDes(materialId);
	}

	public void ClearSwVoidMaterialInfo()
	{
		if (lastSwVoidMaterialInfo != null)
		{
			lastSwVoidMaterialInfo.isShowInfo = false;
			lastSwVoidMaterialInfo.selected.SetActive(false);
		}
		materialDescription.SetActive(false);
	}

	public void ClearSwVoidMaterialInfoInAllMaterial()
	{
		if (lastSelectedInAllMaterial != null)
		{
			lastSelectedInAllMaterial.isShowInfo = false;
			lastSelectedInAllMaterial.selected.SetActive(false);
		}
	}

	public void AutoEquipNewVoid()
	{
		if (mSelectedIndex < m_SwVoidIcons.Count)
		{
			SwVoidIconScript swVoidIconScript = m_SwVoidIcons[mSelectedIndex];
			mUserState.ChangeEquipedSwVoid(swVoidIconScript.swVoidIdOfPlayer);
			if (mIconEquipIndex != -1 && mUserState.SwVoidIdInUsing != 0)
			{
				m_SwVoidIcons[mIconEquipIndex].Equip(false);
			}
			swVoidIconScript.Equip();
			mIconEquipIndex = mSelectedIndex;
		}
	}

	public void RefreshCountLimit()
	{
		if (mMenuState == ESwVoidMenuTabState.MAKE_TAB)
		{
			voidCount.text = string.Empty;
			voidCountBack.gameObject.SetActive(false);
		}
		else
		{
			voidCount.text = templateCount.ToString() + "/" + mUserState.swvoidCountLimit;
			voidCountBack.gameObject.SetActive(true);
		}
		upgradeSlotObject.SetActive(false);
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
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			UIDailyRewardInfo component = gameObject.GetComponent<UIDailyRewardInfo>();
			component.Init(3, material.Key, material.Value);
			materialChestInfo.Add(component);
		}
		MaterialPanel.GetComponent<UIGrid>().Reposition();
		MaterialPanel.GetComponent<UIPanel>().clipRange = new Vector4(300f, -100f, 750f, 350f);
	}

	private void ShowAllMaterialPage()
	{
		AllMaterialListObj.SetActive(true);
		materialNameInAllMaterial.text = string.Empty;
		materialDesInAllMaterial.text = string.Empty;
		while (AllMaterialList.transform.childCount != 0)
		{
			UnityEngine.Object.DestroyImmediate(AllMaterialList.transform.GetChild(0).gameObject);
		}
		UserState userState = GameApp.GetInstance().GetUserState();
		Dictionary<short, short> swVoidMaterialList = userState.swVoidMaterialList;
		AddMaterialToShow(1, userState.GetMaterialNumber(1), userState.GetMaterialIsNew(1));
		AddMaterialToShow(201, userState.GetMaterialNumber(201), userState.GetMaterialIsNew(201));
		AddMaterialToShow(101, userState.GetMaterialNumber(101), userState.GetMaterialIsNew(101));
		AddMaterialToShow(301, userState.GetMaterialNumber(301), userState.GetMaterialIsNew(301));
		AddMaterialToShow(2, userState.GetMaterialNumber(2), userState.GetMaterialIsNew(2));
		AddMaterialToShow(202, userState.GetMaterialNumber(202), userState.GetMaterialIsNew(202));
		AddMaterialToShow(102, userState.GetMaterialNumber(102), userState.GetMaterialIsNew(102));
		AddMaterialToShow(302, userState.GetMaterialNumber(302), userState.GetMaterialIsNew(302));
		AddMaterialToShow(3, userState.GetMaterialNumber(3), userState.GetMaterialIsNew(3));
		AddMaterialToShow(203, userState.GetMaterialNumber(203), userState.GetMaterialIsNew(203));
		AddMaterialToShow(103, userState.GetMaterialNumber(103), userState.GetMaterialIsNew(103));
		AddMaterialToShow(303, userState.GetMaterialNumber(303), userState.GetMaterialIsNew(303));
		AddMaterialToShow(4, userState.GetMaterialNumber(4), userState.GetMaterialIsNew(4));
		AddMaterialToShow(204, userState.GetMaterialNumber(204), userState.GetMaterialIsNew(204));
		AddMaterialToShow(104, userState.GetMaterialNumber(104), userState.GetMaterialIsNew(104));
		AddMaterialToShow(304, userState.GetMaterialNumber(304), userState.GetMaterialIsNew(304));
		AddMaterialToShow(5, userState.GetMaterialNumber(5), userState.GetMaterialIsNew(5));
		AddMaterialToShow(205, userState.GetMaterialNumber(205), userState.GetMaterialIsNew(205));
		AddMaterialToShow(105, userState.GetMaterialNumber(105), userState.GetMaterialIsNew(105));
		AddMaterialToShow(305, userState.GetMaterialNumber(305), userState.GetMaterialIsNew(305));
		AllMaterialList.Reposition();
	}

	private void AddMaterialToShow(short id, short amount, bool isNew)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(AllMaterialItemTemplate) as GameObject;
		gameObject.transform.parent = AllMaterialList.transform;
		gameObject.transform.localPosition = new Vector3(0f, 0f, -2f);
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		gameObject.SetActive(true);
		SwVoidMaterialInfo component = gameObject.GetComponent<SwVoidMaterialInfo>();
		component.info.text = amount.ToString();
		component.Icon.spriteName = SwVoidDataManager.GetMaterialIconName(id);
		component.Rarity.spriteName = SwVoidDataManager.GetMaterialRarityName(id);
		component.materialId = id;
		if (isNew)
		{
			if (component.newTag != null)
			{
				component.newTag.gameObject.SetActive(true);
			}
			GameApp.GetInstance().GetUserState().SetMaterialIsNew(id, false);
		}
	}
}
