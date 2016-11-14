using UnityEngine;
using System.Collections;

public class PlayCharacterSound : MonoBehaviour {
	AudioSource _myAud;
	public AudioClip _rollingClip;

	void Start(){
		_myAud = GetComponent<AudioSource> ();
	}

	void PlayRolling(float _volume = 1.0f){
		_myAud.clip = _rollingClip;
		_myAud.volume = _volume;
		_myAud.Play();
	}	
}
