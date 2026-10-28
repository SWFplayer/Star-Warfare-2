using UnityEngine;

internal class EnemyStateResponse : Response
{
	protected short m_enemyID;

	protected byte state;

	protected short m_x;

	protected short m_y;

	protected short m_z;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		m_enemyID = bytesBuffer.ReadShort();
		state = bytesBuffer.ReadByte();
		m_x = bytesBuffer.ReadShort();
		m_y = bytesBuffer.ReadShort();
		m_z = bytesBuffer.ReadShort();
	}

	public override void ProcessLogic()
	{
		EEnemyStateID eEnemyStateID = (EEnemyStateID)state;
		if (Lobby.GetInstance().IsMasterPlayer && eEnemyStateID != EEnemyStateID.BLACKHOLE && eEnemyStateID != EEnemyStateID.STUN)
		{
			return;
		}
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		Enemy enemyByID = gameWorld.GetEnemyByID("E_" + m_enemyID);
		if (enemyByID == null)
		{
			return;
		}
		Vector3 position = new Vector3((float)m_x / 10f, (float)m_y / 10f, (float)m_z / 10f);
		enemyByID.UpdatePosition(position);
		if (eEnemyStateID != EEnemyStateID.NO_STATE && enemyByID.Hp > 0)
		{
			if (enemyByID.IsNewState)
			{
				enemyByID.ChangeToConditionState(enemyByID.mAllConditionState[state - 1]);
				enemyByID.SetState((EnemyState)enemyByID.mAllConditionState[state - 1].mState, false);
				enemyByID.SetNextConditionState(enemyByID.mAllConditionState[state - 1]);
			}
			else
			{
				enemyByID.SetState(gameWorld.GetEnemyState(eEnemyStateID));
			}
		}
	}

	public override void ProcessRobotLogic(UIRobot robot)
	{
		if (UIRobot.sShowLog)
		{
			Debug.Log("EnemyStateDataResponse::ProcessRobotLogic");
		}
	}
}
