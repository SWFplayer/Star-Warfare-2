using System.Collections;
using UnityEngine;

public class LevelBehaviorScript : MonoBehaviour
{
	private bool mIsLoadingFinished = true;

	private AsyncOperation async;

	public IEnumerator LoadLevel(string levelName)
	{
		async = Application.LoadLevelAsync(levelName);
		yield return async;
		if (GameWorld.NextLevelType == ELevelType.MENU)
		{
			mIsLoadingFinished = true;
		}
	}

	public void StartLoadLevel(string levelName)
	{
		mIsLoadingFinished = false;
		StartCoroutine(LoadLevel(levelName));
	}

	public void OnLoadingFinished()
	{
		mIsLoadingFinished = true;
	}

	public bool IsLoadingFinished()
	{
		return mIsLoadingFinished;
	}
}
