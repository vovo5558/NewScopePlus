using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vuforia;

public class VirtualButton : MonoBehaviour, IVirtualButtonEventHandler 
{
	public GameObject _WingsInfo;
	public GameObject _HeadInfo;
	public GameObject _Pupa;
	//public GameObject _Ball;
	//public GameObject movie1;
	//public int count=1;
	//public bool movietag = false;
	//private MovieTexture movie;

	// Use this for initialization
	void Start () {
		//Search for all children from this ImageTarget with type VirtualButtonBehaviour
		VirtualButtonBehaviour[] vbs = GetComponentsInChildren<VirtualButtonBehaviour> ();
		for (int i = 0; i < vbs.Length; ++i) {
			vbs[i].RegisterEventHandler(this);
		}
		//_Ball = transform.FindChild ("Ball").gameObject;
	//	_Ball.SetActive (false);
	//	movie1 = transform.Find("movie1").gameObject;
		//_Pupa.SetActive(true);
	}
	public void OnButtonPressed(VirtualButtonAbstractBehaviour vb){
	/*	_Ball.SetActive(true);*/
		switch (vb.name) {
		case "Wings":
			_WingsInfo.SetActive (true);
		break;
		case "Head":
			_HeadInfo.SetActive (true);
			break;
		case "Pupa":
			_Pupa.SetActive(true);
			_Pupa.GetComponent<MovieController> ().count++;
			_Pupa.GetComponent<MovieController>().movieTag = true;
		break;
		}
	
		print ("press!!!!!");
	}
	public void OnButtonReleased(VirtualButtonAbstractBehaviour vb){
	/*		_Ball.SetActive (false);
	 * */
		switch (vb.name) {
		case "Wings":
			_WingsInfo.SetActive (false);
		break;
		case "Head":
			_HeadInfo.SetActive (false);
		break;
		case "Pupa":
			_Pupa.SetActive(false);
			//movie1.GetComponent<MovieController> ().count++;
			_Pupa.GetComponent<MovieController>().movieTag = false;
			break;
		}

		print ("release!!!");
	}
}
