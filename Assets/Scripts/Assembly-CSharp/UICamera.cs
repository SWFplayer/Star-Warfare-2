using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("NGUI/UI/Camera")]
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class UICamera : MonoBehaviour
{
	public enum ClickNotification
	{
		None = 0,
		Always = 1,
		BasedOnDelta = 2
	}

	public class MouseOrTouch
	{
		public Vector2 pos;

		public Vector2 delta;

		public Vector2 totalDelta;

		public Camera pressedCam;

		public GameObject current;

		public GameObject pressed;

		public GameObject dragged;

		public float clickTime;

		public ClickNotification clickNotification = ClickNotification.Always;

		public bool touchBegan = true;

		public bool pressStarted;

		public bool dragStarted;
	}

	public enum EventType
	{
		World = 0,
		UI = 1
	}

	private class Highlighted
	{
		public GameObject go;

		public int counter;
	}

	private struct DepthEntry
	{
		public int depth;

		public RaycastHit hit;
	}

	public delegate void OnCustomInput();

	public static List<UICamera> list = new List<UICamera>();

	public EventType eventType = EventType.UI;

	public LayerMask eventReceiverMask = -1;

	public bool debug;

	public bool useMouse = true;

	public bool useTouch = true;

	public bool allowMultiTouch = true;

	public bool useKeyboard = true;

	public bool useController = true;

	public bool stickyPress = true;

	public bool stickyTooltip = true;

	public float tooltipDelay = 1f;

	public float mouseDragThreshold = 4f;

	public float mouseClickThreshold = 10f;

	public float touchDragThreshold = 40f;

	public float touchClickThreshold = 40f;

	public float rangeDistance = -1f;

	public string scrollAxisName = "Mouse ScrollWheel";

	public string verticalAxisName = "Vertical";

	public string horizontalAxisName = "Horizontal";

	public KeyCode submitKey0 = KeyCode.Return;

	public KeyCode submitKey1 = KeyCode.JoystickButton0;

	public KeyCode cancelKey0 = KeyCode.Escape;

	public KeyCode cancelKey1 = KeyCode.JoystickButton1;

	public static OnCustomInput onCustomInput;

	public static bool showTooltips = true;

	public static Vector2 lastTouchPosition = Vector2.zero;

	public static RaycastHit lastHit;

	public static UICamera current = null;

	public static Camera currentCamera = null;

	public static int currentTouchID = -1;

	public static MouseOrTouch currentTouch = null;

	public static bool inputHasFocus = false;

	public static GameObject genericEventHandler;

	public static GameObject fallThrough;

	private static List<Highlighted> mHighlighted = new List<Highlighted>();

	private static GameObject mCurrentSelection = null;

	private static GameObject mNextSelection = null;

	private static MouseOrTouch[] mMouse = new MouseOrTouch[3]
	{
		new MouseOrTouch(),
		new MouseOrTouch(),
		new MouseOrTouch()
	};

	private static GameObject mHover;

	private static MouseOrTouch mController = new MouseOrTouch();

	private static float mNextEvent = 0f;

	private static Dictionary<int, MouseOrTouch> mTouches = new Dictionary<int, MouseOrTouch>();

	private GameObject mTooltip;

	private Camera mCam;

	private LayerMask mLayerMask;

	private float mTooltipTime;

	private bool mIsEditor;

	public static bool isDragging = false;

	public static GameObject hoveredObject;

	private static DepthEntry mHit = default(DepthEntry);

	private static BetterList<DepthEntry> mHits = new BetterList<DepthEntry>();

	private static RaycastHit mEmpty = default(RaycastHit);

	private bool handlesEvents
	{
		get
		{
			return eventHandler == this;
		}
	}

	public Camera cachedCamera
	{
		get
		{
			if (mCam == null)
			{
				mCam = base.GetComponent<Camera>();
			}
			return mCam;
		}
	}

	public static GameObject selectedObject
	{
		get
		{
			return mCurrentSelection;
		}
		set
		{
			if (mNextSelection != null)
			{
				mNextSelection = value;
			}
			else
			{
				if (!(mCurrentSelection != value))
				{
					return;
				}
				if (mCurrentSelection != null)
				{
					UICamera uICamera = FindCameraForLayer(mCurrentSelection.layer);
					if (uICamera != null)
					{
						current = uICamera;
						currentCamera = uICamera.mCam;
						Notify(mCurrentSelection, "OnSelect", false);
						if (uICamera.useController || uICamera.useKeyboard)
						{
							Highlight(mCurrentSelection, false);
						}
						current = null;
					}
				}
				mCurrentSelection = null;
				mNextSelection = value;
				if (mNextSelection != null)
				{
					UICamera uICamera2 = FindCameraForLayer(mNextSelection.layer);
					if (uICamera2 != null)
					{
						uICamera2.StartCoroutine(uICamera2.ChangeSelection(mNextSelection));
					}
				}
			}
		}
	}

	public static int touchCount
	{
		get
		{
			int num = 0;
			foreach (KeyValuePair<int, MouseOrTouch> mTouch in mTouches)
			{
				if (mTouch.Value.pressed != null)
				{
					num++;
				}
			}
			for (int i = 0; i < mMouse.Length; i++)
			{
				if (mMouse[i].pressed != null)
				{
					num++;
				}
			}
			if (mController.pressed != null)
			{
				num++;
			}
			return num;
		}
	}

	public static int dragCount
	{
		get
		{
			int num = 0;
			foreach (KeyValuePair<int, MouseOrTouch> mTouch in mTouches)
			{
				if (mTouch.Value.dragged != null)
				{
					num++;
				}
			}
			for (int i = 0; i < mMouse.Length; i++)
			{
				if (mMouse[i].dragged != null)
				{
					num++;
				}
			}
			if (mController.dragged != null)
			{
				num++;
			}
			return num;
		}
	}

	public static Camera mainCamera
	{
		get
		{
			UICamera uICamera = eventHandler;
			return (!(uICamera != null)) ? null : uICamera.cachedCamera;
		}
	}

	public static UICamera eventHandler
	{
		get
		{
			for (int i = 0; i < list.Count; i++)
			{
				UICamera uICamera = list[i];
				if (!(uICamera == null) && uICamera.enabled && NGUITools.GetActive(uICamera.gameObject))
				{
					return uICamera;
				}
			}
			return null;
		}
	}

	private IEnumerator ChangeSelection(GameObject go)
	{
		yield return new WaitForEndOfFrame();
		mCurrentSelection = go;
		mNextSelection = null;
		if (mCurrentSelection != null)
		{
			current = this;
			currentCamera = mCam;
			if (useController || useKeyboard)
			{
				Highlight(mCurrentSelection, true);
			}
			Notify(mCurrentSelection, "OnSelect", true);
			current = null;
		}
	}

	private void OnApplicationQuit()
	{
		mHighlighted.Clear();
	}

	private static int CompareFunc(UICamera a, UICamera b)
	{
		if (a.cachedCamera.depth < b.cachedCamera.depth)
		{
			return 1;
		}
		if (a.cachedCamera.depth > b.cachedCamera.depth)
		{
			return -1;
		}
		return 0;
	}

	public static bool Raycast(Vector3 inPos, out RaycastHit hit)
	{
		for (int i = 0; i < list.Count; i++)
		{
			UICamera uICamera = list[i];
			if (!uICamera.enabled || !NGUITools.GetActive(uICamera.gameObject))
			{
				continue;
			}
			currentCamera = uICamera.cachedCamera;
			Vector3 vector = currentCamera.ScreenToViewportPoint(inPos);
			if (float.IsNaN(vector.x) || float.IsNaN(vector.y) || vector.x < 0f || vector.x > 1f || vector.y < 0f || vector.y > 1f)
			{
				continue;
			}
			Ray ray = currentCamera.ScreenPointToRay(inPos);
			int layerMask = currentCamera.cullingMask & (int)uICamera.eventReceiverMask;
			float distance = ((!(uICamera.rangeDistance > 0f)) ? (currentCamera.farClipPlane - currentCamera.nearClipPlane) : uICamera.rangeDistance);
			if (uICamera.eventType == EventType.World)
			{
				if (Physics.Raycast(ray, out hit, distance, layerMask))
				{
					hoveredObject = hit.collider.gameObject;
					return true;
				}
			}
			else
			{
				if (uICamera.eventType != EventType.UI)
				{
					continue;
				}
				RaycastHit[] array = Physics.RaycastAll(ray, distance, layerMask);
				if (array.Length > 1)
				{
					for (int j = 0; j < array.Length; j++)
					{
						GameObject go = array[j].collider.gameObject;
						mHit.depth = NGUITools.CalculateRaycastDepth(go);
						if (mHit.depth != int.MaxValue)
						{
							mHit.hit = array[j];
							mHits.Add(mHit);
						}
					}
					mHits.Sort((DepthEntry r1, DepthEntry r2) => r2.depth.CompareTo(r1.depth));
					for (int num = 0; num < mHits.size; num++)
					{
						if (IsVisible(ref mHits.buffer[num]))
						{
							hit = mHits[num].hit;
							hoveredObject = hit.collider.gameObject;
							mHits.Clear();
							return true;
						}
					}
					mHits.Clear();
				}
				else if (array.Length == 1 && IsVisible(ref array[0]))
				{
					hit = array[0];
					hoveredObject = hit.collider.gameObject;
					return true;
				}
			}
		}
		hit = mEmpty;
		return false;
	}

	private static bool IsVisible(ref RaycastHit hit)
	{
		UIPanel uIPanel = NGUITools.FindInParents<UIPanel>(hit.collider.gameObject);
		if (uIPanel == null || uIPanel.IsVisible(hit.point))
		{
			return true;
		}
		return false;
	}

	private static bool IsVisible(ref DepthEntry de)
	{
		UIPanel uIPanel = NGUITools.FindInParents<UIPanel>(de.hit.collider.gameObject);
		return uIPanel == null || uIPanel.IsVisible(de.hit.point);
	}

	public static UICamera FindCameraForLayer(int layer)
	{
		int num = 1 << layer;
		for (int i = 0; i < list.Count; i++)
		{
			UICamera uICamera = list[i];
			Camera camera = uICamera.cachedCamera;
			if (camera != null && (camera.cullingMask & num) != 0)
			{
				return uICamera;
			}
		}
		return null;
	}

	private static int GetDirection(KeyCode up, KeyCode down)
	{
		if (Input.GetKeyDown(up))
		{
			return 1;
		}
		if (Input.GetKeyDown(down))
		{
			return -1;
		}
		return 0;
	}

	private static int GetDirection(KeyCode up0, KeyCode up1, KeyCode down0, KeyCode down1)
	{
		if (Input.GetKeyDown(up0) || Input.GetKeyDown(up1))
		{
			return 1;
		}
		if (Input.GetKeyDown(down0) || Input.GetKeyDown(down1))
		{
			return -1;
		}
		return 0;
	}

	private static int GetDirection(string axis)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		if (mNextEvent < realtimeSinceStartup)
		{
			float axis2 = Input.GetAxis(axis);
			if (axis2 > 0.75f)
			{
				mNextEvent = realtimeSinceStartup + 0.25f;
				return 1;
			}
			if (axis2 < -0.75f)
			{
				mNextEvent = realtimeSinceStartup + 0.25f;
				return -1;
			}
		}
		return 0;
	}

	public static bool IsHighlighted(GameObject go)
	{
		int num = mHighlighted.Count;
		while (num > 0)
		{
			Highlighted highlighted = mHighlighted[--num];
			if (highlighted.go == go)
			{
				return true;
			}
		}
		return false;
	}

	private static void Highlight(GameObject go, bool highlighted)
	{
		if (!(go != null))
		{
			return;
		}
		int num = mHighlighted.Count;
		while (num > 0)
		{
			Highlighted highlighted2 = mHighlighted[--num];
			if (highlighted2 == null || highlighted2.go == null)
			{
				mHighlighted.RemoveAt(num);
			}
			else if (highlighted2.go == go)
			{
				if (highlighted)
				{
					highlighted2.counter++;
				}
				else if (--highlighted2.counter < 1)
				{
					mHighlighted.Remove(highlighted2);
					Notify(go, "OnHover", false);
				}
				return;
			}
		}
		if (highlighted)
		{
			Highlighted highlighted3 = new Highlighted();
			highlighted3.go = go;
			highlighted3.counter = 1;
			mHighlighted.Add(highlighted3);
			Notify(go, "OnHover", true);
		}
	}

	public static void Notify(GameObject go, string funcName, object obj)
	{
		if (go != null)
		{
			go.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
			if (genericEventHandler != null && genericEventHandler != go)
			{
				genericEventHandler.SendMessage(funcName, obj, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	public static MouseOrTouch GetTouch(int id)
	{
		MouseOrTouch value = null;
		if (!mTouches.TryGetValue(id, out value))
		{
			value = new MouseOrTouch();
			value.touchBegan = true;
			mTouches.Add(id, value);
		}
		return value;
	}

	public static void RemoveTouch(int id)
	{
		mTouches.Remove(id);
	}

	private void Awake()
	{
		cachedCamera.eventMask = 0;
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			useMouse = false;
			useTouch = true;
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				useKeyboard = false;
				useController = false;
			}
		}
		else if (Application.platform == RuntimePlatform.PS3 || Application.platform == RuntimePlatform.XBOX360)
		{
			useMouse = false;
			useTouch = false;
			useKeyboard = false;
			useController = true;
		}
		else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
		{
			mIsEditor = true;
		}
		mMouse[0].pos.x = Input.mousePosition.x;
		mMouse[0].pos.y = Input.mousePosition.y;
		lastTouchPosition = mMouse[0].pos;
		if ((int)eventReceiverMask == -1)
		{
			eventReceiverMask = cachedCamera.cullingMask;
		}
	}

	private void OnEnable()
	{
		list.Add(this);
		list.Sort(CompareFunc);
	}

	private void OnDisable()
	{
		list.Remove(this);
	}

	private void FixedUpdate()
	{
		if (useMouse && Application.isPlaying && handlesEvents)
		{
			if (!Raycast(Input.mousePosition, out lastHit))
			{
				hoveredObject = fallThrough;
			}
			if (hoveredObject == null)
			{
				hoveredObject = genericEventHandler;
			}
			for (int i = 0; i < 3; i++)
			{
				mMouse[i].current = hoveredObject;
			}
		}
	}

	private void Update()
	{
		if (!Application.isPlaying || !handlesEvents)
		{
			return;
		}
		current = this;
		if (useTouch)
		{
			if (mIsEditor)
			{
				ProcessMouse();
			}
			else
			{
				ProcessTouches();
				if (useMouse && Input.touchCount == 0)
				{
					ProcessMouse();
				}
			}
		}
		else if (useMouse)
		{
			ProcessMouse();
		}
		if (onCustomInput != null)
		{
			onCustomInput();
		}
		if (useMouse && mCurrentSelection != null && ((cancelKey0 != KeyCode.None && Input.GetKeyDown(cancelKey0)) || (cancelKey1 != KeyCode.None && Input.GetKeyDown(cancelKey1))))
		{
			selectedObject = null;
		}
		if (mCurrentSelection != null)
		{
			string text = Input.inputString;
			if (useKeyboard && Input.GetKeyDown(KeyCode.Delete))
			{
				text += "\b";
			}
			if (text.Length > 0)
			{
				if (!stickyTooltip && mTooltip != null)
				{
					ShowTooltip(false);
				}
				Notify(mCurrentSelection, "OnInput", text);
			}
		}
		else
		{
			inputHasFocus = false;
		}
		if (mCurrentSelection != null)
		{
			ProcessOthers();
		}
		if (useMouse && mHover != null)
		{
			float axis = Input.GetAxis(scrollAxisName);
			if (axis != 0f)
			{
				Notify(mHover, "OnScroll", axis);
			}
			if (showTooltips && mTooltipTime != 0f && (mTooltipTime < Time.realtimeSinceStartup || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
			{
				mTooltip = mHover;
				ShowTooltip(true);
			}
		}
		current = null;
	}

	public void ProcessMouse()
	{
		bool flag = useMouse && Time.timeScale < 0.9f;
		if (!flag)
		{
			for (int i = 0; i < 3; i++)
			{
				if (Input.GetMouseButton(i) || Input.GetMouseButtonUp(i))
				{
					flag = true;
					break;
				}
			}
		}
		mMouse[0].pos = Input.mousePosition;
		mMouse[0].delta = mMouse[0].pos - lastTouchPosition;
		bool flag2 = mMouse[0].pos != lastTouchPosition;
		lastTouchPosition = mMouse[0].pos;
		if (flag)
		{
			if (!Raycast(Input.mousePosition, out lastHit))
			{
				hoveredObject = fallThrough;
			}
			if (hoveredObject == null)
			{
				hoveredObject = genericEventHandler;
			}
			mMouse[0].current = hoveredObject;
		}
		for (int j = 1; j < 3; j++)
		{
			mMouse[j].pos = mMouse[0].pos;
			mMouse[j].delta = mMouse[0].delta;
			mMouse[j].current = mMouse[0].current;
		}
		bool flag3 = false;
		for (int k = 0; k < 3; k++)
		{
			if (Input.GetMouseButton(k))
			{
				flag3 = true;
				break;
			}
		}
		if (flag3)
		{
			mTooltipTime = 0f;
		}
		else if (useMouse && flag2 && (!stickyTooltip || mHover != mMouse[0].current))
		{
			if (mTooltipTime != 0f)
			{
				mTooltipTime = Time.realtimeSinceStartup + tooltipDelay;
			}
			else if (mTooltip != null)
			{
				ShowTooltip(false);
			}
		}
		if (useMouse && !flag3 && mHover != null && mHover != mMouse[0].current)
		{
			if (mTooltip != null)
			{
				ShowTooltip(false);
			}
			Highlight(mHover, false);
			mHover = null;
		}
		if (useMouse)
		{
			for (int l = 0; l < 3; l++)
			{
				bool mouseButtonDown = Input.GetMouseButtonDown(l);
				bool mouseButtonUp = Input.GetMouseButtonUp(l);
				currentTouch = mMouse[l];
				currentTouchID = -1 - l;
				if (mouseButtonDown)
				{
					currentTouch.pressedCam = currentCamera;
				}
				else if (currentTouch.pressed != null)
				{
					currentCamera = currentTouch.pressedCam;
				}
				ProcessTouch(mouseButtonDown, mouseButtonUp);
			}
			currentTouch = null;
		}
		if (useMouse && !flag3 && mHover != mMouse[0].current)
		{
			mTooltipTime = Time.realtimeSinceStartup + tooltipDelay;
			mHover = mMouse[0].current;
			Highlight(mHover, true);
		}
	}

	public void ProcessTouches()
	{
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch touch = Input.GetTouch(i);
			currentTouchID = ((!allowMultiTouch) ? 1 : touch.fingerId);
			currentTouch = GetTouch(currentTouchID);
			bool flag = touch.phase == TouchPhase.Began || currentTouch.touchBegan;
			bool flag2 = touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended;
			currentTouch.touchBegan = false;
			if (flag)
			{
				currentTouch.delta = Vector2.zero;
			}
			else
			{
				currentTouch.delta = touch.position - currentTouch.pos;
			}
			currentTouch.pos = touch.position;
			if (!Raycast(currentTouch.pos, out lastHit))
			{
				hoveredObject = fallThrough;
			}
			if (hoveredObject == null)
			{
				hoveredObject = genericEventHandler;
			}
			currentTouch.current = hoveredObject;
			lastTouchPosition = currentTouch.pos;
			if (flag)
			{
				currentTouch.pressedCam = currentCamera;
			}
			else if (currentTouch.pressed != null)
			{
				currentCamera = currentTouch.pressedCam;
			}
			if (touch.tapCount > 1)
			{
				currentTouch.clickTime = Time.realtimeSinceStartup;
			}
			ProcessTouch(flag, flag2);
			if (flag2)
			{
				RemoveTouch(currentTouchID);
			}
			currentTouch = null;
			if (!allowMultiTouch)
			{
				break;
			}
		}
	}

	public void ProcessOthers()
	{
		currentTouchID = -100;
		currentTouch = mController;
		inputHasFocus = mCurrentSelection != null && mCurrentSelection.GetComponent<UIInput>() != null;
		bool flag = (submitKey0 != KeyCode.None && Input.GetKeyDown(submitKey0)) || (submitKey1 != KeyCode.None && Input.GetKeyDown(submitKey1));
		bool flag2 = (submitKey0 != KeyCode.None && Input.GetKeyUp(submitKey0)) || (submitKey1 != KeyCode.None && Input.GetKeyUp(submitKey1));
		if (flag || flag2)
		{
			currentTouch.current = mCurrentSelection;
			ProcessTouch(flag, flag2);
			currentTouch.current = null;
		}
		int num = 0;
		int num2 = 0;
		if (useKeyboard)
		{
			if (inputHasFocus)
			{
				num += GetDirection(KeyCode.UpArrow, KeyCode.DownArrow);
				num2 += GetDirection(KeyCode.RightArrow, KeyCode.LeftArrow);
			}
			else
			{
				num += GetDirection(KeyCode.W, KeyCode.UpArrow, KeyCode.S, KeyCode.DownArrow);
				num2 += GetDirection(KeyCode.D, KeyCode.RightArrow, KeyCode.A, KeyCode.LeftArrow);
			}
		}
		if (useController)
		{
			if (!string.IsNullOrEmpty(verticalAxisName))
			{
				num += GetDirection(verticalAxisName);
			}
			if (!string.IsNullOrEmpty(horizontalAxisName))
			{
				num2 += GetDirection(horizontalAxisName);
			}
		}
		if (num != 0)
		{
			Notify(mCurrentSelection, "OnKey", (num <= 0) ? KeyCode.DownArrow : KeyCode.UpArrow);
		}
		if (num2 != 0)
		{
			Notify(mCurrentSelection, "OnKey", (num2 <= 0) ? KeyCode.LeftArrow : KeyCode.RightArrow);
		}
		if (useKeyboard && Input.GetKeyDown(KeyCode.Tab))
		{
			Notify(mCurrentSelection, "OnKey", KeyCode.Tab);
		}
		if (cancelKey0 != KeyCode.None && Input.GetKeyDown(cancelKey0))
		{
			Notify(mCurrentSelection, "OnKey", KeyCode.Escape);
		}
		if (cancelKey1 != KeyCode.None && Input.GetKeyDown(cancelKey1))
		{
			Notify(mCurrentSelection, "OnKey", KeyCode.Escape);
		}
		currentTouch = null;
	}

	public void ProcessTouch(bool pressed, bool unpressed)
	{
		bool flag = currentTouch == mMouse[0] || currentTouch == mMouse[1] || currentTouch == mMouse[2];
		float num = ((!flag) ? touchDragThreshold : mouseDragThreshold);
		float num2 = ((!flag) ? touchClickThreshold : mouseClickThreshold);
		if (pressed)
		{
			if (mTooltip != null)
			{
				ShowTooltip(false);
			}
			currentTouch.pressStarted = true;
			Notify(currentTouch.pressed, "OnPress", false);
			currentTouch.pressed = currentTouch.current;
			currentTouch.dragged = currentTouch.current;
			currentTouch.clickNotification = ClickNotification.BasedOnDelta;
			currentTouch.totalDelta = Vector2.zero;
			currentTouch.dragStarted = false;
			Notify(currentTouch.pressed, "OnPress", true);
			if (currentTouch.pressed != mCurrentSelection)
			{
				if (mTooltip != null)
				{
					ShowTooltip(false);
				}
				selectedObject = null;
			}
		}
		else
		{
			if (!stickyPress && !unpressed && currentTouch.pressStarted && currentTouch.pressed != hoveredObject)
			{
				isDragging = true;
				Notify(currentTouch.pressed, "OnPress", false);
				currentTouch.pressed = hoveredObject;
				Notify(currentTouch.pressed, "OnPress", true);
				isDragging = false;
			}
			if (currentTouch.pressed != null)
			{
				float magnitude = currentTouch.delta.magnitude;
				if (magnitude != 0f)
				{
					currentTouch.totalDelta += currentTouch.delta;
					magnitude = currentTouch.totalDelta.magnitude;
					if (!currentTouch.dragStarted && num < magnitude)
					{
						currentTouch.dragStarted = true;
						currentTouch.delta = currentTouch.totalDelta;
					}
					if (currentTouch.dragStarted)
					{
						if (mTooltip != null)
						{
							ShowTooltip(false);
						}
						isDragging = true;
						bool flag2 = currentTouch.clickNotification == ClickNotification.None;
						Notify(currentTouch.dragged, "OnDrag", currentTouch.delta);
						isDragging = false;
						if (flag2)
						{
							currentTouch.clickNotification = ClickNotification.None;
						}
						else if (currentTouch.clickNotification == ClickNotification.BasedOnDelta && num2 < magnitude)
						{
							currentTouch.clickNotification = ClickNotification.None;
						}
					}
				}
			}
		}
		if (!unpressed)
		{
			return;
		}
		currentTouch.pressStarted = false;
		if (mTooltip != null)
		{
			ShowTooltip(false);
		}
		if (currentTouch.pressed != null)
		{
			Notify(currentTouch.pressed, "OnPress", false);
			if (useMouse && currentTouch.pressed == mHover)
			{
				Notify(currentTouch.pressed, "OnHover", true);
			}
			if (currentTouch.dragged == currentTouch.current || (currentTouch.clickNotification != ClickNotification.None && currentTouch.totalDelta.magnitude < num))
			{
				if (currentTouch.pressed != mCurrentSelection)
				{
					mNextSelection = null;
					mCurrentSelection = currentTouch.pressed;
					Notify(currentTouch.pressed, "OnSelect", true);
				}
				else
				{
					mNextSelection = null;
					mCurrentSelection = currentTouch.pressed;
				}
				if (currentTouch.clickNotification != ClickNotification.None)
				{
					float realtimeSinceStartup = Time.realtimeSinceStartup;
					Notify(currentTouch.pressed, "OnClick", null);
					if (currentTouch.clickTime + 0.35f > realtimeSinceStartup)
					{
						Notify(currentTouch.pressed, "OnDoubleClick", null);
					}
					currentTouch.clickTime = realtimeSinceStartup;
				}
			}
			else
			{
				Notify(currentTouch.current, "OnDrop", currentTouch.dragged);
			}
		}
		currentTouch.dragStarted = false;
		currentTouch.pressed = null;
		currentTouch.dragged = null;
	}

	public void ShowTooltip(bool val)
	{
		mTooltipTime = 0f;
		Notify(mTooltip, "OnTooltip", val);
		if (!val)
		{
			mTooltip = null;
		}
	}
}
