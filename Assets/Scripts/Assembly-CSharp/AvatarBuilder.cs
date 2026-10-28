using System.Collections.Generic;
using UnityEngine;

internal class AvatarBuilder
{
	protected static AvatarBuilder instance = new AvatarBuilder();

	protected UserState lastUserState;

	public static AvatarBuilder GetInstance()
	{
		return instance;
	}

	private void BindBones(SkinnedMeshRenderer smr, List<Transform> bones, GameObject parentObj)
	{
		Transform[] array = new Transform[smr.bones.Length];
		for (int i = 0; i < smr.bones.Length; i++)
		{
			string name = smr.bones[i].name;
			for (int j = 0; j < bones.Count; j++)
			{
				if (name == bones[j].name)
				{
					array[i] = bones[j];
					break;
				}
			}
		}
		smr.bones = array;
		smr.transform.parent = parentObj.transform;
	}

	private void TraverseBones(Transform t, List<Transform> bones)
	{
		bones.Add(t);
		if (t.childCount > 0)
		{
			for (int i = 0; i < t.childCount; i++)
			{
				TraverseBones(t.GetChild(i), bones);
			}
		}
	}

	private void AddAnimation(string name, GameObject playerObject)
	{
		GameObject gameObject = Resources.Load("Character/Animation/" + name) as GameObject;
		playerObject.GetComponent<Animation>().AddClip(gameObject.GetComponent<Animation>()[name].clip, name);
	}

