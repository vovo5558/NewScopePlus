using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Uk.Org.Adcock.Parallel;
using OpenCvSharp;

[Serializable]
public class Regions
{
	public enum Type
	{
		Button,

		Switch
	}

	public string name = "";
	public bool available = false;
	public Type type = Type.Button;

	public int posX;
	public int posY;
	public int width;
	public int height;

	public string callback = "";

	[HideInInspector]
	public bool lastFlag = false;

	[HideInInspector]
	public bool flag = false;

	[HideInInspector]
	public bool toggle = false;

	[HideInInspector]
	public bool state = false;
}

public class ColorDetectVirtualButton : MonoBehaviour {
	
	// Use this for initialization
	private Texture2D T;
	MeshRenderer mr;
	public int posX;
	public int posY;
	public int width;
	public int height;
	public float threshold;
	public Vector3 detectColor;

	public bool debugDraw = false;
	
	public List<Region> Area;

	public Camera LeftCamera;
	public Transform LEDPosition;
	public Region ledRegion;
	Vector3 position;

	
	void Start () {
		mr = GetComponent<MeshRenderer> ();
		T = new Texture2D (1600, 1200);
		detectColor = new Vector3 (0.404f, 0.133f, 0.205f);
		StartCoroutine("colorDetect");
	}
	
	void Update()
	{
		/*RenderTexture.active = (RenderTexture)mr.material.mainTexture
		T.ReadPixels (new Rect (0, 0, 1600, 1200), 0, 0);
		//T.SetPixels (posX, posY, width, height, new Color[width*height]);
		T.Apply ();
		mr.material.mainTexture = T;*/
		//Debug.Log("HAHA");
		if (Input.GetKey ((KeyCode.E))) {
		//	Debug.Log("YAHA");
			detectColor = devColorCalibration ();
        //    Debug.Log (detectColor);
		}
		position = LeftCamera.WorldToScreenPoint(LEDPosition.position);
		//Debug.Log (position);
		ledRegion.posX = (int)position.x - 20;
		ledRegion.posY = (int)position.y - 20;
		ledRegion.height = 40;
		ledRegion.width = 40;
		//Debug.Log (ledRegion.posX + " " + ledRegion.posY);
	}
	
	Vector3 devColorCalibration()
	{
		Color[] tmp = T.GetPixels (posX, posY, width, height);
		Vector3 total = new Vector3();
    
		foreach (Color i in tmp) {
			total.x += i.r;
			total.y += i.g;
			total.z += i.b;
		}
		total /= tmp.Length;

		Debug.Log (total);
		return total;
	}
    IEnumerator colorDetect()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.4f);
            RenderTexture.active = (RenderTexture)mr.material.mainTexture;
            T.ReadPixels(new Rect(0, 0, 1600, 1200), 0, 0);
            //T.SetPixels (posX, posY, width, height, new Color[width*height]);
            if (debugDraw)
            {
                //T.SetPixels(posX, posY, width, height, new Color[width * height]);
				T.SetPixels(ledRegion.posX, ledRegion.posY, ledRegion.width, ledRegion.height, new Color[ledRegion.width * ledRegion.height]);
            }
            T.Apply();
            mr.material.mainTexture = T;
            //Vector3 total = new Vector3(0, 0, 0);
            //float test = 0.0f;
            //Debug.Log(Area.Count);
            

			if(ledRegion.available)
			{
				Vector3 total = new Vector3();
				Color[] tmp = T.GetPixels(ledRegion.posX, ledRegion.posY, ledRegion.width, ledRegion.height);
				foreach (Color t in tmp) {
					total.x += t.r;
					total.y += t.g;
					total.z += t.b;
				}
				total /= tmp.Length;
				Debug.Log(total);
			}

            foreach (Region area in Area)
            {
                if (!area.available)
                    continue;

                //for(int i = 0; i < Area.Count; i++)
                //Debug.Log(T.GetPixels((int)area.x, (int)area.y, (int)area.z, (int)area.w));

                //Color[] tmp = T.GetPixels (Area[0].posX, Area[0].posY, Area[0].width, Area[0].height);
                //Debug.Log(Area[0].posX);

                Vector3 total = new Vector3();
                //T.Resize(400, 300);
                Color[] tmp = T.GetPixels(area.posX, area.posY, area.width, area.height);
               
                // T.Resize(1600, 1200);
                foreach (Color t in tmp) {
                    total.x += t.r;
                    total.y += t.g;
                    total.z += t.b;
                }

   
                total /= tmp.Length;
                // Debug.Log(total);
                area.flag = false;
                if (Mathf.Abs(detectColor.x - total.x) < threshold
                   && Mathf.Abs(detectColor.y - total.y) < threshold
                   && Mathf.Abs(detectColor.z - total.z) < threshold)
                {
                    Debug.Log("Press Color Button");
                    area.flag = true;

                }

                if ((area.flag != area.lastFlag) && (area.flag))
                    area.toggle = !area.toggle;

                if (area.callback != "")
                {
                    if (area.type == Region.Type.Button)
                    {
                        if (area.state != (area.flag != area.lastFlag) && (area.flag))
                            SendMessage(area.callback, (area.flag != area.lastFlag) && (area.flag));

                        area.state = (area.flag != area.lastFlag) && (area.flag);
                    }
                    else if (area.type == Region.Type.Switch)
                    {
                        if (area.state != area.toggle)
                            SendMessage(area.callback, area.toggle);

                        area.state = area.toggle;
                    }
                }

                area.lastFlag = area.flag;
            }
            //   RenderTexture.ReleaseTemporary
        }
        //Debug.Log (T.GetPixel(0,1199));
        //System.Threading.Thread.Sleep (500);
        //GC.Collect();
    }
	
}