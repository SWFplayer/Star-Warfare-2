using System.Collections.Generic;
using UnityEngine;

public class SwVoidIconScript : MonoBehaviour, UIMsgListener
{
	[HideInInspector]
	public int mIndex;

	public UISprite swVoidSelectBackground;

	public UISprite swVoidIcon;

	public SwVoidMaterialInfo[] materialNeedObjs = new SwVoidMaterialInfo[4];

	public UILabel m_SwVoidName;

	public UILabel m_Description;

	public UILabel m_AttributeOne;

	public UILabel m_AttributeTwo;

	public UILabel m_NextAttributeOne;

	public UILabel m_NextAttributeTwo;

	public UILabel m_NextLevelLable;

	public UISprite m_PassiveSkillOne;

	public UISprite m_PassiveSkillTwo;

	public UISprite m_PassiveSkillOneIcon;

	public UISprite m_PassiveSkillTwoIcon;

	public UISprite m_EquipedIcon;

	public GameObject passiveSkillDescription;

	public UILabel m_PassiveSkillDescriptionLable;

	public GameObject equipButton;

	public GameObject resolveButton;

	public GameObject upgradeButton;

	public UILabel m_UpgradeButton;

	public UILabel m_EquipButton;

	private bool isEmpty;

	private List<UIWidget> mWidgets = new List<UIWidget>();

	private SwVoidSavedata m_SwVoidSavedate;

	private UserState mUserState;

	private Color enoughColor = new Color(0f, 1f, 0f);

	private Color notEnoughColor = new Color(1f, 0f, 0f);

	private byte level;

	private ESwVoidType type;

	private float m_BaseAlpha;

	private bool isMaterialEnough;

	private short[] upgradeValue;

	public int swVoidIdOfPlayer;

