public class SlasherParameter : BossParameter
{
	public enum EDamageType
	{
		TOUCH = 0,
		MELEE_ATTACK01 = 1,
		MELEE_ATTACK02 = 2,
		WEAPON_ATTACK01 = 3,
		WEAPON_ATTACK02 = 4,
		DASH = 5,
		WEAPON_DASH = 6,
		RAIN_EXPLODE = 7,
		LEAP = 8,
		WAVE_ROUND = 9,
		WAVE_DIRECT = 10,
		WEAPON_WAVE_DIRECT = 11,
		WEAPON_BLADE = 12,
		WEAPON_DROP = 13,
		BACK = 14,
		WEAPON_BACK = 15
	}

	public static SlasherParameter me = new SlasherParameter();

	public static float TURN_SPEED = 0.06f;

	public static int MELEE_ATTACK_MIN_COUNT = 3;

	public static int MELLE_ATTACK_MAX_COUNT = 5;

	public static float[] MIN_IDLE_TIME = new float[3] { 0.4f, 0.2f, 0.2f };

	public static float[] MAX_IDLE_TIME = new float[3] { 0.8f, 0.4f, 0.4f };

	public static float[] MIN_WEAPON_IDLE_TIME = new float[3] { 0.4f, 0.2f, 0.2f };

	public static float[] MAX_WEAPON_IDLE_TIME = new float[3] { 0.8f, 0.4f, 0.4f };

	public static float MAX_WEAPON_ON_TIME = 30f;

	public static float MAX_WEAPON_OFF_TIME = 15f;

	public static float MAX_CATCHING_TIME = 5f;

	public static float[] PROBABILITY_SINGLE_ATTACK = new float[3] { 70f, 70f, 70f };

	public static float[] PROBABILITY_CATCHING = new float[3] { 20f, 20f, 20f };

	public static float[] PROBABILITY_RAIN = new float[3] { 45f, 45f, 45f };

	public static float[] PROBABILITY_LEAP = new float[3] { 70f, 70f, 70f };

	public static float[] WEAPON_PROBABILITY_SINGLE_ATTACK = new float[3] { 70f, 70f, 70f };

	public static float[] WEAPON_PROBABILITY_CATCHING = new float[2] { 20f, 20f };

	public static float[] WEAPON_PROBABILITY_RAIN = new float[3] { 35f, 35f, 35f };

	public static float[] WEAPON_PROBABILITY_WAVE = new float[3] { 65f, 65f, 65f };

	public static float MELEE_ATTACK_DISTANCE = 8f;

	public static float DASH_ATTACK_DISTANCE = 28f;

	public static float GROUP_ATTACK_DISTANCE = 8f;

	public static float WEAPON_MELEE_ATTACK_DISTANCE = 7f;

	public static float WEAPON_GROUP_ATTACK_DISTANCE = 8f;

	public static float CATCHING_SPEED = 8f;

	public static float GET_WEAPON_DASH_SPEED = 20f;

	public static float HP_PERCENTAGE_PER_HIT = 0.13f;

	public static float HP_PERCENTAGE_TO_RAGE = 0.61f;

	public static float MINIMUM_DISTANCE_BEFORE_WALL = 2f;

	public static float TOUCH_RADIUS = 3.5f;

	public static float TOUCH_KNOCK_SPEED = 0.06f;

	public static float MELEE_ATTACK01_KNOCK_SPEED = 0.06f;

	public static float MELEE_ATTACK02_KNOCK_SPEED = 0.06f;

	public static float WEAPON_ATTACK01_KNOCK_SPEED = 0.1f;

	public static float WEAPON_ATTACK02_KNOCK_SPEED = 0.1f;

	public static float WEAPON_ATTACK01_RADIUS = 9f;

	public static float WEAPON_ATTACK02_RADIUS = 9f;

	public static float WEAPON_ATTACK01_ANGLE = 30f;

	public static float WEAPON_ATTACK02_ANGLE = 60f;

	public static float DASH_LOCK_DISTANCE = 24f;

