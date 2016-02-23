using UnityEngine;
using System.Collections;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

public class about_project : MonoBehaviour {
    public GameObject butterfly;
    public Projector projector_l, projector_r;
    public GameObject sp1, sp2, sp3, sp4, sp_l, sp_r; //sp_l,sp_r是main camera的子物件
    GameObject[] sp;
    Vector3[] pos_sp;
    public Camera maincamera;
    Vector3 pos_l, pos_r;
    Texture2D tex,tex_warp;

    public GameObject  Imagetarget;
    public Texture2D tex_ori;

    int count = 0;
    
    bool check = false; //for butterfly render
    Renderer[] renderers;

	// Use this for initialization
	void Start () {
       // butterfly.SetActive(false);
        sp = new GameObject[4];
        pos_sp = new Vector3[4];
        sp[0] = sp1;
        sp[1] = sp2;
        sp[2] = sp3;
        sp[3] = sp4;
        //把定位球的render關掉
       /* for (int i = 0; i < 4; i++)
            sp[i].GetComponent<Renderer>().enabled = false;
        sp_l.GetComponent<Renderer>().enabled = false;
        sp_r.GetComponent<Renderer>().enabled = false;*/
       
       // tex_ori = (Texture2D)projector_l.material.mainTexture;
       // tex_ori = (Texture2D)a.GetComponent<Renderer>().material.mainTexture;

       
	
	}
	
	// Update is called once per frame
    void Update()
    {
            if (Input.GetKeyDown(KeyCode.Space))
            {

                
                Imagetarget.SetActive(true);
;               butterfly.SetActive(false);
                
                //getkey十要做
                //cut down game screen, sl跟sr是左下跟右上
                pos_l = maincamera.WorldToScreenPoint(sp_l.transform.position);
                pos_r = maincamera.WorldToScreenPoint(sp_r.transform.position);
                int width = (int)(pos_r.x - pos_l.x);
                int height = (int)(pos_r.y - pos_l.y);
                tex = new Texture2D(width, height);


                RenderTexture currentActiveRT = RenderTexture.active;
                // Set the supplied RenderTexture as the active one
                RenderTexture.active = maincamera.targetTexture;

                tex.ReadPixels(new UnityEngine.Rect(pos_l.x, pos_l.y, width, height), 0, 0);
                tex.Apply();
                Cv.ShowImage("11",Texture2dToMat(tex));

                RenderTexture.active = currentActiveRT;//條回去

                //設定位球再tex上的xy位置用來warp
                CvMat P_ori = new CvMat(4, 2, MatrixType.F32C1);
                CvMat P_def = new CvMat(4, 2, MatrixType.F32C1);
                print(tex_ori.width);
                print(tex_ori.height);
                float[] p_ori = new float[8] { 0.0f, 0.0f, 0.0f, 512.0f, 512.0f, 512.0f, 512.0f, 0f }; //從原圖標定的定位點 
                for (int i = 0; i < 4; i++)
                {
                    pos_sp[i] = maincamera.WorldToScreenPoint(sp[i].transform.position);
                    P_def.Set2D(i, 0, pos_sp[i].x - pos_l.x);
                    P_def.Set2D(i, 1, pos_r.y - pos_sp[i].y);

                    P_ori.Set2D(i, 0, p_ori[i * 2]);
                    P_ori.Set2D(i, 1, p_ori[i * 2 + 1]);
                }

                if (CheckBoudary(pos_sp, pos_l, pos_r)) //check boundary
                {
                    tex_warp = warp(tex_ori, tex, P_ori, P_def);
                    //Cv.ShowImage("11",tex_warp);
                    projector_l.material.SetTexture("_ShadowTex", tex_warp);
                    projector_r.material.SetTexture("_ShadowTex", tex_warp);
                    butterfly.SetActive(true);
                }
                else
                    print("Part of the butterfly is out of range");
                 
                  butterfly.SetActive(true);
            }

            
            else if (Input.GetKey(KeyCode.LeftAlt))
            {
                butterfly.SetActive(false);
            }
       

    }

    Texture2D warp(Texture2D t_ori, Texture2D t_def, CvMat P_ori, CvMat P_def)
    {
        CvMat H = new CvMat(3, 3, MatrixType.F64C1);
        CvMat warp = new CvMat(t_ori.height, t_ori.width, MatrixType.U8C3);
        CvMat def = Texture2dToMat(t_def);
        Cv.FindHomography(P_def, P_ori, H);
        Cv.WarpPerspective(def, warp, H);
        Cv.ShowImage("111", warp);
        Texture2D warptexture = MatToTexture2d(warp);
        return warptexture;
    
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
    private Texture2D MatToTexture2d(CvMat ma)
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.LoadImage(ma.ToBytes(".jpg"));
        return tex;
    }

    bool CheckBoudary(Vector3[] pos_sp, Vector3 pos_l, Vector3 pos_r)
    {
        bool check = true;
        for (int i = 0; i < 4; i++)
        {
            if (pos_sp[i].x < pos_l.x || pos_sp[i].x > pos_r.x || pos_sp[i].y < pos_l.y || pos_sp[i].y > pos_r.y)
                check = false;
        }
        return check;
    }
}
