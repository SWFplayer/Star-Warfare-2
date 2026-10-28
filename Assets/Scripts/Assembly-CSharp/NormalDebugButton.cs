using UnityEngine;

public class NormalDebugButton : MonoBehaviour
{
	public enum ENormalShaderFunctionType
	{
		CHANGE_SHADER = 0,
		ADD_LIGHT = 1,
		MINUS_LIGHT = 2,
		ADD_SHINING = 3,
		MINUS_SHINING = 4,
		STOP_ROTATE = 5,
		START_LEVEL = 6,
		START_LEVEL_NEW = 7
	}

	public GameObject model;

	public ENormalShaderFunctionType functionType;

	public NormalDebug normalShaderDebug;

	private void OnClick()
	{
		switch (functionType)
		{
		case ENormalShaderFunctionType.CHANGE_SHADER:
			normalShaderDebug.ChangeAllShader();
			break;
		case ENormalShaderFunctionType.ADD_LIGHT:
			normalShaderDebug.ChangeLightIntensity(0.1f);
			break;
		case ENormalShaderFunctionType.MINUS_LIGHT:
			normalShaderDebug.ChangeLightIntensity(-0.1f);
			break;
		case ENormalShaderFunctionType.MINUS_SHINING:
			normalShaderDebug.ChangeShiningValue(-0.01f);
			break;
		case ENormalShaderFunctionType.ADD_SHINING:
			normalShaderDebug.ChangeShiningValue(0.01f);
			break;
		case ENormalShaderFunctionType.STOP_ROTATE:
			model.GetComponent<RotateScript>().enabled = !model.GetComponent<RotateScript>().enabled;
			break;
		case ENormalShaderFunctionType.START_LEVEL:
			Application.LoadLevel("NormalTest");
			break;
		case ENormalShaderFunctionType.START_LEVEL_NEW:
			GameApp.GetInstance().GetUserState().SetSelectedStageIndex(4);
			GameApp.GetInstance().GetUserState().SetSelectedSubStageIndex(0);
			Application.LoadLevel("NormalTest02");
			break;
		}
	}
}
