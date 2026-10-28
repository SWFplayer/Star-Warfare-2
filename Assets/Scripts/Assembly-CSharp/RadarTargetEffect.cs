using UnityEngine;

public class RadarTargetEffect : MonoBehaviour
{
	public Camera TargetCamera;

	public UISprite Icon;

	protected string mKey = string.Empty;

	protected Vector3 mTargetPositionInScene;

	protected Transform mTargetTransformOnRadar;

	private bool mIsMoving;

	private float mStartTime;

	private float mStartMoveTime = 1f;

	public void Init(string key, Vector3 position)
	{
		mIsMoving = false;
		mKey = key;
		mTargetPositionInScene = position;
		mStartTime = Time.time;
		Update();
	}

	private void Update()
	{
		if (mIsMoving)
		{
			return;
		}
		Vector3 position = Camera.main.WorldToViewportPoint(mTargetPositionInScene);
		position.x = Mathf.Clamp(position.x, 0f, 1f);
		position.y = Mathf.Clamp(position.y, 0f, 1f);
		position.z = 2f;
		if (!IsTimeToMove())
		{
			mStartTime = Time.time;
			if (RelativeAngle() <= 70f)
			{
				if (position.x >= 0.5f)
				{
					position.x = 1f;
				}
				else
				{
					position.x = 0f;
				}
				base.transform.position = TargetCamera.ViewportToWorldPoint(position);
			}
			else if (RelativeAngle() > 70f && RelativeAngle() <= 91.75f)
			{
				if (position.x >= 0.5f)
				{
					position.x = 1f;
				}
				else
				{
					position.x = 0f;
				}
				position.y = 0.5f;
				base.transform.position = TargetCamera.ViewportToWorldPoint(position);
			}
			else
			{
				if (position.x >= 0.5f)
				{
					position.x = 0f;
				}
				else
				{
					position.x = 1f;
				}
				position.y = 0.5f;
				base.transform.position = TargetCamera.ViewportToWorldPoint(position);
			}
		}
		else
		{
			base.transform.position = TargetCamera.ViewportToWorldPoint(position);
		}
		if (Time.time - mStartTime >= mStartMoveTime)
		{
			MoveToRadar();
		}
	}

	public void MoveToRadar()
	{
		mIsMoving = true;
		mTargetTransformOnRadar = UIRadar.mInstance.m_UIMiniMap.GetSpriteWorldPosition(mKey);
		TweenTransform tweenTransform = TweenTransform.Begin(base.gameObject, 1f, mTargetTransformOnRadar);
		tweenTransform.ignoreTimeScale = false;
		tweenTransform.onFinished.Add(new EventDelegate(this, "SelfDestroy"));
	}

	private bool IsTimeToMove()
	{
		float num = RelativeAngle();
		return num < 40f;
	}

	private float RelativeAngle()
	{
		Transform transform = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.GetTransform();
		Vector3 forward = transform.forward;
		Vector3 to = mTargetPositionInScene - transform.position;
		return Vector3.Angle(forward, to);
	}

	public void SelfDestroy()
	{
		Object.Destroy(base.gameObject);
	}
}
