using System;
using System.Collections.Generic;
using UnityEngine;

public class CreateRoomUI : UIPanelX, UIHandler
{
	public enum Command
	{
		Confirm = 0,
		Cancel = 1
	}

	private const byte PASSWORD_ON = 59;

	private const byte PASSWORD_OFF = 58;

	private const byte PASSWORD_OFF_BEGIN = 60;

	private const byte PASSWORD_OFF_COUNT = 4;

	public UIStateManager stateMgr;

	private static byte BACKGROUND_BEGIN_IMG = 3;

	private static byte BACKGROUND_COUNT_IMG = 28;

	private UIBlock m_block;

	private UIImage shadowImg;

	private UIImage backgroundImg;

	private UIImage fillBGImg;

	private UIClickButton hostNameBtn;

	private UIClickButton confirmBtn;

	private UIClickButton cancelBtn;

	private static byte[] CONFIRM_NORMAL = new byte[2] { 39, 41 };

	private static byte[] CONFIRM_PRESSED = new byte[2] { 38, 40 };

	private static byte[] CANCEL_NORMAL = new byte[2] { 37, 43 };

	private static byte[] CANCEL_PRESSED = new byte[2] { 36, 42 };

	private static byte[] COMBOBOX_GAMEMODE_VS = new byte[7] { 5, 3, 0, 1, 2, 20, 1 };

	private static byte[] COMBOBOX_COOP_PLAYER_NUM = new byte[7] { 5, 2, 3, 1, 2, 20, 1 };

	private static byte[] COMBOBOX_WIN_CONDITION = new byte[7] { 5, 3, 3, 4, 5, 20, 4 };

	private static byte[] COMBOBOX_WIN_VALUE = new byte[7] { 5, 3, 6, 4, 5, 20, 4 };

	private static byte[] COMBOBOX_GAMEMODE_COOP = new byte[7] { 5, 2, 0, 1, 2, 20, 1 };

	private UIComboBox gameModeCoopCmb;

	private UIComboBox gameModeVSCmb;

	private UIComboBox playerNumCmb;

	private UIComboBox winConditionCmb;

	private UIComboBox winValueCmb;

	private UIImage winPanelBGImg;

	private UIImage winPanelFillImg;

	private static byte WIN_BACKGROUND_BEGIN_IMG = 80;

	private static byte WIN_BACKGROUND_COUNT_IMG = 14;

	private UITextImage balanceImg;

	private UISliderSwitch balanceSwitch;

	private Rect nameRect;

	private UISliderNetStage swapStage;

	private List<UIImage> pageNavImg = new List<UIImage>();

	private UIImage selectStagePageImg;

	private int stageCount;

	public byte stageIdx;

	private UIImage passwordImg;

	private UIImage passwordBGImg;

	private static byte[] PASSWORD_BACKGROUND = new byte[3] { 55, 56, 57 };

	private UISliderNum[] passwordNum;

	private UIImage[] passwordMaskImg;

	private UIImage rankBG;

	private static byte[] RANK_BACKGROUND = new byte[6] { 64, 65, 66, 67, 68, 69 };

	private UIImage rankLowerMaskImg;

	private UIImage rankUpperMaskImg;

	private UISliderNum rankLower;

	private UISliderNum rankUpper;

	private bool bPasswordOn;

	private UISliderTab vsModeTab;

	private Mode gameMode;

	private static byte[,] TAB_GAME_MODE = new byte[2, 2]
	{
		{ 73, 74 },
		{ 75, 76 }
	};

	public CreateRoomUI(UIStateManager stateMgr)
	{
		this.stateMgr = stateMgr;
	}

	public void Create()
	{
	}

	public void ResetUIStage()
	{
	}

	public void ResetPageNav()
	{
		UnitUI unitUI = Res2DManager.GetInstance().vUI[5];
		int num = 0;
		num = ((gameMode == Mode.Boss) ? Global.TOTAL_BOSS_STAGE : ((gameMode != Mode.Survival) ? Global.TOTAL_VS_STAGE : Global.TOTAL_SURVIVAL_STAGE));
		int num2 = 32;
		Rect modulePositionRect = unitUI.GetModulePositionRect(0, 0, 95);
		float num3 = modulePositionRect.y + modulePositionRect.height * 0.5f - (float)((num - 1) * num2) * 0.5f;
		for (int i = 0; i < pageNavImg.Count; i++)
		{
			pageNavImg[i].Rect = new Rect(modulePositionRect.x, num3 - modulePositionRect.height * 0.5f, modulePositionRect.width, modulePositionRect.height);
			num3 += (float)num2;
			if (i < num)
			{
				pageNavImg[i].Visible = true;
			}
			else
			{
				pageNavImg[i].Visible = false;
			}
		}
		selectStagePageImg.Rect = pageNavImg[ConvertToUIIndex(stageIdx)].Rect;
	}

