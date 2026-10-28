using System.Collections.Generic;
using UnityEngine;

public class SwVoidOnlineProcess
{
	public static void SwVoidCreateResponse(SwVoidCreateResponseData data)
	{
		CustomizeUIScript.mInstance.StopWaiting();
		if (data != null)
		{
			SwVoidMenuScript.mInstance.OnlineWait.SetActive(false);
			if (data.result == SwErrorCode.SUCCESS)
			{
				UserState userState = GameApp.GetInstance().GetUserState();
				SwVoidSavedata newSwVoid = new SwVoidSavedata(data.swVoid);
				userState.AddNewSwVoid(data.swVoid.id, newSwVoid);
				SwVoidMaterialResponseData[] materialArray = data.materialArray;
				for (int i = 0; i < materialArray.Length; i++)
				{
					SwVoidMaterialResponseData swVoidMaterialResponseData = materialArray[i];
					userState.SetMaterialNumber(swVoidMaterialResponseData.type, swVoidMaterialResponseData.number);
				}
				SwVoidMenuScript.mInstance.SetFocusIndex(userState.swVoidList.Count - 1);
				SwVoidMenuScript.mInstance.Refresh(ESwVoidMenuTabState.VOID_TAB);
				SwVoidMenuScript.mInstance.AutoEquipNewVoid();
				userState.AnalysisManager.ProduceVoid(data.swVoid.type);
			}
			else
			{
				UIMsgBox.instance.ShowErrorCodeMessage(data.result);
			}
		}
		SwVoidMenuScript.mInstance.ReceiveOnlineInfo();
	}

	public static void SwVoidGetDataResponse(SwVoidGetDataResponseData data)
	{
		CustomizeUIScript.mInstance.StopWaiting();
		if (data != null)
		{
			if (data.result == SwErrorCode.SUCCESS)
			{
				UserState userState = GameApp.GetInstance().GetUserState();
				int swVoidIdInUsing = userState.SwVoidIdInUsing;
				userState.ClearAllSwVoidData();
				SwVoidMaterialResponseData[] materialArray = data.materialArray;
				for (int i = 0; i < materialArray.Length; i++)
				{
					SwVoidMaterialResponseData swVoidMaterialResponseData = materialArray[i];
					userState.SetMaterialNumber(swVoidMaterialResponseData.type, swVoidMaterialResponseData.number);
				}
				SwVoidResponseData[] swVoidArray = data.swVoidArray;
				foreach (SwVoidResponseData swVoidResponseData in swVoidArray)
				{
					SwVoidSavedata newSwVoid = new SwVoidSavedata(swVoidResponseData);
					userState.AddNewSwVoid(swVoidResponseData.id, newSwVoid);
				}
				userState.ChangeEquipedSwVoid(swVoidIdInUsing);
				CustomizeUIScript.mInstance.ShowSwVoidMenu();
				userState.swvoidCountLimit = data.swVoidCountLimit;
			}
			else
			{
				Debug.Log(data.result);
				UIMsgBox.instance.ShowErrorCodeMessage(data.result);
			}
		}
		if (SwVoidMenuScript.mInstance != null && SwVoidMenuScript.mInstance.GetLastCenterIcon() != null)
		{
			SwVoidMenuScript.mInstance.GetLastCenterIcon().Refresh();
		}
	}

	public static void SwVoidUpgradeResponse(SwVoidUpgradeResponseData data)
	{
		CustomizeUIScript.mInstance.StopWaiting();
		SwVoidMenuScript.mInstance.ReceiveOnlineInfo();
		if (data == null)
		{
			return;
		}
		if (data.result == SwErrorCode.SUCCESS)
		{
			UserState userState = GameApp.GetInstance().GetUserState();
			SwVoidMaterialResponseData[] materialArray = data.materialArray;
			for (int i = 0; i < materialArray.Length; i++)
			{
				SwVoidMaterialResponseData swVoidMaterialResponseData = materialArray[i];
				userState.SetMaterialNumber(swVoidMaterialResponseData.type, swVoidMaterialResponseData.number);
			}
			SwVoidMenuScript.mInstance.ReceiveUpgradeInfo(data.newAttributeArray);
		}
		else
		{
			UIMsgBox.instance.ShowErrorCodeMessage(data.result);
		}
	}

	public static void SwVoidUpgradeConfirmResopnse(SwVoidUpgradeConfirmResponseData data)
	{
		CustomizeUIScript.mInstance.StopWaiting();
		SwVoidMenuScript.mInstance.ReceiveOnlineInfo();
		if (data != null)
		{
			if (data.result == SwErrorCode.SUCCESS)
			{
				SwVoidMenuScript.mInstance.ReceiveConfirmUpgrade(data.swVoid);
			}
			else
			{
				UIMsgBox.instance.ShowErrorCodeMessage(data.result);
			}
		}
	}

