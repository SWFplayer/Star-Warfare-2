using UnityEngine;

public class DiyColorMove : MonoBehaviour
{
	public float speed = 0.1f;

	public float colorHalfWidth = 0.02f;

	public bool scanFromX = true;

	private float value;

	private Material mMat;

	private float originWidth;

	private bool originScan;

	private void Start()
	{
		mMat = base.gameObject.GetComponent<MeshRenderer>().material;
		if (mMat == null)
		{
			base.enabled = false;
		}
		if (mMat.HasProperty("_Width"))
		{
			mMat.SetFloat("_Width", colorHalfWidth);
		}
		else
		{
			base.enabled = false;
		}
		if (mMat.HasProperty("_RollFromX"))
		{
			if (scanFromX)
			{
				mMat.SetFloat("_RollFromX", 1f);
			}
			else
			{
				mMat.SetFloat("_RollFromX", 0f);
			}
		}
		else
		{
			base.enabled = false;
		}
		originWidth = colorHalfWidth;
		originScan = scanFromX;
	}

	private void Update()
	{
		if (mMat.HasProperty("_Roll"))
		{
			value += Time.deltaTime * speed;
			while (value >= 1f)
			{
				value--;
			}
			while (value <= 0f)
			{
				value++;
			}
			mMat.SetFloat("_Roll", value);
		}
		if (originWidth != colorHalfWidth)
		{
			mMat.SetFloat("_Width", colorHalfWidth);
			originWidth = colorHalfWidth;
		}
		if (originScan == scanFromX)
		{
			return;
		}
		if (mMat.HasProperty("_RollFromX"))
		{
			if (scanFromX)
			{
				mMat.SetFloat("_RollFromX", 1f);
			}
			else
			{
				mMat.SetFloat("_RollFromX", 0f);
			}
		}
		originScan = scanFromX;
	}
}
