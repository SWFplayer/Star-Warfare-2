using UnityEngine;

public class CerberliskParameter : BossParameter
{
	public enum EDamageType
	{
		TOUCH = 0,
		MELEE_ATTACK01 = 1,
		MELEE_ATTACK02 = 2,
		THREE_HIT_ATTACK = 3,
		THREE_FIRE_BALL = 4,
		BIG_FIRE_BALL = 5,
		BREATHE_IN_ATTACK = 6,
		DASH = 7,
		FIRE_GROUND = 8,
		CYCLONE = 9,
		MOUTH = 10
	}

	public static CerberliskParameter me = new CerberliskParameter();

	public static float MELEE_ATTACK_RADIUS = 8f;

	public static float RANGE_ATTACK_RADIUS = 30f;

	public static float TURN_SPEED = 0.06f;

	public static float[] RUN_SPEED = new float[3] { 10f, 12f, 13f };

	public static float TOUCH_RADIUS = 4f;

	public static float TOUCH_KNOCK_SPEED = 0.06f;

	public static float[] IDLE_TIME = new float[3] { 0.5f, 0.3f, 0.2f };

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

	public static float[] MELEE_ATTACK_CHANCE1 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] MELEE_ATTACK_CHANCE2 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static Vector2 MELEE_ATTACK_CHECK_TIME1 = new Vector2(0.37f, 0.8f);

	public static float MELEE_ATTACK_KNOCK_SPEED1 = 0.1f;

	public static Vector2 MELEE_ATTACK_CHECK_TIME2 = new Vector2(0.17f, 0.7f);

	public static float MELEE_ATTACK_KNOCK_SPEED2 = 0.1f;

	public static float[] MIDDLE_ATTACK_CHANCE1 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] MIDDLE_ATTACK_CHANCE2 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] MIDDLE_ATTACK_CHANCE3 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] MIDDLE_ATTACK_CHANCE4 = new float[3] { 0.2f, 0.3f, 0.3f };

	public static float[] MIDDLE_ATTACK_CHANCE5 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static Vector2 THREE_ATTACK_CHECK_TIME1 = new Vector2(0f, 0.8f);

	public static float THREE_ATTACK_RADIUS1 = 6f;

	public static float THREE_ATTACK_ANGLE1 = 60f;

	public static float THREE_ATTACK_KNOCK_SPEED1 = 0.1f;

	public static float THREE_ATTACK_ROTATE_TIME1 = 0.2f;

	public static float THREE_ATTACK_ROTATE_SPEED1 = 6f;

	public static Vector2 THREE_ATTACK_CHECK_TIME2 = new Vector2(0f, 0.8f);

	public static float THREE_ATTACK_RADIUS2 = 6f;

	public static float THREE_ATTACK_ANGLE2 = 60f;

	public static float THREE_ATTACK_KNOCK_SPEED2 = 0.1f;

	public static float THREE_ATTACK_ROTATE_TIME2 = 0.2f;

	public static float THREE_ATTACK_ROTATE_SPEED2 = 6f;

	public static Vector2 THREE_ATTACK_CHECK_TIME3 = new Vector2(0f, 0.8f);

	public static float THREE_ATTACK_RADIUS3 = 6f;

	public static float THREE_ATTACK_ANGLE3 = 60f;

	public static float THREE_ATTACK_KNOCK_SPEED3 = 0.1f;

	public static float THREE_ATTACK_ROTATE_TIME3 = 0.2f;

	public static float THREE_ATTACK_ROTATE_SPEED3 = 6f;

	public static float THREE_FIRE_BALL_SPEED = 20f;

	public static float[] THREE_FIRE_BALL_TIME = new float[3] { 0.36f, 0.55f, 0.8f };

	public static int THREE_FIRE_BALL_TIMES = 3;

	public static int THREE_FIRE_BALL_NUM = 7;

	public static float THREE_FIRE_BALL_ANGLE = 10f;

	public static float THREE_FIRE_BALL_ROTATE_SPEED = 5f;

	public static float BREATHE_FIRE_BALL_ROTATE_SPEED = 3f;

	public static Vector2 BREATHE_FIRE_BALL_TIME = new Vector2(0.1f, 0.8f);

	public static float[] BREATHE_FIRE_BALL_FIRE_TIME = new float[3] { 0.45f, 0.63f, 0.83f };

	public static float BIG_FIRE_BALL_TRIGGER_TIME = 0.55f;

	public static float BIG_FIRE_BALL_ROTATE_SPEED = 0.5f;

	public static float BIG_FIRE_BALL_SPEED = 35f;

	public static float BIG_FIRE_BALL_EXPLODE_RADIUS = 5f;

	public static float BIG_FIRE_BALL_DOT_DURATION = 4f;

	public static Vector2 BREATHE_IN_TIME = new Vector2(0.1f, 0.65f);

	public static Vector2 BREATHE_IN_ATTACK_TIME = new Vector2(0.56f, 0.85f);

	public static Vector2 BREATHE_IN_ROTATE_TIME = new Vector2(0.1f, 0.8f);

	public static float BREATHE_IN_ROTATE_SPEED = 5f;

	public static float BREATHE_IN_ATTACK_RADIUS = 8f;

	public static float BREATHE_IN_ATTACK_ANGLE = 60f;

	public static float BREATHE_IN_ATTACK_KNOCK_SPEED = 0.1f;

	public static float BREATHE_IN_RADIUS_SQR = 10000f;

	public static float BREATHE_IN_ANGLE = 170f;

	public static float BREATHE_IN_SPEED = 10f;

	public static float MOUTH_KNOCK_SPEED = 0.1f;

	public static float CYCLONE_FIRE_TIME = 0.27f;

	public static float CYCLONE_ROTATE_TIME = 3f;

	public static float CYCLONE_OFFSET = -3f;

	public static float CYCLONE_DURATION = 5f;

	public static float CYCLONE_MOVE_SPEED = 7f;

	public static float CYCLONE_HIT_MOVE_SPEED = 6f;

	public static float CYCLONE_KNOCK_SPEED = 0.1f;

	public static float CYCLONE_FIRE_BALL_SPEED = 40f;

	public static float CYCLONE_START_BREATHE_OUT_TIME = 0.35f;

	public static float[] RANGE_ATTACK_CHANCE1 = new float[3] { 0.2f, 0.2f, 0.2f };

	public static float[] RANGE_ATTACK_CHANCE2 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float DASH_ROTATE_SPEED = 5f;

	public static sDashAttack DASH_ATTACK = new sDashAttack
	{
		DASH_LOCK_DISTANCE = 30f,
		DASH_OVER_DISTANCE = 5f,
		DASH_STOP_DISTANCE = 25f,
		DASH_KNOCK_SPEED = 0.15f,
		DASH_SPEED = new float[3] { 35f, 35f, 35f },
		DASH_SPEED1 = new float[3] { 35f, 35f, 35f },
		DASH_HIT_RADIUS = 5f
	};

	public static Vector2 PUSH_TIME = new Vector2(0.48f, 0.84f);

	public static float PUSH_RADIUS_SQR = 10000f;

	public static float PUSH_ANGLE = 170f;

	public static float PUSH_SPEED = 15f;

	public static float[] CERBERLISK_PLAYER_BOSS_ANIMATION_SPEED = new float[7] { 1.5f, 1.3f, 1.2f, 1f, 1f, 1f, 1.5f };

	public static CerberliskParameter GetInstance()
	{
		return me;
	}
}
