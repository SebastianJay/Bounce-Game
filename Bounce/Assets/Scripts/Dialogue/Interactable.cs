using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour {

	public TextAsset dialogueFile;
	private Interaction dialogue;
	// Use this for initialization
	void Awake () {
		dialogue = new Interaction (dialogueFile);
	}
	
	// Update is called once per frame
	void Update () {	
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			//animation appears to show you can talk
			PlayerBallControl pbc = other.GetComponent<PlayerBallControl>();

			if (Input.GetButtonDown("Action"))
			{
				//do we handle input key here or in PlayerBallControl?
				pbc.isTalking = true;
				pbc.npcTalker = this;
			}
		}
	}

	void OnTriggerStay2D(Collider2D other) {
		if (other.tag == "Player") {
			PlayerBallControl pbc = other.GetComponent<PlayerBallControl>();
			if (Input.GetButtonDown("Action"))
			{
				//do we handle input key here or in PlayerBallControl?
				pbc.isTalking = true;
				pbc.npcTalker = this;
			}
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "Player") {
			//animation disappears
		}
	}
}
