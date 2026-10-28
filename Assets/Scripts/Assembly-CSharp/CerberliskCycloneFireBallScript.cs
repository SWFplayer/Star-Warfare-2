using System;
using UnityEngine;

public class CerberliskCycloneFireBallScript : BaseLaserScript
{
	[NonSerialized]
	public Vector3 Speed;

	[NonSerialized]
	public float ExplodeRadius;

	[NonSerialized]
	public string Vfx;

	[NonSerialized]
	public string VfxSound;

	[NonSerialized]
	public string CycloneName;

	[NonSerialized]
	public GameObject Target;

	private HitCheck mHitCheck = new HitCheck();

	private float mStartTime;

	private float mStateStartTime;

	private int mState;

	private GameObject mFireBallGameObject;

	public void Reset()
	{
		mHitCheck.Clear();
		mStartTime = Time.time;
		mState = 0;
		mFireBallGameObject = Pool.sGetInstance().Allocate("SW2_Effect/B4_bigfireball", base.transform.position);
		mFireBallGameObject.transform.parent = base.transform;
	}

	private void Update()
	{
		if (Time.time - mStartTime > 10f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			Pool.sGetInstance().Deallocate(mFireBallGameObject);
		}
		if (mState == 0)
		{
			base.transform.Translate(Speed * Time.deltaTime, Space.World);
		}
		else if (mState == 1 && Time.time - mStateStartTime > 1f)
		{
			mState++;
			mStateStartTime = Time.time;
			GameObject go = Pool.sGetInstance().Allocate("SW2_Effect/B4_cyclone_stay", base.transform.position);
			CerberliskCycloneScript component = GameWorld.GetComponent<CerberliskCycloneScript>(go);
			component.SetDamageInfo(CerberliskParameter.GetInstance(), 9);
			component.Duration = CerberliskParameter.CYCLONE_DURATION;
			component.Target = Target;
			component.Attacker = Attacker;
			component.Reset();
		}
		else if (mState == 2 && Time.time - mStateStartTime > 2f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer != PhysicsLayer.FLOOR && other.gameObject.layer != PhysicsLayer.WALL_AGAINST_BULLET)
		{
			return;
		}
		mState = 1;
		mStateStartTime = Time.time;
		Pool.sGetInstance().Deallocate(mFireBallGameObject);
		mHitCheck.CheckHit(Attacker, base.transform.position, ExplodeRadius * ExplodeRadius, DamageType, Damage, PlayerDamageType);
		if (Vfx != string.Empty)
		{
			Ray ray = new Ray(base.transform.position + Vector3.up, Vector3.down);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, 10f, 1 << PhysicsLayer.FLOOR))
			{
				Pool.sAllocate(Vfx, hitInfo.point, 3f);
			}
			else
			{
				Debug.Log("No floor.");
			}
		}
		if (VfxSound != null)
		{
			AudioManager.GetInstance().PlaySoundAt(VfxSound, base.transform.position);
		}
	}
}
