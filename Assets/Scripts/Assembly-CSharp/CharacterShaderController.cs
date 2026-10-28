using UnityEngine;

public class CharacterShaderController : MonoBehaviour
{
	public static AvatarColorData[] TempData;

	private AvatarColorData[] CurrentData;

	public BodyType m_BodyType;

	public int m_EquipID;

	public bool useCustomColor;

	public float LightSpeed = 1f;

	public int changeColorType;

	public float ColorSpeed = 0.1f;

	private Color customColor;

	private Material[] mats;

	private float value;

	private float colorValue;

	private float seXiang;

	private void Start()
	{
		Transform root = base.transform.root;
		int result = 0;
		int.TryParse(root.name, out result);
		bool flag = false;
		if (result != 0)
		{
			if (GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(result) == null)
			{
				CurrentData = GameApp.GetInstance().GetUserState().GetCurrentCustomize()
					.mAvatarColorArray;
			}
			else
			{
				CurrentData = GameApp.GetInstance().GetGameWorld().GetRemotePlayerByUserID(result)
					.GetUserState()
					.GetCurrentCustomize()
					.mAvatarColorArray;
				flag = true;
			}
		}
		else if (root.name == "PlayerInfoUI(Clone)")
		{
			CurrentData = TempData;
		}
		else if (root.name == "CompareRecordUI(Clone)")
		{
			if (base.transform.parent.parent.name == "AvatarFrameCloud")
			{
				CurrentData = TempData;
			}
			else
			{
				CurrentData = GameApp.GetInstance().GetUserState().GetCurrentCustomize()
					.mAvatarColorArray;
			}
		}
		else
		{
			CurrentData = GameApp.GetInstance().GetUserState().GetCurrentCustomize()
				.mAvatarColorArray;
		}
		mats = base.GetComponent<Renderer>().materials;
		if (mats == null || mats.Length == 0 || (int)m_BodyType >= CurrentData.Length)
		{
			base.enabled = false;
			return;
		}
		if ((InterMissionScript.GetInstance() != null && (InterMissionScript.GetInstance().CurrentPage == InterMissionPageType.Shop || (InterMissionScript.GetInstance().CurrentPage == InterMissionPageType.Custom && m_EquipID != CurrentData[(int)m_BodyType].mCurrentEquipID))) || (CustomizeUIScript.mInstance != null && CustomizeUIScript.mInstance.gameObject.activeSelf && CustomizeUIScript.mInstance.IsInLoadout() && m_EquipID != CurrentData[(int)m_BodyType].mCurrentEquipID) || (flag && GameApp.GetInstance().GetUserState().mGraphicQuality.mCapbility <= DeviceCaps.Capbility.Middle) || GameApp.GetInstance().GetUserState().mGraphicQuality.mCapbility == DeviceCaps.Capbility.Low)
		{
			useCustomColor = false;
		}
		else
		{
			useCustomColor = CurrentData[(int)m_BodyType].mMode != 0;
			customColor = CurrentData[(int)m_BodyType].mColor;
			seXiang = (float)(int)CurrentData[(int)m_BodyType].mSliderValue * 0.01f;
			changeColorType = CurrentData[(int)m_BodyType].mMode;
		}
		Material[] array = mats;
		foreach (Material material in array)
		{
			if (material.HasProperty("_useCustomColor"))
			{
				if (useCustomColor)
				{
					material.SetFloat("_useCustomColor", 1f);
					if (material.HasProperty("_paintColor"))
					{
						material.SetColor("_paintColor", customColor);
					}
				}
				else
				{
					material.SetFloat("_useCustomColor", 0f);
				}
			}
			else
			{
				Debug.Log("there is no this property : _useCustomColor");
			}
			if (material.HasProperty("_seXiang"))
			{
				SetSeXiang(material, seXiang);
			}
			if (material.HasProperty("_lightRate"))
			{
				material.SetFloat("_lightRate", value);
			}
			else
			{
				Debug.Log("there is no this property : _lightRate");
			}
		}
	}

	private void Update()
	{
		value = Mathf.PingPong(Time.time * LightSpeed, 1f);
		Material[] array = mats;
		foreach (Material material in array)
		{
			if (material.HasProperty("_lightRate"))
			{
				material.SetFloat("_lightRate", value);
			}
			if (changeColorType != 0 && changeColorType != 1)
			{
				colorValue = Mathf.PingPong(Time.time, 1f);
				Color color = GetColor(colorValue);
				if (material.HasProperty("_paintColor"))
				{
					material.SetColor("_paintColor", color);
				}
				if (material.HasProperty("_seXiang"))
				{
					SetSeXiang(material, GetHSV(color));
				}
			}
		}
	}

