using UnityEngine;
using System.Collections.Generic;

//[RequireComponent (typeof(BoxCollider2D))]
public class Interactable : MonoBehaviour {

	public TextAsset dialogueFile;
	private Interaction dialogue;
	public AudioClip talkNoise;
	public float talkVolume = 1f;

	private string npcName;
	private bool inTrigger = false;
	private bool inConversation = false;
	private GameObject playerObj;
  	private AudioSource talkSrc;
	private GameObject dSystem;	//reference to the dialogue system

	// Use this for initialization
	void Awake () {
		dialogue = new Interaction (dialogueFile);
		if (dialogueFile.name.IndexOf('.') != -1)
			npcName = dialogueFile.name.Substring(0, dialogueFile.name.IndexOf ('.'));
		else
			npcName = dialogueFile.name;
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
		//manually check if player is in bounds
		//if (playerObj != null &&
		//    playerObj.transform.position.x > transform.position.x + GetComponent<BoxCollider2D>().center.x - GetComponent<BoxCollider2D>()

		if (Input.GetButtonDown ("Action") && (inTrigger || inConversation))
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
			if (inConversation)
			{
				if (dSystem.GetComponent<DialogueSystem>().IsAnimating())
					dSystem.GetComponent<DialogueSystem>().StepAnimation();
				else
				{
					List<string> lines = dialogue.Step(dSystem.GetComponent<DialogueSystem>().GetCursor());
					if(lines.Count > 0)
					{
						string name = npcName;
						if (lines[0].IndexOf(':') != -1)
						{
							name = lines[0].Substring(0, lines[0].IndexOf(':'));	//the colon is reserved for specifying speaker manually
							lines[0] = lines[0].Substring(lines[0].IndexOf(':')+1);
						}
						dSystem.GetComponent<DialogueSystem>().PushNPCText(lines[0], transform.position, name);
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
