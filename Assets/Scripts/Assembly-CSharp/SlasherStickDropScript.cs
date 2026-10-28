using UnityEngine;

public class SlasherStickDropScript : BaseLaserScript
{
	private const float OFFSET = -2f;

	public UnityEngine.AI.NavMeshObstacle Obstacle;

	public new float KnockSpeed;

	public Vector3 MoveSpeed;

	public float RotateSpeed;

	private bool mIsStop;

	private bool mHitPlayer;

	private void Start()
	{
		mIsStop = false;
		mHitPlayer = false;
	}

	private void Update()
	{
		if (!mIsStop)
		{
			base.transform.Translate(MoveSpeed * Time.deltaTime, Space.World);
			base.transform.Rotate(Vector3.right, RotateSpeed * Time.deltaTime);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (mIsStop)
		{
			return;
		}
		if (other.gameObject.layer == PhysicsLayer.WALL_AGAINST_BULLET)
		{
			mIsStop = true;
			base.gameObject.layer = PhysicsLayer.WALL_AGAINST_BULLET;
			base.GetComponent<Collider>().isTrigger = false;
			Obstacle.enabled = true;
			AudioManager.GetInstance().StopSound(AudioName.SLASHER_WEAPON_SPIN, base.gameObject);
			AudioManager.GetInstance().PlaySoundAt(AudioName.SLASHER_WEAPON_HIT, base.transform.position);
		}
		else if (other.gameObject.layer == PhysicsLayer.FLOOR)
		{
			MoveSpeed = new Vector3(MoveSpeed.x, 0f - MoveSpeed.y, MoveSpeed.z);
		}
		else
		{
			if (other.gameObject.layer != PhysicsLayer.PLAYER || mHitPlayer)
			{
				return;
			}
			mHitPlayer = true;
			Player localPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if (localPlayer != null)
			{
				localPlayer.OnHit(new DamageProperty(Attacker, Damage, PlayerDamageType));
				Vector3 vector = new Vector3(localPlayer.GetTransform().forward.x, 0f, localPlayer.GetTransform().forward.z);
				float f = Vector3.Angle(to: new Vector3(MoveSpeed.x, 0f, MoveSpeed.z), from: vector);
				float num = KnockSpeed;
				if (Mathf.Abs(f) > 90f)
				{
					num = 0f - num;
				}
				localPlayer.OnKnocked(num);
				if (GameApp.GetInstance().GetGameMode().IsMultiPlayer())
				{
					PlayerOnKnockedRequest request = new PlayerOnKnockedRequest(num, localPlayer.GetChannelID());
					GameApp.GetInstance().GetNetworkManager().SendRequest(request);
				}
			}
		}
	}
}
