using UnityEngine;

internal class EnemyChangeTargetResponse : Response
{
	protected short m_enemyID;

	protected int m_targetID;

	protected short m_x;

	protected short m_y;

	protected short m_z;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		m_enemyID = bytesBuffer.ReadShort();
		m_targetID = bytesBuffer.ReadInt();
		m_x = bytesBuffer.ReadShort();
		m_y = bytesBuffer.ReadShort();
		m_z = bytesBuffer.ReadShort();
	}

	public override void ProcessLogic()
	{
		if (!Lobby.GetInstance().IsMasterPlayer)
		{
			Enemy enemyByID = GameApp.GetInstance().GetGameWorld().GetEnemyByID("E_" + m_enemyID);
			if (enemyByID != null)
			{
				Vector3 position = new Vector3((float)m_x / 10f, (float)m_y / 10f, (float)m_z / 10f);
				enemyByID.UpdatePosition(position);
				enemyByID.ChangeTargetPlayer(m_targetID);
			}
		}
	}

	public override void ProcessRobotLogic(UIRobot robot)
	{
		if (UIRobot.sShowLog)
		{
			Debug.Log("EnemyChangeTargetResponse::ProcessRobotLogic");
		}
	}
}