	private void AddAnimations(GameObject player, Player p)
	{
		List<Weapon> battleWeapons = lastUserState.GetBattleWeapons();
		HashSet<WeaponType> hashSet = new HashSet<WeaponType>();
		foreach (Weapon item in battleWeapons)
		{
			if (!hashSet.Contains(item.GetWeaponType()))
			{
				hashSet.Add(item.GetWeaponType());
			}
		}
		bool flag = ((p.GetSkills().GetSkill(SkillsType.FLY) > 0f) ? true : false);
		AddAnimation("stand_all_attacked_back", player);
		AddAnimation("stand_all_attacked_back2", player);
		AddAnimation("stand_all_attacked_front", player);
		AddAnimation(AnimationString.Dead01, player);
		AddAnimation(AnimationString.Dead02, player);
		AddAnimation("stand_all_win01", player);
		AddAnimation("stand_all_win02", player);
		AddAnimation("stand_all_hurt", player);
		AddAnimation("stand_all_hurt_idle", player);
		AddAnimation("fly_all_idle", player);
		AddAnimation("fly_all_forward", player);
		AddAnimation("fly_all_back", player);
		AddAnimation("fly_all_left", player);
		AddAnimation("fly_all_right", player);
		AddAnimation("fly_all_dead01", player);
		AddAnimation("fly_all_win01", player);
		AddAnimation("ATV_mini_idle", player);
		AddAnimation("ATV_mini_forward", player);
		AddAnimation("ATV_mini_back", player);
		AddAnimation("ATV_mini_left", player);
		AddAnimation("ATV_mini_right", player);
		AddAnimation("ATV_mini_shoot_idle", player);
		AddAnimation("ATV_mini_shoot_forward", player);
		AddAnimation("ATV_mini_shoot_back", player);
		AddAnimation("ATV_mini_shoot_left", player);
		AddAnimation("ATV_mini_shoot_right", player);
		AddAnimation("ATV_mini_GetOn", player);
		AddAnimation("ATV_mini_GetOff", player);
		AddAnimation("ATV_mini_FallOff", player);
		AddAnimation("UNCC_Turret_FallOff", player);
		AddAnimation("UNCC_Turret_GetOn", player);
		AddAnimation("UNCC_Turret_idle", player);
		AddAnimation("UNCC_Turret_left", player);
		AddAnimation("UNCC_Turret_right", player);
		AddAnimation("UNCC_Turret_shoot_idle", player);
		AddAnimation("UNCC_Turret_shoot_left", player);
		AddAnimation("UNCC_Turret_shoot_right", player);
		AddAnimation("UNCC_BUDDY01_idle", player);
		AddAnimation("UNCC_BUDDY01_GetOn", player);
		AddAnimation("UNCC_BUDDY01_left", player);
		AddAnimation("UNCC_BUDDY01_right", player);
		AddAnimation("UNCC_BUDDY01_shoot_left", player);
		AddAnimation("UNCC_BUDDY01_shoot_right", player);
		AddAnimation("UNCC_BUDDY01_GetOff", player);
		AddAnimation("UNCC_BUDDY02_idle", player);
		AddAnimation("UNCC_BUDDY02_GetOn", player);
		AddAnimation("UNCC_BUDDY02_GetOff", player);
		AddAnimation("UNCC_BUDDY02_left", player);
		AddAnimation("UNCC_BUDDY02_right", player);
		AddAnimation("UNCC_BUDDY02_shoot_left", player);
		AddAnimation("UNCC_BUDDY02_shoot_right", player);
		foreach (WeaponType item2 in hashSet)
		{
			switch (item2)
			{
			case WeaponType.RailGun:
			case WeaponType.MachineGun:
				AddAnimation(AnimationString.Stand + "_" + Weapon.GetAnimationName(item2) + "_" + AnimationString.Idle, player);
				AddAnimation(AnimationString.Stand + "_" + Weapon.GetAnimationName(item2) + "_" + AnimationString.Run, player);
				AddAnimation(AnimationString.Stand + "_" + Weapon.GetAnimationName(item2) + "_" + AnimationString.Attack, player);
				AddAnimation(AnimationString.Stand + "_" + Weapon.GetAnimationName(item2) + "_" + AnimationString.RunAttack + "_down", player);
				AddAnimation(AnimationString.Stand + "_" + Weapon.GetAnimationName(item2) + "_" + AnimationString.RunAttack + "_up", player);
				AddAnimation(AnimationString.Stand + "_" + Weapon.GetAnimationName(item2) + "_" + AnimationString.RunAttack + "_idle", player);
				AddAnimation(AnimationString.Fly + "_" + Weapon.GetAnimationName(item2) + "_" + AnimationString.Idle, player);
				AddAnimation(AnimationString.Fly + "_" + Weapon.GetAnimationName(item2) + "_" + AnimationString.Run, player);
				AddAnimation(AnimationString.Fly + "_" + Weapon.GetAnimationName(item2) + "_" + AnimationString.Attack, player);
				AddAnimation(AnimationString.Fly + "_" + Weapon.GetAnimationName(item2) + "_" + AnimationString.RunAttack, player);
				AddAnimation(AnimationString.Fly + "_" + Weapon.GetAnimationName(item2) + "_" + AnimationString.RunAttack + "_idle", player);
				break;
			case WeaponType.BowSword:
				AddWeaponAnimation("LaserBow", player);
				AddWeaponAnimation("LaserSword", player);
				AddAnimation(AnimationString.Stand + "_LaserSword_" + AnimationString.Attack + "01", player);
				AddAnimation(AnimationString.Stand + "_LaserSword_" + AnimationString.RunAttack + "01", player);
				AddAnimation(AnimationString.Fly + "_LaserSword_" + AnimationString.Attack + "01", player);
				AddAnimation(AnimationString.Fly + "_LaserSword_" + AnimationString.RunAttack + "01", player);
				break;
			case WeaponType.Sword:
				AddWeaponAnimation("LaserSword", player);
				AddAnimation(AnimationString.Stand + "_LaserSword_" + AnimationString.Attack + "01", player);
				AddAnimation(AnimationString.Stand + "_LaserSword_" + AnimationString.RunAttack + "01", player);
				AddAnimation(AnimationString.Fly + "_LaserSword_" + AnimationString.Attack + "01", player);
				AddAnimation(AnimationString.Fly + "_LaserSword_" + AnimationString.RunAttack + "01", player);
				break;
			default:
				AddWeaponAnimation(Weapon.GetAnimationName(item2), player);
				break;
			}
		}
	}

	private void AddWeaponAnimation(string weaponAnimName, GameObject player)
	{
		AddAnimation(AnimationString.Stand + "_" + weaponAnimName + "_" + AnimationString.Idle, player);
		AddAnimation(AnimationString.Stand + "_" + weaponAnimName + "_" + AnimationString.Run, player);
		AddAnimation(AnimationString.Stand + "_" + weaponAnimName + "_" + AnimationString.Attack, player);
		AddAnimation(AnimationString.Stand + "_" + weaponAnimName + "_" + AnimationString.RunAttack, player);
		AddAnimation(AnimationString.Fly + "_" + weaponAnimName + "_" + AnimationString.Idle, player);
		AddAnimation(AnimationString.Fly + "_" + weaponAnimName + "_" + AnimationString.Run, player);
		AddAnimation(AnimationString.Fly + "_" + weaponAnimName + "_" + AnimationString.Attack, player);
		AddAnimation(AnimationString.Fly + "_" + weaponAnimName + "_" + AnimationString.RunAttack, player);
	}

