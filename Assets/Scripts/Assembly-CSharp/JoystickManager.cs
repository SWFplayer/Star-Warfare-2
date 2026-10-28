using UnityEngine;

public class JoystickManager : MonoBehaviour
{
	public Joystick m_MoveJoystick;

	public Joystick m_ShootJoystick;

	public Camera UICamera;

	private Vector3 offset = new Vector3((float)Screen.width / 2f, (float)Screen.height / 2f, 0f);

	private UIRoot uiRoot;

	private bool mInit;

	private bool mChangeSide;

	private static JoystickManager mInstance;

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

	public static JoystickManager GetInstance()
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
		m_ShootJoystick.gameObject.SetActive(state);
	}

	private Vector2 GetMoveJoystickCenter()
	{
		Vector3 vector = UICamera.WorldToScreenPoint(m_MoveJoystick.transform.position);
		return vector;
	}

	private Vector2 GetShootJoystickCenter()
	{
		Vector3 vector = UICamera.WorldToScreenPoint(m_ShootJoystick.transform.position);
		return vector;
	}

	private float GetMoveJoystickRadius()
	{
		float num = NGUITools.FindInParents<UIRoot>(base.gameObject).activeHeight;
		return m_MoveJoystick.radius * (float)Screen.height / num;
	}

	private float GetShootJoystickRadius()
	{
		float num = NGUITools.FindInParents<UIRoot>(base.gameObject).activeHeight;
		return m_ShootJoystick.radius * (float)Screen.height / num;
	}

	private void Update()
	{
        if (!mInit && GameApp.GetInstance().GetGameWorld() != null && GameApp.GetInstance().GetGameWorld().GetLocalPlayer() != null)
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
		m_ShootJoystick.Hide();
	}

	public void Show()
	{
		m_MoveJoystick.Show();
		m_ShootJoystick.Show();
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
