using UnityEngine;

public class BlingBling : MonoBehaviour
{
	public float Speed = 1f;

	private Material mMat;

	private float value;

	private void Start()
	{
		mMat = base.gameObject.GetComponent<Renderer>().material;
		if (mMat != null)
		{
			if (mMat.HasProperty("_lightRate"))
			{
				mMat.SetFloat("_lightRate", value);
			}
			else
			{
				base.enabled = false;
			}
		}
		else
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		value = Mathf.PingPong(Time.time * Speed, 1f);
		if (mMat != null && mMat.HasProperty("_lightRate"))
		{
			mMat.SetFloat("_lightRate", value);
		}
	}
}
