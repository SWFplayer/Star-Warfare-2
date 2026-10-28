using UnityEngine;

public class SpectrumController : HighlightingController
{
	public float speed = 200f;

	private readonly int period = 1530;

	private float counter;

	protected override void AfterUpdate()
	{
		base.AfterUpdate();
		int x = (int)counter;
		Color color = new Color(GetColorValue(1020, x), GetColorValue(0, x), GetColorValue(510, x), 1f);
		ho.ConstantOnImmediate(color);
		counter += Time.deltaTime * speed;
		counter %= period;
	}

	private float GetColorValue(int offset, int x)
	{
		int num = 0;
		x = (x - offset) % period;
		if (x < 0)
		{
			x += period;
		}
		if (x < 255)
		{
			num = x;
		}
		if (x >= 255 && x < 765)
		{
			num = 255;
		}
		if (x >= 765 && x < 1020)
		{
			num = 1020 - x;
		}
		return (float)num / 255f;
	}
}
