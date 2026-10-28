using System.Collections.Generic;
using UnityEngine;

public class StateBuff : MonoBehaviour
{
	public enum EBuffType
	{
		FLASK = 0,
		SKILL = 1,
		SHIELD = 2,
		MACHINE_GUN_STACK = 3,
		NIGHTMARE_CHARGE = 4,
		SUPER_SHIELD = 5
	}

	public static StateBuff mInstance;

	public GameObject BuffTemplate;

	public UIGrid grid;

	public UIAtlas FlaskIconAtlas;

	public UIAtlas SkillIconAtlas;

	public GameObject DescriptionObj;

	public UISprite DescriptionFrame;

	public UILabel DescriptionLabel;

	public TweenAlpha DescFrameTween;

	public TweenAlpha DescLabelTween;

	private float mLastUpdateTime;

	private float mUpdateInterval = 0.1f;

	private bool isShiledShow;

	private bool isMachineGunShow;

	private bool isChargeGunShow;

	private bool isSuperShieldStateShow;

	private GameObject ShieldBuffObject;

	private GameObject MachineGunStackObject;

	private GameObject ChargeGunStackObject;

	private GameObject SuperShieldObject;

	private Dictionary<int, GameObject> mActivedAdvancedSkillBuffs = new Dictionary<int, GameObject>();

	private void Start()
	{
		mLastUpdateTime = Time.time;
		mActivedAdvancedSkillBuffs.Clear();
		mInstance = this;
		List<Item> flaskItemList = GameApp.GetInstance().GetUserState().GetFlaskItemList();
		foreach (Item item in flaskItemList)
		{
			AddBuff(EBuffType.FLASK, item.GetItemID(), null, item);
		}
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void Update()
	{
		if (!(Time.time - mLastUpdateTime >= mUpdateInterval))
		{
			return;
		}
		mLastUpdateTime = Time.time;
		Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (localPlayer == null)
		{
			return;
		}
		PlayerSkill skills = localPlayer.GetSkills();
		List<AdvancedSkill> advancedSkills = skills.GetAdvancedSkills();
		List<int> list = new List<int>();
		foreach (AdvancedSkill item in advancedSkills)
		{
			list.Add(item.GetExternalId());
			if (item.GetBuffState() != AdvancedSkill.EBuffState.NONE && item.GetIconName() != string.Empty)
			{
				if (item.IsActived())
				{
					if (!mActivedAdvancedSkillBuffs.ContainsKey(item.GetExternalId()))
					{
						AddBuff(EBuffType.SKILL, -1, item, null);
					}
				}
				else if (mActivedAdvancedSkillBuffs.ContainsKey(item.GetExternalId()))
				{
					Object.DestroyImmediate(mActivedAdvancedSkillBuffs[item.GetExternalId()]);
					mActivedAdvancedSkillBuffs.Remove(item.GetExternalId());
					grid.Reposition();
				}
			}
			if (item.GetOnTriggerType() == AdvancedSkill.EOnTriggerType.OPEN_SUPER_SHIELD && !isSuperShieldStateShow)
			{
				isSuperShieldStateShow = true;
				AddBuff(EBuffType.SUPER_SHIELD, -1, item, null);
			}
		}
		if (localPlayer.ItemShield > 0)
		{
			if (!isShiledShow)
			{
				isShiledShow = true;
				AddBuff(EBuffType.SHIELD, -1, null, null);
			}
		}
		else if (isShiledShow && ShieldBuffObject != null)
		{
			isShiledShow = false;
			Object.DestroyImmediate(ShieldBuffObject);
			grid.Reposition();
		}
		if (localPlayer.GetWeapon().GetWeaponType() == WeaponType.MachineGun)
		{
			if (!isMachineGunShow)
			{
				isMachineGunShow = true;
				AddBuff(EBuffType.MACHINE_GUN_STACK, -1, null, null);
			}
		}
		else if (isMachineGunShow && MachineGunStackObject != null)
		{
			isMachineGunShow = false;
			Object.DestroyImmediate(MachineGunStackObject);
			grid.Reposition();
		}
		if (localPlayer.GetWeapon().GetWeaponType() == WeaponType.ChargeDoublePistol)
		{
			if (!isChargeGunShow)
			{
				isChargeGunShow = true;
				AddBuff(EBuffType.NIGHTMARE_CHARGE, -1, null, null);
			}
		}
		else if (isChargeGunShow && ChargeGunStackObject != null)
		{
			isChargeGunShow = false;
			Object.DestroyImmediate(ChargeGunStackObject);
			grid.Reposition();
		}
		List<int> list2 = new List<int>();
		foreach (int key in mActivedAdvancedSkillBuffs.Keys)
		{
			if (!list.Contains(key))
			{
				list2.Add(key);
			}
		}
		foreach (int item2 in list2)
		{
			Object.DestroyImmediate(mActivedAdvancedSkillBuffs[item2]);
			mActivedAdvancedSkillBuffs.Remove(item2);
			grid.Reposition();
		}
	}

	public void AddBuff(EBuffType type, int buffId, AdvancedSkill skill, Item item)
	{
		GameObject gameObject = Object.Instantiate(BuffTemplate, Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.transform.parent = grid.transform;
		gameObject.transform.localScale = Vector3.one;
		gameObject.transform.localPosition = Vector3.zero;
		StateBuffContent component = gameObject.GetComponent<StateBuffContent>();
		switch (type)
		{
		case EBuffType.FLASK:
			if (buffId != -1)
			{
				component.InitForFlask(FlaskIconAtlas, buffId, item);
			}
			break;
		case EBuffType.SKILL:
			if (skill != null)
			{
				component.InitForSkill(SkillIconAtlas, skill);
				mActivedAdvancedSkillBuffs.Add(skill.GetExternalId(), gameObject);
			}
			break;
		case EBuffType.SHIELD:
			ShieldBuffObject = gameObject;
			component.InitForShield(SkillIconAtlas);
			break;
		case EBuffType.MACHINE_GUN_STACK:
			MachineGunStackObject = gameObject;
			component.InitForMachineGun(SkillIconAtlas);
			break;
		case EBuffType.NIGHTMARE_CHARGE:
			ChargeGunStackObject = gameObject;
			component.InitForChargeGun(SkillIconAtlas);
			break;
		case EBuffType.SUPER_SHIELD:
			SuperShieldObject = gameObject;
			component.InitForSuperShield(SkillIconAtlas, skill);
			break;
		}
		component.SetAlpha(0.8f);
		grid.Reposition();
	}

	public void ShowDescription(string desc, string frameSpriteName)
	{
		DescriptionObj.SetActive(true);
		DescriptionLabel.text = desc;
		DescriptionFrame.spriteName = frameSpriteName;
	}

	public void HideDescription()
	{
		DescriptionObj.SetActive(false);
	}
}
