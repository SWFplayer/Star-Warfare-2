using System.Collections.Generic;
using UnityEngine;

public class MissionActionDestroy : MissionAction
{
	private int mDestroyID;

	private string mDestroyVFX = string.Empty;

	private GameObject VFX;

	private List<GameObject> mDestroyObject = new List<GameObject>();

	public override void LoadConfig()
	{
		GameDataTable dataTable = GetDataTable();
		for (int i = 0; i < dataTable.Count; i++)
		{
			int num = dataTable.GetInt(i, 0);
			if (num == base.ID)
			{
				mDestroyID = dataTable.GetInt(i, 1);
				mDestroyVFX = dataTable.GetString(i, 2);
				break;
			}
		}
	}

	public override void LevelStart()
	{
		if (mDestroyVFX != string.Empty)
		{
			Object original = Resources.Load(mDestroyVFX);
			VFX = Object.Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject;
			VFX.SetActive(false);
		}
	}

	public override void StartAction()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.MISSION_DESTRUCTION);
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (!(gameObject != null))
			{
				continue;
			}
			MissionIDScript component = gameObject.GetComponent<MissionIDScript>();
			if (component.id == mDestroyID)
			{
				Debug.Log("Destroy " + gameObject.name);
				Object.Destroy(gameObject);
				if (VFX != null)
				{
					VFX.transform.position = gameObject.transform.position;
					VFX.SetActive(true);
				}
			}
		}
	}
}
