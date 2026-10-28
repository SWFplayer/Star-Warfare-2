using System;
using UnityEngine;

public class WeaponLevelUpWindow : MonoBehaviour
{
	private const int STATE_1 = 0;

	private const int STATE_2 = 1;

	private const int STATE_3 = 2;

	private const int STATE_4 = 3;

	public UISprite WeaponIcon;

	public UISprite InnerRound;

	public TweenPosition LevelLabelMove;

	public UILabel OldLevelLabel;

	public UILabel NewLevelLabel;

	public TweenAlpha LevelUpBlinking;

	public UILabel DamageBaseLabel;

	public UILabel DamageUpLabel;

	public UISprite DamageBaseSprite;

	public UISprite DamageUpSprite;

	public UILabel FireRateBaseLabel;

	public UISprite FireRateBaseSprite;

	public UILabel SpeedBaseLabel;

	public UISprite SpeedBaseSprite;

	public GameObject ConfirmButton;

	private int mState;

	private bool mState1_Entered;

	private bool mState2_Entered;

	private bool mState3_Entered;

	private bool mState4_Entered;

	private float m_Time;

	private float m_StateTime_1 = 1.5f;

	private float m_StateTime_2 = 0.2f;

	private float m_StateTime_3 = 1f;

	private float m_StateTime_4 = 1f;

	private float m_InceaseTime;

	private float m_TotalIncreaseTime = 0.25f;

	private Weapon mTargetWeapon;

	private int mOldLevel = 1;

	private int mNewLevel = 1;

	private float mDeltaDamageFill = 0.1f;

	private float mMaxDamage;

	private float mMaxFireRate = 12f;

	private float mMaxSpeed = 2f;

	private float mMinSpeed = -2f;

	private bool mNeedUpdate;

	private void Start()
	{
		m_Time = 0f;
		mState = 0;
		mState1_Entered = false;
		mState2_Entered = false;
		mState3_Entered = false;
		mState4_Entered = false;
		UIEventListener uIEventListener = UIEventListener.Get(ConfirmButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnButtonClick));
	}

	public void Init(Weapon weapon, int oldLevel, int newLevel)
	{
		if (weapon != null)
		{
			mNeedUpdate = true;
			mTargetWeapon = weapon;
			mOldLevel = oldLevel;
			mNewLevel = newLevel;
			GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.WEAPON);
			int gunID = weapon.GunID;
			WeaponIcon.spriteName = UIConstant.EQUIPS_ICON_STRING_PREFIX[5] + gunID;
			float damageByLevel = weapon.GetDamageByLevel(oldLevel);
			float damageByLevel2 = weapon.GetDamageByLevel(newLevel);
			float num = 1f;
			float num2 = 0f;
			for (int i = 0; i < gameDataTable.Count - 1; i++)
			{
				int num3 = gameDataTable.GetInt(i, 1);
				if (num3 == gunID)
				{
					num = 1f / gameDataTable.GetFloat(i, 3);
					num2 = gameDataTable.GetFloat(i, 11);
					break;
				}
			}
			mMaxDamage = weapon.GetDamageByLevel(Global.MAX_LEVEL_WEAPONW);
			mMaxFireRate = 14f;
			OldLevelLabel.text = oldLevel.ToString();
			NewLevelLabel.text = newLevel.ToString();
			mDeltaDamageFill = (damageByLevel2 - damageByLevel) / (m_TotalIncreaseTime * mMaxDamage);
			DamageBaseSprite.fillAmount = damageByLevel / mMaxDamage;
			DamageUpSprite.fillAmount = DamageBaseSprite.fillAmount;
			DamageBaseLabel.text = Mathf.CeilToInt(damageByLevel).ToString();
			DamageUpLabel.text = "+" + Mathf.CeilToInt(damageByLevel2 - damageByLevel);
			FireRateBaseSprite.fillAmount = num / mMaxFireRate;
			FireRateBaseLabel.text = num.ToString("#0.00");
			SpeedBaseSprite.fillAmount = (num2 - mMinSpeed) / (mMaxSpeed - mMinSpeed);
			SpeedBaseLabel.text = num2.ToString("#0.0");
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
	}

	private void Update()
	{
		if (!mNeedUpdate)
		{
			return;
		}
		m_Time += RealTime.deltaTime;
		switch (mState)
		{
		case 0:
			if (!mState1_Entered)
			{
				mState1_Entered = true;
			}
			else if (m_Time >= m_StateTime_1)
			{
				mState = 1;
				m_Time = 0f;
			}
			break;
		case 1:
			if (!mState2_Entered)
			{
				mState2_Entered = true;
				LevelLabelMove.PlayForward();
			}
			else if (m_Time >= m_StateTime_2)
			{
				mState = 2;
				m_Time = 0f;
			}
			break;
		case 2:
			if (!mState3_Entered)
			{
				mState3_Entered = true;
				LevelUpBlinking.PlayForward();
				DamageUpLabel.gameObject.SetActive(true);
				m_InceaseTime = 0f;
				break;
			}
			m_InceaseTime += RealTime.deltaTime;
			if (m_InceaseTime <= m_TotalIncreaseTime)
			{
				DamageUpSprite.fillAmount += mDeltaDamageFill * RealTime.deltaTime;
			}
			if (m_Time >= m_StateTime_3)
			{
				mState = 3;
				m_Time = 0f;
			}
			break;
		case 3:
			if (!mState4_Entered)
			{
				mState4_Entered = true;
				TweenAlpha.Begin(InnerRound.gameObject, 0.2f, 0f);
				TweenScale.Begin(InnerRound.gameObject, 0.2f, Vector3.one * 2f);
			}
			break;
		}
	}

	private void OnButtonClick(GameObject go)
	{
		if (go == ConfirmButton)
		{
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
			UnityEngine.Object.DestroyImmediate(base.gameObject);
		}
	}
}
