using System.Collections.Generic;
using UnityEngine;

public class Global
{
	public const string HELP_EMAIL = "ios@ifreyr.com";

	public const string APP_ID = "888211509";

	public const int CURRENT_VERSION_MISSION_MITHRIL_LIMIT = 5000;

	public const int IAP_PACKAGE_MAX_INDEX = 6;

	public const int AVATAR_PART_HEAD = 0;

	public const int AVATAR_PART_BODY = 1;

	public const int AVATAR_PART_ARM = 2;

	public const int AVATAR_PART_FOOT = 3;

	public const int AVATAR_PART_BAG = 4;

	public const int TOTAL_WEAPON_NUM = 33;

	public const int TOTAL_ITEM_NUM = 26;

	public const int TOTAL_FLASK_NUM = 4;

	public const byte STAGE_STATE_LOCK = 0;

	public const byte STAGE_STATE_UNLOCK = 1;

	public const byte EQUIP_STATE_LOCKED = 0;

	public const byte EQUIP_STATE_UNLOCKED = 15;

	public const int TEAM_SIZE = 6;

	public const float MIN_BLOOD_EFFECT_INTERVAL = 0.3f;

	public const float VS_WAIT_REBIRTH_TIME = 15f;

	public const string SCREEN_SHOT_FILENAME = "tempscreens.png";

	public const float CAMERA_RAYCAST_MOVE_DISTANCE = 1.8f;

	public const float DEFENCE_UP_VALUE = 0.85f;

	public const float ANDROMEDA_DEFENCE_UP_VALUE = 0.3f;

	public const int FLAG_TIME = 40;

	public const float STEAL_HEALTH_FACTOR = 1f;

	public const float DAMAGE_FACTOR_WHEN_STEAL_HEALTH = 0.65f;

	public const float SPEED_UP_WHEN_GOT_HIT_BUFF_DURATION = 2f;

	public const int MIN_CTF_FFA_RESUME_TIME = 10;

	public const float THUNDER_ALL_UP_VALUE = 1.5f;

	public const float ANIMATION_BLEND_TIME_ON_VEHICLE = 0.5f;

	public const int MAX_CARRY_WEAPON_COUNT = 3;

	public const int MAX_CARRY_ITEM_COUNT = 3;

	public const int MAX_PVP_AWARD_SKILL_COUNT = 3;

	public const int PVP_REBIRTH_MITHRIL_COST = 5;

	public const int PVE_REBIRTH_MITHRIL_COST = 15;

	public const float PVP_FIRST_REBIRTH_TIME = 30f;

	public const int FFA_WIN_VALUE = 1000;

	public const int TDM_WIN_VALUE = 1000;

	public const int CTF_TDM_WIN_VALUE = 500;

	public const int KOTH_WIN_VALUE = 1500;

	public const int DOMINATION_WIN_VALUE = 4000;

	public const float LIGHT_KNOCKED_SPEED = 0.05f;

	public const int TOTAL_SWVOID_MATERIAL_TYPE = 28;

	public const int DEFAULT_ELITE_ENEMY_PROBABILITY = 5;

	public const float AWAY_FROM_BATTLE_TIME = 5f;

	public const float TIME_CONTROLLER_LASTS = 10f;

	public const float TIME_CONTROLLER_RADIUS = 20f;

	public const float REVENGE_BOMB_RADIUS = 7f;

	public const float REVENGE_SLOW_BOMB_RADIUS = 8f;

	public const float REVENGE_FLASH_RADIUS = 20f;

	public const float REVENGE_BOMB_TIME = 1f;

	public const float REVENGE_FLASH_DURING = 5f;

	public const float ALLY_DEAD_RADIUS = 20f;

	public const int TRIPLE_DEAD = 3;

	public const float MISSLE_RADIUS = 10f;

	public const float MISSLE_SLOW_RADIUS = 20f;

	public const float MISSLE_TIME = 1.5f;

	public const float NUCLEAR_TIME = 5f;

	public const float TRIPLE_MISSLE_FIRST = 3f;

	public const float TRIPLE_MISSLE_INTERVAL = 6f;

	public const float ENEMY_NEARBY_RADIUS = 10f;

	public const float PLAYER_DEFAULT_SPEED = 6f;

	public const float ITEM_HIDE_TIME = 5f;

	public const float JOYSTICK_HIDE_TIME = 1f;

	public const float JOYSTICK_HIDE_DURING_TIME = 2f;

	public const int WAITING_RESTART_TIMER = 15;

	public const int TUTORIAL_NUMBER = 18;

	public const int DEFAULT_FRIEND_ACCOUNT_ID = 24;

	public const int TOUCH5_SCREEN_WIDTH = 1136;

