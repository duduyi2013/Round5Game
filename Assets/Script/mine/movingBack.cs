using UnityEngine;
using System.Collections;

public class movingBack : MonoBehaviour {
	AudioSource _myAud;
	public AudioClip _glitchSound;
	public GameObject newLocation;
	private Vector3 newloc_pos;

	private bool startToMove =false;

	public float speed = 0.01f;
	private float lenght;

	GlitchEffect glitchNow;
	public GameObject camera_main;

	// Use this for initialization
	void Start () {
		newloc_pos = new Vector3(-.58f,2.49f,10f);
		lenght = Vector3.Distance (this.transform.position, newloc_pos);
		StartCoroutine (delayMove());
		glitchNow = camera_main.GetComponent<GlitchEffect> ();
		_myAud = GetComponent<AudioSource> ();
	}

	// Update is called once per frame
	void Update () {

		if(startToMove){
			transform.position =  Vector3.Lerp(transform.position, newloc_pos, Time.time * speed / lenght);
			if (newloc_pos.z < 39) {
				newloc_pos.z += .01f;
			}

		}
	}
	IEnumerator delayMove(){
		yield return new WaitForSeconds (4f);
		startToMove = true;
		yield return new WaitForSeconds (10f);
		glitchNow.enabled = true;
		_myAud.clip = _glitchSound;
		_myAud.volume = 1.0f;
		_myAud.Play ();
		yield return new WaitForSeconds (13f);
		//load next scene
		Application.LoadLevel(1);
	}
}
