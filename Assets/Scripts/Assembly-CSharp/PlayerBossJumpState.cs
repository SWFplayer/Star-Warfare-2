using UnityEngine;

internal class PlayerBossJumpState : PlayerBossState
{
	private class MyData : GameUnitStateData
	{
		public Vector3 UpSpeed;

		public float Gravity;

		public float ShadowHeight;
	}

	public override void Start(GameUnit gameUnit)
	{
		Player player = (Player)gameUnit;
		if (!gameUnit.StateData.ContainsKey(this))
		{
			gameUnit.StateData.Add(this, new MyData());
		}
		MyData myData = gameUnit.StateData[this] as MyData;
		player.PlayAnimation(AnimationString.ENEMY_JUMP, WrapMode.ClampForever);
		float num = player.GetObject().GetComponent<Animation>()[AnimationString.ENEMY_JUMP].length - player.JumpTime.x;
		myData.Gravity = -8f * PlayerBossParameter.BossJumpHeight / (num * num);
		float y = (0f - myData.Gravity) * 0.5f * num;
		myData.UpSpeed = new Vector3(0f, y, 0f);
		myData.ShadowHeight = player.Shadow.transform.position.y;
	}

	public override void Update(GameUnit gameUnit)
	{
		Player player = (Player)gameUnit;
		InputController inputController = player.inputController;
		MyData myData = gameUnit.StateData[this] as MyData;
		if (player.IsLocal() && player.CurrentAnimTime >= player.JumpTime.x)
		{
			Vector3 moveDirection = inputController.inputInfo.moveDirection;
			moveDirection.y = 0f;
			moveDirection = moveDirection.normalized;
			moveDirection *= player.JumpHorizontalSpeed;
			Vector3 upSpeed = myData.UpSpeed;
			myData.UpSpeed.y += myData.Gravity * Time.deltaTime;
			upSpeed += myData.UpSpeed;
			upSpeed /= 2f;
			moveDirection += upSpeed;
			player.SimpleMove(moveDirection);
		}
		Vector3 position = player.Shadow.transform.position;
		position.y = myData.ShadowHeight;
		player.Shadow.transform.position = position;
		if (player.Shadow.transform.localPosition.y > player.ShadowHeight)
		{
			Vector3 localPosition = player.Shadow.transform.localPosition;
			localPosition.y = player.ShadowHeight;
			player.Shadow.transform.localPosition = localPosition;
		}
		if (player.IsOnGround())
		{
			StartIdleWhenAnimFinish(player);
			Vector3 localPosition2 = player.Shadow.transform.localPosition;
			localPosition2.y = player.ShadowHeight;
			player.Shadow.transform.localPosition = localPosition2;
		}
	}
}
