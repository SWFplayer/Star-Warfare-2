using UnityEngine;

public class DeadEyeRoofContinueLaserState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public Transform[] AttachTransforms = new Transform[6];

		public GameObject[] LaserObject = new GameObject[6];

		public GameObject[] HintObject = new GameObject[6];

		public LaserScript[] LaserScript = new LaserScript[6];

		public Vector3[] StartDirection = new Vector3[6];

		public Vector3[] Positions = new Vector3[6];

		public Vector3[] Speeds = new Vector3[6];

		public float[] StartTime = new float[6];

		public GameObject[] PrepareVFX = new GameObject[6];

		public int InnerState;

		public int InnerVFXState;
	}

	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation(AnimationString.DEAD_EYE_ROOF_RANGE3, WrapMode.ClampForever);
		enemy.LookAtTargetHorizontal();
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		if (myData.AttachTransforms[0] == null)
		{
			myData.AttachTransforms[0] = enemy.GetTransform().Find(BoneName.DEADEYE_LASER_BACK_POINT_L);
			myData.AttachTransforms[1] = enemy.GetTransform().Find(BoneName.DEADEYE_LASER_BACK_POINT_R);
			myData.AttachTransforms[2] = enemy.GetTransform().Find(BoneName.DEADEYE_LASER_TAIL_POINT_R);
			myData.AttachTransforms[3] = enemy.GetTransform().Find(BoneName.DEADEYE_LASER_TAIL_POINT_L);
			myData.AttachTransforms[4] = enemy.GetTransform().Find(BoneName.DEADEYE_LASER_BACK_POINT_L);
			myData.AttachTransforms[5] = enemy.GetTransform().Find(BoneName.DEADEYE_LASER_BACK_POINT_R);
		}
		for (int i = 0; i < 6; i++)
		{
			myData.LaserObject[i] = Pool.sGetInstance().Allocate("SW2_Effect/B2_Roof_Range03");
			myData.HintObject[i] = Pool.sGetInstance().Allocate("SW2_Effect/B2_Range01_LaserIndic");
			if (myData.LaserObject[i].GetComponent<LaserScript>() == null)
			{
				myData.LaserObject[i].AddComponent<LaserScript>();
			}
			myData.LaserObject[i].GetComponent<LaserScript>().SetDamageInfo(DeadEyeParameter.GetInstance(), 11);
			myData.LaserObject[i].GetComponent<LaserScript>().Attacker = enemy;
			myData.LaserObject[i].SetActive(false);
			myData.HintObject[i].SetActive(false);
		}
		myData.InnerState = 0;
		myData.InnerVFXState = 0;
		float num = Random.Range(30f, 45f);
		Vector3 vector = Quaternion.Euler(0f, num, 0f) * Vector3.left;
		Vector3 vector2 = Quaternion.Euler(0f, 0f - num, 0f) * Vector3.right;
		myData.StartDirection[0] = vector * DeadEyeParameter.DEAD_EYE_CONTINUE_RAY_SPAWN_DISTANCE;
		myData.StartDirection[1] = vector2 * DeadEyeParameter.DEAD_EYE_CONTINUE_RAY_SPAWN_DISTANCE;
		myData.StartDirection[2] = vector * DeadEyeParameter.DEAD_EYE_CONTINUE_RAY_SPAWN_DISTANCE * 2f;
		myData.StartDirection[3] = vector2 * DeadEyeParameter.DEAD_EYE_CONTINUE_RAY_SPAWN_DISTANCE * 2f;
		myData.StartDirection[4] = vector * DeadEyeParameter.DEAD_EYE_CONTINUE_RAY_SPAWN_DISTANCE * 3f;
		myData.StartDirection[5] = vector2 * DeadEyeParameter.DEAD_EYE_CONTINUE_RAY_SPAWN_DISTANCE * 3f;
		for (int j = 0; j < 4; j++)
		{
			myData.PrepareVFX[j] = Pool.sGetInstance().Allocate("SW2_Effect/B2_Laser_Charge_small");
			myData.PrepareVFX[j].transform.parent = myData.AttachTransforms[j];
			myData.PrepareVFX[j].transform.localPosition = Vector3.zero;
			myData.PrepareVFX[j].SetActive(true);
		}
		for (int k = 4; k < 6; k++)
		{
			myData.PrepareVFX[k] = Pool.sGetInstance().Allocate("SW2_Effect/B2_Laser_Charge_small");
			myData.PrepareVFX[k].transform.parent = myData.AttachTransforms[k];
			myData.PrepareVFX[k].transform.localPosition = Vector3.zero;
			myData.PrepareVFX[k].SetActive(false);
		}
		enemy.PlaySound(AudioName.DEADEYE_LASER_LONG0);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.LookAtTargetHorizontal();
		MyData myData = enemy.StateData[this] as MyData;
		if (myData.InnerState < 6 && !myData.HintObject[myData.InnerState].activeSelf && (myData.InnerState == 0 || !myData.LaserObject[myData.InnerState - 1].activeSelf))
		{
			myData.Positions[myData.InnerState] = enemy.GetPosition() + enemy.GetTransform().TransformDirection(myData.StartDirection[myData.InnerState]);
			myData.Positions[myData.InnerState].y = enemy.GetTargetPosition().y;
			myData.Speeds[myData.InnerState] = enemy.GetTargetPosition() - myData.Positions[myData.InnerState];
			myData.Speeds[myData.InnerState].y = 0f;
			myData.Speeds[myData.InnerState] = myData.Speeds[myData.InnerState].normalized * DeadEyeParameter.DEAD_EYE_CONTINUE_RAY_SPEED;
			myData.HintObject[myData.InnerState].transform.position = myData.Positions[myData.InnerState];
			myData.HintObject[myData.InnerState].transform.forward = myData.Speeds[myData.InnerState];
			myData.HintObject[myData.InnerState].SetActive(true);
		}
		if (myData.InnerState < 6 && enemy.CurrentAnimTime >= DeadEyeParameter.DEAD_EYE_CONTINUE_RAY_SHOT_TIME[myData.InnerState])
		{
			enemy.PlaySound(AudioName.DEADEYE_LASER_LONG1);
			myData.LaserObject[myData.InnerState].SetActive(true);
			myData.StartTime[myData.InnerState] = Time.time;
			Pool.sGetInstance().Deallocate(myData.PrepareVFX[myData.InnerState]);
			myData.InnerState++;
		}
		for (int i = 0; i < 6; i++)
		{
			if (myData.LaserObject[i].activeSelf)
			{
				if (Time.time - myData.StartTime[i] > DeadEyeParameter.DEAD_EYE_CONTINUE_RAY_DURATION)
				{
					myData.LaserObject[i].SetActive(false);
					myData.HintObject[i].SetActive(false);
				}
				else
				{
					myData.LaserObject[i].transform.position = myData.AttachTransforms[i].position;
					myData.Positions[i] += myData.Speeds[i] * Time.deltaTime;
					myData.LaserObject[i].transform.LookAt(myData.Positions[i]);
				}
			}
		}
		if (myData.InnerVFXState < 2 && enemy.CurrentAnimTime >= DeadEyeParameter.DEAD_EYE_CONTINUE_RAY_SHOW_PREPARE_VFX_TIME[myData.InnerVFXState])
		{
			myData.PrepareVFX[myData.InnerVFXState + 4].SetActive(true);
			myData.InnerVFXState++;
		}
	}

	public override void End(Enemy enemy)
	{
		MyData myData = enemy.StateData[this] as MyData;
		GameObject[] laserObject = myData.LaserObject;
		foreach (GameObject go in laserObject)
		{
			Pool.sGetInstance().Deallocate(go);
		}
		GameObject[] hintObject = myData.HintObject;
		foreach (GameObject go2 in hintObject)
		{
			Pool.sGetInstance().Deallocate(go2);
		}
		GameObject[] prepareVFX = myData.PrepareVFX;
		foreach (GameObject go3 in prepareVFX)
		{
			Pool.sGetInstance().Deallocate(go3);
		}
	}

	public override bool IsFinish(GameUnit gameUnit)
	{
		MyData myData = gameUnit.StateData[this] as MyData;
		return myData.InnerState >= 6 && !myData.LaserObject[5].activeSelf && gameUnit.CurrentAnimTime >= 1f;
	}
}
