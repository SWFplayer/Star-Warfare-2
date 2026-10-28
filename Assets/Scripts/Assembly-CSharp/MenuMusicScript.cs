using UnityEngine;

public class MenuMusicScript : MonoBehaviour
{
	public static MenuMusicScript mInstance;

	private bool stop;

	private float stopStartTime;

	private float stopIntermission = 1.5f;

	private void Awake()
	{
		mInstance = this;
		AudioSource[] components = base.gameObject.GetComponents<AudioSource>();
		foreach (AudioSource audioSource in components)
		{
			if (!GameApp.GetInstance().GetUserState().GetPlayMusic())
			{
				audioSource.Stop();
			}
			audioSource.volume = GameApp.GetInstance().GetUserState().GetMusicVolume();
		}
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (stop)
		{
			if (RealTime.time - stopStartTime <= stopIntermission)
			{
				AudioSource[] components = base.gameObject.GetComponents<AudioSource>();
				for (int i = 0; i < components.Length; i++)
				{
					components[i].volume *= 1f - (RealTime.time - stopStartTime) / stopIntermission;
				}
			}
			else
			{
				Object.Destroy(base.gameObject);
			}
			return;
		}
		AudioSource[] components2 = base.gameObject.GetComponents<AudioSource>();
		foreach (AudioSource audioSource in components2)
		{
			if (!GameApp.GetInstance().GetUserState().GetPlayMusic())
			{
				audioSource.Stop();
			}
			audioSource.volume = GameApp.GetInstance().GetUserState().GetMusicVolume();
		}
		if (base.gameObject.GetComponents<AudioSource>().Length > 1 && !base.gameObject.GetComponents<AudioSource>()[0].isPlaying && !base.gameObject.GetComponents<AudioSource>()[1].isPlaying && GameApp.GetInstance().GetUserState().GetPlayMusic())
		{
			base.gameObject.GetComponents<AudioSource>()[1].Play();
		}
	}

	public void Stop()
	{
		stop = true;
		stopStartTime = RealTime.time;
	}
}
