using UnityEngine;

public class InputController
{
	public InputInfo inputInfo = new InputInfo();

	public InputInfo previousInputInfo = new InputInfo();

	protected Vector3 moveDirection = Vector3.zero;

	protected TouchInfo lastMoveTouch = new TouchInfo();

	protected TouchInfo lastMoveTouch2 = new TouchInfo();

	protected float sensitivityFactor = 1f;

	protected Touch[] lastTouch = new Touch[2];

	public Vector2 cameraRotation = new Vector2(0f, 0f);

	protected Vector2 deflection;

	protected Vector2 thumbCenter;

	protected Vector2 thumbCenterToScreen;

	protected Vector2 shootThumbCenter;

	protected Vector2 shootThumbCenterToScreen;

	protected Vector2 lastShootTouch = default(Vector2);

	protected float touchX;

	protected float touchY;

	protected float thumbRadius;

	protected int thumbTouchFingerId = -1;

	protected int shootingTouchFingerId = -1;

	protected int moveTouchFingerId = -1;

	protected int moveTouchFingerId2 = -1;

	protected string phaseStr = ".";

	protected GameWorld gameScene;

	protected Player player;

	protected Rect blockRect;

	protected bool bBlock;

	public bool up;

	public bool down;

	public bool fire;

	public bool AutoFireSwitch;

	public Timer fireSwitchTimer;

	public float x;

	public float y;

	private float LeftJoystickControlTime;

	private float RightJoystickControlTime;

	private float TotalTime;

	protected float initSensitivityFactor = 1f;

	private bool isFireing;

	public bool Block
	{
		get
		{
			return bBlock;
		}
		set
		{
			bBlock = value;
		}
	}

	public Vector2 CameraRotation
	{
		get
		{
			return cameraRotation;
		}
		set
		{
			cameraRotation = value;
		}
	}

	public Vector2 ThumbCenterToScreen
	{
		get
		{
			return thumbCenterToScreen;
		}
	}

	public Vector2 LastTouchPos
	{
		get
		{
			return new Vector2(thumbCenterToScreen.x + touchX * thumbRadius, thumbCenterToScreen.y + touchY * thumbRadius);
		}
	}

	public Vector2 LastShootTouch
	{
		get
		{
			return lastShootTouch;
		}
	}

	public Vector2 ShootThumbCenterToScreen
	{
		get
		{
			return shootThumbCenterToScreen;
		}
	}

	public void ResetJoystickControlTime()
	{
		LeftJoystickControlTime = 0f;
		RightJoystickControlTime = 0f;
		TotalTime = 0f;
	}

	public void ResetSensitivity(byte sensitivity)
	{
		sensitivityFactor = Mathf.Pow(Global.SENSITIVITY_STEP, (int)sensitivity) * Global.MIN_SENSITIVITY;
		initSensitivityFactor = sensitivityFactor;
	}

	public void ResetSensitivity(byte sensitivity, bool isPadBoss)
	{
		if (isPadBoss)
		{
			sensitivity *= 2;
		}
		sensitivityFactor = Mathf.Pow(Global.SENSITIVITY_STEP, (int)sensitivity) * Global.MIN_SENSITIVITY;
		initSensitivityFactor = sensitivityFactor;
	}

