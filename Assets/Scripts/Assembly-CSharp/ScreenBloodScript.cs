using UnityEngine;

public class ScreenBloodScript : MonoBehaviour
{
	public static ScreenBloodScript mInstance;

	public float scrollSpeed = 1f;

	protected float alpha;

	protected float startTime;

	protected float deltaTime;

	public string alphaPropertyName = "_Alpha";

	protected GameObject blood;

	protected GameObject deadblood;

	protected GameObject flash;

	protected float currentZoom = 1f;

	private bool zoomed;

	private Vector3 initScale = new Vector3(3.5f, 2.4f, 1f);

	private void Awake()
	{
		mInstance = this;
	}

	private void Start()
	{
		alpha = base.GetComponent<Renderer>().material.GetFloat(alphaPropertyName);
		startTime = Time.time;
		blood = GameObject.Find("Screen_Blood");
		deadblood = GameObject.Find("Screen_DeadBlood");
		flash = GameObject.Find("Screen_Flash");
		if (Screen.width != 960 || Screen.height != 640)
		{
			float num = (float)Screen.width / 960f * 3.5f;
			float num2 = (float)Screen.height / 640f * 2.4f;
			initScale = new Vector3(3f * (float)Screen.width / (float)Screen.height, 3f, 1f);
			blood.transform.localScale = initScale;
			deadblood.transform.localScale = initScale;
			flash.transform.localScale = initScale;
			if (Screen.width == 1024 || Screen.width == 2048)
			{
				blood.transform.localPosition = new Vector3(0f, 0f, 25f);
				deadblood.transform.localPosition = new Vector3(0f, 0f, 25f);
				flash.transform.localPosition = new Vector3(0f, 0f, 25f);
			}
		}
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	public void NewBlood(float damage)
	{
		base.GetComponent<Renderer>().enabled = true;
		alpha = damage;
		alpha = Mathf.Clamp(alpha, 0f, 1f);
	}

	private void Update()
	{
		deltaTime += Time.deltaTime;
		if (!(deltaTime < 0.03f))
		{
			if (alpha > 0f)
			{
				alpha -= 0.5f * deltaTime;
				base.GetComponent<Renderer>().material.SetFloat(alphaPropertyName, alpha);
				Vector3 localScale = initScale * (Camera.main.fieldOfView / 60f * 1.09f);
				localScale.z = 1f;
				base.transform.localScale = localScale;
			}
			else
			{
				base.GetComponent<Renderer>().enabled = false;
			}
			if (deadblood.GetComponent<Renderer>().enabled)
			{
				Vector3 localScale2 = initScale * (Camera.main.fieldOfView / 60f * 1.09f);
				localScale2.z = 1f;
				deadblood.transform.localScale = localScale2;
			}
			if (flash.GetComponent<Renderer>().enabled)
			{
				Vector3 localScale3 = initScale * (Camera.main.fieldOfView / 60f * 1.09f);
				localScale3.z = 1f;
				flash.transform.localScale = localScale3;
			}
			deltaTime = 0f;
		}
	}
}
