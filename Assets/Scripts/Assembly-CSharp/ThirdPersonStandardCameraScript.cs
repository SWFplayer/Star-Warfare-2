using UnityEngine;

[AddComponentMenu("Camera/ThirdPersonStandardCamera")]
public class ThirdPersonStandardCameraScript : MonoBehaviour
{
	protected const float CAMERA_DISTANCE_WHEN_COLLIDED = 0.4f;

	protected float angelH;

	protected float angelV;

	protected float carrierAngleH;

	protected Vector3 carrierForward = Vector3.zero;

	protected float lastUpdateTime;

	protected float deltaTime;

	protected float tempAngelH;

	protected float tempAngelV;

	protected float smoothRotationTime = 0.08f;

	protected float smoothStartTime;

	protected float smoothDeltaH;

	protected float smoothDeltaV;

	public Vector3 pivotPosition;

	public Vector3 pivotPositionNormal;

	public Vector3 pivotPositionRPG;

	public Vector3 pivotPositionVehicle;

	public Vector3 pivotPositionCannon;

	public Vector3 pivotPositionLongSwordSub = new Vector3(0f, 0.7f, 1.5f);

	public Vector3 pivotPositionLongSwordMain = new Vector3(0f, 2.24f, -1f);

	protected Vector3 pivotOffset;

	public float cameraDistanceWhenIdle = 4f;

	public float cameraDistanceWhenRPG = 6f;

	public float cameraDistanceWhenAimed = 2f;

	public float cameraDistanceWhenKnocked = 4f;

	public float cameraDistanceWhenVehicle = 6f;

	public float cameraDistanceWhenCannonAttack = 4f;

	public float cameraDistanceWhenLongSwordMain = 3f;

	public float cameraDistanceWhenLongSwordSub = 2f;

	public float cameraDistance;

	public float cameraSwingSpeed;

	public float minAngelV;

	public float maxAngelV;

	public float fixedAngelV;

	public bool isAngelVFixed;

	public Transform target;

	public Player mLocalPlayer;

	protected CameraClipObject[] lastTransparentObjList = new CameraClipObject[5];

	protected Vector3 moveTo;

	protected bool behindWall;

	public bool lastInWall;

	public bool Enable;

	public float CAMERA_AIM_FOV = 22f;

	public float CAMERA_NORMAL_FOV = 60f;

	public float CAMERA_RPG_FOV = 80f;

	public float CAMERA_KNOCKED_FOV = 60f;

	public float CAMERA_VEHICLE_FOV = 60f;

	public float CAMERA_CANNON_ATTACK_FOV = 40f;

	public float CAMERA_LONGSWORD_MAIN_FOV = 45f;

	public float CAMERA_LONGSWORD_SUB_FOV = 60f;

	protected float cameraDistanceCollided;

	protected bool mIsCameraCollided;

	public Texture reticle;

	public Texture leftTopReticle;

	public Texture rightTopReticle;

	public Texture leftBottomReticle;

	public Texture rightBottomReticle;

	protected Shader transparentShader;

	protected Shader solidShader;

	protected Shader transparentLightmapShader;

	protected float drx;

	protected float dry;

	protected float winTime = -1f;

	protected Vector2 reticlePosition;

	protected Transform cameraTransform;

	protected GameObject pivot;

	public AudioSource loseAudio;

	protected float weaponAngleH;

	protected float initMinAngleV;

	protected float initMaxAngleV;

	private GameObject directionalLight;

	private float angleDirectionalLight;

	private int angleUpdateSign;

	private float directionalLightOldX;

	private float directionalLightOldZ;

	public float MaxDirectionalLightAngle;

	public float DirectionalLightRotateRate;

	public string DirectionalLightName;

	private Color targetLightColor;

	private bool needChangeDirectionalLight;

	private Color lastLightColor;

	private float DirectionalLightPercent;

	public float vehicleTurnSpeed = 2f;

	public float cannonTurn;

	public float smoothSpeed = 100f;

	private bool enableFly;

	private UserState mUserState;

	private float mMaxHorizontalAngle = -1f;

	private float mInitHorizontalAngle;