	private void AddAnimationsForUI(GameObject player, bool needShopAnimation, WeaponType type)
	{
		lastUserState = null;
		switch (type)
		{
		case WeaponType.AssaultRifle:
		case WeaponType.LaserRifle:
			AddAnimation("shop_assault_idle", player);
			break;
		case WeaponType.RPG:
		case WeaponType.TwinRPG:
		case WeaponType.TrackingRPG:
		case WeaponType.SplitRPG:
			AddAnimation("shop_RPG_idle", player);
			break;
		case WeaponType.ShotGun:
		case WeaponType.LaserShotGun:
			AddAnimation("shop_shotgun_idle", player);
			break;
		case WeaponType.DoublePistol:
		case WeaponType.ChargeDoublePistol:
			AddAnimation("shop_DoubleP_idle", player);
			break;
		case WeaponType.RailGun:
			AddAnimation("shop_RailGun_idle", player);
			break;
		case WeaponType.MachineGun:
			AddAnimation("shop_jiatelin_idle", player);
			break;
		case WeaponType.BowSword:
			AddAnimation("shop_LaserBow_idle", player);
			break;
		case WeaponType.Sniper:
		case WeaponType.UltraSniper:
			AddAnimation("shop_Sniper_idle", player);
			break;
		case WeaponType.Fist:
		case WeaponType.QuadrupleFist:
		case WeaponType.Myth370Fist:
			AddAnimation("shop_fist_idle", player);
			break;
		case WeaponType.Sword:
			AddAnimation("shop_LaserSword_idle", player);
			break;
		}
		if (needShopAnimation)
		{
			AddAnimation("shop_assault_show1", player);
			AddAnimation("shop_assault_show2", player);
			AddAnimation("shop_RPG_show1", player);
			AddAnimation("shop_RPG_show2", player);
			AddAnimation("shop_shotgun_show1", player);
			AddAnimation("shop_shotgun_show2", player);
			AddAnimation("shop_DoubleP_show1", player);
			AddAnimation("shop_DoubleP_show2", player);
			AddAnimation("shop_RailGun_show1", player);
			AddAnimation("shop_RailGun_show2", player);
			AddAnimation("shop_jiatelin_show1", player);
			AddAnimation("shop_jiatelin_show2", player);
			AddAnimation("shop_LaserBow_attack", player);
			AddAnimation("shop_LaserBow_show1", player);
			AddAnimation("shop_LaserBow_show2", player);
			AddAnimation("shop_LaserSword_attack", player);
			AddAnimation("shop_LaserSword_idle", player);
			AddAnimation("shop_Sniper_show1", player);
			AddAnimation("shop_Sniper_show2", player);
			AddAnimation("shop_fist_show1", player);
			AddAnimation("shop_fist_show2", player);
		}
	}

	private void AddAnimationsForMainMenu(GameObject player)
	{
		lastUserState = null;
		AddAnimation("Start_assault_idle", player);
		AddAnimation("Start_RPG_idle", player);
		AddAnimation("Start_shotgun_idle", player);
		AddAnimation("Start_DoubleP_idle", player);
		AddAnimation("Start_RailGun_idle", player);
		AddAnimation("Start_LaserBow_idle", player);
		AddAnimation("Start_Sniper_idle", player);
		AddAnimation("Start_fist_idle", player);
	}

