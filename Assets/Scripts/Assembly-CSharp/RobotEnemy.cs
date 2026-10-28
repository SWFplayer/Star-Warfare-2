using UnityEngine;

public class RobotEnemy
{
	public GameObject o;

	public Vector3[] targetPoses = new Vector3[2]
	{
		new Vector3(11.4f, 0f, 14.4f),
		new Vector3(5.5f, 0f, -2.3f)
	};

	public Vector3 targetPos = Vector3.zero;

	protected Timer changeTargetTimer = new Timer();

	protected Timer moveTimer = new Timer();

	protected int currentTargetIndex;

	protected Vector3 lastTarget;

	protected Vector3 dir;

	public short EnemyID { get; set; }

	public RobotEnemy()
	{
		changeTargetTimer.SetTimer(5f, true);
		moveTimer.SetTimer(0.5f, false);
	}

	public void Loop(UIRobot robot)
	{
		if (!robot.lobby.IsMasterPlayer)
		{
			return;
		}
		if (changeTargetTimer.Ready())
		{
			EnemyChangeTargetRequest request = new EnemyChangeTargetRequest(EnemyID, robot.id, o.transform.position);
			robot.network.SendRequest(request);
			currentTargetIndex++;
			if (currentTargetIndex == targetPoses.Length)
			{
				currentTargetIndex = 0;
			}
			lastTarget = targetPoses[currentTargetIndex];
			dir = (lastTarget - o.transform.position).normalized;
			changeTargetTimer.Do();
		}
		if (moveTimer.Ready())
		{
			EnemyStateRequest request2 = new EnemyStateRequest(EnemyID, EEnemyStateID.CATCHING, o.transform.position);
			robot.network.SendRequest(request2);
			moveTimer.Do();
		}
		o.transform.Translate(dir * 4f * Time.deltaTime, Space.World);
	}
}
