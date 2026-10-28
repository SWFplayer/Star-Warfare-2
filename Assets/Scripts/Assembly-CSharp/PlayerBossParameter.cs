using UnityEngine;

public class PlayerBossParameter
{
	public static float CameraDistance = 2.4f;

	public static Vector3[] CameraPivotPosition = new Vector3[4]
	{
		new Vector3(0f, 7.6f, -2.9f),
		new Vector3(0f, 7.6f, -2.9f),
		new Vector3(0f, 11.5f, -3.5f),
		new Vector3(0f, 10.6f, -2.9f)
	};

	public static float BossRotateSpeed = 300f;

	public static float BossJumpHeight = 5f;

	public static float[] BossMoveSpeed = new float[5] { 15f, 15f, 15f, 15f, 15f };

	public static Vector2[] BossJumpTime = new Vector2[4]
	{
		new Vector2(45f / 255f, 21f / 34f),
		new Vector2(2f / 9f, 22f / 27f),
		new Vector2(2f / 9f, 22f / 27f),
		new Vector2(2f / 9f, 22f / 27f)
	};

	public static float[] BossJumpHorizontalSpeed = new float[5] { 15f, 13f, 13f, 13f, 5f };
}
