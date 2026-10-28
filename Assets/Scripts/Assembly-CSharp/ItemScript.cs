using UnityEngine;

public class ItemScript : MonoBehaviour
{
	public LootType itemType;

	private bool moveUp;

	public Vector3 rotationSpeed = new Vector3(0f, 45f, 0f);

	public bool enableUpandDown = true;

	protected float deltaTime;

	public float moveSpeed = 0.2f;

	public float HighPos = 1.2f;

	public float LowPos = 1f;

	public float Param1;

	public float Param2;

	public short sequenceID;

	public GameObject support;

	protected Timer lastPickUpRequestTimer = new Timer();

	protected DynamicPickUpScript pickUpSpawnerPoint;

	private void Start()
	{
		lastPickUpRequestTimer.SetTimer(3f, true);
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (deltaTime < 0.03f)
		{
			return;
		}
		base.transform.Rotate(rotationSpeed * deltaTime);
		if (enableUpandDown)
		{
			if (!moveUp)
			{
				float num = Mathf.MoveTowards(base.transform.position.y, LowPos, moveSpeed * deltaTime);
				base.transform.position = new Vector3(base.transform.position.x, num, base.transform.position.z);
				if (num <= LowPos)
				{
					moveUp = true;
				}
			}
			else
			{
				float num2 = Mathf.MoveTowards(base.transform.position.y, HighPos, moveSpeed * deltaTime);
				base.transform.position = new Vector3(base.transform.position.x, num2, base.transform.position.z);
				if (num2 >= HighPos)
				{
					moveUp = false;
				}
			}
		}
		deltaTime = 0f;
	}

	private void OnTriggerEnter(Collider c)
	{
		if (!lastPickUpRequestTimer.Ready() || c.GetComponent<Collider>().gameObject.layer != PhysicsLayer.PLAYER)
		{
			return;
		}
		Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		if (!localPlayer.InPlayingState() || localPlayer.IsJustRespawn || ((localPlayer.mVehicle != null || !localPlayer.TakeVehiclePickupTimer.Ready()) && itemType == LootType.VEHICLE))
		{
			return;
		}
		localPlayer.TakeVehiclePickupTimer.Do();
		if (GameApp.GetInstance().GetGameMode().IsSingle())
		{
			localPlayer.OnPickUp(itemType, Param1, Param2);
			if (itemType == LootType.VEHICLE)
			{
				GameApp.GetInstance().GetGameWorld().RemoveRadarPointByTransform(base.gameObject.transform);
			}
			Destroy();
			if (GetSpawner() != null)
			{
				GetSpawner().ItemPickUp();
			}
		}
		else
		{
			PickUpItemRequest request = new PickUpItemRequest(sequenceID);
			GameApp.GetInstance().GetNetworkManager().SendRequest(request);
		}
		lastPickUpRequestTimer.Do();
	}

	public void SetSpawner(DynamicPickUpScript spawner)
	{
		pickUpSpawnerPoint = spawner;
	}

	public DynamicPickUpScript GetSpawner()
	{
		return pickUpSpawnerPoint;
	}

	public void Destroy()
	{
		base.gameObject.SetActive(false);
		GameApp.GetInstance().GetGameWorld().RemoveItem(this);
	}

	private void OnEnable()
	{
		Start();
	}

	private void OnDisable()
	{
		if (support != null)
		{
			support.SetActive(false);
		}
	}
}
