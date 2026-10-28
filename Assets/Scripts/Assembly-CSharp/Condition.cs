using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition : GraphNode
{
	public static MessageBaseData MessageData;

	public List<BaseCompareCondition> mBaseConditionList = new List<BaseCompareCondition>();

	public GraphNode mNextNode;

	public Condition()
	{
		Type = EGraphNodeType.CONDITION;
	}

	public static bool IsStatePassTime(GameUnit gameUnit, float time)
	{
		return Time.time - gameUnit.StateStartTime > time;
	}

	public static bool IsMoveIdleFinish(GameUnit gameUnit)
	{
		Robot robot = gameUnit as Robot;
		return Time.time - robot.MoveStateStartTime >= robot.MoveIdleDuration;
	}

	public static bool IsNPCIdleFinish(GameUnit gameUnit)
	{
		Robot robot = gameUnit as Robot;
		if (!robot.Arrived && Time.time - robot.MoveStateStartTime >= robot.NPCIdleDuration[robot.CurrentLocation])
		{
			robot.CurrentLocation++;
			return true;
		}
		return false;
	}

	public static bool IsAttackIdleFinish(GameUnit gameUnit)
	{
		Robot robot = gameUnit as Robot;
		return Time.time - robot.StateStartTime >= robot.AttackIdleDuration;
	}

	public static bool IsAttackFinish(GameUnit gameUnit)
	{
		Robot robot = gameUnit as Robot;
		return Time.time - robot.StateStartTime >= robot.AttackDuration;
	}

	public static bool IsAttackKilled(GameUnit gameUnit)
	{
		Robot robot = gameUnit as Robot;
		return !robot.GetTarget().InPlayingState();
	}

	public static bool IsMoveStatePassTime(GameUnit gameUnit, float time)
	{
		return Time.time - ((Robot)gameUnit).MoveStateStartTime > time;
	}

	public static bool IsReachTargetPosition(GameUnit gameUnit)
	{
		Vector3 vector = gameUnit.GetPosition() - gameUnit.GetTargetPosition();
		vector.y = 0f;
		return vector.sqrMagnitude <= 0.25f;
	}

	public static bool IsHaveTarget(GameUnit gameUnit)
	{
		return gameUnit.GetTarget() != null && gameUnit.GetTarget().InPlayingState();
	}

	public static bool IsNoTarget(GameUnit gameUnit)
	{
		return gameUnit.GetTarget() == null || !gameUnit.GetTarget().InPlayingState();
	}

	public static bool IsJustReturnedToGame(GameUnit gameUnit)
	{
		Robot robot = gameUnit as Robot;
		return robot.IsJustReturnedToGame;
	}

	public static bool IsHitTarget(GameUnit gameUnit)
	{
		return gameUnit.HasHitEnemy();
	}

	public static bool IsGameStart(GameUnit gameUnit)
	{
		return true;
	}

	public static bool HasKilledEnemy(GameUnit gameUnit, float num)
	{
		Robot robot = gameUnit as Robot;
		return robot.GetLeader().Kills >= Mathf.RoundToInt(num);
	}

	public static bool PerKilledEnemy(GameUnit gameUnit, int num, int times)
	{
		Robot robot = gameUnit as Robot;
		return robot.GetLeader().Kills >= num * times;
	}

	public static bool IsPlayerHpBelow(GameUnit gameUnit, float num)
	{
		Robot robot = gameUnit as Robot;
		Player leader = robot.GetLeader();
		return (float)leader.Hp <= (float)leader.MaxHp * num;
	}

	public static bool IsHpBelow(GameUnit gameUnit, float num)
	{
		return (float)gameUnit.Hp <= (float)gameUnit.MaxHp * num;
	}

	public static bool IsSurroundedByEnemies(GameUnit gameUnit, float radius, float num)
	{
		Robot robot = gameUnit as Robot;
		GameUnit[] array = GameWorld.FindEnemyTargetList(robot.GetLeader(), false, radius);
		if (array != null && (float)array.Length >= num)
		{
			return true;
		}
		return false;
	}

	public static bool IsEliteEnemyClose(GameUnit gameUnit, float radius)
	{
		GameUnit leader = ((Robot)gameUnit).GetLeader();
		Hashtable enemies = GameApp.GetInstance().GetGameWorld().GetEnemies();
		float num = radius * radius;
		foreach (DictionaryEntry item in enemies)
		{
			Enemy enemy = item.Value as Enemy;
			if (enemy.IsElite && Vector3.SqrMagnitude(leader.GetPosition() - enemy.GetPosition()) < num)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsPlayerNotHitTargetIn(GameUnit gameUnit, float time)
	{
		Robot robot = gameUnit as Robot;
		return Time.time - robot.GetLeader().LastHitTargetTime >= time;
	}

	public static bool IsEnemyBehindPlayer(GameUnit gameUnit, float radius, float angle)
	{
		GameUnit leader = ((Robot)gameUnit).GetLeader();
		Hashtable enemies = GameApp.GetInstance().GetGameWorld().GetEnemies();
		foreach (DictionaryEntry item in enemies)
		{
			Enemy enemy = item.Value as Enemy;
			if (GameWorld.CheckEnemyInRange(leader, enemy.GetPosition(), radius, 0f - angle))
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsTargetInRangeSqrHorizontal(GameUnit gameUnit, float distanceSqr)
	{
		Vector3 vector = gameUnit.GetPosition() - gameUnit.GetTargetPosition();
		vector.y = 0f;
		return vector.sqrMagnitude < distanceSqr;
	}

	public static bool IsInMeleeAttackRange(GameUnit gameUnit)
	{
		float num = 0f;
		if (gameUnit.GameUnitType == EGameUnitType.ENEMY)
		{
			switch (((Enemy)gameUnit).EnemyType)
			{
			case EnemyType.STALKER:
				num = StalkerParameter.MELEE_ATTACK_DISTANCE * StalkerParameter.MELEE_ATTACK_DISTANCE;
				break;
			case EnemyType.SCORPION:
				num = TacorlionParameter.MeleeAttackDistance * TacorlionParameter.MeleeAttackDistance;
				break;
			default:
				num = ((Enemy)gameUnit).AttackRangeSqr;
				break;
			}
		}
		return Vector3.SqrMagnitude(gameUnit.GetPosition() - gameUnit.GetTargetPosition()) < num;
	}

	public static bool IsInRangeAttackRange(GameUnit gameUnit)
	{
		float num = 0f;
		if (gameUnit.GameUnitType == EGameUnitType.ENEMY)
		{
			switch (((Enemy)gameUnit).EnemyType)
			{
			case EnemyType.STALKER:
				num = StalkerParameter.JUMP_ATTACK_DISTANCE * StalkerParameter.JUMP_ATTACK_DISTANCE;
				break;
			case EnemyType.SCORPION:
				num = TacorlionParameter.RangeAttackDistance * TacorlionParameter.RangeAttackDistance;
				break;
			}
		}
		return Vector3.SqrMagnitude(gameUnit.GetPosition() - gameUnit.GetTargetPosition()) < num;
	}

	public static bool IsAnimationFinished(GameUnit gameUnit)
	{
		return gameUnit.IsAnimationFinish();
	}

	public static bool IsAnimationPlayed(GameUnit gameUnit, float time)
	{
		return gameUnit.IsAnimationPlayed(time);
	}

	public static bool NoIceArmor(GameUnit gameUnit)
	{
		FrostStone frostStone = gameUnit as FrostStone;
		return !frostStone.HasIceArmor;
	}

	public static bool IsDemsterRotateFinish(GameUnit gameUnit)
	{
		Demster demster = gameUnit as Demster;
		if (demster != null)
		{
			float num = ((!demster.IsElite) ? 4.5f : 10f);
			return Time.time - demster.StateStartTime > num;
		}
		Debug.LogError("Wrong.");
		return false;
	}

	public static bool IsHardMode(GameUnit gameUnit)
	{
		return gameUnit.IsHardMode();
	}

	public void AddBaseCondition(BaseCompareCondition baseCondition)
	{
		mBaseConditionList.Add(baseCondition);
	}

	public bool IsTriggered(GameUnit gameUnit)
	{
		foreach (BaseCompareCondition mBaseCondition in mBaseConditionList)
		{
			if (!mBaseCondition.IsTriggered(gameUnit))
			{
				return false;
			}
		}
		return true;
	}

	public bool ActivateNextState(GameUnit gameUnit)
	{
		if (mNextNode.Type == EGraphNodeType.CONDITION_STATE)
		{
			ConditionState conditionState = (ConditionState)mNextNode;
			if (gameUnit.GameUnitType == EGameUnitType.ROBOT)
			{
				Robot robot = gameUnit as Robot;
				robot.SetState(conditionState);
			}
			else
			{
				if (gameUnit.GameUnitType != EGameUnitType.ENEMY)
				{
					Debug.LogError("Robot is null.");
					return false;
				}
				Enemy enemy = gameUnit as Enemy;
				enemy.ChangeToConditionState(conditionState);
				enemy.SetState((EnemyState)conditionState.mState, false);
				enemy.SetNextConditionState(conditionState);
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer() && GameApp.GetInstance().GetGameMode().IsCoopMode())
				{
					EnemyStateRequest request = new EnemyStateRequest(enemy.EnemyID, conditionState.ID, enemy.GetPosition());
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
			}
			return true;
		}
		if (mNextNode.Type == EGraphNodeType.CONDITION_MESSAGE)
		{
			ConditionMessage conditionMessage = (ConditionMessage)mNextNode;
			MessageData = conditionMessage.mData;
			return gameUnit.HandleMessage(conditionMessage.mMessage);
		}
		return false;
	}
}
