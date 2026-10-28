using UnityEngine;

public class DeadEyeRoofNetLaserState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public Transform[] AttachTransforms = new Transform[8];

		public GameObject[] LaserObject = new GameObject[8];

		public GameObject[] HintObject = new GameObject[8];

		public LaserScript[] LaserScript = new LaserScript[8];

		public Vector3[] StartDirection = new Vector3[8];

		public Vector3[] Positions = new Vector3[8];

		public Vector3[] Speeds = new Vector3[8];

		public GameObject[] PrepareVFX = new GameObject[4];

		public int InnerState;
	}

	public const int LaserNum = 8;

	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation(AnimationString.DEAD_EYE_ROOF_RANGE4, WrapMode.ClampForever);
		enemy.LookAtTargetHorizontal();
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		if (myData.AttachTransforms[0] == null)
		{
			myData.AttachTransforms[0] = enemy.GetTransform().Find(BoneName.DEADEYE_LASER_TAIL_POINT_L);
			myData.AttachTransforms[1] = enemy.GetTransform().Find(BoneName.DEADEYE_LASER_TAIL_POINT_L);
			myData.AttachTransforms[2] = enemy.GetTransform().Find(BoneName.DEADEYE_LASER_BACK_POINT_L);
			myData.AttachTransforms[3] = enemy.GetTransform().Find(BoneName.DEADEYE_LASER_BACK_POINT_L);
			myData.AttachTransforms[4] = enemy.GetTransform().Find(BoneName.DEADEYE_LASER_TAIL_POINT_R);
			myData.AttachTransforms[5] = enemy.GetTransform().Find(BoneName.DEADEYE_LASER_TAIL_POINT_R);
			myData.AttachTransforms[6] = enemy.GetTransform().Find(BoneName.DEADEYE_LASER_BACK_POINT_R);
			myData.AttachTransforms[7] = enemy.GetTransform().Find(BoneName.DEADEYE_LASER_BACK_POINT_R);
		}
		for (int i = 0; i < 8; i++)
		{
			myData.LaserObject[i] = Pool.sGetInstance().Allocate("SW2_Effect/B2_Roof_Range04");
			myData.HintObject[i] = Pool.sGetInstance().Allocate("SW2_Effect/B2_Roof_Range04_Ground");
			myData.LaserObject[i].SetActive(false);
			myData.HintObject[i].SetActive(false);
		}
		for (int j = 0; j < 4; j++)
		{
			myData.PrepareVFX[j] = Pool.sGetInstance().Allocate("SW2_Effect/B2_Laser_Charge_small");
			myData.PrepareVFX[j].transform.parent = myData.AttachTransforms[j * 2];
			myData.PrepareVFX[j].transform.localPosition = Vector3.zero;
		}
		myData.InnerState = 0;
		float num = Random.Range(30f, 45f);
		Vector3 vector = Quaternion.Euler(0f, num, 0f) * Vector3.left;
		Vector3 vector2 = Quaternion.Euler(0f, 0f - num, 0f) * Vector3.right;
		myData.StartDirection[0] = vector * DeadEyeParameter.DEAD_EYE_NET_RAY_SPAWN_DISTANCE;
		myData.StartDirection[1] = vector * DeadEyeParameter.DEAD_EYE_NET_RAY_SPAWN_DISTANCE * 2f;
		myData.StartDirection[2] = vector * DeadEyeParameter.DEAD_EYE_NET_RAY_SPAWN_DISTANCE * 3f;
		myData.StartDirection[3] = vector * DeadEyeParameter.DEAD_EYE_NET_RAY_SPAWN_DISTANCE * 4f;
		myData.StartDirection[4] = vector2 * DeadEyeParameter.DEAD_EYE_NET_RAY_SPAWN_DISTANCE;
		myData.StartDirection[5] = vector2 * DeadEyeParameter.DEAD_EYE_NET_RAY_SPAWN_DISTANCE * 2f;
		myData.StartDirection[6] = vector2 * DeadEyeParameter.DEAD_EYE_NET_RAY_SPAWN_DISTANCE * 3f;
		myData.StartDirection[7] = vector2 * DeadEyeParameter.DEAD_EYE_NET_RAY_SPAWN_DISTANCE * 4f;
		enemy.PlaySound(AudioName.DEADEYE_LASER_LONG0);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.LookAtTargetHorizontal();
		MyData myData = enemy.StateData[this] as MyData;
		if (myData.InnerState == 0)
		{
			if (enemy.CurrentAnimTime >= DeadEyeParameter.DEAD_EYE_NET_RAY_CHECK_TIME.x)
			{
				myData.InnerState++;
				for (int i = 0; i < 8; i++)
				{
					myData.Positions[i] = enemy.GetPosition() + enemy.GetTransform().TransformDirection(myData.StartDirection[i]);
					myData.Positions[i].y = enemy.GetTargetPosition().y;
				}
				Vector3 vector = enemy.GetTargetPosition() - myData.Positions[1];
				vector.y = 0f;
				vector = vector.normalized;
				Vector3 vector2 = enemy.GetTargetPosition() - myData.Positions[5];
				vector2.y = 0f;
				vector2 = vector2.normalized;
				for (int j = 0; j < 8; j++)
				{
					if (j < 4)
					{
						myData.Speeds[j] = vector * DeadEyeParameter.DEAD_EYE_NET_RAY_SPEED;
					}
					else
					{
						myData.Speeds[j] = vector2 * DeadEyeParameter.DEAD_EYE_NET_RAY_SPEED;
					}
					myData.HintObject[j].transform.position = myData.Positions[j];
					myData.HintObject[j].transform.forward = myData.Speeds[j];
				}
				for (int k = 0; k < 8; k++)
				{
					myData.LaserObject[k].SetActive(true);
					myData.HintObject[k].SetActive(true);
				}
				for (int l = 0; l < 4; l++)
				{
					Pool.sGetInstance().Deallocate(myData.PrepareVFX[l]);
				}
				enemy.PlaySound(AudioName.DEADEYE_ROOF_RANGE04);
			}
		}
		else if (myData.InnerState == 1)
		{
			if (enemy.CurrentAnimTime >= DeadEyeParameter.DEAD_EYE_NET_RAY_LASER_DISAPPEAR_TIME)
			{
				myData.InnerState++;
				for (int m = 0; m < 8; m++)
				{
					myData.LaserObject[m].SetActive(false);
				}
			}
		}
		else if (myData.InnerState == 2 && enemy.CurrentAnimTime >= DeadEyeParameter.DEAD_EYE_NET_RAY_CHECK_TIME.y)
		{
			myData.InnerState++;
			for (int n = 0; n < 8; n++)
			{
				myData.HintObject[n].SetActive(false);
				GameObject gameObject = Pool.sAllocate("SW2_Effect/B2_Roof_Range04_Groundout", myData.HintObject[n].transform.position);
				gameObject.transform.forward = myData.HintObject[n].transform.forward;
				gameObject.GetComponent<LaserScript>().SetDamageInfo(DeadEyeParameter.GetInstance(), 12);
				gameObject.GetComponent<LaserScript>().Attacker = enemy;
			}
			enemy.PlaySound(AudioName.DEADEYE_ROOF_RANGE04_END);
		}
		for (int num = 0; num < 8; num++)
		{
			if (myData.LaserObject[num].activeSelf)
			{
				myData.LaserObject[num].transform.position = myData.AttachTransforms[num].position;
				myData.Positions[num] += myData.Speeds[num] * Time.deltaTime;
				myData.LaserObject[num].transform.LookAt(myData.Positions[num]);
			}
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
}
