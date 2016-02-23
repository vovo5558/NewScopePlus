using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vuforia;
using System;
using Uk.Org.Adcock.Parallel;
using OpenCvSharp;

public class VirtualButtonCircuits : MonoBehaviour,IVirtualButtonEventHandler 
{
	public GameObject _leftup, _rightup, _leftdown, _rightdown;
	public List<GameObject> leftup_object, rightup_object, leftdown_object, rightdown_object;

	void Start () {
			//Search for all children from this ImageTarget with type VirtualButtonBehaviour
		VirtualButtonBehaviour[] vbs = GetComponentsInChildren<VirtualButtonBehaviour> ();
		for (int i = 0; i < vbs.Length; ++i) {
			vbs[i].RegisterEventHandler(this);
		}

		//	movie1 = transform.Find("movie1").gameObject;
		//_Pupa.SetActive(true);
	}

	public void OnButtonPressed(VirtualButtonAbstractBehaviour vb){
		/*	_Ball.SetActive(true);*/
		switch (vb.name) {
		case "leftup":
			/*foreach(GameObject r in leftup_object){
				Color colorStart = r.GetComponent<Renderer>().material.color;
				Color colorEnd = new Color(colorStart.r, colorStart.g, colorStart.b, 0.0f);
				for (float t = 0.0f; t < 1.0f; t += Time.deltaTime) {
					r.GetComponent<Renderer>().material.color = Color.Lerp (colorStart, colorEnd, (t/1.0f));
				}
			}*/
			Debug.Log("leftup");
			_leftup.SetActive (false);
			break;
		case "rightup":
			_rightup.SetActive (false);
			break;
		case "leftdown":
			_leftdown.SetActive (false);
			break;
		case "rightdown":
			_rightdown.SetActive (false);
			break;

		}
		print ("press!!!!!");
	}

	public void OnButtonReleased(VirtualButtonAbstractBehaviour vb){
	switch (vb.name) {
			case "leftup":
		/*	foreach(GameObject r in leftup_object){
				while(r.GetComponent<Renderer>().material.color.a < 1)
					r.GetComponent<Renderer>().material.color += new Color(0f, 0f, 0f, 0.1f);
				
			}*/
			_leftup.SetActive (true);
			break;
			case "rightup":
			_rightup.SetActive (true);
			break;
			case "leftdown":
			_leftdown.SetActive (true);
			break;
			case "rightdown":
			_rightdown.SetActive (true);
			break;
		}
		
		print ("release!!!");
	}
}
