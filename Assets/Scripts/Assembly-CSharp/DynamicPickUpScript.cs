using System;
using System.Collections.Generic;
using UnityEngine;

public class DynamicPickUpScript : MonoBehaviour
{
	public enum PickUpType
	{
		FIXED = 0,
		RANDOM = 1,
		KILL_STREAK = 2
	}

	public PickUpType m_Type;

	public List<DynamicPickUpInfo> pickUpList;

	public float timeOfCreateFixedInterval;

	[NonSerialized]
	public byte IdInOnlineMode;

	private float timeToReSpawn;

	private bool hasItem;

	private GameObject mItem;

	private DynamicPickUpScript previous;

	private DynamicPickUpScript next;

	private RandomPickUpManagerScript mManager;

	private byte pickUpInfoId;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void UpdateByManager()
	{
		if (!hasItem && Time.time > timeToReSpawn)
		{
			CreatePickUp();
		}
	}

	public int CreatePickUp()
	{
		if (pickUpList.Count == 0)
		{
			base.enabled = false;
			return -1;
		}
		if (m_Type == PickUpType.FIXED || m_Type == PickUpType.RANDOM)
		{
			hasItem = true;
		}
		int num = 0;
		if (GameApp.GetInstance().GetGameMode().IsSingle())
		{
			previous = null;
			next = null;
			num = UnityEngine.Random.Range(0, pickUpList.Count);
			pickUpInfoId = (byte)num;
			GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
			if (gameWorld != null)
			{
				mItem = gameWorld.CreatePickUp(pickUpList[pickUpInfoId].m_LootType, base.gameObject.transform.position, pickUpList[pickUpInfoId].Param1, pickUpList[pickUpInfoId].Param2, 0, this);
			}
			mManager.SetInQueue(this);
		}
		else
		{
			num = UnityEngine.Random.Range(0, pickUpList.Count);
			ItemSpawnInRandomRequest request = new ItemSpawnInRandomRequest(IdInOnlineMode, (byte)num);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
		return num;
	}

	public void CreatePickUp(byte pickupID, short id)
	{
		previous = null;
		next = null;
		hasItem = true;
		pickUpInfoId = pickupID;
		GameWorld gameWorld = GameApp.GetInstance().GetGameWorld();
		if (gameWorld != null)
		{
			mItem = gameWorld.CreatePickUp(pickUpList[pickupID].m_LootType, base.gameObject.transform.position, pickUpList[pickupID].Param1, pickUpList[pickupID].Param2, id, this);
		}
		if (mItem != null)
		{
			Debug.Log("Spawn pickup in point " + IdInOnlineMode.ToString() + " from response. gameobj is " + mItem.name + ", current num = " + mManager.randomPickUpCount);
		}
	}

	public void ItemPickUp()
	{
		hasItem = false;
		if (m_Type == PickUpType.FIXED)
		{
			timeToReSpawn = Time.time + timeOfCreateFixedInterval;
		}
		else if (m_Type == PickUpType.RANDOM)
		{
			ReQueueBecauseDelete();
		}
	}

	public bool isHasItem()
	{
		return hasItem;
	}

	public void SetPrevious(DynamicPickUpScript theNew)
	{
		previous = theNew;
	}

	public DynamicPickUpScript GetPrevious()
	{
		return previous;
	}

	public void SetNext(DynamicPickUpScript theNew)
	{
		next = theNew;
	}

	public DynamicPickUpScript GetNext()
	{
		return next;
	}

	public void ReQueueBecauseDelete()
	{
		if (next != null)
		{
			next.SetPrevious(previous);
		}
		else
		{
			if (previous != null)
			{
				previous.SetNext(null);
			}
			mManager.SetLast(previous);
		}
		if (previous != null)
		{
			previous.SetNext(next);
		}
		else
		{
			if (next != null)
			{
				next.SetPrevious(null);
			}
			mManager.SetFirst(next);
		}
		mManager.randomPickUpCount--;
	}

	public void setManager(RandomPickUpManagerScript manager)
	{
		mManager = manager;
	}

	public void DestoryItem()
	{
		if (mItem != null)
		{
			Debug.Log("Destroy " + mItem.name + " in point " + IdInOnlineMode);
			mItem.SetActive(false);
		}
		ReQueueBecauseDelete();
		hasItem = false;
	}

	public void SetTimeInterval(float time)
	{
		if (timeOfCreateFixedInterval == 0f)
		{
			timeOfCreateFixedInterval = time;
		}
	}

	public void ResetWithNoSpawn()
	{
		timeToReSpawn = Time.time + timeOfCreateFixedInterval;
	}

	public byte GetPickUpInfoId()
	{
		return pickUpInfoId;
	}

	public ItemScript GetItemScript()
	{
		if (mItem == null)
		{
			return null;
		}
		if (mItem.GetComponent<ItemScript>() != null)
		{
			return mItem.GetComponent<ItemScript>();
		}
		return null;
	}
}