	private byte ConvertToUIIndex(byte curId)
	{
		byte b = curId;
		if (gameMode == Mode.Survival)
		{
			return curId;
		}
		if (gameMode == Mode.Boss)
		{
			return curId;
		}
		return (byte)(curId - Global.TOTAL_SURVIVAL_STAGE - Global.TOTAL_BOSS_STAGE);
	}

	private byte ConvertToStageIndex(byte curId)
	{
		byte b = curId;
		if (gameMode == Mode.Survival)
		{
			return curId;
		}
		if (gameMode == Mode.Boss)
		{
			return curId;
		}
		return (byte)(curId + Global.TOTAL_SURVIVAL_STAGE + Global.TOTAL_BOSS_STAGE);
	}

	public void InitModePanel()
	{
		if (GameApp.GetInstance().GetGameMode().IsCoopMode(gameMode))
		{
			gameModeCoopCmb.Visible = true;
			gameModeCoopCmb.Enable = true;
			playerNumCmb.Visible = true;
			playerNumCmb.Enable = true;
			gameModeVSCmb.Visible = false;
			gameModeVSCmb.Enable = false;
			gameModeVSCmb.SetState(3);
			winConditionCmb.Visible = false;
			winValueCmb.Visible = false;
			balanceSwitch.Visible = false;
			balanceImg.Visible = false;
			return;
		}
		gameModeVSCmb.Visible = true;
		gameModeVSCmb.Enable = true;
		gameModeCoopCmb.Visible = false;
		gameModeCoopCmb.Enable = false;
		gameModeCoopCmb.SetState(3);
		playerNumCmb.Visible = false;
		playerNumCmb.Enable = false;
		playerNumCmb.SetState(3);
		winConditionCmb.Visible = true;
		winValueCmb.Visible = true;
		if (gameMode == Mode.VS_CTF_FFA || gameMode == Mode.VS_CTF_TDM)
		{
			winConditionCmb.SetSelection(0);
			winConditionCmb.Enable = false;
		}
		else
		{
			winConditionCmb.Enable = true;
		}
		if (winConditionCmb.GetSelectIndex() == 0)
		{
			ResetUIWinValueCmb(COMBOBOX_WIN_VALUE, UIConstant.WIN_VALUE_FOR_SCORE);
		}
		else
		{
			ResetUIWinValueCmb(COMBOBOX_WIN_VALUE, UIConstant.WIN_VALUE_FOR_TIMER);
		}
		if (gameMode == Mode.VS_TDM || gameMode == Mode.VS_CTF_TDM)
		{
			balanceSwitch.Visible = true;
			balanceImg.Visible = true;
		}
		else
		{
			balanceSwitch.Visible = false;
			balanceImg.Visible = false;
		}
	}

	public void InitVSMode()
	{
		UnitUI ui = Res2DManager.GetInstance().vUI[5];
		vsModeTab.Clear();
		vsModeTab.SetBackground(ui, 0, 72);
		vsModeTab.SetSlider(ui, 0, 77);
		vsModeTab.SetClipRect(vsModeTab.backgroundImg.Rect.x, vsModeTab.backgroundImg.Rect.y, vsModeTab.backgroundImg.Rect.width, vsModeTab.backgroundImg.Rect.height);
		for (int i = 0; i < UIConstant.GAME_MODE_TOTAL; i++)
		{
			UITab uITab = new UITab();
			uITab.AddObject(UITab.State.Normal, ui, 0, TAB_GAME_MODE[i, 0]);
			uITab.AddObject(UITab.State.Selected, ui, 0, TAB_GAME_MODE[i, 1]);
			uITab.Rect = uITab.GetObjectRect(UITab.State.Normal);
			uITab.Enable = false;
			vsModeTab.Add(uITab);
		}
		vsModeTab.SetScroller(0f, 165f, 165f, vsModeTab.backgroundImg.Rect);
		vsModeTab.m_scroller.DeltaTime = 0.016f;
		if (GameApp.GetInstance().GetGameMode().IsVSMode(gameMode))
		{
			vsModeTab.SetSelection(0);
		}
		else
		{
			vsModeTab.SetSelection(1);
		}
	}

