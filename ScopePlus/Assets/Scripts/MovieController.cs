using UnityEngine;
using System.Collections;

public class MovieController : MonoBehaviour {
	private MovieTexture mask;
	private MovieTexture movie;
	// Use this for initialization

	public bool movieTag = false;
	public int count = 0;
	void Start () {
	 
		mask = (MovieTexture)GetComponent<Renderer>().material.GetTexture("_AlphaTex");
		movie = (MovieTexture)GetComponent<Renderer>().material.GetTexture("_MainTex");
		mask.loop = true;
		movie.loop = true;
		GetComponent<Renderer>().enabled = false;
		//mask.Play();
		//movie.Play();
	}
	
	// Update is called once per frame
	void Update () {
		/*if(!movie.isPlaying){
			//print ("stopvideo");
			//movie.Stop();
			//mask.Stop();
			GetComponent<Renderer>().enabled = false;
			
		}*/
		/*if(movieTag == true && (count%2 == 1)){
			if(movie.isPlaying){
				print ("stopvideo");
				count++;
				movie.Stop();
				mask.Stop();
				GetComponent<Renderer>().enabled = false;

			}
			else{
				print ("playvideo");
				GetComponent<Renderer>().enabled = true;
				count++;
				//mask.
				mask.Play();
				movie.Play();
			}
	}*/

		if (movieTag == false) {
				print ("stopvideo");
				movie.Stop();
				mask.Stop();
				GetComponent<Renderer>().enabled = false;
			}
			else{
				print ("playvideo");
				GetComponent<Renderer>().enabled = true;
				mask.Play();
				movie.Play();
			}
		}

}