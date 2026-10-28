using UnityEngine;

public class ScreenFlashScript : MonoBehaviour
{
	public delegate void VoidDelegate();

	private float RecoveryTime;

	private string alphaPropertyName = "_Alpha";

	private float flashStartTime;

	private float startAlpha;

	private float alpha;

	private Timer constantFlashTimer = new Timer(0f, true);

	public VoidDelegate OnFinish;

	private void Start()
	{
		RecoveryTime = 5f;
	}

	public void Flash(float startAlpha)
	{
		this.startAlpha = startAlpha;
		base.GetComponent<Renderer>().enabled = true;
		base.GetComponent<Renderer>().material.SetFloat(alphaPropertyName, startAlpha);
		flashStartTime = Time.time;
	}

	public void FlashSeconds(float seconds, VoidDelegate onFinish)
	{
		constantFlashTimer.SetTimer(seconds, false);
		startAlpha = 1f;
		base.GetComponent<Renderer>().enabled = true;
		base.GetComponent<Renderer>().material.SetFloat(alphaPropertyName, startAlpha);
		flashStartTime = Time.time + seconds;
		OnFinish = onFinish;
	}

	private void Update()
	{
		if (constantFlashTimer.Ready())
		{
			if (OnFinish != null)
			{
				OnFinish();
				OnFinish = null;
			}
			alpha = startAlpha - Mathf.Pow((Time.time - flashStartTime) / RecoveryTime, 3f);
			if (alpha <= 0f)
			{
				base.GetComponent<Renderer>().enabled = false;
			}
			else
			{
				base.GetComponent<Renderer>().material.SetFloat(alphaPropertyName, alpha);
			}
		}
	}
}
