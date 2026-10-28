public class Skill
{
	public SkillsType skillType;

	public float data;

	public static string GetSkillsTypeString(SkillsType type)
	{
		switch (type)
		{
		case SkillsType.HP_BOOTH:
			return "LOC_ATT_HP";
		case SkillsType.EXP_BOOTH:
			return "LOC_ATT_EXP_BONUS";
		case SkillsType.MONEY_BOOTH:
			return "LOC_ATT_GOLD_BONUS";
		case SkillsType.ALL_RES:
			return "LOC_ATT_R_ALL";
		case SkillsType.SPEED_BOOTH:
			return "LOC_ATT_MOVE_SPEED";
		case SkillsType.BULLET_DAMAGE:
			return "LOC_ATT_D_BULLET";
		case SkillsType.ENERGY_DAMAGE:
			return "LOC_ATT_D_ENERGY";
		case SkillsType.EXPLOSION_DAMAGE:
			return "LOC_ATT_D_EXPLOSIVE";
		case SkillsType.MELEE_DAMAGE:
			return "LOC_ATT_D_MELEE";
		case SkillsType.ALL_DAMAGE:
			return "LOC_ATT_D_ALL";
		case SkillsType.COOL_DOWN_SPEEDUP:
			return "LOC_ATT_REDUCECD";
		case SkillsType.BULLET_RES:
			return "LOC_ATT_R_BULLET";
		case SkillsType.ENERGY_RES:
			return "LOC_ATT_R_ENERGY";
		case SkillsType.EXPLOSION_RES:
			return "LOC_ATT_R_EXPLOSIVE";
		case SkillsType.MELEE_RES:
			return "LOC_ATT_R_MELEE";
		case SkillsType.HP_UP_RATE:
			return "LOC_ATT_LIFE_BONUS";
		case SkillsType.HP_AUTO_RECOVERY:
			return "LOC_ATT_LIFE_PERSEC";
		default:
			return type.ToString();
		}
	}

	public static bool isPercent(SkillsType skillType)
	{
		switch (skillType)
		{
		case SkillsType.MONEY_BOOTH:
		case SkillsType.EXP_BOOTH:
		case SkillsType.ASS_ATTACK:
		case SkillsType.RECOVERY_BOOTH:
		case SkillsType.HP_RECOVERY_WHEN_MAKE_KILL:
		case SkillsType.BLOCK_AT_A_RATE:
		case SkillsType.ASSAULT_BOOTH:
		case SkillsType.SHOTGUN_BOOTH:
		case SkillsType.RPG_BOOTH:
		case SkillsType.SWORD_BOOTH:
		case SkillsType.SNIPER_BOOTH:
		case SkillsType.ASSAULT_DEFENCE:
		case SkillsType.SHOTGUN_DEFENCE:
		case SkillsType.RPG_DEFENCE:
		case SkillsType.SWORD_DEFENCE:
		case SkillsType.SNIPER_DEFENCE:
		case SkillsType.HEALTH_STEAL:
		case SkillsType.BULLET_DAMAGE:
		case SkillsType.ENERGY_DAMAGE:
		case SkillsType.EXPLOSION_DAMAGE:
		case SkillsType.MELEE_DAMAGE:
		case SkillsType.ALL_DAMAGE:
		case SkillsType.COOL_DOWN_SPEEDUP:
		case SkillsType.BULLET_RES:
		case SkillsType.ENERGY_RES:
		case SkillsType.EXPLOSION_RES:
		case SkillsType.MELEE_RES:
		case SkillsType.ALL_RES:
		case SkillsType.HP_UP_RATE:
		case SkillsType.VEHICLE_ENERGY:
		case SkillsType.MONSTER_HUNTER:
		case SkillsType.BOSS_HUNTER:
		case SkillsType.HP_AUTO_PERCENT_RECOVERY:
		case SkillsType.SHOOT_SPEED:
		case SkillsType.DAMAGE_RETURN:
		case SkillsType.PISTOL_BOOTH:
		case SkillsType.PISTOL_DEFENCE:
		case SkillsType.BE_HIT_DAMAGE_LIMITED:
		case SkillsType.ASSAULT_ACC:
		case SkillsType.SHOTGUN_ACC:
		case SkillsType.RPG_ACC:
		case SkillsType.SWORD_ACC:
		case SkillsType.SNIPER_ACC:
		case SkillsType.PISTOL_ACC:
		case SkillsType.FINAL_DAMAGE:
		case SkillsType.FINAL_RES:
		case SkillsType.FINAL_PVP_HP_RATE:
		case SkillsType.SHARE_RATE:
		case SkillsType.SHOOT_PUNISH_REDUCE:
		case SkillsType.FINAL_BULLET_DEFENCE:
		case SkillsType.FINAL_ENERGY_DEFENCE:
		case SkillsType.FINAL_EXPLOSION_DEFENCE:
		case SkillsType.FINAL_MELEE_DEFENCE:
		case SkillsType.PERCENT_EXTRA_DAMAGE:
		case SkillsType.RPG_RADIUS_PERCENT_INCREASE:
		case SkillsType.SKILL_DAMAGE_DEFENCE:
		case SkillsType.DEBUFF_DURING_INCREASE:
		case SkillsType.PICKUP_DURING_INCREASE:
			return true;
		default:
			return false;
		}
	}
}
