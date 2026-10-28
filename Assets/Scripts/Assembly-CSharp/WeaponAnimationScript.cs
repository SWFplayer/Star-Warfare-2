using UnityEngine;

public class WeaponAnimationScript : MonoBehaviour
{
	public float minRandomTime = 3f;

	public float maxRandomTime = 5f;

	public float closeTime = 1f;

	public string animationName = "open";

	private bool isCurrentStatusOpen;

	private Timer animationTimer;

	private void Start()
	{
		if (base.GetComponent<Animation>()[animationName] != null)
		{
			base.GetComponent<Animation>()[animationName].wrapMode = WrapMode.ClampForever;
			base.GetComponent<Animation>()[animationName].speed = 1f;
			base.GetComponent<Animation>()[animationName].normalizedTime = 0f;
			base.GetComponent<Animation>().Play(animationName);
			isCurrentStatusOpen = true;
			animationTimer = new Timer(Random.Range(minRandomTime, maxRandomTime), false);
		}
	}

	private void Update()
	{
		if (base.GetComponent<Animation>() != null && animationTimer.Ready())
		{
			if (isCurrentStatusOpen)
			{
				base.GetComponent<Animation>()[animationName].wrapMode = WrapMode.ClampForever;
				base.GetComponent<Animation>()[animationName].speed = -1f;
				base.GetComponent<Animation>()[animationName].normalizedTime = 1f;
				base.GetComponent<Animation>().Play(animationName);
				isCurrentStatusOpen = false;
				animationTimer = new Timer(closeTime, false);
			}
			else
			{
				base.GetComponent<Animation>()[animationName].wrapMode = WrapMode.ClampForever;
				base.GetComponent<Animation>()[animationName].speed = 1f;
				base.GetComponent<Animation>()[animationName].normalizedTime = 0f;
				base.GetComponent<Animation>().Play(animationName);
				isCurrentStatusOpen = true;
				animationTimer = new Timer(Random.Range(minRandomTime, maxRandomTime), false);
			}
		}
	}
}
