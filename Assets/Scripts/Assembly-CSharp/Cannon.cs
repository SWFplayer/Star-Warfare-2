using UnityEngine;

public class Cannon : Vehicle
{
	protected Shader oldShader;

	protected virtual Transform GetRotateTransform()
	{
		return null;
	}

	public override void GetOn()
	{
		base.GetOn();
		rider.mVehicleState = EPlayerVehicleState.CANNON;
	}

	public override void GetOff()
	{
		base.GetOff();
		if (rider.IsLocal())
		{
			ShowHead();
		}
	}

	public override void DeadOff()
	{
		base.DeadOff();
		if (rider.IsLocal())
		{
			ShowHead();
		}
	}

	public override void ShowHead()
	{
		if (oldShader == null)
		{
			oldShader = Shader.Find("StarWarfare/SNDM_new");
		}
		rider.GetHeadObj().GetComponent<Renderer>().material.shader = oldShader;
		Material[] materials = rider.GetBodyObj().GetComponent<Renderer>().materials;
		Material[] array = materials;
		foreach (Material material in array)
		{
			material.shader = oldShader;
		}
		rider.GetHandObj().GetComponent<Renderer>().material.shader = oldShader;
		rider.GetBagObj().SetActive(true);
		rider.GetHeadObj().GetComponent<Renderer>().enabled = true;
		rider.GetBodyObj().GetComponent<Renderer>().enabled = true;
		rider.GetHandObj().GetComponent<Renderer>().enabled = true;
		oldShader = null;
	}

	public override void HideHead(float alpha)
	{
		if (alpha == 0f)
		{
			rider.GetHeadObj().GetComponent<Renderer>().enabled = false;
			rider.GetBodyObj().GetComponent<Renderer>().enabled = false;
			rider.GetHandObj().GetComponent<Renderer>().enabled = false;
			rider.GetBagObj().SetActive(false);
		}
		if (rider.GetHeadObj().GetComponent<Renderer>().material.shader != Shader.Find("StarWarfare/SNDM_new_Alpha"))
		{
			oldShader = rider.GetHeadObj().GetComponent<Renderer>().material.shader;
		}
		rider.GetHeadObj().GetComponent<Renderer>().material.shader = Shader.Find("StarWarfare/SNDM_new_Alpha");
		rider.GetHeadObj().GetComponent<Renderer>().material.SetFloat("_Alpha", alpha);
		Material[] materials = rider.GetBodyObj().GetComponent<Renderer>().materials;
		Material[] array = materials;
		foreach (Material material in array)
		{
			material.shader = Shader.Find("StarWarfare/SNDM_new_Alpha");
			material.SetFloat("_Alpha", alpha);
		}
		rider.GetHandObj().GetComponent<Renderer>().material.shader = Shader.Find("StarWarfare/SNDM_new_Alpha");
		rider.GetHandObj().GetComponent<Renderer>().material.SetFloat("_Alpha", alpha);
		rider.GetBagObj().SetActive(false);
	}
}