	public void Init()
	{
		thumbCenterToScreen = UserStateHUD.GetInstance().Joystick.MoveCenter;
		shootThumbCenterToScreen = UserStateHUD.GetInstance().Joystick.ShootCenter;
		thumbRadius = UserStateHUD.GetInstance().Joystick.Radius;
		byte touchInputSensitivity = GameApp.GetInstance().GetUserState().TouchInputSensitivity;
		ResetSensitivity(touchInputSensitivity);
		lastShootTouch = shootThumbCenterToScreen;
		for (int i = 0; i < 2; i++)
		{
			lastTouch[i] = default(Touch);
		}
		gameScene = GameApp.GetInstance().GetGameWorld();
		player = gameScene.GetLocalPlayer();
		bBlock = false;
		if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android)
		{
			fireSwitchTimer = new Timer();
			fireSwitchTimer.SetTimer(0.2f, true);
		}
	}

	public void ResetJoystickStatus()
	{
		thumbCenterToScreen = UserStateHUD.GetInstance().Joystick.MoveCenter;
		shootThumbCenterToScreen = UserStateHUD.GetInstance().Joystick.ShootCenter;
		thumbRadius = UserStateHUD.GetInstance().Joystick.Radius;
	}

	public void Process()
	{
		if (bBlock)
		{
			cameraRotation = Vector2.zero;
			inputInfo.moving = false;
			inputInfo.fire = false;
			moveDirection = Vector3.zero;
			isFireing = false;
			return;
		}
		bool flag = false;
		if (Application.platform != RuntimePlatform.IPhonePlayer && Application.platform != RuntimePlatform.Android)
		{
			if (Input.GetButton("Positive1") && null != HUDBase.instance && null != HUDBase.instance.m_SkillHUDManager && HUDBase.instance.m_SkillHUDManager.m_SkillList.Count >= 1)
			{
				HUDBase.instance.m_SkillHUDManager.m_SkillList[0].OnClick();
			}
			if (Input.GetButton("Positive2") && null != HUDBase.instance && null != HUDBase.instance.m_SkillHUDManager && HUDBase.instance.m_SkillHUDManager.m_SkillList.Count >= 2)
			{
				HUDBase.instance.m_SkillHUDManager.m_SkillList[1].OnClick();
			}
			if (Input.GetButton("Positive3") && null != HUDBase.instance && null != HUDBase.instance.m_SkillHUDManager && HUDBase.instance.m_SkillHUDManager.m_SkillList.Count >= 3)
			{
				HUDBase.instance.m_SkillHUDManager.m_SkillList[2].OnClick();
			}
			if (Input.GetButton("FireSwitch") && fireSwitchTimer.Ready())
			{
				AutoFireSwitch = !AutoFireSwitch;
				fireSwitchTimer.Do();
			}
            if (Input.GetButtonDown("Positive4") && null != HUDBase.instance && null != HUDBase.instance.m_SkillHUDManager)
            {
                HUDBase.instance.m_Weapon.ChangeWeaponByJoystick();
                
            }
            if (CursorManager.Instance == null || CursorManager.Instance.isCursorLocked)
            {
                if (Input.GetButton("Fire1"))
                {
                    inputInfo.fire = true;
                }
                else
                {
                    inputInfo.fire = AutoFireSwitch;
                }
            }
            else
            {
                inputInfo.fire = false;
            }

            if (HUDBase.instance != null && HUDBase.instance.m_BossStickmanager != null)
            {
                var skillJoysticks = HUDBase.instance.m_BossStickmanager.m_SkillJoysticks;
                
                System.Action<int> triggerBossSkill = (index) =>
                {
                    if (index < skillJoysticks.Length && skillJoysticks[index] != null && skillJoysticks[index].gameObject.activeSelf)
                    {
                        
                        skillJoysticks[index].SendMessage("OnPress", true, SendMessageOptions.DontRequireReceiver);
                        
                        skillJoysticks[index].SendMessage("OnPress", false, SendMessageOptions.DontRequireReceiver);
                    }
                };
                
                if (Input.GetKeyDown(KeyCode.Alpha1)) triggerBossSkill(0);
                
                if (Input.GetKeyDown(KeyCode.Alpha2)) triggerBossSkill(1);
                
                if (Input.GetKeyDown(KeyCode.Alpha3)) triggerBossSkill(2);
                
                if (Input.GetKeyDown(KeyCode.Alpha4)) triggerBossSkill(3);
                
                if (Input.GetButtonDown("Fire1")) triggerBossSkill(0);
            }

            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
			cameraRotation.x = Input.GetAxis("Mouse X");
			cameraRotation.y = Input.GetAxis("Mouse Y");
            if (Input.GetKeyDown(KeyCode.Escape) && HUDBase.instance != null && GameApp.GetInstance().GetGameMode().IsSingle() && (ResultPageUI.mInstance == null || !ResultPageUI.mInstance.gameObject.activeSelf))
            {
                Time.timeScale = 0f;
                AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
                HUDBase.instance.OnPause();
                GameApp.GetInstance().GetUIStateManager().FrGoToPhase(14, false, true, false);
            }
            if (Input.GetKeyDown(KeyCode.Escape) && HUDBase.instance != null && GameApp.GetInstance().GetGameMode().IsCoopMode() && (ResultPageUI.mInstance == null || !ResultPageUI.mInstance.gameObject.activeSelf))
            {
                AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
                HUDBase.instance.OnPause();
                GameApp.GetInstance().GetUIStateManager().FrGoToPhase(14, false, true, false);
            }
            if (Input.GetKeyDown(KeyCode.Tab) && HUDBase.instance != null && GameApp.GetInstance().GetGameMode().IsVSMode())
            {
                HUDOnline onlineHud = HUDBase.instance as HUDOnline;

                if (onlineHud != null)
                {
                   
                    bool isCurrentlyOpen = onlineHud.ScorePage.activeSelf;
                    onlineHud.OpenScorePage(!isCurrentlyOpen);
                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (GameApp.GetInstance().GetGameMode().IsVSMode())
                {
                    if (HUDBase.instance != null && HUDBase.instance.m_KillStreakHUDManager != null && HUDBase.instance.m_KillStreakHUDManager.GetKillStreakList().Count > 0)
                    {
                        HUDBase.instance.m_KillStreakHUDManager.GetKillStreakList()[0].OnClick();
                    }
                }
                else if (HUDBase.instance != null && HUDBase.instance.m_StateItem != null && HUDBase.instance.m_StateItem.ItemsGrid != null && HUDBase.instance.m_StateItem.ItemsGrid.transform.childCount > 0 && HUDBase.instance.m_StateItem.ItemsGrid.transform.GetChild(0).GetComponent<ItemButton>() != null)
                {
                    HUDBase.instance.m_StateItem.ItemsGrid.transform.GetChild(0).GetComponent<ItemButton>().OnClick();
                }
            }
            if (HUDBase.instance != null && (HUDBase.instance.m_BossStickmanager == null || !HUDBase.instance.m_BossStickmanager.gameObject.activeInHierarchy))
            {

                System.Action<int> useItemSlot = (slotIndex) =>
                {
                    var itemsGrid = HUDBase.instance.m_StateItem;
                    if (itemsGrid != null && itemsGrid.ItemsGrid != null && itemsGrid.ItemsGrid.transform.childCount > slotIndex)
                    {
                        ItemButton btn = itemsGrid.ItemsGrid.transform.GetChild(slotIndex).GetComponent<ItemButton>();
                        if (btn != null) btn.OnClick();
                    }
                };

                if (Input.GetKeyDown(KeyCode.Alpha1)) useItemSlot(0);
                if (Input.GetKeyDown(KeyCode.Alpha2)) useItemSlot(1);
                if (Input.GetKeyDown(KeyCode.Alpha3)) useItemSlot(2);
                
                if (GameApp.GetInstance().GetGameMode().IsVSMode())
                {
                    System.Action<int> useKillStreakSlot = (slotIndex) =>
                    {
                        var killStreaks = HUDBase.instance.m_KillStreakHUDManager;
                        if (killStreaks != null && killStreaks.GetKillStreakList().Count > slotIndex)
                        {
                            killStreaks.GetKillStreakList()[slotIndex].OnClick();
                        }
                    };

                    if (Input.GetKeyDown(KeyCode.Alpha4)) useKillStreakSlot(0);
                    if (Input.GetKeyDown(KeyCode.Alpha5)) useKillStreakSlot(1);
                    if (Input.GetKeyDown(KeyCode.Alpha6)) useKillStreakSlot(2);
                }
            
            }
        }
		else
		{
			player = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
			touchX = 0f;
			touchY = 0f;
			cameraRotation.x = 0f;
			cameraRotation.y = 0f;
			inputInfo.fire = false;
			inputInfo.moving = false;
			if (Input.touchCount == 0)
			{
				thumbTouchFingerId = -1;
				shootingTouchFingerId = -1;
				lastShootTouch = shootThumbCenterToScreen;
			}
			sensitivityFactor = initSensitivityFactor;
			if (player.GetWeapon().GetWeaponType() == WeaponType.Sniper || player.GetWeapon().GetWeaponType() == WeaponType.UltraSniper)
			{
				Sniper sniper = player.GetWeapon() as Sniper;
				if (sniper.isAiming())
				{
					sensitivityFactor = initSensitivityFactor * 0.5f;
				}
			}
			bool flag2 = false;
			for (int i = 0; i < Input.touchCount && i != 2; i++)
			{
				Touch touch = Input.GetTouch(i);
				flag = blockRect.Contains(touch.position);
				Vector2 vector = touch.position - thumbCenterToScreen;
				bool flag3 = vector.sqrMagnitude < thumbRadius * thumbRadius;
				bool flag4 = touch.fingerId == thumbTouchFingerId;
				if (touch.phase == TouchPhase.Began)
				{
					if (!UserStateHUD.GetInstance().Joystick.IsFixed && ((!GameApp.GetInstance().GetUserState().GetChangeSide() && touch.position.x < (float)Screen.width * UserStateHUD.GetInstance().Joystick.MoveJoystickRatio) || (GameApp.GetInstance().GetUserState().GetChangeSide() && touch.position.x > (float)Screen.width * (1f - UserStateHUD.GetInstance().Joystick.MoveJoystickRatio))))
					{
						UserStateHUD.GetInstance().Joystick.MoveCenter = (thumbCenterToScreen = touch.position);
						flag3 = (touch.position - thumbCenterToScreen).sqrMagnitude < thumbRadius * thumbRadius;
						flag4 = touch.fingerId == thumbTouchFingerId;
					}
					if (flag3 || flag4)
					{
						UserStateHUD.GetInstance().Joystick.isMoveJoystickPressed = true;
					}
				}
				else if (touch.phase == TouchPhase.Stationary)
				{
					if (flag3 || flag4)
					{
						if (flag3)
						{
							touchX = vector.x / thumbRadius;
							touchY = vector.y / thumbRadius;
						}
						else
						{
							touchX = vector.x / thumbRadius;
							touchY = vector.y / thumbRadius;
							if (Mathf.Abs(touchX) > Mathf.Abs(touchY))
							{
								touchY /= Mathf.Abs(touchX);
								touchX = ((touchX > 0f) ? 1 : (-1));
							}
							else if (touchY != 0f)
							{
								touchX /= Mathf.Abs(touchY);
								touchY = ((touchY > 0f) ? 1 : (-1));
							}
							else
							{
								touchX = 0f;
								touchY = 0f;
							}
						}
						thumbTouchFingerId = touch.fingerId;
						inputInfo.moving = true;
					}
					else
					{
						Vector2 vector2 = touch.position - shootThumbCenterToScreen;
						bool flag5 = vector2.sqrMagnitude < thumbRadius * thumbRadius;
						flag2 = vector2.sqrMagnitude < thumbRadius * 0.1f * (thumbRadius * 0.1f);
						if (flag5 || shootingTouchFingerId == touch.fingerId)
						{
							if (flag5)
							{
								if (!GameApp.GetInstance().GetUserState().GetSteadyModeOn())
								{
									cameraRotation.x = Mathf.Clamp(vector2.x, 0f - thumbRadius, thumbRadius) * 0.005f * sensitivityFactor;
								}
								lastShootTouch = touch.position;
							}
							else
							{
								if (!GameApp.GetInstance().GetUserState().GetSteadyModeOn())
								{
									cameraRotation.x = Mathf.Sign(vector2.x) * thumbRadius * 0.01f * sensitivityFactor / 3f;
								}
								Vector2 normalized = (touch.position - shootThumbCenterToScreen).normalized;
								lastShootTouch = shootThumbCenterToScreen + normalized * thumbRadius;
							}
							inputInfo.fire = true;
							shootingTouchFingerId = touch.fingerId;
						}
					}
				}
				else if (touch.phase == TouchPhase.Moved)
				{
					if (flag3 || flag4)
					{
						if (flag3)
						{
							touchX = vector.x / thumbRadius;
							touchY = vector.y / thumbRadius;
						}
						else
						{
							touchX = vector.x / thumbRadius;
							touchY = vector.y / thumbRadius;
							if (Mathf.Abs(touchX) > Mathf.Abs(touchY))
							{
								touchY /= Mathf.Abs(touchX);
								touchX = ((touchX > 0f) ? 1 : (-1));
							}
							else if (touchY != 0f)
							{
								touchX /= Mathf.Abs(touchY);
								touchY = ((touchY > 0f) ? 1 : (-1));
							}
							else
							{
								touchX = 0f;
								touchY = 0f;
							}
						}
						thumbTouchFingerId = touch.fingerId;
						inputInfo.moving = true;
					}
					else
					{
						if (lastMoveTouch.phase == TouchPhase.Moved)
						{
							if (touch.fingerId == moveTouchFingerId)
							{
								cameraRotation.x = (touch.position.x - lastMoveTouch.position.x) * 0.3f * sensitivityFactor;
								cameraRotation.y = (touch.position.y - lastMoveTouch.position.y) * 0.16f * sensitivityFactor;
							}
							else if (touch.fingerId == moveTouchFingerId2)
							{
								cameraRotation.x = (touch.position.x - lastMoveTouch2.position.x) * 0.3f * sensitivityFactor;
								cameraRotation.y = (touch.position.y - lastMoveTouch2.position.y) * 0.16f * sensitivityFactor;
							}
						}
						if (moveTouchFingerId == -1)
						{
							moveTouchFingerId = touch.fingerId;
						}
						if (moveTouchFingerId != -1 && touch.fingerId != moveTouchFingerId)
						{
							moveTouchFingerId2 = touch.fingerId;
						}
						if (touch.fingerId == moveTouchFingerId)
						{
							lastMoveTouch.phase = TouchPhase.Moved;
							lastMoveTouch.position = touch.position;
						}
						if (touch.fingerId == moveTouchFingerId2)
						{
							lastMoveTouch2.phase = TouchPhase.Moved;
							lastMoveTouch2.position = touch.position;
						}
						Vector2 vector3 = touch.position - shootThumbCenterToScreen;
						bool flag6 = vector3.sqrMagnitude < thumbRadius * thumbRadius;
						flag2 = vector3.sqrMagnitude < thumbRadius * 0.1f * (thumbRadius * 0.1f);
						if (shootingTouchFingerId == touch.fingerId || flag6)
						{
							inputInfo.fire = true;
							if (flag6)
							{
								if (!GameApp.GetInstance().GetUserState().GetSteadyModeOn())
								{
									cameraRotation.x += Mathf.Clamp(vector3.x, 0f - thumbRadius, thumbRadius) * 0.002f * sensitivityFactor;
								}
								lastShootTouch = touch.position;
							}
							else
							{
								Vector2 normalized2 = (touch.position - shootThumbCenterToScreen).normalized;
								lastShootTouch = shootThumbCenterToScreen + normalized2 * thumbRadius;
								if (!GameApp.GetInstance().GetUserState().GetSteadyModeOn())
								{
									cameraRotation.x += Mathf.Sign(vector3.x) * thumbRadius * 0.006f * sensitivityFactor / 3f;
								}
							}
							shootingTouchFingerId = touch.fingerId;
						}
					}
				}
				else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
				{
					if (touch.fingerId == thumbTouchFingerId)
					{
						thumbTouchFingerId = -1;
					}
					if (touch.fingerId == shootingTouchFingerId)
					{
						shootingTouchFingerId = -1;
						lastShootTouch = shootThumbCenterToScreen;
					}
					if (touch.fingerId == moveTouchFingerId)
					{
						moveTouchFingerId = -1;
						lastMoveTouch.phase = TouchPhase.Ended;
					}
					if (touch.fingerId == moveTouchFingerId2)
					{
						moveTouchFingerId2 = -1;
						lastMoveTouch2.phase = TouchPhase.Ended;
					}
				}
				if ((player.GetWeapon().GetWeaponType() == WeaponType.Sniper || player.GetWeapon().GetWeaponType() == WeaponType.UltraSniper) && flag2)
				{
					Sniper sniper2 = player.GetWeapon() as Sniper;
					if (sniper2.isAiming())
					{
						cameraRotation.x = 0f;
						cameraRotation.y = 0f;
					}
				}
				lastTouch[i] = touch;
			}
			touchX = Mathf.Clamp(touchX, -1f, 1f);
			touchY = Mathf.Clamp(touchY, -1f, 1f);
			moveDirection = new Vector3(touchX, 0f, touchY);
		}
		if (GameAppScript.IsControllerConnected)
		{
			Vector3 vector4 = moveDirection;
			Vector3 vector5 = new Vector3(Input.GetAxis("AndroidGCHorizontal"), 0f, Input.GetAxis("AndroidGCVertical"));
			moveDirection = ((!(vector4.magnitude > vector5.magnitude)) ? vector5 : vector4);
			if (FreyrPlugin.CanUseVirtualCursor() == GCType.GC_FireTV)
			{
				if (Input.GetKeyDown(KeyCode.JoystickButton7))
				{
					isFireing = true;
				}
				if (Input.GetKeyUp(KeyCode.JoystickButton7))
				{
					isFireing = false;
				}
				inputInfo.fire = isFireing;
			}
			else if (Input.GetAxis("AndroidGCFire") > 0.12f)
			{
				inputInfo.fire = true;
			}
			if (inputInfo.fire)
			{
				cameraRotation.x = ((!(Mathf.Abs(Input.GetAxis("AndroidGCHorizontal2") * 0.5f) > Mathf.Abs(cameraRotation.x))) ? cameraRotation.x : (Input.GetAxis("AndroidGCHorizontal2") * 0.5f)) * sensitivityFactor;
				cameraRotation.y = ((!(Mathf.Abs(Input.GetAxis("AndroidGCVertical2") * 0.5f) > Mathf.Abs(cameraRotation.y))) ? cameraRotation.y : (Input.GetAxis("AndroidGCVertical2") * 0.5f)) * sensitivityFactor;
			}
			else
			{
				cameraRotation.x = ((!(Mathf.Abs(Input.GetAxis("AndroidGCHorizontal2") * 1.5f) > Mathf.Abs(cameraRotation.x))) ? cameraRotation.x : (Input.GetAxis("AndroidGCHorizontal2") * 1.5f)) * sensitivityFactor;
				cameraRotation.y = ((!(Mathf.Abs(Input.GetAxis("AndroidGCVertical2") * 1.5f) > Mathf.Abs(cameraRotation.y))) ? cameraRotation.y : (Input.GetAxis("AndroidGCVertical2") * 1.5f)) * sensitivityFactor;
			}
			if (Input.GetKeyUp(KeyCode.JoystickButton1) && null != HUDBase.instance && null != HUDBase.instance.m_SkillHUDManager && HUDBase.instance.m_SkillHUDManager.m_SkillList.Count >= 1)
			{
				HUDBase.instance.m_SkillHUDManager.m_SkillList[0].OnClick();
			}
			if (Input.GetKeyUp(KeyCode.JoystickButton3) && null != HUDBase.instance && null != HUDBase.instance.m_SkillHUDManager && HUDBase.instance.m_SkillHUDManager.m_SkillList.Count >= 2)
			{
				HUDBase.instance.m_SkillHUDManager.m_SkillList[1].OnClick();
			}
			if (Input.GetKeyUp(KeyCode.JoystickButton2) && null != HUDBase.instance && null != HUDBase.instance.m_SkillHUDManager && HUDBase.instance.m_SkillHUDManager.m_SkillList.Count >= 3)
			{
				HUDBase.instance.m_SkillHUDManager.m_SkillList[2].OnClick();
			}
			if (Input.GetKeyUp(KeyCode.JoystickButton0) && null != HUDBase.instance && null != HUDBase.instance.m_Weapon)
			{
				HUDBase.instance.m_Weapon.ChangeWeaponByJoystick();
			}
			if (Input.GetKeyUp(KeyCode.JoystickButton10) && HUDBase.instance != null && GameApp.GetInstance().GetGameMode().IsSingle() && (ResultPageUI.mInstance == null || !ResultPageUI.mInstance.gameObject.activeSelf))
			{
				Time.timeScale = 0f;
				AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
				HUDBase.instance.OnPause();
				GameApp.GetInstance().GetUIStateManager().FrGoToPhase(14, false, true, false);
			}
			if (Input.GetKeyUp(KeyCode.JoystickButton4))
			{
				if (GameApp.GetInstance().GetGameMode().IsVSMode())
				{
					if (HUDBase.instance != null && HUDBase.instance.m_KillStreakHUDManager != null && HUDBase.instance.m_KillStreakHUDManager.GetKillStreakList().Count > 0)
					{
						HUDBase.instance.m_KillStreakHUDManager.GetKillStreakList()[0].OnClick();
					}
				}
				else if (HUDBase.instance != null && HUDBase.instance.m_StateItem != null && HUDBase.instance.m_StateItem.ItemsGrid != null && HUDBase.instance.m_StateItem.ItemsGrid.transform.childCount > 0 && HUDBase.instance.m_StateItem.ItemsGrid.transform.GetChild(0).GetComponent<ItemButton>() != null)
				{
					HUDBase.instance.m_StateItem.ItemsGrid.transform.GetChild(0).GetComponent<ItemButton>().OnClick();
				}
			}
		}
		if (GameApp.GetInstance().GetUserState().GetHorizontalJoyStickRevert())
		{
			cameraRotation.x = 0f - cameraRotation.x;
		}
		if (GameApp.GetInstance().GetUserState().GetVerticalJoyStickRevert())
		{
			cameraRotation.y = 0f - cameraRotation.y;
		}
		if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.z))
		{
			if (moveDirection.x > 0f)
			{
				inputInfo.dir = MoveDirection.Right;
			}
			else
			{
				inputInfo.dir = MoveDirection.Left;
			}
		}
		else if (moveDirection.z > 0f)
		{
			inputInfo.dir = MoveDirection.Forward;
		}
		else
		{
			inputInfo.dir = MoveDirection.Backward;
		}
		if (GameApp.GetInstance().GetGameMode().IsVSMode())
		{
			float magnitude = moveDirection.magnitude;
			if (magnitude > 0f)
			{
				moveDirection.x /= moveDirection.magnitude;
				moveDirection.z /= moveDirection.magnitude;
				if (moveDirection.z < 0f)
				{
					moveDirection.z *= 0.9f;
				}
			}
		}
		ThirdPersonStandardCameraScript component = Camera.main.GetComponent<ThirdPersonStandardCameraScript>();
		GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.GetTransform()
			.Rotate(new Vector3(0f, 0f - component.WeaponAngleH, 0f));
		if (GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.GetSkills()
			.HasSkill(SkillsType.JOYSTICK_REVERT))
		{
			moveDirection = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.GetTransform()
				.TransformDirection(-moveDirection);
		}
		else
		{
			moveDirection = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.GetTransform()
				.TransformDirection(moveDirection);
		}
		GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.GetTransform()
			.Rotate(new Vector3(0f, component.WeaponAngleH, 0f));
		if (GameApp.GetInstance().GetGameWorld().State != GameState.SwitchBossLevel)
		{
			moveDirection += Vector3.down * 1.9f;
		}
		inputInfo.moveDirection = moveDirection;
		if (moveDirection.magnitude != 0f)
		{
			LeftJoystickControlTime += Time.deltaTime;
		}
		if (cameraRotation.magnitude != 0f)
		{
			RightJoystickControlTime += Time.deltaTime;
		}
		TotalTime += Time.deltaTime;
		if (flag)
		{
			cameraRotation.x = 0f;
			cameraRotation.y = 0f;
		}
	}

	public bool NeedShowContorlTip()
	{
		if (LeftJoystickControlTime / TotalTime < 0.1f || RightJoystickControlTime / TotalTime < 0.1f)
		{
			return true;
		}
		return false;
	}
}
