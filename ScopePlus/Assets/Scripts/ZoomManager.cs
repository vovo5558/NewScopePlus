using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ZoomManager : MonoBehaviour {

	public Transform TranslateLeft;
	public Transform TranslateRight;
	public Transform ScaleLeft;
	public Transform ScaleRight;
	public Transform VirtualObjectDisparity;
	// Use this for initialization
	void Start () {
	
	}

	//public float CurrentZoom;
	
	// Update is called once per frame
	void Update () {


		if (Input.GetKey(KeyCode.Z)) 
		{
			ScaleLeft.localScale += new Vector3(0.04f, 0.04f, 0.04f);
			ScaleRight.localScale += new Vector3(0.04f, 0.04f, 0.04f);
			//leftCamera
		}

		if (Input.GetKey(KeyCode.X)) 
		{
			ScaleLeft.localScale -= new Vector3(0.04f, 0.04f, 0.04f);
			ScaleRight.localScale -= new Vector3(0.04f, 0.04f, 0.04f);
			//leftCamera
		}

		if (Input.GetKey (KeyCode.C)) 
		{
			TranslateLeft.localPosition += new Vector3(0.04f, 0.0f, 0.0f);
			TranslateRight.localPosition -= new Vector3(0.04f, 0.0f, 0.0f);
			//leftCamera
		}
		
		if (Input.GetKey (KeyCode.V)) 
		{
			TranslateLeft.localPosition -= new Vector3(0.04f, 0.0f, 0.0f);
			TranslateRight.localPosition += new Vector3(0.04f, 0.0f, 0.0f);
			//leftCamera
		}

		if (Input.GetKey (KeyCode.A)) 
		{
			VirtualObjectDisparity.localPosition += new Vector3(0.0f, 0.0f, 5.0f);
			//VirtualObjectIPD
		}
		
		if (Input.GetKey (KeyCode.D)) 
		{
			VirtualObjectDisparity.localPosition -= new Vector3(0.0f, 0.0f, 5.0f);
			//VirtualObjectIPD
		}
	}
}
