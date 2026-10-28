using System.Collections.Generic;
using UnityEngine;

public class NetStageChoiseUI : UIPanelX, UIHandler
{
	public enum Command
	{
		Click = 0
	}

	private UIImage shadowImg;

	private UIImage stageChoiseImg;

	private UIClickButton confirmBtn;

	private UISliderStage swapStage;

	private UIBlock m_block;

	private List<UIImage> pageNavImg = new List<UIImage>();

	private UIImage selectStagePageImg;

	public byte stageIdx;

	private static byte[] CONFIRM_NORMAL = new byte[2] { 26, 30 };

	private static byte[] CONFIRM_PRESSED = new byte[2] { 24, 29 };

	private Mode gameMode;

	private int stageCount;

	public void Create()
	{
	}

	public void Init()
	{
		ResetUIStage();
		ResetPageNav();
	}

	public void SetNetStage(Mode mode, byte stageIdx)
	{
		gameMode = mode;
		this.stageIdx = stageIdx;
	}

	public void ResetUIStage()
	{
	}

	public void ResetPageNav()
	{
		UnitUI unitUI = Res2DManager.GetInstance().vUI[3];
		foreach (UIImage item in pageNavImg)
		{
			Remove(item);
		}
		pageNavImg.Clear();
		Remove(selectStagePageImg);
		int num = 0;
		num = ((gameMode == Mode.Boss) ? Global.TOTAL_BOSS_STAGE : ((gameMode != Mode.Survival) ? Global.TOTAL_VS_STAGE : Global.TOTAL_SURVIVAL_STAGE));
		int num2 = 32;
		Rect modulePositionRect = unitUI.GetModulePositionRect(0, 0, 20);
		float num3 = modulePositionRect.x + modulePositionRect.width * 0.5f - (float)((num - 1) * num2) * 0.5f;
		for (int i = 0; i < num; i++)
		{
			UIImage uIImage = new UIImage();
			uIImage.AddObject(unitUI, 0, 20);
			uIImage.Rect = new Rect(num3 - modulePositionRect.width * 0.5f, modulePositionRect.y, modulePositionRect.width, modulePositionRect.height);
			num3 += (float)num2;
			pageNavImg.Add(uIImage);
			Add(uIImage);
		}
		selectStagePageImg.Rect = pageNavImg[ConvertToUIIndex(stageIdx)].Rect;
		Add(selectStagePageImg);
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

	public void HandleEvent(UIControl control, int command, float wparam, float lparam)
	{
	}
}
