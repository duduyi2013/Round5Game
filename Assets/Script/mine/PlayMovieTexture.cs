using UnityEngine;
using System.Collections;

public class PlayMovieTexture : MonoBehaviour {
	public MovieTexture _myVideo;
	// Use this for initialization
	void Start () {
		_myVideo.Play ();
		StartCoroutine (goTonextLevel());
	}

	IEnumerator goTonextLevel(){
		yield return new WaitForSeconds (23f);
		//go to next level
		Application.LoadLevel(2);
	}
	

}
