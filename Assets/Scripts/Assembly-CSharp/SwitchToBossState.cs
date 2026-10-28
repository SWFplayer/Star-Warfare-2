using UnityEngine;

public class SwitchToBossState : PlayerState
{
	public override void NextState(Player player, float deltaTime)
	{
		if (player.DoSwitchToBoss() && player.IsLocal())
		{
			Debug.Log("levelName = " + GlobalData.GetInstance().BossLevelName);
			LoadingUIScript.mInstance.StartLoading(LocalizationManager.GetInstance().GetString("LOC_MESSEGE_32"), LoadingUIScript.mInstance.GetRandomTip());
			GameWorld.LoadLevel(GlobalData.GetInstance().BossLevelName, ELevelType.GAME);
		}
	}
}
