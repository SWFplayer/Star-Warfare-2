using System.Collections.Generic;
using UnityEngine;

public class PlayerBossState : GameUnitState
{
	public static byte Index;

	public static Dictionary<byte, GameUnitState> States = new Dictionary<byte, GameUnitState>();

	public byte ID;

	public PlayerBossState()
	{
		ID = Index++;
		States.Add(ID, this);
	}

	public virtual void ChangeInnerState(Player player, int innerState)
	{
	}

	protected void StartIdleWhenAnimFinish(Player player)
	{
		if (player.CurrentAnimTime >= 1f)
		{
			player.SetState(Player.BOSS_IDLE_STATE);
			if (GameApp.GetInstance().GetGameMode().IsBossVSPlayer() && player.IsLocal())
			{
				PlayerBossStateRequest request = new PlayerBossStateRequest(Player.BOSS_IDLE_STATE);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
	}

	protected void RestrictRotationSpeed(Player player, float raidanSpeed)
	{
		if (player.IsLocal())
		{
			LocalPlayer localPlayer = player as LocalPlayer;
			InputController inputController = localPlayer.inputController;
			ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
			float num = raidanSpeed * Time.deltaTime * 57.29578f;
			float x = Mathf.Clamp(inputController.CameraRotation.x, 0f - num, num);
			float y = Mathf.Clamp(inputController.CameraRotation.y, 0f - num, num);
			inputController.CameraRotation = new Vector2(x, y);
		}
	}
}
