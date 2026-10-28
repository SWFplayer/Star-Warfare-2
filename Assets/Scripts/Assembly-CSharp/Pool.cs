using System.Collections.Generic;
using UnityEngine;

public class Pool
{
	private static Pool sInstance;

	private Dictionary<string, List<GameObject>> mPool;

	private Dictionary<string, Vector3> mPoolLocalScale = new Dictionary<string, Vector3>();

	private Dictionary<GameObject, bool> mPoolStats;

	private GameObject mFolder;

	private List<GameObject> mTempList = new List<GameObject>();

	public Pool()
	{
		sInstance = this;
		mPool = new Dictionary<string, List<GameObject>>();
		mPoolStats = new Dictionary<GameObject, bool>();
		mFolder = new GameObject("Pool");
		mFolder.transform.position = new Vector3(0f, -99999f, 0f);
	}

	public static Pool sGetInstance()
	{
		return sInstance;
	}

	private void SetInUse(GameObject go, bool isUsing)
	{
		if (mPoolStats.ContainsKey(go))
		{
			mPoolStats[go] = isUsing;
		}
		else
		{
			mPoolStats.Add(go, isUsing);
		}
	}

	public static GameObject sAllocate(string prefab)
	{
		GameObject original = Resources.Load(prefab) as GameObject;
		return Object.Instantiate(original) as GameObject;
	}

	public static GameObject sAllocate(string prefab, Vector3 position)
	{
		GameObject gameObject = sAllocate(prefab);
		if (gameObject != null)
		{
			gameObject.transform.position = position;
		}
		return gameObject;
	}

	public static GameObject sAllocate(string prefab, Vector3 position, Vector3 forward)
	{
		GameObject gameObject = sAllocate(prefab, position);
		gameObject.transform.rotation = Quaternion.LookRotation(forward);
		return gameObject;
	}

	public static GameObject sAllocate(string prefab, Vector3 position, float life)
	{
		GameObject gameObject = sAllocate(prefab);
		if (gameObject != null)
		{
			gameObject.transform.position = position;
			GameWorld.GetComponent<AutoDestroyScript>(gameObject).life = life;
		}
		return gameObject;
	}

	public GameObject Allocate(string prefab, Vector3 position, Vector3 rotation)
	{
		GameObject gameObject = Allocate(prefab);
		gameObject.transform.position = position;
		gameObject.transform.rotation = Quaternion.LookRotation(rotation);
		return gameObject;
	}

	public GameObject Allocate(string prefab, Vector3 position)
	{
		GameObject gameObject = Allocate(prefab);
		gameObject.transform.position = position;
		return gameObject;
	}

	public GameObject Allocate(string prefab)
	{
		List<GameObject> list = null;
		if (mPool.ContainsKey(prefab))
		{
			list = mPool[prefab];
			foreach (GameObject item in list)
			{
				if (!mPoolStats[item])
				{
					item.transform.localScale = mPoolLocalScale[prefab];
					item.SetActive(true);
					SetInUse(item, true);
					return item;
				}
			}
		}
		else
		{
			list = new List<GameObject>();
			mPool.Add(prefab, list);
		}
		GameObject gameObject = null;
		if (list.Count > 0)
		{
			gameObject = Object.Instantiate(list[0]) as GameObject;
			gameObject.transform.localScale = mPoolLocalScale[prefab];
		}
		else
		{
			GameObject gameObject2 = Resources.Load(prefab) as GameObject;
			mPoolLocalScale[prefab] = gameObject2.transform.localScale;
			if (gameObject2 == null)
			{
				Debug.LogError("Could not find " + prefab);
			}
			gameObject = Object.Instantiate(gameObject2) as GameObject;
		}
		gameObject.SetActive(true);
		gameObject.transform.parent = mFolder.transform;
		list.Add(gameObject);
		SetInUse(gameObject, true);
		return gameObject;
	}

	public void PreAllocate(string prefab, int count = 1)
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < count; i++)
		{
			GameObject item = Allocate(prefab);
			list.Add(item);
		}
		foreach (GameObject item2 in list)
		{
			ParticleSystem[] componentsInChildren = item2.GetComponentsInChildren<ParticleSystem>(true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				if (componentsInChildren[j] != null)
				{
					componentsInChildren[j].Clear();
				}
			}
			item2.SetActive(false);
			SetInUse(item2, false);
		}
	}

	public void VFXPreAllocate(string prefab, int count = 1)
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < count; i++)
		{
			GameObject gameObject = Allocate(prefab);
			gameObject.transform.localPosition = Vector3.zero;
			list.Add(gameObject);
		}
		foreach (GameObject item in list)
		{
			ParticleSystem[] componentsInChildren = item.GetComponentsInChildren<ParticleSystem>(true);
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				if (componentsInChildren[j] != null)
				{
					componentsInChildren[j].Clear();
				}
			}
			Renderer[] componentsInChildren2 = item.GetComponentsInChildren<Renderer>();
			Renderer[] array = componentsInChildren2;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = false;
			}
			Animation[] componentsInChildren3 = item.GetComponentsInChildren<Animation>();
			Animation[] array2 = componentsInChildren3;
			foreach (Animation animation in array2)
			{
				animation.Stop();
			}
			SetInUse(item, false);
		}
	}

	public void Deallocate(GameObject go)
	{
		if (go != null)
		{
			go.transform.parent = mFolder.transform;
			go.SetActive(false);
			SetInUse(go, false);
		}
	}

	public void Deallocate(string name)
	{
		if (!mPool.ContainsKey(name))
		{
			return;
		}
		foreach (GameObject item in mPool[name])
		{
			Object.Destroy(item);
		}
		mPool.Remove(name);
	}

	public void VFXDeallocate(GameObject go)
	{
		if (go != null)
		{
			go.transform.parent = mFolder.transform;
			go.transform.localPosition = Vector3.zero;
			SetInUse(go, false);
		}
	}

	public void Update()
	{
		mTempList.Clear();
		foreach (KeyValuePair<GameObject, bool> mPoolStat in mPoolStats)
		{
			if (mPoolStat.Value && mPoolStat.Key != null && !mPoolStat.Key.activeSelf)
			{
				mTempList.Add(mPoolStat.Key);
			}
		}
		foreach (GameObject mTemp in mTempList)
		{
			Deallocate(mTemp);
		}
	}
}
