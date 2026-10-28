using UnityEngine;

public class SlasherWaveScript : MonoBehaviour
{
	public Transform ParentTransform;

	public SlasherWaveRootScript RootScript;

	private float mStartTime;

	private byte mDamageType;

	private int mDamage;

	private float mKnockSpeed;

	private PlayerDamageType mPlayerDamageType;

	private void Start()
	{
		mStartTime = Time.time;
		mDamage = RootScript.Damage;
		mKnockSpeed = RootScript.KnockSpeed;
		mDamageType = RootScript.DamageType;
		mPlayerDamageType = RootScript.PlayerDamageType;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!RootScript.mHitCheck.OnPassiveHit(RootScript.Attacker, other.gameObject, mDamageType, mDamage, mPlayerDamageType, mKnockSpeed))
		{
			return;
		}
		Player player = null;
		if (other.gameObject.layer == PhysicsLayer.PLAYER)
		{
			player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			if (player.GetUserState().GetSelectedStageIndex() == 0 && player.GetUserState().GetSelectedSubStageIndex() == Global.BOSS_FINAL_SUB_STAGE_ID)
			{
				player.mGetSlasherWaveHited = true;
			}
		}
		else if (other.gameObject.layer == PhysicsLayer.REMOTE_PLAYER)
		{
			int userID = int.Parse(other.gameObject.name);
			player = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(userID);
		}
	}
}
