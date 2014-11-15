using UnityEngine;
using System.Collections.Generic;

public class Interactable : MonoBehaviour {

	public TextAsset dialogueFile;
	private Interaction dialogue;
	public TextMesh gText;

	private bool inTrigger = false;
	private GameObject playerObj;
	private bool inConveration = false;

	// Use this for initialization
	void Awake () {
		dialogue = new Interaction (dialogueFile);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Action") && inTrigger)
		{
			if (!inConveration)
				inConveration = true;
			List<string> lines = dialogue.Step(playerObj.GetComponent<DialogueResponses>().cursor);
			if(lines.Count > 0)
			{
				gText.text = lines[0];
				List<string> heroLines = lines.GetRange(1, lines.Count - 1);
				playerObj.GetComponent<DialogueResponses>().SetResponses(heroLines);
				playerObj.rigidbody2D.velocity = Vector2.zero;
			}
			else
			{
				gText.text = "";
				inConveration = false;
			}
		}
		else if (Input.GetButtonDown("Jump") && inConveration)
		{
			playerObj.GetComponent<DialogueResponses>().MoveCursor(true);
		}
		else if (Input.GetButtonDown("Floor") && inConveration)
		{
			playerObj.GetComponent<DialogueResponses>().MoveCursor(false);
		}
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			//animation appears to show you can talk
			inTrigger = true;
			playerObj = other.gameObject;
		}
	}
	
	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "Player") {
			//animation disappears
			inTrigger = false;
		}
	}
}
