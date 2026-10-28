using UnityEngine;

public class DeadEyeRockAttackState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public int InnerState;

		public float[] Distances;

		public Vector3 StartDirecion;
	}

	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation(AnimationString.DEAD_EYE_ROCK_ATTACK);
		enemy.LookAtTargetHorizontal();
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		myData.InnerState = 0;
		myData.Distances = new float[DeadEyeParameter.ROCK_ATTACK_RANDOM_OFFSET.Length];
		for (int i = 0; i < DeadEyeParameter.ROCK_ATTACK_RANDOM_OFFSET.Length; i++)
		{
			myData.Distances[i] = Random.Range((i != 0) ? DeadEyeParameter.ROCK_ATTACK_RANDOM_OFFSET[i - 1] : 0f, DeadEyeParameter.ROCK_ATTACK_RANDOM_OFFSET[i]);
		}
		myData.StartDirecion = new Vector3(Random.value, 0f, Random.value);
		myData.StartDirecion = myData.StartDirecion.normalized;
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData myData = enemy.StateData[this] as MyData;
		if (myData.InnerState == 0 && Time.time - enemy.StateStartTime >= DeadEyeParameter.ShowShadowTime)
		{
			myData.InnerState++;
			GameObject original = Resources.Load("SW2_Effect/B2_RockAttack_Fall") as GameObject;
			for (int i = 0; i < DeadEyeParameter.ROCK_ATTACK_TIME.Length; i++)
			{
				Vector3 vector = Quaternion.Euler(0f, (float)i * 360f / (float)DeadEyeParameter.ROCK_ATTACK_TIME.Length, 0f) * myData.StartDirecion * myData.Distances[i];
				GameObject gameObject = Object.Instantiate(original, enemy.GetTargetPosition() + vector, Quaternion.identity) as GameObject;
				DelayActivateScript component = gameObject.GetComponent<DelayActivateScript>();
				component.ActivateTime = DeadEyeParameter.ROCK_ATTACK_TIME[i] - DeadEyeParameter.ShowShadowTime;
				component.Reset();
				LaserScript[] componentsInChildren = gameObject.GetComponentsInChildren<LaserScript>(true);
				componentsInChildren[0].SetDamageInfo(DeadEyeParameter.GetInstance(), 6);
				componentsInChildren[0].Attacker = enemy;
				GameObject gameObject2 = componentsInChildren[0].gameObject;
				PlaySoundIn playSoundIn = gameObject2.GetComponent<PlaySoundIn>();
				if (playSoundIn == null)
				{
					playSoundIn = gameObject2.AddComponent<PlaySoundIn>();
				}
				playSoundIn.InTime = 0.3f;
				playSoundIn.Sound = AudioName.DEADEYE_ROCK_FALL;
			}
		}
		if (enemy.IsAnimationTriggered(0.2f) || enemy.IsAnimationTriggered(0.67f))
		{
			enemy.PlaySound(AudioName.DEADEYE_ROCK_ATTACK);
		}
	}
}
