using System;
using UnityEngine;

public class DeadEyeParameter : BossParameter
{
	public enum EDamageType
	{
		TOUCH = 0,
		MELEE_ATTACK01 = 1,
		MELEE_ATTACK02 = 2,
		X_RAY = 3,
		_4_RAY = 4,
		SHOULDER_RAY = 5,
		ROCK_ATTACK = 6,
		DASH = 7,
		LEAP_ATTACK = 8,
		ROOF_X_RAY = 9,
		ROOF_4_RAY = 10,
		CONTINUE_RAY = 11,
		NET_RAY = 12,
		DOWN_ATTACK = 13
	}

	public static DeadEyeParameter me = new DeadEyeParameter();

	public static float MELEE_ATTACK_RADIUS = 8f;

	public static float RANGE_ATTACK_RADIUS = 28f;

	public static float TURN_SPEED = 0.06f;

	public static float[] RUN_SPEED = new float[3] { 9f, 11f, 12f };

	public static float TOUCH_RADIUS = 3.5f;

	public static float TOUCH_KNOCK_SPEED = 0.06f;

	public static float[] IDLE_TIME = new float[3] { 0.8f, 0.5f, 0.4f };

	public static float[] ROOF_IDLE_TIME = new float[3] { 0.8f, 0.5f, 0.4f };

	public static float ROOF_IDLE_TIME_SHORT_CHANCE = 0.3f;

	public static float HP_PERCENTAGE_TO_RAGE = 0.4f;

	public static float HP_PERCENTAGE_PER_HIT = 0.15f;

	public static float CHANGE_TO_ROOF_HP = 0.75f;

	public static float CHANGE_TO_ROOF_INTERVAL = 25f;

	public static float CHANGE_TO_GROUND_INTERVAL = 25f;

	public static float ROOF_MIN_DURATION = 10f;

	public static float ROOF_UP_TIME = 0.3846154f;

	public static float ROOF_UP_SPEED = 30f;

	public static float ROOF_UP_DECELERATE = 10f;

	public static float ROOF_UP_EXTRA_SPEED = 20f;

	public static float ROOF_DOWN_TIME = 0.5f;

	public static float ROOF_DOWN_SPEED = 55f;

	public static float ROOF_RUN_SPEED = 15f;

	public static float CATCH_BETWEEN_LASER_ATTACK_CHANCE = 0.6f;

	public static float CATCH_BETWEEN_LASER_ATTACK_TIME = 0.7f;

	public static float ROOF_RUN_BACK_CHANCE = 1f;

	public static float ROOF_RUN_BACK_TIME = 0.8f;

	public static float ROOF_RUN_BACK_SPEED = 8f;

