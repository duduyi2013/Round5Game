using UnityEngine;
using System.Collections;

public class level3_manager : MonoBehaviour {

	Movement _characterMove;
	public GameObject UI;

	// Use this for initialization
	void Start () {
		_characterMove = GetComponent<Movement> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnTriggerEnter(Collider c){
		
		if(c.tag == "Robot" ){
			StartCoroutine (RalkTORobot());
		}
		if (c.tag == "TeleportTo4") {
			Application.LoadLevel (3);
		}

	}

	void OnTriggerStay(Collider c){
		
		if (c.tag == "goTolevel4") {	
			_characterMove.MakeCamStatic ();
		} 
	}

	void OnTriggerExit(Collider c){
		Debug.Log ("trigger work");
		if (c.tag == "goTolevel4") {	
			_characterMove.MakeCamFree ();
		}
	}

	IEnumerator RalkTORobot(){
		UI.SetActive (true);
		_characterMove.SwitchPerspective ();
		yield return new WaitForSeconds (5);


		_characterMove.SwitchPerspective ();
	}




	// detach robot

	//go to level4
}