	public void ReviseNetStage()
	{
		int selectedNetIndex = GameApp.GetInstance().GetUserState().GetSelectedNetIndex();
		if (gameMode == Mode.Survival)
		{
			if (selectedNetIndex >= Global.TOTAL_SURVIVAL_STAGE)
			{
				GameApp.GetInstance().GetUserState().SetSelectedNetIndex(0);
			}
		}
		else if (gameMode == Mode.Boss)
		{
			if (selectedNetIndex >= Global.TOTAL_BOSS_STAGE)
			{
				GameApp.GetInstance().GetUserState().SetSelectedNetIndex(0);
			}
		}
		else if (selectedNetIndex < Global.TOTAL_SURVIVAL_STAGE + Global.TOTAL_BOSS_STAGE)
		{
			GameApp.GetInstance().GetUserState().SetSelectedNetIndex((byte)(Global.TOTAL_SURVIVAL_STAGE + Global.TOTAL_BOSS_STAGE));
		}
	}

	public void InitbalanceSwitch()
	{
		UnitUI ui = Res2DManager.GetInstance().vUI[5];
		balanceSwitch.SetBackground(ui, 3, 9);
		balanceSwitch.SetSlider(ui, 3, 8);
		balanceSwitch.SetClipRect(balanceSwitch.backgroundImg.Rect.x + 1f, balanceSwitch.backgroundImg.Rect.y, balanceSwitch.backgroundImg.Rect.width - 2f, balanceSwitch.backgroundImg.Rect.height);
		balanceSwitch.SetScroller(0f, 67f, 67f, balanceSwitch.backgroundImg.Rect);
		balanceSwitch.SetSelection(0);
	}

	public void ResetUIGameSubModeForCoop(byte[] resIndex, string[] str)
	{
		gameModeCoopCmb.Clear();
		UnitUI unitUI = Res2DManager.GetInstance().vUI[5];
		Rect modulePositionRect = unitUI.GetModulePositionRect(0, resIndex[1], resIndex[2]);
		if (modulePositionRect.y - (float)str.Length * modulePositionRect.height < 10f)
		{
			gameModeCoopCmb.SetComboBox(UIComboBox.Dir.UP);
			gameModeCoopCmb.SetClipRect(modulePositionRect.x, modulePositionRect.y + modulePositionRect.height, modulePositionRect.width, modulePositionRect.height * (float)str.Length);
		}
		else
		{
			gameModeCoopCmb.SetClipRect(modulePositionRect.x, modulePositionRect.y - modulePositionRect.height * (float)str.Length, modulePositionRect.width, modulePositionRect.height * (float)str.Length);
		}
		for (int i = 0; i < str.Length; i++)
		{
			UIComboBox.UIItemIcon uIItemIcon = new UIComboBox.UIItemIcon(gameModeCoopCmb);
			uIItemIcon.m_background.AddObject(unitUI, resIndex[1], resIndex[3]);
			uIItemIcon.m_background.Rect = uIItemIcon.m_background.GetObjectRect();
			uIItemIcon.m_text.Set("font2", str[i], UIConstant.fontColor_cyan, uIItemIcon.m_background.Rect.width);
			uIItemIcon.m_text.AlignStyle = UIText.enAlignStyle.CENTER_CENTER;
			uIItemIcon.m_text.Rect = uIItemIcon.m_background.Rect;
			uIItemIcon.SetClip(gameModeVSCmb.m_showRect);
			uIItemIcon.Visible = false;
			uIItemIcon.Enable = false;
			uIItemIcon.m_textOffsetLeftTop = new Vector2(-20f, 0f);
			uIItemIcon.Rect = uIItemIcon.m_background.Rect;
			gameModeCoopCmb.Add(uIItemIcon);
		}
		gameModeCoopCmb.SetSelection(0);
		gameModeCoopCmb.Enable = true;
	}

