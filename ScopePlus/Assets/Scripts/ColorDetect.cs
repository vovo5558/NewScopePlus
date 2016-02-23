using Emgu.CV.CvEnum;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System.IO.Ports;
using Emgu.CV;
using Emgu.CV.UI;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.Util;
using System.Drawing;
using System.Runtime.InteropServices;

[Serializable]
public class Region
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

public class ColorDetect : MonoBehaviour {
	
	// Use this for initialization
	MeshRenderer mr;
	Image<Bgr, Byte> oriImage;
	Image<Bgr, Byte> resImage;
	RenderTexture was;
	Texture2D tex;
	public int posX;
	public int posY;
	const int FRAME_WIDTH = 800;
	const int FRAME_HEIGHT = 600;
	public int width;
	public int height;
	public bool DebugDraw = false;
	Mat nonZeroCoordinates;
	MCvScalar avgPixelIntensity;

	public List<Region> Area;

	public Camera RightCamera;
	public Transform LEDPosition;
	public bool ledDebugDraw = false;
	public int ledOffsetX = 67;
	public int ledOffsetY = 46;
	public Region ledRegion;
	Vector3 position;
	public GameObject specialEffect;
	 int H_MIN = 0;
	 int H_MAX = 4;
	 int S_MIN = 111;
	 int S_MAX = 255;
	 int V_MIN = 0;
	 int V_MAX = 255;
	// Use this for initialization
	static Texture2D textureI2TC3;
	static byte[] dataI2TC3;
	static Texture2D textureI2TC4;
	static byte[] dataI2TC4;
	
	void Start () {
		mr = GetComponent<MeshRenderer> ();
		resImage = new Image<Bgr, Byte> (FRAME_WIDTH, FRAME_HEIGHT);
		tex = new Texture2D (FRAME_WIDTH, FRAME_HEIGHT);
		nonZeroCoordinates = new Mat();
		textureI2TC3 = new Texture2D(FRAME_WIDTH, FRAME_HEIGHT, TextureFormat.RGB24, false);
		dataI2TC3 = new byte[FRAME_WIDTH * FRAME_HEIGHT * 3];
		textureI2TC4 = new Texture2D(FRAME_WIDTH, FRAME_HEIGHT, TextureFormat.RGBA32, false);
		dataI2TC4 = new byte[FRAME_WIDTH * FRAME_HEIGHT * 4];
		StartCoroutine("colorDetect");
		specialEffect.SetActive(false);
	}
	
	void Update()
	{
		if(ledRegion.available)	{
			position = RightCamera.WorldToScreenPoint(LEDPosition.position);
			ledRegion.posX = FRAME_WIDTH -(int)position.x - ledOffsetX;
			ledRegion.posY = FRAME_HEIGHT -(int)position.y - ledOffsetY;
			ledRegion.height = 40;
			ledRegion.width = 40;
		}
	}
	void changeStatus(bool value)
	{
		ledRegion.available = value;
	}


