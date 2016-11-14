using UnityEngine;
using System.Collections;

public class PlayEyeSounds : MonoBehaviour {
	AudioSource _myAud;
	public AudioClip _eyeRoar;
	float _latency = 0.5f;
	public int _myIndex;

	void Start(){
		_myAud = GetComponent<AudioSource> ();
		if (name != "endScene") {
			PlayRolling (_myIndex * _latency);
		}
	}

	void PlayRolling(float _ltn){
		_myAud.clip = _eyeRoar;
		_myAud.volume = 1f;
		_myAud.PlayDelayed(_ltn);
	}	
}
