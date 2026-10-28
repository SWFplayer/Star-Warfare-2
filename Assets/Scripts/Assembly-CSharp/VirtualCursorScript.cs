using UnityEngine;

public class VirtualCursorScript : MonoBehaviour
{
	public GameObject Cursor;

	public static VirtualCursorScript mInstance;

	private bool mIsHide;

	private UICamera.MouseOrTouch mMouse = new UICamera.MouseOrTouch();

	private int Sensitivity;

	private KeyCode OKButton;

	public static void _HideCursor()
	{
		if (mInstance != null)
		{
			mInstance.HideCursor();
		}
	}

	public static void _ShowCursor()
	{
		if (mInstance != null)
		{
			mInstance.ShowCursor();
		}
	}

	private void Awake()
	{
		mInstance = this;
		Cursor.transform.position = base.gameObject.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.5f));
		mIsHide = false;
		OKButton = KeyCode.JoystickButton0;
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	private void ShowCursor()
	{
		mIsHide = false;
		Cursor.SetActive(true);
		Cursor.transform.position = base.gameObject.GetComponent<Camera>().ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0.5f));
	}

	private void HideCursor()
	{
		mIsHide = true;
		Cursor.SetActive(false);
	}

	private void Update()
	{
		if (!mIsHide)
		{
			Vector3 vector = default(Vector3);
			Vector3 vector2 = new Vector3(Input.GetAxis("AndroidGCHorizontal"), Input.GetAxis("AndroidGCVertical"), 0f);
			Vector3 vector3 = new Vector3(Input.GetAxis("AndroidGCHorizontal3"), Input.GetAxis("AndroidGCVertical3"), 0f);
			if (Time.timeScale < 0.5f)
			{
				vector2 = new Vector3(Input.GetAxisRaw("AndroidGCHorizontal"), Input.GetAxisRaw("AndroidGCVertical"), 0f);
				vector3 = new Vector3(Input.GetAxisRaw("AndroidGCHorizontal3"), Input.GetAxisRaw("AndroidGCVertical3"), 0f);
			}
			vector = ((!(vector2.magnitude > vector3.magnitude)) ? vector3 : vector2);
			if (vector.magnitude > 0.7f)
			{
				vector *= 4f;
			}
			else if (vector.magnitude > 0.35f)
			{
				vector *= 2f;
			}
			Sensitivity = GameApp.GetInstance().GetUserState().TouchInputSensitivity;
			vector *= (float)Sensitivity;
			Vector3 position = base.gameObject.GetComponent<Camera>().WorldToScreenPoint(Cursor.transform.position);
			position += vector;
			if (position.x > (float)Screen.width)
			{
				position.x = Screen.width;
			}
			else if (position.x < 0f)
			{
				position.x = 0f;
			}
			if (position.y > (float)Screen.height)
			{
				position.y = Screen.height;
			}
			else if (position.y < 0f)
			{
				position.y = 0f;
			}
			Cursor.transform.position = base.gameObject.GetComponent<Camera>().ScreenToWorldPoint(position);
			ProcessMouse();
		}
	}

	public void ProcessMouse()
	{
		bool flag = Time.timeScale < 0.9f;
		if (!flag)
		{
			for (int i = 0; i < 3; i++)
			{
				if (Input.GetKey(OKButton) || Input.GetKeyUp(OKButton))
				{
					flag = true;
					break;
				}
			}
		}
		Vector3 vector = base.gameObject.GetComponent<Camera>().WorldToScreenPoint(Cursor.transform.position);
		mMouse.pos = vector;
		mMouse.delta = mMouse.pos - UICamera.lastTouchPosition;
		bool flag2 = mMouse.pos != UICamera.lastTouchPosition;
		UICamera.lastTouchPosition = mMouse.pos;
		if (flag)
		{
			if (!UICamera.Raycast(vector, out UICamera.lastHit))
			{
				UICamera.hoveredObject = UICamera.fallThrough;
			}
			if (UICamera.hoveredObject == null)
			{
				UICamera.hoveredObject = UICamera.genericEventHandler;
			}
			mMouse.current = UICamera.hoveredObject;
		}
		bool keyDown = Input.GetKeyDown(OKButton);
		bool keyUp = Input.GetKeyUp(OKButton);
		UICamera.currentTouch = mMouse;
		UICamera.currentTouchID = -4;
		if (keyDown)
		{
			UICamera.currentTouch.pressedCam = UICamera.currentCamera;
		}
		else if (UICamera.currentTouch.pressed != null)
		{
			UICamera.currentCamera = UICamera.currentTouch.pressedCam;
		}
		ProcessTouch(keyDown, keyUp);
		UICamera.currentTouch = null;
	}

	public void ProcessTouch(bool pressed, bool unpressed)
	{
		float num = 4f;
		if (pressed)
		{
			UICamera.currentTouch.pressStarted = true;
			UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", false);
			UICamera.currentTouch.pressed = UICamera.currentTouch.current;
			UICamera.currentTouch.dragged = UICamera.currentTouch.current;
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.Always;
			UICamera.currentTouch.totalDelta = Vector2.zero;
			UICamera.currentTouch.dragStarted = false;
			UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", true);
		}
		else
		{
			if (!unpressed && UICamera.currentTouch.pressStarted && UICamera.currentTouch.pressed != UICamera.hoveredObject)
			{
				UICamera.isDragging = true;
				UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", false);
				UICamera.currentTouch.pressed = UICamera.hoveredObject;
				UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", true);
				UICamera.isDragging = false;
			}
			if (UICamera.currentTouch.pressed != null)
			{
				float magnitude = UICamera.currentTouch.delta.magnitude;
				if (magnitude != 0f)
				{
					UICamera.currentTouch.totalDelta += UICamera.currentTouch.delta;
					magnitude = UICamera.currentTouch.totalDelta.magnitude;
					if (!UICamera.currentTouch.dragStarted && num < magnitude)
					{
						UICamera.currentTouch.dragStarted = true;
						UICamera.currentTouch.delta = UICamera.currentTouch.totalDelta;
					}
					if (UICamera.currentTouch.dragStarted)
					{
						UICamera.isDragging = true;
						bool flag = UICamera.currentTouch.clickNotification == UICamera.ClickNotification.None;
						UICamera.Notify(UICamera.currentTouch.dragged, "OnDrag", UICamera.currentTouch.delta);
						UICamera.isDragging = false;
						if (flag)
						{
							UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
						}
						else if (UICamera.currentTouch.clickNotification == UICamera.ClickNotification.BasedOnDelta)
						{
							UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;
						}
					}
				}
			}
		}
		if (!unpressed)
		{
			return;
		}
		UICamera.currentTouch.pressStarted = false;
		if (UICamera.currentTouch.pressed != null)
		{
			UICamera.Notify(UICamera.currentTouch.pressed, "OnPress", false);
			if (UICamera.currentTouch.dragged == UICamera.currentTouch.current || (UICamera.currentTouch.clickNotification != UICamera.ClickNotification.None && UICamera.currentTouch.totalDelta.magnitude < num))
			{
				if (UICamera.currentTouch.clickNotification != UICamera.ClickNotification.None)
				{
					float realtimeSinceStartup = Time.realtimeSinceStartup;
					UICamera.Notify(UICamera.currentTouch.pressed, "OnClick", null);
					if (UICamera.currentTouch.clickTime + 0.35f > realtimeSinceStartup)
					{
						UICamera.Notify(UICamera.currentTouch.pressed, "OnDoubleClick", null);
					}
					UICamera.currentTouch.clickTime = realtimeSinceStartup;
				}
			}
			else
			{
				UICamera.Notify(UICamera.currentTouch.current, "OnDrop", UICamera.currentTouch.dragged);
			}
		}
		UICamera.currentTouch.dragStarted = false;
		UICamera.currentTouch.pressed = null;
		UICamera.currentTouch.dragged = null;
	}
}
