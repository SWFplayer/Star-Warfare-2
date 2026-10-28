using UnityEngine;

public class GameUnitStateComponent
{
	private Timer mUpdateTargetPositionTimer = new Timer(0.1f, false);

	public void UpdateTargetPosition(GameUnit gameUnit, float length, bool force, bool isSend = true)
	{
		if (gameUnit.GameUnitType == EGameUnitType.ENEMY)
		{
			return;
		}
		Player player = gameUnit as Player;
		if (player.IsLocal() && (force || mUpdateTargetPositionTimer.Ready()))
		{
			mUpdateTargetPositionTimer.Do();
			ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
			Transform transform = component.transform;
			Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(component.ReticlePosition.x, component.ReticlePosition.y, Camera.main.nearClipPlane));
			Vector3 normalized = (vector - transform.position).normalized;
			Ray ray = new Ray(transform.position, normalized);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, length, (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.FLOOR)))
			{
				gameUnit.SetTargetPosition(hitInfo.point);
			}
			else
			{
				gameUnit.SetTargetPosition(vector + normalized * length);
			}
			if (isSend)
			{
				SendTargetPosition(player);
			}
		}
	}

	public void UpdateTargetPositionOnGround(GameUnit gameUnit, float length, bool force, bool isSend = true)
	{
		if (gameUnit.GameUnitType == EGameUnitType.ENEMY)
		{
			return;
		}
		Player player = gameUnit as Player;
		if (player == null)
		{
			Debug.LogError("不是player。");
		}
		if (player.IsLocal() && (force || mUpdateTargetPositionTimer.Ready()))
		{
			mUpdateTargetPositionTimer.Do();
			ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
			Transform transform = component.transform;
			Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(component.ReticlePosition.x, component.ReticlePosition.y, Camera.main.nearClipPlane));
			Vector3 normalized = (vector - transform.position).normalized;
			Ray ray = new Ray(transform.position, normalized);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, length, (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.FLOOR)))
			{
				gameUnit.SetTargetPosition(hitInfo.point);
			}
			else
			{
				gameUnit.SetTargetPosition(gameUnit.GetTransform().TransformDirection(Vector3.forward) * length + gameUnit.GetPosition());
			}
			if (isSend)
			{
				SendTargetPosition(player);
			}
		}
	}

	private void SendTargetPosition(Player player)
	{
		if (player.IsLocal() && GameApp.GetInstance().GetGameMode().IsBossVSPlayer())
		{
			PlayerBossChangeTargetPositionRequest request = new PlayerBossChangeTargetPositionRequest(player.GetChannelID(), player.GetTargetPosition(), player.GetPosition());
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}
}
