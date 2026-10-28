using UnityEngine;

public class CrimsonQueenParameter : BossParameter
{
	public enum EDamageType
	{
		TOUCH = 0,
		MELEE_ATTACK01 = 1,
		MELEE_ATTACK02 = 2,
		TAIL_LASER = 3,
		FUNNEL_LASER = 4,
		TRACKING_RPG = 5,
		EXPLOSION_SPAWN = 6,
		DASH = 7,
		FUNNEL_LASER_ATTACK = 8
	}

	public static CrimsonQueenParameter me = new CrimsonQueenParameter();

	public static float MELEE_ATTACK_RADIUS = 8f;

	public static float RANGE_ATTACK_RADIUS = 30f;

	public static float TURN_SPEED = 0.06f;

	public static float[] RUN_SPEED = new float[3] { 9f, 11f, 12f };

	public static float[] IDLE_TIME = new float[3] { 0.5f, 0.5f, 0.5f };

	public static float TOUCH_RADIUS = 3.5f;

	public static float TOUCH_KNOCK_SPEED = 0.06f;

	public static float HP_PERCENTAGE_TO_RAGE = 0.4f;

	public static float HP_PERCENTAGE_PER_HIT = 0.15f;

	public static float MINIMUM_DISTANCE_BEFORE_WALL = 4f;

	public static sStepBack STEP_BACK = new sStepBack
	{
		TRIGGER_STEP_BACK_MELEE_ATTACK_HIT_NUM = 2,
		TRIGGER_STEP_BACK_MELEE_ATTACK_NUM = 4,
		MINIMUM_DISTANCE_BEFORE_WALL = 4f,
		BACK_DISTANCE = 16f,
		BACK_MAXIMUM_SPEED = 6f,
		BACK_MINIMUM_SPEED = 0.8f,
		BACK_TIME = 0.16f
	};

	public static float BORN_FUNNEL_LASER_RADIUS = 10f;

	public static float BEE_OPEN_SOUND_TIME = 0.1f;

	public static float BEE_START_BACK_ANIM_TIME = 0.8f;

