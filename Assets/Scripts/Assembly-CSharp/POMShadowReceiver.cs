using UnityEngine;

public class POMShadowReceiver : ShadowReceiver
{
	public Texture normalMap;

	public Texture heightMap;

	public Vector4 lightDir = new Vector4(0f, 2.97f, 2.07f, 0f);

	public float _Parallax = 0.045f;

	protected override void ChangeMaterial(Material mat)
	{
		mat.SetTexture("_BumpMap", normalMap);
		mat.SetTexture("_ParallaxMap", heightMap);
		mat.SetVector("_LightDir", lightDir);
		mat.SetFloat("_Parallax", _Parallax);
	}

	protected override void Init()
	{
		shaderSuffix = "_Parallax_Shadow";
	}
}