	public void ResetUIGameSubModeForVS(byte[] resIndex, string[] str)
	{
		gameModeVSCmb.Clear();
		UnitUI unitUI = Res2DManager.GetInstance().vUI[5];
		Rect modulePositionRect = unitUI.GetModulePositionRect(0, resIndex[1], resIndex[2]);
		if (modulePositionRect.y - (float)str.Length * modulePositionRect.height < 10f)
		{
			gameModeVSCmb.SetComboBox(UIComboBox.Dir.UP);
			gameModeVSCmb.SetClipRect(modulePositionRect.x, modulePositionRect.y + modulePositionRect.height, modulePositionRect.width, modulePositionRect.height * (float)str.Length);
		}
		else
		{
			gameModeVSCmb.SetClipRect(modulePositionRect.x, modulePositionRect.y - modulePositionRect.height * (float)str.Length, modulePositionRect.width, modulePositionRect.height * (float)str.Length);
		}
		for (int i = 0; i < str.Length; i++)
		{
			UIComboBox.UIItemIcon uIItemIcon = new UIComboBox.UIItemIcon(gameModeVSCmb);
			uIItemIcon.m_background.AddObject(unitUI, resIndex[1], resIndex[3]);
			uIItemIcon.m_background.Rect = uIItemIcon.m_background.GetObjectRect();
			uIItemIcon.m_text.Set("font2", str[i], UIConstant.fontColor_cyan, uIItemIcon.m_background.Rect.width);
			uIItemIcon.m_text.AlignStyle = UIText.enAlignStyle.CENTER_CENTER;
			uIItemIcon.m_text.Rect = uIItemIcon.m_background.Rect;
			uIItemIcon.SetClip(gameModeVSCmb.m_showRect);
			uIItemIcon.Visible = false;
			uIItemIcon.Enable = false;
			uIItemIcon.m_textOffsetLeftTop = new Vector2(-20f, 0f);
			uIItemIcon.Rect = uIItemIcon.m_background.Rect;
			gameModeVSCmb.Add(uIItemIcon);
		}
		gameModeVSCmb.SetSelection(0);
		gameModeVSCmb.Enable = true;
	}

	public void ResetUIPlayerNum(byte[] resIndex)
	{
		playerNumCmb.Clear();
		UnitUI unitUI = Res2DManager.GetInstance().vUI[5];
		Rect modulePositionRect = unitUI.GetModulePositionRect(0, resIndex[1], resIndex[2]);
		string[] pLAYER_NUM_COOP = UIConstant.PLAYER_NUM_COOP;
		if (modulePositionRect.y - (float)pLAYER_NUM_COOP.Length * modulePositionRect.height < 10f)
		{
			playerNumCmb.SetComboBox(UIComboBox.Dir.UP);
			playerNumCmb.SetClipRect(modulePositionRect.x, modulePositionRect.y + modulePositionRect.height, modulePositionRect.width, modulePositionRect.height * (float)pLAYER_NUM_COOP.Length);
		}
		else
		{
			playerNumCmb.SetClipRect(modulePositionRect.x, modulePositionRect.y - modulePositionRect.height * (float)pLAYER_NUM_COOP.Length, modulePositionRect.width, modulePositionRect.height * (float)pLAYER_NUM_COOP.Length);
		}
		for (int i = 0; i < pLAYER_NUM_COOP.Length; i++)
		{
			UIComboBox.UIItemIcon uIItemIcon = new UIComboBox.UIItemIcon(playerNumCmb);
			uIItemIcon.m_background.AddObject(unitUI, resIndex[1], resIndex[3]);
			uIItemIcon.m_background.Rect = uIItemIcon.m_background.GetObjectRect();
			uIItemIcon.m_text.Set("font2", Convert.ToString(pLAYER_NUM_COOP[i]), UIConstant.fontColor_cyan, uIItemIcon.m_background.Rect.width);
			uIItemIcon.m_text.AlignStyle = UIText.enAlignStyle.CENTER_CENTER;
			uIItemIcon.m_text.Rect = uIItemIcon.m_background.Rect;
			uIItemIcon.SetClip(playerNumCmb.m_showRect);
			uIItemIcon.Visible = false;
			uIItemIcon.Enable = false;
			uIItemIcon.m_textOffsetLeftTop = new Vector2(-20f, 0f);
			uIItemIcon.Rect = uIItemIcon.m_background.Rect;
			playerNumCmb.Add(uIItemIcon);
		}
		playerNumCmb.SetSelection(pLAYER_NUM_COOP.Length - 1);
	}

