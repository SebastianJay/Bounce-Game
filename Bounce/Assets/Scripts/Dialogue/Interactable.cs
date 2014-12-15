using UnityEngine;
using System.Collections.Generic;

public class Interactable : MonoBehaviour {

	public TextAsset dialogueFile;
	private Interaction dialogue;
	public AudioClip talkNoise;
	public float talkVolume = 1f;

	private bool inTrigger = false;
	private bool inConversation = false;
	private GameObject playerObj;
  	private AudioSource talkSrc;
	private GameObject dSystem;	//reference to the dialogue system

	// Use this for initialization
	void Awake () {
		dialogue = new Interaction (dialogueFile);
		if (talkNoise != null)
		{
			talkSrc = gameObject.AddComponent<AudioSource>();
			talkSrc.clip = talkNoise;
			talkSrc.volume = talkVolume;
		}
		dSystem = GameObject.FindGameObjectWithTag("DialogueSystem");
		if (dSystem == null)
			Debug.LogError("No dialogue system present in scene!");
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Action") && inTrigger)
		{
			PlayerBallControl bScript = playerObj.GetComponent<PlayerBallControl>();
			if (!bScript.inConversation)
			{
				bScript.inConversation = true;
				bScript.playerLock = true;
				this.inConversation = true;
				if (talkSrc != null)
					talkSrc.Play();
			}
			List<string> lines = dialogue.Step(dSystem.GetComponent<DialogueSystem>().GetCursor());
			if(lines.Count > 0)
			{
				dSystem.GetComponent<DialogueSystem>().PushNPCText(lines[0], transform.position);
				dSystem.GetComponent<DialogueSystem>().PushPlayerText(lines.GetRange(1, lines.Count - 1), 
				                                                      GameObject.FindGameObjectWithTag("Player").transform.position);
				playerObj.rigidbody2D.velocity = Vector2.zero;
				playerObj.rigidbody2D.angularVelocity = 0f;
			}
			else
			{
				dSystem.GetComponent<DialogueSystem>().EndConversation();
				bScript.inConversation = false;
				this.inConversation = false;
				bScript.playerLock = false;
			}
		}
		else if (Input.GetButtonDown("Jump") && inConversation)
		{
			dSystem.GetComponent<DialogueSystem>().MoveCursor(true);
		}
		else if (Input.GetButtonDown("Floor") && inConversation)
		{
			dSystem.GetComponent<DialogueSystem>().MoveCursor(false);
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
