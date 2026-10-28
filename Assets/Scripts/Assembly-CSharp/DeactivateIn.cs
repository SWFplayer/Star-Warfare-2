using UnityEngine;

public class DeactivateIn : MonoBehaviour
{
	public float InTime;

	private float mSpawnTime;

	private void Start()
	{
		mSpawnTime = Time.time;
	}

	private void Update()
	{
		if (Time.time - mSpawnTime > InTime)
		{
			base.gameObject.SetActive(false);
		}
	}
}