	private GameObject ReBuildAvatar(UserState userState, Player p)
	{
		short[] array = new short[5] { 1, 1, 1, 1, 1 };
		if (userState != null)
		{
			array = userState.GetAvatar();
		}
		lastUserState = userState;
		string text = Global.QUALITY_LEVEL_1;
		if (GameApp.GetInstance().GetGameMode().IsVSMode() && p.GameUnitType == EGameUnitType.REMOTE_PLAYER && GameApp.GetInstance().GetUserState().mGraphicQuality.mCapbility <= DeviceCaps.Capbility.Middle)
		{
			text = "L3";
		}
		else if (GameApp.GetInstance().GetUserState().mGraphicQuality.mCapbility == DeviceCaps.Capbility.Low)
		{
			text = "L3";
		}
		Object original = Resources.Load(Global.AVATAR_PATH + "/" + Global.BONE_NAME);
		Object original2 = Resources.Load(Global.AVATAR_PATH + "/" + text + "/" + string.Format("{0:D2}", array[0]) + "/" + Global.HEAD_NAME);
		Object original3 = Resources.Load(Global.AVATAR_PATH + "/" + text + "/" + string.Format("{0:D2}", array[1]) + "/" + Global.BODY_NAME);
		Object original4 = Resources.Load(Global.AVATAR_PATH + "/" + text + "/" + string.Format("{0:D2}", array[2]) + "/" + Global.HAND_NAME);
		Object original5 = Resources.Load(Global.AVATAR_PATH + "/" + text + "/" + string.Format("{0:D2}", array[3]) + "/" + Global.FOOT_NAME);
		Object original6 = Resources.Load(Global.BAG_PATH + "/" + text + "/" + Global.BAG_PREFIX + string.Format("{0:D2}", array[4]));
		GameObject gameObject = Object.Instantiate(original) as GameObject;
		GameObject gameObject2 = Object.Instantiate(original3) as GameObject;
		GameObject gameObject3 = Object.Instantiate(original2) as GameObject;
		GameObject gameObject4 = Object.Instantiate(original4) as GameObject;
		GameObject gameObject5 = Object.Instantiate(original5) as GameObject;
		GameObject gameObject6 = Object.Instantiate(original6) as GameObject;
		if (array[4] == 3)
		{
			FlyBagAnimationScript componentInChildren = gameObject6.GetComponentInChildren<FlyBagAnimationScript>();
		}
		List<Transform> list = new List<Transform>();
		Transform transform = gameObject.transform;
		TraverseBones(transform.GetChild(0), list);
		for (int i = 0; i < list.Count; i++)
		{
		}
		SkinnedMeshRenderer component = gameObject2.transform.Find("body").GetComponent<SkinnedMeshRenderer>();
		SkinnedMeshRenderer component2 = gameObject3.transform.Find("head").GetComponent<SkinnedMeshRenderer>();
		SkinnedMeshRenderer component3 = gameObject4.transform.Find("hand").GetComponent<SkinnedMeshRenderer>();
		SkinnedMeshRenderer component4 = gameObject5.transform.Find("foot").GetComponent<SkinnedMeshRenderer>();
		BindBones(component, list, gameObject);
		BindBones(component2, list, gameObject);
		BindBones(component3, list, gameObject);
		BindBones(component4, list, gameObject);
		gameObject6.transform.position = gameObject.transform.Find(BoneName.Bag).position;
		if (array[1] == 5)
		{
			gameObject6.transform.position = gameObject.transform.Find(BoneName.Bag).position + Vector3.forward * -0.05f + Vector3.up * 0.03f;
		}
		gameObject6.transform.parent = gameObject.transform.Find(BoneName.Bag);
		gameObject6.name = "Bag";
		Object.Destroy(gameObject2);
		Object.Destroy(gameObject3);
		Object.Destroy(gameObject4);
		Object.Destroy(gameObject5);
		component.gameObject.layer = PhysicsLayer.PLAYER;
		component2.gameObject.layer = PhysicsLayer.PLAYER;
		component3.gameObject.layer = PhysicsLayer.PLAYER;
		component4.gameObject.layer = PhysicsLayer.PLAYER;
		gameObject6.layer = PhysicsLayer.PLAYER;
		Renderer[] componentsInChildren = gameObject6.GetComponentsInChildren<Renderer>();
		if (componentsInChildren != null)
		{
			Renderer[] array2 = componentsInChildren;
			foreach (Renderer renderer in array2)
			{
				renderer.gameObject.layer = PhysicsLayer.PLAYER;
			}
		}
		gameObject.transform.position = new Vector3(4f, 0f, -5f);
		AddAnimations(gameObject, p);
		p.SetBagObj(gameObject6);
		BuildSpecialEffect(array, gameObject);
		return gameObject;
	}

