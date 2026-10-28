using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
	public bool bMuteSound = true;

	public bool bMuteMusic = true;

	public float soundVolume = 1f;

	public float musicVolume = 1f;

	protected Dictionary<string, List<AudioObject>> m_soundClips = new Dictionary<string, List<AudioObject>>();

	protected Dictionary<string, List<GameObject>> m_musicClips = new Dictionary<string, List<GameObject>>();

	protected Dictionary<string, AudioClip> cachedSoundClips = new Dictionary<string, AudioClip>();

	private void Awake()
	{
		base.transform.position = Vector3.zero;
		base.transform.rotation = Quaternion.identity;
		bMuteSound = !GameApp.GetInstance().GetUserState().GetPlaySound();
		bMuteMusic = !GameApp.GetInstance().GetUserState().GetPlayMusic();
		soundVolume = GameApp.GetInstance().GetUserState().GetSoundVolume();
		musicVolume = GameApp.GetInstance().GetUserState().GetMusicVolume();
		NGUITools.soundVolume = soundVolume;
		if (bMuteSound)
		{
			NGUITools.soundVolume = 0f;
		}
		m_soundClips.Clear();
		m_musicClips.Clear();
	}

	public static AudioManager GetInstance()
	{
		if (Camera.main != null)
		{
			return Camera.main.GetComponent<AudioManager>();
		}
		return null;
	}

	public AudioClip LoadMusic(string name)
	{
		return Resources.Load(name, typeof(AudioClip)) as AudioClip;
	}

	public AudioClip LoadSound(string name)
	{
		if (cachedSoundClips.ContainsKey(name))
		{
			return cachedSoundClips[name];
		}
		AudioClip audioClip = Resources.Load(name, typeof(AudioClip)) as AudioClip;
		cachedSoundClips.Add(name, audioClip);
		return audioClip;
	}

	private AudioObject GetAudioObject(string name)
	{
		AudioObject audioObject = null;
		List<AudioObject> list = new List<AudioObject>();
		if (m_soundClips.ContainsKey(name))
		{
			foreach (AudioObject item in m_soundClips[name])
			{
				if (item != null && item.clipObject != null)
				{
					AudioSource component = item.clipObject.GetComponent<AudioSource>();
					if (!component.isPlaying)
					{
						item.owner = null;
						item.clipObject.transform.parent = null;
						audioObject = item;
						break;
					}
				}
				else
				{
					list.Add(audioObject);
				}
			}
			foreach (AudioObject item2 in list)
			{
				m_soundClips[name].Remove(item2);
			}
		}
		if (audioObject == null)
		{
			AudioClip audioClip = LoadSound(name);
			GameObject gameObject = new GameObject("AudioSound::" + audioClip.name);
			audioObject = new AudioObject();
			audioObject.clipObject = gameObject;
			audioObject.owner = null;
			gameObject.transform.parent = null;
			AudioSource audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
			audioSource.clip = audioClip;
			audioSource.playOnAwake = false;
			audioSource.mute = bMuteSound;
			audioSource.loop = false;
			AddSound(name, audioObject);
		}
		return audioObject;
	}

	protected void AddSound(string name, AudioObject obj)
	{
		if (m_soundClips.ContainsKey(name))
		{
			if (m_soundClips[name] == null)
			{
				m_soundClips[name] = new List<AudioObject>();
				m_soundClips[name].Add(obj);
			}
			else
			{
				m_soundClips[name].Add(obj);
			}
		}
		else
		{
			List<AudioObject> list = new List<AudioObject>();
			list.Add(obj);
			m_soundClips.Add(name, list);
		}
	}

	protected void AddMusic(string name, GameObject obj)
	{
		if (m_musicClips.ContainsKey(name))
		{
			if (m_musicClips[name] == null)
			{
				m_musicClips[name] = new List<GameObject>();
				m_musicClips[name].Add(obj);
			}
			else
			{
				m_musicClips[name].Add(obj);
			}
		}
		else
		{
			List<GameObject> list = new List<GameObject>();
			list.Add(obj);
			m_musicClips.Add(name, list);
		}
	}

	private void OnDestroy()
	{
		cachedSoundClips.Clear();
		StopAllMusic();
		StopAllSound();
	}

	public void PlayMusic(string name)
	{
		AudioClip audioClip = LoadMusic(name);
		GameObject gameObject = new GameObject("AudioMusic::" + audioClip.name);
		gameObject.transform.parent = base.transform;
		gameObject.transform.position = base.transform.position;
		AudioSource audioSource = gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
		audioSource.clip = audioClip;
		audioSource.loop = true;
		audioSource.volume = musicVolume;
		audioSource.playOnAwake = false;
		audioSource.mute = bMuteMusic;
		audioSource.Play();
		AddMusic(name, gameObject);
	}

	public bool IsPlayingMusic(string name)
	{
		if (m_musicClips.ContainsKey(name) && m_musicClips[name].Count > 0)
		{
			return true;
		}
		return false;
	}

	public void StopAllMusic()
	{
		foreach (KeyValuePair<string, List<GameObject>> musicClip in m_musicClips)
		{
			foreach (GameObject item in musicClip.Value)
			{
				if (item != null)
				{
					Object.Destroy(item);
				}
			}
		}
		m_musicClips.Clear();
	}

	public void StopAllSound()
	{
		foreach (KeyValuePair<string, List<AudioObject>> soundClip in m_soundClips)
		{
			foreach (AudioObject item in soundClip.Value)
			{
				if (item.clipObject != null)
				{
					Object.Destroy(item.clipObject);
				}
			}
		}
		m_soundClips.Clear();
	}

	public void SetSoundMute(bool bMute)
	{
		bMuteSound = bMute;
		foreach (KeyValuePair<string, List<AudioObject>> soundClip in m_soundClips)
		{
			foreach (AudioObject item in soundClip.Value)
			{
				if (item.clipObject != null)
				{
					AudioSource audioSource = item.clipObject.GetComponent(typeof(AudioSource)) as AudioSource;
					audioSource.mute = bMute;
				}
			}
		}
	}

	public void SetMusicMute(bool bMute)
	{
		bMuteMusic = bMute;
		foreach (KeyValuePair<string, List<GameObject>> musicClip in m_musicClips)
		{
			foreach (GameObject item in musicClip.Value)
			{
				if (item != null)
				{
					AudioSource audioSource = item.GetComponent(typeof(AudioSource)) as AudioSource;
					audioSource.mute = bMute;
				}
			}
		}
	}

	public void SetSoundVolume(float volume)
	{
		soundVolume = volume;
		foreach (KeyValuePair<string, List<AudioObject>> soundClip in m_soundClips)
		{
			foreach (AudioObject item in soundClip.Value)
			{
				if (item.clipObject != null)
				{
					AudioSource audioSource = item.clipObject.GetComponent(typeof(AudioSource)) as AudioSource;
					audioSource.volume = volume;
				}
			}
		}
	}

	public void SetMusicVolume(float volume)
	{
		musicVolume = volume;
		foreach (KeyValuePair<string, List<GameObject>> musicClip in m_musicClips)
		{
			foreach (GameObject item in musicClip.Value)
			{
				if (item != null)
				{
					AudioSource audioSource = item.GetComponent(typeof(AudioSource)) as AudioSource;
					audioSource.volume = volume;
				}
			}
		}
	}

	public void PlaySoundAt(string name, Vector3 pos)
	{
		PlaySoundAt(name, null, pos);
	}

	public void PlaySoundAt(string name, Vector3 pos, AudioRolloffMode mode, float maxDistance)
	{
		PlaySoundAt(name, null, pos, mode, maxDistance);
	}

	public void PlaySound(string name)
	{
		PlaySound(name, null);
	}

	public void PlaySoundLoop(string name)
	{
		PlaySoundLoop(name, null);
	}

	public void PlaySoundSingle(string name)
	{
		if (!IsPlayingSound(name))
		{
			PlaySound(name);
		}
	}

	public void PlaySoundSingleLoop(string name)
	{
		if (!IsPlayingSound(name))
		{
			PlaySoundLoop(name);
		}
	}

	public bool IsPlayingSound(string name)
	{
		if (m_soundClips.ContainsKey(name) && m_soundClips[name].Count > 0)
		{
			bool result = false;
			foreach (AudioObject item in m_soundClips[name])
			{
				if (item.clipObject != null)
				{
					AudioSource component = item.clipObject.GetComponent<AudioSource>();
					if (component.isPlaying)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}
		return false;
	}

	public void PlaySoundSingleAt(string name, Vector3 pos)
	{
		if (!IsPlayingSound(name))
		{
			PlaySoundAt(name, pos);
		}
	}

	public void PlaySoundSingleAt(string name, Vector3 pos, AudioRolloffMode mode, float maxDistance)
	{
		if (!IsPlayingSound(name))
		{
			PlaySoundAt(name, pos, mode, maxDistance);
		}
	}

	public void StopSound(string name)
	{
		if (!m_soundClips.ContainsKey(name))
		{
			return;
		}
		for (int i = 0; i < m_soundClips[name].Count; i++)
		{
			AudioObject audioObject = m_soundClips[name][i];
			if (audioObject.clipObject != null)
			{
				AudioSource component = audioObject.clipObject.GetComponent<AudioSource>();
				component.Stop();
			}
			audioObject.owner = null;
		}
	}

	public void PlaySoundAt(string name, GameObject owner, Vector3 pos)
	{
		if (soundVolume != 0f && !bMuteSound)
		{
			AudioObject audioObject = GetAudioObject(name);
			audioObject.owner = owner;
			audioObject.clipObject.transform.parent = base.transform;
			audioObject.clipObject.transform.position = pos;
			AudioSource component = audioObject.clipObject.GetComponent<AudioSource>();
			component.volume = soundVolume;
			component.maxDistance = 300f;
			component.rolloffMode = AudioRolloffMode.Linear;
			component.loop = false;
			component.Play();
		}
	}

	public void PlaySoundAt(string name, GameObject owner, Vector3 pos, AudioRolloffMode mode, float maxDistance)
	{
		if (soundVolume != 0f && !bMuteSound)
		{
			AudioObject audioObject = GetAudioObject(name);
			audioObject.owner = owner;
			audioObject.clipObject.transform.parent = base.transform;
			audioObject.clipObject.transform.position = pos;
			AudioSource component = audioObject.clipObject.GetComponent<AudioSource>();
			component.volume = soundVolume;
			component.maxDistance = maxDistance;
			component.rolloffMode = mode;
			component.loop = false;
			component.Play();
		}
	}

	public void PlaySound(string name, GameObject owner)
	{
		if (soundVolume != 0f && !bMuteSound)
		{
			AudioObject audioObject = GetAudioObject(name);
			audioObject.owner = owner;
			audioObject.clipObject.transform.parent = base.transform;
			audioObject.clipObject.transform.position = base.transform.position;
			AudioSource component = audioObject.clipObject.GetComponent<AudioSource>();
			component.volume = soundVolume;
			component.loop = false;
			component.Play();
		}
	}

	public void PlaySoundLoop(string name, GameObject owner, Transform parent)
	{
		if (soundVolume != 0f && !bMuteSound)
		{
			AudioObject audioObject = GetAudioObject(name);
			audioObject.owner = owner;
			audioObject.clipObject.transform.parent = parent;
			audioObject.clipObject.transform.position = parent.position;
			AudioSource component = audioObject.clipObject.GetComponent<AudioSource>();
			component.volume = soundVolume;
			component.rolloffMode = AudioRolloffMode.Linear;
			component.maxDistance = 200f;
			component.loop = true;
			component.Play();
		}
	}

	public void PlaySoundLoop(string name, GameObject owner)
	{
		PlaySoundLoop(name, owner, base.transform);
	}

	public void PlaySoundSingle(string name, GameObject owner)
	{
		if (!IsPlayingSound(name, owner))
		{
			PlaySound(name, owner);
		}
	}

	public void PlaySoundSingleLoop(string name, GameObject owner, Transform parent)
	{
		if (!IsPlayingSound(name, owner))
		{
			PlaySoundLoop(name, owner, parent);
		}
	}

	public void PlaySoundSingleLoop(string name, GameObject owner)
	{
		if (!IsPlayingSound(name, owner))
		{
			PlaySoundLoop(name, owner);
		}
	}

	public bool IsPlayingSound(string name, GameObject owner)
	{
		if (m_soundClips.ContainsKey(name) && m_soundClips[name].Count > 0)
		{
			bool result = false;
			foreach (AudioObject item in m_soundClips[name])
			{
				if (item.owner == owner && item.clipObject != null)
				{
					AudioSource component = item.clipObject.GetComponent<AudioSource>();
					if (component.isPlaying)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}
		return false;
	}

	public void PlaySoundSingleAt(string name, GameObject owner, Vector3 pos)
	{
		if (!IsPlayingSound(name, owner))
		{
			PlaySoundAt(name, owner, pos);
		}
	}

	public void PlaySoundSingleAt(string name, GameObject owner, Vector3 pos, AudioRolloffMode mode, float maxDistance)
	{
		if (!IsPlayingSound(name, owner))
		{
			PlaySoundAt(name, owner, pos, mode, maxDistance);
		}
	}

	public void StopSound(string name, GameObject owner)
	{
		if (!m_soundClips.ContainsKey(name))
		{
			return;
		}
		for (int i = 0; i < m_soundClips[name].Count; i++)
		{
			AudioObject audioObject = m_soundClips[name][i];
			if (audioObject.owner == owner)
			{
				if (audioObject.clipObject != null)
				{
					AudioSource component = audioObject.clipObject.GetComponent<AudioSource>();
					component.Stop();
				}
				audioObject.owner = null;
			}
		}
	}
}