	public static void SwVoidResolveResponse(SwVoidResolveResponseData data)
	{
		CustomizeUIScript.mInstance.StopWaiting();
		SwVoidMenuScript.mInstance.ReceiveOnlineInfo();
		if (data == null)
		{
			return;
		}
		if (data.result == SwErrorCode.SUCCESS)
		{
			UserState userState = GameApp.GetInstance().GetUserState();
			SwVoidMaterialResponseData[] materialArray = data.materialArray;
			for (int i = 0; i < materialArray.Length; i++)
			{
				SwVoidMaterialResponseData swVoidMaterialResponseData = materialArray[i];
				userState.SetMaterialNumber(swVoidMaterialResponseData.type, swVoidMaterialResponseData.number);
				userState.SetMaterialIsNew(swVoidMaterialResponseData.type, true);
			}
			SwVoidMenuScript.mInstance.ReceiveResolveInfo(data.resolvedVoidId);
			SwVoidMenuScript.mInstance.ShowRewardInfo(data.returnMaterials);
		}
		else
		{
			UIMsgBox.instance.ShowErrorCodeMessage(data.result);
		}
	}

	public static void SwVoidGetRandomDropResponse(SwVoidGetRandomDropResponseData data)
	{
		if (data != null)
		{
			if (data.result == SwErrorCode.SUCCESS)
			{
				SwVoidDataManager.GetInstance().SetDropMaterialInfo(data.dropRoundArray, data.dropTypeArray, data.dropBossArray);
			}
			else
			{
				SwVoidDataManager.GetInstance().NoMaterialDrop();
			}
		}
		else
		{
			SwVoidDataManager.GetInstance().NoMaterialDrop();
		}
	}

	public static void SwVoidGetRewardResponse(SwVoidGetRewardResponseData data)
	{
		if (GameApp.GetInstance().GetGameWorld() != null)
		{
			GameApp.GetInstance().GetGameWorld().VoidRewardData = data;
		}
		if (data != null && data.result == SwErrorCode.SUCCESS && data.materialTypeArray != null && data.materialTypeArray.Length != 0)
		{
			short[] materialTypeArray = data.materialTypeArray;
			foreach (short num in materialTypeArray)
			{
				GameApp.GetInstance().GetUserState().AnalysisManager.GetCurrentPVEStageInfo().AddSwVoidMat(num);
				GameApp.GetInstance().GetUserState().SetMaterialIsNew(num, true);
			}
			GameApp.GetInstance().GetUserState().Achievement.AddVoidMat(data.materialTypeArray.Length);
		}
	}

	public static void GetFirstBossStaticMaterialsResponse(SwVoidGetFirstBossStaticMaterialsResponseData data)
	{
		UIMsgBox.instance.CloseMessage();
		if (data != null)
		{
			if (data.result == SwErrorCode.SUCCESS)
			{
				GameApp.GetInstance().GetUserState().SetHasGetFirstBossVoidMaterials(true);
			}
			SwVoidMenuScript.GetSwVoidDataFromServer(true);
		}
	}

	public static void SwVoidAddSlotResponse(SwVoidAddSlotResponseData data)
	{
		CustomizeUIScript.mInstance.StopWaiting();
		if (data != null && data.result == SwErrorCode.SUCCESS)
		{
			GameApp.GetInstance().GetUserState().swvoidCountLimit = data.newSlotCount;
			GameApp.GetInstance().GetUserState().BuyWithMithril(SwVoidDataManager.CostOfBuyNewSlot(data.newSlotCount));
			GameApp.GetInstance().GetUserState().AnalysisManager.mithrilInfo.SwVoidSlotCost += SwVoidDataManager.CostOfBuyNewSlot(data.newSlotCount);
			if (GameApp.GetInstance().IsConnectedToServer())
			{
				SyncMithrilRequest request = new SyncMithrilRequest();
				GameApp.GetInstance().GetNetworkManager().SendRequest(request);
			}
		}
		if (SwVoidMenuScript.mInstance != null)
		{
			SwVoidMenuScript.mInstance.RefreshCountLimit();
		}
	}

	public static void SwVoidCheckServerResponse(SwVoidCheckServerResponseData data)
	{
		if (data != null)
		{
			if (data.result == SwErrorCode.SUCCESS)
			{
				if (IapScript.mInstance != null)
				{
					IapScript.mInstance.VoidServerCheckSuccess();
				}
			}
			else if (IapScript.mInstance != null)
			{
				IapScript.mInstance.IAPFailedInVoidServer(data.result);
			}
		}
		else if (IapScript.mInstance != null)
		{
			IapScript.mInstance.IAPFailed();
		}
	}

	public static void SwVoidShowChestReward(SwVoidIapChestResponseData data)
	{
		if (data == null)
		{
			return;
		}
		if (data.result == SwErrorCode.SUCCESS)
		{
			foreach (KeyValuePair<short, short> material in data.materialList)
			{
				GameApp.GetInstance().GetUserState().SetMaterialIsNew(material.Key, true);
				short materialNumber = GameApp.GetInstance().GetUserState().GetMaterialNumber(material.Key);
				materialNumber += material.Value;
				GameApp.GetInstance().GetUserState().SetMaterialNumber(material.Key, materialNumber);
			}
			if (IapScript.mInstance != null)
			{
				IapScript.mInstance.ShowRewardInfo(data.materialList);
			}
		}
		else
		{
			Debug.Log(data.result);
		}
	}

	public static void NotRegistedInfo()
	{
	}

	public static void NotRegistedMessage()
	{
		UIMsgBox.instance.ShowMessage(CustomizeUIScript.mInstance, LocalizationManager.GetInstance().GetString("LOC_CUSTOMIZE_VOID_ERROR"), 8, 23);
	}
}