	public Color GetColor(float colorValue)
	{
		switch (changeColorType)
		{
		case 2:
			return Color.Lerp(Global.COLORSET_1[0], Global.COLORSET_1[1], colorValue);
		case 3:
			return Color.Lerp(Global.COLORSET_2[0], Global.COLORSET_2[1], colorValue);
		case 4:
			return Color.Lerp(Global.COLORSET_3[0], Global.COLORSET_3[1], colorValue);
		case 5:
			return Color.Lerp(Global.COLORSET_4[0], Global.COLORSET_4[1], colorValue);
		case 6:
			return Color.Lerp(Global.COLORSET_5[0], Global.COLORSET_5[1], colorValue);
		case 7:
			return Color.Lerp(Global.COLORSET_6[0], Global.COLORSET_6[1], colorValue);
		case 8:
			return Color.Lerp(Global.COLORSET_7[0], Global.COLORSET_7[1], colorValue);
		case 9:
			return Color.Lerp(Global.COLORSET_8[0], Global.COLORSET_8[1], colorValue);
		case 10:
			return Color.Lerp(Global.COLORSET_9[0], Global.COLORSET_9[1], colorValue);
		default:
			return Color.Lerp(Global.COLORSET_1[0], Global.COLORSET_1[1], colorValue);
		}
	}

	public void SwtichCustomColor()
	{
		useCustomColor = !useCustomColor;
		SetUserCustomColor(useCustomColor);
	}

	public bool SetUserCustomColor(bool useCustom)
	{
		bool result = false;
		Material[] array = mats;
		foreach (Material material in array)
		{
			if (material != null && material.HasProperty("_useCustomColor"))
			{
				useCustomColor = useCustom;
				if (useCustomColor)
				{
					material.SetFloat("_useCustomColor", 1f);
				}
				else
				{
					material.SetFloat("_useCustomColor", 0f);
				}
				UserState userState = GameApp.GetInstance().GetUserState();
				if (userState.GetCurrentCustomize().mAvatarColorArray[(int)m_BodyType].mMode == 0 && useCustom)
				{
					result = true;
				}
			}
		}
		return result;
	}

	public void UpdateColor(float newSexiang, Color newColor)
	{
		if (mats == null)
		{
			return;
		}
		Material[] array = mats;
		foreach (Material material in array)
		{
			if (material.HasProperty("_seXiang"))
			{
				SetSeXiang(material, newSexiang);
			}
			if (material.HasProperty("_paintColor"))
			{
				material.SetColor("_paintColor", newColor);
			}
		}
	}

	public static void SetSeXiang(Material mat, float sexiang)
	{
		if (sexiang >= 1f)
		{
			sexiang--;
		}
		float num = sexiang * 6f;
		int num2 = (int)Mathf.Floor(num);
		Vector4 vector = default(Vector4);
		Vector4 vector2 = default(Vector4);
		float num3 = num - (float)num2;
		float new_w = 1f - num3;
		switch (num2)
		{
		case 0:
			vector.Set(0f, 0f, 0f, 0f);
			vector2.Set(0f, 1f, num3, new_w);
			break;
		case 1:
			vector.Set(0f, 0f, 0f, 1f);
			vector2.Set(0f, 0f, num3, new_w);
			break;
		case 2:
			vector.Set(0f, 0f, 1f, 0f);
			vector2.Set(0f, 0f, num3, new_w);
			break;
		case 3:
			vector.Set(0f, 1f, 0f, 0f);
			vector2.Set(0f, 0f, num3, new_w);
			break;
		case 4:
			vector.Set(1f, 0f, 0f, 0f);
			vector2.Set(0f, 0f, num3, new_w);
			break;
		default:
			vector.Set(0f, 0f, 0f, 0f);
			vector2.Set(1f, 0f, num3, new_w);
			break;
		}
		mat.SetFloat("_seXiang", sexiang);
		mat.SetVector("_seXiang_vector1", vector);
		mat.SetVector("_seXiang_vector2", vector2);
	}

	public float GetHSV(Color color)
	{
		float num = Mathf.Max(color.r, color.g, color.b);
		float num2 = Mathf.Min(color.r, color.g, color.b);
		float num3 = 0f;
		if (num == num2)
		{
			num3 = 0f;
		}
		else if (num == color.r && color.g >= color.b)
		{
			num3 = 60f * ((color.g - color.b) / (num - num2));
		}
		else if (num == color.r && color.g < color.b)
		{
			num3 = 60f * ((color.g - color.b) / (num - num2)) + 360f;
		}
		else if (num == color.g)
		{
			num3 = 60f * ((color.b - color.r) / (num - num2)) + 120f;
		}
		else if (num == color.b)
		{
			num3 = 60f * ((color.r - color.g) / (num - num2)) + 240f;
		}
		return num3 / 360f;
	}
}
