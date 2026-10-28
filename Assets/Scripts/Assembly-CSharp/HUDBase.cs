using UnityEngine;

public class HUDBase : GameUI
{
	public enum HUDType
	{
		None = 0,
		Battle = 1,
		CoopBattle = 2,
		VSTDMBattle = 3,
		VSKOHBattle = 4,
		VSDominationBattle = 5,
		VSCTFBattle = 6,
		VSFFABattle = 7,
		VSMutant = 8
	}

	protected enum HUDState
	{
		None = 0,
		Show = 1,
		Hide = 2
	}

	public const string BUNDLE_KEY_HUD_TYPE = "HUDType";

	public static HUDBase instance;

	public GameObject m_StaticPanel;

	public SkillHUDManager m_SkillHUDManager;

	public KillStreakHUDManager m_KillStreakHUDManager;

	public JoystickManager m_JoystickManager;

	public BossStickManager m_BossStickmanager;

	public StateWeapon m_Weapon;

	public StateAim m_Aim;

	public StateItem m_StateItem;

	public GameObject VehicleBarObject;

	public UISprite VehicleEnergyBar;

	public UISprite EnergyHighlight;

	public Transform InitHighlightPos;

	public GameObject getOffVehicle;

	public GameObject PlayerHPObj;

	public UISprite m_PlayerHpSprite;

	public UISprite m_PlayerHpBackgroundSprite;

	public UISprite m_PlayerHpEffect;

	public UILabel m_PlayerHpLabel;

	public UILabel m_AverageFrameTimeLabel;

	protected HUDType mType;

	protected float mLastUpdateAimTime;

	protected HUDState hudState;

	protected Player player;

	protected int mVehicleBarMaxWidth = 240;

	private float mHpEffectSpeed = 0.3f;

	private long mPassedFrame;

	private float mPassedTime;

	private float mUpdateFrameLabelTime = 0.5f;

	private float mLastUpdateFrameLabelTime;

	private Vector3 getOffVehiclePosition = new Vector3(37.1f, 0f, 0f);

	private Vector3 getOnVehiclePosition = new Vector3(68.64f, -41.1f, 0f);

	private void Awake()
	{
		instance = this;
		CloseAll();
		mType = HUDType.None;
		player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		mPassedFrame = 0L;
		mPassedTime = 0f;
		mLastUpdateFrameLabelTime = Time.time;
		if (m_AverageFrameTimeLabel != null)
		{
			m_AverageFrameTimeLabel.gameObject.SetActive(false);
		}
	}

	protected override void OnInit(GameUIBundle bundle)
	{
		base.OnInit(bundle);
		if (bundle != null)
		{
			object obj = bundle.Get("HUDType");
			if (obj != null)
			{
				LoadHUD((HUDType)(int)obj);
			}
		}
		UIAtlasManager.UpdateMatInGame();
	}

	public void LoadHUD(HUDType type)
	{
		if (type == HUDType.Battle)
		{
			LoadSubUI(new BattleSubHUD());
		}
	}

	private void OnDestroy()
	{
		instance = null;
		Debug.Log("HUDManager destroy");
	}

	public void Invalidate()
	{
		CloseAll();
		mType = HUDType.None;
	}

	protected override void OnUIUpdate()
	{
		if (player == null)
		{
			player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		}
        
        if (InGameFinishState())
		{
			Hide();
		}
		else if (InWaitRebirthState())
		{
			HideWhenWaitingRebirth();
		}
		else
		{
			Show();
		}
		UpdatePlayerHP();
		if (player != null && player.mVehicle != null)
		{
			float remainTimePercentage = player.mVehicle.GetRemainTimePercentage();
			VehicleEnergyBar.fillAmount = remainTimePercentage;
			EnergyHighlight.transform.localPosition = new Vector3(InitHighlightPos.localPosition.x - (1f - remainTimePercentage) * (float)mVehicleBarMaxWidth, 0f, 0f);
		}
		else if (player != null)
		{
			if (!getOffVehicle.activeInHierarchy && ((LocalPlayer)player).GetNeighborLongSwordRider() != null)
			{
				HideShowVehicleGetOn();
			}
			else if (getOffVehicle.activeInHierarchy && ((LocalPlayer)player).GetNeighborLongSwordRider() == null)
			{
				HideShowVehicleGetOn(false);
			}
		}
	}

	protected void CloseAll()
	{
	}

	protected override void OnShow()
	{
		hudState = HUDState.Show;
	}

	protected override void OnHide()
	{
		hudState = HUDState.Hide;
	}

	public HUDType GetRunningHUDType()
	{
		return mType;
	}

	public void Hide()
	{
		m_SkillHUDManager.Hide();
		m_JoystickManager.Hide();
		m_Weapon.Hide();
		m_Aim.Hide();
	}

	public void Show()
	{
		m_SkillHUDManager.Show();
		m_JoystickManager.Show();
		m_Weapon.Show();
		m_Aim.Show();
	}

	public virtual void HideWhenWaitingRebirth()
	{
		m_SkillHUDManager.Hide();
		m_JoystickManager.Hide();
		m_Weapon.Hide();
		m_Aim.Hide();
	}

	public bool InGameFinishState()
	{
		if (player != null)
		{
			return player.State == Player.WIN_STATE || player.State == Player.LOSE_STATE;
		}
		return false;
	}

	public bool InWaitRebirthState()
	{
		if (player != null)
		{
			return player.State == Player.DEAD_STATE || player.State == Player.WAIT_REBIRTH_STATE || player.State == Player.WAIT_VS_REBIRTH_STATE;
		}
		return false;
	}

