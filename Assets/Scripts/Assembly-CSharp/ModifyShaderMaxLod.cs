using UnityEngine;

public class ModifyShaderMaxLod : MonoBehaviour
{
	public bool isUse;

	private bool hasChanged;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnClick()
	{
		if (isUse)
		{
			hasChanged = !hasChanged;
			if (hasChanged)
			{
				Shader.globalMaximumLOD = 400;
			}
			else
			{
				Shader.globalMaximumLOD = 600;
			}
		}
	}
}
