using UnityEngine;

public class DeadEyeRoofXLaserState : EnemyState
{
	public class MyData : GameUnitStateData
	{
		public Transform[] AttachTransforms = new Transform[2];

		public GameObject[] LaserObject = new GameObject[2];

		public Vector3[] Positions = new Vector3[2];

		public Vector3[] Speeds = new Vector3[2];

		public GameObject[] PrepareVFX = new GameObject[2];

		public int InnerState;
	}

	public override void Start(Enemy enemy)
	{
		enemy.PlayAnimation(AnimationString.DEAD_EYE_ROOF_RANGE1, WrapMode.ClampForever);
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
		}
		for (int i = 0; i < 2; i++)
		{
			myData.LaserObject[i] = Pool.sGetInstance().Allocate("SW2_Effect/B2_Range01_Laser");
			if (myData.LaserObject[i].GetComponent<LaserScript>() == null)
			{
				myData.LaserObject[i].AddComponent<LaserScript>();
			}
			myData.LaserObject[i].GetComponent<LaserScript>().SetDamageInfo(DeadEyeParameter.GetInstance(), 9);
			myData.LaserObject[i].GetComponent<LaserScript>().Attacker = enemy;
			myData.LaserObject[i].SetActive(false);
			myData.PrepareVFX[i] = Pool.sGetInstance().Allocate("SW2_Effect/B2_Laser_Charge_small");
			myData.PrepareVFX[i].transform.parent = myData.AttachTransforms[i];
			myData.PrepareVFX[i].transform.localPosition = Vector3.zero;
		}
		myData.InnerState = 0;
		enemy.PlaySound(AudioName.DEADEYE_LASER_LONG0);
	}

	public override void Update(Enemy enemy, float deltaTime)
	{
		enemy.LookAtTargetHorizontal();
		MyData myData = enemy.StateData[this] as MyData;
		if (myData.InnerState == 0)
		{
			if (enemy.CurrentAnimTime >= DeadEyeParameter.ROOF_X_RAY_CHECK_TIME.x)
			{
				enemy.PlaySound(AudioName.DEADEYE_LASER_LONG1);
				for (int i = 0; i < 2; i++)
				{
					myData.LaserObject[i].SetActive(true);
					Pool.sGetInstance().Deallocate(myData.PrepareVFX[i]);
				}
				Vector3 vector = enemy.GetTargetPosition() - enemy.GetPosition();
				float num = DeadEyeParameter.ROOF_X_RAY_OFFSET * Mathf.InverseLerp(0f, 40f, vector.magnitude);
				vector.y = 0f;
				vector = vector.normalized;
				myData.Speeds[0] = Quaternion.Euler(0f, -45f, 0f) * vector;
				myData.Speeds[1] = Quaternion.Euler(0f, 45f, 0f) * vector;
				myData.Positions[0] = -myData.Speeds[0] * 10f + enemy.GetTargetPosition() - num * vector;
				myData.Positions[1] = -myData.Speeds[1] * 10f + enemy.GetTargetPosition() - num * vector;
				myData.Speeds[0] *= DeadEyeParameter.ROOF_X_RAY_SPEED;
				myData.Speeds[1] *= DeadEyeParameter.ROOF_X_RAY_SPEED;
				myData.LaserObject[0].transform.position = myData.AttachTransforms[0].position;
				myData.LaserObject[1].transform.position = myData.AttachTransforms[1].position;
				myData.LaserObject[0].transform.LookAt(myData.Positions[0]);
				myData.LaserObject[1].transform.LookAt(myData.Positions[1]);
				myData.InnerState++;
			}
		}
		else
		{
			if (myData.InnerState != 1)
			{
				return;
			}
			for (int j = 0; j < 2; j++)
			{
				myData.LaserObject[j].transform.position = myData.AttachTransforms[j].position;
				myData.Positions[j] += myData.Speeds[j] * Time.deltaTime;
				myData.LaserObject[j].transform.LookAt(myData.Positions[j]);
			}
			if (enemy.CurrentAnimTime >= DeadEyeParameter.ROOF_X_RAY_CHECK_TIME.y)
			{
				myData.InnerState++;
				for (int k = 0; k < 2; k++)
				{
					myData.LaserObject[k].SetActive(false);
				}
			}
		}
	}

	public override void End(Enemy enemy)
	{
		MyData myData = enemy.StateData[this] as MyData;
		for (int i = 0; i < 2; i++)
		{
			Pool.sGetInstance().Deallocate(myData.LaserObject[i]);
			Pool.sGetInstance().Deallocate(myData.PrepareVFX[i]);
		}
	}
}
