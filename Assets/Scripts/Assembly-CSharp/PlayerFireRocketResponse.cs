using UnityEngine;

internal class PlayerFireRocketResponse : Response
{
	public byte weaponId;

	public EVehicleWeaponType vehicleWeaponType;

	public Vector3 pos;

	public Vector3 dir;

	public float bulletSpeed;

	private bool isMine;

	protected int trackingID;

	public override void ReadData(byte[] data)
	{
		BytesBuffer bytesBuffer = new BytesBuffer(data);
		isMine = bytesBuffer.ReadByte() == 1;
		weaponId = bytesBuffer.ReadByte();
		if (weaponId == 0)
		{
			vehicleWeaponType = (EVehicleWeaponType)bytesBuffer.ReadByte();
			bulletSpeed = (float)bytesBuffer.ReadShort() / 10f;
		}
		else if (weaponId == 31)
		{
			bulletSpeed = (float)bytesBuffer.ReadShort() / 10f;
		}
		short num = bytesBuffer.ReadShort();
		short num2 = bytesBuffer.ReadShort();
		short num3 = bytesBuffer.ReadShort();
		short num4 = bytesBuffer.ReadShort();
		short num5 = bytesBuffer.ReadShort();
		short num6 = bytesBuffer.ReadShort();
		if (weaponId != 0)
		{
			WeaponConfig weaponConfig = Weapon.sGetWeaponConfigById(weaponId);
			if (weaponConfig.mWeaponType == WeaponType.TrackingRPG || weaponConfig.mWeaponType == WeaponType.SplitRPG)
			{
				trackingID = bytesBuffer.ReadInt();
			}
		}
		float x = (float)num / 10f;
		float y = (float)num2 / 10f;
		float z = (float)num3 / 10f;
		pos = new Vector3(x, y, z);
		x = (float)num4 / 10000f;
		y = (float)num5 / 10000f;
		z = (float)num6 / 10000f;
		dir = new Vector3(x, y, z);
	}

	public override void ProcessLogic()
	{
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld == null || gameWorld.State != GameState.Playing)
		{
			return;
		}
		WeaponConfig weaponConfig = new WeaponConfig();
		EffectPoolType type = EffectPoolType.NONE;
		if (weaponId != 0)
		{
			weaponConfig = Weapon.sGetWeaponConfigById(weaponId);
			switch (weaponConfig.mWeaponType)
			{
			case WeaponType.BowSword:
				type = EffectPoolType.BOW_ROCKET;
				break;
			case WeaponType.RPG:
			case WeaponType.TrackingRPG:
				type = EffectPoolType.W3_ROCKET;
				break;
			case WeaponType.SplitRPG:
				type = ((trackingID != 0) ? EffectPoolType.W28_ROCKET1 : EffectPoolType.W28_ROCKET);
				break;
			case WeaponType.QuadrupleFist:
				type = EffectPoolType.W8_FIST_EXP;
				break;
			case WeaponType.Myth370Fist:
				type = EffectPoolType.W31_ROCKET;
				break;
			case WeaponType.Fist:
				type = EffectPoolType.W8_FIST;
				break;
			case WeaponType.TwinRPG:
				type = EffectPoolType.W15_ROCKET;
				break;
			}
		}
		else if (isMine)
		{
			type = EffectPoolType.MINE_PROJECTILE;
		}
		else if (vehicleWeaponType == EVehicleWeaponType.ATV_MINI_RPG)
		{
			type = EffectPoolType.W8_FIST;
		}
		GameObject gameObject = gameWorld.GetEffectPool(type).CreateObject(pos, dir, Quaternion.identity);
		gameObject.transform.localScale = Vector3.one;
		ProjectileScript component = gameObject.GetComponent<ProjectileScript>();
		component.enabled = true;
		component.dir = dir;
		component.life = 10f;
		component.damage = 10;
		component.isLocal = false;
		component.isMine = isMine;
		if (weaponId != 0)
		{
			if (weaponId != 31)
			{
				component.flySpeed = weaponConfig.mBulletSpeed;
			}
			else
			{
				component.flySpeed = bulletSpeed;
			}
			component.explodeRadius = weaponConfig.mExplodeRadius;
			component.GunType = weaponConfig.mWeaponType;
			if (weaponConfig.mWeaponType == WeaponType.TrackingRPG && trackingID != -1)
			{
				if (GameApp.GetInstance().GetGameMode().IsVSMode())
				{
					Player playerByUserID = gameWorld.GetPlayerByUserID(trackingID);
					if (playerByUserID != null)
					{
						component.trackingPlayer = playerByUserID;
					}
				}
				else
				{
					Enemy enemyByID = gameWorld.GetEnemyByID("E_" + trackingID);
					if (enemyByID != null)
					{
						component.trackingEnemy = enemyByID;
					}
				}
			}
			else if (weaponConfig.mWeaponType == WeaponType.SplitRPG)
			{
				if (trackingID == 0)
				{
					component.isSpliting = true;
				}
				else
				{
					if (trackingID == -1)
					{
						return;
					}
					component.isSplited = true;
					if (GameApp.GetInstance().GetGameMode().IsVSMode())
					{
						Player playerByUserID2 = gameWorld.GetPlayerByUserID(trackingID);
						if (playerByUserID2 != null)
						{
							component.trackingPlayer = playerByUserID2;
						}
					}
					else
					{
						Enemy enemyByID2 = gameWorld.GetEnemyByID("E_" + trackingID);
						if (enemyByID2 != null)
						{
							component.trackingEnemy = enemyByID2;
						}
					}
				}
			}
			else if (weaponConfig.mWeaponType == WeaponType.Myth370Fist)
			{
				component.isUseGravity = true;
			}
		}
		else if (isMine)
		{
			component.flySpeed = 50f;
			component.explodeRadius = 6f;
			component.GunType = WeaponType.NoGun;
		}
		else
		{
			component.flySpeed = bulletSpeed;
			component.explodeRadius = 5f;
			component.GunType = WeaponType.NoGun;
			component.VehicleWeaponType = vehicleWeaponType;
		}
	}
}
