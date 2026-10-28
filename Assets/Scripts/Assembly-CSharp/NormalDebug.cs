using UnityEngine;

public class NormalDebug : MonoBehaviour
{
	private static string SpecularNormal = "Mobile/Bumped Specular (1 Directional Light)";

	private static string DiffuseNormal = "Mobile/Bumped Diffuse";

	private GameObject head;

	private GameObject body;

	private GameObject foot;

	private GameObject hand;

	private GameObject jian;

	public new Light light;

	private UILabel lightPower;

	private UILabel ShiningPower;

	private bool isSpecular = true;

	private float allShiningValue;

	private bool hidenOrigin;

	private GameObject originBone;

	private void Start()
	{
	}

	private void Update()
	{
		if (originBone == null)
		{
			originBone = GameObject.Find("Bone(Clone)");
		}
		else if (originBone != null)
		{
			originBone.SetActive(false);
		}
	}

	private void ChangeAllShader(string shaderName)
	{
		ChangeShader(shaderName, head);
		ChangeShader(shaderName, body);
		ChangeShader(shaderName, foot);
		ChangeShader(shaderName, hand);
		ChangeShader(shaderName, jian);
	}

	private void ChangeAllShining()
	{
		Material material = hand.GetComponent<SkinnedMeshRenderer>().material;
		if (material.HasProperty("_Shininess"))
		{
			material.SetFloat("_Shininess", allShiningValue);
		}
		material = head.GetComponent<SkinnedMeshRenderer>().material;
		if (material.HasProperty("_Shininess"))
		{
			material.SetFloat("_Shininess", allShiningValue);
		}
		material = body.GetComponent<SkinnedMeshRenderer>().materials[0];
		if (material.HasProperty("_Shininess"))
		{
			material.SetFloat("_Shininess", allShiningValue);
		}
		material = body.GetComponent<SkinnedMeshRenderer>().materials[1];
		if (material.HasProperty("_Shininess"))
		{
			material.SetFloat("_Shininess", allShiningValue);
		}
		material = foot.GetComponent<SkinnedMeshRenderer>().material;
		if (material.HasProperty("_Shininess"))
		{
			material.SetFloat("_Shininess", allShiningValue);
		}
	}

	public void ChangeAllShader()
	{
		if (isSpecular)
		{
			ChangeAllShader(DiffuseNormal);
			isSpecular = false;
			return;
		}
		ChangeAllShader(SpecularNormal);
		isSpecular = true;
		Material material = head.GetComponent<SkinnedMeshRenderer>().material;
		allShiningValue = material.GetFloat("_Shininess");
	}

	private void ChangeShader(string shaderName, GameObject changeObject)
	{
		Material material = changeObject.GetComponent<SkinnedMeshRenderer>().material;
		material.shader = Shader.Find(shaderName);
		if (changeObject == body)
		{
			material = changeObject.GetComponent<SkinnedMeshRenderer>().materials[1];
			material.shader = Shader.Find(shaderName);
		}
	}

	private void ChangeMaterial()
	{
	}

	public void ChangeLightIntensity(float delta)
	{
		light.intensity += delta;
	}

	public void ChangeShiningValue(float delta)
	{
		if (allShiningValue == 0f)
		{
			Material material = hand.GetComponent<SkinnedMeshRenderer>().material;
			if (material.HasProperty("_Shininess"))
			{
				allShiningValue = material.GetFloat("_Shininess");
			}
		}
		allShiningValue += delta;
		ChangeAllShining();
	}
}
