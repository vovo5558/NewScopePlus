using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using System;
using System.Text;
using System.IO;
[System.Serializable]
public class ArrayMode
{
	public int minX, minY, midX, midY, maxX, maxY;	
}
public class SerialPortControl : MonoBehaviour {
	//change it if comport were wrong
	private static string COM_PORT = "COM4";
	private static int BUAD_RATE = 250000;
	/// public variable 
	public float boundaryX = 120f;
	public float boundaryY = 140f;
	public float zMovement = 0.2f;
	public float xyMovement = 1f;
	public float xySpeed = 600f;
	public float zSpeed = 140f;
	public float waitTime = 0.04f;
	public ArrayMode arrayMode;
	
	static string serialBuffer = "";
	static string expectedEcho = null;
	static object expectedEchoLock = new object();
	static ManualResetEvent expectedEchoReceived = new ManualResetEvent(false);
	public static SerialPort sp = new SerialPort(COM_PORT, BUAD_RATE, Parity.None, 8, StopBits.One);
	public static string strIn;
	
	
	private float curX, curY;
	private bool changeMode = false;
	private bool isWriting = false; 
	private int mode = 0;
	private int arrayState;
	// Use this for initialization
	void Start () {
		OpenConnection ();
		sp.DataReceived += new SerialDataReceivedEventHandler (port_DataReceived);
		
		///////////////////////////////////////////////////////////////////////////////////
		/////////////////////////////Gcode initialization//////////////////////////////////
		///////////////////////////////////////////////////////////////////////////////////
		
		
		SendCommand ("M110 \r\n");
		SendCommand ("M92 X80.00 Y80.00 Z4000.00\r\n");
		SendCommand ("M203 X500.00 Y500.00 Z5.00\r\n");
		SendCommand ("M201 X3000 Y3000 Z100\r\n");
		SendCommand ("M204 S1000.00 T3000.00\r\n");
		SendCommand ("M205 S0.00 T0.00 B20000 Z0.40\r\n");
		SendCommand ("M206 X0.00 Y0.00 Z0.00\r\n");
		SendCommand ("M301 P17.92 I1.04 D79.94\r\n");
		SendCommand ("M107\r\n");
		SendCommand ("G91\r\n");
		SendCommand ("G28 X0 Y0\r\n");
		Write ("G1 X80 Y100 F3000");
		curX = 60f;
		curY = 70f;
		//Write  ("G28 Z0");
		//Write  ("G1 Z30 F100");
		
		
	}
	void Update () {
		
		/////get joystick movement/////
		float m_JoyX = 0.0f;  
		float m_JoyY = 0.0f;
		m_JoyX = Input.GetAxis ("Horizontal");  
		m_JoyY = Input.GetAxis ("Vertical");  
		
		
		///////caculating the valacity///////
		float radius = (Mathf.Sqrt (Mathf.Pow (m_JoyX, 2) + Mathf.Pow (m_JoyY, 2)));
		int dRadius = 0;
		if (m_JoyX != 0 || m_JoyY != 0) {
			//if(radius < )
			if (radius < Mathf.Sqrt (2) / 2)
				dRadius = 1;
			else
				dRadius = 2;
		}
		
		////////Z direction movement////////
		int zdirection = 0;
		if (Input.GetKey (KeyCode.JoystickButton11) || Input.GetKey (KeyCode.UpArrow))
			zdirection = 1;
		else if (Input.GetKey (KeyCode.JoystickButton10) || Input.GetKey (KeyCode.DownArrow))
			zdirection = -1;
		
		////////0 for coord mode and 1 for array mode//////
		if (Input.GetKeyDown (KeyCode.Joystick1Button7)) {
			if (mode == 0){
				mode = 1;
				changeMode = true;
			}
			else {
				changeMode = true;
				mode = 0;
				
			}
		}
		
		//coord mode
		if (mode == 0) {
			// if mode is changed, system most be initialized
			if(changeMode){
				SendCommand ("G91\r\n");
				SendCommand ("G28 X0 Y0\r\n");
				Write ("G1 X80 Y100 F3000");
				curX = 60f;
				curY = 70f;
				changeMode = false;
			}
			//////////////////G-code command//////////////////
			float x_value = 0;
			float y_value = 0;
			if (m_JoyX > 0)
				x_value = xyMovement;
			else if (m_JoyX < 0)
				x_value = -1 * xyMovement;
			if (m_JoyY > 0)
				y_value = xyMovement;
			else if (m_JoyY < 0)
				y_value = -1 * xyMovement;
			if (dRadius >= 0.35 && !isWriting) {
				//If both x axis and y axis is moving, than I'll set sqrt(2)*speed rate
				if (x_value != 0 && y_value != 0) {
					//test the boundary condition
					if (curX + dRadius * x_value <= boundaryX && curX + dRadius * x_value >= 0 &&
					    curY + dRadius * y_value <= boundaryY && curY + dRadius * y_value >= 0) {
						Write ("G1 X" + dRadius * x_value +
						       " Y" + dRadius * y_value +
						       " F" + xySpeed * dRadius * Mathf.Sqrt (2));
						curX += dRadius * x_value;
						curY += dRadius * y_value;
					} else if (curX + dRadius * x_value <= boundaryX && curX + dRadius * x_value >= 0) {
						Write ("G1 X" + dRadius * x_value +
						       " F" + xySpeed * dRadius);
						curX += dRadius * x_value;
					} else if (curY + dRadius * y_value <= boundaryY && curY + dRadius * y_value >= 0) {
						Write ("G1 Y" + dRadius * y_value +
						       " F" + xySpeed * dRadius);
						curY += dRadius * y_value;
					}
				} else if (x_value != 0 && 
				           curX + dRadius * x_value <= boundaryX && curX + dRadius * x_value >= 0) {
					Write ("G1 X" + dRadius * x_value +
					       " F" + xySpeed * dRadius);
					curX += dRadius * x_value;
				} else if (y_value != 0 && 
				           curY + dRadius * y_value <= boundaryY && curY + dRadius * y_value >= 0) {
					Write ("G1 Y" + dRadius * y_value +
					       " F" + xySpeed * dRadius);
					curY += dRadius * y_value;
				}
				//print ("curX:" + curX + "curY:" + curY);
				//wait for movement be done
				StartCoroutine (Wait (waitTime));
				
			}
			// Z direction movement
			if (zdirection != 0 && !isWriting) {
				Write ("G1 Z" + zdirection * zMovement + " F" + zSpeed);
				StartCoroutine (Wait (waitTime));
			}
			///////////////End G-code command///////////////
		} else if (mode == 1) {
			//if mode changed, system most be initialized
			if (changeMode) {
				SendCommand ("G90\r\n");
				SendCommand ("G28 X0 Y0\r\n");
				Write ("G1 X80 Y100 F3000");
				changeMode = false;
				arrayState = 5;
			}
			if (radius >= 1) {						//insure will not accidentally touch
				SendCommand ("G90\r\n");
				if (m_JoyX > 0.2 && m_JoyY > 0.2 && arrayState != 3) {
					//uppper right corner
					Write ("G1 X" + arrayMode.maxX + "Y" + arrayMode.maxY + "F3000");
				} else if (m_JoyX > 0.2 && m_JoyY < 0.2 && m_JoyY > -0.2 && arrayState != 6) {
					//right side
					Write ("G1 X" + arrayMode.maxX + "Y" + arrayMode.midY + "F3000");
				} else if (m_JoyX > 0.2 && m_JoyY < -0.2 && arrayState != 9) {
					//lower right corner
					Write ("G1 X" + arrayMode.maxX + "Y" + arrayMode.minY + "F3000");
				} else if (m_JoyX < 0.2 && m_JoyX > -0.2 && m_JoyY < -0.2 && arrayState != 8) {
					// down side
					Write ("G1 X" + arrayMode.midX + "Y" + arrayMode.minY + "F3000");
				} else if (m_JoyX < -0.2 && m_JoyY < -0.2 && arrayState != 7) {
					//lower left corner
					Write ("G1 X" + arrayMode.minX + "Y" + arrayMode.minY + "F3000");
				} else if (m_JoyX < -0.2 && m_JoyY < 0.2 && m_JoyY > -0.2 && arrayState != 4) {
					//left side
					Write ("G1 X" + arrayMode.minX + "Y" + arrayMode.midY + "F3000");
				} else if (m_JoyX < -0.2 && m_JoyY > 0.2 && arrayState != 1) {
					//upper left corner
					Write ("G1 X" + arrayMode.minX + "Y" + arrayMode.maxY + "F3000");
				} else if (m_JoyX < 0.2 && m_JoyX > -0.2 && m_JoyY > 0.2 && arrayState != 2) {
					//upper side
					Write ("G1 X" + arrayMode.midX + "Y" + arrayMode.maxY + "F3000");
				}
			}
		}
		// Z direction movement
		if (zdirection != 0 && !isWriting) {
			SendCommand ("G91\r\n");
			Write ("G1 Z" + zdirection * zMovement + " F" + zSpeed);
			StartCoroutine (Wait (waitTime));
		}
	}
	
