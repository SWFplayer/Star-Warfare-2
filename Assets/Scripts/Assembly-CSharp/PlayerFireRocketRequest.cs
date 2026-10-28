using UnityEngine;

public class PlayerFireRocketRequest : Request
{
	protected byte weaponId;

	protected EVehicleWeaponType vehicleWeaponType;

	protected short x;

	protected short y;

	protected short z;

	protected short dx;

	protected short dy;

	protected short dz;

	protected int trackingID;

	protected short bulletSpeed;

	protected byte isMine;

	public PlayerFireRocketRequest(int weaponId, Vector3 pos, Vector3 dir)
	{
		requestID = 114;
		this.weaponId = (byte)weaponId;
		x = (short)(pos.x * 10f);
		y = (short)(pos.y * 10f);
		z = (short)(pos.z * 10f);
		dx = (short)(dir.x * 10000f);
		dy = (short)(dir.y * 10000f);
		dz = (short)(dir.z * 10000f);
	}

	public PlayerFireRocketRequest(int weaponId, Vector3 pos, Vector3 dir, float flySpeed)
	{
		requestID = 114;
		this.weaponId = (byte)weaponId;
		x = (short)(pos.x * 10f);
		y = (short)(pos.y * 10f);
		z = (short)(pos.z * 10f);
		dx = (short)(dir.x * 10000f);
		dy = (short)(dir.y * 10000f);
		dz = (short)(dir.z * 10000f);
		bulletSpeed = (short)(flySpeed * 10f);
	}

	public PlayerFireRocketRequest(AdvancedSkill.ESpecialSkillType type, Vector3 pos, Vector3 dir)
	{
		requestID = 114;
		weaponId = 0;
		x = (short)(pos.x * 10f);
		y = (short)(pos.y * 10f);
		z = (short)(pos.z * 10f);
		dx = (short)(dir.x * 10000f);
		dy = (short)(dir.y * 10000f);
		dz = (short)(dir.z * 10000f);
		isMine = 1;
	}

	public PlayerFireRocketRequest(EVehicleWeaponType vehicleWeaponType, Vector3 pos, Vector3 dir, float bulletSpeed)
	{
		requestID = 114;
		weaponId = 0;
		this.vehicleWeaponType = vehicleWeaponType;
		x = (short)(pos.x * 10f);
		y = (short)(pos.y * 10f);
		z = (short)(pos.z * 10f);
		dx = (short)(dir.x * 10000f);
		dy = (short)(dir.y * 10000f);
		dz = (short)(dir.z * 10000f);
		this.bulletSpeed = (short)(bulletSpeed * 10f);
	}

	public PlayerFireRocketRequest(int weaponId, Vector3 pos, Vector3 dir, int trackingID)
	{
		requestID = 114;
		this.weaponId = (byte)weaponId;
		x = (short)(pos.x * 10f);
		y = (short)(pos.y * 10f);
		z = (short)(pos.z * 10f);
		dx = (short)(dir.x * 10f);
		dy = (short)(dir.y * 10f);
		dz = (short)(dir.z * 10f);
		this.trackingID = trackingID;
	}

	public override BytesBuffer GetBody()
	{
		BytesBuffer bytesBuffer = new BytesBuffer(18);
		bytesBuffer.AddByte(isMine);
		bytesBuffer.AddByte(weaponId);
		if (weaponId == 0)
		{
			bytesBuffer.AddByte((byte)vehicleWeaponType);
			bytesBuffer.AddShort(bulletSpeed);
		}
		else if (weaponId == 31)
		{
			bytesBuffer.AddShort(bulletSpeed);
		}
		bytesBuffer.AddShort(x);
		bytesBuffer.AddShort(y);
		bytesBuffer.AddShort(z);
		bytesBuffer.AddShort(dx);
		bytesBuffer.AddShort(dy);
		bytesBuffer.AddShort(dz);
		if (weaponId != 0)
		{
			WeaponConfig weaponConfig = Weapon.sGetWeaponConfigById(weaponId);
			if (weaponConfig.mWeaponType == WeaponType.TrackingRPG || weaponConfig.mWeaponType == WeaponType.SplitRPG)
			{
				bytesBuffer.AddInt(trackingID);
			}
		}
		return bytesBuffer;
	}
}
