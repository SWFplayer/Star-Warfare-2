using System;
using UnityEngine;

public class DelayActivateScript : MonoBehaviour
{
	[NonSerialized]
	public float ActivateTime;

	public float life;

	private float mSpawnTime;

	private bool mIsActive;

	private GameObject mShadow;

	private void Update()
	{
		if (mIsActive)
		{
			if (Time.time - mSpawnTime > ActivateTime + life)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
			if (mShadow != null && Time.time - mSpawnTime > ActivateTime + 0.3f)
			{
				UnityEngine.Object.Destroy(mShadow);
			}
		}
		else if (Time.time - mSpawnTime > ActivateTime)
		{
			mIsActive = true;
			for (int i = 0; i < base.gameObject.transform.childCount; i++)
			{
				Transform child = base.gameObject.transform.GetChild(i);
				child.gameObject.SetActive(true);
			}
		}
	}

	public void Reset()
	{
		mIsActive = false;
		mSpawnTime = Time.time;
		for (int i = 0; i < base.gameObject.transform.childCount; i++)
		{
			Transform child = base.gameObject.transform.GetChild(i);
			child.gameObject.SetActive(false);
		}
		GameObject original = Resources.Load("_Old/_Effect/Shadow") as GameObject;
		mShadow = UnityEngine.Object.Instantiate(original) as GameObject;
		mShadow.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		mShadow.transform.position = base.transform.position + new Vector3(0f, 0.1f, 0f);
	}
}
