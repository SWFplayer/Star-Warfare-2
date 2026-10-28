using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderManager
{
	private static ShaderManager instance;

	public static ShaderManager GetInstance()
	{
		if (instance == null)
		{
			instance = new ShaderManager();
		}
		return instance;
	}

	public void SwitchShader(GameObject Avatar)
	{
		List<Material> list = new List<Material>();
		list.Add(Avatar.transform.Find("body").GetComponent<Renderer>().material);
		list.Add(Avatar.transform.Find("head").GetComponent<Renderer>().material);
		list.Add(Avatar.transform.Find("hand").GetComponent<Renderer>().material);
		list.Add(Avatar.transform.Find("foot").GetComponent<Renderer>().material);
		foreach (Material item in list)
		{
			if (item.shader.name == "StarWarfare/SNDM_new")
			{
				item.shader = Shader.Find("StarWarfare/SNDM_new_outline");
				item.SetColor("_OutlineColor", Color.blue);
				item.SetFloat("_Outline", 0.04f);
			}
			else if (item.shader.name == "StarWarfare/SNDM_new_outline")
			{
				item.shader = Shader.Find("StarWarfare/SNDM_new");
			}
		}
	}

	public void SwitchEnemyShader(GameObject Avatar)
	{
		Material material = Avatar.transform.Find("objpolySurface57").GetComponent<Renderer>().material;
		if (material.shader.name == "StarWarfare/Enemy/Bumped_Specular")
		{
			material.shader = Shader.Find("StarWarfare/Enemy/Bumped_Specular_Outline");
			material.SetColor("_OutlineColor", Color.red);
			material.SetFloat("_Outline", 0.04f);
		}
		else if (material.shader.name == "StarWarfare/Enemy/Bumped_Specular_Outline")
		{
			material.shader = Shader.Find("StarWarfare/Enemy/Bumped_Specular");
		}
	}

	public void EnableOutline(GameObject player, Hashtable enemyList, List<Robot> robotList)
	{
		Material material;
		foreach (Enemy value in enemyList.Values)
		{
			Debug.Log(value.GetObject().name);
			Debug.Log(value.GetObject().transform.Find("objpolySurface57") == null);
			try
			{
				material = value.GetObject().transform.Find("objpolySurface57").GetComponent<Renderer>().material;
				Debug.Log(material.shader.name);
				if (material.shader.name == "StarWarfare/Enemy/Bumped_Specular")
				{
					Debug.Log("change");
					material.shader = Shader.Find("StarWarfare/Enemy/Bumped_Specular_Outline");
					Debug.Log(material.shader.name);
					material.SetColor("_OutlineColor", Color.red);
					material.SetFloat("_Outline", 0.04f);
				}
			}
			catch
			{
			}
		}
		material = player.transform.Find("body").GetComponent<Renderer>().material;
		if (material.shader.name == "StarWarfare/SNDM_new")
		{
			GetInstance().SwitchShader(player);
		}
		foreach (Robot robot in robotList)
		{
			material = robot.GetObject().transform.Find("body").GetComponent<Renderer>().material;
			if (material.shader.name == "StarWarfare/SNDM_new")
			{
				GetInstance().SwitchShader(robot.GetObject());
			}
		}
	}

	public void CloseOutline(GameObject player, Hashtable enemyList, List<Robot> robotList)
	{
		Material material;
		foreach (Enemy value in enemyList.Values)
		{
			try
			{
				material = value.GetObject().transform.Find("objpolySurface57").GetComponent<Renderer>().material;
				if (material.shader.name == "StarWarfare/Enemy/Bumped_Specular_Outline")
				{
					material.shader = Shader.Find("StarWarfare/Enemy/Bumped_Specular");
				}
			}
			catch
			{
			}
		}
		material = player.transform.Find("body").GetComponent<Renderer>().material;
		if (material.shader.name == "StarWarfare/SNDM_new_outline")
		{
			GetInstance().SwitchShader(player);
		}
		foreach (Robot robot in robotList)
		{
			material = robot.GetObject().transform.Find("body").GetComponent<Renderer>().material;
			if (material.shader.name == "StarWarfare/SNDM_new_outline")
			{
				GetInstance().SwitchShader(robot.GetObject());
			}
		}
	}
}
