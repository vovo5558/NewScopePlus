using UnityEngine;
using System.Collections;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
public class tes : MonoBehaviour {
    public GameObject a;
    public Material c,newma;
    public Texture2D b;
	// Use this for initialization
    void Start()
    {
       // Cv.ShowImage("11", Texture2dToMat(b));
        //Cv.ShowImage("112", Texture2dToMat((Texture2D)c.mainTexture));
       // c.mainTexture = b;
        //b = (Texture2D)c.mainTexture;
        //b.Apply();
        //a.GetComponent<Renderer>().material.mainTexture = b;
        
       // a.GetComponent<Projector>().material.mainTexture = b;

        //newma.SetTexture("_ShadowTex", b);
        a.GetComponent<Projector>().material.SetTexture("_ShadowTex", b);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private CvMat Texture2dToMat(Texture2D tex)
    {
        CvMat a = new CvMat(tex.height, tex.width, MatrixType.U8C3);
        var cols = tex.GetPixels();

        for (int i = 0; i < tex.height; i++)
        {
            for (int j = 0; j < tex.width; j++)
            {
                CvColor cvcol;
                Color col;
                col = cols[j + i * tex.width];
                cvcol.R = (byte)(col.r * 255);
                cvcol.G = (byte)(col.g * 255);
                cvcol.B = (byte)(col.b * 255);
                a.Set2D(tex.height - 1 - i, j, cvcol);
            }
        }
        return a;
    }
}
