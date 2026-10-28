using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderManagerTesterOff : MonoBehaviour
{
	private void OnClick()
	{
		GameObject player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.GetObject();
		Hashtable enemies = GameApp.GetInstance().GetGameWorld().GetEnemies();
		List<Robot> robotList = GameApp.GetInstance().GetGameWorld().GetRobotList();
		ShaderManager.GetInstance().CloseOutline(player, enemies, robotList);
	}
}
