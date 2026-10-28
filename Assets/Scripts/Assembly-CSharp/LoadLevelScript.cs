using UnityEngine;

public class LoadLevelScript : MonoBehaviour
{
	private void Start()
	{
		GameApp.GetInstance().GetLevelManager().LoadLevel(GameWorld.NextLevel);
	}
}
