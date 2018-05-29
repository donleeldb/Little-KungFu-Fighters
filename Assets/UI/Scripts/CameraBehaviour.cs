using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour {

	public float PPU;
	public float PPUscaleMobile;
	public float PPUscaleTablet;

	// Use this for initialization
	void Start () {

		float PPUscale = isTablet() ? PPUscaleTablet : PPUscaleMobile; 
		Camera.main.orthographicSize = (Screen.height / (PPU * PPUscale)) * 0.5f;
	}
	
	// Update is called once per frame
	void Update () {

	}

	bool isTablet() {

		float screenHeightInInch =  Screen.height / Screen.dpi;
		if (screenHeightInInch < 3.1)
		{
			return false;
		}
		else
		{
			// it's tablet
			return true;
		}

	}
}