	public void ResetUIWinConditionCmb(byte[] resIndex, string[] str)
	{
		winConditionCmb.Clear();
		UnitUI unitUI = Res2DManager.GetInstance().vUI[5];
		Rect modulePositionRect = unitUI.GetModulePositionRect(0, resIndex[1], resIndex[2]);
		if (modulePositionRect.y - (float)str.Length * modulePositionRect.height < 10f)
		{
			winConditionCmb.SetComboBox(UIComboBox.Dir.UP);
			winConditionCmb.SetClipRect(modulePositionRect.x, modulePositionRect.y + modulePositionRect.height, modulePositionRect.width, modulePositionRect.height * (float)str.Length);
		}
		else
		{
			winConditionCmb.SetClipRect(modulePositionRect.x, modulePositionRect.y - modulePositionRect.height * (float)str.Length, modulePositionRect.width, modulePositionRect.height * (float)str.Length);
		}
		for (int i = 0; i < str.Length; i++)
		{
			UIComboBox.UIItemIcon uIItemIcon = new UIComboBox.UIItemIcon(winConditionCmb);
			uIItemIcon.m_background.AddObject(unitUI, resIndex[1], resIndex[3]);
			uIItemIcon.m_background.Rect = uIItemIcon.m_background.GetObjectRect();
			uIItemIcon.m_text.Set("font2", str[i], UIConstant.fontColor_cyan, uIItemIcon.m_background.Rect.width);
			uIItemIcon.m_text.AlignStyle = UIText.enAlignStyle.CENTER_CENTER;
			uIItemIcon.m_text.Rect = uIItemIcon.m_background.Rect;
			uIItemIcon.SetClip(winConditionCmb.m_showRect);
			uIItemIcon.Visible = false;
			uIItemIcon.Enable = false;
			uIItemIcon.m_textOffsetLeftTop = new Vector2(-15f, 0f);
			uIItemIcon.Rect = uIItemIcon.m_background.Rect;
			winConditionCmb.Add(uIItemIcon);
		}
		winConditionCmb.SetSelection(0);
	}

	public void ResetUIWinValueCmb(byte[] resIndex, string[] str)
	{
		winValueCmb.Clear();
		UnitUI unitUI = Res2DManager.GetInstance().vUI[5];
		Rect modulePositionRect = unitUI.GetModulePositionRect(0, resIndex[1], resIndex[2]);
		if (modulePositionRect.y - (float)str.Length * modulePositionRect.height < 10f)
		{
			winValueCmb.SetComboBox(UIComboBox.Dir.UP);
			winValueCmb.SetClipRect(modulePositionRect.x, modulePositionRect.y + modulePositionRect.height, modulePositionRect.width, modulePositionRect.height * (float)str.Length);
		}
		else
		{
			winValueCmb.SetClipRect(modulePositionRect.x, modulePositionRect.y - modulePositionRect.height * (float)str.Length, modulePositionRect.width, modulePositionRect.height * (float)str.Length);
		}
		for (int i = 0; i < str.Length; i++)
		{
			UIComboBox.UIItemIcon uIItemIcon = new UIComboBox.UIItemIcon(winValueCmb);
			uIItemIcon.m_background.AddObject(unitUI, resIndex[1], resIndex[3]);
			uIItemIcon.m_background.Rect = uIItemIcon.m_background.GetObjectRect();
			uIItemIcon.m_text.Set("font2", str[i], UIConstant.fontColor_cyan, uIItemIcon.m_background.Rect.width);
			uIItemIcon.m_text.AlignStyle = UIText.enAlignStyle.CENTER_CENTER;
			uIItemIcon.m_text.Rect = uIItemIcon.m_background.Rect;
			uIItemIcon.SetClip(winConditionCmb.m_showRect);
			uIItemIcon.Visible = false;
			uIItemIcon.Enable = false;
			uIItemIcon.m_textOffsetLeftTop = new Vector2(-15f, 0f);
			uIItemIcon.Rect = uIItemIcon.m_background.Rect;
			winValueCmb.Add(uIItemIcon);
		}
		winValueCmb.SetSelection(0);
	}

