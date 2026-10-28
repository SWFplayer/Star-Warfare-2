using System.Collections.Generic;
using UnityEngine;

public class RandomPickUpManagerScript : MonoBehaviour
{
	private enum EPickUpCapColumn
	{
		STAGE = 0,
		LEVEL = 1,
		DIFFICULTY = 2,
		CAP = 3,
		CD = 4
	}

	private enum EPickUpDataColumn
	{
		STAGE = 0,
		LEVEL = 1,
		PICKUP_ID = 2,
		DIFFICULTY = 3,
		TYPE = 4,
		CD = 5,
		SIZE = 6,
		ITEM1 = 7,
		ITEM2 = 8,
		ITEM3 = 9,
		ITEM4 = 10,
		ITEM5 = 11,
		ITEM6 = 12,
		ITEM7 = 13,
		ITEM8 = 14,
		ITEM9 = 15,
		ITEM10 = 16
	}

	public int MAX = 5;

	public float timeOfCreateRandomPickUp = 5f;

	public float timeOfCreateFixedPickUp = 10f;

	public float InDistanceNoItem = 5f;

	public float DistanceToCreateItem = 50f;

	private float lastTimeOfCreate;

	private List<DynamicPickUpScript> fixedPointList;

	private List<DynamicPickUpScript> randomPointList;

	private List<DynamicPickUpScript> killStreakPointList;

	private DynamicPickUpScript theLastOne;

	private DynamicPickUpScript theFirstOne;

	private int _randomCount;

	public static RandomPickUpManagerScript instance;

	public int randomPickUpCount
	{
		get
		{
			return _randomCount;
		}
		set
		{
			_randomCount = value;
		}
	}

	private void Awake()
	{
		instance = this;
	}

	private void OnDestroy()
	{
		instance = null;
	}

	private void Start()
	{
		for (int i = 0; i < base.transform.childCount; i++)
		{
			DynamicPickUpScript component = base.transform.GetChild(i).gameObject.GetComponent<DynamicPickUpScript>();
			if (component != null)
			{
				component.IdInOnlineMode = (byte)i;
			}
		}
		LoadConfig();
		randomPickUpCount = 0;
		PreInit();
		lastTimeOfCreate = 1f + Time.time;
	}

	private void Update()
	{
		if (!Lobby.GetInstance().IsMasterPlayer && !GameApp.GetInstance().GetGameMode().IsSingle())
		{
			return;
		}
		foreach (DynamicPickUpScript fixedPoint in fixedPointList)
		{
			fixedPoint.UpdateByManager();
		}
		if (Time.time > lastTimeOfCreate + timeOfCreateRandomPickUp && GameApp.GetInstance().GetGameWorld().GetLocalPlayer() != null)
		{
			CreateARandomItem();
		}
	}

	private void SpawnFirstWave()
	{
		if (!Lobby.GetInstance().IsMasterPlayer && !GameApp.GetInstance().GetGameMode().IsSingle())
		{
			return;
		}
		foreach (DynamicPickUpScript fixedPoint in fixedPointList)
		{
			if (fixedPoint != null)
			{
				fixedPoint.CreatePickUp();
			}
		}
	}

	private void PreInit()
	{
		fixedPointList = new List<DynamicPickUpScript>();
		randomPointList = new List<DynamicPickUpScript>();
		killStreakPointList = new List<DynamicPickUpScript>();
		DynamicPickUpScript[] componentsInChildren = base.gameObject.transform.GetComponentsInChildren<DynamicPickUpScript>();
		foreach (DynamicPickUpScript dynamicPickUpScript in componentsInChildren)
		{
			if (dynamicPickUpScript.enabled && dynamicPickUpScript.gameObject.activeSelf)
			{
				if (dynamicPickUpScript.m_Type == DynamicPickUpScript.PickUpType.FIXED)
				{
					fixedPointList.Add(dynamicPickUpScript);
					dynamicPickUpScript.SetTimeInterval(timeOfCreateFixedPickUp);
					dynamicPickUpScript.ResetWithNoSpawn();
				}
				else if (dynamicPickUpScript.m_Type == DynamicPickUpScript.PickUpType.RANDOM)
				{
					randomPointList.Add(dynamicPickUpScript);
				}
				else if (dynamicPickUpScript.m_Type == DynamicPickUpScript.PickUpType.KILL_STREAK)
				{
					killStreakPointList.Add(dynamicPickUpScript);
				}
				dynamicPickUpScript.setManager(this);
			}
		}
	}