	public GameObject CreateUIAvatar(short[] avatar, bool needShopAnimation = true)
	{
		Object original = Resources.Load(Global.AVATAR_PATH + "/" + Global.BONE_NAME);
		Object original2 = Resources.Load(Global.AVATAR_PATH + "/" + Global.QUALITY_LEVEL_1 + "/" + string.Format("{0:D2}", avatar[0]) + "/" + Global.HEAD_NAME);
		Object original3 = Resources.Load(Global.AVATAR_PATH + "/" + Global.QUALITY_LEVEL_1 + "/" + string.Format("{0:D2}", avatar[1]) + "/" + Global.BODY_NAME);
		Object original4 = Resources.Load(Global.AVATAR_PATH + "/" + Global.QUALITY_LEVEL_1 + "/" + string.Format("{0:D2}", avatar[2]) + "/" + Global.HAND_NAME);
		Object original5 = Resources.Load(Global.AVATAR_PATH + "/" + Global.QUALITY_LEVEL_1 + "/" + string.Format("{0:D2}", avatar[3]) + "/" + Global.FOOT_NAME);
		Object original6 = Resources.Load(Global.BAG_PATH + "/" + Global.QUALITY_LEVEL_1 + "/" + Global.BAG_PREFIX + string.Format("{0:D2}", avatar[4]));
		GameObject gameObject = Object.Instantiate(original) as GameObject;
		GameObject gameObject2 = Object.Instantiate(original3) as GameObject;
		GameObject gameObject3 = Object.Instantiate(original2) as GameObject;
		GameObject gameObject4 = Object.Instantiate(original4) as GameObject;
		GameObject gameObject5 = Object.Instantiate(original5) as GameObject;
		GameObject gameObject6 = Object.Instantiate(original6) as GameObject;
		if (avatar[4] == 15)
		{
			FlyBagAnimationScript componentInChildren = gameObject6.GetComponentInChildren<FlyBagAnimationScript>();
		}
		List<Transform> list = new List<Transform>();
		Transform transform = gameObject.transform;
		TraverseBones(transform.GetChild(0), list);
		for (int i = 0; i < list.Count; i++)
		{
		}
		SkinnedMeshRenderer component = gameObject2.transform.Find("body").GetComponent<SkinnedMeshRenderer>();
		SkinnedMeshRenderer component2 = gameObject3.transform.Find("head").GetComponent<SkinnedMeshRenderer>();
		SkinnedMeshRenderer component3 = gameObject4.transform.Find("hand").GetComponent<SkinnedMeshRenderer>();
		SkinnedMeshRenderer component4 = gameObject5.transform.Find("foot").GetComponent<SkinnedMeshRenderer>();
		BindBones(component, list, gameObject);
		BindBones(component2, list, gameObject);
		BindBones(component3, list, gameObject);
		BindBones(component4, list, gameObject);
		gameObject6.transform.position = gameObject.transform.Find(BoneName.Bag).position;
		if (avatar[1] == 5)
		{
			gameObject6.transform.position = gameObject.transform.Find(BoneName.Bag).position + Vector3.forward * -0.05f + Vector3.up * 0.03f;
		}
		gameObject6.transform.parent = gameObject.transform.Find(BoneName.Bag);
		gameObject6.name = "Bag";
		Object.Destroy(gameObject2);
		Object.Destroy(gameObject3);
		Object.Destroy(gameObject4);
		Object.Destroy(gameObject5);
		Weapon weapon = GameApp.GetInstance().GetUserState().GetWeapons()[avatar[5]];
		AddAnimationsForUI(gameObject, needShopAnimation, weapon.GetWeaponType());
		BuildSpecialEffect(avatar, gameObject);
		return gameObject;
	}

