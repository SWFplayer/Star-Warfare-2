using UnityEngine;

public class BossCoop
{
	public enum EHPState
	{
		State1 = 0,
		State2 = 1,
		State3 = 2,
		ToCastSkillState = 3,
		AfterSkill = 4,
		Single = 5,
		SetDefend = 6
	}

	public static float[] StateHPArray = new float[3] { 0.8f, 0.6f, 0.4f };

	private EnemyBoss mBoss1;

	private EnemyBoss mBoss2;

	private EHPState mState;

	public Vector3 mBoss1Position;

	public Vector3 mBoss2Position;

	private Vector3[] mStayPositions = new Vector3[12];

	private Vector3[] mAttackPositions = new Vector3[12];

	private int mPositionIndex;

	private float mCoopSkillStartTime;

	private float mCoopSkillInterval;

	public void Init()
	{
		mBoss1Position = GameObject.Find("BossMainSpawnPoint").transform.position;
		mBoss2Position = GameObject.Find("BossSubSpawnPoint").transform.position;
		for (int i = 0; i < 6; i++)
		{
			mStayPositions[i] = GameObject.Find("StayPointCenter" + (i + 1)).transform.position;
			mStayPositions[6 + i] = GameObject.Find("StayPointSide" + (i + 1)).transform.position;
			mAttackPositions[i] = GameObject.Find("AttackPointCenter" + (i + 1)).transform.position;
			mAttackPositions[6 + i] = GameObject.Find("AttackPointSide" + (i + 1)).transform.position;
		}
	}

	public void SetBoss(EnemyBoss boss1, EnemyBoss boss2)
	{
		Vector3 position = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.GetPosition();
		mBoss1 = boss1;
		mBoss2 = boss2;
		mBoss1.IsInBossCoop = true;
		mBoss2.IsInBossCoop = true;
		mBoss1.GetTransform().position = mBoss1Position;
		mBoss1.LookAtPointHorizontalAtOnce(position);
		mBoss2.GetTransform().position = mBoss2Position;
		mBoss2.LookAtPointHorizontalAtOnce(position);
		mBoss1.CreateNavMeshAgent();
		mBoss2.CreateNavMeshAgent();
		mBoss1.StartCoopRage();
		mBoss2.CoopBorn();
		mState = EHPState.State1;
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && Lobby.GetInstance().IsMasterPlayer)
		{
			EnemySpawnRequest request = new EnemySpawnRequest(mBoss1.EnemyType, EnemySpawnType.BOSS, Vector3.zero, 0, mBoss1.EnemyID, false, false);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			request = new EnemySpawnRequest(mBoss2.EnemyType, EnemySpawnType.BOSS, Vector3.zero, 0, mBoss2.EnemyID, false, false);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
	}