	public void CreateNRandomItems(int n)
	{
		List<int> list = new List<int>();
		while (n > 0)
		{
			int num = Random.Range(0, killStreakPointList.Count);
			while (list.Contains(num))
			{
				num = Random.Range(0, killStreakPointList.Count);
			}
			list.Add(num);
			CreateARandomItemByKillStreak(num);
			n--;
		}
	}

	private void CreateARandomItemByKillStreak(int index)
	{
		DynamicPickUpScript dynamicPickUpScript = null;
		Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (localPlayer != null)
		{
			dynamicPickUpScript = killStreakPointList[index];
			localPlayer.GetUserState().Achievement.AddPickupIDList(dynamicPickUpScript.CreatePickUp() + 1);
			lastTimeOfCreate = Time.time;
		}
	}

	public void CreateARandomItem()
	{
		float num = 9999f;
		DynamicPickUpScript dynamicPickUpScript = null;
		List<DynamicPickUpScript> list = new List<DynamicPickUpScript>();
		Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (localPlayer == null)
		{
			return;
		}
		foreach (DynamicPickUpScript randomPoint in randomPointList)
		{
			if (!randomPoint.isHasItem())
			{
				list.Add(randomPoint);
				float magnitude = (GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
					.GetTransform()
					.position - randomPoint.transform.position).magnitude;
				if (magnitude < num && magnitude > 5f)
				{
					num = magnitude;
					dynamicPickUpScript = randomPoint;
				}
			}
		}
		if (list.Count > 0)
		{
			if (num > 10f)
			{
				dynamicPickUpScript = list[Random.Range(0, list.Count)];
			}
			dynamicPickUpScript.CreatePickUp();
		}
		lastTimeOfCreate = Time.time;
	}

	public void SetInQueue(DynamicPickUpScript theNew)
	{
		if (theFirstOne == null)
		{
			theFirstOne = theNew;
			theLastOne = theNew;
			randomPickUpCount++;
			return;
		}
		if (theLastOne != null)
		{
			theLastOne.SetNext(theNew);
			theNew.SetPrevious(theLastOne);
			theLastOne = theNew;
			randomPickUpCount++;
			if (randomPickUpCount > MAX)
			{
				theFirstOne.DestoryItem();
			}
			return;
		}
		string text = string.Empty;
		DynamicPickUpScript next = theFirstOne;
		DynamicPickUpScript dynamicPickUpScript = theFirstOne;
		while (next != null)
		{
			dynamicPickUpScript = next;
			text += next.name;
			next = next.GetNext();
		}
		theLastOne = dynamicPickUpScript;
		theLastOne.SetNext(theNew);
		theNew.SetPrevious(theLastOne);
		theLastOne = theNew;
	}

	public void SetLast(DynamicPickUpScript theNew)
	{
		theLastOne = theNew;
	}

	public DynamicPickUpScript GetLast()
	{
		return theLastOne;
	}

	public void SetFirst(DynamicPickUpScript theNew)
	{
		theFirstOne = theNew;
	}

	public DynamicPickUpScript GetFirst()
	{
		return theFirstOne;
	}