	public GameObject CreateMainMenuAvatar(short[] avatar)
	{
		Object original = Resources.Load(Global.AVATAR_PATH + "/" + Global.BONE_NAME);
		Object original2 = Resources.Load(Global.AVATAR_PATH + "/" + Global.QUALITY_LEVEL_1 + "/" + string.Format("{0:D2}", avatar[0]) + "/" + Global.HEAD_NAME);
		Object original3 = Resources.Load(Global.AVATAR_PATH + "/" + Global.QUALITY_LEVEL_1 + "/" + string.Format("{0:D2}", avatar[1]) + "/" + Global.BODY_NAME);
		Object original4 = Resources.Load(Global.AVATAR_PATH + "/" + Global.QUALITY_LEVEL_1 + "/" + string.Format("{0:D2}", avatar[2]) + "/" + Global.HAND_NAME);
		Object original5 = Resources.Load(Global.AVATAR_PATH + "/" + Global.QUALITY_LEVEL_1 + "/" + string.Format("{0:D2}", avatar[3]) + "/" + Global.FOOT_NAME);
		Object original6 = Resources.Load(Global.BAG_PATH + "/" + Global.QUALITY_LEVEL_1 + "/" + Global.BAG_PREFIX + string.Format("{0:D2}", avatar[4]));
		GameObject gameObject = Object.Instantiate(original) as GameObject;
		GameObject gameObject2 = Object.Instantiate(original3) as GameObject;
		GameObject gameObject3 = Object.Instantiate(original2) as GameObject;
		GameObject gameObject4 = Object.Instantiate(original4) as GameObject;
		GameObject gameObject5 = Object.Instantiate(original5) as GameObject;
		GameObject gameObject6 = Object.Instantiate(original6) as GameObject;
		if (avatar[4] == 15)
		{
			FlyBagAnimationScript componentInChildren = gameObject6.GetComponentInChildren<FlyBagAnimationScript>();
		}
		List<Transform> list = new List<Transform>();
		Transform transform = gameObject.transform;
		TraverseBones(transform.GetChild(0), list);
		for (int i = 0; i < list.Count; i++)
		{
		}
		SkinnedMeshRenderer component = gameObject2.transform.Find("body").GetComponent<SkinnedMeshRenderer>();
		SkinnedMeshRenderer component2 = gameObject3.transform.Find("head").GetComponent<SkinnedMeshRenderer>();
		SkinnedMeshRenderer component3 = gameObject4.transform.Find("hand").GetComponent<SkinnedMeshRenderer>();
		SkinnedMeshRenderer component4 = gameObject5.transform.Find("foot").GetComponent<SkinnedMeshRenderer>();
		BindBones(component, list, gameObject);
		BindBones(component2, list, gameObject);
		BindBones(component3, list, gameObject);
		BindBones(component4, list, gameObject);
		gameObject6.transform.position = gameObject.transform.Find(BoneName.Bag).position;
		if (avatar[1] == 5)
		{
			gameObject6.transform.position = gameObject.transform.Find(BoneName.Bag).position + Vector3.forward * -0.05f + Vector3.up * 0.03f;
		}
		gameObject6.transform.parent = gameObject.transform.Find(BoneName.Bag);
		gameObject6.name = "Bag";
		Object.Destroy(gameObject2);
		Object.Destroy(gameObject3);
		Object.Destroy(gameObject4);
		Object.Destroy(gameObject5);
		AddAnimationsForMainMenu(gameObject);
		BuildSpecialEffect(avatar, gameObject);
		return gameObject;
	}

	public GameObject ReBuildPlayerAvatar(UserState userState, Player p)
	{
		GameObject gameObject = ReBuildAvatar(userState, p);
		gameObject.AddComponent<CharacterController>().center = new Vector3(0f, 1f, 0f);
		if (!p.IsRobot() && (p.IsLocal() || GameApp.GetInstance().GetGameMode().IsCoopMode()))
		{
			p.SetNavMeshObstacle(gameObject.AddComponent<UnityEngine.AI.NavMeshObstacle>());
			p.GetNavMeshObstacle().height = 0.5f;
		}
		GameObject original = Resources.Load("_Old/_Effect/Shadow") as GameObject;
		GameObject gameObject2 = Object.Instantiate(original) as GameObject;
		gameObject2.transform.position = gameObject.transform.position;
		gameObject2.transform.localScale = Vector3.one * 0.15f;
		gameObject2.transform.parent = gameObject.transform;
		gameObject2.name = "Shadow";
		return gameObject;
	}

	public static void SetModelLayer(GameObject go, int layer)
	{
		go.layer = layer;
		for (int i = 0; i < go.transform.childCount; i++)
		{
			SetModelLayer(go.transform.GetChild(i).gameObject, layer);
		}
	}

	private void BuildSpecialEffect(short[] avatar, GameObject player)
	{
		if (avatar[1] == 12)
		{
			Object original = Resources.Load("SW3_Effect/avatar12_L");
			GameObject gameObject = Object.Instantiate(original) as GameObject;
			Object original2 = Resources.Load("SW3_Effect/avatar12_R");
			GameObject gameObject2 = Object.Instantiate(original2) as GameObject;
			gameObject.transform.parent = player.transform.Find(BoneName.UpperBodyChildL);
			gameObject2.transform.parent = player.transform.Find(BoneName.UpperBodyChildR);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject2.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject2.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			gameObject2.transform.localScale = Vector3.one;
		}
	}
}
