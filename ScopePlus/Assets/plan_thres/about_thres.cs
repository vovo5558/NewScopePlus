using UnityEngine;
using System.Collections;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

public class about_thres : MonoBehaviour {
	public GameObject butterfly;
	//public Projector projector_l, projector_r;
	public GameObject sp1, sp2, sp3, sp4, sp_l, sp_r, sp12, sp22, sp32, sp42; //sp_l,sp_r是main camera的子物件
	
	public GameObject sp13, sp23, sp33, sp43; //for body position
	
	float[] p_ori;
	GameObject[] sp, sp_2 , sp_3;
	Vector3[] pos_sp, pos_sp2 ,pos_sp3;
	public Camera maincamera;
	
	Vector3 pos_l, pos_r;
	Texture2D tex, tex_warp;
	public Texture2D tex_ori, tex_ori2 ,tex_ori3; //1為上翅 2為下翅 3 body
	public GameObject wingUL,wingDL,wingUR,wingDR ,body;
	public GameObject imagetarget;
	Renderer[] renderers;
	//public RenderTexture rt;
	bool check = false;
	Animator anima;


	public GameObject bottom;
	Texture2D tex_bot;


	bool bool_bottom = false;
	float duration = 2.0f;
	Color startcol;
	Color endcol;
	int newAnimType;
	public GameObject sp_l_for_bottom, sp_r_for_bottom;
	public GameObject shadow;

	public GameObject vir1, vir2, vir3;

	
	// Use this for initialization
	void Start () {
		
		check = false;

		renderers = butterfly.GetComponentsInChildren<Renderer>(); //vuforia會自動把所有renderer都打開 所以要把她都關掉 包誇所有子物件
		anima = butterfly.GetComponent<Animator>();




		//
		bottom.GetComponent<Renderer>().enabled = false;
		shadow.GetComponent<Renderer>().enabled = false;
		startcol = bottom.GetComponent<Renderer> ().material.color;
		startcol.a = 1.0f;
		endcol = new Color (startcol.r, startcol.g, startcol.b, 0.0f);

		
		// butterfly.SetActive(false);
		
		sp = new GameObject[4];
		sp_2 = new GameObject[4];
		sp_3 = new GameObject[4];
		
		pos_sp = new Vector3[4];
		pos_sp2 = new Vector3[4];
		pos_sp3 = new Vector3[4];
		
		sp[0] = sp1;
		sp[1] = sp2;
		sp[2] = sp3;
		sp[3] = sp4;

		sp_2[0] = sp12;
		sp_2[1] = sp22;
		sp_2[2] = sp32;
		sp_2[3] = sp42;

		sp_3[0] = sp13;
		sp_3[1] = sp23;
		sp_3[2] = sp33;
		sp_3[3] = sp43;

		
		p_ori = new float[8] { 0.0f, 0.0f, 0.0f, 512.0f, 512.0f, 512.0f, 512.0f, 0f }; //從原圖標定的定位點 四個角 左上左下右上右下 
		
		//把定位球的render關掉
		
		for (int i = 0; i < 4; i++) { 
			sp[i].GetComponent<Renderer>().enabled = false;
			sp_2[i].GetComponent<Renderer>().enabled = false;
		}
		//sp_l.GetComponent<Renderer>().enabled = false;
		//sp_r.GetComponent<Renderer>().enabled = false;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		
		if (Input.GetKey(KeyCode.N)) //only flay
		{
			print ("fly");
			//set up fade out parameter
			bool_bottom = true;
			newAnimType = 0;

			Color botcol = bottom.GetComponent<Renderer> ().material.color;
			botcol.a = 0.0f;
			bottom.GetComponent<Renderer> ().material.color = botcol;
			bottom.GetComponent<Renderer>().enabled = true;

			shadow.GetComponent<Renderer> ().material.color = botcol;
			shadow.GetComponent<Renderer>().enabled = true;

			bottom.GetComponent<Renderer> ().material.mainTexture = tex_bot;
			
			butterflyAnim(0);  
 
		}
		if (Input.GetKey (KeyCode.RightAlt))  //use flyaway animation
		{

			print ("flyaway");
			//set up fade out parameter
			bool_bottom = true;
			newAnimType = 1;
			
			Color botcol = bottom.GetComponent<Renderer> ().material.color;
			botcol.a = 0.0f;
			bottom.GetComponent<Renderer> ().material.color = botcol;
			bottom.GetComponent<Renderer>().enabled = true;

			shadow.GetComponent<Renderer> ().material.color = botcol;
			shadow.GetComponent<Renderer>().enabled = true;


			//
			bottom.GetComponent<Renderer> ().material.mainTexture = tex_bot;

			butterflyAnim(1);
		}
		
		
		if (Input.GetKey(KeyCode.LeftAlt))  //turn off the buttfly
		{

			print("leftalt");
			//butterfly.SetActive(false);
			check = false;
			anima.SetBool("flybool", false);
			anima.SetBool("flyawaybool", false);
			
			foreach (Renderer r in renderers)
			{
				r.enabled = false;
			}
			bottom.GetComponent<Renderer>().enabled = false;
			shadow.GetComponent<Renderer>().enabled = false;

			//butterfly.SetActive(false);
			
			vir1.SetActive(true);			
			vir2.SetActive(true);			
			vir3.SetActive(true);
		}
	
		
		if(Input.GetKey(KeyCode.Y))
		{
			imagetarget.SetActive(true);
		}

		if (Input.GetKey (KeyCode.P)) //print bottom
		{
			print ("print screen");
			pos_l = maincamera.WorldToScreenPoint(sp_l_for_bottom.transform.position);
			pos_r = maincamera.WorldToScreenPoint(sp_r_for_bottom.transform.position);
			int width = (int)(pos_r.x - pos_l.x);
			int height = (int)(pos_r.y - pos_l.y);
			// print(width);
			// print(height);
			// print(pos_l.y);
			// print(pos_l.x);
			tex_bot = new Texture2D(width, height);
			//tex.ReadPixels(new UnityEngine.Rect(pos_l.x, pos_l.y, width, height), 0, 0);
			
			RenderTexture currentActiveRT = RenderTexture.active;
			// Set the supplied RenderTexture as the active one
			RenderTexture.active = maincamera.targetTexture;
			// Create a new Texture2D and read the RenderTexture image into it
			tex_bot.ReadPixels(new UnityEngine.Rect(pos_l.x, pos_l.y, width, height), 0, 0);
			// Restorie previously active render texture
			RenderTexture.active = currentActiveRT;
			
			
			tex_bot.Apply();
			Cv.ShowImage("bottom",Texture2dToMat(tex_bot));
		}

		if (bool_bottom)
		{
			Invoke("bottomFade", 0.02f);
		}

		if(!check)
		{ 
			foreach (Renderer r in renderers)
			{
				r.enabled = false;
			}
			bottom.GetComponent<Renderer>().enabled = false;
			shadow.GetComponent<Renderer>().enabled = false;
		}
		if (Input.GetKey (KeyCode.M))
		{
			vir1.SetActive(false);
			
			vir2.SetActive(false);
			
			vir3.SetActive(false);

		}



	}

