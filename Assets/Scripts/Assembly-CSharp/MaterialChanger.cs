using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
	public Material oldMaterial;

	public Material newMaterial;

	private Renderer mRender;

	private float newY;

	private float speed;

	private float aSpeed = 0.5f;

	private float maxY = 2f;

	private void Start()
	{
		if (mRender != null)
		{
			mRender.material = oldMaterial;
		}
	}

	private void Update()
	{
		if (oldMaterial != null)
		{
			mRender.material.SetTextureOffset("_InterlacePattern", new Vector2(0f, newY));
			if (newY < maxY)
			{
				speed += aSpeed * Time.deltaTime;
				newY += speed * Time.deltaTime;
			}
			else
			{
				mRender.material = newMaterial;
				Object.Destroy(this);
			}
		}
	}

	public void SetMaterialInfo()
	{
		mRender = base.gameObject.GetComponent<MeshRenderer>();
		if (mRender != null)
		{
			newMaterial = mRender.material;
			oldMaterial = Resources.Load("Materials/lineTest") as Material;
		}
		if (mRender != null)
		{
			Texture texture = newMaterial.GetTexture("_MainTex");
			mRender.material = oldMaterial;
			if (texture != null)
			{
				oldMaterial.SetTexture("_MainTex", texture);
			}
		}
		if (base.gameObject.GetComponent<MaterialChangeInfo>() != null)
		{
			maxY = base.gameObject.GetComponent<MaterialChangeInfo>().maxY;
			aSpeed = base.gameObject.GetComponent<MaterialChangeInfo>().YAspeed;
		}
	}

	public static void Change(GameObject obj, Material theNewMat)
	{
		obj.GetComponent<MeshRenderer>().material = theNewMat;
	}
}
