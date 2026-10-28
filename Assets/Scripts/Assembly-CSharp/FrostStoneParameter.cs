using UnityEngine;

public class FrostStoneParameter : BossParameter
{
	public enum EDamageType
	{
		TOUCH = 0,
		MELEE_ATTACK01 = 1,
		MELEE_ATTACK02 = 2,
		SMALL_SNOW_BALL = 3,
		BIG_SNOW_BALL = 4,
		ICE_SPIKE = 5,
		ICE_SPIKE2 = 6,
		DASH = 7,
		ICE_GROUND = 8,
		JUMP_ATTACK = 9
	}

	public static FrostStoneParameter me = new FrostStoneParameter();

	public static float MELEE_ATTACK_RADIUS = 8f;

	public static float RANGE_ATTACK_RADIUS = 30f;

	public static float TURN_SPEED = 0.06f;

	public static float[] RUN_SPEED = new float[3] { 9f, 11f, 12f };

	public static float TOUCH_RADIUS = 3.5f;

	public static float TOUCH_KNOCK_SPEED = 0.06f;

	public static float[] IDLE_TIME = new float[3] { 0.8f, 0.5f, 0.4f };

	public static float HP_PERCENTAGE_TO_RAGE = 0.55f;

	public static float HP_PERCENTAGE_PER_HIT = 0.15f;

	public static float MINIMUM_DISTANCE_BEFORE_WALL = 4f;

	public static float ICE_ARMOR_SHOW_TIME = 0.5f;

	public static float ICE_ARMOR_DECREASE_DAMAGE = 0.5f;

	public static float ICE_ARMOR_REFRESH_ARMOR_TIME = 20f;

	public static float ICE_ARMOR_DROP_HP = 0.05f;

	public static float DESTRUCTBLE_DISTANCE = 4f;

	public static float ICE_GROUND_VFX_DISTANCE_SQR = 64f;

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

	public static float[] MELEE_ATTACK_CHANCE1 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] MELEE_ATTACK_CHANCE2 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static Vector2 MELEE_ATTACK_CHECK_TIME1 = new Vector2(0.37f, 0.8f);

	public static float MELEE_ATTACK_KNOCK_SPEED1 = 0.1f;

	public static Vector2 MELEE_ATTACK_CHECK_TIME2 = new Vector2(0.17f, 0.7f);

	public static float MELEE_ATTACK_KNOCK_SPEED2 = 0.1f;

	public static float[] MIDDLE_ATTACK_CHANCE1 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] MIDDLE_ATTACK_CHANCE2 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] MIDDLE_ATTACK_CHANCE3 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] MIDDLE_ATTACK_CHANCE4 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] MIDDLE_ATTACK_CHANCE5 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] MIDDLE_ATTACK_CHANCE6 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float ICE_SPIKE_VFX_APPEAR_TIME = 0.25f;

	public static float ICE_SPIKE_VFX_DURATION = 4f;

	public static float ICE_SPIKE_APPEAR_TIME = 0.55f;

	public static float ICE_SPIKE_KNOCK_SPEED = 0.1f;

	public static float[] ICE_SPIKE_RANDOM_OFFSET = new float[5] { 0f, 7.5f, 10f, 13f, 15f };

	public static float ICE_SPIKE_DURATION = 10f;

	public static float SMALL_SNOW_BALL_HEIGHT = 10f;

	public static float SMALL_SNOW_BALL_TRIGGER_TIME = 43f / 115f;

	public static int SMALL_SNOW_BALL_NUM = 5;

	public static float SMALL_SNOW_BALL_BEHIND_PLAYER_DISTANCE = 2f;

	public static float SMALL_SNOW_BALL_HORIZONTAL_SPEED = 30f;

	public static Vector2 SAMLL_SNOW_BALL_RADIUS = new Vector2(3f, 10f);

	public static float BIG_SNOW_BALL_HEIGHT = 25f;

	public static float BIG_SNOW_BALL_TRIGGER_TIME = 0.57f;

	public static float BIG_SNOW_BALL_TIME = 0.8f;

	public static float BIG_SNOW_BALL_EXPLODE_RADIUS = 4.5f;

	public static float BIG_SNOW_BALL_ROTATE_SPEED = 0.5f;

	public static float JUMP_ATTACK_HEIGHT = 30f;

	public static float JUMP_ATTACK_TIME = 0.85f;

	public static float JUMP_ATTACK_HIT_RADIUS = 6.5f;

	public static float JUMP_ATTACK_KNOCK_SPEED = 0.1f;

	public static int JUMP_ATTACK_BALL_NUM = 8;

	public static Vector2 JUMP_ATTACK_BALL_RADIUS = new Vector2(5f, 15f);

	public static float JUMP_ATTACK_ROTATE_SPEED = 0.5f;

	public static int RANDOM_DASH_TIMES = 5;

	public static int RANDOM_DASH_QUIT_HIT_NUM = 1;

	public static float[] RANDOM_DASH_MAX_SPEED = new float[2] { 50f, 70f };

	public static float RANDOM_DASH_ACCELERATE = 50f;

	public static float RANDOM_DASH_CHANGE_DIR_DISTANCE = 5f;

	public static float RANDOM_DASH_DELTA_ANGLE = 30f;

	public static float[] RANGE_ATTACK_CHANCE1 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] RANGE_ATTACK_CHANCE2 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static sDashAttack DASH_ATTACK = new sDashAttack
	{
		DASH_LOCK_DISTANCE = 24f,
		DASH_OVER_DISTANCE = 5f,
		DASH_STOP_DISTANCE = 2f,
		DASH_KNOCK_SPEED = 0.15f,
		DASH_SPEED = new float[3] { 30f, 30f, 30f },
		DASH_SPEED1 = new float[3] { 30f, 30f, 30f },
		DASH_HIT_RADIUS = 5f
	};

	public static float[] FROST_STONE_PLAYER_BOSS_ANIMATION_SPEED = new float[6] { 1.2f, 2f, 2.2f, 2f, 2f, 1f };

	public static FrostStoneParameter GetInstance()
	{
		return me;
	}
}