	public const int PICK_UP_NUMBER = 17;

	public const float RESPAWN_INVINCIBLE_TIME = 2f;

	public const int MAX_PVP_RANK_WHEN_NOT_REGISTER = 5;

	public const int PVP_TOP_LEVEL = 46;

	public const float HP_PERCENT_TO_POPUP_ITEM_BAR = 0.3f;

	public const int VOID_ACHIEVEMENT_LEVEL = 4;

	public const int VOID_ACHIEVEMENT_COUNT = 4;

	public const int VOID_ACHIEVEMENT_CANCEL_I = 1;

	public const int VOID_ACHIEVEMENT_CANCEL_II = 10;

	public const int VOID_ACHIEVEMENT_CANCEL_III = 1000;

	public const int WEAPON_WIN_WITH_I = 10;

	public const int WEAPON_WIN_WITH_II = 50;

	public const int WEAPON_WIN_WITH_III = 100;

	public const int WEAPON_WIN_WITH_IV = 200;

	public const int ENEMY_ACHIEVEMENT_KILL_I = 1000;

	public const int ENEMY_ACHIEVEMENT_KILL_II = 100000;

	public const int WEAPON_ACHIEVEMENT_LEVEL = 8;

	public const int ACHIEVEMENT_FULL_ARMOR = 5;

	public const int ACHIEVEMENT_COMBAT_KILLS_I = 1;

	public const int ACHIEVEMENT_COMBAT_KILLS_II = 100;

	public const int ACHIEVEMENT_COMBAT_KILLS_III = 1000;

	public const int ACHIEVEMENT_COMBAT_KILLS_IV = 10000;

	public const int ACHIEVEMENT_COMBAT_KILLS_V = 100000;

	public const int ACHIEVEMENT_COMBAT_KILLS_VI = 1000000;

	public const int ACHIEVEMENT_DOUBLE_KILL = 100;

	public const int ACHIEVEMENT_TRIPLE_KILL = 100;

	public const int ACHIEVEMENT_GOOD_WEAPON = 24;

	public const int ACHIEVEMENT_GOOD_ARMOR = 8;

	public const int ACHIEVEMENT_GOOD_BAG = 6;

	public const int ACHIEVEMENT_GOOD_VOID = 4;

	public const float ACHIVEMENT_PICK_UP_INTERVAL = 15f;

	public const int ACHIVEMENT_PICK_UP_COUNT = 4;

	public const int ACHIVEMENT_DIFFPICKUP_COUNT_I = 1;

	public const int ACHIVEMENT_DIFFPICKUP_COUNT_II = 10;

	public const int ACHIEVEMENT_VS_WINS = 200;

	public const int ACHIEVEMENT_SAINT_WARROR = 3;

	public const string BossCoopLevelName = "Campaign01_boss_test";

	public const int PLAY_TIME_WITHOUT_RATE = 10;

	public const int TEXTURE_MAXSIZE = 256;

	public static bool ENABLE_DEBUG_UI = false;

	public static string QUALITY_LEVEL_1 = "L1";

	public static int QUALITY_LEVEL = 0;

	public static float MAX_LOGIN_WAITING_TIME = 15f;

	public static string AVATAR_PATH = "Character/Avatar";

	public static string BONE_NAME = "Bone";

	public static string HEAD_NAME = "Head";

	public static string BODY_NAME = "Body";

	public static string HAND_NAME = "Hand";

	public static string FOOT_NAME = "Foot";

	public static string BAG_PATH = "Character/Bag";

	public static string BAG_PREFIX = "Bag";

	public static string WEAPON_PATH = "Character/Weapon";

	public static string WEAPON_PREFIX = "weapon";

	public static string GAME_APP_SCRIPT_OBJECT_NAME = "GameAppScript";

	public static string PACKAGE_UNLOCK_VERSION_STRING = "99";

	public static string TRIAL_UNLOCK_VERSION_STRING = "98";

	public static string UPGRATE_PUSH_VERSION_STRING = "97";

	public static int AVATAR_PART_NUM = 5;

	public static int ITEM_MAX_COUNT = 99;

	public static int TOTAL_ARMOR_NUM = 12;

	public static int TOTAL_ARMOR_NUM_IN_EXCEL = 12;

	public static int TOTAL_ARMOR_HEAD_NUM = 12;

	public static int TOTAL_ARMOR_BODY_NUM = 12;

	public static int TOTAL_ARMOR_ARM_NUM = 12;

	public static int TOTAL_ARMOR_FOOT_NUM = 12;

	public static int TOTAL_ARMOR_BAG_NUM = 11;

	public static int CAMERA_ZOOM_SPEED = 10;

