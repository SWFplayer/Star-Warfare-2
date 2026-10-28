using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRecordDataInfo : MonoBehaviour
{
	public UILabel LastLoginTime;

	public UILabel TotalPlayTime;

	public UILabel Gold;

	public UILabel Mithril;

	public GameObject[] Weapons;

	public UISprite[] WeaponIcons;

	public GameObject Void;

	public GameObject VoidIcon;

	public GameObject AvatarFrame;

	private GameObject mModel;

	private Vector3 m_Rotation = new Vector3(0f, 128f, 0f);

	private Vector3 m_Pos;

	private Vector3 m_Scale = Vector3.one * 0.7f;

	private string mAnimationName;

	public void InitByUserState(UserState userState)
	{
		LastLoginTime.text = string.Empty;
		if (userState.GetSyncRecordTime().Ticks > 0)
		{
			LastLoginTime.text = userState.GetSyncRecordTime().ToString("yyyy/MM/dd");
		}
		TimeSpan timeSpan = TimeSpan.FromSeconds(userState.GetTotalPlayTime());
		TotalPlayTime.text = string.Format("{0:G}:{1:D2}", timeSpan.Hours + timeSpan.Days * 24, timeSpan.Minutes);
		Gold.text = userState.GetCash().ToString();
		Mithril.text = userState.GetMithril().ToString();
		List<Weapon> battleWeapons = userState.GetBattleWeapons();
		for (int i = 0; i < Weapons.Length; i++)
		{
			if (i < battleWeapons.Count && battleWeapons[i] != null)
			{
				Weapons[i].SetActive(true);
				WeaponIcons[i].spriteName = UIConstant.EQUIPS_ICON_STRING_PREFIX[5] + battleWeapons[i].GunID;
			}
			else
			{
				Weapons[i].SetActive(false);
			}
		}
		short[] array = new short[6];
		short[] avatar = userState.GetAvatar();
		for (int j = 0; j < avatar.Length; j++)
		{
			array[j] = avatar[j];
		}
		Weapon weapon = userState.GetBattleWeapons()[0];
		array[5] = (short)weapon.GunID;
		ChangeAvatar(array, false);
	}

	public void InitByRecordData(RecordData recordData)
	{
		LastLoginTime.text = recordData.mSyncRecordTime.ToString("yyyy/MM/dd");
		TimeSpan timeSpan = TimeSpan.FromSeconds(recordData.mTotalPlayTime);
		TotalPlayTime.text = string.Format("{0:G}:{1:D2}", timeSpan.Hours + timeSpan.Days * 24, timeSpan.Minutes);
		Gold.text = recordData.mCash.ToString();
		Mithril.text = recordData.mMithril.ToString();
		short[] mWeaponIdArray = recordData.mCustomizeData.mWeaponIdArray;
		short num = 0;
		for (int i = 0; i < Weapons.Length; i++)
		{
			if (i < mWeaponIdArray.Length && mWeaponIdArray[i] > 0)
			{
				Weapons[i].SetActive(true);
				WeaponIcons[i].spriteName = UIConstant.EQUIPS_ICON_STRING_PREFIX[5] + mWeaponIdArray[i];
				if (num == 0)
				{
					num = mWeaponIdArray[i];
				}
			}
			else
			{
				Weapons[i].SetActive(false);
			}
		}
		short[] array = new short[6];
		short[] mAvatarIdArray = recordData.mCustomizeData.mAvatarIdArray;
		for (int j = 0; j < mAvatarIdArray.Length; j++)
		{
			array[j] = mAvatarIdArray[j];
		}
		array[5] = num;
		ChangeAvatar(array, false);
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
		UnityEngine.Object original = Resources.Load(path);
		GameObject gameObject2 = (GameObject)UnityEngine.Object.Instantiate(original);
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
			GameObject gameObject3 = (GameObject)UnityEngine.Object.Instantiate(original);
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
			UnityEngine.Object.Destroy(mModel);
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