	private void Awake()
	{
		mWidgets.Clear();
		UIWidget[] componentsInChildren = GetComponentsInChildren<UIWidget>();
		foreach (UIWidget item in componentsInChildren)
		{
			mWidgets.Add(item);
		}
		mUserState = GameApp.GetInstance().GetUserState();
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Init(int index, ESwVoidType newSwVoidType)
	{
		type = newSwVoidType;
		level = 0;
		Init(index, null, 0);
	}

	public void Init(int index, SwVoidSavedata swvoidSavedata, int swVoidId)
	{
		m_BaseAlpha = 1f;
		mIndex = index;
		swVoidIdOfPlayer = swVoidId;
		if (swvoidSavedata == null)
		{
			isEmpty = true;
			swVoidIcon.spriteName = SwVoidDataManager.GetIconNameOfSwVoid(type, (byte)(level + 1));
			m_BaseAlpha = 1f;
			return;
		}
		isEmpty = false;
		m_SwVoidSavedate = swvoidSavedata;
		level = m_SwVoidSavedate.GetLevel();
		type = m_SwVoidSavedate.GetType();
		swVoidIcon.spriteName = SwVoidDataManager.GetIconNameOfSwVoid(type, level);
		if (mUserState.SwVoidIdInUsing != 0 && mUserState.SwVoidIdInUsing == swVoidIdOfPlayer)
		{
			m_EquipedIcon.gameObject.SetActive(true);
			if (!mWidgets.Contains(m_EquipedIcon))
			{
				mWidgets.Add(m_EquipedIcon);
			}
		}
		else
		{
			m_EquipedIcon.gameObject.SetActive(false);
		}
	}

	public void Refresh()
	{
		if (SwVoidDataManager.GetAvailable(type, (byte)(level + 1)))
		{
			upgradeButton.SetActive(true);
			isMaterialEnough = true;
			short[] materialNeedNum = SwVoidDataManager.GetMaterialNeedNum((byte)type, (byte)(level + 1));
			short[] materialNeedId = SwVoidDataManager.GetMaterialNeedId((byte)type, (byte)(level + 1));
			mUserState.swVoidMaterialWishList.Clear();
			for (int i = 0; i < 4; i++)
			{
				if (materialNeedNum[i] > 0)
				{
					materialNeedObjs[i].gameObject.SetActive(true);
					string empty = string.Empty;
					short materialNumber = mUserState.GetMaterialNumber(materialNeedId[i]);
					empty += materialNumber;
					empty += "/";
					empty += materialNeedNum[i];
					materialNeedObjs[i].info.text = empty;
					bool flag = materialNumber >= materialNeedNum[i];
					materialNeedObjs[i].info.color = ((!flag) ? notEnoughColor : enoughColor);
					materialNeedObjs[i].Icon.spriteName = SwVoidDataManager.GetMaterialIconName(materialNeedId[i]);
					materialNeedObjs[i].Rarity.spriteName = SwVoidDataManager.GetMaterialRarityName(materialNeedId[i]);
					materialNeedObjs[i].materialId = materialNeedId[i];
					GameApp.GetInstance().GetUserState().SetMaterialIsNew(materialNeedId[i], false);
					if (!flag)
					{
						isMaterialEnough = false;
						mUserState.SetMaterialWish(materialNeedId[i], true);
					}
				}
				else
				{
					materialNeedObjs[i].gameObject.SetActive(false);
				}
			}
		}
		else
		{
			upgradeButton.SetActive(false);
			SwVoidMaterialInfo[] array = materialNeedObjs;
			foreach (SwVoidMaterialInfo swVoidMaterialInfo in array)
			{
				swVoidMaterialInfo.gameObject.SetActive(false);
			}
		}
		if (m_SwVoidSavedate != null)
		{
			m_UpgradeButton.text = LocalizationManager.GetInstance().GetString("LOC_BUTTON_UPGRADE");
			m_SwVoidName.text = m_SwVoidSavedate.GetName();
			m_Description.text = SwVoidDataManager.GetDescriptionOfSwVoid(type, level, GameApp.GetInstance().GetUserState());
			m_AttributeOne.text = m_SwVoidSavedate.GetAttributeDescription(0);
			m_AttributeTwo.text = m_SwVoidSavedate.GetAttributeDescription(1);
			if (!m_SwVoidSavedate.isMaxLevel())
			{
				m_NextLevelLable.gameObject.SetActive(true);
				m_NextAttributeOne.text = m_SwVoidSavedate.GetAttributeNextDescription(0);
				m_NextAttributeTwo.text = m_SwVoidSavedate.GetAttributeNextDescription(1);
			}
			else
			{
				m_NextLevelLable.gameObject.SetActive(false);
				m_NextAttributeOne.text = string.Empty;
				m_NextAttributeTwo.text = string.Empty;
			}
			swVoidIcon.spriteName = SwVoidDataManager.GetIconNameOfSwVoid(type, level);
			m_PassiveSkillOne.gameObject.SetActive(true);
			m_PassiveSkillTwo.gameObject.SetActive(true);
			if (m_SwVoidSavedate.isPassiveSkillShow(0))
			{
				m_PassiveSkillOneIcon.spriteName = SwVoidDataManager.GetIconNameOfPassiveSkill(m_SwVoidSavedate.GetPassiveSkillId(0));
				if (m_SwVoidSavedate.isPassiveSkillActive(0))
				{
					m_PassiveSkillOneIcon.alpha = 1f;
				}
				else
				{
					m_PassiveSkillOneIcon.alpha = 0.5f;
				}
			}
			else
			{
				m_PassiveSkillOneIcon.spriteName = "skill_any";
			}
			if (m_SwVoidSavedate.isPassiveSkillShow(1))
			{
				m_PassiveSkillTwoIcon.spriteName = SwVoidDataManager.GetIconNameOfPassiveSkill(m_SwVoidSavedate.GetPassiveSkillId(1));
				if (m_SwVoidSavedate.isPassiveSkillActive(1))
				{
					m_PassiveSkillTwoIcon.alpha = 1f;
				}
				else
				{
					m_PassiveSkillTwoIcon.alpha = 0.5f;
				}
			}
			else
			{
				m_PassiveSkillTwoIcon.spriteName = "skill_any";
			}
			equipButton.SetActive(true);
			resolveButton.SetActive(true);
		}
		else
		{
			m_UpgradeButton.text = LocalizationManager.GetInstance().GetString("LOC_BUTTON_CREATE");
			m_SwVoidName.text = SwVoidDataManager.GetFinalNameFromIndex(type, 1);
			m_Description.text = SwVoidDataManager.GetDescriptionOfSwVoid(type, 1, GameApp.GetInstance().GetUserState(), true);
			m_AttributeOne.text = "???";
			m_AttributeTwo.text = "???";
			m_NextLevelLable.gameObject.SetActive(false);
			m_NextAttributeOne.text = string.Empty;
			m_NextAttributeTwo.text = string.Empty;
			swVoidIcon.spriteName = SwVoidDataManager.GetIconNameOfSwVoid(type, (byte)(level + 1));
			m_PassiveSkillOne.gameObject.SetActive(false);
			m_PassiveSkillTwo.gameObject.SetActive(false);
			m_PassiveSkillOneIcon.spriteName = string.Empty;
			m_PassiveSkillTwoIcon.spriteName = string.Empty;
			equipButton.SetActive(false);
			resolveButton.SetActive(false);
		}
		CheckEquipState();
	}

	public void Equip(bool equip = true)
	{
		m_EquipedIcon.gameObject.SetActive(equip);
		CheckEquipState();
	}

	public void Select()
	{
		swVoidSelectBackground.gameObject.SetActive(true);
		Refresh();
	}

	public void UnSelect()
	{
		swVoidSelectBackground.gameObject.SetActive(false);
	}

	private void OnPress(bool isPress)
	{
		if (isPress)
		{
			SwVoidMenuScript.mInstance.SetNeedUpdate();
		}
	}

	public void SetAlpha(float alpha)
	{
		foreach (UIWidget mWidget in mWidgets)
		{
			mWidget.alpha = alpha * m_BaseAlpha;
		}
	}

	public void ClickUpgradeButton()
	{
		if (isMaterialEnough)
		{
			if (isEmpty)
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_VOID_CREATE_TIPS"), 2, 50);
			}
			else
			{
				UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_VOID_UPGRADE"), 2, 50);
			}
		}
		else
		{
			UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_VOID_MATERIAL_NOTENOUGH"), 2, 54);
		}
	}

