using System.Collections.Generic;
using UnityEngine;

public class UIMiniMap : MonoBehaviour
{
	public const string TYPE_PLAYER_KEY = "Player";

	public const string TYPE_QUEST_TARGET = "QuestTarget";

	public const string TYPE_QUEST_PROTECT = "QuestProtect";

	public const string TYPE_QUEST_ENEMY_AREA = "QuestEnemyArea";

	public const string TYPE_QUEST_BOMB = "QuestBomb";

	public const string TYPE_QUEST_BOSS = "QuestBoss";

	public const string TYPE_PICKUP_VEHICLE = "PickupVehicle";

	public const string TYPE_ENEMY_KEY = "Enemy";

	public const string TYPE_ENEMY_PLAYER_KEY = "EnemyPlayer";

	public const string TYPE_PARTNER_KEY = "Partner";

	public const string TYPE_DOMINATION_POINT_KEY = "Point";

	public const string TYPE_PICKUP_POINT_KEY = "PickupPoint";

	public const string TYPE_FLAG = "Flag";

	public const string TYPE_FLAG_EFFECT = "FlagEffect";

	public const string TYPE_POINT_EFFECT = "PointEffect";

	public const string TYPE_POINT_EFFECT_DELAY = "PointEffectDelay";

	public const string TYPE_RESPAWN_POINT = "RespawnPoint";

	[SerializeField]
	private Vector2 m_MapScale = new Vector2(512f, 512f);

	[SerializeField]
	private GameObject m_MapContainer;

	[SerializeField]
	private GameObject m_SpriteContainer;

	private UISpriteBundle[] mSpriteBundleList;

	private Dictionary<string, UISpriteBundle> mSpriteDic = new Dictionary<string, UISpriteBundle>();

	private Vector2 mMapTopLeftPoint;

	private float changePositionTolerance = 10f;

	private void Awake()
	{
		mMapTopLeftPoint = new Vector2((0f - m_MapScale.x) / 2f, m_MapScale.y / 2f);
		m_MapContainer.transform.localPosition = Vector3.zero;
		m_SpriteContainer.transform.localPosition = Vector3.zero;
		m_MapContainer.transform.localScale = Vector3.one;
		mSpriteBundleList = m_SpriteContainer.GetComponentsInChildren<UISpriteBundle>(true);
		UISpriteBundle[] array = mSpriteBundleList;
		foreach (UISpriteBundle uISpriteBundle in array)
		{
			uISpriteBundle.sprite.gameObject.transform.localPosition = Vector3.zero;
			uISpriteBundle.sprite.gameObject.SetActive(false);
		}
	}

	public void LoadMapResource(GameObject map)
	{
		map.transform.parent = m_MapContainer.transform;
		map.transform.localPosition = Vector3.zero;
		map.transform.localEulerAngles = Vector3.zero;
		map.transform.localScale = Vector3.one;
	}

	public bool ActivateSpriteIfNot(string spriteKey, string typeKey)
	{
		if (!mSpriteDic.ContainsKey(spriteKey))
		{
			UISpriteBundle[] array = mSpriteBundleList;
			foreach (UISpriteBundle uISpriteBundle in array)
			{
				if (uISpriteBundle.key.Equals(typeKey) && !uISpriteBundle.sprite.gameObject.activeSelf)
				{
					uISpriteBundle.sprite.gameObject.SetActive(true);
					mSpriteDic.Add(spriteKey, uISpriteBundle);
					return true;
				}
			}
		}
		return false;
	}

	public bool ActivateSpriteIfNot(string spriteKey, string typeKey, float scale)
	{
		if (!mSpriteDic.ContainsKey(spriteKey))
		{
			UISpriteBundle[] array = mSpriteBundleList;
			foreach (UISpriteBundle uISpriteBundle in array)
			{
				if (uISpriteBundle.key.Equals(typeKey) && !uISpriteBundle.sprite.gameObject.activeSelf)
				{
					uISpriteBundle.sprite.gameObject.SetActive(true);
					uISpriteBundle.sprite.gameObject.transform.localScale = Vector3.one;
					uISpriteBundle.sprite.height = (int)scale;
					uISpriteBundle.sprite.width = (int)scale;
					mSpriteDic.Add(spriteKey, uISpriteBundle);
					return true;
				}
			}
		}
		return false;
	}

