using UnityEngine;

public class DeadEyeShoulderLaserComponent : GameUnitStateComponent
{
	public class MyData : GameUnitStateData
	{
		public Transform[] AttachTransforms = new Transform[2];

		public GameObject[] LaserObject = new GameObject[2];

		public GameObject[] PrepareVFX = new GameObject[2];

		public int InnerState;

		public bool HasShownVFX;
	}

	public void Start(GameUnit gameUnit, GameUnitState state)
	{
		if (!gameUnit.StateData.ContainsKey(state))
		{
			gameUnit.StateData.Add(state, new MyData());
		}
		MyData myData = gameUnit.StateData[state] as MyData;
		myData.InnerState = 0;
		myData.HasShownVFX = false;
		if (myData.AttachTransforms[0] == null)
		{
			myData.AttachTransforms[0] = gameUnit.GetTransform().Find(BoneName.DEADEYE_SHOULDER_L);
			myData.AttachTransforms[1] = gameUnit.GetTransform().Find(BoneName.DEADEYE_SHOULDER_R);
		}
		for (int i = 0; i < 2; i++)
		{
			myData.LaserObject[i] = Pool.sGetInstance().Allocate("SW2_Effect/B2_Range03_Laser");
			if (myData.LaserObject[i].GetComponent<LaserScript>() == null)
			{
				myData.LaserObject[i].AddComponent<LaserScript>();
			}
			myData.LaserObject[i].GetComponent<LaserScript>().SetDamageInfo(DeadEyeParameter.GetInstance(), 5);
			myData.LaserObject[i].GetComponent<LaserScript>().Attacker = gameUnit;
			myData.LaserObject[i].SetActive(false);
			myData.PrepareVFX[i] = Pool.sGetInstance().Allocate("SW2_Effect/B2_Laser_Charge_big");
			myData.PrepareVFX[i].transform.parent = myData.AttachTransforms[i];
			myData.PrepareVFX[i].transform.localPosition = Vector3.zero;
			myData.PrepareVFX[i].SetActive(false);
		}
		UpdateTargetPositionOnGround(gameUnit, 200f, true);
	}

	public void Update(GameUnit gameUnit, GameUnitState state)
	{
		MyData myData = gameUnit.StateData[state] as MyData;
		if (!myData.HasShownVFX && gameUnit.CurrentAnimTime >= DeadEyeParameter.SHOULDER_RAY_VFX_TIME)
		{
			myData.HasShownVFX = true;
			GameObject original = Resources.Load("SW2_Effect/B2_Range03_Laserdust") as GameObject;
			Object.Instantiate(original, gameUnit.GetPosition(), gameUnit.GetTransform().rotation);
		}
		if (myData.InnerState == 0)
		{
			UpdateTargetPositionOnGround(gameUnit, 40f, false);
			if (gameUnit.CurrentAnimTime >= DeadEyeParameter.SHOULDER_RAY_PREPARE_VFX_SHOW_TIME)
			{
				gameUnit.PlaySound(AudioName.DEADEYE_RANGE03);
				myData.InnerState++;
				for (int i = 0; i < 2; i++)
				{
					myData.PrepareVFX[i].SetActive(true);
				}
			}
		}
		else if (myData.InnerState == 1)
		{
			UpdateTargetPositionOnGround(gameUnit, 40f, false);
			if (!(gameUnit.CurrentAnimTime >= DeadEyeParameter.SHOULDER_RAY_CHECK_TIME.x))
			{
				return;
			}
			for (int j = 0; j < 2; j++)
			{
				myData.LaserObject[j].SetActive(true);
				myData.LaserObject[j].transform.position = myData.AttachTransforms[j].position;
				Vector3 vector = gameUnit.GetPosition() + gameUnit.GetTransform().forward * Vector3.Magnitude(gameUnit.GetPosition() - gameUnit.GetTargetPosition());
				if (gameUnit.GameUnitType == EGameUnitType.ENEMY)
				{
					myData.LaserObject[j].transform.LookAt(vector + Vector3.up);
				}
				else
				{
					myData.LaserObject[j].transform.LookAt(gameUnit.GetTargetPosition());
				}
				Pool.sGetInstance().Deallocate(myData.PrepareVFX[j]);
			}
			myData.InnerState++;
		}
		else if (myData.InnerState == 2 && gameUnit.CurrentAnimTime >= DeadEyeParameter.SHOULDER_RAY_CHECK_TIME.y)
		{
			myData.InnerState++;
			for (int k = 0; k < 2; k++)
			{
				myData.LaserObject[k].SetActive(false);
			}
		}
	}

	public void End(GameUnit gameUnit, GameUnitState state)
	{
		MyData myData = gameUnit.StateData[state] as MyData;
		for (int i = 0; i < 2; i++)
		{
			Pool.sGetInstance().Deallocate(myData.LaserObject[i]);
			Pool.sGetInstance().Deallocate(myData.PrepareVFX[i]);
		}
	}
}