    IEnumerator colorDetect()
    {
		while (true) {
			yield return new WaitForSeconds (0.2f);
			was = RenderTexture.active;
			RenderTexture.active = (RenderTexture)mr.material.mainTexture;
			tex.ReadPixels (new Rect (0, 0, FRAME_WIDTH, FRAME_HEIGHT), 0, 0);
			tex.Apply ();
			RenderTexture.active = was;
			oriImage = Texture2dToImage<Bgr, byte> (tex, true);
			if (DebugDraw) {
				tex.SetPixels (posX, posY, width, height, new UnityEngine.Color[width * height]);
			}
			if (ledDebugDraw) {
				tex.SetPixels (ledRegion.posX, ledRegion.posY, ledRegion.width, ledRegion.height, new UnityEngine.Color[ledRegion.width * ledRegion.height]);
			}
			tex.Apply ();
			mr.material.mainTexture = tex;
			Image<Hsv, Byte> hsv_image = oriImage.Convert<Hsv, Byte> ();
			// Change the HSV value here
			Hsv hsvmin = new Hsv (H_MIN, S_MIN, V_MIN);
			Hsv hsvmax = new Hsv (H_MAX, S_MAX, V_MAX);

			hsv_image = hsv_image.SmoothGaussian (5, 5, 0.1, 0.1);

			Image<Gray, byte> red_object = hsv_image.InRange (hsvmin, hsvmax);

			red_object = red_object.Erode (1);
			red_object = red_object.Dilate (1);

			CvInvoke.FindNonZero (red_object, nonZeroCoordinates);
			avgPixelIntensity = CvInvoke.Mean (nonZeroCoordinates);

		//	CvInvoke.Imshow ("Origin Image", red_object); //Show the image
//			Debug.Log (avgPixelIntensity.V0 + " " + avgPixelIntensity.V1);
			if (ledRegion.available) {
				ledRegion.flag = false;
				if (avgPixelIntensity.V0 > ledRegion.posX && avgPixelIntensity.V0 < ledRegion.posX + ledRegion.width
					&& avgPixelIntensity.V1 > (FRAME_HEIGHT - ledRegion.posY) - ledRegion.width && avgPixelIntensity.V1 < (FRAME_HEIGHT - ledRegion.posY)) {
					Debug.Log ("Press Color Button");
					ledRegion.flag = true;
					specialEffect.SetActive (true);
				} else {
					specialEffect.SetActive (false);
				}

				if ((ledRegion.flag != ledRegion.lastFlag) && (ledRegion.flag))
					ledRegion.toggle = !ledRegion.toggle;
				
				if (ledRegion.callback != "") {
					SendMessage (ledRegion.callback, ledRegion.flag);
				}
				
				ledRegion.lastFlag = ledRegion.flag;
			}

			foreach (Region area in Area) {
				if (!area.available)
					continue;
	
				area.flag = false;
				if (avgPixelIntensity.V0 > area.posX && avgPixelIntensity.V0 < area.posX + area.width
					&& avgPixelIntensity.V1 > (FRAME_HEIGHT - area.posY) - area.width && avgPixelIntensity.V1 < (FRAME_HEIGHT - area.posY)) {
					Debug.Log ("Press Color Button");
					area.flag = true;

				}

				if ((area.flag != area.lastFlag) && (area.flag))
					area.toggle = !area.toggle;

				if (area.callback != "") {
					if (area.type == Region.Type.Button) {
						if (area.state != (area.flag != area.lastFlag) && (area.flag))
							SendMessage (area.callback, (area.flag != area.lastFlag) && (area.flag));

						area.state = (area.flag != area.lastFlag) && (area.flag);
					} else if (area.type == Region.Type.Switch) {
						if (area.state != area.toggle)
							SendMessage (area.callback, area.toggle);

						area.state = area.toggle;
					}
				}

				area.lastFlag = area.flag;
			}
		}
    }
	public static Image<TColor, TDepth> Texture2dToImage<TColor, TDepth>(Texture2D texture, bool correctForVerticleFlip = true)
		where TColor : struct, IColor
		where TDepth : new()
	{
		int width = texture.width;
		int height = texture.height;

		Image<TColor, TDepth> result = new Image<TColor, TDepth>(width, height);
		try
		{
			Color32[] colors = texture.GetPixels32();
			GCHandle handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
			using (Image<Rgba, Byte> rgba = new Image<Rgba, byte>(width, height, width * 4, handle.AddrOfPinnedObject()))
			{
				result.ConvertFrom(rgba);
			}
			handle.Free();
		}
		catch (Exception)
		{
			byte[] jpgBytes = texture.EncodeToJPG();
			using (Mat tmp = new Mat())
			{
				CvInvoke.Imdecode(jpgBytes, LoadImageType.AnyColor, tmp);
				result.ConvertFrom(tmp);
			}
		}
		if (correctForVerticleFlip)
			CvInvoke.Flip(result, result, Emgu.CV.CvEnum.FlipType.Vertical);
		return result;
	}

	public static Texture2D ImageToTexture2D<TColor, TDepth>(Image<TColor, TDepth> image, bool correctForVerticleFlip = true)
		where TColor : struct, IColor
		where TDepth : new()
	{
		Size size = image.Size;

		if (typeof(TColor) == typeof(Rgb) && typeof(TDepth) == typeof(Byte))
		{

			GCHandle dataHandle = GCHandle.Alloc(dataI2TC3, GCHandleType.Pinned);
			using (Image<Rgb, byte> rgb = new Image<Rgb, byte>(size.Width, size.Height, size.Width * 3, dataHandle.AddrOfPinnedObject()))
			{
				rgb.ConvertFrom(image);
				if (correctForVerticleFlip)
					CvInvoke.Flip(rgb, rgb, Emgu.CV.CvEnum.FlipType.Vertical);
			}
			dataHandle.Free();
			textureI2TC3.LoadRawTextureData(dataI2TC3);
			textureI2TC3.Apply();
			return textureI2TC3;
		}
		else //if (typeof(TColor) == typeof(Rgba) && typeof(TDepth) == typeof(Byte))
		{

			GCHandle dataHandle = GCHandle.Alloc(dataI2TC4, GCHandleType.Pinned);
			using (Image<Rgba, byte> rgba = new Image<Rgba, byte>(size.Width, size.Height, size.Width * 4, dataHandle.AddrOfPinnedObject()))
			{
				rgba.ConvertFrom(image);
				if (correctForVerticleFlip)
					CvInvoke.Flip(rgba, rgba, Emgu.CV.CvEnum.FlipType.Vertical);
			}
			dataHandle.Free();
			textureI2TC4.LoadRawTextureData(dataI2TC4);
			textureI2TC4.Apply();
			return textureI2TC4;
		}

		//return null;
	}
	
}