	///data receive handler
	/// <summary>
	/// Port_s the data received.
	/// </summary>
	/// <param name="sender">Sender.</param>
	/// <param name="e">E.</param>
	static void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
	{
		while (sp.BytesToRead > 0) {
			byte[] buffer = new byte[sp.BytesToRead];
			int bytesRead = sp.Read(buffer, 0, buffer.Length);
			if(bytesRead <= 0)
				return;
			serialBuffer += Encoding.UTF8.GetString(buffer, 0, bytesRead);
			string[] lines = serialBuffer.Split('\r', '\n');
			for(int i = 0; i < (lines.Length - 1); i++){
				if(lines[i].Length > 0){
					Debug.Log("Get lines");
					ProcessLine(lines[i]);
					
				}
			}
			serialBuffer = lines[lines.Length - 1];
		}
	}
	/// <summary>
	/// Processes the line.
	/// </summary>
	/// <param name="line">Line.</param>
	static void ProcessLine(String line){
		Debug.Log ("HAHAHA");
		bool unsolicitedMessageReceived = false;
		lock (expectedEchoLock) {
			if(line == expectedEcho){
				Debug.Log ("response:");
				Debug.Log (line);
				expectedEchoReceived.Set();
			}
			else
				unsolicitedMessageReceived = true;
		}
		if (unsolicitedMessageReceived) {
			Debug.Log ("response:");
			Debug.Log (line);
		}
	}
	/// <summary>
	/// Sends the command.
	/// </summary>
	/// <returns><c>true</c>, if command was sent, <c>false</c> otherwise.</returns>
	/// <param name="command">Command.</param>
	static bool SendCommand (String command){
		lock (expectedEchoLock) {
			expectedEchoReceived.Reset();
			expectedEcho = command;
		}
		sp.Write (command);
		return expectedEchoReceived.WaitOne (50);
	}
	/// <summary>
	/// Write the specified block.
	/// </summary>
	/// <param name="block">Block.</param>
	public void Write(String block)
	{
		block.Trim ();
		block.Replace (" ", "");
		block.Replace ("\t", "");
		sp.WriteLine(block+"\n");
		//Read("OK");
		//Thread.Sleep(250);
		//StartCoroutine(Wait(0.5F));
	}
	/// <summary>
	/// Read the specified expect.
	/// </summary>
	/// <param name="expect">Expect.</param>
	public void Read(String expect)
	{
		while(true)
		{
			//Debug.Log("Read");
			String response = sp.ReadLine();
			//Thread.Sleep(1000);
			Debug.Log(" something");
			response.Trim();
			if(expect == null){
				Debug.Log("NULL");
				return;
			}
			else if(response.ToLower().Contains(expect.ToLower()))
			{	
				Debug.Log ("OK");
				//Thread.Sleep(100);
				return;
			}
			else {
				Debug.Log ("<" + response);
				//Thread.Sleep(100);
				return;
				
			}
		}
	}
	/// <summary>
	/// Wait the specified waitTime.
	/// </summary>
	/// <param name="waitTime">Wait time.</param>
	IEnumerator Wait(float waitTime) {
		isWriting = true;
		yield return new WaitForSeconds(waitTime);
		isWriting = false;
		//print("WaitAndPrint " + Time.time);
	}
	/// <summary>
	/// Opens the connection.
	/// </summary>
	public void OpenConnection()
	{
		if (sp != null) {
			if (sp.IsOpen) {
				sp.Close ();
				Debug.Log ("Closing port, because it was already open");
			} else {
				sp.Open ();
				sp.ReadTimeout = 1000;  // sets the timeout value before reporting error
				Debug.Log ("Port Opened!");
				Debug.Log("Baud Rate:" + sp.BaudRate + "\n");
				//Thread.Sleep(2000);
			}
		} else {
			if(sp.IsOpen)
			{
				Debug.Log("Port is already open\n");
				
			}
			else
			{
				Debug.Log("Port == null");
			}
		}
	}
	/// <summary>
	/// Raises the application quit event.
	/// </summary>
	void OnApplicationQuit() 
	{
		sp.Close();
	}
}
