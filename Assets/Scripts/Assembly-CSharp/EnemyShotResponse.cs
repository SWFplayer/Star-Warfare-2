using System;
using UnityEngine;

internal class EnemyShotResponse : Response
{
	protected byte m_enemyType;

	protected short m_x;

	protected short m_y;

	protected short m_z;

	protected short m_sx;

	protected short m_sy;

	protected short m_sz;

	protected short mEnemyId;

	protected bool mHasEnemyId = true;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		m_enemyType = bytesBuffer.ReadByte();
		m_x = bytesBuffer.ReadShort();
		m_y = bytesBuffer.ReadShort();
		m_z = bytesBuffer.ReadShort();
		m_sx = bytesBuffer.ReadShort();
		m_sy = bytesBuffer.ReadShort();
		m_sz = bytesBuffer.ReadShort();
		try
		{
			mEnemyId = bytesBuffer.ReadShort();
		}
		catch (Exception)
		{
			mHasEnemyId = false;
		}
	}

	public override void ProcessLogic()
	{
		Vector3 position = new Vector3((float)m_x / 10f, (float)m_y / 10f, (float)m_z / 10f);
		Vector3 vector = new Vector3((float)m_sx / 10f, (float)m_sy / 10f, (float)m_sz / 10f);
		string path = "_Old/_Effect/Scorpion_Shot";
		switch ((EnemyType)m_enemyType)
		{
		case EnemyType.SCORPION:
			path = EffectName.E3_Stick;
			break;
		case EnemyType.BIGBEE:
			path = EffectName.E4_Ball;
			break;
		case EnemyType.MYTH370:
			path = EffectName.E5_BigBall;
			break;
		}
		GameObject original = Resources.Load(path) as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original, position, Quaternion.LookRotation(-vector)) as GameObject;
		EnemyShotScript component = gameObject.GetComponent<EnemyShotScript>();
		component.enemyType = (EnemyType)m_enemyType;
		if (mHasEnemyId)
		{
			component.attacker = GameApp.GetInstance().GetGameWorld().GetEnemyByID("E_" + mEnemyId);
		}
		switch ((EnemyType)m_enemyType)
		{
		case EnemyType.SCORPION:
			component.trType = TrajectoryType.Parabola;
			component.damageType = EnemyDamageType.Normal;
			component.attackDamage = Tacorlion.ShotAttackDamage;
			component.PlayerDamageType = PlayerDamageType.BULLET;
			break;
		case EnemyType.BIGBEE:
			component.trType = TrajectoryType.Straight;
			component.damageType = EnemyDamageType.Sputtering;
			component.attackDamage = BigBee.ShotAttackDamage;
			component.areaDamage = BigBee.ShotAreaDamage;
			component.explodeRadius = BigBee.ShotExplodeRadius;
			component.PlayerDamageType = PlayerDamageType.BULLET;
			break;
		case EnemyType.MYTH370:
			component.trType = TrajectoryType.Parabola;
			component.damageType = EnemyDamageType.Explosion;
			component.attackDamage = Myth370.ShotAttackDamage;
			component.areaDamage = Myth370.ShotAreaDamage;
			component.explodeRadius = Myth370.ShotExplodeRadius;
			component.PlayerDamageType = PlayerDamageType.ENERGY;
			break;
		}
		component.speed = vector;
	}
}
