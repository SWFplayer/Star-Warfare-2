using System;
using UnityEngine;

public class MissionActionPickup : MissionAction
{
	private LootType mLootType;

	private int mTransformId;

	private float mRadius;

	private float mParameter1;

	private float mParameter2;

	private Vector3 mSpawnPosition;

	public override void LoadConfig()
	{
		GameDataTable dataTable = GetDataTable();
		for (int i = 0; i < dataTable.Count; i++)
		{
			int num = dataTable.GetInt(i, 0);
			if (num == base.ID)
			{
				string text = dataTable.GetString(i, 1);
				if (Enum.IsDefined(typeof(LootType), text))
				{
					mLootType = (LootType)(int)Enum.Parse(typeof(LootType), text, true);
				}
				else
				{
					Debug.LogError("[INVALID PICKUP TYPE]: lootTypeString = " + text);
				}
				mTransformId = dataTable.GetInt(i, 2);
				mRadius = dataTable.GetFloat(i, 3);
				mParameter1 = dataTable.GetFloat(i, 4);
				mParameter2 = dataTable.GetFloat(i, 5);
				break;
			}
		}
	}

	public override void LevelStart()
	{
		if (mTransformId == 0)
		{
			return;
		}
		string text = MissionNamePrefix.ACTION_PICKUP + mTransformId;
		bool flag = false;
		GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.MISSION_ACTION_PICKUP);
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (text == gameObject.name)
			{
				flag = true;
				mSpawnPosition = gameObject.transform.position;
				break;
			}
		}
		if (!flag)
		{
			Debug.LogError("[NO TRANSFORM FOUND MISSION_ACTION_PICKUP] ActionID = " + base.ID + ", mTransformId = " + mTransformId);
		}
	}

	public override void StartAction()
	{
		if (mTransformId == 0)
		{
			Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if (localPlayer != null && localPlayer.GetTransform() != null)
			{
				mSpawnPosition = localPlayer.GetTransform().position;
			}
		}
		Vector3 vector = mSpawnPosition;
		if (mRadius > 0f)
		{
			float f = UnityEngine.Random.Range(0f, 360f) * ((float)Math.PI / 180f);
			vector = mSpawnPosition + mRadius * new Vector3(Mathf.Cos(f), 0f, Mathf.Sin(f));
			Ray ray = new Ray(vector, Vector3.down);
			int layerMask = 1 << PhysicsLayer.FLOOR;
			RaycastHit hitInfo;
			if (!Physics.Raycast(ray, out hitInfo, 2f, layerMask))
			{
				vector = mSpawnPosition;
			}
		}
		if (GameApp.GetInstance().GetGameMode().IsSingle() || Lobby.GetInstance().IsMasterPlayer)
		{
			GameWorld.TryCreatePickUp(mLootType, vector, mParameter1, mParameter2);
		}
	}
}
