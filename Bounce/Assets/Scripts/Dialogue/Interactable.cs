using UnityEngine;
using System.Collections.Generic;

public class Interactable : MonoBehaviour {

	public TextAsset dialogueFile;
	private Interaction dialogue;
	public TextMesh gText;
	private bool inTrigger = false;

	// Use this for initialization
	void Awake () {
		dialogue = new Interaction (dialogueFile);
		//gText = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Action") && inTrigger)
		{
			Debug.Log("Stepping through dialogue");
			List<string> lines = dialogue.Step();
			if(lines.Count > 0)
			{
				gText.text = lines[0];
			}
			else
				gText.text = "";
		}
	}
	
	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			//animation appears to show you can talk

			inTrigger = true;
		}
	}
	
	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "Player") {
			//animation disappears

			inTrigger = false;
		}
	}
}
