using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class ConditionStateManager
{
	public struct GameUnitKey
	{
		public EGameUnitType GameUnitType;

		public int Type2;

		public GameUnitKey(EGameUnitType type, int type2)
		{
			GameUnitType = type;
			Type2 = type2;
		}
	}

	public static ConditionStateManager me = new ConditionStateManager();

	public Dictionary<GameUnitKey, CreatureConditionInfo> mConditionStateDic = new Dictionary<GameUnitKey, CreatureConditionInfo>();

	private CreatureConditionInfo mCurrentConditionInfo;

	public static ConditionStateManager GetInstance()
	{
		return me;
	}

	public void Init()
	{
	}

	public void InitConditionState(GameUnit creature)
	{
		GameUnitKey key = default(GameUnitKey);
		key.GameUnitType = creature.GameUnitType;
		key.Type2 = (int)((creature.GameUnitType == EGameUnitType.ENEMY) ? ((Enemy)creature).EnemyType : EnemyType.DRONE);
		key.Type2 = ((creature.GameUnitType != EGameUnitType.ROBOT) ? key.Type2 : ((int)((Robot)creature).GetRobotType()));
		if (!mConditionStateDic.ContainsKey(key))
		{
			InitState(creature);
		}
		if (creature.GameUnitType == EGameUnitType.ROBOT)
		{
			Robot robot = creature as Robot;
			robot.mStateSwitchDic = mConditionStateDic[key].mStateDic;
			robot.SetState(mConditionStateDic[key].mMoveState, 0);
			robot.SetState(mConditionStateDic[key].mInitState, 1);
			robot.SetState(mConditionStateDic[key].mMindState, 2);
			robot.mStateTag = mConditionStateDic[key].mStateTag;
			robot.mAllConditionState = mConditionStateDic[key].mAllConditionState;
		}
		else if (creature.GameUnitType == EGameUnitType.ENEMY)
		{
			Enemy enemy = creature as Enemy;
			enemy.mStateSwitchDic = mConditionStateDic[key].mStateDic;
			enemy.mStateTag = mConditionStateDic[key].mStateTag;
			enemy.mAllConditionState = mConditionStateDic[key].mAllConditionState;
		}
		else
		{
			Debug.LogError("dd");
		}
	}

	public ConditionState CreateConditionState(GameUnitState state, bool addToSwitchList = false)
	{
		ConditionState conditionState = new ConditionState();
		conditionState.ID = mCurrentConditionInfo.mCurrentId++;
		conditionState.SetState(state);
		if (addToSwitchList)
		{
			mCurrentConditionInfo.mStateDic.Add(state, conditionState);
		}
		mCurrentConditionInfo.mAllConditionState.Add(conditionState);
		return conditionState;
	}

	public ConditionState CreateConditionState(GameUnitState state, string tag)
	{
		ConditionState conditionState = new ConditionState();
		conditionState.ID = mCurrentConditionInfo.mCurrentId++;
		conditionState.SetState(state);
		if (mCurrentConditionInfo.mStateTag.ContainsKey(tag))
		{
			Debug.LogError("Duplicate tag: " + tag);
		}
		else
		{
			mCurrentConditionInfo.mStateTag[tag] = conditionState;
		}
		mCurrentConditionInfo.mAllConditionState.Add(conditionState);
		return conditionState;
	}

	public BaseCompareCondition CreateTriggerCompareCondition(int i)
	{
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.ROBOT, DataSheetName.ROBOT_TRIGGER);
		switch (i)
		{
		case 1:
		{
			CDVoidCompareCondition cDVoidCompareCondition = new CDVoidCompareCondition();
			cDVoidCompareCondition.mDelegate = Condition.IsGameStart;
			cDVoidCompareCondition.mCD = gameDataTable.GetFloat(i - 1, 1);
			float num3 = gameDataTable.GetFloat(i - 1, gameDataTable[i - 1].Count - 1);
			if (num3 > float.Epsilon)
			{
				cDVoidCompareCondition.SetTimerInterval(num3);
			}
			return cDVoidCompareCondition;
		}
		case 2:
		case 3:
		case 4:
		{
			CDFloatCompareCondition cDFloatCompareCondition3 = new CDFloatCompareCondition();
			cDFloatCompareCondition3.mDelegate = Condition.HasKilledEnemy;
			cDFloatCompareCondition3.mValue = gameDataTable.GetFloat(i - 1, 1);
			cDFloatCompareCondition3.mCD = gameDataTable.GetFloat(i - 1, 2);
			float num5 = gameDataTable.GetFloat(i - 1, gameDataTable[i - 1].Count - 1);
			if (num5 > float.Epsilon)
			{
				cDFloatCompareCondition3.SetTimerInterval(num5);
			}
			return cDFloatCompareCondition3;
		}
		case 5:
		{
			PerKillCompareCondition perKillCompareCondition = new PerKillCompareCondition();
			perKillCompareCondition.mDelegate = Condition.PerKilledEnemy;
			perKillCompareCondition.mValue1 = gameDataTable.GetInt(i - 1, 1);
			float num2 = gameDataTable.GetFloat(i - 1, gameDataTable[i - 1].Count - 1);
			if (num2 > float.Epsilon)
			{
				perKillCompareCondition.SetTimerInterval(num2);
			}
			return perKillCompareCondition;
		}
		case 6:
		{
			CDFloatCompareCondition cDFloatCompareCondition6 = new CDFloatCompareCondition();
			cDFloatCompareCondition6.mDelegate = Condition.IsPlayerHpBelow;
			cDFloatCompareCondition6.mValue = gameDataTable.GetFloat(i - 1, 1);
			cDFloatCompareCondition6.mCD = gameDataTable.GetFloat(i - 1, 2);
			float num9 = gameDataTable.GetFloat(i - 1, gameDataTable[i - 1].Count - 1);
			if (num9 > float.Epsilon)
			{
				cDFloatCompareCondition6.SetTimerInterval(num9);
			}
			return cDFloatCompareCondition6;
		}
		case 7:
		case 8:
		{
			CDFloatCompareCondition cDFloatCompareCondition5 = new CDFloatCompareCondition();
			cDFloatCompareCondition5.mDelegate = Condition.IsHpBelow;
			cDFloatCompareCondition5.mValue = gameDataTable.GetFloat(i - 1, 1);
			cDFloatCompareCondition5.mCD = gameDataTable.GetFloat(i - 1, 2);
			float num8 = gameDataTable.GetFloat(i - 1, gameDataTable[i - 1].Count - 1);
			if (num8 > float.Epsilon)
			{
				cDFloatCompareCondition5.SetTimerInterval(num8);
			}
			return cDFloatCompareCondition5;
		}
		case 9:
		{
			CDVoidCompareCondition cDVoidCompareCondition2 = new CDVoidCompareCondition();
			cDVoidCompareCondition2.mDelegate = Condition.IsJustReturnedToGame;
			cDVoidCompareCondition2.mCD = gameDataTable.GetFloat(i - 1, 1);
			float num6 = gameDataTable.GetFloat(i - 1, gameDataTable[i - 1].Count - 1);
			if (num6 > float.Epsilon)
			{
				cDVoidCompareCondition2.SetTimerInterval(num6);
			}
			return cDVoidCompareCondition2;
		}
		case 10:
		{
			CDFloat2CompareCondition cDFloat2CompareCondition2 = new CDFloat2CompareCondition();
			cDFloat2CompareCondition2.mDelegate = Condition.IsSurroundedByEnemies;
			cDFloat2CompareCondition2.mCD = gameDataTable.GetFloat(i - 1, 1);
			cDFloat2CompareCondition2.mValue1 = gameDataTable.GetFloat(i - 1, 2);
			cDFloat2CompareCondition2.mValue2 = gameDataTable.GetFloat(i - 1, 3);
			float num10 = gameDataTable.GetFloat(i - 1, gameDataTable[i - 1].Count - 1);
			if (num10 > float.Epsilon)
			{
				cDFloat2CompareCondition2.SetTimerInterval(num10);
			}
			return cDFloat2CompareCondition2;
		}
		case 11:
		case 12:
		{
			CDFloatCompareCondition cDFloatCompareCondition4 = new CDFloatCompareCondition();
			cDFloatCompareCondition4.mDelegate = Condition.IsPlayerNotHitTargetIn;
			cDFloatCompareCondition4.mCD = gameDataTable.GetFloat(i - 1, 1);
			cDFloatCompareCondition4.mValue = gameDataTable.GetFloat(i - 1, 2);
			cDFloatCompareCondition4.StartNow = false;
			float num7 = gameDataTable.GetFloat(i - 1, gameDataTable[i - 1].Count - 1);
			if (num7 > float.Epsilon)
			{
				cDFloatCompareCondition4.SetTimerInterval(num7);
			}
			return cDFloatCompareCondition4;
		}
		case 13:
		{
			CDFloatCompareCondition cDFloatCompareCondition2 = new CDFloatCompareCondition();
			cDFloatCompareCondition2.mDelegate = Condition.IsEliteEnemyClose;
			cDFloatCompareCondition2.mCD = gameDataTable.GetFloat(i - 1, 1);
			cDFloatCompareCondition2.mValue = gameDataTable.GetFloat(i - 1, 2);
			float num4 = gameDataTable.GetFloat(i - 1, gameDataTable[i - 1].Count - 1);
			if (num4 > float.Epsilon)
			{
				cDFloatCompareCondition2.SetTimerInterval(num4);
			}
			return cDFloatCompareCondition2;
		}
		case 14:
		{
			CDFloat2CompareCondition cDFloat2CompareCondition = new CDFloat2CompareCondition();
			cDFloat2CompareCondition.mDelegate = Condition.IsEnemyBehindPlayer;
			cDFloat2CompareCondition.mCD = gameDataTable.GetFloat(i - 1, 1);
			cDFloat2CompareCondition.mValue1 = gameDataTable.GetFloat(i - 1, 2);
			cDFloat2CompareCondition.mValue2 = gameDataTable.GetFloat(i - 1, 3);
			float num = gameDataTable.GetFloat(i - 1, gameDataTable[i - 1].Count - 1);
			if (num > float.Epsilon)
			{
				cDFloat2CompareCondition.SetTimerInterval(num);
			}
			return cDFloat2CompareCondition;
		}
		case 15:
		{
			DynamicCDFloat2CompareCondition dynamicCDFloat2CompareCondition = new DynamicCDFloat2CompareCondition();
			dynamicCDFloat2CompareCondition.mValue1 = gameDataTable.GetFloat(i - 1, 1);
			dynamicCDFloat2CompareCondition.mValue2 = gameDataTable.GetFloat(i - 1, 2);
			return dynamicCDFloat2CompareCondition;
		}
		default:
		{
			CDFloatCompareCondition cDFloatCompareCondition = new CDFloatCompareCondition();
			cDFloatCompareCondition.mDelegate = Condition.HasKilledEnemy;
			cDFloatCompareCondition.mCD = gameDataTable.GetFloat(i - 1, 1);
			cDFloatCompareCondition.mValue = 10000f;
			return cDFloatCompareCondition;
		}
		}
	}

	public ConditionMessage CreateTriggerConditionMessage(int i, string value1, string value2)
	{
		switch (i)
		{
		case 1:
		{
			ConditionMessage conditionMessage2 = new ConditionMessage(EConditionMessage.EDialog);
			DialogMessageData dialogMessageData = new DialogMessageData();
			dialogMessageData.StartString = value1;
			dialogMessageData.Num = int.Parse(value2);
			conditionMessage2.mData = dialogMessageData;
			return conditionMessage2;
		}
		case 2:
			return new ConditionMessage(EConditionMessage.ECastSkill);
		case 3:
		{
			ConditionMessage conditionMessage3 = new ConditionMessage(EConditionMessage.EKeepOnFire);
			FireMessageData fireMessageData = new FireMessageData();
			fireMessageData.Time = float.Parse(value1);
			conditionMessage3.mData = fireMessageData;
			return conditionMessage3;
		}
		case 4:
			return new ConditionMessage(EConditionMessage.EAttackLocalPlayerBehind);
		case 5:
			return new ConditionMessage(EConditionMessage.EGoneTeleport);
		case 6:
			return new ConditionMessage(EConditionMessage.EBackToBattle);
		case 7:
			return new ConditionMessage(EConditionMessage.EUseAttackSkill);
		case 8:
			return new ConditionMessage(EConditionMessage.EUseHealSkill);
		case 9:
		{
			ConditionMessage conditionMessage = new ConditionMessage(EConditionMessage.ERandomChangeGun);
			ChangeWeaponMessageData changeWeaponMessageData = new ChangeWeaponMessageData();
			MatchCollection matchCollection = Regex.Matches(value1, "\\d+");
			changeWeaponMessageData.Chances = new float[matchCollection.Count];
			int num = 0;
			foreach (Match item in matchCollection)
			{
				changeWeaponMessageData.Chances[num++] = (float)int.Parse(item.Value) / 100f;
			}
			conditionMessage.mData = changeWeaponMessageData;
			return conditionMessage;
		}
		default:
			Debug.LogError("CreateTriggerConditionMessage invalid value: " + i);
			return null;
		}
	}

	private void InitState(GameUnit gameUnit)
	{
		if (gameUnit.GameUnitType == EGameUnitType.ROBOT)
		{
			Robot robot = gameUnit as Robot;
			if (robot.GetRobotType() == ERobotType.XIAOHUOBAN)
			{
				mCurrentConditionInfo = new CreatureConditionInfo();
				ConditionState conditionState = CreateConditionState(Robot.RobotMoveIdleState, true);
				ConditionState conditionState2 = CreateConditionState(Robot.RobotMoveCatchState, true);
				ConditionState conditionState3 = CreateConditionState(Robot.RobotIdleState, true);
				ConditionState conditionState4 = CreateConditionState(Robot.RobotFireState, true);
				ConditionState conditionState5 = CreateConditionState(Robot.RobotDeadState, true);
				ConditionState conditionState6 = CreateConditionState(Robot.RobotInvalidState, true);
				ConditionState conditionState7 = CreateConditionState(Robot.RobotMindState);
				conditionState.AddCondition(Condition.IsMoveIdleFinish, EConditionMessage.EMoveToNextPoint);
				conditionState2.AddCondition(Condition.IsReachTargetPosition, EConditionMessage.EDecideMoveOrIdle);
				conditionState2.AddCondition(Condition.IsMoveStatePassTime, 5f, conditionState);
				conditionState3.AddCondition(Condition.IsAttackIdleFinish, EConditionMessage.ETryAttack);
				conditionState4.AddCondition(Condition.IsAttackFinish, conditionState3);
				mCurrentConditionInfo.mMoveState = conditionState;
				mCurrentConditionInfo.mInitState = conditionState3;
				mCurrentConditionInfo.mMindState = conditionState7;
				mConditionStateDic.Add(new GameUnitKey(gameUnit.GameUnitType, (int)robot.GetRobotType()), mCurrentConditionInfo);
				GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.ROBOT, DataSheetName.ROBOT_CORRESPONDENCE);
				for (int i = 0; i < gameDataTable.Count; i++)
				{
					int i2 = gameDataTable.GetInt(i, 0);
					int i3 = gameDataTable.GetInt(i, 1);
					Condition condition = new Condition();
					condition.mBaseConditionList.Add(CreateTriggerCompareCondition(i2));
					condition.mNextNode = CreateTriggerConditionMessage(i3, gameDataTable.GetString(i, 2), gameDataTable.GetString(i, 3));
					conditionState7.AddCondition(condition);
				}
			}
			else if (robot.GetRobotType() == ERobotType.PROTECTED_NPC)
			{
				mCurrentConditionInfo = new CreatureConditionInfo();
				ConditionState conditionState8 = CreateConditionState(Robot.RobotIdleState, "NPCIdle");
				ConditionState conditionState9 = CreateConditionState(Robot.RobotFireState, "NPCFire");
				ConditionState conditionState10 = CreateConditionState(Robot.RobotMoveCatchState, "NPCMoveCatch");
				ConditionState conditionState11 = CreateConditionState(Robot.RobotMoveIdleState, "NPCMoveIdle");
				ConditionState conditionState12 = CreateConditionState(Robot.RobotDeadState, true);
				ConditionState conditionState13 = CreateConditionState(Robot.RobotInvalidState, true);
				ConditionState conditionState14 = CreateConditionState(Robot.RobotMindState);
				conditionState11.AddCondition(Condition.IsNPCIdleFinish, EConditionMessage.ENPCMoveNext);
				conditionState10.AddCondition(Condition.IsReachTargetPosition, conditionState11);
				conditionState8.AddCondition(Condition.IsAttackIdleFinish, EConditionMessage.EDefenseAttack);
				conditionState9.AddCondition(Condition.IsAttackKilled, EConditionMessage.EFindNextTarget);
				conditionState9.AddCondition(Condition.IsAttackFinish, conditionState8);
				mCurrentConditionInfo.mMoveState = conditionState11;
				mCurrentConditionInfo.mInitState = conditionState8;
				mCurrentConditionInfo.mMindState = conditionState14;
				mConditionStateDic.Add(new GameUnitKey(gameUnit.GameUnitType, (int)robot.GetRobotType()), mCurrentConditionInfo);
				GameDataTable gameDataTable2 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.ROBOT, DataSheetName.ROBOT_CORRESPONDENCE);
				for (int j = 0; j < gameDataTable2.Count; j++)
				{
					int i4 = gameDataTable2.GetInt(j, 0);
					int i5 = gameDataTable2.GetInt(j, 1);
					Condition condition2 = new Condition();
					condition2.mBaseConditionList.Add(CreateTriggerCompareCondition(i4));
					condition2.mNextNode = CreateTriggerConditionMessage(i5, gameDataTable2.GetString(j, 2), gameDataTable2.GetString(j, 3));
					conditionState14.AddCondition(condition2);
				}
			}
			else if (robot.GetRobotType() == ERobotType.DEFENDER)
			{
				mCurrentConditionInfo = new CreatureConditionInfo();
				ConditionState conditionState15 = CreateConditionState(Robot.RobotMoveIdleState, true);
				ConditionState conditionState16 = CreateConditionState(Robot.RobotMoveCatchState, true);
				ConditionState conditionState17 = CreateConditionState(Robot.RobotIdleState, true);
				ConditionState conditionState18 = CreateConditionState(Robot.RobotFireState, true);
				ConditionState conditionState19 = CreateConditionState(Robot.RobotDeadState, true);
				ConditionState conditionState20 = CreateConditionState(Robot.RobotInvalidState, true);
				ConditionState conditionState21 = CreateConditionState(Robot.RobotMindState);
				conditionState15.AddCondition(Condition.IsMoveIdleFinish, EConditionMessage.EMoveToNextPoint);
				conditionState16.AddCondition(Condition.IsReachTargetPosition, EConditionMessage.EDecideMoveOrIdle);
				conditionState16.AddCondition(Condition.IsMoveStatePassTime, 5f, conditionState15);
				conditionState17.AddCondition(Condition.IsAttackIdleFinish, EConditionMessage.ETryAttack);
				conditionState18.AddCondition(Condition.IsAttackKilled, EConditionMessage.EFindNextTarget);
				conditionState18.AddCondition(Condition.IsAttackFinish, conditionState17);
				mCurrentConditionInfo.mMoveState = conditionState15;
				mCurrentConditionInfo.mInitState = conditionState17;
				mCurrentConditionInfo.mMindState = conditionState21;
				mConditionStateDic.Add(new GameUnitKey(gameUnit.GameUnitType, (int)robot.GetRobotType()), mCurrentConditionInfo);
				GameDataTable gameDataTable3 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.ROBOT, DataSheetName.ROBOT_CORRESPONDENCE);
				for (int k = 0; k < gameDataTable3.Count; k++)
				{
					int i6 = gameDataTable3.GetInt(k, 0);
					int i7 = gameDataTable3.GetInt(k, 1);
					Condition condition3 = new Condition();
					condition3.mBaseConditionList.Add(CreateTriggerCompareCondition(i6));
					condition3.mNextNode = CreateTriggerConditionMessage(i7, gameDataTable3.GetString(k, 2), gameDataTable3.GetString(k, 3));
					conditionState21.AddCondition(condition3);
				}
			}
		}
		else if (gameUnit.GameUnitType == EGameUnitType.ENEMY)
		{
			Enemy enemy = gameUnit as Enemy;
			if (enemy.EnemyType == EnemyType.STALKER)
			{
				mCurrentConditionInfo = new CreatureConditionInfo();
				ConditionState conditionState22 = CreateConditionState(Enemy.GRAVEBORN_STATE, true);
				ConditionState conditionState23 = CreateConditionState(Enemy.IDLE_STATE, true);
				ConditionState conditionState24 = CreateConditionState(Enemy.CATCHING_STATE, true);
				ConditionState conditionState25 = CreateConditionState(Enemy.CATCHING_STATE, "ContinueCatch");
				ConditionState conditionState26 = CreateConditionState(Stalker.JUMP_CATCH_STATE, true);
				ConditionState nextState = CreateConditionState(Enemy.ATTACK_STATE, true);
				ConditionState conditionState27 = CreateConditionState(Stalker.JUMP_ATTACK_STATE, true);
				ConditionState conditionState28 = CreateConditionState(Enemy.GOTHIT_STATE, true);
				ConditionState conditionState29 = CreateConditionState(Enemy.DEAD_STATE, true);
				ConditionState conditionState30 = CreateConditionState(Enemy.FADE_OUT_STATE, true);
				ConditionState conditionState31 = CreateConditionState(Enemy.BLACK_HOLE_STATE, true);
				ConditionState conditionState32 = CreateConditionState(Enemy.STUN_STATE, true);
				ConditionState conditionState33 = CreateConditionState(Enemy.ROUTE_STATE, true);
				conditionState24.AddCondition(Condition.IsInRangeAttackRange, EConditionMessage.ETryAttack);
				conditionState27.AddCondition(Condition.IsAnimationFinished, conditionState23);
				conditionState27.AddCondition(Condition.IsAnimationPlayed, StalkerParameter.JUMP_ATTACK_STOP_MOVE_TIME, EConditionMessage.EStopMove);
				conditionState26.AddConditonStateFinish(Condition.IsInMeleeAttackRange, nextState);
				conditionState26.AddConditonStateFinish(conditionState27);
				mCurrentConditionInfo.mInitState = conditionState23;
				mConditionStateDic.Add(new GameUnitKey(gameUnit.GameUnitType, (int)enemy.EnemyType), mCurrentConditionInfo);
			}
			else if (enemy.EnemyType == EnemyType.BIGBEE)
			{
				mCurrentConditionInfo = new CreatureConditionInfo();
				ConditionState conditionState34 = CreateConditionState(Enemy.GRAVEBORN_STATE, true);
				ConditionState conditionState35 = CreateConditionState(Enemy.IDLE_STATE, true);
				ConditionState conditionState36 = CreateConditionState(Enemy.CATCHING_STATE, true);
				ConditionState conditionState37 = CreateConditionState(Enemy.ATTACK_STATE, true);
				ConditionState conditionState38 = CreateConditionState(Enemy.GOTHIT_STATE, true);
				ConditionState conditionState39 = CreateConditionState(Enemy.DEAD_STATE, true);
				ConditionState conditionState40 = CreateConditionState(Enemy.FADE_OUT_STATE, true);
				ConditionState conditionState41 = CreateConditionState(Enemy.STUN_STATE, true);
				ConditionState conditionState42 = CreateConditionState(Enemy.ROUTE_STATE, true);
				ConditionState conditionState43 = CreateConditionState(BigBee.BIGBEE_3_SHOT_STATE, true);
				ConditionState conditionState44 = CreateConditionState(Enemy.BLACK_HOLE_STATE, true);
				conditionState43.AddCondition(Condition.IsAnimationFinished, conditionState35);
				mCurrentConditionInfo.mInitState = conditionState35;
				mConditionStateDic.Add(new GameUnitKey(gameUnit.GameUnitType, (int)enemy.EnemyType), mCurrentConditionInfo);
			}
			else if (enemy.EnemyType == EnemyType.SCORPION)
			{
				mCurrentConditionInfo = new CreatureConditionInfo();
				ConditionState conditionState45 = CreateConditionState(Enemy.GRAVEBORN_STATE, true);
				ConditionState conditionState46 = CreateConditionState(Enemy.IDLE_STATE, true);
				ConditionState conditionState47 = CreateConditionState(Enemy.ATTACK_STATE, true);
				ConditionState conditionState48 = CreateConditionState(Enemy.GOTHIT_STATE, true);
				ConditionState conditionState49 = CreateConditionState(Enemy.DEAD_STATE, true);
				ConditionState conditionState50 = CreateConditionState(Enemy.FADE_OUT_STATE, true);
				ConditionState conditionState51 = CreateConditionState(Enemy.CONDITON_CATCH_STATE, "MeleeAttackCatch");
				ConditionState conditionState52 = CreateConditionState(Enemy.CONDITON_CATCH_STATE, "RangeAttackCatch");
				ConditionState conditionState53 = CreateConditionState(Tacorlion.TACORLION_RANGE_ATTACK_STATE, true);
				ConditionState conditionState54 = CreateConditionState(Enemy.BLACK_HOLE_STATE, true);
				ConditionState conditionState55 = CreateConditionState(Enemy.STUN_STATE, true);
				ConditionState conditionState56 = CreateConditionState(Enemy.ROUTE_STATE, true);
				conditionState51.AddCondition(Condition.IsInMeleeAttackRange, conditionState47);
				conditionState51.AddCondition(Condition.IsStatePassTime, 5f, conditionState52);
				conditionState52.AddCondition(Condition.IsInRangeAttackRange, conditionState53);
				conditionState47.AddEndMessage(EConditionMessage.EResetNum);
				conditionState53.AddCondition(Condition.IsAnimationFinished, conditionState46);
				conditionState53.AddEndMessage(EConditionMessage.EIncreaseNum);
				mCurrentConditionInfo.mInitState = conditionState46;
				mConditionStateDic.Add(new GameUnitKey(gameUnit.GameUnitType, (int)enemy.EnemyType), mCurrentConditionInfo);
			}
			else if (enemy.EnemyType == EnemyType.BANELING)
			{
				mCurrentConditionInfo = new CreatureConditionInfo();
				ConditionState conditionState57 = CreateConditionState(Enemy.GRAVEBORN_STATE, true);
				ConditionState mInitState = CreateConditionState(Enemy.IDLE_STATE, true);
				ConditionState conditionState58 = CreateConditionState(Enemy.CATCHING_STATE, true);
				ConditionState conditionState59 = CreateConditionState(Enemy.ATTACK_STATE, true);
				ConditionState conditionState60 = CreateConditionState(Enemy.GOTHIT_STATE, true);
				ConditionState conditionState61 = CreateConditionState(Enemy.DEAD_STATE, true);
				ConditionState conditionState62 = CreateConditionState(Enemy.FADE_OUT_STATE, true);
				ConditionState conditionState63 = CreateConditionState(Enemy.BLACK_HOLE_STATE, true);
				ConditionState conditionState64 = CreateConditionState(Enemy.STUN_STATE, true);
				ConditionState conditionState65 = CreateConditionState(Enemy.ROUTE_STATE, true);
				mCurrentConditionInfo.mInitState = mInitState;
				mConditionStateDic.Add(new GameUnitKey(gameUnit.GameUnitType, (int)enemy.EnemyType), mCurrentConditionInfo);
			}
			else if (enemy.EnemyType == EnemyType.TANK)
			{
				mCurrentConditionInfo = new CreatureConditionInfo();
				ConditionState conditionState66 = CreateConditionState(Enemy.GRAVEBORN_STATE, true);
				ConditionState conditionState67 = CreateConditionState(Enemy.IDLE_STATE, true);
				ConditionState conditionState68 = CreateConditionState(Enemy.CATCHING_STATE, true);
				ConditionState conditionState69 = CreateConditionState(Enemy.ATTACK_STATE, true);
				ConditionState conditionState70 = CreateConditionState(Enemy.GOTHIT_STATE, true);
				ConditionState conditionState71 = CreateConditionState(Enemy.DEAD_STATE, true);
				ConditionState conditionState72 = CreateConditionState(Enemy.FADE_OUT_STATE, true);
				ConditionState conditionState73 = CreateConditionState(Tank.TANK_SHOCK_ATTACK_STATE, true);
				ConditionState conditionState74 = CreateConditionState(Tank.TANK_DASH_STATE, true);
				ConditionState conditionState75 = CreateConditionState(Enemy.ROUTE_STATE, true);
				conditionState73.AddCondition(Condition.IsAnimationFinished, conditionState67);
				conditionState74.AddConditonStateFinish(conditionState73);
				conditionState74.AddCondition(Condition.IsStatePassTime, TankParameter.DashDuration, conditionState67);
				mCurrentConditionInfo.mInitState = conditionState67;
				mConditionStateDic.Add(new GameUnitKey(gameUnit.GameUnitType, (int)enemy.EnemyType), mCurrentConditionInfo);
			}
			else if (enemy.EnemyType == EnemyType.MYTH370)
			{
				mCurrentConditionInfo = new CreatureConditionInfo();
				ConditionState conditionState76 = CreateConditionState(Enemy.GRAVEBORN_STATE, true);
				ConditionState mInitState2 = CreateConditionState(Enemy.IDLE_STATE, true);
				ConditionState conditionState77 = CreateConditionState(Enemy.CATCHING_STATE, true);
				ConditionState conditionState78 = CreateConditionState(Enemy.ATTACK_STATE, true);
				ConditionState conditionState79 = CreateConditionState(Enemy.GOTHIT_STATE, true);
				ConditionState conditionState80 = CreateConditionState(Enemy.DEAD_STATE, true);
				ConditionState conditionState81 = CreateConditionState(Enemy.FADE_OUT_STATE, true);
				ConditionState conditionState82 = CreateConditionState(Myth370.TELEPORT_STATE, true);
				ConditionState conditionState83 = CreateConditionState(Enemy.BLACK_HOLE_STATE, true);
				ConditionState conditionState84 = CreateConditionState(Enemy.STUN_STATE, true);
				ConditionState conditionState85 = CreateConditionState(Enemy.ROUTE_STATE, true);
				conditionState78.AddStartMessage(EConditionMessage.EIncreaseNum);
				conditionState82.AddStartMessage(EConditionMessage.EResetNum);
				conditionState82.AddCondition(Condition.IsAnimationFinished, conditionState78);
				conditionState77.AddCondition(Condition.IsStatePassTime, Myth370Parameter.CatchTimeOut, conditionState82);
				mCurrentConditionInfo.mInitState = mInitState2;
				mConditionStateDic.Add(new GameUnitKey(gameUnit.GameUnitType, (int)enemy.EnemyType), mCurrentConditionInfo);
			}
			else if (enemy.EnemyType == EnemyType.DEMSTER)
			{
				mCurrentConditionInfo = new CreatureConditionInfo();
				ConditionState conditionState86 = CreateConditionState(Enemy.GRAVEBORN_STATE, true);
				ConditionState conditionState87 = CreateConditionState(Enemy.IDLE_STATE, true);
				ConditionState conditionState88 = CreateConditionState(Enemy.CATCHING_STATE, true);
				ConditionState conditionState89 = CreateConditionState(Enemy.ATTACK_STATE, true);
				ConditionState conditionState90 = CreateConditionState(Enemy.GOTHIT_STATE, true);
				ConditionState conditionState91 = CreateConditionState(Enemy.DEAD_STATE, true);
				ConditionState conditionState92 = CreateConditionState(Enemy.FADE_OUT_STATE, true);
				ConditionState conditionState93 = CreateConditionState(Demster.DEMSTER_ROTATE_ATTACK_STATE, true);
				ConditionState conditionState94 = CreateConditionState(Demster.DEMSTER_ROTATE_ATTACK_FINISH_STATE, true);
				ConditionState conditionState95 = CreateConditionState(Enemy.STUN_STATE, true);
				ConditionState conditionState96 = CreateConditionState(Enemy.BLACK_HOLE_STATE, true);
				ConditionState conditionState97 = CreateConditionState(Enemy.ROUTE_STATE, true);
				conditionState93.AddCondition(Condition.IsDemsterRotateFinish, conditionState94);
				conditionState93.AddConditonStateFinish(conditionState94);
				conditionState94.AddCondition(Condition.IsAnimationFinished, conditionState87);
				mCurrentConditionInfo.mInitState = conditionState87;
				mConditionStateDic.Add(new GameUnitKey(gameUnit.GameUnitType, (int)enemy.EnemyType), mCurrentConditionInfo);
			}
			else if (enemy.EnemyType == EnemyType.SLASHER)
			{
				mCurrentConditionInfo = new CreatureConditionInfo();
				ConditionState conditionState98 = CreateConditionState(Enemy.IDLE_STATE, true);
				CreateConditionState(Enemy.CATCHING_STATE, true);
				CreateConditionState(Enemy.GOTHIT_STATE, true);
				CreateConditionState(Enemy.ATTACK_STATE, true);
				CreateConditionState(Enemy.DEAD_STATE, true);
				CreateConditionState(Enemy.FADE_OUT_STATE, true);
				CreateConditionState(EnemyBoss.BOSS_INIT_STATE, true);
				ConditionState conditionState99 = CreateConditionState(EnemyBoss.BOSS_RAGE_STATE, true);
				CreateConditionState(Slasher.SLASHER_ATTACK01_STATE, true);
				CreateConditionState(Slasher.SLASHER_ATTACK02_STATE, true);
				CreateConditionState(Slasher.SLASHER_ATTACK03_STATE, true);
				CreateConditionState(Slasher.SLASHER_DASH01_STATE, true);
				CreateConditionState(Slasher.SLASHER_DASH02_STATE, true);
				CreateConditionState(Slasher.SLASHER_DASH03_STATE, true);
				CreateConditionState(Slasher.SLASHER_RANGE01_STATE, true);
				CreateConditionState(Slasher.SLASHER_RANGE02_STATE, true);
				CreateConditionState(Slasher.SLASHER_WEAPON_GET_DASH01_STATE, true);
				CreateConditionState(Slasher.SLASHER_WEAPON_GET_DASH02_STATE, true);
				CreateConditionState(Slasher.SLASHER_WEAPON_GET_DASH03_STATE, true);
				CreateConditionState(Slasher.SLASHER_WEAPON_GET_STATE, true);
				CreateConditionState(Slasher.SLASHER_WEAPON_IDLE_STATE, true);
				CreateConditionState(Slasher.SLASHER_WEAPON_RUN_STATE, true);
				CreateConditionState(Slasher.SLASHER_WEAPON_ATTACK01_STATE, true);
				CreateConditionState(Slasher.SLASHER_WEAPON_ATTACK02_STATE, true);
				CreateConditionState(Slasher.SLASHER_WEAPON_DASH01_STATE, true);
				CreateConditionState(Slasher.SLASHER_WEAPON_DASH02_STATE, true);
				CreateConditionState(Slasher.SLASHER_WEAPON_DASH03_STATE, true);
				CreateConditionState(Slasher.SLASHER_WEAPON_RANGE01_STATE, true);
				CreateConditionState(Slasher.SLASHER_WEAPON_ATTACKED_STATE, true);
				CreateConditionState(Slasher.SLASHER_WEAPON_DROP_STATE, true);
				CreateConditionState(Slasher.SLASHER_WEAPON_WAVE_STATE, true);
				CreateConditionState(Slasher.SLASHER_WEAPON_BLADE_STATE, true);
				CreateConditionState(Slasher.SLASHER_STEPBACK_STATE, true);
				CreateConditionState(Slasher.SLASHER_WEAPON_STEPBACK_STATE, true);
				conditionState98.AddCondition(EConditionMessage.EMakeDecisionInIdle);
				ConditionState conditionState100 = CreateConditionState(EnemyBoss.BOSS_RAGE_STATE, "CoopBorn");
				ConditionState nextState2 = CreateConditionState(Enemy.IDLE_STATE, "CoopLongIdle");
				ConditionState conditionState101 = CreateConditionState(Enemy.IDLE_STATE, "CoopRangeIdle");
				ConditionState conditionState102 = CreateConditionState(Enemy.IDLE_STATE, "CoopSkillIdle");
				ConditionState conditionState103 = CreateConditionState(Slasher.SLASHER_DASH_MOVE_STATE1, "CoopDash");
				ConditionState conditionState104 = CreateConditionState(Slasher.SLASHER_DASH_MOVE_STATE2, "CoopDash2");
				conditionState99.AddCondition(Condition.IsAnimationFinished, EConditionMessage.ERageToIdle);
				ConditionState conditionState105 = CreateConditionState(Slasher.SLASHER_ATTACK03_STATE, "CoopRange1");
				ConditionState conditionState106 = CreateConditionState(Slasher.SLASHER_RANGE01_STATE, "CoopRange2");
				ConditionState conditionState107 = CreateConditionState(Slasher.SLASHER_ATTACK03_STATE, "CoopSkill1");
				ConditionState conditionState108 = CreateConditionState(Slasher.SLASHER_RANGE01_STATE, "CoopSkill2");
				conditionState100.AddCondition(Condition.IsAnimationFinished, nextState2);
				conditionState101.AddCondition(EConditionMessage.ETryCoopRangeAttack);
				conditionState105.AddCondition(Condition.IsAnimationFinished, conditionState101);
				conditionState106.AddCondition(Condition.IsAnimationFinished, conditionState101);
				conditionState103.AddStartMessage(EConditionMessage.ELookAtTargetPostion);
				conditionState103.AddCondition(Condition.IsAnimationFinished, conditionState104);
				conditionState104.AddCondition(Condition.IsReachTargetPosition, conditionState102);
				conditionState104.AddCondition(Condition.IsStatePassTime, 4f, conditionState102);
				conditionState102.AddStartMessage(EConditionMessage.EChangeTargetPositionInCoop);
				conditionState102.AddStartMessage(EConditionMessage.ELookAtTargetPostion);
				conditionState102.AddCondition(EConditionMessage.EMakeDecisionInSkillIdle);
				conditionState107.AddCondition(Condition.IsAnimationFinished, conditionState98);
				conditionState107.AddEndMessage(EConditionMessage.ENeedUpdateTargetPosition);
				conditionState108.AddCondition(Condition.IsAnimationFinished, conditionState98);
				conditionState108.AddEndMessage(EConditionMessage.ENeedUpdateTargetPosition);
				mConditionStateDic.Add(new GameUnitKey(gameUnit.GameUnitType, (int)enemy.EnemyType), mCurrentConditionInfo);
			}
			else if (enemy.EnemyType == EnemyType.DEAD_EYE)
			{
				mCurrentConditionInfo = new CreatureConditionInfo();
				ConditionState conditionState109 = CreateConditionState(DeadEye.DEAD_EYE_BORN_STATE, true);
				ConditionState conditionState110 = CreateConditionState(Enemy.IDLE_STATE, true);
				ConditionState conditionState111 = CreateConditionState(Enemy.CATCHING_STATE, true);
				ConditionState conditionState112 = CreateConditionState(Enemy.CATCHING_STATE, "ContinueCatch");
				ConditionState conditionState113 = CreateConditionState(Enemy.CONDITON_CATCH_STATE, "LaserCatch");
				ConditionState conditionState114 = CreateConditionState(DeadEye.DEAD_EYE_MELEE_ATTACK01_STATE, true);
				ConditionState conditionState115 = CreateConditionState(DeadEye.DEAD_EYE_MELEE_ATTACK02_STATE, true);
				ConditionState conditionState116 = CreateConditionState(DeadEye.DEAD_EYE_X_RAY_STATE, true);
				ConditionState conditionState117 = CreateConditionState(DeadEye.DEAD_EYE_4_RAY_STATE, true);
				ConditionState conditionState118 = CreateConditionState(DeadEye.DEAD_EYE_SHOULDER_RAY_STATE, true);
				ConditionState conditionState119 = CreateConditionState(DeadEye.DEAD_EYE_LEAP_ATTACK_STATE, true);
				ConditionState conditionState120 = CreateConditionState(DeadEye.DEAD_EYE_DASH01_ATTACK_STATE, true);
				ConditionState nextState3 = CreateConditionState(DeadEye.DEAD_EYE_DASH02_ATTACK_STATE, true);
				ConditionState conditionState121 = CreateConditionState(DeadEye.DEAD_EYE_DASH03_ATTACK_STATE, true);
				ConditionState conditionState122 = CreateConditionState(DeadEye.DEAD_EYE_ROCK_ATTACK_STATE, true);
				ConditionState conditionState123 = CreateConditionState(EnemyBoss.BOSS_STEP_BACK_STATE, true);
				ConditionState conditionState124 = CreateConditionState(DeadEye.DEAD_EYE_DASH01_ATTACK_STATE, "DashUp");
				ConditionState conditionState125 = CreateConditionState(DeadEye.DEAD_EYE_DASH_UP_STATE, true);
				ConditionState conditionState126 = CreateConditionState(DeadEye.DEAD_EYE_UP_STATE, true);
				ConditionState conditionState127 = CreateConditionState(DeadEye.DEAD_EYE_ROOF_ROCK_ATTACK_STATE, true);
				ConditionState conditionState128 = CreateConditionState(DeadEye.DEAD_EYE_ROOF_IDLE_STATE, true);
				ConditionState conditionState129 = CreateConditionState(DeadEye.DEAD_EYE_ROOF_X_RAY_STATE, true);
				ConditionState conditionState130 = CreateConditionState(DeadEye.DEAD_EYE_ROOF_4_RAY_STATE, true);
				ConditionState conditionState131 = CreateConditionState(DeadEye.DEAD_EYE_ROOF_CONTINUE_RAY_STATE, true);
				ConditionState conditionState132 = CreateConditionState(DeadEye.DEAD_EYE_ROOF_NET_RAY_STATE, true);
				ConditionState conditionState133 = CreateConditionState(DeadEye.DEAD_EYE_ROOF_NET_RAY_STATE);
				ConditionState conditionState134 = CreateConditionState(DeadEye.DEAD_EYE_ROOF_NET_RAY_STATE);
				ConditionState conditionState135 = CreateConditionState(DeadEye.DEAD_EYE_ROOF_IDLE_STATE);
				ConditionState conditionState136 = CreateConditionState(DeadEye.DEAD_EYE_ROOF_IDLE_STATE);
				ConditionState conditionState137 = CreateConditionState(DeadEye.DEAD_EYE_ROOF_CATCH_STATE, true);
				ConditionState conditionState138 = CreateConditionState(DeadEye.DEAD_EYE_ROOF_MOVE_BACK_STATE, true);
				ConditionState conditionState139 = CreateConditionState(DeadEye.DEAD_EYE_ROOF_DOWN_STATE, true);
				ConditionState conditionState140 = CreateConditionState(DeadEye.DEAD_EYE_ROOF_FALL_STATE, "RoofFall");
				ConditionState conditionState141 = CreateConditionState(DeadEye.DEAD_EYE_ROOF_FALL_STATE, "RoofDeadFall");
				ConditionState conditionState142 = CreateConditionState(DeadEye.DEAD_EYE_FALL_RECOVER_STATE, true);
				ConditionState conditionState143 = CreateConditionState(Enemy.GOTHIT_STATE, true);
				ConditionState conditionState144 = CreateConditionState(EnemyBoss.BOSS_RAGE_STATE, true);
				ConditionState conditionState145 = CreateConditionState(Enemy.DEAD_STATE, true);
				ConditionState conditionState146 = CreateConditionState(DeadEye.DEAD_EYE_ROOF_DEAD_STATE, true);
				ConditionState conditionState147 = CreateConditionState(Enemy.FADE_OUT_STATE, true);
				conditionState110.AddCondition(EConditionMessage.EMakeDecisionInIdle);
				conditionState109.AddCondition(Condition.IsAnimationFinished, conditionState110);
				conditionState113.AddCondition(Condition.IsStatePassTime, DeadEyeParameter.CATCH_BETWEEN_LASER_ATTACK_TIME, EConditionMessage.ETryRangeAttack);
				conditionState114.AddCondition(Condition.IsAnimationFinished, conditionState110);
				conditionState115.AddCondition(Condition.IsAnimationFinished, conditionState110);
				conditionState116.AddCondition(Condition.IsAnimationFinished, conditionState110);
				conditionState117.AddCondition(Condition.IsAnimationFinished, conditionState110);
				conditionState118.AddCondition(Condition.IsAnimationFinished, conditionState110);
				conditionState119.AddCondition(Condition.IsAnimationFinished, conditionState110);
				conditionState120.AddCondition(Condition.IsAnimationFinished, nextState3);
				conditionState121.AddCondition(Condition.IsAnimationFinished, conditionState110);
				conditionState122.AddCondition(Condition.IsAnimationFinished, conditionState110);
				conditionState123.AddCondition(Condition.IsAnimationFinished, conditionState110);
				conditionState124.AddCondition(Condition.IsAnimationFinished, conditionState125);
				conditionState125.AddConditonStateFinish(conditionState126);
				conditionState126.AddConditonStateFinish(conditionState127);
				conditionState127.AddCondition(Condition.IsAnimationFinished, conditionState128);
				conditionState128.AddConditonStateFinish(EConditionMessage.ETryAttack2);
				conditionState129.AddCondition(Condition.IsAnimationFinished, conditionState128);
				conditionState130.AddCondition(Condition.IsAnimationFinished, conditionState128);
				conditionState131.AddConditonStateFinish(conditionState128);
				conditionState132.AddCondition(Condition.IsHardMode, Condition.IsAnimationFinished, conditionState135);
				conditionState132.AddCondition(Condition.IsAnimationFinished, conditionState128);
				conditionState135.AddCondition(Condition.IsStatePassTime, 0.5f, conditionState133);
				conditionState133.AddCondition(Condition.IsAnimationFinished, conditionState136);
				conditionState136.AddCondition(Condition.IsStatePassTime, 0.5f, conditionState134);
				conditionState134.AddCondition(Condition.IsAnimationFinished, conditionState128);
				conditionState138.AddCondition(Condition.IsStatePassTime, DeadEyeParameter.ROOF_RUN_BACK_TIME, EConditionMessage.ETryRangeAttack2);
				conditionState137.AddCondition(Condition.IsTargetInRangeSqrHorizontal, 4f, conditionState139);
				conditionState137.AddCondition(Condition.IsStatePassTime, 3f, conditionState139);
				conditionState139.AddConditonStateFinish(conditionState110);
				conditionState140.AddConditonStateFinish(conditionState142);
				conditionState142.AddCondition(Condition.IsAnimationFinished, conditionState110);
				conditionState144.AddCondition(Condition.IsAnimationFinished, conditionState110);
				ConditionState conditionState148 = CreateConditionState(EnemyBoss.BOSS_RAGE_STATE, "CoopBorn");
				ConditionState conditionState149 = CreateConditionState(Enemy.IDLE_STATE, "CoopLongIdle");
				ConditionState conditionState150 = CreateConditionState(Enemy.IDLE_STATE, "CoopRangeIdle");
				ConditionState conditionState151 = CreateConditionState(Enemy.IDLE_STATE, "CoopSkillIdle");
				ConditionState conditionState152 = CreateConditionState(DeadEye.DEAD_EYE_DASH01_ATTACK_STATE, "CoopDash");
				ConditionState conditionState153 = CreateConditionState(DeadEye.DEAD_EYE_DASH_MOVE_STATE, "CoopDash2");
				ConditionState conditionState154 = CreateConditionState(DeadEye.DEAD_EYE_X_RAY_STATE, "CoopRange1");
				ConditionState conditionState155 = CreateConditionState(DeadEye.DEAD_EYE_4_RAY_STATE, "CoopRange2");
				ConditionState conditionState156 = CreateConditionState(DeadEye.DEAD_EYE_SHOULDER_RAY_STATE, "CoopRange3");
				ConditionState conditionState157 = CreateConditionState(DeadEye.DEAD_EYE_X_RAY_STATE, "CoopSkill1");
				ConditionState conditionState158 = CreateConditionState(DeadEye.DEAD_EYE_4_RAY_STATE, "CoopSkill2");
				ConditionState conditionState159 = CreateConditionState(DeadEye.DEAD_EYE_SHOULDER_RAY_STATE, "CoopSkill3");
				ConditionState conditionState160 = CreateConditionState(DeadEye.DEAD_EYE_ROCK_ATTACK_STATE, "CoopSkill4");
				conditionState148.AddCondition(Condition.IsAnimationFinished, conditionState149);
				conditionState149.AddCondition(EConditionMessage.EMakeDecisionInCoopLongIdle);
				conditionState150.AddCondition(EConditionMessage.ETryCoopRangeAttack);
				conditionState154.AddCondition(Condition.IsAnimationFinished, conditionState150);
				conditionState155.AddCondition(Condition.IsAnimationFinished, conditionState150);
				conditionState156.AddCondition(Condition.IsAnimationFinished, conditionState150);
				conditionState152.AddCondition(Condition.IsAnimationFinished, conditionState153);
				conditionState153.AddCondition(Condition.IsReachTargetPosition, conditionState151);
				conditionState153.AddCondition(Condition.IsStatePassTime, 4f, conditionState151);
				conditionState151.AddStartMessage(EConditionMessage.EChangeTargetPositionInCoop);
				conditionState151.AddStartMessage(EConditionMessage.ELookAtTargetPostion);
				conditionState151.AddCondition(EConditionMessage.EMakeDecisionInSkillIdle);
				conditionState157.AddCondition(Condition.IsAnimationFinished, conditionState110);
				conditionState157.AddEndMessage(EConditionMessage.ENeedUpdateTargetPosition);
				conditionState158.AddCondition(Condition.IsAnimationFinished, conditionState110);
				conditionState158.AddEndMessage(EConditionMessage.ENeedUpdateTargetPosition);
				conditionState159.AddCondition(Condition.IsAnimationFinished, conditionState110);
				conditionState159.AddEndMessage(EConditionMessage.ENeedUpdateTargetPosition);
				conditionState160.AddCondition(Condition.IsAnimationFinished, conditionState110);
				conditionState160.AddEndMessage(EConditionMessage.ENeedUpdateTargetPosition);
				mConditionStateDic.Add(new GameUnitKey(gameUnit.GameUnitType, (int)enemy.EnemyType), mCurrentConditionInfo);
			}
			else if (enemy.EnemyType == EnemyType.FROST_STONE)
			{
				mCurrentConditionInfo = new CreatureConditionInfo();
				ConditionState conditionState161 = CreateConditionState(FrostStone.FROST_STONE_BORN_STATE, true);
				ConditionState conditionState162 = CreateConditionState(Enemy.IDLE_STATE, true);
				ConditionState conditionState163 = CreateConditionState(Enemy.CATCHING_STATE, true);
				ConditionState conditionState164 = CreateConditionState(FrostStone.FROST_STONE_MELEE_ATTACK_STATE01, true);
				ConditionState conditionState165 = CreateConditionState(FrostStone.FROST_STONE_MELEE_ATTACK_STATE02, true);
				ConditionState conditionState166 = CreateConditionState(Enemy.GOTHIT_STATE, true);
				ConditionState conditionState167 = CreateConditionState(Enemy.DEAD_STATE, true);
				ConditionState conditionState168 = CreateConditionState(Enemy.FADE_OUT_STATE, true);
				ConditionState conditionState169 = CreateConditionState(EnemyBoss.BOSS_RAGE_STATE, true);
				ConditionState conditionState170 = CreateConditionState(EnemyBoss.BOSS_STEP_BACK_STATE, true);
				ConditionState conditionState171 = CreateConditionState(FrostStone.FROST_STONE_JUMP_ATTACK_STATE01, true);
				ConditionState conditionState172 = CreateConditionState(FrostStone.FROST_STONE_JUMP_ATTACK_STATE02, true);
				ConditionState conditionState173 = CreateConditionState(FrostStone.FROST_STONE_JUMP_ATTACK_STATE03, true);
				ConditionState conditionState174 = CreateConditionState(FrostStone.FROST_STONE_DASH_ATTACK_STATE, true);
				ConditionState conditionState175 = CreateConditionState(FrostStone.FROST_STONE_RANDOM_DASH_ATTACK_STATE01, true);
				ConditionState conditionState176 = CreateConditionState(FrostStone.FROST_STONE_RANDOM_DASH_ATTACK_STATE02, true);
				ConditionState conditionState177 = CreateConditionState(FrostStone.FROST_STONE_RANDOM_DASH_ATTACK_STATE03, true);
				ConditionState conditionState178 = CreateConditionState(FrostStone.FROST_STONE_SMALL_SNOW_BALL_ATTACK_STATE, true);
				ConditionState conditionState179 = CreateConditionState(FrostStone.FROST_STONE_BIG_SNOW_BALL_ATTACK_STATE, true);
				ConditionState conditionState180 = CreateConditionState(FrostStone.FROST_STONE_ICE_SPIKE_ATTACK_STATE, true);
				ConditionState conditionState181 = CreateConditionState(FrostStone.FROST_STONE_ARMOR_UP_STATE, true);
				conditionState161.AddCondition(Condition.IsAnimationFinished, conditionState162);
				conditionState162.AddCondition(EConditionMessage.EMakeDecisionInIdle);
				conditionState170.AddCondition(Condition.IsAnimationFinished, conditionState162);
				conditionState164.AddCondition(Condition.IsAnimationFinished, conditionState162);
				conditionState165.AddCondition(Condition.IsAnimationFinished, conditionState162);
				conditionState171.AddCondition(Condition.IsAnimationFinished, conditionState172);
				conditionState172.AddConditonStateFinish(conditionState173);
				conditionState173.AddCondition(Condition.IsAnimationFinished, conditionState162);
				conditionState174.AddConditonStateFinish(conditionState162);
				conditionState175.AddCondition(Condition.IsAnimationFinished, conditionState176);
				conditionState176.AddConditonStateFinish(conditionState177);
				conditionState177.AddCondition(Condition.IsAnimationFinished, conditionState162);
				conditionState178.AddCondition(Condition.IsAnimationFinished, conditionState162);
				conditionState179.AddCondition(Condition.IsAnimationFinished, conditionState162);
				conditionState180.AddCondition(Condition.IsAnimationFinished, conditionState162);
				conditionState169.AddCondition(Condition.IsAnimationFinished, Condition.NoIceArmor, conditionState181);
				conditionState169.AddCondition(Condition.IsAnimationFinished, conditionState162);
				conditionState181.AddCondition(Condition.IsAnimationFinished, conditionState162);
				mCurrentConditionInfo.mInitState = conditionState162;
				ConditionState conditionState182 = CreateConditionState(EnemyBoss.BOSS_RAGE_STATE, "CoopBorn");
				ConditionState nextState4 = CreateConditionState(Enemy.IDLE_STATE, "CoopLongIdle");
				ConditionState conditionState183 = CreateConditionState(Enemy.IDLE_STATE, "CoopRangeIdle");
				ConditionState conditionState184 = CreateConditionState(Enemy.IDLE_STATE, "CoopSkillIdle");
				ConditionState conditionState185 = CreateConditionState(FrostStone.FROST_STONE_DASH_MOVE_STATE, "CoopDash");
				ConditionState conditionState186 = CreateConditionState(EnemyBoss.BOSS_RAGE_STATE, "CoopRageState");
				ConditionState conditionState187 = CreateConditionState(FrostStone.FROST_STONE_SMALL_SNOW_BALL_ATTACK_STATE, "CoopRange1");
				ConditionState conditionState188 = CreateConditionState(FrostStone.FROST_STONE_BIG_SNOW_BALL_ATTACK_STATE, "CoopRange2");
				ConditionState conditionState189 = CreateConditionState(FrostStone.FROST_STONE_SMALL_SNOW_BALL_ATTACK_STATE, "CoopSkill1");
				ConditionState conditionState190 = CreateConditionState(FrostStone.FROST_STONE_BIG_SNOW_BALL_ATTACK_STATE, "CoopSkill2");
				conditionState186.AddCondition(Condition.IsAnimationFinished, conditionState162);
				conditionState182.AddCondition(Condition.IsAnimationFinished, nextState4);
				conditionState183.AddCondition(EConditionMessage.ETryCoopRangeAttack);
				conditionState187.AddCondition(Condition.IsAnimationFinished, conditionState183);
				conditionState188.AddCondition(Condition.IsAnimationFinished, conditionState183);
				conditionState185.AddStartMessage(EConditionMessage.ELookAtTargetPostion);
				conditionState185.AddCondition(Condition.IsReachTargetPosition, conditionState184);
				conditionState185.AddConditonStateFinish(conditionState184);
				conditionState184.AddStartMessage(EConditionMessage.EChangeTargetPositionInCoop);
				conditionState184.AddStartMessage(EConditionMessage.ELookAtTargetPostion);
				conditionState184.AddCondition(EConditionMessage.EMakeDecisionInSkillIdle);
				conditionState189.AddCondition(Condition.IsAnimationFinished, conditionState162);
				conditionState189.AddEndMessage(EConditionMessage.ENeedUpdateTargetPosition);
				conditionState190.AddCondition(Condition.IsAnimationFinished, conditionState162);
				conditionState190.AddEndMessage(EConditionMessage.ENeedUpdateTargetPosition);
				mConditionStateDic.Add(new GameUnitKey(gameUnit.GameUnitType, (int)enemy.EnemyType), mCurrentConditionInfo);
			}
			else if (enemy.EnemyType == EnemyType.CERBERLISK)
			{
				mCurrentConditionInfo = new CreatureConditionInfo();
				ConditionState conditionState191 = CreateConditionState(Cerberlisk.CERBERLISK_INIT_STATE, true);
				ConditionState conditionState192 = CreateConditionState(Enemy.IDLE_STATE, true);
				ConditionState conditionState193 = CreateConditionState(Enemy.CATCHING_STATE, true);
				ConditionState conditionState194 = CreateConditionState(Cerberlisk.CERBERLISK_MELEE_ATTACK_STATEO1, true);
				ConditionState conditionState195 = CreateConditionState(Cerberlisk.CERBERLISK_MELEE_ATTACK_STATEO2, true);
				ConditionState conditionState196 = CreateConditionState(Enemy.GOTHIT_STATE, true);
				ConditionState conditionState197 = CreateConditionState(Enemy.DEAD_STATE, true);
				ConditionState conditionState198 = CreateConditionState(Enemy.FADE_OUT_STATE, true);
				ConditionState conditionState199 = CreateConditionState(EnemyBoss.BOSS_RAGE_STATE, true);
				ConditionState conditionState200 = CreateConditionState(EnemyBoss.BOSS_STEP_BACK_STATE, true);
				ConditionState conditionState201 = CreateConditionState(Cerberlisk.CERBERLISK_THREE_ATTACK_PRE, true);
				ConditionState conditionState202 = CreateConditionState(Cerberlisk.CERBERLISK_THREE_ATTACK1, true);
				ConditionState conditionState203 = CreateConditionState(Cerberlisk.CERBERLISK_THREE_ATTACK2, true);
				ConditionState conditionState204 = CreateConditionState(Cerberlisk.CERBERLISK_THREE_ATTACK3, true);
				ConditionState conditionState205 = CreateConditionState(Cerberlisk.CERBERLISK_THREE_FIRE_BALL, true);
				ConditionState conditionState206 = CreateConditionState(Cerberlisk.CERBERLISK_BIG_FIRE_BALL, true);
				ConditionState conditionState207 = CreateConditionState(Cerberlisk.CERBERLISK_BREATHE_IN_STATE, true);
				ConditionState conditionState208 = CreateConditionState(Cerberlisk.CERBERLISK_DASH_ATTACK, true);
				ConditionState conditionState209 = CreateConditionState(Cerberlisk.CERBERLISK_PUSH_STATE, true);
				ConditionState conditionState210 = CreateConditionState(Cerberlisk.CERBERLISK_BREATHE_FIRE_BALL_STATE, true);
				ConditionState conditionState211 = CreateConditionState(Cerberlisk.CERBERLISK_CYCLONE_STATE, true);
				conditionState191.AddCondition(Condition.IsAnimationFinished, conditionState192);
				conditionState192.AddCondition(EConditionMessage.EMakeDecisionInIdle);
				conditionState200.AddCondition(Condition.IsAnimationFinished, conditionState192);
				conditionState194.AddCondition(Condition.IsAnimationFinished, conditionState192);
				conditionState195.AddCondition(Condition.IsAnimationFinished, conditionState192);
				conditionState199.AddCondition(Condition.IsAnimationFinished, conditionState192);
				conditionState201.AddCondition(Condition.IsAnimationFinished, conditionState202);
				conditionState202.AddCondition(Condition.IsAnimationFinished, conditionState203);
				conditionState203.AddCondition(Condition.IsAnimationFinished, conditionState204);
				conditionState204.AddCondition(Condition.IsAnimationFinished, conditionState192);
				conditionState205.AddCondition(Condition.IsAnimationFinished, conditionState192);
				conditionState206.AddCondition(Condition.IsAnimationFinished, conditionState192);
				conditionState207.AddCondition(Condition.IsAnimationFinished, conditionState192);
				conditionState208.AddConditonStateFinish(Condition.IsAnimationFinished, conditionState192);
				conditionState209.AddCondition(Condition.IsAnimationFinished, conditionState192);
				conditionState210.AddCondition(Condition.IsAnimationFinished, conditionState192);
				conditionState211.AddCondition(Condition.IsAnimationFinished, conditionState192);
				mCurrentConditionInfo.mInitState = conditionState192;
				mConditionStateDic.Add(new GameUnitKey(gameUnit.GameUnitType, (int)enemy.EnemyType), mCurrentConditionInfo);
			}
			else if (enemy.EnemyType == EnemyType.CRIMSON_QUEEN)
			{
				mCurrentConditionInfo = new CreatureConditionInfo();
				ConditionState conditionState212 = CreateConditionState(CrimsonQueen.CRIMSON_QUEEN_BORN_STATE, true);
				ConditionState conditionState213 = CreateConditionState(Enemy.IDLE_STATE, true);
				ConditionState conditionState214 = CreateConditionState(Enemy.CATCHING_STATE, true);
				ConditionState conditionState215 = CreateConditionState(CrimsonQueen.CRIMSON_QUEEN_MELEE_ATTACK_STATE01, true);
				ConditionState conditionState216 = CreateConditionState(CrimsonQueen.CRIMSON_QUEEN_MELEE_ATTACK_STATE02, true);
				ConditionState conditionState217 = CreateConditionState(CrimsonQueen.CRIMSON_QUEEN_TAIL_LASER_STATE, true);
				ConditionState conditionState218 = CreateConditionState(CrimsonQueen.CRIMSON_QUEEN_FUNNEL_LASER_STATE, true);
				ConditionState conditionState219 = CreateConditionState(CrimsonQueen.CRIMSON_QUEEN_TRACKING_RPG_STATE, true);
				ConditionState conditionState220 = CreateConditionState(CrimsonQueen.CRIMSON_QUEEN_EXPLOSION_SPAWN_STATE, true);
				ConditionState conditionState221 = CreateConditionState(CrimsonQueen.CRIMSON_QUEEN_DASH_ATTACK_STATE, true);
				ConditionState conditionState222 = CreateConditionState(Enemy.GOTHIT_STATE, true);
				ConditionState conditionState223 = CreateConditionState(Enemy.DEAD_STATE, true);
				ConditionState conditionState224 = CreateConditionState(Enemy.FADE_OUT_STATE, true);
				ConditionState conditionState225 = CreateConditionState(EnemyBoss.BOSS_RAGE_STATE, true);
				ConditionState conditionState226 = CreateConditionState(EnemyBoss.BOSS_STEP_BACK_STATE, true);
				conditionState212.AddConditonStateFinish(conditionState213);
				conditionState213.AddCondition(EConditionMessage.EMakeDecisionInIdle);
				conditionState226.AddCondition(Condition.IsAnimationFinished, conditionState213);
				conditionState225.AddEndMessage(EConditionMessage.ESpawnAssistFunnel);
				conditionState225.AddCondition(Condition.IsAnimationFinished, conditionState213);
				conditionState215.AddCondition(Condition.IsAnimationFinished, conditionState213);
				conditionState216.AddCondition(Condition.IsAnimationFinished, conditionState213);
				conditionState217.AddConditonStateFinish(conditionState213);
				conditionState218.AddConditonStateFinish(conditionState213);
				conditionState219.AddConditonStateFinish(conditionState213);
				conditionState220.AddConditonStateFinish(conditionState213);
				conditionState221.AddConditonStateFinish(conditionState213);
				mCurrentConditionInfo.mInitState = conditionState213;
				mConditionStateDic.Add(new GameUnitKey(gameUnit.GameUnitType, (int)enemy.EnemyType), mCurrentConditionInfo);
			}
		}
	}
}
