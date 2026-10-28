using UnityEngine;

public class ShadowReceiver : MonoBehaviour
{
	public float _ShadowDarkness = 0.45f;

	public Vector4 _ShadowColor = new Vector4(39f / 255f, 28f / 255f, 8f / 255f, 1f);

	protected string shaderSuffix = "_Shadow";

	private Camera shadow_cam;

	private void Awake()
	{
		if (GameApp.GetInstance().GetUserState().IsLowQuality())
		{
			base.enabled = false;
		}
		else
		{
			Init();
		}
	}

	private bool Equal(float a, float b)
	{
		return Mathf.Abs(a - b) < 0.01f;
	}

	public void ChangeTexture()
	{
		if (shadow_cam == null)
		{
			for (int i = 0; i < Camera.allCameras.Length; i++)
			{
				if (Camera.allCameras[i].name == "ShadowCamera")
				{
					shadow_cam = Camera.allCameras[i];
					break;
				}
			}
		}
		if (!(null != shadow_cam))
		{
			return;
		}
		string text = base.GetComponent<Renderer>().material.shader.name;
		if (!text.EndsWith(shaderSuffix))
		{
			Shader shader = Shader.Find(text + shaderSuffix);
			if (shader != null)
			{
				for (int j = 0; j < base.GetComponent<Renderer>().materials.Length; j++)
				{
					base.GetComponent<Renderer>().materials[j].shader = shader;
					base.GetComponent<Renderer>().materials[j].SetTexture("_ShadowMap", shadow_cam.targetTexture);
					base.GetComponent<Renderer>().materials[j].SetFloat("_ShadowDarkness", _ShadowDarkness);
					base.GetComponent<Renderer>().materials[j].SetColor("_ShadowColor", _ShadowColor);
					ChangeMaterial(base.GetComponent<Renderer>().materials[j]);
				}
			}
		}
		Vector3 localScale = base.transform.localScale;
		if (Equal(localScale.x, localScale.y) && Equal(localScale.x, localScale.z) && localScale.x > 1f)
		{
			base.transform.localScale = new Vector3(0f, 0.001f, 0f) + localScale;
		}
	}

	public void ChangeMaterial(string newMaterialName)
	{
		if (shadow_cam == null)
		{
			for (int i = 0; i < Camera.allCameras.Length; i++)
			{
				if (Camera.allCameras[i].name == "ShadowCamera")
				{
					shadow_cam = Camera.allCameras[i];
					break;
				}
			}
		}
		if (null != shadow_cam)
		{
			Material material = Resources.Load(newMaterialName) as Material;
			if (material == null)
			{
				Debug.LogError("could not find material: " + newMaterialName);
			}
			base.GetComponent<Renderer>().material = material;
			base.GetComponent<Renderer>().material.SetTexture("_ShadowMap", shadow_cam.targetTexture);
			Vector3 localScale = base.transform.localScale;
			if (Equal(localScale.x, localScale.y) && Equal(localScale.x, localScale.z) && localScale.x > 1f)
			{
				base.transform.localScale = new Vector3(0f, 0.001f, 0f) + localScale;
			}
		}
	}

	public void OnWillRenderObject()
	{
		if (!(shadow_cam != null))
		{
			return;
		}
		Vector3 v = base.transform.localScale;
		if (Equal(v.x, v.y) && Equal(v.x, v.z))
		{
			if (v.x > 1f)
			{
				v.y += 0.001f;
			}
			else
			{
				v = new Vector3(1f, 1f, 1f);
			}
		}
		Matrix4x4 inverse = Matrix4x4.Scale(v).inverse;
		Matrix4x4 matrix = shadow_cam.projectionMatrix * shadow_cam.worldToCameraMatrix * base.GetComponent<Renderer>().localToWorldMatrix * inverse;
		Material[] materials = base.GetComponent<Renderer>().materials;
		foreach (Material material in materials)
		{
			material.SetMatrix("_LocalToShadowMatrix", matrix);
		}
	}

	protected virtual void ChangeMaterial(Material mat)
	{
	}

	protected virtual void Init()
	{
		shaderSuffix = "_Shadow";
	}
}
