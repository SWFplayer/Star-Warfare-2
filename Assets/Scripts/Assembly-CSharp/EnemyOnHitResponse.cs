using UnityEngine;

internal class EnemyOnHitResponse : Response
{
	protected short m_enemyID;

	protected int m_hp;

	protected byte m_dead;

	protected int m_killerID;

	protected bool m_criticalAttack;

	protected byte m_weaponType;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		m_enemyID = bytesBuffer.ReadShort();
		m_hp = bytesBuffer.ReadInt();
		m_dead = bytesBuffer.ReadByte();
		m_weaponType = bytesBuffer.ReadByte();
		if (m_dead == 1)
		{
			m_killerID = bytesBuffer.ReadInt();
			m_criticalAttack = bytesBuffer.ReadBool();
		}
	}

	public override void ProcessLogic()
	{
		Enemy enemyByID = GameApp.GetInstance().GetGameWorld().GetEnemyByID("E_" + m_enemyID);
		if (enemyByID != null)
		{
			enemyByID.OnHitResponse(m_hp, m_killerID, m_criticalAttack, (WeaponType)m_weaponType);
		}
	}

	public override void ProcessRobotLogic(UIRobot robot)
	{
		if (robot.lobby.IsMasterPlayer && m_hp <= 0)
		{
			robot.RemoveEnemy(m_enemyID);
			Debug.Log("enemy dead:" + m_enemyID);
		}
	}
}
