using UnityEngine;
using System.Collections;
using Vuforia;

public class VirtualButtonBehavior : MonoBehaviour,IVirtualButtonEventHandler {
	
	private float intervalTime = 0;
	private bool isPressed = false;
	
	// 判断是长按还是短按
	private bool isShortPress = false;
	private bool isLongPress = false;
	
	// Use this for initialization
	void Start () {
		VirtualButtonBehaviour[] vbs = GetComponentsInChildren<VirtualButtonBehaviour> ();
		for (int i =0; i<vbs.Length; i++) {
			vbs[i].RegisterEventHandler(this);
		}
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log ("---->"+gameObject.transform.rotation);
		if (isPressed) {
			intervalTime += Time.deltaTime;
			Debug.Log("press time == " + intervalTime);
		}
	}

	public void OnButtonPressed (VirtualButtonAbstractBehaviour vb){
		switch (vb.VirtualButtonName) {
		case "test":
			isPressed = true;
			intervalTime = 0;
			break;
		default:
			break;
		}
	}
	
	public void OnButtonReleased (VirtualButtonAbstractBehaviour vb){
		isPressed = false;
		if (intervalTime <= 1) {
		}
	}
}