using UnityEngine;
using System.Collections;

public class butterflytexture : MonoBehaviour {

	public GameObject butterflyall;
	public GameObject butterfly;
	public GameObject sphere_l;
	public GameObject sphere_r;
    public GameObject bottom;
	//public RenderTexture rt;

	Vector3 pos_l;
	Vector3 pos_r;
	Texture2D tex;
	public Camera camera;

    bool check = false; //for butterfly render
    Renderer[] renderers;
    Animator anima;
    
    // Use this for initialization
	void Start () {

        renderers = butterflyall.GetComponentsInChildren<Renderer>();
        anima = butterflyall.GetComponent<Animator>();
        anima.SetBool("flybool", false);

     
	}
	
	// Update is called once per frame
	void Update () {



        if (Input.GetKeyDown("space"))
        {
            check = true;   //show butterfly
            

             //print ("here");
            //print (rt.width);
            //print (rt.height);

            pos_l = camera.WorldToScreenPoint (sphere_l.transform.position);
            pos_r = camera.WorldToScreenPoint (sphere_r.transform.position);
            print ("x,y");
            print (pos_l.x);
            print (pos_l.y);
            print(pos_l.z);
            print (pos_r.x);
            print (pos_r.y);
            print(pos_r.z);

            
            float texwidth,texheight;
            texwidth = pos_r.x-pos_l.x;
            texheight = pos_r.y-pos_l.y;
            if(texwidth < 0)
                texwidth = -texwidth;
            if(texheight < 0)
                texheight = -texheight;
            print("hi");
            print(texwidth);
            print(texheight);

            Texture2D tex = new Texture2D ((int)(texwidth),(int)(texheight));
            tex.ReadPixels (new Rect (pos_l.x-150, pos_l.y, tex.width, tex.height), 0, 0);
            tex.Apply ();
            bottom.GetComponent<Renderer> ().material.mainTexture = tex;
            bottom.GetComponent<Renderer>().enabled = true;
     



            foreach (Renderer r in renderers)
            {
                r.enabled = true;
            }
            anima.SetBool("flybool", true);
        }
        else
        {
            if (!check)
            {
                foreach (Renderer r in renderers)
                {
                    r.enabled = false;
                }
                bottom.GetComponent<Renderer>().enabled = false;
                sphere_l.GetComponent<Renderer>().enabled = false;
                sphere_r.GetComponent<Renderer>().enabled = false;

            }
        }
       

	}
}
