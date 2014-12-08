using UnityEngine;
using System.Collections.Generic;

public class Interactable : MonoBehaviour {

	public TextAsset dialogueFile;
	private Interaction dialogue;
	public TextMesh gText;
	public AudioClip talkNoise;
	public float talkVolume = 1f;

	private bool inTrigger = false;
	private GameObject playerObj;
	private GameObject NPC;
	private bool inConveration = false;
  private AudioSource talkSrc;

	// Use this for initialization
	void Awake () {
		dialogue = new Interaction (dialogueFile);
		if (talkNoise != null)
		{
			talkSrc = gameObject.AddComponent<AudioSource>();
			talkSrc.clip = talkNoise;
			talkSrc.volume = talkVolume;
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Action") && inTrigger)
		{
			PlayerBallControl bScript = playerObj.GetComponent<PlayerBallControl>();
			if (!inConveration)
			{
				inConveration = true;
				if (talkSrc != null)
					talkSrc.Play();
			}
			List<string> lines = dialogue.Step(playerObj.GetComponent<DialogueResponses>().cursor);
			if(lines.Count > 0)
			{
				gText.text = lines[0];
				List<string> heroLines = lines.GetRange(1, lines.Count - 1);
				playerObj.GetComponent<DialogueResponses>().SetResponses(heroLines);
				playerObj.rigidbody2D.velocity = Vector2.zero;
				playerObj.rigidbody2D.angularVelocity = 0f;
				formatter ();
			}
			else
			{
				gText.text = "";
				inConveration = false;
				bScript.playerLock = false;
			}
			if (inConveration == true) {
				bScript.playerLock = true;
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
	void formatter(){
		if (gText.text.Length > 15){
			//Only line breaks if the string is large enough
			int index = 15;
			//Starts the loop to begin scanning at 15 to mitigate unrequiered scanning
			while (index < (gText.text.Length)){
				//Starts to scan the entire string for spaces after
				//15 characters have been reached to look for a good spot for a line break
				if (gText.text[index] == ' '){
					//Finds only a space to insert the line break

					char[] carray = gText.text.ToCharArray();
					carray[index] = '\n';
					gText.text = new string(carray);
					//Best way to replace a single character in a string I could find

					if (NPC.transform.childCount > 0)
						NPC.transform.GetChild(0).transform.localPosition = new Vector3(-0.733f, 0.911f, 0f);
					//Resets to original posistion
					Vector3 addY = new Vector3(0f, 1f);
					//Decides how much to increase height of text
					if (NPC.transform.childCount > 0)
						NPC.transform.GetChild(0).transform.localPosition += addY;

					index += 15;
					//Prepares a new line to be added after another 15 characters
				}
				index += 1;
			}
		}

		/*float BoxX, BoxY;
		BoxX = NPC.transform.GetChild (0).transform.position.x;
		int BoxX1 = (int) BoxX;
		BoxY = NPC.transform.GetChild (0).transform.position.y;
		int BoxY1 = (int)BoxY;
		Texture2D texture = new Texture2D(BoxX1, BoxY1);
		int y = 0;
		while (y < texture.height) {
			int x = 0;
			while (x < texture.width) {
				Color color = Color.white;
				texture.SetPixel(x, y, color);
				++x;
			}
			++y;
		}
		texture.Apply();
		GUI.skin.box.normal.background = texture;
		//GUI.Box(, GUIContent.none);
		*/
		}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			//animation appears to show you can talk
			inTrigger = true;
			playerObj = other.gameObject;
			NPC = this.gameObject;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "Player") {
			//animation disappears
			inTrigger = false;
		}
	}
}
