using UnityEngine;

public class FlyBagAnimationScript : MonoBehaviour
{
	public Player player;

	public GameObject[] BagFloat;

	public GameObject[] BagFloatMove;

	protected bool enabledFire;

	protected Timer enableFireTimer = new Timer();

	protected Timer disableFireTimer = new Timer();

	private void Start()
	{
		if (base.GetComponent<Animation>() != null)
		{
			base.GetComponent<Animation>()["front"].wrapMode = WrapMode.Loop;
			base.GetComponent<Animation>()["back"].wrapMode = WrapMode.Loop;
			base.GetComponent<Animation>()["left"].wrapMode = WrapMode.Loop;
			base.GetComponent<Animation>()["right"].wrapMode = WrapMode.Loop;
			base.GetComponent<Animation>()["idle"].wrapMode = WrapMode.Loop;
		}
	}

	private void Update()
	{
		if (player == null || player.inputController == null || !player.InPlayingState())
		{
			return;
		}
		InputInfo inputInfo = player.inputController.inputInfo;
		if (inputInfo.IsMoving())
		{
			if (BagFloat[0].activeSelf)
			{
				GameObject[] bagFloat = BagFloat;
				foreach (GameObject gameObject in bagFloat)
				{
					gameObject.SetActive(false);
				}
			}
			if (!BagFloatMove[0].activeSelf)
			{
				GameObject[] bagFloatMove = BagFloatMove;
				foreach (GameObject gameObject2 in bagFloatMove)
				{
					gameObject2.SetActive(true);
				}
			}
			if (base.GetComponent<Animation>() != null)
			{
				switch (inputInfo.dir)
				{
				case MoveDirection.Forward:
					base.GetComponent<Animation>().CrossFade("front", 1f);
					break;
				case MoveDirection.Backward:
					base.GetComponent<Animation>().CrossFade("back", 1f);
					break;
				case MoveDirection.Left:
					base.GetComponent<Animation>().CrossFade("left", 1f);
					break;
				case MoveDirection.Right:
					base.GetComponent<Animation>().CrossFade("right", 1f);
					break;
				}
			}
			return;
		}
		if (!BagFloat[0].activeSelf)
		{
			GameObject[] bagFloat2 = BagFloat;
			foreach (GameObject gameObject3 in bagFloat2)
			{
				gameObject3.SetActive(true);
			}
		}
		if (BagFloatMove[0].activeSelf)
		{
			GameObject[] bagFloatMove2 = BagFloatMove;
			foreach (GameObject gameObject4 in bagFloatMove2)
			{
				gameObject4.SetActive(false);
			}
		}
		if (base.GetComponent<Animation>() != null)
		{
			base.GetComponent<Animation>().CrossFade("idle", 1f);
		}
	}
}
