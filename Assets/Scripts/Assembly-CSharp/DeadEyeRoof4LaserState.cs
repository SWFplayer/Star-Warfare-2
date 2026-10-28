using UnityEngine;

public class DeadEyeRoof4LaserState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public Transform[] AttachTransforms = new Transform[4];

		public GameObject[] HintObject = new GameObject[4];

		public GameObject[] LaserObject = new GameObject[4];

		public Vector3[] Positions = new Vector3[4];

		public Vector3[] Speeds = new Vector3[4];

		public GameObject[] PrepareVFX = new GameObject[4];

		public int InnerState;
	}

	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation(AnimationString.DEAD_EYE_ROOF_RANGE2, WrapMode.ClampForever);
		enemy.LookAtTargetHorizontal();
		if (!enemy.StateData.ContainsKey(this))
		{
			enemy.StateData.Add(this, new MyData());
		}
		MyData myData = enemy.StateData[this] as MyData;
		if (myData.AttachTransforms[0] == null)
		{
			myData.AttachTransforms[0] = enemy.GetTransform().Find(BoneName.DEADEYE_LASER_BACK_POINT_L);
			myData.AttachTransforms[1] = enemy.GetTransform().Find(BoneName.DEADEYE_LASER_TAIL_POINT_L);
			myData.AttachTransforms[2] = enemy.GetTransform().Find(BoneName.DEADEYE_LASER_TAIL_POINT_R);
			myData.AttachTransforms[3] = enemy.GetTransform().Find(BoneName.DEADEYE_LASER_BACK_POINT_R);
		}
		for (int i = 0; i < 4; i++)
		{
			myData.LaserObject[i] = Pool.sGetInstance().Allocate("SW2_Effect/B2_Range02_Laser");
			myData.HintObject[i] = Pool.sGetInstance().Allocate("SW2_Effect/B2_Range01_LaserIndic");
			if (myData.LaserObject[i].GetComponent<LaserScript>() == null)
			{
				myData.LaserObject[i].AddComponent<LaserScript>();
			}
			myData.LaserObject[i].GetComponent<LaserScript>().SetDamageInfo(DeadEyeParameter.GetInstance(), 10);
			myData.LaserObject[i].GetComponent<LaserScript>().Attacker = enemy;
			myData.LaserObject[i].SetActive(false);
			myData.PrepareVFX[i] = Pool.sGetInstance().Allocate("SW2_Effect/B2_Laser_Charge_small");
			myData.PrepareVFX[i].transform.parent = myData.AttachTransforms[i];
			myData.PrepareVFX[i].transform.localPosition = Vector3.zero;
		}
		myData.InnerState = 0;
		float num = Random.Range(-3f, 3f);
		float num2 = 5f;
		float num3 = 5f;
		Vector3 vector = enemy.GetPosition() - enemy.GetTargetPosition();
		vector.y = 0f;
		vector = vector.normalized;
		float max = Random.Range(-0.6f, 0.6f) * 2f;
		max = Mathf.Clamp(-0.5f, 0.5f, max);
		for (int j = 0; j < 4; j++)
		{
			myData.Positions[j] = enemy.GetTargetPosition() + vector * num3;
			myData.Positions[j] += -enemy.GetTransform().right * ((float)j - 1.5f + max) * num2;
			myData.Speeds[j] = enemy.GetTransform().forward;
			myData.Speeds[j].y = 0f;
			myData.Speeds[j] = myData.Speeds[j].normalized;
			myData.Speeds[j] *= (float)DeadEyeParameter.DEAD_EYE_ROOF_4_RAY_SPEED;
			myData.HintObject[j].transform.position = myData.Positions[j];
			myData.HintObject[j].transform.forward = myData.Speeds[j];
			myData.HintObject[j].SetActive(true);
		}
		enemy.PlaySound(AudioName.DEADEYE_LASER_LONG0);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		MyData myData = enemy.StateData[this] as MyData;
		if (myData.InnerState == 0)
		{
			if (enemy.CurrentAnimTime >= DeadEyeParameter.DEAD_EYE_ROOF_4_RAY_CHECK_TIME.x)
			{
				enemy.PlaySound(AudioName.DEADEYE_LASER_LONG1);
				for (int i = 0; i < 4; i++)
				{
					myData.LaserObject[i].transform.position = myData.AttachTransforms[i].position;
					myData.LaserObject[i].transform.LookAt(myData.Positions[i]);
					myData.LaserObject[i].SetActive(true);
					Pool.sGetInstance().Deallocate(myData.PrepareVFX[i]);
				}
				myData.InnerState++;
			}
		}
		else
		{
			if (myData.InnerState != 1)
			{
				return;
			}
			for (int j = 0; j < 4; j++)
			{
				myData.LaserObject[j].transform.position = myData.AttachTransforms[j].position;
				myData.Positions[j] += myData.Speeds[j] * Time.deltaTime;
				myData.LaserObject[j].transform.LookAt(myData.Positions[j]);
			}
			if (enemy.CurrentAnimTime >= DeadEyeParameter.DEAD_EYE_ROOF_4_RAY_CHECK_TIME.y)
			{
				myData.InnerState++;
				for (int k = 0; k < 4; k++)
				{
					myData.LaserObject[k].SetActive(false);
					myData.HintObject[k].SetActive(false);
				}
			}
		}
	}

	public override void End(Enemy enemy)
	{
		MyData myData = enemy.StateData[this] as MyData;
		for (int i = 0; i < 4; i++)
		{
			Pool.sGetInstance().Deallocate(myData.LaserObject[i]);
			Pool.sGetInstance().Deallocate(myData.HintObject[i]);
			Pool.sGetInstance().Deallocate(myData.PrepareVFX[i]);
		}
	}
}
