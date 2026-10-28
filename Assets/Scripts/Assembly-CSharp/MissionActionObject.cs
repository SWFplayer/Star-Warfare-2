using UnityEngine;

public class MissionActionObject : MissionAction
{
	private bool mIsBreakable;

	private string mPrefabPath;

	private GameObject mGameObject;

	private int mMaxHp;

	private int mTransformId;

	private Vector3 mSpawnPosition;

	private Quaternion mSpawnRotation;

	private int mTwinID;

	private bool mIsTwin;

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
				mIsBreakable = dataTable.GetBoolean(i, 3);
				mMaxHp = dataTable.GetInt(i, 4);
				mIsTwin = dataTable.GetBoolean(i, 5);
				mTwinID = dataTable.GetInt(i, 6);
				break;
			}
		}
	}

	public override void LevelStart()
	{
		string text = MissionNamePrefix.ACTION_OBJECT + mTransformId;
		bool flag = false;
		GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.MISSION_ACTION_OBJECT);
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (text == gameObject.name)
			{
				flag = true;
				mSpawnPosition = gameObject.transform.position;
				mSpawnRotation = gameObject.transform.rotation;
				break;
			}
		}
		if (!flag)
		{
			Debug.LogError("[NO TRANSFORM FOUND MISSION ACTION OBJECT] ActionID = " + base.ID + ", mTransformId = " + mTransformId);
		}
	}

	public override void StartAction()
	{
		Debug.Log(Time.time + " : " + mPrefabPath);
		GameObject gameObject = Resources.Load(mPrefabPath) as GameObject;
		if (gameObject != null)
		{
			mGameObject = Object.Instantiate(gameObject, mSpawnPosition, mSpawnRotation) as GameObject;
			if (mIsBreakable)
			{
				DestructibleObjectScript component = mGameObject.GetComponent<DestructibleObjectScript>();
				if (null != component)
				{
					component.MaxHp = mMaxHp;
				}
				int destructibleCount = GameApp.GetInstance().GetGameWorld().GetDestructibleCount();
				mGameObject.name += destructibleCount;
				GameApp.GetInstance().GetGameWorld().AddDestructible(mGameObject);
				if (mIsTwin)
				{
					component.mTwinID = mTwinID;
				}
			}
			else if (mIsTwin)
			{
				mGameObject.GetComponent<TwinBlockScript>().ID = mTwinID;
				mGameObject.GetComponent<TwinBlockScript>().Init();
			}
		}
		else
		{
			Debug.Log("Can't find " + mPrefabPath);
		}
	}
}
