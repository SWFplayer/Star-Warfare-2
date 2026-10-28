using System.Collections;
using UnityEngine;

public class AttractiveForce : MonoBehaviour
{
	public float radius;

	public float enemySpeed;

	public float playerSpeed;

	public Player owner;

	private Hashtable enemyList;

	public static AttractiveForce mInstance;

	private bool sent;

	private void OnEnable()
	{
		mInstance = this;
		enemyList = GameApp.GetInstance().GetGameWorld().GetEnemies();
		sent = false;
	}

	private void OnDisable()
	{
		mInstance = null;
	}

	public void Attract()
	{
		if (!owner.IsLocal())
		{
			return;
		}
		foreach (Enemy value in enemyList.Values)
		{
			if (value != null && value.EnemyType != EnemyType.TANK && !value.IsBoss() && Vector3.Distance(value.GetPosition(), new Vector3(base.transform.position.x, value.GetPosition().y, base.transform.position.z)) <= radius && value.InPlayingState() && value.GetState() != Enemy.BLACK_HOLE_STATE)
			{
				value.SetState(Enemy.BLACK_HOLE_STATE);
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().IsCoopMode())
				{
					EnemyStateRequest request = new EnemyStateRequest(value.EnemyID, value.GetID(EEnemyStateID.BLACKHOLE), value.GetPosition());
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
			}
		}
		if (!sent && GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			sent = true;
			BlackHole blackHole = new BlackHole();
			blackHole.x = base.transform.position.x;
			blackHole.z = base.transform.position.z;
			blackHole.radius = radius;
			blackHole.speed = playerSpeed;
			blackHole.owner = owner;
			blackHole.BlackHoleDuringTime = SwVoidParameter.EXPLODE_TIME;
			blackHole.isAttract = true;
			BlackholeRequest request2 = new BlackholeRequest(blackHole);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
		}
	}
}
