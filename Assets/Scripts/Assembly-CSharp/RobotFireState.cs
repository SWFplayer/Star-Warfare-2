using UnityEngine;

public class RobotFireState : GameUnitState
{
	public override void Start(GameUnit gameUnit)
	{
		Robot robot = (Robot)gameUnit;
		if (robot.GetRobotType() == ERobotType.XIAOHUOBAN)
		{
			robot.AttackDuration = Random.Range(Robot.ATTACK_DURATION.x, Robot.ATTACK_DURATION.y);
		}
		if (robot.GetWeapon() != null)
		{
			robot.GetWeapon().StartAttackState();
		}
	}

	public override void Update(GameUnit gameUnit)
	{
		Robot robot = gameUnit as Robot;
		InputController inputController = robot.inputController;
		GameUnit target = robot.GetTarget();
		Weapon weapon = robot.GetWeapon();
		if (weapon == null)
		{
			return;
		}
		inputController.inputInfo.fire = (inputController.fire = weapon.GetInputFireStatus());
		if (target != null && target.InPlayingState())
		{
			Vector3 value = target.GetPosition() - robot.GetPosition();
			if (weapon.GetWeaponType() == WeaponType.RPG || weapon.GetWeaponType() == WeaponType.TrackingRPG || weapon.GetWeaponType() == WeaponType.TwinRPG || weapon.GetWeaponType() == WeaponType.SplitRPG)
			{
				value += new Vector3(0f, -0.5f, 0f);
			}
			robot.AimDirection = Vector3.Normalize(value);
		}
		Vector3 aimDirection = robot.AimDirection;
		aimDirection.y = 0f;
		robot.GetTransform().LookAt(robot.GetPosition() + aimDirection);
		bool flag = ((robot.GetSkills().GetSkill(SkillsType.FLY) > 0f) ? true : false);
		if (robot.IsLocal())
		{
			if (inputController.inputInfo.IsMoving())
			{
				if (flag)
				{
					AudioManager.GetInstance().StopSound("float_idle");
					AudioManager.GetInstance().PlaySoundSingleAt("_Old/_Audio/float_move", robot.GetTransform().position);
				}
				else
				{
					AudioManager.GetInstance().PlaySoundSingleLoop(AudioName.PLAYER_FOOTSTEP, robot.GetObject(), robot.GetTransform());
				}
			}
			else if (flag)
			{
				AudioManager.GetInstance().StopSound("float_move");
				AudioManager.GetInstance().PlaySoundSingleAt("_Old/_Audio/float_idle", robot.GetTransform().position);
			}
			else
			{
				AudioManager.GetInstance().StopSound(AudioName.PLAYER_FOOTSTEP, robot.GetObject());
			}
		}
		robot.GetWeapon().PlaySound();
		robot.GetWeapon().Loop(Time.deltaTime);
		robot.GetWeapon().AutoDestructEffect();
		robot.GetWeapon().UpdateAttackState(Time.deltaTime);
		robot.GetWeapon().PlayFireAnim();
	}

	public override void End(GameUnit gameUnit)
	{
		Robot robot = (Robot)gameUnit;
		if (robot.GetWeapon() != null)
		{
			robot.GetWeapon().EndAttackState();
		}
	}
}
