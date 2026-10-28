using UnityEngine;

public class UnitMaterial
{
	private string strName;

	private Material material;

	public bool Load(string name)
	{
		return true;
	}

	public Material GetMaterial()
	{
		return material;
	}

	public void Free()
	{
		material = null;
		strName = string.Empty;
	}
}
