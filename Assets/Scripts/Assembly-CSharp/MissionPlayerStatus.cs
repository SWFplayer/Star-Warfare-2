using System;
using UnityEngine;

public class MissionPlayerStatus : Mission
{
	private MissionPlayerStatusType mType;

	private int mTargetAmount;

	private bool mNeedLarger;

	private bool mNeedAlways;

	private Vector3 mLastPosition;

	private float mTotalMoveDistance;

	private float mStartTime;

	public override void LoadConfig(int i)
	{
		base.LoadConfig(i);
		GameDataTable dataTable = GetDataTable();
		mTargetAmount = 0;
		for (int j = 0; j < dataTable.Count; j++)
		{
			int num = dataTable.GetInt(j, 0);
			if (num == base.ID)
			{
				string text = dataTable.GetString(j, 1);
				mType = MissionPlayerStatusType.NO_MISSION;
				if (Enum.IsDefined(typeof(MissionPlayerStatusType), text))
				{
					mType = (MissionPlayerStatusType)(int)Enum.Parse(typeof(MissionPlayerStatusType), text, true);
				}
				else
				{
					Debug.LogError("Invalid Player Status Type: " + text);
				}
				mTargetAmount = dataTable.GetInt(j, 2);
				mNeedLarger = dataTable.GetBoolean(j, 3);
				mNeedAlways = dataTable.GetBoolean(j, 4);
				break;
			}
		}
	}

	public override void LevelStart()
	{
		base.LevelStart();
		mProcess = EMissionProcess.NORMAL;
		if (mType == MissionPlayerStatusType.MOVE_DISTANCE)
		{
			GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
			if (gameWorld != null)
			{
				Player localPlayer = gameWorld.GetLocalPlayer();
				if (localPlayer != null)
				{
					mLastPosition = localPlayer.GetTransform().position;
				}
			}
			mTotalMoveDistance = 0f;
		}
		else if (mType == MissionPlayerStatusType.TIME)
		{
			mStartTime = Time.time;
		}
	}

