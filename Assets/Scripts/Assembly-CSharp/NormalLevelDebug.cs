using System;
using System.Collections.Generic;
using UnityEngine;

public class NormalLevelDebug : MonoBehaviour
{
	[Serializable]
	public class FollowModel
	{
		public GameObject model;

		public Vector3 deltaPos;
	}

	private bool attached;

	public bool inGameModify;

	public List<FollowModel> modelList = new List<FollowModel>();

	private void Start()
	{
	}

	private void Update()
	{
		if (!attached && (bool)GameObject.Find("Player"))
		{
			foreach (FollowModel model in modelList)
			{
				model.model.transform.position = model.deltaPos + GameObject.Find("Player").transform.position;
				model.model.transform.parent = GameObject.Find("Player").transform;
			}
			attached = true;
		}
		if (!inGameModify)
		{
			return;
		}
		foreach (FollowModel model2 in modelList)
		{
			model2.model.transform.position = model2.deltaPos + GameObject.Find("Player").transform.position;
		}
	}
}