	public Transform CameraTransform
	{
		get
		{
			return cameraTransform;
		}
	}

	public Vector2 ReticlePosition
	{
		get
		{
			return reticlePosition;
		}
		set
		{
			reticlePosition = value;
		}
	}

	public float AngelV
	{
		get
		{
			return angelV;
		}
	}

	public float AngelH
	{
		get
		{
			return angelH;
		}
		set
		{
			angelH = value;
		}
	}

	public float TempAngelH
	{
		get
		{
			return tempAngelH;
		}
		set
		{
			tempAngelH = value;
		}
	}

	public float WeaponAngleH
	{
		get
		{
			return weaponAngleH;
		}
	}

	public virtual void Init()
	{
		mLocalPlayer = GameApp.GetInstance().GetGameWorld().GetLocalPlayer();
		target = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
			.GetTransform();

        CursorManager.Instance.LockCursor();

        angelH = target.rotation.eulerAngles.y;
		if (mLocalPlayer.IsBoss)
		{
			cameraDistance = PlayerBossParameter.CameraDistance;
		}
		else
		{
			cameraDistance = cameraDistanceWhenIdle;
		}
		cameraDistanceCollided = cameraDistance;
		base.transform.rotation = Quaternion.Euler(0f - angelV, angelH, 0f);
		
		reticlePosition = new Vector3(Screen.width / 2, Screen.height / 2, 0f);
		if (Application.platform == RuntimePlatform.WindowsPlayer || Screen.width == 960)
		{
		}
		float[] array = new float[32];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = 500f;
		}
		base.GetComponent<Camera>().layerCullDistances = array;
		Enable = true;
		base.GetComponent<Camera>().fieldOfView = CAMERA_NORMAL_FOV;
		pivot = GameObject.CreatePrimitive(PrimitiveType.Cube);
		pivot.GetComponent<Renderer>().enabled = false;
		pivot.GetComponent<Collider>().enabled = false;
		for (int j = 0; j < lastTransparentObjList.Length; j++)
		{
			lastTransparentObjList[j] = new CameraClipObject();
		}
		initMinAngleV = minAngelV;
		initMaxAngleV = maxAngelV;
		SetDirectionalLight();
		enableFly = mLocalPlayer.GetSkills().GetSkill(SkillsType.FLY) > 0f;
		tempAngelH = angelH;
		tempAngelV = angelV;
		smoothStartTime = Time.time + smoothRotationTime;
	}

	public virtual void ResetAngleV()
	{
		minAngelV = initMinAngleV;
		maxAngelV = initMaxAngleV;
	}

	public virtual void CreateScreenBlood(float damage)
	{
	}

	public virtual void ZoomIn(float deltaTime)
	{
		base.GetComponent<Camera>().fov = Mathf.Lerp(base.GetComponent<Camera>().fov, CAMERA_AIM_FOV, deltaTime * (float)Global.CAMERA_ZOOM_SPEED);
		cameraDistance = Mathf.Lerp(cameraDistance, cameraDistanceWhenAimed, deltaTime * (float)Global.CAMERA_ZOOM_SPEED);
		pivotPosition = Vector3.Lerp(pivotPosition, pivotPositionNormal + pivotOffset, deltaTime * (float)Global.CAMERA_ZOOM_SPEED);
	}

	public void SetPivotOffset(Vector3 offset)
	{
		pivotOffset = offset;
	}

	public virtual void ZoomToRPG(float deltaTime)
	{
		base.GetComponent<Camera>().fov = Mathf.Lerp(base.GetComponent<Camera>().fov, CAMERA_RPG_FOV, deltaTime * (float)Global.CAMERA_ZOOM_SPEED);
		cameraDistance = Mathf.Lerp(cameraDistance, cameraDistanceWhenRPG, deltaTime * (float)Global.CAMERA_ZOOM_SPEED);
		pivotPosition = Vector3.Lerp(pivotPosition, pivotPositionRPG + pivotOffset, deltaTime * (float)Global.CAMERA_ZOOM_SPEED);
	}

	public virtual void ZoomToSniper(float deltaTime)
	{
		base.GetComponent<Camera>().fov = Mathf.Lerp(base.GetComponent<Camera>().fov, CAMERA_AIM_FOV * 0.6f, deltaTime * (float)Global.CAMERA_ZOOM_SPEED);
		cameraDistance = Mathf.Lerp(cameraDistance, cameraDistanceWhenAimed * 0.05f, deltaTime * (float)Global.CAMERA_ZOOM_SPEED);
		pivotPosition = Vector3.Lerp(pivotPosition, new Vector3(0.6f, 1.68f, 0f) + pivotOffset, deltaTime * (float)Global.CAMERA_ZOOM_SPEED);
	}

	public virtual void ZoomToVehicle(float deltaTime)
	{
		base.GetComponent<Camera>().fieldOfView = Mathf.Lerp(base.GetComponent<Camera>().fieldOfView, CAMERA_VEHICLE_FOV, deltaTime * Global.CAMERA_ZOOM_SPEED_CARRIER);
		cameraDistance = Mathf.Lerp(cameraDistance, cameraDistanceWhenVehicle, deltaTime * Global.CAMERA_ZOOM_SPEED_CARRIER);
		if (!enableFly)
		{
			pivotPosition = Vector3.Lerp(pivotPosition, pivotPositionVehicle + pivotOffset, deltaTime * Global.CAMERA_ZOOM_SPEED_CARRIER);
		}
		else
		{
			pivotPosition = Vector3.Lerp(pivotPosition, pivotPositionVehicle, deltaTime * Global.CAMERA_ZOOM_SPEED_CARRIER);
		}
	}

	public virtual void ZoomToLongSwordMain(float deltaTime)
	{
		base.GetComponent<Camera>().fieldOfView = Mathf.Lerp(base.GetComponent<Camera>().fieldOfView, CAMERA_LONGSWORD_MAIN_FOV, deltaTime * Global.CAMERA_ZOOM_SPEED_CARRIER);
		cameraDistance = Mathf.Lerp(cameraDistance, cameraDistanceWhenLongSwordMain, deltaTime * Global.CAMERA_ZOOM_SPEED_CARRIER);
		if (!enableFly)
		{
			pivotPosition = Vector3.Lerp(pivotPosition, pivotPositionLongSwordMain + pivotOffset, deltaTime * Global.CAMERA_ZOOM_SPEED_CARRIER);
		}
		else
		{
			pivotPosition = Vector3.Lerp(pivotPosition, pivotPositionLongSwordMain, deltaTime * Global.CAMERA_ZOOM_SPEED_CARRIER);
		}
	}

	public virtual void ZoomToCannonAttack(float deltaTime)
	{
		base.GetComponent<Camera>().fieldOfView = Mathf.Lerp(base.GetComponent<Camera>().fieldOfView, CAMERA_CANNON_ATTACK_FOV, deltaTime * Global.CAMERA_ZOOM_SPEED_CANNON);
		cameraDistance = Mathf.Lerp(cameraDistance, cameraDistanceWhenCannonAttack, deltaTime * Global.CAMERA_ZOOM_SPEED_CANNON);
		if (!enableFly)
		{
			pivotPosition = Vector3.Lerp(pivotPosition, pivotPositionCannon + pivotOffset, deltaTime * Global.CAMERA_ZOOM_SPEED_CANNON);
		}
		else
		{
			pivotPosition = Vector3.Lerp(pivotPosition, pivotPositionCannon, deltaTime * Global.CAMERA_ZOOM_SPEED_CANNON);
		}
	}

	public virtual void ZoomToLongSwordSub(float deltaTime)
	{
		base.GetComponent<Camera>().fieldOfView = Mathf.Lerp(base.GetComponent<Camera>().fieldOfView, CAMERA_LONGSWORD_SUB_FOV, deltaTime * Global.CAMERA_ZOOM_SPEED_CARRIER);
		cameraDistance = Mathf.Lerp(cameraDistance, cameraDistanceWhenLongSwordSub, deltaTime * Global.CAMERA_ZOOM_SPEED_CARRIER);
		if (!enableFly)
		{
			pivotPosition = Vector3.Lerp(pivotPosition, pivotPositionLongSwordSub + pivotOffset, deltaTime * Global.CAMERA_ZOOM_SPEED_CARRIER);
		}
		else
		{
			pivotPosition = Vector3.Lerp(pivotPosition, pivotPositionLongSwordSub, deltaTime * Global.CAMERA_ZOOM_SPEED_CARRIER);
		}
	}

	public virtual void ZoomOut(float deltaTime)
	{
		base.GetComponent<Camera>().fov = Mathf.Lerp(base.GetComponent<Camera>().fov, CAMERA_NORMAL_FOV, deltaTime * (float)Global.CAMERA_ZOOM_SPEED);
		cameraDistance = Mathf.Lerp(cameraDistance, cameraDistanceWhenIdle, deltaTime * (float)Global.CAMERA_ZOOM_SPEED);
		pivotPosition = Vector3.Lerp(pivotPosition, pivotPositionNormal + pivotOffset, deltaTime * (float)Global.CAMERA_ZOOM_SPEED);
	}

	public virtual void ZoomToKnockedView(float deltaTime)
	{
		base.GetComponent<Camera>().fov = Mathf.Lerp(base.GetComponent<Camera>().fov, CAMERA_KNOCKED_FOV, deltaTime * (float)Global.CAMERA_ZOOM_SPEED);
		cameraDistance = Mathf.Lerp(cameraDistance, cameraDistanceWhenKnocked, deltaTime * (float)Global.CAMERA_ZOOM_SPEED);
	}

	private void Awake()
	{
		cameraTransform = Camera.main.transform;
		if (!Global.ENABLE_DEBUG_UI)
		{
			GUILayer component = GetComponent<GUILayer>();
			if (component != null)
			{
				component.enabled = false;
			}
		}
	}

	private void Start()
	{
		solidShader = Shader.Find("iPhone/LightMap");
		transparentShader = Shader.Find("iPhone/AlphaBlend_Color");
		transparentLightmapShader = Shader.Find("iPhone/LightMap_AlphaBlend");
		base.GetComponent<Camera>().nearClipPlane = 10000f;
		mUserState = GameApp.GetInstance().GetUserState();
	}

	private void Update()
	{
		if (target == null && GameApp.GetInstance().GetGameWorld() != null && GameApp.GetInstance().GetGameWorld().GetLocalPlayer() != null)
		{
			target = GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.GetTransform();
		}
		if (target != null && pivot != null)
		{
			pivot.transform.position = target.TransformPoint(pivotPosition);
			pivot.transform.rotation = Quaternion.identity;
			pivot.transform.localScale = Vector3.one * 0.3f;
		}
		UpdateDirectionalLightColor();
	}

    private void LateUpdate()
    {
        if (!Enable || target == null)
        {
            return;
        }
        base.GetComponent<Camera>().nearClipPlane = 0.3f;
        deltaTime = Time.deltaTime;

        if (CursorManager.Instance != null && !CursorManager.Instance.isCursorLocked)
        {

            cannonTurn = 0f;
            tempAngelH = angelH;
            tempAngelV = angelV;
        }
        else
        { 

            cannonTurn = mLocalPlayer.InputController.CameraRotation.x;
            float num = mLocalPlayer.InputController.CameraRotation.x;
            float num2 = mLocalPlayer.InputController.CameraRotation.y;
            if (!mLocalPlayer.IsBoss)
            {
                num *= cameraSwingSpeed;
                num2 *= cameraSwingSpeed;
            }
            if (Time.timeScale != 0f)
            {
                angelH += num;
                angelV += num2;
                UpdateAngelDirectionalLight(num);
                if (mUserState.GetSmoothMoveOn() && (num != 0f || num2 != 0f))
                {
                    smoothStartTime = Time.time;
                    smoothDeltaH = (angelH - tempAngelH) / smoothRotationTime;
                    smoothDeltaV = (angelV - tempAngelV) / smoothRotationTime;
                }
            }
        }

		if (mUserState.GetSmoothMoveOn())
		{
			if (Time.time - smoothStartTime <= smoothRotationTime)
			{
				tempAngelH += smoothDeltaH * deltaTime;
				tempAngelV += smoothDeltaV * deltaTime;
			}
		}
		else
		{
			tempAngelH = angelH;
			tempAngelV = angelV;
		}
		if (isAngelVFixed)
		{
			angelV = fixedAngelV;
			tempAngelV = angelV;
		}
		angelV = Mathf.Clamp(angelV, minAngelV, maxAngelV);
		tempAngelV = Mathf.Clamp(tempAngelV, minAngelV, maxAngelV);
		if (mMaxHorizontalAngle > 0f)
		{
			float num3 = target.parent.rotation.eulerAngles.y + mInitHorizontalAngle - mMaxHorizontalAngle;
			float max = num3 + 2f * mMaxHorizontalAngle;
			angelH = AngleClamp(angelH, num3, max);
			tempAngelH = AngleClamp(tempAngelH, num3, max);
		}
		cameraTransform.rotation = Quaternion.Euler(0f - tempAngelV, tempAngelH, 0f);
		cameraTransform.eulerAngles += mLocalPlayer.CameraVibrateController.CameraRotation;
		float num4 = smoothSpeed;
		if (mLocalPlayer.InPlayingState())
		{
			target.rotation = Quaternion.Euler(0f, tempAngelH, 0f);
		}
		if (mIsCameraCollided)
		{
			Vector3 normalized = cameraTransform.TransformDirection(Vector3.forward).normalized;
			Vector3 vector = target.TransformPoint(pivotPosition) - normalized * (cameraDistance + 1f);
			Vector3 vector2 = target.position + Vector3.up;
			float magnitude = (vector2 - vector).magnitude;
			Ray ray = new Ray(vector, vector2 - vector);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, magnitude, 1 << PhysicsLayer.WALL_AGAINST_PALYER))
			{
				cameraDistanceCollided = Mathf.Lerp(cameraDistanceCollided, 0.4f, deltaTime * (float)Global.CAMERA_ZOOM_SPEED);
			}
			else
			{
				mIsCameraCollided = false;
				cameraDistance = cameraDistanceCollided;
			}
			base.GetComponent<Camera>().useOcclusionCulling = false;
			moveTo = target.TransformPoint(pivotPosition) - normalized * cameraDistanceCollided;
		}
		else
		{
			Vector3 normalized2 = cameraTransform.TransformDirection(Vector3.forward).normalized;
			Vector3 vector3 = target.TransformPoint(pivotPosition) - normalized2 * cameraDistance;
			Vector3 vector4 = target.position + Vector3.up;
			float magnitude2 = (vector4 - vector3).magnitude;
			Ray ray2 = new Ray(vector3, vector4 - vector3);
			RaycastHit hitInfo2;
			if (Physics.Raycast(ray2, out hitInfo2, magnitude2, 1 << PhysicsLayer.WALL_AGAINST_PALYER))
			{
				mIsCameraCollided = true;
				cameraDistanceCollided = cameraDistance;
			}
			ray2 = new Ray(vector4, vector3 - vector4);
			if (Physics.Raycast(ray2, out hitInfo2, magnitude2, 1 << PhysicsLayer.WALL_AGAINST_PALYER))
			{
				base.GetComponent<Camera>().useOcclusionCulling = false;
			}
			else
			{
				base.GetComponent<Camera>().useOcclusionCulling = true;
			}
			moveTo = target.TransformPoint(pivotPosition) - normalized2 * cameraDistance;
		}
		if (mLocalPlayer.mVehicleState == EPlayerVehicleState.CARRIER)
		{
			Vector3 forward = target.forward;
			Vector3 vector5 = Vector3.RotateTowards(carrierForward, forward, vehicleTurnSpeed * Time.deltaTime, 1f);
			target.LookAt(target.position + vector5);
		}
		carrierForward = target.forward;
		cameraTransform.position = Vector3.Lerp(cameraTransform.position, moveTo, num4 * Time.deltaTime);
		if (!mLocalPlayer.InPlayingState() && GameApp.GetInstance().GetGameWorld().State != GameState.SwitchBossLevel)
		{
			minAngelV = -70f;
			maxAngelV = 70f;
			if (winTime == -1f)
			{
				winTime = Time.time;
			}
			float num5 = Time.time - winTime;
			cameraTransform.position = mLocalPlayer.GetTransform().TransformPoint(4f * Mathf.Sin((num5 - 1.7f) * 0.3f), 2.5f, 4f * Mathf.Cos((num5 - 1.7f) * 0.3f));
			cameraTransform.LookAt(mLocalPlayer.GetTransform().position + new Vector3(0f, 1.2f, 0f));
		}
		RotDirectionalLight();
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
		{
			deltaTime = 0f;
		}
	}

	private float AngleClamp(float value, float min, float max)
	{
		float num = Mathf.Abs(Mathf.DeltaAngle(value, min));
		float num2 = Mathf.Abs(Mathf.DeltaAngle(value, max));
		float b = Mathf.Abs(Mathf.DeltaAngle(min, max));
		if (Mathf.Approximately(num + num2, b))
		{
			return value;
		}
		return (!(num <= num2)) ? max : min;
	}

	private void UpdateDirectionalLightColor()
	{
		if (needChangeDirectionalLight)
		{
			DirectionalLightPercent += Time.deltaTime;
			Color color = Color.Lerp(lastLightColor, targetLightColor, DirectionalLightPercent);
			directionalLight.GetComponent<Light>().color = color;
			if (directionalLight.GetComponent<Light>().color.Equals(targetLightColor))
			{
				needChangeDirectionalLight = false;
				DirectionalLightPercent = 0f;
			}
		}
	}

	private void SetDirectionalLight()
	{
		directionalLight = GameObject.Find(DirectionalLightName);
		if (directionalLight != null)
		{
			directionalLight.GetComponent<Light>().enabled = true;
			directionalLightOldX = directionalLight.transform.rotation.eulerAngles.x;
			directionalLightOldZ = directionalLight.transform.rotation.eulerAngles.z;
			directionalLight.transform.rotation = Quaternion.Euler(0f, angelH, 0f);
		}
		else
		{
			Debug.Log("!!!There is no such name as " + DirectionalLightName);
		}
		angleUpdateSign = 1;
	}

	private void RotDirectionalLight()
	{
		if (directionalLight != null)
		{
			if (mLocalPlayer.InPlayingState())
			{
				directionalLight.transform.rotation = Quaternion.Euler(directionalLightOldX, tempAngelH + angleDirectionalLight, directionalLightOldZ);
			}
			else
			{
				directionalLight.transform.rotation = Quaternion.Euler(directionalLightOldX, cameraTransform.rotation.eulerAngles.y, directionalLightOldZ);
			}
		}
	}

	public void ChangeDirectionalLightColor(Color newColor)
	{
		lastLightColor = directionalLight.GetComponent<Light>().color;
		targetLightColor = newColor;
		needChangeDirectionalLight = true;
		DirectionalLightPercent = 0f;
	}

	private void UpdateAngelDirectionalLight(float rx)
	{
		angleDirectionalLight += (float)angleUpdateSign * rx * DirectionalLightRotateRate;
		if (angleDirectionalLight > MaxDirectionalLightAngle)
		{
			angleUpdateSign = -angleUpdateSign;
			angleDirectionalLight = MaxDirectionalLightAngle;
		}
		else if (angleDirectionalLight < 0f - MaxDirectionalLightAngle)
		{
			angleUpdateSign = -angleUpdateSign;
			angleDirectionalLight = 0f - MaxDirectionalLightAngle;
		}
	}

	public void ChangeTarget(Transform newTarget)
	{
		target = newTarget;
		if (mMaxHorizontalAngle > 0f)
		{
			mMaxHorizontalAngle = -1f;
		}
	}

	public void ChangeTarget(Transform newTarget, float maxHorizontalAngle)
	{
		target = newTarget;
		mMaxHorizontalAngle = maxHorizontalAngle;
		mInitHorizontalAngle = target.localRotation.eulerAngles.y;
	}
}
