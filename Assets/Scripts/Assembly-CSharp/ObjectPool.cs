using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
	public bool CanUseFirstOne = true;

	public bool Recycle = true;

	public bool doNotDestruct;

	protected GameObject mPrefab;

	protected List<GameObject> objects = new List<GameObject>();

	protected List<GameObject> mActiveObjects = new List<GameObject>();

	protected Dictionary<GameObject, float> createdTime = new Dictionary<GameObject, float>();

	protected float life;

	protected bool hasAnimation;

	protected bool hasParticleEmitter;

	protected GameObject folderObject;

	private List<GameObject> mRemoveList = new List<GameObject>();

	private List<GameObject> mRemoveActiveList = new List<GameObject>();

	public bool IsInitialized()
	{
		return mPrefab != null;
	}

	public void Init(string poolName, GameObject prefab, int initNum, float life)
	{
		mPrefab = prefab;
		this.life = life;
		folderObject = new GameObject(poolName);
		folderObject.tag = TagName.OBJECT_POOL;
		for (int i = 0; i < initNum; i++)
		{
			GameObject gameObject = Object.Instantiate(mPrefab) as GameObject;
			objects.Add(gameObject);
			createdTime[gameObject] = 0f;
			gameObject.transform.parent = folderObject.transform;
			gameObject.name = mPrefab.name;
			if (gameObject.GetComponent<Animation>() != null)
			{
				hasAnimation = true;
			}
			if (gameObject.GetComponent<ParticleEmitter>() != null)
			{
				hasParticleEmitter = true;
			}
			gameObject.SetActive(false);
		}
	}

	public void SetRoot(GameObject rootObject)
	{
		if (folderObject != null && rootObject != null)
		{
			folderObject.transform.parent = rootObject.transform;
		}
	}

	public GameObject CreateObject(Vector3 position, Vector3 lookAtRotation, Quaternion rotation)
	{
		if (Recycle)
		{
			for (int i = ((!CanUseFirstOne) ? 1 : 0); i < objects.Count; i++)
			{
				if (objects[i] == null)
				{
					GameObject gameObject = Object.Instantiate(mPrefab) as GameObject;
					objects[i] = gameObject;
					createdTime[gameObject] = Time.time;
					gameObject.name = mPrefab.name;
					Debug.Log(gameObject.name + "missed! Reinstantiate!");
					gameObject.transform.parent = folderObject.transform;
					gameObject.transform.position = position;
					if (lookAtRotation != Vector3.zero)
					{
						gameObject.transform.rotation = Quaternion.LookRotation(lookAtRotation);
					}
					if (rotation != Quaternion.identity)
					{
						gameObject.transform.rotation = rotation;
					}
					if (gameObject.GetComponent<Animation>() != null)
					{
						hasAnimation = true;
					}
					if (gameObject.GetComponent<ParticleEmitter>() != null)
					{
						hasParticleEmitter = true;
					}
					gameObject.SetActive(true);
					if (!mActiveObjects.Contains(gameObject))
					{
						mActiveObjects.Add(gameObject);
					}
					return gameObject;
				}
				if (!objects[i].activeSelf)
				{
					objects[i].SetActive(true);
					objects[i].transform.position = position;
					if (lookAtRotation != Vector3.zero)
					{
						objects[i].transform.rotation = Quaternion.LookRotation(lookAtRotation);
					}
					if (rotation != Quaternion.identity)
					{
						objects[i].transform.rotation = rotation;
					}
					createdTime[objects[i]] = Time.time;
					if (!mActiveObjects.Contains(objects[i]))
					{
						mActiveObjects.Add(objects[i]);
					}
					return objects[i];
				}
			}
		}
		GameObject gameObject2 = Object.Instantiate(mPrefab) as GameObject;
		objects.Add(gameObject2);
		createdTime[gameObject2] = Time.time;
		gameObject2.name = mPrefab.name;
		gameObject2.transform.parent = folderObject.transform;
		gameObject2.transform.position = position;
		if (lookAtRotation != Vector3.zero)
		{
			gameObject2.transform.rotation = Quaternion.LookRotation(lookAtRotation);
		}
		if (rotation != Quaternion.identity)
		{
			gameObject2.transform.rotation = rotation;
		}
		if (gameObject2.GetComponent<Animation>() != null)
		{
			hasAnimation = true;
		}
		if (gameObject2.GetComponent<ParticleEmitter>() != null)
		{
			hasParticleEmitter = true;
		}
		gameObject2.SetActive(true);
		if (!mActiveObjects.Contains(gameObject2))
		{
			mActiveObjects.Add(gameObject2);
		}
		return gameObject2;
	}

	public void AutoDestruct()
	{
		if (objects == null)
		{
			return;
		}
		mRemoveList.Clear();
		mRemoveActiveList.Clear();
		for (int i = 0; i < mActiveObjects.Count; i++)
		{
			if (mActiveObjects[i] == null)
			{
				if (Recycle)
				{
					Debug.Log("The Object has been destroyed!" + folderObject.name);
				}
				mRemoveList.Add(mActiveObjects[i]);
				mRemoveActiveList.Add(mActiveObjects[i]);
				continue;
			}
			if (!mActiveObjects[i].activeSelf)
			{
				if (Recycle)
				{
					if (mActiveObjects[i].transform.parent != folderObject.transform)
					{
						mActiveObjects[i].transform.parent = folderObject.transform;
					}
				}
				else
				{
					mRemoveList.Add(mActiveObjects[i]);
				}
				mRemoveActiveList.Add(mActiveObjects[i]);
			}
			if (mActiveObjects[i].activeSelf && Time.time - createdTime[mActiveObjects[i]] > life && !doNotDestruct)
			{
				if (Recycle)
				{
					mActiveObjects[i].SetActive(false);
					mActiveObjects[i].transform.parent = folderObject.transform;
				}
				else
				{
					mRemoveList.Add(mActiveObjects[i]);
				}
				mRemoveActiveList.Add(mActiveObjects[i]);
			}
		}
		foreach (GameObject mRemove in mRemoveList)
		{
			objects.Remove(mRemove);
			if (mRemove != null)
			{
				createdTime.Remove(mRemove);
				Object.Destroy(mRemove);
			}
		}
		foreach (GameObject mRemoveActive in mRemoveActiveList)
		{
			mActiveObjects.Remove(mRemoveActive);
		}
	}

	public void DestructAll()
	{
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i] != null && objects[i].activeSelf)
			{
				objects[i].SetActive(false);
				objects[i].transform.parent = folderObject.transform;
			}
		}
		mActiveObjects.Clear();
	}

	public void ReleaseAll()
	{
		for (int i = 0; i < objects.Count; i++)
		{
			if (objects[i] != null)
			{
				Object.Destroy(objects[i]);
			}
		}
		objects.Clear();
		mActiveObjects.Clear();
	}

	public GameObject FindObjectByName(string objectName)
	{
		if (folderObject == null)
		{
			return null;
		}
		Transform transform = folderObject.transform.Find(objectName);
		if (transform != null)
		{
			return transform.gameObject;
		}
		return null;
	}

	public void SetLife(float newLife)
	{
		life = newLife;
	}

	public GameObject GetPrefab()
	{
		return mPrefab;
	}
}
