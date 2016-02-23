using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;
using System;

public class MenuAction : MonoBehaviour {

	public Animator anim;
    public List<GameObject> imageTarget;
	//public GameObject ledball;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	/*	if (Input.GetMouseButtonDown (0)) {
			string myPath = "E://fb-func//screenshot";
			DirectoryInfo dir = new DirectoryInfo(myPath);
			FileInfo[] info = dir.GetFiles("*.jpg");
			Application.CaptureScreenshot ("E://fb-func//screenshot//screenshot-"+ (info.Length + 1) + ".jpg");
			Process pyProcess = new Process ();
			pyProcess.StartInfo.FileName = @"E:\fb-func\fb2.py"; // change the path (and make sure have installed Python)
			pyProcess.Start ();
		}*/
	}

	public void onMainButton(bool value)
	{
        UnityEngine.Debug.Log("MainButton: " + value);

		//if(anim.GetBool("showMenu"))
		GetComponent<ColorDetect> ().Area [1].available = value;
	/*	GetComponent<ColorDetect> ().Area [2].available = value;
		GetComponent<ColorDetect> ().Area [3].available = value;
		GetComponent<ColorDetect> ().Area [4].available = value;

		if(value)
			GameObject.Find ("ImageTarget").GetComponent<ImageTargetManager> ().setActiveAllVirtualButtons (false);*/
        for (int i = 0; i < imageTarget.Count; ++i)
        {
            imageTarget[i].SetActive(false);
        }
		anim.SetBool("showMenu", value);
	}

	public void onInformation(bool value)
	{
       // UnityEngine.Debug.Log("Information: " + value);
        for (int i = 0; i < imageTarget.Count; ++i)
        {
            imageTarget[i].SetActive(true);
        }
		anim.SetBool ("showMenu", false);
		GetComponent<ColorDetect> ().Area [1].available = false;
		//GetComponent<ColorDetect> ().Area [0].toggle = false;
			//value = true;

	}



	public void onShare(bool value)
	{
	/*	if (value == true) {
			string myPath = "D://fb-func//screenshot";
			DirectoryInfo dir = new DirectoryInfo (myPath);
			FileInfo[] info = dir.GetFiles ("*.jpg");
			Application.CaptureScreenshot ("D://fb-func//screenshot//screenshot-" + (info.Length + 1) + ".jpg");
			Process pyProcess = new Process ();
			pyProcess.StartInfo.FileName = @"D:\fb-func\fb2.py"; // change the path (and make sure have installed Python)
			pyProcess.Start ();
		}
		//GameObject.Find ("ImageTarget").GetComponent<ImageTargetManager> ().setActiveAllVirtualButtons (value);
		if (value == false)
			return;*/
		//Debug.LogError("ccc" + value);
	}

	/*public void onLED(bool value)
	{
		if (value == true)
			ledball.SetActive (true);
		else if (value == false)
			ledball.SetActive (false);
	}*/
}
