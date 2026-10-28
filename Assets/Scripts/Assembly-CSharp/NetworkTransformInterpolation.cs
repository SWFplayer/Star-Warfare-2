using System;
using UnityEngine;

public class NetworkTransformInterpolation
{
	private float extrapolationForwardTime;

	protected NetworkTransform[] bufferedTransforms = new NetworkTransform[50];

	protected Transform mTransform;

	protected Transform mPositionTransform;

	protected Transform mRotationTransform;

	protected int transCount;

	protected Vector3 diff = Vector3.zero;

	protected RemotePlayer remotePlayer;

	protected int enterExpolation;

	protected bool lastExpolation;

	protected int enterInterpolation;

	protected bool lastInterpolation;

	protected float lastCannonAngle;

	public float cannonTurn;

	protected Vector3 lastPosition;

	public MoveDirection moveDirection;

	public NetworkTransformInterpolation(RemotePlayer remotePlayer)
	{
		this.remotePlayer = remotePlayer;
	}

	public void SetTransform(Transform transform)
	{
		mTransform = transform;
		mPositionTransform = transform;
		mRotationTransform = transform;
	}

	public void SetPositionTransform(Transform transfrom)
	{
		mPositionTransform = transfrom;
	}

	public void SetRotationTransform(Transform transform)
	{
		mRotationTransform = transform;
	}

	public void ReceiveTransform(NetworkTransform nTrans)
	{
		for (int num = bufferedTransforms.Length - 1; num >= 1; num--)
		{
			bufferedTransforms[num] = bufferedTransforms[num - 1];
		}
		bufferedTransforms[0] = nTrans;
		transCount = Mathf.Min(transCount + 1, bufferedTransforms.Length);
		for (int i = 0; i < transCount - 1; i++)
		{
			if (bufferedTransforms[i].TimeStamp < bufferedTransforms[i + 1].TimeStamp)
			{
			}
		}
	}

	public void Clear()
	{
		transCount = 0;
		bufferedTransforms = new NetworkTransform[50];
	}

