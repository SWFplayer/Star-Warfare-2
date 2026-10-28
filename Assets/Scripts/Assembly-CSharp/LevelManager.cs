using UnityEngine;

public class LevelManager
{
	private LevelBehaviorScript mBehavior;

	public LevelManager()
	{
		GameObject gameObject = GameObject.Find("LevelManager");
		if (gameObject == null)
		{
			gameObject = new GameObject("LevelManager");
			Object.DontDestroyOnLoad(gameObject);
			gameObject.AddComponent<LevelBehaviorScript>();
		}
		mBehavior = gameObject.GetComponent<LevelBehaviorScript>();
	}

	public void LoadLevel(string levelName)
	{
		mBehavior.StartLoadLevel(levelName);
	}

	public void OnLoadingFinished()
	{
		mBehavior.OnLoadingFinished();
	}

	public bool IsLoadingFinished()
	{
		return mBehavior.IsLoadingFinished();
	}
}
