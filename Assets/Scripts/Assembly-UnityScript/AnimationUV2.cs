using System;
using UnityEngine;

[Serializable]
public class AnimationUV2 : MonoBehaviour
{
	public float speed;

	public AnimationUV2()
	{
		speed = 0.5f;
	}

	public virtual void Start()
	{
	}

	public virtual void Update()
	{
		float x = GetComponent<Renderer>().material.mainTextureOffset.x + speed * Time.deltaTime;
		Vector2 mainTextureOffset = GetComponent<Renderer>().material.mainTextureOffset;
		float num = (mainTextureOffset.x = x);
		Vector2 vector = (GetComponent<Renderer>().material.mainTextureOffset = mainTextureOffset);
		if (!(GetComponent<Renderer>().material.mainTextureOffset.x <= 1f))
		{
			int num2 = 0;
			Vector2 mainTextureOffset2 = GetComponent<Renderer>().material.mainTextureOffset;
			float num3 = (mainTextureOffset2.x = num2);
			Vector2 vector3 = (GetComponent<Renderer>().material.mainTextureOffset = mainTextureOffset2);
		}
		if (!(GetComponent<Renderer>().material.mainTextureOffset.x >= 0f))
		{
			int num4 = 1;
			Vector2 mainTextureOffset3 = GetComponent<Renderer>().material.mainTextureOffset;
			float num5 = (mainTextureOffset3.x = num4);
			Vector2 vector5 = (GetComponent<Renderer>().material.mainTextureOffset = mainTextureOffset3);
		}
	}

	public virtual void Main()
	{
	}
}