	public void HideShowVehicleGetOff(bool activeButton = true)
	{
		if (activeButton)
		{
			StateBuff.mInstance.transform.localPosition = Vector3.down * 35f;
		}
		else if (!activeButton)
		{
			StateBuff.mInstance.transform.localPosition = Vector3.zero;
		}
		getOffVehicle.SetActive(activeButton);
		getOffVehicle.transform.localScale = Vector3.one;
		getOffVehicle.transform.localPosition = getOffVehiclePosition;
		VehicleBarObject.SetActive(activeButton);
		VehicleEnergyBar.width = mVehicleBarMaxWidth;
	}

	public void HideShowVehicleGetOn(bool activeButton = true)
	{
		if (activeButton)
		{
			StateBuff.mInstance.transform.localPosition = Vector3.down * 100f;
		}
		else if (!activeButton)
		{
			StateBuff.mInstance.transform.localPosition = Vector3.zero;
		}
		getOffVehicle.SetActive(activeButton);
		getOffVehicle.transform.localScale = Vector3.one * 2f;
		getOffVehicle.transform.localPosition = getOnVehiclePosition;
		VehicleBarObject.SetActive(false);
	}

	public virtual void OnPause()
	{
	}

	public virtual void OnResume()
	{
	}

	public void ChangeBossControlHUD(byte[] bossSkills)
	{
		m_JoystickManager.gameObject.SetActive(false);
		m_BossStickmanager.gameObject.SetActive(true);
		m_SkillHUDManager.gameObject.SetActive(false);
		m_StateItem.gameObject.SetActive(false);
		m_Weapon.Hide();
		if (StateBuff.mInstance != null)
		{
			StateBuff.mInstance.gameObject.SetActive(false);
		}
		m_Aim.ChangeWeapon("front star9");
		for (int i = 0; i < bossSkills.Length; i++)
		{
			if (i >= m_BossStickmanager.m_SkillJoysticks.Length)
			{
				continue;
			}
			Debug.Log(bossSkills[i]);
			if (bossSkills[i] == 0)
			{
				m_BossStickmanager.m_SkillJoysticks[i].gameObject.SetActive(false);
				continue;
			}
			if (player == null)
			{
				break;
			}
			m_BossStickmanager.m_SkillJoysticks[i].gameObject.SetActive(true);
			m_BossStickmanager.m_SkillJoysticks[i].GetComponent<SkillStick>().SetSkill(bossSkills[i], i + 1);
			if (MutantSkill.isActiveBuff(bossSkills[i]))
			{
				m_BossStickmanager.m_SkillJoysticks[i].GetComponent<SkillStick>().SetTimer((byte)player.BossIndex);
			}
			else
			{
				m_BossStickmanager.m_SkillJoysticks[i].GetComponent<SkillStick>().SetTimerNull();
			}
		}
	}

	public void ItemBarReposition()
	{
		if (m_KillStreakHUDManager != null && m_KillStreakHUDManager.m_KillStreakGrid != null && m_KillStreakHUDManager.m_KillStreakGrid.transform.childCount > 0)
		{
			if (m_StateItem != null && m_StateItem.ItemsGrid != null && m_StateItem.ItemsGrid.transform.childCount > 0)
			{
				float num = (float)m_StateItem.ItemsGrid.transform.childCount * m_StateItem.ItemsGrid.cellWidth;
				float num2 = (float)m_KillStreakHUDManager.m_KillStreakGrid.transform.childCount * m_KillStreakHUDManager.m_KillStreakGrid.cellWidth;
				Vector3 localPosition = m_StateItem.ItemsGrid.transform.localPosition;
				Vector3 localPosition2 = m_KillStreakHUDManager.m_KillStreakGrid.transform.localPosition;
				m_StateItem.ItemsGrid.transform.localPosition = new Vector3((0f - (num + num2)) / 2f + m_StateItem.ItemsGrid.cellWidth / 2f, localPosition.y, localPosition.z);
				m_StateItem.ItemsGrid.Reposition();
				m_KillStreakHUDManager.m_KillStreakGrid.transform.localPosition = new Vector3(m_StateItem.ItemsGrid.transform.localPosition.x + num + m_StateItem.ItemsGrid.cellWidth / 2f - m_KillStreakHUDManager.m_KillStreakGrid.cellWidth / 2f, localPosition2.y, localPosition2.z);
				m_KillStreakHUDManager.m_KillStreakGrid.Reposition();
			}
			else
			{
				m_KillStreakHUDManager.Reposition();
			}
		}
		else if (m_StateItem != null)
		{
			m_StateItem.Reposition();
		}
	}

	protected void UpdatePlayerHP()
	{
		if (player != null)
		{
			float num = (float)player.Hp / (float)player.MaxHp;
			m_PlayerHpSprite.fillAmount = num;
			if (m_PlayerHpLabel != null && this is HUDManager)
			{
				m_PlayerHpLabel.text = string.Format("{0:N0} / {1:N0}", player.Hp, player.MaxHp);
			}
			if (num < m_PlayerHpEffect.fillAmount)
			{
				m_PlayerHpEffect.fillAmount -= Time.deltaTime * mHpEffectSpeed;
			}
			else
			{
				m_PlayerHpEffect.fillAmount = num;
			}
			if (num < 0.3f)
			{
				m_PlayerHpSprite.spriteName = "bar_4_full";
				m_PlayerHpBackgroundSprite.spriteName = "bar_4_empty";
			}
			else
			{
				m_PlayerHpSprite.spriteName = "bar_3_full";
				m_PlayerHpBackgroundSprite.spriteName = "bar_3_empty";
			}
		}
	}
}
