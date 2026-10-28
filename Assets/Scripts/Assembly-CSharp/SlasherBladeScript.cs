using UnityEngine;

public class SlasherBladeScript : BaseLaserScript
{
	public float SpeedValue;

	public Vector3 Speed;

	public float MinScale;

	public float MaxScale;

	public float ScaleSpeed;

	private bool mHitGround;

	private float mCurrentScale;

	private HitCheck mHitCheck = new HitCheck();

	private float mStartTime;

	private void Start()
	{
		mCurrentScale = MinScale;
		base.transform.localScale = new Vector3(mCurrentScale, mCurrentScale, mCurrentScale);
		mHitCheck.Clear();
		mStartTime = Time.time;
	}

	private void Update()
	{
		base.transform.Translate(Speed * Time.deltaTime, Space.World);
		mCurrentScale = Mathf.Clamp(mCurrentScale + ScaleSpeed * Time.deltaTime, MinScale, MaxScale);
		base.transform.localScale = new Vector3(mCurrentScale, mCurrentScale, mCurrentScale);
		if (Time.time - mStartTime > 10f)
		{
			Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == PhysicsLayer.WALL_AGAINST_BULLET)
		{
			Object.DestroyObject(base.gameObject);
		}
		else if (!mHitGround && other.gameObject.layer == PhysicsLayer.FLOOR)
		{
			mHitGround = true;
			Speed.y = 0f;
			Speed = Speed.normalized * SpeedValue;
		}
		mHitCheck.OnHit(Attacker, other.gameObject, DamageType, Damage, PlayerDamageType, 0f);
	}
}
