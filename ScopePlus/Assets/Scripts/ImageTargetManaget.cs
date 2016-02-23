using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ImageTargetManaget : MonoBehaviour {


    public List<GameObject> imageTarget;

	// Use this for initialization
	void Start () {
        Invoke("setActiveAllImageTarget", 3.0f);
        //setActiveAllImageTarget(false);
	}
	
	// Update is called once per frame
	/*void Update () {
	
	}*/

    public void setActiveAllImageTarget()
    {
        for (int i = 0; i < imageTarget.Count; ++i)
        {
            imageTarget[i].SetActive(false);
        }
    }
}
