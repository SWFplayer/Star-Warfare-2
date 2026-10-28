using System;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScript : MonoBehaviour
{
	private const int TutorialTexNum = 26;

	public GameObject RightButton;

	public GameObject LeftButton;

	public GameObject ExitButton;

	public GameObject FullScreenButton;

	public GameObject Panel;

	public GameObject barTemplate;

	public UITexture TextureObject;

	public GameObject Grid;

	public GameObject ProgressBar;

	public UIAnchor TextAnchor;

	public GameObject TextTemplate;

	private List<TutorialData> mToDoList = new List<TutorialData>();

	private int currentIndex;

	private GameObject currentTex;

	private static TutorialScript mInstance;

	private string mTutorialType = string.Empty;

	private Texture2D currentTexture;

	private string currentTexturePath = string.Empty;

	private GameObject[] barList = new GameObject[26];

	private float skipTime = 3f;

	private float realDeltaTime;

	private List<GameObject> textList = new List<GameObject>();

	private void Awake()
	{
		mInstance = this;
		float num = (float)Screen.width / 1136f;
		TextureObject.transform.localScale = new Vector3(num, num, 1f);
	}

	private void OnDestroy()
	{
		mInstance = null;
	}

	public static TutorialScript GetInstance()
	{
		return mInstance;
	}

	private void Start()
	{
		currentIndex = 0;
		mToDoList = TutorialManager.GetInstance().GetToDoList();
		UIEventListener uIEventListener = UIEventListener.Get(RightButton);
		uIEventListener.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener2 = UIEventListener.Get(LeftButton);
		uIEventListener2.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener2.onClick, new UIEventListener.VoidDelegate(OnClick));
		UIEventListener uIEventListener3 = UIEventListener.Get(ExitButton);
		uIEventListener3.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener3.onClick, new UIEventListener.VoidDelegate(OnClick));
		if (GameUIManager.GetInstance() == null)
		{
			base.gameObject.AddComponent<GameUIManager>();
		}
		for (int i = 0; i < 26; i++)
		{
			barList[i] = UnityEngine.Object.Instantiate(barTemplate) as GameObject;
			barList[i].transform.parent = Grid.transform;
			barList[i].transform.localPosition = Vector3.zero;
			barList[i].transform.localScale = Vector3.one * 0.7f;
		}
		Grid.GetComponent<UIGrid>().Reposition();
		GetData(currentIndex);
		SetBar(true);
		if (mTutorialType != TutorialTriggerType.FREE_WATCH)
		{
			Grid.SetActive(false);
			LeftButton.SetActive(false);
			ExitButton.SetActive(false);
			UIEventListener uIEventListener4 = UIEventListener.Get(FullScreenButton);
			uIEventListener4.onClick = (UIEventListener.VoidDelegate)Delegate.Combine(uIEventListener4.onClick, new UIEventListener.VoidDelegate(OnClick));
		}
		else
		{
			ProgressBar.SetActive(false);
		}
		base.gameObject.transform.localScale = Vector3.one;
		SetDepedency();
		TutorialManager.GetInstance().SetTutorialWatched(mTutorialType, true);
		GameApp.GetInstance().Save();
	}

	private void Fresh()
	{
		currentIndex = 0;
		mToDoList = TutorialManager.GetInstance().GetToDoList();
		GetData(currentIndex);
	}

	private void GetData(int index)
	{
		if (mToDoList.Count == 0)
		{
			Debug.LogError("Tutorial Data Cannot be read from xls.");
			return;
		}
		TutorialType type = mToDoList[index].type;
		string iD = mToDoList[index].ID;
		GameDataTable gameDataTable = GameDataManager.GetInstance().GetGameDataTable(DataFileName.TUTORIAL, DataSheetName.TUTORIAL_ALL);
		switch (type)
		{
		case TutorialType.DIALOG:
		{
			List<UIDialogData> list = UserStateUI.GetInstance().StoryDialogManager.LoadDialog(iD);
			GameUIManager.GetInstance().LoadStoryDialog(list);
			break;
		}
		case TutorialType.TEXTURE:
		{
			Resources.UnloadAsset(currentTexture);
			currentTexturePath = mToDoList[index].texturePath;
			currentTexture = Resources.Load(currentTexturePath) as Texture2D;
			if (currentTexture != null)
			{
				TextureObject.mainTexture = currentTexture;
			}
			else
			{
				Debug.LogError(currentTexturePath + " deosn't exist!");
			}
			foreach (GameObject text in textList)
			{
				UnityEngine.Object.Destroy(text);
			}
			textList = new List<GameObject>();
			GameDataTable gameDataTable2 = GameDataManager.GetInstance().GetGameDataTable(DataFileName.TUTORIAL, DataSheetName.TUTORIAL_TEXT);
			for (int i = 0; i < gameDataTable2.Count; i++)
			{
				if (gameDataTable2.GetString(i, 0) == iD)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(TextTemplate) as GameObject;
					gameObject.GetComponent<UILabel>().text = gameDataTable2.GetString(i, 1);
					gameObject.GetComponent<UILocalizationX>().enabled = true;
					float x = gameDataTable2.GetFloat(i, 2);
					float num = gameDataTable2.GetFloat(i, 3);
					int width = gameDataTable2.GetInt(i, 4);
					gameObject.transform.parent = TextAnchor.transform;
					gameObject.transform.localPosition = new Vector3(x, 0f - num, 0f);
					gameObject.GetComponent<UILabel>().width = width;
					SetRelativePosition(gameObject.transform);
					textList.Add(gameObject.gameObject);
				}
			}
			break;
		}
		}
		if (mTutorialType != TutorialTriggerType.FREE_WATCH)
		{
			RightButton.SetActive(false);
			realDeltaTime = 0f;
			ProgressBar.transform.localScale = new Vector3(0f, 1f, 1f);
		}
		else
		{
			RightButton.SetActive(true);
		}
	}

	private void SetBar(bool isForward)
	{
		if (isForward)
		{
			barList[mToDoList[currentIndex].index].transform.Find("OnBar").gameObject.SetActive(true);
			if (mToDoList[currentIndex].index > 0)
			{
				barList[mToDoList[currentIndex].index - 1].transform.Find("OnBar").gameObject.SetActive(false);
			}
		}
		else
		{
			barList[mToDoList[currentIndex].index].transform.Find("OnBar").gameObject.SetActive(true);
			if (mToDoList[currentIndex].index < 25)
			{
				barList[mToDoList[currentIndex].index + 1].transform.Find("OnBar").gameObject.SetActive(false);
			}
		}
	}

	private void Update()
	{
		if (!LoadingUIScript.mInstance.gameObject.activeSelf)
		{
			realDeltaTime += RealTime.deltaTime;
		}
		if (realDeltaTime <= 3f)
		{
			ProgressBar.transform.localScale = new Vector3(realDeltaTime / 3f, 1f, 1f);
		}
		if (GameObject.Find("UIStoryDialog(Clone)") != null)
		{
			GameObject.Find("UIStoryDialog(Clone)").transform.position = new Vector3(0f, 0f, -5f);
			RightButton.SetActive(false);
		}
		else if (realDeltaTime >= skipTime && !RightButton.activeSelf)
		{
			RightButton.SetActive(true);
		}
		if (TextureObject.mainTexture == null)
		{
			currentTexture = Resources.Load(currentTexturePath) as Texture2D;
			if (currentTexture != null)
			{
				TextureObject.mainTexture = currentTexture;
			}
			else
			{
				Debug.LogError(currentTexturePath + " deosn't exist!");
			}
		}
	}

	private void GetNext()
	{
		currentIndex++;
		if (currentIndex < mToDoList.Count)
		{
			SetBar(true);
			GetData(currentIndex);
		}
		else
		{
			OnTutorialFinish();
		}
	}

	private void GetLast()
	{
		currentIndex--;
		if (currentIndex >= 0)
		{
			SetBar(false);
			GetData(currentIndex);
		}
		else
		{
			OnTutorialFinish();
		}
	}

	public void SetType(string value)
	{
		mTutorialType = value;
	}

	private void OnClick(GameObject go)
	{
		if (go == RightButton)
		{
			GetNext();
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		}
		if (go == FullScreenButton && RightButton.activeSelf)
		{
			GetNext();
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		}
		else if (go == LeftButton)
		{
			GetLast();
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_OK);
		}
		else if (go == ExitButton)
		{
			OnTutorialFinish();
			AudioManager.GetInstance().PlaySound(AudioName.MENU_BOTTON_CANCEL);
		}
	}

	private void SetDepedency()
	{
		if (mTutorialType == TutorialTriggerType.FIRST_BOSS)
		{
			if (!GameApp.GetInstance().GetUserState().HasLoginOnline())
			{
				TutorialManager.GetInstance().SetTutorialWatched(TutorialTriggerType.REGISTER_TUTORIAL, false);
				LoginPageScript.mTurnToRegister = true;
			}
			TutorialManager.GetInstance().SetTutorialWatched(TutorialTriggerType.VOID_TUTORIAL, false);
		}
		else if (mTutorialType == TutorialTriggerType.VOID_TUTORIAL)
		{
			TutorialManager.GetInstance().SetTutorialWatched(TutorialTriggerType.VOID_COMPOSE, false);
		}
		else if (mTutorialType == TutorialTriggerType.SHOP_TUTORIAL)
		{
			TutorialManager.GetInstance().SetTutorialWatched(TutorialTriggerType.SUIT_SET_TUTORIAL, true);
		}
		else if (mTutorialType == TutorialTriggerType.VOID_COMPOSE)
		{
			GameApp.GetInstance().GetUserState().mReadyToShowRate = true;
		}
	}

	public void OnTutorialFinish()
	{
		TextureObject.mainTexture = null;
		Resources.UnloadAsset(currentTexture);
		if (currentTex != null)
		{
			currentTex.SetActive(false);
		}
		if (mTutorialType == TutorialTriggerType.FIRST_MISSION)
		{
			TutorialManager.GetInstance().Trigger(TutorialTriggerType.ATTACK_TUTORIAL);
			Fresh();
		}
		else if (mTutorialType == TutorialTriggerType.ATTACK_TUTORIAL || mTutorialType == TutorialTriggerType.BOSS_INSTRUCTION)
		{
			Time.timeScale = 1f;
			GameApp.GetInstance().GetUIStateManager().FrGoToPhase(6, true, false, false);
			UnityEngine.Object.Destroy(base.gameObject);
			VirtualCursorScript._HideCursor();
		}
		else if (mTutorialType == TutorialTriggerType.GAME_STORY)
		{
			GameApp.GetInstance().GetUserState().SetSelectedStageIndex(0);
			GameApp.GetInstance().GetUserState().SetSelectedSubStageIndex(1);
			GameApp.GetInstance().GetUserState().SetSelectedDifficultMode(EDifficultMode.NORMAL);
			GameApp.GetInstance().Save();
			GameApp.GetInstance().GetGameMode().TypeOfNetwork = NetworkType.Single;
			GameApp.GetInstance().GetGameMode().ModePlay = Mode.CamPain;
			if (MenuMusicScript.mInstance != null)
			{
				MenuMusicScript.mInstance.Stop();
			}
			string levelName = GameApp.GetInstance().GetUserState().GetLevelConfig(0, 1)
				.levelName;
			MissionSelectUIScript.ShowLoadingPage(EDifficultMode.NORMAL, 0, 1);
			SwVoidDataManager.TryGetRandomDropFromOnline(EDifficultMode.NORMAL, 0, 1, false);
			GameWorld.LoadLevel(levelName, ELevelType.GAME);
		}
		else if (mTutorialType == TutorialTriggerType.FIRST_BOSS)
		{
			UserState userState = GameApp.GetInstance().GetUserState();
			string dialogId = userState.GetSelectedStageIndex() + 1 + "-" + userState.GetSelectedSubStageIndex();
			List<UIDialogData> list = UserStateUI.GetInstance().StoryDialogManager.LoadDialog(dialogId);
			GameUIManager.GetInstance().LoadStoryDialog(list);
			AudioManager.GetInstance().PlaySound(AudioName.MENU_MISSION_DIALOGUE);
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else if (mTutorialType == TutorialTriggerType.VOID_TUTORIAL)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			InterMissionScript.GetInstance().ChangePage(InterMissionPageType.Custom);
		}
		else if (mTutorialType == TutorialTriggerType.SHOP_TUTORIAL)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			InterMissionScript.GetInstance().ChangePage(InterMissionPageType.Shop);
		}
		else if (mTutorialType == TutorialTriggerType.SUIT_SET_TUTORIAL)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			InterMissionScript.GetInstance().ChangePage(InterMissionPageType.Shop);
		}
		else if (mTutorialType == TutorialTriggerType.VOID_COMPOSE)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void SetRelativePosition(Transform textTransfrom)
	{
		float num = (float)Screen.width / 1136f;
		textTransfrom.localScale = new Vector3(num, num, 1f);
		float num2 = ((float)Screen.height - 640f * num) / 2f;
		textTransfrom.localPosition *= num;
		textTransfrom.localPosition += Vector3.down * num2;
	}

	private void OnEnable()
	{
		VirtualCursorScript._ShowCursor();
	}
}