	public override EMissionProcess CheckIfComplete()
	{
		if (mProcess == EMissionProcess.NORMAL)
		{
			GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
			if (gameWorld != null)
			{
				Player localPlayer = gameWorld.GetLocalPlayer();
				if (localPlayer != null)
				{
					if (mType == MissionPlayerStatusType.MOVE_DISTANCE)
					{
						mTotalMoveDistance = localPlayer.GetTotalMoveDistance();
					}
					if (mNeedAlways)
					{
						if (mType == MissionPlayerStatusType.HP)
						{
							if (mNeedLarger && (float)localPlayer.Hp / (float)localPlayer.MaxHp < (float)mTargetAmount / 100f)
							{
								mProcess = EMissionProcess.FAIL;
							}
						}
						else if (mType == MissionPlayerStatusType.PRECISION)
						{
							if (localPlayer.GetAttackCount() > 0)
							{
								if (mNeedLarger)
								{
									if ((float)localPlayer.GetHitEnemyCount() * 100f / (float)localPlayer.GetAttackCount() < (float)mTargetAmount)
									{
										mProcess = EMissionProcess.FAIL;
									}
								}
								else if ((float)localPlayer.GetHitEnemyCount() * 100f / (float)localPlayer.GetAttackCount() > (float)mTargetAmount)
								{
									mProcess = EMissionProcess.FAIL;
								}
							}
						}
						else if (mType == MissionPlayerStatusType.PROTECTED_ITEM_HP && mNeedLarger && MissionManager.GetInstance().ProtectedItemHpPercentage < (float)mTargetAmount)
						{
							mProcess = EMissionProcess.FAIL;
						}
					}
					else if (mType == MissionPlayerStatusType.COMBO)
					{
						if (mNeedLarger)
						{
							if (localPlayer.GetMaxCombo() >= mTargetAmount)
							{
								mProcess = EMissionProcess.SUCCESS;
							}
						}
						else if (localPlayer.GetMaxCombo() > mTargetAmount)
						{
							mProcess = EMissionProcess.FAIL;
						}
					}
					else if (mType == MissionPlayerStatusType.MOVE_DISTANCE)
					{
						if (mNeedLarger)
						{
							if (localPlayer.GetTotalMoveDistance() >= (float)mTargetAmount)
							{
								mProcess = EMissionProcess.SUCCESS;
							}
						}
						else if (localPlayer.GetTotalMoveDistance() > (float)mTargetAmount)
						{
							mProcess = EMissionProcess.FAIL;
						}
					}
					else if (mType == MissionPlayerStatusType.TIME)
					{
						if (!mNeedLarger && Time.time - mStartTime > (float)mTargetAmount)
						{
							mProcess = EMissionProcess.FAIL;
						}
					}
					else if (mType == MissionPlayerStatusType.DAMAGE_TO_BOSS && mNeedLarger && MissionManager.GetInstance().DamageToSpecialEnemy >= mTargetAmount)
					{
						mProcess = EMissionProcess.SUCCESS;
					}
					if (MissionManager.GetInstance().MainMissionFinished)
					{
						if (mType == MissionPlayerStatusType.HP)
						{
							if (mNeedLarger)
							{
								if ((float)localPlayer.Hp / (float)localPlayer.MaxHp >= (float)mTargetAmount / 100f)
								{
									mProcess = EMissionProcess.SUCCESS;
								}
							}
							else if ((float)localPlayer.Hp / (float)localPlayer.MaxHp <= (float)mTargetAmount / 100f)
							{
								mProcess = EMissionProcess.SUCCESS;
							}
						}
						else if (mType == MissionPlayerStatusType.COMBO)
						{
							if (!mNeedLarger && localPlayer.GetMaxCombo() <= mTargetAmount)
							{
								mProcess = EMissionProcess.SUCCESS;
							}
						}
						else if (mType == MissionPlayerStatusType.MOVE_DISTANCE && !mNeedLarger && localPlayer.GetTotalMoveDistance() <= (float)mTargetAmount)
						{
							mProcess = EMissionProcess.SUCCESS;
						}
						if (mType == MissionPlayerStatusType.PRECISION)
						{
							if (localPlayer.GetAttackCount() > 0)
							{
								if (mNeedLarger)
								{
									if ((float)localPlayer.GetHitEnemyCount() * 100f / (float)localPlayer.GetAttackCount() >= (float)mTargetAmount)
									{
										mProcess = EMissionProcess.SUCCESS;
									}
								}
								else if ((float)localPlayer.GetHitEnemyCount() * 100f / (float)localPlayer.GetAttackCount() <= (float)mTargetAmount)
								{
									mProcess = EMissionProcess.SUCCESS;
								}
							}
						}
						else if (mType == MissionPlayerStatusType.TIME)
						{
							if (mNeedLarger)
							{
								if (Time.time - mStartTime >= (float)mTargetAmount)
								{
									mProcess = EMissionProcess.SUCCESS;
								}
							}
							else if (Time.time - mStartTime <= (float)mTargetAmount)
							{
								mProcess = EMissionProcess.SUCCESS;
							}
						}
						if (mType == MissionPlayerStatusType.PROTECTED_ITEM_HP && mNeedLarger && MissionManager.GetInstance().ProtectedItemHpPercentage >= (float)mTargetAmount)
						{
							mProcess = EMissionProcess.SUCCESS;
						}
					}
				}
			}
		}
		return mProcess;
	}

	public override float GetProgressPercentage()
	{
		if (mType == MissionPlayerStatusType.MOVE_DISTANCE)
		{
			return mTotalMoveDistance / (float)mTargetAmount;
		}
		if (mType == MissionPlayerStatusType.DAMAGE_TO_BOSS)
		{
			return (float)MissionManager.GetInstance().DamageToSpecialEnemy / (float)mTargetAmount;
		}
		return 0f;
	}
}