	public static float CAMERA_ZOOM_SPEED_CANNON = 5f;

	public static float CAMERA_ZOOM_SPEED_CARRIER = 5f;

	public static int FLOORHEIGHT = 0;

	public static int TOTAL_VS_STAGE = 7;

	public static int TOTAL_BOSS_STAGE = 5;

	public static int TOTAL_SURVIVAL_STAGE = 7;

	public static int SURVIVAL_SUB_STAGE_ID = 0;

	public static int BOSS_SUB_STAGE_ID = 6;

	public static int BOSS_FINAL_SUB_STAGE_ID = 7;

	public static int TOTAL_STAGE = 5;

	public static int TOTAL_ACTIVATED_STAGE = 5;

	public static int IRREGULAR_STAGE_ID = 3;

	public static int TOTAL_SUB_STAGE = 8;

	public static int MISSION_SIZE = 5;

	public static int TOTAL_SOLO_BOSS_NUM = 5;

	public static int TOTAL_COOP_BOSS_NUM = 5;

	public static byte STORAGE_MAX_PANEL = 6;

	public static byte STORAGE_MAX_NUM = 54;

	public static int MAX_CASH = 999999999;

	public static int MAX_ENEGY = 999999999;

	public static int MAX_MITHRIL = 999999999;

	public static int MAX_EXP = 999999999;

	public static int MAX_LEVEL_WEAPONW = 8;

	public static int TOTAL_IAP_CATEGORY_NUM = 2;

	public static int TOTAL_IAP_PURCHASE = 9;

	public static int TOTAL_IAP_EXCHANGE = 6;

	public static int IAP_CATEGORY_PURCHASE = 0;

	public static int IAP_CATEGORY_EXCHANGE = 1;

	public static int[] TOTAL_IAP_CATEGORY = new int[2] { 9, 6 };

	public static int IAP_UNLOCK_LEVEL_WEAPON_INDEX = 3;

	public static int SURVIVAL_DIFFICULTY_UP_EVERY_ROUND = 13;

	public static int VOID_TYPE_NUM = 5;

	public static int SURVIVAL_DIFFICULTY_UP_ENEMY_NUM = 45;

	public static int[] ENEMY_SPAWN_ID_OFFSET = new int[6] { 0, 10000, 20000, 25000, 30000, 31000 };

	public static float[] KillBossTime = new float[4];

	public static int ACHIEVEMENT_PERK_NUM = 5;

	public static int ACHIVEMENT_PVPLEVELI = 10;

	public static int ACHIVEMENT_PVPLEVELII = 20;

	public static int ACHIVEMENT_PVPLEVELIII = 30;

	public static int ACHIVEMENT_PVPLEVELIV = 40;

	public static int ACHIVEMENT_PVPLEVELV = 50;

	public static int ACHIEVEMENT_FFACOMBOWINS = 3;

	public static int ACHIEVEMENT_TDMKILLWINS = 12;

	public static int ACHIEVEMENT_TDMGOODASSIST = 15;

	public static int ACHIEVEMENT_TDMGOODASSISTWINNUM = 12;

	public static int ACHIEVEMENT_CTPSPECIALWINNUM = 10;

	public static int ACHIEVEMENT_C3PSPECIALWINNUM = 10;

	public static int ACHIEVEMENT_FFATOPONE = 12;

	public static int ACHIEVEMENT_KILLFLAGER = 500;

	public static int ACHIEVEMENT_FLAGKILL = 500;

	public static int ACHIEVEMENT_CTFRELAY = 4;

	public static float ACHIEVEMENT_CTF250WITHFLAG = 250f;

	public static int ACHIVEMENT_SETWINCOUNT1 = 4;

	public static int ACHIVEMENT_SETWINCOUNT2 = 10;

	public static int ACHIVEMENT_SETWINCOUNT3 = 20;

	public static int ACHIVEMENT_SETWINCOUNT4 = 30;

	public static int TOTAL_SET = 36;

	public static int ACHIEVEMENT_SPIKES = 25;

	public static Color[] HeadFrameColor = new Color[8]
	{
		new Color(0f, 248f / 255f, 22f / 255f, 1f),
		new Color(0f, 203f / 255f, 1f, 1f),
		new Color(252f / 255f, 1f, 0f, 1f),
		new Color(1f, 186f / 255f, 0f, 1f),
		new Color(1f, 1f / 3f, 0f, 1f),
		new Color(1f, 0f, 2f / 255f, 1f),
		new Color(1f, 0f, 125f / 255f, 1f),
		new Color(212f / 255f, 0f, 1f, 1f)
	};

	public static Color32[] COLORSET_1 = new Color32[2]
	{
		new Color32(byte.MaxValue, 200, 52, byte.MaxValue),
		new Color32(byte.MaxValue, 215, 0, byte.MaxValue)
	};

