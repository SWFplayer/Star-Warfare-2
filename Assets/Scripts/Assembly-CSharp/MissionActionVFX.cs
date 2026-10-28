using UnityEngine;

public class MissionActionVFX : MissionAction
{
	private string mPrefabPath;

	private GameObject mGameObject;

	private int mTransformId;

	private Vector3 mSpawnPosition;

	private Quaternion mSpawnRotation;

	public override void LoadConfig()
	{
		GameDataTable dataTable = GetDataTable();
		for (int i = 0; i < dataTable.Count; i++)
		{
			int num = dataTable.GetInt(i, 0);
			if (num == base.ID)
			{
				mTransformId = dataTable.GetInt(i, 1);
				mPrefabPath = dataTable.GetString(i, 2);
				break;
			}
		}
	}

	public override void LevelStart()
	{
		string text = MissionNamePrefix.ACTION_VFX + mTransformId;
		bool flag = false;
		GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.MISSION_ACTION_VFX);
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (gameObject != null && text == gameObject.name)
			{
				flag = true;
				mSpawnPosition = gameObject.transform.position;
				mSpawnRotation = gameObject.transform.rotation;
				GameObject original = Resources.Load(mPrefabPath) as GameObject;
				mGameObject = Object.Instantiate(original, mSpawnPosition, mSpawnRotation) as GameObject;
				mGameObject.SetActive(false);
				break;
			}
		}
		if (!flag)
		{
			Debug.LogError("[NO TRANSFORM FOUND MISSION ACTION VFX] ActionID = " + base.ID + ", mTransformId = " + mTransformId);
		}
	}

	public override void StartAction()
	{
		Debug.Log(Time.time + " : " + mPrefabPath);
		mGameObject.SetActive(true);
	}
}
