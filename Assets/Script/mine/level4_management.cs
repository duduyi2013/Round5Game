using UnityEngine;
using System.Collections;

public class level4_management : MonoBehaviour {

	public GameObject playerIdel;
	Animator anime;
	//private bool animationPlay = true;

	Movement characterMove;
	public GameObject destination;

	public GameObject boss;
	public GameObject endingAnimation;
	private Animator anie_end;

	GlitchEffect glitchNow;
	public GameObject camera_main;
	public GameObject fakeWorld;
	public GameObject monsterModel;
	public GameObject hidMonster;
	//public GameObject realWorld;
	public GameObject deadPeople;
	private bool endNow=false;
    bool _isMoveUnderControl = true;

	AudioSource _myAud;
	public AudioClip BattlingBGM;
	public AudioClip AfterBattleBGM;


	// Use this for initialization
	void Start () {
		characterMove = GetComponent<Movement> ();
		anime = playerIdel.GetComponent<Animator> ();
		anie_end = endingAnimation.GetComponent<Animator> ();

		characterMove.TurnOffMoveControll();
		_isMoveUnderControl = characterMove.MoveTo (destination.transform.position);

		glitchNow = camera_main.GetComponent<GlitchEffect> ();

		StartCoroutine (startAnimation());
		PublicVariables.monsterDead = false;

		_myAud = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(PublicVariables.monsterDead == true && endNow==false){
			StartCoroutine (EndAnimation());
		}

        if (!_isMoveUnderControl)
        {
            _isMoveUnderControl = characterMove.MoveTo(destination.transform.position);
        }
	}

	void OnTriggerEnter (Collider c){
		if (c.tag == "Monsters") {
			anime.SetTrigger ("hit");
		}
	}

	//character walking to the centrl

	//animation start

	IEnumerator startAnimation(){
		yield return new WaitForSeconds (4);
		characterMove.SwitchPerspective ();
		characterMove.TurnOnMoveControll();
		yield return new WaitForSeconds (1);
		anie_end.SetTrigger ("playStart");

		yield return new WaitForSeconds (10);
		characterMove.SwitchPerspective ();
		boss.SetActive (true);
		endingAnimation.SetActive (false);

		_myAud.clip = BattlingBGM;
		_myAud.volume = 1.0f;
		_myAud.Play ();
	}



	IEnumerator EndAnimation(){
		endNow = true;
		yield return new WaitForSeconds (1);
		characterMove.TurnOffMoveControll();
		characterMove.MoveTo (destination.transform.position);

		yield return new WaitForSeconds (5);
		glitchNow.enabled = true;

		_myAud.clip = AfterBattleBGM;
		_myAud.volume = 1f;
		_myAud.Play ();

		fakeWorld.SetActive (false);
		deadPeople.SetActive (true);
		monsterModel.SetActive (false);
		yield return new WaitForSeconds (2);
		glitchNow.enabled = false;
		fakeWorld.SetActive (true);
		deadPeople.SetActive (false);
		monsterModel.SetActive (true);
		yield return new WaitForSeconds (2);
		fakeWorld.SetActive (false);
		deadPeople.SetActive (true);
		monsterModel.SetActive (false);
		characterMove.SwitchPerspective ();

		yield return new WaitForSeconds (2);
		endingAnimation.SetActive (true);
		hidMonster.SetActive (false);
		anie_end.SetTrigger ("finalshot");
	}


}
