using UnityEngine;

public class ChangeTintColor : MonoBehaviour
{
	public Color from;

	public Color to;

	public float Speed;

	private Material mMat;

	private void Start()
	{
		if (base.gameObject.GetComponent<Renderer>().material != null && base.gameObject.GetComponent<Renderer>().material.HasProperty("_TintColor"))
		{
			mMat = base.gameObject.GetComponent<Renderer>().material;
		}
	}

	private void Update()
	{
		float t = Mathf.PingPong(Time.time * Speed, 1f);
		if (mMat != null)
		{
			mMat.SetColor("_TintColor", Color.Lerp(from, to, t));
		}
	}
}
