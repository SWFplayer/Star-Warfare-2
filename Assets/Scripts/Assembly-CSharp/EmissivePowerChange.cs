using UnityEngine;

public class EmissivePowerChange : MonoBehaviour
{
	private Material mMaterail;

	private float powerInMaterial;

	public bool isChanging;

	public float maxPower = 1f;

	public float speed = 1f;

	private void Awake()
	{
		base.gameObject.layer = PhysicsLayer.TRIGGER;
	}

	private void Start()
	{
		mMaterail = base.GetComponent<Renderer>().material;
		if (mMaterail == null)
		{
			base.enabled = false;
		}
		else if (mMaterail.HasProperty("_EmissivePower"))
		{
			powerInMaterial = mMaterail.GetFloat("_EmissivePower");
		}
		else
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (isChanging)
		{
			powerInMaterial += speed * Time.deltaTime;
			if (powerInMaterial > maxPower)
			{
				powerInMaterial = maxPower;
				isChanging = false;
				base.enabled = false;
			}
			mMaterail.SetFloat("_EmissivePower", powerInMaterial);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == PhysicsLayer.PLAYER)
		{
			isChanging = true;
		}
	}
}