	public void Update()
	{
		if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && !Lobby.GetInstance().IsMasterPlayer)
		{
			return;
		}
		if (mState < EHPState.Single && (!mBoss1.InPlayingState() || !mBoss2.InPlayingState()))
		{
			mState = EHPState.Single;
			if (mBoss1.InPlayingState())
			{
				mBoss1.SetNextAction(ENextAction.CoopDead);
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
				{
					BossCoopRequest bossCoopRequest = new BossCoopRequest((byte)mState, 4);
					bossCoopRequest.Data.AddInt(mBoss1.EnemyID);
					GameApp.GetInstance().GetNetworkManager().SendRequest(bossCoopRequest);
				}
			}
			if (mBoss2.InPlayingState())
			{
				mBoss2.SetNextAction(ENextAction.CoopDead);
				mBoss2.SetInvincible(false);
				mBoss2.mBossDefend.bornDefentObj.SetActive(false);
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
				{
					BossCoopRequest bossCoopRequest2 = new BossCoopRequest((byte)mState, 4);
					bossCoopRequest2.Data.AddInt(mBoss2.EnemyID);
					GameApp.GetInstance().GetNetworkManager().SendRequest(bossCoopRequest2);
				}
			}
		}
		if (mState < EHPState.ToCastSkillState && 0 < mBoss1.Hp && (float)mBoss1.Hp < (float)mBoss1.MaxHp * StateHPArray[2])
		{
			mState = EHPState.ToCastSkillState;
			mPositionIndex = Random.Range(0, 6) * 2;
			mBoss1.CoopMoveToPosition(mStayPositions[mPositionIndex]);
			mBoss2.CoopMoveToPosition(mStayPositions[mPositionIndex + 1]);
			mBoss2.SetInvincible(false);
			mBoss2.mBossDefend.bornDefentObj.SetActive(false);
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				BossCoopRequest bossCoopRequest3 = new BossCoopRequest((byte)mState, 1);
				bossCoopRequest3.Data.AddByte((byte)mPositionIndex);
				GameApp.GetInstance().GetNetworkManager().SendRequest(bossCoopRequest3);
			}
		}
		else if (mState < EHPState.State3 && 0 < mBoss1.Hp && (float)mBoss1.Hp < (float)mBoss1.MaxHp * StateHPArray[1])
		{
			mState = EHPState.State3;
			mBoss1.StartCoopRage();
			mBoss1.SetMindState(EnemyBoss.MindState.RAGE1);
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				BossCoopRequest request = new BossCoopRequest((byte)mState, 0);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
		else if (mState < EHPState.State2 && 0 < mBoss1.Hp && (float)mBoss1.Hp < (float)mBoss1.MaxHp * StateHPArray[0])
		{
			mState = EHPState.State2;
			mBoss2.StartCoopRange();
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				BossCoopRequest request2 = new BossCoopRequest((byte)mState, 0);
				GameApp.GetInstance().GetNetworkManager().SendRequest(request2);
			}
		}
		if (mState == EHPState.ToCastSkillState)
		{
			if (mBoss1.IsStateMatch("CoopSkillIdle") && mBoss2.IsStateMatch("CoopSkillIdle"))
			{
				mState++;
				mCoopSkillStartTime = Time.time;
				mCoopSkillInterval = Random.Range(10f, 20f);
				mBoss1.CoopCastSkill();
				mBoss2.CoopCastSkill();
				mBoss1.SendStateChangeRequest();
				mBoss2.SendStateChangeRequest();
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
				{
					BossCoopRequest request3 = new BossCoopRequest((byte)mState, 0);
					GameApp.GetInstance().GetNetworkManager().SendRequest(request3);
				}
			}
		}
		else if (mState == EHPState.AfterSkill && Time.time - mCoopSkillStartTime > mCoopSkillInterval)
		{
			mState = EHPState.ToCastSkillState;
			mPositionIndex = Random.Range(0, 6) * 2;
			mBoss1.CoopMoveToPosition(mStayPositions[mPositionIndex]);
			mBoss2.CoopMoveToPosition(mStayPositions[mPositionIndex + 1]);
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				BossCoopRequest bossCoopRequest4 = new BossCoopRequest((byte)mState, 1);
				bossCoopRequest4.Data.AddByte((byte)mPositionIndex);
				GameApp.GetInstance().GetNetworkManager().SendRequest(bossCoopRequest4);
			}
		}
	}

	public Vector3 GetAttackPosition(Enemy enemy)
	{
		if (enemy == mBoss1)
		{
			return mAttackPositions[mPositionIndex];
		}
		return mAttackPositions[mPositionIndex + 1];
	}

	public void OnGotHitInDash(EnemyBoss enemy)
	{
		if (Lobby.GetInstance().IsMasterPlayer)
		{
			mCoopSkillStartTime = Time.time;
			mCoopSkillInterval = Random.Range(10f, 20f);
			mState = EHPState.AfterSkill;
			if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
			{
				BossCoopRequest bossCoopRequest = new BossCoopRequest((byte)mState, 1);
				bossCoopRequest.Data.AddByte((byte)mPositionIndex);
				GameApp.GetInstance().GetNetworkManager().SendRequest(bossCoopRequest);
			}
			EnemyBoss enemyBoss = ((enemy != mBoss1) ? mBoss1 : mBoss2);
			if (enemyBoss.IsStateMatch("CoopDash") || enemyBoss.IsStateMatch("CoopDash2"))
			{
				enemyBoss.SetState(Enemy.IDLE_STATE);
				enemyBoss.SendStateChangeRequest();
			}
			else if (enemyBoss.IsStateMatch("CoopSkillIdle"))
			{
				enemyBoss.CoopCastSkill();
				enemyBoss.SendStateChangeRequest();
			}
		}
	}

	public void DoResponse(BytesBuffer buffer)
	{
		byte b = buffer.ReadByte();
		mState = (EHPState)b;
		if (mState == EHPState.Single)
		{
			int num = buffer.ReadInt();
			EnemyBoss enemyBoss = GameApp.GetInstance().GetGameWorld().GetEnemyByID("E_" + num) as EnemyBoss;
			if (enemyBoss != null)
			{
				enemyBoss.SetNextAction(ENextAction.CoopDead);
				enemyBoss.SetInvincible(false);
				enemyBoss.mBossDefend.bornDefentObj.SetActive(false);
			}
		}
		else if (mState == EHPState.State2)
		{
			mBoss2.StartCoopRange();
		}
		else if (mState == EHPState.State3)
		{
			mBoss1.StartCoopRage();
			mBoss1.SetMindState(EnemyBoss.MindState.RAGE1);
		}
		else if (mState == EHPState.ToCastSkillState)
		{
			mPositionIndex = buffer.ReadByte();
			mBoss1.CoopMoveToPosition(mStayPositions[mPositionIndex]);
			mBoss2.CoopMoveToPosition(mStayPositions[mPositionIndex + 1]);
			mBoss2.SetInvincible(false);
			mBoss2.mBossDefend.bornDefentObj.SetActive(false);
		}
		else if (mState == EHPState.AfterSkill)
		{
			mCoopSkillStartTime = Time.time;
			mCoopSkillInterval = Random.Range(10f, 20f);
		}
		else if (mState == EHPState.SetDefend)
		{
			int num2 = buffer.ReadInt();
			EnemyBoss enemyBoss2 = GameApp.GetInstance().GetGameWorld().GetEnemyByID("E_" + num2) as EnemyBoss;
			if (enemyBoss2 != null)
			{
				enemyBoss2.SetInDefendRepetition();
			}
		}
	}
}
