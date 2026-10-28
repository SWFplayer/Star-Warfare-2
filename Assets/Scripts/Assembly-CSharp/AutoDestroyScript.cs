using UnityEngine;

public class AutoDestroyScript : MonoBehaviour
{
	public float life;

	public string destroySound = string.Empty;

	public AutoDestroyType type = AutoDestroyType.DESTROY;

	protected float createdTime;

	private void Start()
	{
		createdTime = Time.time;
	}

	private void Update()
	{
		if (Time.time - createdTime > life)
		{
			if (!destroySound.Equals(string.Empty))
			{
				AudioManager.GetInstance().PlaySoundAt(destroySound, base.gameObject.transform.position);
			}
			if (type == AutoDestroyType.DESTROY)
			{
				Object.Destroy(base.gameObject);
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}
	}

	private void OnEnable()
	{
		createdTime = Time.time;
	}
}
