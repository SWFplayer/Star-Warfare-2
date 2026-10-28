using System.Collections.Generic;
using UnityEngine;

public class GiantBetrayalModeLogic : VsModeLogic
{
	public enum EGiantBetrayalState
	{
		WAIT_CHOOSE_BOSS = 0,
		WAIT_BOSS_CHOOSE_SKILL = 1,
		FIGHTING_BOSS = 2
	}

	public const int PLAYER_BOSS_SKILL_COUNT = 4;

	public const float PLAYER_BOSS_TIME_TO_WIN = 120f;

	public const float MUTANT_WIN_TIME = 960f;

	public const float MUTANT_SCORE_EXP_RATE = 20f;

	public Dictionary<int, SkillGroup> bossPlayerActiveBufferList = new Dictionary<int, SkillGroup>();

	protected Player playerBoss;

	protected float playerBossWinTime;

	protected float matchOverTime;

	public EGiantBetrayalState State { get; set; }

	public override void Init()
	{
		State = EGiantBetrayalState.WAIT_CHOOSE_BOSS;
	}

	public Player GetPlayerBoss()
	{
		return playerBoss;
	}

	public void SetPlayerBoss(Player playerBoss)
	{
		this.playerBoss = playerBoss;
	}

	public void SetPlayerBossWinTime(float bossTimeToWin)
	{
		playerBossWinTime = Time.time + bossTimeToWin;
	}

	public float GetPlayerBossWinTime()
	{
		return playerBossWinTime;
	}

	public float GetPlayerBossTimeToWin()
	{
		return playerBossWinTime - Time.time;
	}

	public void PlayerBossActiveBuff(int id, PlayerSkill playerSkill)
	{
		if (!bossPlayerActiveBufferList.ContainsKey(id))
		{
			SkillGroup skillGroup = new SkillGroup();
			skillGroup.InitSkillInfo(MutantSkill.GetSkillInfoId(id));
			bossPlayerActiveBufferList.Add(id, skillGroup);
		}
		else
		{
			bossPlayerActiveBufferList[id].UninitFromPlayerSkill();
		}
		if (!bossPlayerActiveBufferList.ContainsKey(id))
		{
			return;
		}
		bossPlayerActiveBufferList[id].InitToPlayerSkill(playerSkill);
		foreach (AdvancedSkill advancedSkill in bossPlayerActiveBufferList[id].GetAdvancedSkills())
		{
			advancedSkill.Active();
		}
	}

	public float getActiveBufferEffectTime(int id)
	{
		if (bossPlayerActiveBufferList.ContainsKey(id))
		{
			return bossPlayerActiveBufferList[id].GetFirstAdvancedSkill().GetOffTriggerValues()[0];
		}
		return 0f;
	}

	public override void SendBattleInfoToNewPlayer(int newPlayerId)
	{
		MutantInfoRequest request = new MutantInfoRequest(GetPlayerBossTimeToWin(), newPlayerId);
		GameApp.GetInstance().GetNetworkManager().SendRequest(request);
	}

	public void SetMatchRemainingTime(float leftTime)
	{
		matchOverTime = Time.time + leftTime;
	}

	public float GetMatchRemainingTime()
	{
		if (matchOverTime == 0f)
		{
			return 960f;
		}
		return matchOverTime - Time.time;
	}
}