	public static float[] MELEE_ATTACK_CHANCE1 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] MELEE_ATTACK_CHANCE2 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static Vector2 MELEE_ATTACK_CHECK_TIME1 = new Vector2(0.37f, 0.8f);

	public static Vector2 MELEE_ATTACK_CHECK_TIME2 = new Vector2(0.17f, 0.7f);

	public static float MELEE_ATTACK_KNOCK_SPEED1 = 0.1f;

	public static float MELEE_ATTACK_KNOCK_SPEED2 = 0.1f;

	public static Vector2 MELEE_ATTACK_KNOCK_CHECK_TIME2 = new Vector2(0.4f, 0.65f);

	public static float MELEE_ATTACK_2_INTERVAL = 0.3f;

	public static float MELEE_ATTACK_2_ROTATE_SPEED = 0.25f;

	public static float MELEE_ATTACK_2_MOVE_SPEED = 10f;

	public static Vector2 MELEE_ATTACK_2_MOVE_TIME = new Vector2(0.3f, 0.7f);

	public static float[] MIDDLE_ATTACK_CHANCE1 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] MIDDLE_ATTACK_CHANCE2 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] MIDDLE_ATTACK_CHANCE3 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] MIDDLE_ATTACK_CHANCE4 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] MIDDLE_ATTACK_CHANCE5 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float X_RAY_SPEED = 40f;

	public static Vector2 X_RAY_CHECK_TIME = new Vector2(11f / 27f, 17f / 27f);

	public static float DEAD_EYE_4_RAY_INTERVAL = 0.4f;

	public static float DEAD_EYE_4_RAY_DURATION = 1.6f;

	public static Vector2 DEAD_EYE_4_RAY_CHECK_TIME = new Vector2(0.2823f, 0.70588f);

	public static float DEAD_EYE_4_RAY_SPEED = 40f;

	public static float[] DEAD_EYE_4_RAY_SHOT_TIME = new float[6] { 0.192f, 0.272f, 0.352f, 0.512f, 0.592f, 0.672f };

	public static float DEAD_EYE_4_RAY_ROTATE_SPEED = 0.16f;

	public static Vector2 SHOULDER_RAY_CHECK_TIME = new Vector2(0.54744524f, 0.8f);

	public static float SHOULDER_RAY_ROTATE_SPEED = 0.25f;

	public static float SHOULDER_RAY_VFX_TIME = 0.54744524f;

	public static float SHOULDER_RAY_PREPARE_VFX_SHOW_TIME = 0.4f;

	public static float[] ROCK_ATTACK_TIME = new float[5] { 1.5f, 1.75f, 2f, 2.25f, 2.5f };

	public static float[] ROCK_ATTACK_RANDOM_OFFSET = new float[5] { 0f, 7.5f, 10f, 12.5f, 15f };

	public static float ShowShadowTime = 0.5f;

	public static float[] RANGE_ATTACK_CHANCE1 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] RANGE_ATTACK_CHANCE2 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static sDashAttack DASH_ATTACK = new sDashAttack
	{
		DASH_LOCK_DISTANCE = 24f,
		DASH_OVER_DISTANCE = 5f,
		DASH_STOP_DISTANCE = 16f,
		DASH_KNOCK_SPEED = 0.15f,
		DASH_SPEED = new float[3] { 15f, 16f, 17f },
		DASH_SPEED1 = new float[3] { 16f, 17f, 18f },
		DASH_HIT_RADIUS = 6f,
		DASH_SECTOR_RADIUS = 7f,
		DASH_SECTOR_ANGLE = 30f
	};

	public static float MINIMUM_DISTANCE_BEFORE_WALL = 2f;

	public static float LEAP_TIME = 0.5f;

	public static float LEAP_MAXIMUM_SPEED = 1.3f;

	public static float LEAP_MINIMUM_SPEED = 0.7f;

	public static float LEAP_START_JUMP_TIME = 0.3f;

	public static float LEAP_CHECK_RADIUS = 5f;

	public static float LEAP_SECTOR_RADIUS = 7f;

	public static float LEAP_SECTOR_ANGLE = 60f;

	public static Vector2 LEAP_ATTACK_CHECK_TIME = new Vector2(0.1f, 1f);

	public static float LEAP_KNOCK_SPEED = 0.15f;

	public static float LEAP_OVER_DISTANCE = 3f;

	public static float LEAP_STOP_MOVE_TIME = 0.7f;

	public static float LEAP_SOUND_TIME = 0.2f;

	public static sStepBack STEP_BACK = new sStepBack
	{
		TRIGGER_STEP_BACK_MELEE_ATTACK_HIT_NUM = 2,
		TRIGGER_STEP_BACK_MELEE_ATTACK_NUM = 4,
		MINIMUM_DISTANCE_BEFORE_WALL = 2f,
		BACK_DISTANCE = 16f,
		BACK_MAXIMUM_SPEED = 6f,
		BACK_MINIMUM_SPEED = 0.8f,
		BACK_TIME = 0.16f
	};

	public static float[] ROOF_ATTACK_CHANCE1 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] ROOF_ATTACK_CHANCE2 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] ROOF_ATTACK_CHANCE3 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] ROOF_ATTACK_CHANCE4 = new float[3] { 0.1f, 0.1f, 0.1f };

	public static float[] ROOF_ATTACK_CHANCE5 = new float[3] { 0f, 0f, 0.1f };

	public static float ROOF_X_RAY_SPEED = 40f;

	public static Vector2 ROOF_X_RAY_CHECK_TIME = new Vector2(0.5f, 0.7f);

	public static float ROOF_X_RAY_OFFSET = -2f;

	public static int DEAD_EYE_ROOF_4_RAY_SPEED = 20;

	public static Vector2 DEAD_EYE_ROOF_4_RAY_CHECK_TIME = new Vector2(0.3f, 0.6875f);

	public static float DEAD_EYE_CONTINUE_RAY_SPAWN_DISTANCE = 10f;

	public static int DEAD_EYE_CONTINUE_RAY_SPEED = 60;

	public static float DEAD_EYE_CONTINUE_RAY_INTERVAL = 1f;

	public static float DEAD_EYE_CONTINUE_RAY_DURATION = 0.5f;

	public static float[] DEAD_EYE_CONTINUE_RAY_SHOT_TIME = new float[6]
	{
		0.112676054f,
		250f / (339f * (float)Math.PI),
		0.33802816f,
		0.48356807f,
		0.6103286f,
		0.7370892f
	};

	public static float[] DEAD_EYE_CONTINUE_RAY_SHOW_PREPARE_VFX_TIME = new float[2] { 0.5164319f, 0.64319247f };

	public static float DEAD_EYE_NET_RAY_SPAWN_DISTANCE = 5f;

	public static int DEAD_EYE_NET_RAY_SPEED = 100;

	public static Vector2 DEAD_EYE_NET_RAY_CHECK_TIME = new Vector2(0.21276596f, 0.85106385f);

	public static float DEAD_EYE_NET_RAY_LASER_DISAPPEAR_TIME = 0.7446808f;

	public static float DEAD_EYE_DOWN_ATTACK_HIT_RADIUS = 5f;

	public static float DEAD_EYE_DOWN_ATTACK_KNOCK_DOWN_SPEED = 0.2f;

	public static float[] DEAD_EYE_PLAYER_BOSS_ANIMATION_SPEED = new float[5] { 1.3f, 3.3f, 2f, 1.2f, 1f };

	public static float PLAYER_BOSS_4_RAY_ROTATE_SPEED = 1.99f;

	public static float PLAYER_BOSS_SHOULDER_RAY_ROTATE_SPEED = 0.99f;

	public static DeadEyeParameter GetInstance()
	{
		return me;
	}
}
