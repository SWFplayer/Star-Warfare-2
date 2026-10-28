using UnityEngine;

public class RobotIdleState : GameUnitState
{
	public override void Start(GameUnit gameUnit)
	{
		Robot robot = (Robot)gameUnit;
		if (robot.GetRobotType() == ERobotType.XIAOHUOBAN)
		{
			robot.AttackIdleDuration = Random.Range(Robot.ATTACK_IDLE_DURATION.x, Robot.ATTACK_IDLE_DURATION.y);
		}
		if (robot.GetWeapon() != null)
		{
			robot.GetWeapon().StartIdleState();
		}
	}

	public override void Update(GameUnit gameUnit)
	{
		Robot robot = gameUnit as Robot;
		InputController inputController = robot.inputController;
		if (robot.GetWeapon() == null)
		{
			return;
		}
		robot.GetWeapon().StopFire();
		robot.GetWeapon().AutoDestructEffect();
		robot.GetWeapon().StopSound();
		robot.GetWeapon().PlayIdleAnim();
		bool flag = ((robot.GetSkills().GetSkill(SkillsType.FLY) > 0f) ? true : false);
		if (inputController.inputInfo.IsMoving())
		{
			if (flag)
			{
				robot.PlayAnimation(AnimationString.GetFlyAniString(inputController.inputInfo.dir), WrapMode.Loop);
				robot.PlayAnimation(AnimationString.Fly + "_" + robot.GetWeaponAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
				AudioManager.GetInstance().StopSound("float_idle");
				AudioManager.GetInstance().PlaySoundSingleAt("_Old/_Audio/float_move", robot.GetTransform().position);
			}
			else
			{
				robot.PlayAnimationAllLayers(AnimationString.Stand + "_" + robot.GetWeaponAnimationSuffix() + "_" + AnimationString.Run, WrapMode.Loop);
				AudioManager.GetInstance().PlaySoundSingleLoop(AudioName.PLAYER_FOOTSTEP, robot.GetObject(), robot.GetTransform());
			}
		}
		else if (flag)
		{
			robot.PlayAnimation(AnimationString.FlyIdle, WrapMode.Loop);
			robot.PlayAnimation(AnimationString.Fly + "_" + robot.GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
			AudioManager.GetInstance().StopSound("float_move");
			AudioManager.GetInstance().PlaySoundSingleAt("_Old/_Audio/float_idle", robot.GetTransform().position);
		}
		else
		{
			robot.PlayAnimationAllLayers(AnimationString.Stand + "_" + robot.GetWeaponAnimationSuffix() + "_" + AnimationString.Idle, WrapMode.Loop);
			AudioManager.GetInstance().StopSound(AudioName.PLAYER_FOOTSTEP, robot.GetObject());
		}
	}

	public override void End(GameUnit gameUnit)
	{
	}
}
