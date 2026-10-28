using UnityEngine;

public class HealBagAnimationScript : MonoBehaviour
{
	public enum State
	{
		None = 0,
		Open = 1,
		OpenIdle = 2,
		Close = 3,
		CloseIdle = 4
	}

	public float RotateSpeed = 1f;

	public Player player;

	private float mValue;

	private State mState;

	private void Start()
	{
		base.GetComponent<Animation>()["fang"].wrapMode = WrapMode.ClampForever;
		base.GetComponent<Animation>()["fang_idle"].wrapMode = WrapMode.Loop;
		base.GetComponent<Animation>()["fang_idle"].speed = RotateSpeed;
		base.GetComponent<Animation>()["shou"].wrapMode = WrapMode.ClampForever;
		base.GetComponent<Animation>()["shou_idle"].wrapMode = WrapMode.Loop;
		base.GetComponent<Animation>().Play("shou_idle");
	}

	public void Init()
	{
		mValue = player.GetSkills().GetHealValue();
		Debug.Log("..........." + mValue);
	}

	private void Update()
	{
		if (player == null)
		{
			return;
		}
		if (!player.InPlayingState())
		{
			base.GetComponent<Animation>().CrossFade("shou_idle", 0.5f);
			return;
		}
		float num = (float)player.Hp / (float)player.MaxHp;
		if (mState == State.None || mState == State.CloseIdle)
		{
			if (num < mValue)
			{
				base.GetComponent<Animation>().CrossFade("fang", 0.5f);
				mState = State.Open;
			}
		}
		else if (mState == State.Open && base.GetComponent<Animation>()["fang"].time >= 1f)
		{
			if (num < mValue)
			{
				base.GetComponent<Animation>().CrossFade("fang_idle", 0.5f);
				mState = State.OpenIdle;
			}
			else
			{
				base.GetComponent<Animation>().CrossFade("shou", 0.5f);
				mState = State.Close;
			}
		}
		else if (mState == State.Close && base.GetComponent<Animation>()["shou"].time >= 1f)
		{
			if (num < mValue)
			{
				base.GetComponent<Animation>().CrossFade("fang", 0.5f);
				mState = State.Open;
			}
			else
			{
				base.GetComponent<Animation>().CrossFade("shou_idle", 0.5f);
				mState = State.CloseIdle;
			}
		}
		else if (mState == State.OpenIdle && num > mValue)
		{
			base.GetComponent<Animation>().CrossFade("shou", 0.5f);
			mState = State.Close;
		}
	}
}
