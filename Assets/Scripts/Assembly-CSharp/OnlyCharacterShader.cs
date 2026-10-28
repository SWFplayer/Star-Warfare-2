using UnityEngine;

public class OnlyCharacterShader : MonoBehaviour
{
	public float Speed = 1f;

	public bool useCustomColor;

	private Material mat;

	public Color customColor;

	public float value;

	private void Start()
	{
		mat = base.GetComponent<Renderer>().material;
		if (mat == null)
		{
			base.enabled = false;
			return;
		}
		if (mat.HasProperty("_useCustomColor"))
		{
			if (useCustomColor)
			{
				mat.SetFloat("_useCustomColor", 1f);
				if (mat.HasProperty("_paintColor"))
				{
					mat.SetColor("_paintColor", customColor);
				}
			}
			else
			{
				mat.SetFloat("_useCustomColor", 0f);
			}
		}
		else
		{
			Debug.Log("there is no this property : _useCustomColor");
		}
		if (mat.HasProperty("_lightRate"))
		{
			mat.SetFloat("_lightRate", value);
		}
		else
		{
			Debug.Log("there is no this property : _lightRate");
		}
	}

	private void Update()
	{
		value = Mathf.PingPong(Time.time * Speed, 1f);
		if (mat.HasProperty("_lightRate"))
		{
			mat.SetFloat("_lightRate", value);
		}
	}
}
