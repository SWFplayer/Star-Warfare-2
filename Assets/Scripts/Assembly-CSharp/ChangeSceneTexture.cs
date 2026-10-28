using System;
using System.Collections.Generic;
using UnityEngine;

public class ChangeSceneTexture : MonoBehaviour
{
	[Serializable]
	public class Data
	{
		public Material material;

		public List<IndexName> indexNames;
	}

	[Serializable]
	public class IndexName
	{
		public string property;

		public string name;
	}

	public Data[] mData;

	public int SceneIndex;

	private void Start()
	{
	}
}
