using System;
using UnityEngine;

public class MissionActionCamera : MissionAction
{
	private bool mIsChangeCamera;

	private int mAlterCameraId;

	private bool mHasAnimation;

	private float mTotalTime;

	private bool mIsViberate;

	private CameraVibrateConfig mConfig;

	private MissionActionCameraScript mCameraScript;

	public override void LoadConfig()
	{
		GameDataTable dataTable = GetDataTable();
		for (int i = 0; i < dataTable.Count; i++)
		{
			int num = dataTable.GetInt(i, 0);
			if (num != base.ID)
			{
				continue;
			}
			mIsChangeCamera = dataTable.GetBoolean(i, 1);
			mAlterCameraId = dataTable.GetInt(i, 2);
			mHasAnimation = dataTable.GetBoolean(i, 3);
			mTotalTime = dataTable.GetFloat(i, 4);
			mIsViberate = dataTable.GetBoolean(i, 5);
			if (mIsViberate)
			{
				mConfig = new CameraVibrateConfig();
				mConfig.TotalTime = dataTable.GetFloat(i, 6);
				mConfig.OutRatio = dataTable.GetFloat(i, 7);
				string text = dataTable.GetString(i, 8);
				mConfig.Type = CameraVibrateType.NORMAL;
				if (Enum.IsDefined(typeof(CameraVibrateType), text))
				{
					mConfig.Type = (CameraVibrateType)(int)Enum.Parse(typeof(CameraVibrateType), text, true);
				}
				else
				{
					Debug.LogError("[INVALID CAMERA VIBRATE TYPE] typeString = " + text);
				}
				float x = dataTable.GetFloat(i, 9);
				float y = dataTable.GetFloat(i, 10);
				float z = dataTable.GetFloat(i, 11);
				mConfig.TargetOffset = new Vector3(x, y, z);
				mConfig.ChangeOffsetTime = dataTable.GetFloat(i, 12);
				mConfig.ChangeOffsetFactor = dataTable.GetFloat(i, 13);
			}
			break;
		}
	}

	public override void LevelStart()
	{
		if (!mIsChangeCamera)
		{
			return;
		}
		string text = MissionNamePrefix.ACTION_CAMERA + mAlterCameraId;
		Debug.Log("cameraName = " + text);
		bool flag = false;
		GameObject[] array = GameObject.FindGameObjectsWithTag(TagName.MISSION_ACTION_CAMERA);
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			Debug.Log("c.name = " + gameObject.name);
			if (text == gameObject.name)
			{
				flag = true;
				mCameraScript = gameObject.GetComponent<MissionActionCameraScript>();
				break;
			}
		}
		if (!flag)
		{
			Debug.LogError("[NO CAMERA FOUND MISSION_ACTION_CAMERA] ActionID = " + base.ID + ", mAlterCameraId = " + mAlterCameraId);
		}
		else if (mCameraScript != null)
		{
			mCameraScript.TotalTime = mTotalTime;
			mCameraScript.HasAnimation = mHasAnimation;
		}
		else
		{
			Debug.LogError("[NO COMPONENT MissionActionCameraScript] ActionID = " + base.ID + ", mAlterCameraId = " + mAlterCameraId);
		}
	}

	public override void StartAction()
	{
		Debug.Log("MissionActionCamera.StartAction");
		if (mIsChangeCamera && mCameraScript != null)
		{
			mCameraScript.StartAction();
		}
		if (mIsViberate)
		{
			GameApp.GetInstance().GetGameWorld().GetLocalPlayer()
				.CameraVibrateController.Vibrate(mConfig);
		}
	}
}