	public void SpawnItemWithPointPickUpID(short id, byte pointID, byte pickupID, bool isTheFirstTime)
	{
		foreach (DynamicPickUpScript randomPoint in randomPointList)
		{
			if (randomPoint.IdInOnlineMode == pointID)
			{
				randomPoint.CreatePickUp(pickupID, id);
				if (!isTheFirstTime)
				{
					SetInQueue(randomPoint);
				}
			}
		}
		foreach (DynamicPickUpScript fixedPoint in fixedPointList)
		{
			if (fixedPoint.IdInOnlineMode == pointID)
			{
				fixedPoint.CreatePickUp(pickupID, id);
			}
		}
		foreach (DynamicPickUpScript killStreakPoint in killStreakPointList)
		{
			if (killStreakPoint.IdInOnlineMode == pointID)
			{
				if (killStreakPoint.isHasItem())
				{
					killStreakPoint.DestoryItem();
				}
				killStreakPoint.CreatePickUp(pickupID, id);
			}
		}
	}

	public void SendPickUpMessageToRemotePlayer(int channelID)
	{
		List<pickUpPointInfo> list = new List<pickUpPointInfo>();
		List<byte> list2 = new List<byte>();
		bool flag = false;
		foreach (DynamicPickUpScript fixedPoint in fixedPointList)
		{
			if (fixedPoint.isHasItem() && fixedPoint.GetItemScript() != null)
			{
				list.Add(new pickUpPointInfo(fixedPoint.IdInOnlineMode, fixedPoint.GetPickUpInfoId(), fixedPoint.GetItemScript().sequenceID));
			}
		}
		foreach (DynamicPickUpScript randomPoint in randomPointList)
		{
			if (randomPoint.isHasItem())
			{
				if (randomPoint.GetItemScript() != null)
				{
					list.Add(new pickUpPointInfo(randomPoint.IdInOnlineMode, randomPoint.GetPickUpInfoId(), randomPoint.GetItemScript().sequenceID));
				}
				flag = true;
			}
		}
		foreach (DynamicPickUpScript killStreakPoint in killStreakPointList)
		{
			if (killStreakPoint.isHasItem() && killStreakPoint.GetItemScript() != null)
			{
				list.Add(new pickUpPointInfo(killStreakPoint.IdInOnlineMode, killStreakPoint.GetPickUpInfoId(), killStreakPoint.GetItemScript().sequenceID));
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		if (flag)
		{
			DynamicPickUpScript next = theFirstOne;
			while (next != null)
			{
				list2.Add(next.IdInOnlineMode);
				next = next.GetNext();
			}
		}
		SendPickUpInfoRequest request = new SendPickUpInfoRequest(list, list2, channelID);
		GameApp.GetInstance().GetNetworkManager().SendRequest(request);
	}

	protected DynamicPickUpScript GetOnlinePoint(byte id)
	{
		foreach (DynamicPickUpScript randomPoint in randomPointList)
		{
			if (randomPoint.IdInOnlineMode == id)
			{
				return randomPoint;
			}
		}
		foreach (DynamicPickUpScript fixedPoint in fixedPointList)
		{
			if (fixedPoint.IdInOnlineMode == id)
			{
				return fixedPoint;
			}
		}
		foreach (DynamicPickUpScript killStreakPoint in killStreakPointList)
		{
			if (killStreakPoint.IdInOnlineMode == id)
			{
				return killStreakPoint;
			}
		}
		return null;
	}

	public void SpawnItemFromOnlineForTheFirstTime(List<pickUpPointInfo> pickUpInfoList, List<byte> randomPickUpQueue)
	{
		foreach (pickUpPointInfo pickUpInfo in pickUpInfoList)
		{
			if (pickUpInfo.itemId != 0)
			{
				SpawnItemWithPointPickUpID(pickUpInfo.itemId, pickUpInfo.idInOnline, pickUpInfo.pickUpInfoId, true);
			}
		}
		byte b = (byte)randomPickUpQueue.Count;
		if (b > 0)
		{
			for (byte b2 = 0; b2 < b; b2++)
			{
				SetInQueue(GetOnlinePoint(randomPickUpQueue[b2]));
			}
		}
	}

	public List<DynamicPickUpScript> GetAvailablePickupPoints()
	{
		List<DynamicPickUpScript> list = new List<DynamicPickUpScript>();
		foreach (DynamicPickUpScript randomPoint in randomPointList)
		{
			if (randomPoint.isHasItem() && randomPoint.GetItemScript() != null)
			{
				list.Add(randomPoint);
			}
		}
		foreach (DynamicPickUpScript fixedPoint in fixedPointList)
		{
			if (fixedPoint.isHasItem() && fixedPoint.GetItemScript() != null)
			{
				list.Add(fixedPoint);
			}
		}
		foreach (DynamicPickUpScript killStreakPoint in killStreakPointList)
		{
			if (killStreakPoint.isHasItem() && killStreakPoint.GetItemScript() != null)
			{
				list.Add(killStreakPoint);
			}
		}
		return list;
	}

	public void LoadConfig()
	{
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			return;
		}
		UserState userState = GameApp.GetInstance().GetUserState();
		byte b = (byte)userState.GetSelectedDifficultMode();
		byte selectedStageIndexGeneral = userState.GetSelectedStageIndexGeneral();
		byte selectedSubStageIndexGeneral = userState.GetSelectedSubStageIndexGeneral();
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PICK_UP, DataSheetName.PICK_UP_CAP);
		bool flag = false;
		for (int i = 0; i < gameDataTable.Count; i++)
		{
			if (selectedStageIndexGeneral == gameDataTable.GetInt(i, 0) && selectedSubStageIndexGeneral == gameDataTable.GetInt(i, 1) && b == gameDataTable.GetInt(i, 2))
			{
				MAX = gameDataTable.GetInt(i, 3);
				timeOfCreateRandomPickUp = gameDataTable.GetInt(i, 4);
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		GameDataTable gameDataTable2 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.PICK_UP, DataSheetName.PICK_UP_DATA);
		DynamicPickUpScript[] componentsInChildren = base.gameObject.transform.GetComponentsInChildren<DynamicPickUpScript>();
		foreach (DynamicPickUpScript dynamicPickUpScript in componentsInChildren)
		{
			dynamicPickUpScript.enabled = false;
		}
		for (int k = 0; k < gameDataTable2.Count; k++)
		{
			if (selectedStageIndexGeneral != gameDataTable2.GetInt(k, 0) || selectedSubStageIndexGeneral != gameDataTable2.GetInt(k, 1) || b != gameDataTable2.GetInt(k, 3))
			{
				continue;
			}
			int num = gameDataTable2.GetInt(k, 2);
			Transform transform = base.transform.Find("Pickup_Point_" + num);
			if (!(transform != null))
			{
				continue;
			}
			DynamicPickUpScript component = transform.gameObject.GetComponent<DynamicPickUpScript>();
			if (!(component != null))
			{
				continue;
			}
			component.pickUpList = new List<DynamicPickUpInfo>();
			component.timeOfCreateFixedInterval = gameDataTable2.GetInt(k, 5);
			component.m_Type = (DynamicPickUpScript.PickUpType)gameDataTable2.GetInt(k, 4);
			for (int l = 0; l < gameDataTable2.GetInt(k, 6) && l < 10; l++)
			{
				string text = gameDataTable2.GetString(k, 7 + l);
				DynamicPickUpInfo dynamicPickUpInfo = new DynamicPickUpInfo();
				if (text.StartsWith("P"))
				{
					dynamicPickUpInfo.m_LootType = LootType.PICKUP;
				}
				else if (text.StartsWith("V"))
				{
					dynamicPickUpInfo.m_LootType = LootType.VEHICLE;
				}
				float param = float.Parse(text.Substring(1));
				dynamicPickUpInfo.Param1 = param;
				component.pickUpList.Add(dynamicPickUpInfo);
			}
			component.enabled = true;
		}
	}
}
