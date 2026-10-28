using UnityEngine;

public class EnemyPlayAnimState : EnemyState
{
	public string AnimName;

	public bool IsLoop;

	public bool IsLookAtTarget;

	public string Sound;

	public float SoundTime;

	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation(AnimName, (!IsLoop) ? WrapMode.ClampForever : WrapMode.Loop);
		if (IsLookAtTarget)
		{
			enemy.LookAtTargetHorizontal();
		}
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		if (Sound != null && enemy.IsAnimationTriggered(SoundTime))
		{
			enemy.PlaySound(Sound);
		}
	}
}