	void bottomFade()
	{
		Color bottcol = bottom.GetComponent<Renderer> ().material.color;
		if (bottcol.a < 1.0) {
			bottcol.a += 0.2f;
			bottom.GetComponent<Renderer> ().material.color = bottcol;
			Color shadowcol = bottcol;
			shadowcol.a = shadowcol.a/1.95f;
			shadow.GetComponent<Renderer>().material.color = shadowcol;

		} 
		else 
		{
			if(newAnimType == 0)
				anima.SetBool("flybool", true);
			else if(newAnimType == 1)
				anima.SetBool("flyawaybool", true);
			bool_bottom = false;
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
		bool checkb = true;
		for (int i = 0; i < 4; i++)
		{
			if (pos_sp[i].x < pos_l.x || pos_sp[i].x > pos_r.x || pos_sp[i].y < pos_l.y || pos_sp[i].y > pos_r.y)
				checkb = false;
		}
		return checkb;
	}
	
	
	void SetColorToTransWings(Texture2D tex_warp, Texture2D tex_ori, GameObject wingL, GameObject wingR ,int mirror)
	{
		var pixel_warp =tex_warp.GetPixels();
		var pixel_ori = tex_ori.GetPixels();
		for (int i = 0; i < tex_ori.width * tex_ori.height; i++)
		{
			Color col = pixel_ori[i];
			pixel_warp[i].a = col.a;
		}
		Texture2D tex_dest = new Texture2D(tex_ori.width, tex_ori.height, TextureFormat.ARGB32, false);
		tex_dest.SetPixels(pixel_warp);
		tex_dest.Apply();
		wingL.GetComponent<Renderer>().material.mainTexture = tex_dest;
		
		if(mirror==1)
		{
			//對稱的右圖
			Texture2D tex_opp = new Texture2D(tex_ori.width, tex_ori.height, TextureFormat.ARGB32, false);
			Color[] pixel_oppsite = new Color[tex_ori.height*tex_ori.width];
			for (int i = 0; i < tex_ori.height; i++)
			{ 
				for (int j = 0; j < tex_ori.width; j++)
				{
					pixel_oppsite[i * tex_ori.width + j] = pixel_warp[i * tex_ori.width + (tex_ori.width - 1 - j)];
				}
			}
			tex_opp.SetPixels(pixel_oppsite);
			tex_opp.Apply();
			// Cv.ShowImage("op",tex)
			wingR.GetComponent<Renderer>().material.mainTexture = tex_opp;
		}
		
	}
	
	void butterflyAnim(int animType)
	{


		//butterfly.SetActive (true);



		anima.SetBool("flybool", false);
		anima.SetBool("flyawaybool",false);
		
		//開始
		pos_l = maincamera.WorldToScreenPoint(sp_l.transform.position);
		pos_r = maincamera.WorldToScreenPoint(sp_r.transform.position);
		int width = (int)(pos_r.x - pos_l.x);
		int height = (int)(pos_r.y - pos_l.y);
		// print(width);
		// print(height);
		// print(pos_l.y);
		// print(pos_l.x);
		tex = new Texture2D(width, height);
		//tex.ReadPixels(new UnityEngine.Rect(pos_l.x, pos_l.y, width, height), 0, 0);
		
		RenderTexture currentActiveRT = RenderTexture.active;
		// Set the supplied RenderTexture as the active one
		RenderTexture.active = maincamera.targetTexture;
		// Create a new Texture2D and read the RenderTexture image into it
		tex.ReadPixels(new UnityEngine.Rect(pos_l.x, pos_l.y, width, height), 0, 0);
		// Restorie previously active render texture
		RenderTexture.active = currentActiveRT;
		
		
		tex.Apply();
		Cv.ShowImage("21",Texture2dToMat(tex));
		
		//設定位球再tex上的xy位置用來warp
		CvMat P_ori = new CvMat(4, 2, MatrixType.F32C1);
		CvMat P_def = new CvMat(4, 2, MatrixType.F32C1);
		CvMat P_def2 = new CvMat(4, 2, MatrixType.F32C1);
		CvMat P_def3 = new CvMat(4, 2, MatrixType.F32C1);
		//print(tex_ori.width);
		//print(tex_ori.height);
		
		for (int i = 0; i < 4; i++)
		{
			pos_sp[i] = maincamera.WorldToScreenPoint(sp[i].transform.position);
			pos_sp2[i] = maincamera.WorldToScreenPoint(sp_2[i].transform.position);
			pos_sp3[i] = maincamera.WorldToScreenPoint(sp_3[i].transform.position);
			
			P_def.Set2D(i, 0, pos_sp[i].x - pos_l.x);
			P_def.Set2D(i, 1, pos_r.y - pos_sp[i].y);

			P_def2.Set2D(i, 0, pos_sp2[i].x - pos_l.x);
			P_def2.Set2D(i, 1, pos_r.y - pos_sp2[i].y);

			P_def3.Set2D(i, 0, pos_sp3[i].x - pos_l.x);
			P_def3.Set2D(i, 1, pos_r.y - pos_sp3[i].y);
			
			P_ori.Set2D(i, 0, p_ori[i * 2]);
			P_ori.Set2D(i, 1, p_ori[i * 2 + 1]);
		}
		
		if (CheckBoudary(pos_sp, pos_l, pos_r) && CheckBoudary(pos_sp2, pos_l, pos_r)&& CheckBoudary(pos_sp3, pos_l, pos_r)) //check boundary
		{
			
			tex_warp = warp(tex_ori, tex, P_ori, P_def);//左
			SetColorToTransWings(tex_warp, tex_ori, wingUL, wingUR,1); //右上左上翅
			tex_warp = warp(tex_ori2, tex, P_ori, P_def2);//右
			SetColorToTransWings(tex_warp, tex_ori2, wingDL, wingDR,1);//右下左下翅
			
			Texture2D tex_body = warp(tex_ori3, tex, P_ori, P_def3); //body
			Cv.ShowImage("1123",Texture2dToMat(tex_body));
			SetColorToTransWings(tex_body, tex_ori3, body,body,0); 

			
			//把蝴蝶秀出ne
			check = true;
			foreach (Renderer r in renderers)
			{
				r.enabled = true;
			}


			//把定位球關掉
			for (int i = 0; i < 4; i++)
			{
				sp[i].GetComponent<Renderer>().enabled = false;
				sp_2[i].GetComponent<Renderer>().enabled = false;
			}
			
			/*
			//現在才開始飛
			if(animType == 0)
				anima.SetBool("flybool", true);
			else if(animType == 1)
				anima.SetBool("flyawaybool", true);
				*/
			
			//butterfly.SetActive(true);
		}
		else
			print("Part of the butterfly is out of range");
	}


	
	
	
}