	public void Upgrade()
	{
		if (isEmpty)
		{
			if (mUserState.swVoidList.Count >= mUserState.swvoidCountLimit)
			{
				SwVoidMenuScript.mInstance.OnButtonClick(SwVoidMenuScript.mInstance.voidCountBack);
			}
			else if (GameApp.GetInstance().IsConnectedToInternet())
			{
				CustomizeUIScript.mInstance.WaitForSwVoid();
				if (GameApp.GetInstance().IsConnectedToServer())
				{
					SwVoidCreateRequest request = new SwVoidCreateRequest(type);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
				else
				{
					SwVoidCreateWWWRequest request2 = new SwVoidCreateWWWRequest(type);
					GameApp.GetInstance().GetWWWManager().SendRequest(request2);
					SwVoidMenuScript.mInstance.WaitOnlineInfo();
				}
			}
			else
			{
				UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_6"));
			}
		}
		else if (GameApp.GetInstance().IsConnectedToInternet())
		{
			CustomizeUIScript.mInstance.WaitForSwVoid();
			if (GameApp.GetInstance().IsConnectedToServer())
			{
				SwVoidUpgradeRequest request3 = new SwVoidUpgradeRequest(m_SwVoidSavedate.GetId());
				GameApp.GetInstance().GetNetworkManager().SendRequest(request3);
			}
			else
			{
				SwVoidUpgradeWWWRequest request4 = new SwVoidUpgradeWWWRequest(m_SwVoidSavedate.GetId());
				GameApp.GetInstance().GetWWWManager().SendRequest(request4);
				SwVoidMenuScript.mInstance.WaitOnlineInfo();
			}
		}
		else
		{
			UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_6"));
		}
	}

	public void ConfirmUpgrade()
	{
		if (GameApp.GetInstance().IsConnectedToInternet())
		{
			CustomizeUIScript.mInstance.WaitForSwVoid();
			if (GameApp.GetInstance().IsConnectedToServer())
			{
				SwVoidUpgradeConfirmRequest request = new SwVoidUpgradeConfirmRequest(m_SwVoidSavedate.GetId());
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				return;
			}
			SwVoidUpgradeConfirmWWWRequest request2 = new SwVoidUpgradeConfirmWWWRequest(m_SwVoidSavedate.GetId());
			GameApp.GetInstance().GetWWWManager().SendRequest(request2);
			SwVoidMenuScript.mInstance.WaitOnlineInfo();
			Refresh();
		}
		else
		{
			UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_6"));
		}
	}

	public void ReceiveConfirmUpgrade()
	{
		if (swVoidIdOfPlayer == mUserState.SwVoidIdInUsing && mUserState.SwVoidIdInUsing != 0)
		{
			mUserState.swVoidList[swVoidIdOfPlayer].TakeOff();
		}
		SwVoidDataManager.RealLevelUp(mUserState.swVoidList[swVoidIdOfPlayer], upgradeValue);
		level = mUserState.swVoidList[swVoidIdOfPlayer].GetLevel();
		Refresh();
		if (swVoidIdOfPlayer == mUserState.SwVoidIdInUsing && mUserState.SwVoidIdInUsing != 0)
		{
			mUserState.swVoidList[swVoidIdOfPlayer].TakeOn(mUserState.playerSkill);
		}
		mUserState.AnalysisManager.VoidUpgrade((int)m_SwVoidSavedate.GetType(), level);
		if (level >= 4)
		{
			mUserState.Achievement.CheckAchievement_VoidLevel5();
		}
	}

	public void ShowUpgradeInfo(short[] onlineUpgradeValue)
	{
		Refresh();
		upgradeValue = onlineUpgradeValue;
		if (upgradeValue != null)
		{
			string text = LocalizationManager.GetInstance().GetString("LOC_VOID_UPGRADE_CONFIRM") + "\n";
			string text2 = text;
			text = text2 + m_SwVoidSavedate.GetAttributeName(0) + " " + upgradeValue[0] + "\n";
			text = text + m_SwVoidSavedate.GetAttributeName(1) + " " + upgradeValue[1];
			if (SwVoidDataManager.IsAttributeMax(m_SwVoidSavedate.GetAttributeId(0), (byte)(level + 1), upgradeValue[0]) && SwVoidDataManager.IsAttributeMax(m_SwVoidSavedate.GetAttributeId(1), (byte)(level + 1), upgradeValue[1]) && level == 3)
			{
				mUserState.Achievement.CheckAchievement_GOODLUCK();
			}
			UIMsgBox.instance.ShowMessage(this, text, 2, 52);
		}
	}

	public void ShowPassiveDescription(int index)
	{
		if (m_SwVoidSavedate.isPassiveSkillShow(index))
		{
			passiveSkillDescription.SetActive(true);
			m_PassiveSkillDescriptionLable.text = string.Empty;
			m_PassiveSkillDescriptionLable.text += LocalizationManager.GetInstance().GetString(SwVoidDataManager.GetNameOfPassiveSkill(m_SwVoidSavedate.GetPassiveSkillId(index)));
			m_PassiveSkillDescriptionLable.text += "\n";
			m_PassiveSkillDescriptionLable.text += LocalizationManager.GetInstance().GetString(SwVoidDataManager.GetDescriptionOfPassiveSkill(m_SwVoidSavedate.GetPassiveSkillId(index)));
			if (!m_SwVoidSavedate.isPassiveSkillActive(index))
			{
				m_PassiveSkillDescriptionLable.text += "\n";
				m_PassiveSkillDescriptionLable.text += LocalizationManager.GetInstance().GetString("LOC_VOID_PSKILL_TIPS");
			}
		}
		else
		{
			passiveSkillDescription.SetActive(true);
			m_PassiveSkillDescriptionLable.text = LocalizationManager.GetInstance().GetString("LOC_VOID_PSKILL_TIPS_1");
		}
	}

	public void TryResolve()
	{
		UIMsgBox.instance.ShowMessage(this, LocalizationManager.GetInstance().GetString("LOC_VOID_RESOLVE_TIPS"), 2, 51);
	}

	public void Resolve()
	{
		if (GameApp.GetInstance().IsConnectedToInternet())
		{
			CustomizeUIScript.mInstance.WaitForSwVoid();
			if (GameApp.GetInstance().IsConnectedToServer())
			{
				SwVoidResolveRequest request = new SwVoidResolveRequest(m_SwVoidSavedate.GetId());
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
			else
			{
				SwVoidResolveWWWRequest request2 = new SwVoidResolveWWWRequest(m_SwVoidSavedate.GetId());
				GameApp.GetInstance().GetWWWManager().SendRequest(request2);
				SwVoidMenuScript.mInstance.WaitOnlineInfo();
			}
		}
		else
		{
			UIMsgBox.instance.ShowConfirmMessage(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_6"));
		}
	}

	public void receiveResolveInfo()
	{
		int num = mIndex - 1;
		if (num < 0)
		{
			num = 0;
		}
		if (mUserState.SwVoidIdInUsing == swVoidIdOfPlayer)
		{
			mUserState.SwVoidIdInUsing = 0;
		}
		mUserState.ResolveSwVoid(m_SwVoidSavedate.GetId());
		mUserState.AnalysisManager.ResolveVoid((int)m_SwVoidSavedate.GetType(), m_SwVoidSavedate.GetLevel());
		SwVoidMenuScript.mInstance.SetFocusIndex(num);
		SwVoidMenuScript.mInstance.Refresh(ESwVoidMenuTabState.VOID_TAB);
	}

	public void OnMsgButtonClick(UIMsg whichMsg, UIMsg.UIMsgButton buttonId)
	{
		if (whichMsg.EventId == 50)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				Upgrade();
			}
			else
			{
				UIMsgBox.instance.CloseMessage();
			}
		}
		else if (whichMsg.EventId == 51)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				Resolve();
			}
			else
			{
				UIMsgBox.instance.CloseMessage();
			}
		}
		else if (whichMsg.EventId == 52)
		{
			if (buttonId == UIMsg.UIMsgButton.Ok)
			{
				UIMsgBox.instance.CloseMessage();
				ConfirmUpgrade();
			}
			else
			{
				mUserState.Achievement.AddVoidUpgradeCancels();
				mUserState.AnalysisManager.GetVoidInfo()[(int)m_SwVoidSavedate.GetType()].CancelNum[m_SwVoidSavedate.GetLevel()]++;
				UIMsgBox.instance.CloseMessage();
			}
		}
		else if (whichMsg.EventId == 0)
		{
			UIMsgBox.instance.CloseMessage();
		}
		else if (whichMsg.EventId == 54)
		{
			UIMsgBox.instance.CloseMessage();
			if (buttonId == UIMsg.UIMsgButton.Ok && IapScript.mInstance != null)
			{
				IapScript.mInstance.OpenPage(IapScript.IapType.Nothing);
				IapScript.mInstance.IsGotoMaterialChest = true;
				IapScript.mInstance.OpenPage(IapScript.IapType.Package);
			}
		}
		else if (whichMsg.EventId == 93)
		{
			UIMsgBox.instance.CloseMessage();
		}
	}

	public void CheckEquipState()
	{
		if (m_SwVoidSavedate != null)
		{
			if (mUserState.SwVoidIdInUsing != 0 && mUserState.SwVoidIdInUsing == swVoidIdOfPlayer)
			{
				m_EquipedIcon.gameObject.SetActive(true);
				m_EquipButton.text = LocalizationManager.GetInstance().GetString("LOC_CUSTOMIZE_TAKEOFF");
			}
			else
			{
				m_EquipedIcon.gameObject.SetActive(false);
				m_EquipButton.text = LocalizationManager.GetInstance().GetString("LOC_BUTTON_EQUIP");
			}
		}
	}

	public void ResetSwVoidSaveData()
	{
		if (swVoidIdOfPlayer != 0)
		{
			m_SwVoidSavedate = mUserState.swVoidList[swVoidIdOfPlayer];
		}
	}
}
