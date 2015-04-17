using UnityEngine;
using System.Collections.Generic;

//[RequireComponent (typeof(BoxCollider2D))]
public class Interactable : MonoBehaviour {

	public TextAsset dialogueFile;
	private Interaction dialogue;
	public Transform talkBubblePrefab;
	public float talkBubbleOffset = 1.6f;
	public AudioClip talkNoise;
	public float talkVolume = 1f;
	public float cameraOrthoThreshold = 8f;
	public Color npcBoxColor = Color.white;

	private string npcName;
	private bool inTrigger = false;
	private bool inConversation = false;
	private GameObject playerObj;
  	private AudioSource talkSrc;
	private GameObject dSystem;	//reference to the dialogue system
	private GameObject cam;
	private Transform talkBubble;
	private static int endedTalkFrame = -1;
	private CameraFollowConfig lastConfig;
	private bool changedCamConfig = false;

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
		cam = GameObject.FindGameObjectWithTag("MainCamera");
		playerObj = GameObject.FindGameObjectWithTag("Player");
		if (dSystem == null)
			Debug.LogError("No dialogue system present in scene!");
	}

	// Update is called once per frame
	void Update () {
		if (playerObj == null)
			playerObj = GameObject.FindGameObjectWithTag ("Player");
		if (Input.GetButtonDown ("Action") && (inTrigger || inConversation) 
		    && Time.frameCount != endedTalkFrame && !DialogueConstantParser.eventLock)
		{
			StepConvo();
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

	public void StepConvo() {
		bool flag = false;
		if (playerObj.GetComponent<PlayerBallControl>() != null) {
			PlayerBallControl bScript = playerObj.GetComponent<PlayerBallControl>(); 
			flag = bScript.inConversation;
			if (!flag) {
				bScript.inConversation = true;
				bScript.npcTalker = transform;
				bScript.playerLock = true;
			}
		} else if (playerObj.GetComponent<PlayerBodyControl>() != null) {
			PlayerBodyControl bScript = playerObj.GetComponent<PlayerBodyControl>(); 
			flag = bScript.inConversation;
			if (!flag) {
				bScript.inConversation = true;
				bScript.npcTalker = transform;
				bScript.playerLock = true;
			}
		}
		if (!flag)
		{
			this.inConversation = true;
			if (talkSrc != null)
				talkSrc.Play();
			if (talkBubble != null)
				talkBubble.gameObject.SetActive(false);
			if (cam.GetComponent<Camera>().orthographicSize > cameraOrthoThreshold) {
				lastConfig = cam.GetComponent<CameraFollow>().GetConfig();
				CameraFollowConfig camConfig = new CameraFollowConfig();
				camConfig.position = transform.position;
				camConfig.lockedPosition = transform.position;
				camConfig.isLocked = true;
				camConfig.orthoSize = cameraOrthoThreshold;
				cam.GetComponent<CameraFollow>().LoadConfig(camConfig, false);
				changedCamConfig = true;
			}
			else
				changedCamConfig = false;
			dSystem.GetComponent<DialogueSystem>().npcBgColor = npcBoxColor;
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
					dSystem.GetComponent<DialogueSystem>().PushPlayerText(lines.GetRange(1, lines.Count - 1), playerObj.transform.position);
					playerObj.rigidbody2D.velocity = Vector2.zero;
					playerObj.rigidbody2D.angularVelocity = 0f;
				}
				else
				{
					dSystem.GetComponent<DialogueSystem>().EndConversation();
					this.inConversation = false;
					if (playerObj.GetComponent<PlayerBallControl>() != null) {
						PlayerBallControl bScript = playerObj.GetComponent<PlayerBallControl>();
						bScript.inConversation = false;
						bScript.playerLock = false;
					} else if (playerObj.GetComponent<PlayerBodyControl>() != null) {
						PlayerBodyControl bScript = playerObj.GetComponent<PlayerBodyControl>();
						bScript.inConversation = false;
						bScript.playerLock = false;
					}
					endedTalkFrame = Time.frameCount;
					if (talkBubble != null)
						talkBubble.gameObject.SetActive(true);
					if (changedCamConfig)
						cam.GetComponent<CameraFollow>().LoadConfig(lastConfig, false);
				}
			}
		}
	}

	public void ForceQuitConvo() {
		dSystem.GetComponent<DialogueSystem>().EndConversation();
		this.inConversation = false;
		if (playerObj.GetComponent<PlayerBallControl>() != null) {
			PlayerBallControl bScript = playerObj.GetComponent<PlayerBallControl>();
			bScript.inConversation = false;
			bScript.npcTalker = null;
			bScript.playerLock = false;
		} else if (playerObj.GetComponent<PlayerBodyControl>() != null) {
			PlayerBodyControl bScript = playerObj.GetComponent<PlayerBodyControl>();
			bScript.inConversation = false;
			bScript.npcTalker = null;
			bScript.playerLock = false;
		}
		endedTalkFrame = Time.frameCount;
		if (talkBubble != null)
			talkBubble.gameObject.SetActive(true);
		if (changedCamConfig)
			cam.GetComponent<CameraFollow>().LoadConfig(lastConfig, false);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			//animation appears to show you can talk
			if (talkBubblePrefab != null && talkBubble == null && !inConversation) {
				talkBubble = Instantiate(talkBubblePrefab, 
				                         transform.position + new Vector3(0f, talkBubbleOffset, 0f), 
				                         Quaternion.identity) as Transform;
				talkBubble.parent = transform;
			}
			inTrigger = true;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "Player") {
			//animation disappears
			if (talkBubble != null)
				Destroy (talkBubble.gameObject);
			inTrigger = false;
		}
	}
}