	public static float DASH_OVER_DISTANCE = 5f;

	public static float DASH_STOP_DISTANCE = 10f;

	public static float DASH_KNOCK_SPEED = 0.15f;

	public static float[] DASH_SPEED = new float[3] { 11f, 13f, 13f };

	public static float DASH_CHECK_RADIUS = 6f;

	public static float DASH_SECTOR_RADIUS = 7f;

	public static float DASH_SECTOR_ANGLE = 30f;

	public static float WEAPON_DASH_KNOCK_SPEED = 0.15f;

	public static float WEAPON_DASH_SPEED = 12f;

	public static float WEAPON_DASH_CHECK_RADIUS = 6f;

	public static float WEAPON_DASH_SECTOR_RADIUS = 7f;

	public static float WEAPON_DASH_SECTOR_ANGLE = 30f;

	public static float RAIN_SPEED = 23f;

	public static float RAIN_EXPLODE_TIME = 3f;

	public static float RAIN_EXPLODE_RADIUS = 4f;

	public static float RAIN_INTERVAL = 0.05f;

	public static float RAIN_HORIZONTAL_RANGE = 16f;

	public static float RAIN_VERTICAL_UP_RANGE = 4f;

	public static float RAIN_VERTICAL_DOWN_RANGE = 0f;

	public static float LEAP_KNOCK_SPEED = 0.15f;

	public static float LEAP_OVER_DISTANCE = 3f;

	public static float LEAP_MAXIMUM_SPEED = 1.2f;

	public static float LEAP_MINIMUM_SPEED = 0.7f;

	public static float LEAP_TIME = 0.3f;

	public static float LEAP_CHECK_RADIUS = 6.6f;

	public static float LEAP_SECTOR_RADIUS = 7f;

	public static float LEAP_SECTOR_ANGLE = 30f;

	public static float WAVE_OFFSET = 4.7f;

	public static float WAVE_RADIUS = 7.5f;

	public static float WAVE_KNOCK_SPEED = 0.15f;

	public static float WAVE_LIGHT_KNOCK_SPEED = 0.04f;

	public static int WEAPON_BLADE_SPEED = 18;

	public static float WEAPON_BLADE_ANGLE = 30f;

	public static int WEAPON_BLADE_WIND_NUMBER1 = 3;

	public static int WEAPON_BLADE_WIND_NUMBER2 = 4;

	public static float WEAPON_DROP_MOVE_SPEED = 36f;

	public static float WEAPON_DROP_ROTATE_SPEED = 1080f;

	public static float WEAPON_DROP_KNOCK_SPEED = 0.1f;

	public static float BACK_DISTANCE = 16f;

	public static float BACK_MAXIMUM_SPEED = 6f;

	public static float BACK_MINIMUM_SPEED = 0.8f;

	public static float BACK_TIME = 0.16f;

	public static float BACK_KNOCK_SPEED = 0.15f;

	public static float BACK_CHECK_RADIUS = 4f;

	public static float WEAPON_BACK_KNOCK_SPEED = 0.15f;

	public static float WEAPON_BACK_CHECK_RADIUS = 4f;

	public static float SPECIAL_LEAVE_TIME = 25f;

	public static float SPECIAL_MAX_IDLE_TIME = 0.5f;

	public static float SPECIAL_MAX_CATCHING_TIME = 3f;

	public static float SPECIAL_GOTHIT_INTERVAL = 15f;

	public static float SPECIAL_APPEAR_SPEED = 20f;

	public static float SPECIAL_LEAVE_SPEED = 20f;

	public static float SPECIAL_ATTACK_RADIUS = 10f;

	public static float SPECIAL_PROBABILITY_BLADE = 33f;

	public static float SPECIAL_PROBABILITY_WAVE = 66f;

	public static float[] SLASHER_PLAYER_BOSS_ANIMATION_SPEED = new float[6] { 1.5f, 1.5f, 2f, 2f, 1.5f, 1.8f };

	public static SlasherParameter GetInstance()
	{
		return me;
	}
}
