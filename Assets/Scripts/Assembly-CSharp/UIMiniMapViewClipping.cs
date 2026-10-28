using System;
using UnityEngine;

public class UIMiniMapViewClipping : MonoBehaviour
{
	[SerializeField]
	private UIMiniMap m_UIMiniMap;

	[SerializeField]
	private Vector2 m_MiniMapViewClipping = new Vector2(200f, 200f);

	private float mClippingHalfWidth;

	private float m_ClippingHalfHeight;

	private UIPanel mMapPanel;

	private Vector2 mMiniMapViewClippingCenter
	{
		get
		{
			return -m_UIMiniMap.transform.localPosition;
		}
		set
		{
			m_UIMiniMap.transform.localPosition = new Vector3(0f - value.x, 0f - value.y, m_UIMiniMap.transform.localPosition.z);
		}
	}

	private void Awake()
	{
		mMapPanel = GetComponent<UIPanel>();
		if (mMapPanel == null)
		{
			base.gameObject.AddComponent<UIPanel>();
		}
		mMapPanel.transform.localPosition = Vector3.zero;
		mMapPanel.clipping = UIDrawCall.Clipping.SoftClip;
		SetViewClipping(m_MiniMapViewClipping);
	}

	public void SetViewClipping(Vector2 clipping)
	{
		m_MiniMapViewClipping = clipping;
		mMapPanel.clipRange = new Vector4(0f, 0f, m_MiniMapViewClipping.x, m_MiniMapViewClipping.y);
		mClippingHalfWidth = m_MiniMapViewClipping.x / 2f;
		m_ClippingHalfHeight = m_MiniMapViewClipping.y / 2f;
	}

	public void MoveClippingTo(Vector2 pos)
	{
		mMiniMapViewClippingCenter = pos;
	}

	public void RotateClippingTo(float angle)
	{
		m_UIMiniMap.transform.parent.localEulerAngles = new Vector3(m_UIMiniMap.transform.parent.localEulerAngles.x, m_UIMiniMap.transform.parent.localEulerAngles.y, angle);
	}

	public Vector2 LimitSpritePosInClipping(Vector2 pos)
	{
		return LimitSpritePosInClipping(pos, Vector2.one);
	}

	public Vector2 LimitSpritePosInClipping(Vector2 pos, Vector2 ratio)
	{
		float z = m_UIMiniMap.transform.parent.localEulerAngles.z;
		float num = Vector2.Distance(mMiniMapViewClippingCenter, pos);
		float angleHorizontal = GetAngleHorizontal(mMiniMapViewClippingCenter, pos);
		float num2 = Mathf.Cos((angleHorizontal + z) * (float)Math.PI / 180f) * num;
		float num3 = Mathf.Sin((angleHorizontal + z) * (float)Math.PI / 180f) * num;
		float num4 = mClippingHalfWidth / ratio.x;
		float num5 = m_ClippingHalfHeight / ratio.y;
		if (num2 > num4)
		{
			num2 = num4;
		}
		if (num2 < 0f - num4)
		{
			num2 = 0f - num4;
		}
		if (num3 > num5)
		{
			num3 = num5;
		}
		if (num3 < 0f - num5)
		{
			num3 = 0f - num5;
		}
		num = Vector2.Distance(Vector2.zero, new Vector2(num2, num3));
		angleHorizontal = GetAngleHorizontal(Vector2.zero, new Vector2(num2, num3));
		num2 = Mathf.Cos((angleHorizontal - z) * (float)Math.PI / 180f) * num;
		num3 = Mathf.Sin((angleHorizontal - z) * (float)Math.PI / 180f) * num;
		return new Vector2(mMiniMapViewClippingCenter.x + num2, mMiniMapViewClippingCenter.y + num3);
	}

	public float GetAngleHorizontal(Vector2 center, Vector2 target)
	{
		float num = target.x - center.x;
		float num2 = target.y - center.y;
		if (num == 0f)
		{
			if (num2 < 0f)
			{
				return -90f;
			}
			return 90f;
		}
		float num3 = Mathf.Atan(num2 / num) * 180f / (float)Math.PI;
		if (num < 0f)
		{
			return num3 + 180f;
		}
		num3 += 360f;
		return num3 % 360f;
	}
}