	public byte GetMaxPlayers()
	{
		byte b = 0;
		if (GameApp.GetInstance().GetGameMode().IsCoopMode(gameMode))
		{
			return (byte)(playerNumCmb.GetSelectIndex() + 1);
		}
		if (gameMode == Mode.VS_TDM)
		{
			return 12;
		}
		return 12;
	}

	public void OnGUI()
	{
		string userName = Lobby.GetInstance().GetUserName();
		MultiMenuScript multiMenuScript = (MultiMenuScript)stateMgr;
		if (multiMenuScript != null)
		{
			multiMenuScript.playerNameStyle.alignment = TextAnchor.MiddleCenter;
			GUI.Label(nameRect, userName, multiMenuScript.playerNameStyle);
		}
	}

	public override void Draw()
	{
		base.Draw();
	}

	public override bool HandleInput(UITouchInner touch)
	{
		if (base.HandleInput(touch))
		{
			return true;
		}
		return false;
	}

	public void ResetPassword(UISliderNum num)
	{
		UnitUI ui = Res2DManager.GetInstance().vUI[5];
		for (int i = 0; i < passwordNum.Length; i++)
		{
			if (bPasswordOn)
			{
				passwordNum[i].m_numIcons[0].m_background.SetTexture(ui, 1, 0);
			}
			else
			{
				passwordNum[i].m_numIcons[0].m_background.SetTexture(ui, 1, 10);
			}
			Rect objectRect = passwordNum[i].m_numIcons[0].m_background.GetObjectRect();
			passwordNum[i].m_numIcons[0].m_background.Rect = new Rect(passwordMaskImg[i].Rect.x + (passwordMaskImg[i].Rect.width - objectRect.width) * 0.5f, passwordMaskImg[i].Rect.y + (passwordMaskImg[i].Rect.height - objectRect.height) * 0.5f, objectRect.width, objectRect.height);
			if (num != passwordNum[i])
			{
				passwordNum[i].SetSelection(0);
			}
		}
		if (bPasswordOn)
		{
			passwordImg.SetTexture(ui, 0, 59);
		}
		else
		{
			passwordImg.SetTexture(ui, 0, 58);
		}
	}

	private short PasswordToShort()
	{
		int num = 0;
		if (bPasswordOn)
		{
			int num2 = 1;
			for (int num3 = passwordNum.Length - 1; num3 >= 0; num3--)
			{
				num += passwordNum[num3].GetSelectedIndex() * num2;
				num2 *= 10;
			}
		}
		else
		{
			num = -1;
		}
		return (short)num;
	}

	public void ResetMap()
	{
		if (gameMode == Mode.Survival)
		{
			stageIdx = 0;
			ResetUIStage();
			ResetPageNav();
		}
		else if (gameMode == Mode.Boss)
		{
			stageIdx = 0;
			ResetUIStage();
			ResetPageNav();
		}
		else if (gameMode == Mode.VS_TDM)
		{
			stageIdx = (byte)(Global.TOTAL_SURVIVAL_STAGE + Global.TOTAL_BOSS_STAGE);
			ResetUIStage();
			ResetPageNav();
		}
		else
		{
			stageIdx = (byte)(Global.TOTAL_SURVIVAL_STAGE + Global.TOTAL_BOSS_STAGE);
			ResetUIStage();
			ResetPageNav();
		}
	}

	public void ProcessCmb(UIControl control)
	{
		if (gameModeCoopCmb.m_state == 0 && control != gameModeCoopCmb)
		{
			gameModeCoopCmb.SetState(2);
		}
		if (gameModeVSCmb.m_state == 0 && control != gameModeVSCmb)
		{
			gameModeVSCmb.SetState(2);
		}
		if (playerNumCmb.m_state == 0 && control != playerNumCmb)
		{
			playerNumCmb.SetState(2);
		}
		if (winConditionCmb.m_state == 0 && control != winConditionCmb)
		{
			winConditionCmb.SetState(2);
		}
		if (winValueCmb.m_state == 0 && control != winValueCmb)
		{
			winValueCmb.SetState(2);
		}
	}

	public Mode GetGameMode(int wparam)
	{
		switch (wparam)
		{
		case 0:
			return Mode.VS_CTF_TDM;
		case 1:
			return Mode.VS_KOTH;
		case 2:
			return Mode.VS_TDM;
		case 3:
			return Mode.VS_FFA;
		default:
			return Mode.VS_KOTH;
		}
	}

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
	}
}