	public static float[] MELEE_ATTACK_CHANCE1 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] MELEE_ATTACK_CHANCE2 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static Vector2 MELEE_ATTACK_CHECK_TIME1 = new Vector2(0.37f, 0.8f);

	public static float MELEE_ATTACK_KNOCK_SPEED1 = 0.1f;

	public static Vector2 MELEE_ATTACK_MOVE_FORWARD_TIME1 = new Vector2(0.37f, 0.8f);

	public static float MELEE_ATTACK_MOVE_FORWARD_SPEED1 = 10f;

	public static Vector2 MELEE_ATTACK_CHECK_TIME2 = new Vector2(0.17f, 0.7f);

	public static float MELEE_ATTACK_KNOCK_SPEED2 = 0.1f;

	public static Vector2 MELEE_ATTACK_MOVE_FORWARD_TIME2 = new Vector2(0.17f, 0.7f);

	public static float MELEE_ATTACK_MOVE_FORWARD_SPEED2 = 5f;

	public static float[] MIDDLE_ATTACK_CHANCE1 = new float[3] { 1f, 1f, 1f };

	public static float[] MIDDLE_ATTACK_CHANCE2 = new float[3] { 0.5f, 0.5f, 0.5f };

	public static float[] MIDDLE_ATTACK_CHANCE3 = new float[3] { 1f, 1f, 1f };

	public static float[] MIDDLE_ATTACK_CHANCE4 = new float[3] { 1f, 1f, 1f };

	public static float[] MIDDLE_ATTACK_CHANCE5 = new float[3] { 1f, 1f, 1f };

	public static float TAIL_LASER_DURATION = 3f;

	public static float TAIL_LASER_ROTATE_SPEED = 17f;

	public static float TAIL_LASER_KNOCK_SPEED = 0.1f;

	public static float FUNNEL_LASER_MOVE_TO_POINT_SPEED = 25f;

	public static int[] FUNNEL_LASER_NUM = new int[3] { 5, 8, 8 };

	public static int[] FUNNEL_LASER_ATTACK_NUM = new int[3] { 1, 3, 3 };

	public static float FUNNEL_LASER_RADIUS = 40f;

	public static float FUNNEL_LASER_MOVE_SPEED = 20f;

	public static float FUNNEL_LASER_MOVE_ACC = 10f;

	public static float FUNNEL_LASER_MOVE_BACK_SPEED = 40f;

	public static float FUNNEL_LASER_BEE_SPAWN_TIME = 0.5f;

	public static float FUNNEL_LASER_KNOCK_SPEED = 0.1f;

	public static float TRACKING_RPG_CD = 10f;

	public static float TRACKING_RPG_SECOND_WAVE_TIME = 3f;

	public static int[] TRACKING_RPG_NUM = new int[3] { 6, 6, 6 };

	public static float TRACKING_RPG_RADIUS = 4f;

	public static float TRACKING_RPG_STATE2_DURATION = 2f;

	public static float TRACKING_RPG_MOVE_SPEED = 10f;

	public static float TRACKING_RPG_ROTATE_SPEED = 2f;

	public static int TRACKING_RPG_HP = 100;

	public static int TRACKING_RPG_HP_HERO = 5000;

	public static float TRACKING_RPG_DURATION = 8f;

	public static float TRACKING_RPG_BOSS_ROTATE_SPEED = 5f;

	public static float EXPLOSION_SPAWN_MOVE_TO_START_SPEED = 25f;

	public static float EXPLOSION_SPAWN_MOVE_UP_TIME = 0.5f;

	public static float EXPLOSION_SPAWN_MOVE_UP_SPEED = 4f;

	public static float EXPLOSION_SPAWN_MOVE_UP_ACC = 5f;

	public static float EXPLOSION_SPAWN_MOVE_UP_HEIGHT = 5f;

	public static float EXPLOSION_SPAWN_FLY_MAX_RADIUS = 35f;

	public static float EXPLOSION_SPAWN_FLY_BACK_RADIUS = 25f;

	public static float EXPLOSION_SPAWN_FLY_SPEED = 30f;

	public static float EXPLOSION_SPAWN_FLY_ACC = 30f;

	public static float EXPLOSION_SPAWN_FLY_ROTATE_ANGLE = 5f;

	public static float EXPLOSION_SPAWN_MOVE_DOWN_SPEED = 20f;

	public static float[] EXPLOSION_SPAWN_BOMB_INTERVAL_DISTANCE = new float[3] { 25f, 20f, 20f };

	public static int EXPLOSION_SPAWN_BOMB_HP = 100;

	public static int EXPLOSION_SPAWN_BOMB_HP_HERO = 1000;

	public static float EXPLOSION_SPAWN_BOMB_SPEED = 20f;

	public static float EXPLOSION_SPAWN_BOMB_RADIUS = 5f;

	public static float EXPLOSION_SPAWN_BOMB_DURATION = 10f;

	public static float EXPLOSION_SPAWN_BOMB_KNOCK_SPEED = 0.1f;

	public static float EXPLOSION_SPAWN_OUTSIDE_MOVE_TIME = 1.5f;

	public static float[] RANGE_ATTACK_CHANCE1 = new float[3] { 0.2f, 0.2f, 0.2f };

	public static float[] RANGE_ATTACK_CHANCE2 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float DASH_ROTATE_SPEED = 3f;

	public static sDashAttack DASH_ATTACK = new sDashAttack
	{
		DASH_LOCK_DISTANCE = 1000f,
		DASH_OVER_DISTANCE = 5f,
		DASH_STOP_DISTANCE = 25f,
		DASH_KNOCK_SPEED = 0.15f,
		DASH_SPEED = new float[3] { 50f, 50f, 50f },
		DASH_SPEED1 = new float[3] { 50f, 50f, 50f },
		DASH_HIT_RADIUS = 5f
	};

	public static int ASSIST_FUNNEL_HP = 100;

	public static int ASSIST_FUNNEL_HP_HERO = 500;

	public static float ASSIST_FUNNEL_MOVE_MAX_DISTANCE = 3f;

	public static float[] ASSIST_FUNNEL_MOVE_SPEED = new float[2] { 30f, 100f };

	public static float ASSIST_FUNNEL_RESPAWN_TIME = 20f;

	public static float ASSIST_FUNNEL_ATTACK_INTERVAL = 3f;

	public static float ASSIST_FUNNEL_LASER_SPEED = 40f;

	public static CrimsonQueenParameter GetInstance()
	{
		return me;
	}
}
