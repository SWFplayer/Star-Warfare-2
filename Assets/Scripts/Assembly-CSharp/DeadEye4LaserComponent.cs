using UnityEngine;

public class DeadEye4LaserComponent : GameUnitStateComponent
{
	public class MyData : GameUnitStateData
	{
		public Transform[] AttachTransforms = new Transform[6];

		public int InnerState;
	}

	public void Start(GameUnit gameUnit, GameUnitState state)
	{
		if (!gameUnit.StateData.ContainsKey(state))
		{
			gameUnit.StateData.Add(state, new MyData());
		}
		MyData myData = gameUnit.StateData[state] as MyData;
		myData.InnerState = 0;
		myData.AttachTransforms[0] = gameUnit.GetTransform().Find(BoneName.DEADEYE_HEAD_POINT[0]);
		myData.AttachTransforms[1] = gameUnit.GetTransform().Find(BoneName.DEADEYE_HEAD_POINT[1]);
		myData.AttachTransforms[2] = gameUnit.GetTransform().Find(BoneName.DEADEYE_HEAD_POINT[2]);
		myData.AttachTransforms[3] = gameUnit.GetTransform().Find(BoneName.DEADEYE_HEAD_POINT[3]);
		myData.AttachTransforms[4] = gameUnit.GetTransform().Find(BoneName.DEADEYE_HEAD_POINT[4]);
		myData.AttachTransforms[5] = gameUnit.GetTransform().Find(BoneName.DEADEYE_HEAD_POINT[5]);
		UpdateTargetPosition(gameUnit, 100f, true);
	}

	public void Update(GameUnit gameUnit, GameUnitState state, bool isModifyRadius = false)
	{
		MyData myData = gameUnit.StateData[state] as MyData;
		UpdateTargetPosition(gameUnit, 100f, false);
		if (myData.InnerState < DeadEyeParameter.DEAD_EYE_4_RAY_SHOT_TIME.Length && gameUnit.CurrentAnimTime >= DeadEyeParameter.DEAD_EYE_4_RAY_SHOT_TIME[myData.InnerState])
		{
			myData.InnerState++;
			GameObject original = Resources.Load("SW2_Effect/B2_Small_Laser") as GameObject;
			GameObject gameObject = Object.Instantiate(original, myData.AttachTransforms[myData.InnerState - 1].position, Quaternion.identity) as GameObject;
			if (gameUnit.GameUnitType == EGameUnitType.ENEMY)
			{
				gameObject.transform.LookAt(gameUnit.GetTargetPosition() + Vector3.up);
			}
			else
			{
				gameObject.transform.LookAt(gameUnit.GetTargetPosition());
			}
			gameObject.transform.Translate(0f, 0f, -0.5f, Space.Self);
			DeadEyeSmallLaser component = gameObject.GetComponent<DeadEyeSmallLaser>();
			component.Attacker = gameUnit;
			component.Speed = component.transform.forward * DeadEyeParameter.DEAD_EYE_4_RAY_SPEED;
			component.SetDamageInfo(DeadEyeParameter.GetInstance(), 4);
			component.Reset();
			if (isModifyRadius)
			{
				component.SetRadius(1.5f);
			}
			gameUnit.PlaySound(AudioName.DEADEYE_LASER_SHORT);
		}
	}

	public void End(GameUnit gameUnit, GameUnitState state)
	{
	}
}
