using System.Collections.Generic;
using UnityEngine;

public class StateWeapon : MonoBehaviour
{
	public UISprite m_Weapon;

	public UISprite m_Background;

	public GameObject ChargeBarObject;

	public UISprite ChargeBarSprite;

	public UISprite ChargeHighlight;

	public Transform InitHighlightPos;

	public UISprite FullChargeEffect;

	public GameObject ChangeWeaponIcon;

	private Player player;

	private UserState userState;

	private List<Weapon> mBagWeaponList = new List<Weapon>();

	private int mCurrentIndex;

	private int mChargeBarMaxWidth = 118;

	private void Awake()
	{
		Refresh();
	}

	public void Refresh()
	{
		player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (player.IsBoss)
		{
			return;
		}
		userState = GameApp.GetInstance().GetUserState();
		mBagWeaponList = userState.GetBattleWeapons();
		if (mBagWeaponList.Count > 1)
		{
			ChangeWeaponIcon.SetActive(true);
		}
		else
		{
			ChangeWeaponIcon.SetActive(false);
		}
		mCurrentIndex = 0;
		if (player == null)
		{
			return;
		}
		for (int i = 0; i < mBagWeaponList.Count; i++)
		{
			if (mBagWeaponList[i] == player.GetWeapon())
			{
				mCurrentIndex = i;
				break;
			}
		}
		m_Weapon.spriteName = "weapons_" + mBagWeaponList[mCurrentIndex].GunID;
		player.ChangeWeaponInBag(userState.GetWeaponBagIndex(mBagWeaponList[mCurrentIndex]));
		if (player.GetWeapon().GetWeaponType() == WeaponType.RailGun)
		{
			ChargeBarObject.SetActive(true);
		}
		else
		{
			ChargeBarObject.SetActive(false);
		}
	}

	private void Update()
	{
		if (player == null)
		{
			player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if (player != null)
			{
				for (int i = 0; i < mBagWeaponList.Count; i++)
				{
					if (mBagWeaponList[i] == player.GetWeapon())
					{
						mCurrentIndex = i;
						break;
					}
				}
				m_Weapon.spriteName = "weapons_" + mBagWeaponList[mCurrentIndex].GunID;
			}
		}
		if (player != null && player.GetWeapon().GetWeaponType() == WeaponType.RailGun)
		{
			RailGun railGun = player.GetWeapon() as RailGun;
			float energyPercentage = railGun.GetEnergyPercentage();
			ChargeBarSprite.fillAmount = energyPercentage;
			ChargeHighlight.transform.localPosition = new Vector3(InitHighlightPos.localPosition.x - (1f - energyPercentage) * (float)mChargeBarMaxWidth, 0f, 0f);
			if (energyPercentage >= 1f)
			{
				FullChargeEffect.gameObject.SetActive(true);
			}
			else if (FullChargeEffect.gameObject.activeSelf)
			{
				FullChargeEffect.gameObject.SetActive(false);
			}
			if (energyPercentage <= 0f || energyPercentage >= 1f)
			{
				ChargeHighlight.gameObject.SetActive(false);
			}
			else if (!ChargeHighlight.gameObject.activeSelf)
			{
				ChargeHighlight.gameObject.SetActive(true);
			}
		}
	}

	private void OnPress(bool pressed)
	{
		if (pressed)
		{
			if (!player.IsInVehicleState())
			{
				m_Background.spriteName = "weapon_bk1";
				mCurrentIndex++;
				mCurrentIndex %= mBagWeaponList.Count;
				ChangeWeapon();
			}
		}
		else
		{
			m_Background.spriteName = "weapon_bk";
		}
	}

	public void ChangeWeaponByJoystick()
	{
		if (!player.IsInVehicleState())
		{
			m_Background.spriteName = "weapon_bk1";
			mCurrentIndex++;
			mCurrentIndex %= mBagWeaponList.Count;
			ChangeWeapon();
			m_Background.spriteName = "weapon_bk";
		}
	}

	private void ChangeWeapon()
	{
		m_Weapon.spriteName = "weapons_" + mBagWeaponList[mCurrentIndex].GunID;
		AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_DRAG);
		player.ChangeWeaponInBag(userState.GetWeaponBagIndex(mBagWeaponList[mCurrentIndex]));
		if (player.GetWeapon().GetWeaponType() == WeaponType.RailGun)
		{
			ChargeBarObject.SetActive(true);
		}
		else
		{
			ChargeBarObject.SetActive(false);
		}
	}

	public void Hide()
	{
		m_Weapon.gameObject.SetActive(false);
		base.GetComponent<Collider>().enabled = false;
	}

	public void Show()
	{
		m_Weapon.gameObject.SetActive(true);
		base.GetComponent<Collider>().enabled = true;
	}
}