	public void Loop()
	{
		float num = 20f;
		float num2 = 12f;
		if (remotePlayer != null && remotePlayer.IsBoss)
		{
			if (remotePlayer.GetPlayerBossState() == Player.CERBERLISK_DASH_ATTACK_STATE)
			{
				num = 60f;
				num2 = 36f;
			}
			if (remotePlayer.GetPlayerBossState() != Player.BOSS_DEAD_STATE && remotePlayer.GetPlayerBossState() != Player.BOSS_IDLE_STATE && remotePlayer.GetPlayerBossState() != Player.BOSS_JUMP_STATE)
			{
				num = 40f;
				num2 = 24f;
			}
		}
		if ((remotePlayer.GetObject() != null && remotePlayer.GetObject().GetComponent<HookForceScript>() != null) || transCount == 0)
		{
			return;
		}
		int networkTime = TimeManager.GetInstance().NetworkTime;
		int num3 = networkTime - TimeManager.GetInstance().InterplolationBackTime;
		if (bufferedTransforms[0].TimeStamp > num3)
		{
			for (int i = 0; i < transCount; i++)
			{
				if (bufferedTransforms[i].TimeStamp > num3 && i != transCount - 1)
				{
					continue;
				}
				NetworkTransform networkTransform = bufferedTransforms[Mathf.Max(i - 1, 0)];
				NetworkTransform networkTransform2 = bufferedTransforms[i];
				double num4 = networkTransform.TimeStamp - networkTransform2.TimeStamp;
				float t = 0f;
				if (num4 > 0.0001)
				{
					t = (float)((double)(num3 - networkTransform2.TimeStamp) / num4);
				}
				Vector3 vector = Vector3.Lerp(networkTransform2.Pos, networkTransform.Pos, t);
				diff = mPositionTransform.position - vector;
				float num5 = Vector3.Distance(mPositionTransform.position, vector);
				if (num5 > 0.5f && num5 < 8f && transCount > 40)
				{
					float num6 = 8f;
					if (num5 > 4f)
					{
						num6 = num;
					}
					mPositionTransform.position -= diff.normalized * num6 * Time.deltaTime;
				}
				else
				{
					mPositionTransform.position = Vector3.MoveTowards(mPositionTransform.position, vector, num2 * Time.deltaTime);
				}
				float num7 = Mathf.LerpAngle(networkTransform2.Angle, networkTransform.Angle, t);
				if (remotePlayer.mVehicleState == EPlayerVehicleState.CANNON)
				{
					UpdateCannonRotation(num7);
				}
				else if (remotePlayer.mVehicleState == EPlayerVehicleState.CARRIER)
				{
					UpdateCarrierMoveDirection(mPositionTransform.position);
				}
				mRotationTransform.rotation = Quaternion.Euler(mRotationTransform.rotation.eulerAngles.x, num7, mRotationTransform.rotation.eulerAngles.z);
				return;
			}
			lastExpolation = false;
			return;
		}
		enterInterpolation = 0;
		lastInterpolation = false;
		float num8 = Convert.ToSingle(networkTime - bufferedTransforms[0].TimeStamp) / 1000f;
		if (num8 < extrapolationForwardTime && transCount > 1)
		{
			Vector3 vector2 = bufferedTransforms[0].Pos - bufferedTransforms[1].Pos;
			float num9 = Vector3.Distance(bufferedTransforms[0].Pos, bufferedTransforms[1].Pos);
			float num10 = Convert.ToSingle(bufferedTransforms[0].TimeStamp - bufferedTransforms[1].TimeStamp) / 1000f;
			if (Mathf.Approximately(num9, 0f) || Mathf.Approximately(num10, 0f))
			{
				mPositionTransform.position = bufferedTransforms[0].Pos;
				mRotationTransform.rotation = Quaternion.Euler(mRotationTransform.rotation.eulerAngles.x, bufferedTransforms[0].Angle, mRotationTransform.rotation.eulerAngles.z);
				return;
			}
			float num11 = num9 / num10;
			vector2 = vector2.normalized;
			Vector3 to = bufferedTransforms[0].Pos + vector2 * num8 * num11;
			mPositionTransform.position = Vector3.Lerp(mPositionTransform.position, to, Time.deltaTime * num11);
		}
		else
		{
			Vector3 pos = bufferedTransforms[0].Pos;
			diff = mPositionTransform.position - pos;
			float num12 = Vector3.Distance(mPositionTransform.position, pos);
			if (num12 > 0.5f && num12 < 8f && transCount > 40)
			{
				float num13 = 8f;
				if (num12 > 4f)
				{
					num13 = num;
				}
				mPositionTransform.position -= diff.normalized * num13 * Time.deltaTime;
			}
			else
			{
				mPositionTransform.position = Vector3.MoveTowards(mPositionTransform.position, pos, num2 * Time.deltaTime);
			}
		}
		mRotationTransform.rotation = Quaternion.Euler(mRotationTransform.rotation.eulerAngles.x, bufferedTransforms[0].Angle, mRotationTransform.rotation.eulerAngles.z);
	}

	public void UpdateCannonRotation(float newAngel)
	{
		float num = newAngel - lastCannonAngle;
		if (num <= 1f && num >= -1f)
		{
			cannonTurn = 0f;
		}
		else if (num > 0f && num < 180f)
		{
			cannonTurn = 1f;
		}
		else
		{
			cannonTurn = -1f;
		}
		lastCannonAngle = newAngel;
	}

	public void UpdateCarrierMoveDirection(Vector3 newPositon)
	{
		Vector3 vector = newPositon - lastPosition;
		Quaternion quaternion = Quaternion.Euler(0f, 0f - remotePlayer.mVehicle.GetTransform().rotation.eulerAngles.y, 0f);
		vector = quaternion * vector;
		if (Mathf.Abs(vector.x) > Mathf.Abs(vector.z))
		{
			if (vector.x > 0f)
			{
				moveDirection = MoveDirection.Right;
			}
			else if (vector.x < 0f)
			{
				moveDirection = MoveDirection.Left;
			}
		}
		else if (Mathf.Abs(vector.x) < Mathf.Abs(vector.z))
		{
			if (vector.z > 0f)
			{
				moveDirection = MoveDirection.Forward;
			}
			else if (vector.z < 0f)
			{
				moveDirection = MoveDirection.Backward;
			}
		}
		lastPosition = newPositon;
	}
}