	public void RemoveSpriteWithout(params string[] spriteKeys)
	{
		List<string> list = new List<string>();
		foreach (string item in spriteKeys)
		{
			list.Add(item);
		}
		RemoveSpriteWithout(list);
		list.Clear();
	}

	public void RemoveSpriteWithout(List<string> keyList)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, UISpriteBundle> item in mSpriteDic)
		{
			if (!keyList.Contains(item.Key))
			{
				list.Add(item.Key);
			}
		}
		foreach (string item2 in list)
		{
			mSpriteDic[item2].sprite.gameObject.SetActive(false);
			mSpriteDic.Remove(item2);
			if (UIRadar.mInstance.mTargetEffectKeys.Contains(item2))
			{
				UIRadar.mInstance.mTargetEffectKeys.Remove(item2);
			}
		}
		list.Clear();
	}

	public bool RemoveSprite(string spriteKey)
	{
		if (mSpriteDic.ContainsKey(spriteKey))
		{
			mSpriteDic[spriteKey].sprite.gameObject.SetActive(false);
			mSpriteDic.Remove(spriteKey);
		}
		return false;
	}

	public void RemoveAllSprite()
	{
		mSpriteDic.Clear();
		UISpriteBundle[] array = mSpriteBundleList;
		foreach (UISpriteBundle uISpriteBundle in array)
		{
			uISpriteBundle.sprite.gameObject.SetActive(false);
		}
	}

	public void UpdateSpritePos(string key, Vector2 pos)
	{
		if (!mSpriteDic.ContainsKey(key) || !mSpriteDic[key].gameObject.activeSelf)
		{
			return;
		}
		Vector3 localPosition = mSpriteDic[key].transform.localPosition;
		Vector3 vector = new Vector3(pos.x, pos.y, localPosition.z);
		float magnitude = (vector - localPosition).magnitude;
		if (magnitude > changePositionTolerance)
		{
			TweenAlpha component = mSpriteDic[key].sprite.GetComponent<TweenAlpha>();
			if (component != null)
			{
				component.Reset();
				component.PlayForward();
			}
		}
		mSpriteDic[key].sprite.transform.localPosition = vector;
	}

	public void UpdateSpriteScale(string key, Vector2 pos)
	{
		if (mSpriteDic.ContainsKey(key) && mSpriteDic[key].gameObject.activeSelf)
		{
			mSpriteDic[key].sprite.transform.localScale = new Vector3(pos.x, pos.y, mSpriteDic[key].transform.localScale.z);
		}
	}

	public void UpdateSpriteAngle(string key, float angle)
	{
		if (mSpriteDic.ContainsKey(key) && mSpriteDic[key].gameObject.activeSelf)
		{
			mSpriteDic[key].sprite.transform.localEulerAngles = new Vector3(mSpriteDic[key].transform.localEulerAngles.x, mSpriteDic[key].transform.localEulerAngles.y, 0f - angle);
		}
	}

	public Vector2 GetMapSize()
	{
		return m_MapScale;
	}

	public Vector2 GetMapTopLeftPoint()
	{
		return mMapTopLeftPoint;
	}

	public Transform GetSpriteWorldPosition(string key)
	{
		if (mSpriteDic.ContainsKey(key) && mSpriteDic[key].gameObject.activeSelf)
		{
			return mSpriteDic[key].transform;
		}
		return null;
	}

	public UISprite GetSprite(string key)
	{
		if (mSpriteDic.ContainsKey(key) && mSpriteDic[key].gameObject.activeSelf)
		{
			return mSpriteDic[key].sprite;
		}
		return null;
	}

	public bool HasKeyAndType(string key, string typeKey)
	{
		if (mSpriteDic.ContainsKey(key) && mSpriteDic[key].gameObject.activeSelf && mSpriteDic[key].key == typeKey)
		{
			return true;
		}
		return false;
	}
}
