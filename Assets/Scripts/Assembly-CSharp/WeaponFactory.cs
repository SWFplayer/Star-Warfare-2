public class WeaponFactory
{
	protected static WeaponFactory instance;

	protected static WeaponType[] weaponIDToWeaponType = new WeaponType[34];

	public static WeaponFactory GetInstance()
	{
		if (instance == null)
		{
			instance = new WeaponFactory();
		}
		return instance;
	}

	public Weapon CreateWeapon(byte weaponRow)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.EQUIPMENT, DataSheetName.WEAPON);
		Weapon weapon = null;
		if (gameDataTable != null)
		{
			byte wType = (byte)gameDataTable.GetInt(weaponRow, 15);
			byte wID = (byte)gameDataTable.GetInt(weaponRow, 1);
			weapon = CreateWeapon((WeaponType)wType, wID);
			weapon.RowID = weaponRow;
		}
		return weapon;
	}

	public Weapon CreateWeapon(WeaponType wType, byte wID)
	{
		Weapon weapon = null;
		switch (wType)
		{
		case WeaponType.AssaultRifle:
			weapon = new AssaultRifle();
			break;
		case WeaponType.MachineGun:
			weapon = new MachineGun();
			break;
		case WeaponType.ShotGun:
			weapon = new ShotGun();
			break;
		case WeaponType.RPG:
			weapon = new RPG();
			break;
		case WeaponType.SplitRPG:
			weapon = new SplitRPG();
			break;
		case WeaponType.BowSword:
			weapon = new BowSword();
			break;
		case WeaponType.LaserRifle:
			weapon = new LaserRifle();
			break;
		case WeaponType.TwinRPG:
			weapon = new TwinRPG();
			break;
		case WeaponType.Fist:
			weapon = new Fist();
			break;
		case WeaponType.Myth370Fist:
			weapon = new MythFist();
			break;
		case WeaponType.QuadrupleFist:
			weapon = new QuadrupleFist();
			break;
		case WeaponType.LaserShotGun:
			weapon = new LaserShotGun();
			break;
		case WeaponType.Sniper:
			weapon = new Sniper();
			break;
		case WeaponType.TrackingRPG:
			weapon = new TrackingRPG();
			break;
		case WeaponType.DoublePistol:
			weapon = new DoublePistol();
			break;
		case WeaponType.RailGun:
			weapon = new RailGun();
			break;
		case WeaponType.UltraSniper:
			weapon = new UltraSniper();
			break;
		case WeaponType.Sword:
			weapon = new Sword();
			break;
		case WeaponType.ChargeDoublePistol:
			weapon = new ChargeDoublePistol();
			break;
		}
		weapon.GunID = wID;
		return weapon;
	}
}
