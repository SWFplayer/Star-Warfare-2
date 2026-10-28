using System.Collections.Generic;
using UnityEngine;

public class NightmareLaserScript : MonoBehaviour
{
	public enum EFireLineType
	{
		WEAPON = 0,
		SUB = 1,
		POWER_EFFECT = 2
	}

	private float dis;

	public bool growing = true;

	public List<Transform> rollTrans;

	[HideInInspector]
	public ChargeDoublePistol weapon;

	public bool mIsLeftFire;

	public Transform gunfire;

	public float length = 80f;

	public Transform targetTrans;

	public GameUnit targetGameUnit;

	public Vector3 hitPoint;

	public NightmareLaserScript nightmareLaserScript;

	private float beginTime;

	private float rollRotate;

	public EFireLineType m_Type;

	public GameObject root;

	public bool needUpdate;

	protected bool needSelfDestory;

	public void Init()
	{
		m_Type = EFireLineType.WEAPON;
		growing = true;
		beginTime = Time.time;
		base.transform.localScale = new Vector3(base.transform.localScale.x, base.transform.localScale.y, 0.01f);
		needUpdate = true;
	}

	public void Update()
	{
		if (needSelfDestory)
		{
			Object.Destroy(base.gameObject);
		}
		if (!needUpdate)
		{
			return;
		}
		if (checkDisappear())
		{
			root.SetActive(false);
			if (needSelfDestory)
			{
				Object.Destroy(base.gameObject);
			}
			return;
		}
		base.transform.position = gunfire.position;
		Vector3 vector = gunfire.position;
		Vector3 vector2 = gunfire.position + gunfire.forward.normalized * length;
		if (m_Type != EFireLineType.WEAPON && targetTrans != null)
		{
			vector2 = ((targetGameUnit == null || !(targetGameUnit.GetTransform() == targetTrans)) ? (targetTrans.position + Vector3.up * 1f) : (targetTrans.position + Vector3.up * 1f));
		}
		if (m_Type == EFireLineType.SUB)
		{
			bool flag = false;
			if (nightmareLaserScript != null && nightmareLaserScript.hitPoint != Vector3.zero)
			{
				base.transform.position = nightmareLaserScript.hitPoint;
				vector = nightmareLaserScript.hitPoint;
				flag = true;
			}
			if (!flag)
			{
				base.transform.position = gunfire.position + Vector3.up * 1f;
				vector = gunfire.position + Vector3.up * 1f;
			}
		}
		if (m_Type == EFireLineType.WEAPON && weapon != null && weapon.player != null && weapon.player.IsLocal() && HUDBase.instance != null && HUDBase.instance.m_Aim.HasAimAssisted)
		{
			Camera main = Camera.main;
			vector2 = main.ViewportToWorldPoint(HUDBase.instance.m_Aim.AssistedAimPositionInViewport);
		}
		base.transform.rotation = Quaternion.LookRotation(vector2 - vector);
		dis = Vector3.Distance(vector, vector2);
		if (m_Type == EFireLineType.WEAPON)
		{
			Ray ray = new Ray(vector, vector2 - vector);
			int layerMask = (1 << PhysicsLayer.WALL_AGAINST_BULLET) | (1 << PhysicsLayer.FLOOR) | (1 << PhysicsLayer.TRANSPARENT_WALL) | (1 << PhysicsLayer.VOID_SHIELD) | (1 << PhysicsLayer.ENEMY);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, dis, layerMask) && !(targetTrans == hitInfo.collider.transform))
			{
				dis = Vector3.Distance(hitInfo.point, vector);
				if (m_Type == EFireLineType.WEAPON)
				{
					if (hitInfo.collider.gameObject.layer == PhysicsLayer.ENEMY)
					{
						hitPoint = hitInfo.point;
					}
					else
					{
						hitPoint = Vector3.zero;
					}
				}
			}
		}
		float num = dis;
		if (growing)
		{
			if (growing && base.transform.localScale.z > num * 0.9f)
			{
				growing = false;
			}
			if (growing)
			{
				float z = Mathf.Min(num, base.transform.localScale.z + 200f * Time.deltaTime);
				base.transform.localScale = new Vector3(base.transform.localScale.x, base.transform.localScale.y, z);
			}
		}
		else
		{
			base.transform.localScale = new Vector3(base.transform.localScale.x, base.transform.localScale.y, num);
		}
		roll();
	}

	protected void roll()
	{
		rollRotate = Time.deltaTime * 180f + rollRotate;
		if (rollRotate > 360f)
		{
			rollRotate -= 360f;
		}
		foreach (Transform rollTran in rollTrans)
		{
			rollTran.localRotation = Quaternion.Euler(-90f, rollRotate, 0f);
		}
	}

	public void resetTime()
	{
		beginTime = Time.time;
		needUpdate = true;
		root.SetActive(true);
	}

	protected bool checkDisappear()
	{
		if (gunfire == null || !gunfire.gameObject.activeSelf)
		{
			needSelfDestory = true;
			return true;
		}
		if (m_Type != EFireLineType.WEAPON && (targetTrans == null || (targetTrans != null && !targetTrans.gameObject.activeSelf)))
		{
			return true;
		}
		if (weapon != null && m_Type != EFireLineType.POWER_EFFECT && !weapon.isAttacking())
		{
			return true;
		}
		return Time.time - beginTime > 0.5f;
	}
}
