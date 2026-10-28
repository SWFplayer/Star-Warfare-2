using UnityEngine;

public class BossStickManager : MonoBehaviour
{
	public Joystick m_MoveJoystick;

	public Joystick[] m_SkillJoysticks;

	public GameObject m_CenterObject;

	public Camera UICamera;

	private Vector3 offset = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);

	private UIRoot uiRoot;

	private bool mInit;

	private bool mChangeSide;

	private static BossStickManager mInstance;

	private void Awake()
	{
		uiRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
		mInit = false;
		mInstance = this;
		ChangeSide();
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	public static BossStickManager GetInstance()
	{
		return mInstance;
	}

	public void SetAllActiveRecursively(bool state)
	{
		if (UserStateHUD.GetInstance().Joystick.IsFixed)
		{
			m_MoveJoystick.gameObject.SetActive(state);
		}
		else
		{
			m_MoveJoystick.gameObject.SetActive(false);
		}
		Joystick[] skillJoysticks = m_SkillJoysticks;
		foreach (Joystick joystick in skillJoysticks)
		{
			joystick.gameObject.SetActive(state);
		}
	}

	private Vector2 GetMoveJoystickCenter()
	{
		Vector3 vector = UICamera.WorldToScreenPoint(m_MoveJoystick.transform.position);
		Debug.Log("GetMoveJoystickCenter : " + vector);
		return vector;
	}

	private Vector2 GetShootJoystickCenter()
	{
		Vector3 vector = UICamera.WorldToScreenPoint(m_CenterObject.transform.position);
		Debug.Log("GetShootJoystickCenter : " + vector);
		return vector;
	}

	private float GetMoveJoystickRadius()
	{
		float num = NGUITools.FindInParents<UIRoot>(base.gameObject).activeHeight;
		Debug.Log("GetMoveJoystickRadius : " + m_MoveJoystick.radius * (float)Screen.height / num);
		return m_MoveJoystick.radius * (float)Screen.height / num;
	}

	private void Update()
	{
		if (!mInit)
		{
			mInit = true;
			UserStateHUD.GetInstance().Joystick.MoveCenter = GetMoveJoystickCenter();
			UserStateHUD.GetInstance().Joystick.ShootCenter = GetShootJoystickCenter();
			UserStateHUD.GetInstance().Joystick.Radius = GetMoveJoystickRadius();
			GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.InputController.ResetJoystickStatus();
		}
	}

	private void LateUpdate()
	{
		if (mChangeSide)
		{
			mChangeSide = false;
			UserStateHUD.GetInstance().Joystick.MoveCenter = GetMoveJoystickCenter();
			UserStateHUD.GetInstance().Joystick.ShootCenter = GetShootJoystickCenter();
			UserStateHUD.GetInstance().Joystick.Radius = GetMoveJoystickRadius();
			GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.InputController.ResetJoystickStatus();
		}
	}

	public void Hide()
	{
		m_MoveJoystick.Hide();
		Joystick[] skillJoysticks = m_SkillJoysticks;
		foreach (Joystick joystick in skillJoysticks)
		{
			joystick.Hide();
		}
	}

	public void Show()
	{
		m_MoveJoystick.Show();
		Joystick[] skillJoysticks = m_SkillJoysticks;
		foreach (Joystick joystick in skillJoysticks)
		{
			joystick.Show();
		}
	}

	public void ChangeSide()
	{
		bool changeSide = GameApp.GetInstance().GetUserState().GetChangeSide();
		UIAnchor component = base.gameObject.transform.Find("Anchor-BottomLeft").GetComponent<UIAnchor>();
		UIAnchor component2 = base.gameObject.transform.Find("Anchor-BottomRight").GetComponent<UIAnchor>();
		if (changeSide)
		{
			component.side = UIAnchor.Side.BottomRight;
			component.relativeOffset = new Vector2(-0.37f, 0f);
			component2.side = UIAnchor.Side.BottomLeft;
			component2.relativeOffset = new Vector2(0.37f, 0f);
		}
		else
		{
			component.side = UIAnchor.Side.BottomLeft;
			component.relativeOffset = new Vector2(0f, 0f);
			component2.side = UIAnchor.Side.BottomRight;
			component2.relativeOffset = new Vector2(0f, 0f);
		}
		mChangeSide = true;
	}
}
