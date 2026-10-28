using System.Collections.Generic;
using UnityEngine;

public class UIAtlasManager
{
	public static Material Mat_OutOfGame;

	public static Material Mat_InGame;

	public static void UpdateMatInGame()
	{
		Dictionary<string, string> mAtlasPath = LocalizationManager.GetInstance().mAtlasPath;
		if (Mat_InGame != null && mAtlasPath.ContainsKey(Mat_InGame.name))
		{
			Texture2D texture2D = Resources.Load(mAtlasPath[Mat_InGame.name], typeof(Texture2D)) as Texture2D;
			if (Mat_InGame.mainTexture != null && texture2D != Mat_InGame.mainTexture)
			{
				Resources.UnloadAsset(Mat_InGame.mainTexture);
				Mat_InGame.mainTexture = texture2D;
			}
			else if (Mat_InGame.mainTexture == null)
			{
				Mat_InGame.mainTexture = texture2D;
			}
		}
		Resources.UnloadUnusedAssets();
	}

	public static void UpdateMatOutOfGame()
	{
		Dictionary<string, string> mAtlasPath = LocalizationManager.GetInstance().mAtlasPath;
		if (Mat_OutOfGame != null && mAtlasPath.ContainsKey(Mat_OutOfGame.name))
		{
			Texture2D texture2D = Resources.Load(mAtlasPath[Mat_OutOfGame.name], typeof(Texture2D)) as Texture2D;
			if (Mat_OutOfGame.mainTexture != null && texture2D != Mat_OutOfGame.mainTexture)
			{
				Resources.UnloadAsset(Mat_OutOfGame.mainTexture);
				Mat_OutOfGame.mainTexture = texture2D;
			}
			else if (Mat_OutOfGame.mainTexture == null)
			{
				Mat_OutOfGame.mainTexture = texture2D;
			}
		}
		Resources.UnloadUnusedAssets();
	}
}