	public static Color32[] COLORSET_2 = new Color32[2]
	{
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue),
		new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue)
	};

	public static Color32[] COLORSET_3 = new Color32[2]
	{
		new Color32(0, 0, 0, byte.MaxValue),
		new Color32(0, 0, 0, byte.MaxValue)
	};

	public static Color32[] COLORSET_4 = new Color32[2]
	{
		new Color32(185, byte.MaxValue, 103, byte.MaxValue),
		new Color32(185, byte.MaxValue, 103, byte.MaxValue)
	};

	public static Color32[] COLORSET_5 = new Color32[2]
	{
		new Color32(239, 201, byte.MaxValue, byte.MaxValue),
		new Color32(239, 201, byte.MaxValue, byte.MaxValue)
	};

	public static Color32[] COLORSET_6 = new Color32[2]
	{
		new Color32(byte.MaxValue, 0, 0, byte.MaxValue),
		new Color32(byte.MaxValue, 0, 240, byte.MaxValue)
	};

	public static Color32[] COLORSET_7 = new Color32[2]
	{
		new Color32(216, 0, byte.MaxValue, byte.MaxValue),
		new Color32(0, 12, byte.MaxValue, byte.MaxValue)
	};

	public static Color32[] COLORSET_8 = new Color32[2]
	{
		new Color32(0, 114, byte.MaxValue, byte.MaxValue),
		new Color32(0, byte.MaxValue, 30, byte.MaxValue)
	};

	public static Color32[] COLORSET_9 = new Color32[2]
	{
		new Color32(96, byte.MaxValue, 0, byte.MaxValue),
		new Color32(235, 132, 0, byte.MaxValue)
	};

	public static bool ENABLE_IAP = true;

	public static float REFRESH_MISSION_INTERVAL = 30f;

	public static byte NORMAL_SENSITIVITY_LEVEL = 5;

	public static float SENSITIVITY_STEP = 1.3f;

	public static float MIN_SENSITIVITY = 0.2693f;

	public static List<int> MAX_PLAYER_SELECTIONS = new List<int> { 8, 10, 12 };

	public static List<int> MAX_COOP_PLAYERS = new List<int> { 3 };

	public static List<int> PVP_LEVEL_LIMIT_SELECTIONS = new List<int> { 1, 10, 20, 30, 40, 50 };

	public static int ROOKIE_PACKAGE_LOSE_PUSH_TIMES = 2;

	public static int ROOKIE_PACKAGE_AVATAR_INDEX = 1;

	public static int ROOKIE_PACKAGE_PUSH_STAGE = 1;

	public static int EXPERT_PACKAGE_LOSE_PUSH_TIMES = 2;

	public static int EXPERT_PACKAGE_AVATAR_INDEX = 4;

	public static int EXPERT_PACKAGE_PUSH_STAGE = 3;

	public static int PACKAGE_PUSH_SUB_STAGE = 5;

	public static int LOOT_CASH_STACK = 50;

	public static byte TRIAL_WEAPON_LEVEL = 4;

	public static float TRIAL_WEAPON_BUY_SECONDS_MAX = 86400f;

	public static float TRIAL_WEAPON_SALE_RATE = 0.8f;

	public static List<short> ETERNAL_TRIAL_PUSH_DISPLAY_ID_LIST = new List<short> { 20, 22, 23, 27, 31, 32 };

	public static List<short> ROOKIE_TRIAL_PUSH_DISPLAY_ID_LIST = new List<short> { 9 };

	public static List<int> WEAPON_UPGRADE_PUSH_UP_LEVELS = new List<int> { 3, 5, 7 };

	public static List<float> WEAPON_UPGRADE_PUSH_OFF = new List<float> { 0.7f, 0.55f, 0.4f };

	public static int WEAPON_UPGRADE_PUSH_MITHRIL_EXCHANGE_RATE = 1000;

	public static int WEAPON_UPGRADE_PUSH_INTERVAL_HOUR_MIN = 24;

	public static int WEAPON_UPGRADE_PUSH_INTERVAL_HOUR_MAX = 72;

	public static float PROJECTILE_GRAVITY = 16f;

	public static float MYTH_FIST_ANGLEV_FACTOR = 0.1f;

	public static float MYTH_FIST_ACC_BY_MOVE = 12f;

	public static float MYTH_FIST_INIT_DIS = 10f;

	public static float MYTH_FIST_DIS_FACTOR = 0.02f;

	public static int MACHINEGUN_MAX_STACK_NUM = 30;

	public static float PVP_KNOCKED_SPEED = 0.1f;
}
