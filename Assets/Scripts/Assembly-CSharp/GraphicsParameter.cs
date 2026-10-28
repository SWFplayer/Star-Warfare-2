using System;
using UnityEngine;

[Serializable]
public class GraphicsParameter
{
	public EImproveType improveType = EImproveType.Apply_Shadow_POM;

	public GameObject meshObj;

	public string normalMapFile;

	public string heightMapFile;

	public Vector4 lightDir = new Vector4(0f, 2.97f, 2.07f, 0f);
}
