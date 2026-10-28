using System.Collections.Generic;
using UnityEngine;

public class UIUtil
{
	public int armorIndex;

	public int subArmorIndex;

	public int weaponIndex;

	public bool loadNoBreak;

	public UIUtil()
	{
		armorIndex = 0;
		subArmorIndex = 0;
		weaponIndex = 0;
		loadNoBreak = false;
	}

	public bool LoadAllAvatar(int type)
	{
		List<List<Armor>> armor = GameApp.GetInstance().GetUserState().GetArmor();
		Shader shader = Shader.Find("iPhone/SolidTexture_Color");
		Shader shader2 = Shader.Find("iPhone/AlphaBlend_Color_TwoSides");
		Shader shader3 = Shader.Find("iPhone/SolidAndAlphaTexture");
		Shader shader4 = Shader.Find("iPhone/Additive");
		Object original = Resources.Load("_Old/_UI/lock");
		Object original2 = Resources.Load("_Old/_UI/unlock");
		Object original3 = Resources.Load("_Old/_UI/unlock1");
		if (type < 5)
		{
			List<Armor> list = armor[type];
			for (int i = 0; i < list.Count; i++)
			{
				if (!(list[i].Equipment != null))
				{
					list[i].CreateArmor();
					GameObject gameObject = list[i].Equipment;
					Transform transform = gameObject.transform.Find(UIConstant.SUB_AVATAR[type]);
					GameObject gameObject2 = Object.Instantiate(original) as GameObject;
					gameObject2.transform.parent = gameObject.transform;
					GameObject gameObject3 = Object.Instantiate(original2) as GameObject;
					gameObject3.transform.parent = gameObject.transform;
					GameObject gameObject4 = Object.Instantiate(original3) as GameObject;
					gameObject4.transform.parent = gameObject.transform;
					if (transform != null)
					{
						gameObject = transform.gameObject;
					}
					if (type == Global.AVATAR_PART_NUM - 1 || i >= 4)
					{
					}
					BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
					list[i].Center = boxCollider.center;
					list[i].Equipment.SetActiveRecursively(false);
				}
			}
		}
		else
		{
			Dictionary<int, Weapon> weapons = GameApp.GetInstance().GetUserState().GetWeapons();
			for (int j = weaponIndex; j < weapons.Count; j++)
			{
				Weapon weapon = weapons[j];
				if (!(weapon.Gun != null))
				{
					weapon.CreateGunForUI();
					GameObject gameObject5 = weapon.Gun;
					switch (j)
					{
					case 23:
						gameObject5 = weapon.Gun.transform.GetChild(0).gameObject;
						break;
					case 24:
					case 25:
						gameObject5 = weapon.Gun.transform.GetChild(1).gameObject;
						break;
					case 22:
						gameObject5 = weapon.Gun.transform.GetChild(1).gameObject;
						break;
					}
					GameObject gameObject6 = Object.Instantiate(original) as GameObject;
					gameObject6.transform.parent = weapon.Gun.transform;
					GameObject gameObject7 = Object.Instantiate(original2) as GameObject;
					gameObject7.transform.parent = weapon.Gun.transform;
					GameObject gameObject8 = Object.Instantiate(original3) as GameObject;
					gameObject8.transform.parent = weapon.Gun.transform;
					BoxCollider boxCollider2 = gameObject5.AddComponent<BoxCollider>();
					weapon.Center = boxCollider2.center;
					weapon.Gun.SetActiveRecursively(false);
				}
			}
		}
		return true;
	}

	public void SetShader(GameObject obj, Shader shader)
	{
		Material[] materials = obj.GetComponent<Renderer>().materials;
		foreach (Material material in materials)
		{
			Texture texture = material.mainTexture;
			if (texture == null)
			{
				texture = material.GetTexture("_MainTex");
			}
			material.shader = shader;
			material.SetTexture("_MainTex", texture);
		}
	}

	public void FreeAllAvatar()
	{
		List<List<Armor>> armor = GameApp.GetInstance().GetUserState().GetArmor();
		for (int i = 0; i < armor.Count; i++)
		{
			List<Armor> list = armor[i];
			for (int j = 0; j < list.Count; j++)
			{
				if (!(list[j].Equipment == null))
				{
					Object.Destroy(list[j].Equipment);
					list[j].Equipment = null;
				}
			}
		}
		Dictionary<int, Weapon> weapons = GameApp.GetInstance().GetUserState().GetWeapons();
		for (int k = 0; k < weapons.Count; k++)
		{
			if (!(weapons[k].Gun == null))
			{
				Object.Destroy(weapons[k].Gun);
				weapons[k].Gun = null;
			}
		}
	}
}
