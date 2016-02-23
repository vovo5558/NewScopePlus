using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour {

    public Projector a;
    public Texture2D b;
	// Use this for initialization
	void Start () {
        a.material.mainTexture = b;


	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
