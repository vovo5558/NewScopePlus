using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vuforia;

public class VirtualButtonCircuit : MonoBehaviour, IVirtualButtonEventHandler 
{
	public GameObject _leftup, _rightup, _leftdown, _rightdown, _rightmid;

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
		case "rightmid":
			_rightup.SetActive(true);
			break;
		}
		
		print ("press!!!!!");
	}
	public void OnButtonReleased(VirtualButtonAbstractBehaviour vb){
	switch (vb.name) {
			case "leftup":
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
			case "rightmid":
			_rightup.SetActive(true);
			break;
		}
		
		print ("release!!!");
	}
}
