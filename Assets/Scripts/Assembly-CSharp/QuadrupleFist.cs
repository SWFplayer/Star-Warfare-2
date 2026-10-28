using UnityEngine;

public class QuadrupleFist : Fist
{
	private const int MAX_BULLET_COUNT = 4;

	private const float BULLET_INTERVAL = 0.1f;

	private int mCurrentBulletCount;

	private float mLastBulletTime;

	public override WeaponType GetWeaponType()
	{
		return WeaponType.QuadrupleFist;
	}

	public override void Init(Player player)
	{
		base.Init(player);
	}

	public override void GunOn()
	{
		base.GunOn();
		mCurrentBulletCount = 4;
	}

	public override void Loop(float deltaTime)
	{
		base.Loop(deltaTime);
		if (mCurrentBulletCount < 4 && Time.time - mLastBulletTime > 0.1f)
		{
			mCurrentBulletCount++;
			mLastBulletTime = Time.time;
			AudioManager.GetInstance().PlaySound(base.FireSoundName);
			Vector3 direction;
			Ray weaponRay = GetWeaponRay(gunfire, out direction);
			GameObject gameObject = gameWorld.GetEffectPool(GetFistVFX()).CreateObject(gunfire.position, direction, Quaternion.identity);
			gameObject.GetComponent<AutoDestroyScript>().enabled = false;
			gameObject.GetComponent<FireLineScript>().enabled = false;
			ProjectileScript component = gameObject.GetComponent<ProjectileScript>();
			component.enabled = true;
			component.isLocal = true;
			component.dir = direction;
			component.flySpeed = bulletSpeed;
			component.explodeRadius = GetBombRange();
			component.hitForce = hitForce;
			component.life = 8f;
			component.damage = (int)damage;
			component.GunType = WeaponType.QuadrupleFist;
			component.targetPos = aimTarget;
			component.weapon = this;
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				PlayerFireRocketRequest request = new PlayerFireRocketRequest(gunID, gunfire.position, direction);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
	}

	public override void Attack(float deltaTime)
	{
		lastAttackTime = Time.time;
		mCurrentBulletCount = 0;
		if (aas != null)
		{
			aas.StartAnimation();
		}
	}

	public override void LoadConfig(byte ownedState)
	{
		base.LoadConfig(ownedState);
		attackFrenquencyInit = (attackFrenquency *= 4f);
	}

	public override EffectPoolType GetFistVFX()
	{
		return EffectPoolType.W8_FIST_EXP;
	}

	public override void CheatCheck()
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.WEAPON);
		float num = gameDataTable.GetFloat(rowID, 3);
		num *= 4f;
		byte b = 0;
		if (base.AttackFrenquencyInit < num)
		{
			b++;
		}
		if (player != null && player.GetSkills() != null)
		{
			float accBuff = GetAccBuff();
			if (accBuff > 2.25f)
			{
				b += 2;
			}
		}
		if (player != null)
		{
			if (attackFrenquency * 1.05f < num / (GetAccBuff() + 1f))
			{
				b += 4;
			}
		}
		else if (attackFrenquency < num)
		{
			b += 4;
		}
		if (b != 0 && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			CheatCheckRequest request = new CheatCheckRequest(CheatType.WEAPON_SPEED_RATE, attackFrenquency, b, (byte)gunID);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
		float obj = gameDataTable.GetFloat(rowID, 11);
		if (!GetSpeedDrag().Equals(obj) && GameApp.GetInstance().GetGameMode().IsMultiPlayer())
		{
			CheatCheckRequest request2 = new CheatCheckRequest(CheatType.PLAYER_SPEED, GetSpeedDrag(), 0, (byte)gunID);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
		}
	}
}
