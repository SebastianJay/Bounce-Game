using UnityEngine;
using System.Collections.Generic;

//Stick this to an empty game object and tag it with "DialogueSystem"
//so the Interactable objects can use it to push dialogue text to the scene
public class DialogueSystem : MonoBehaviour {

	public Transform backgroundPrefab;
	public Transform framePrefab;
	public Transform textPrefab;
	public Color npcBgColor = Color.white;
	public Color playerBgColor = Color.cyan;
	public Color frameColor = Color.black;
	public Color npcTextColor = Color.black;
	public Color playerRespNormalColor = Color.black;
	public Color playerRespHighlightColor = Color.yellow;
	public float frameThickness = 0.3f;
	public float bgMargin = 0.3f;

	public Vector2 npcOffset;
	public Vector2 playerOffset;
	public int wordWrapCharCount = 25;
	public float animateSpeed = 0.03f;	//seconds per character for NPC text scrolling
	public float responseSpeed = 0.5f;	//time between NPC line and player response appearing
	public float punctPauseMargin = 0.02f;	//additional pause for punctuation marks

	//animation
	private int cursor;
	private bool animating = false;
	private bool settingUpNPCText = false;
	private bool settingUpPlayerText = false;
	private bool waitingForResponse = false;
	private int animateIndex = 0;
	private float animateTimer = 0f;
	private const string puncString = ".,!?:;";
	private float originalAnimateSpeed;
	//npc vars
	private GameObject npcContainer;
	private Transform npcTextObj;
	private string npcName;
	private string npcText;
	private Rect npcTextBounds;
	private Transform newParent;
	//player vars
	private GameObject playerContainer;
	private string playerText;
	private List<Transform> playerTextObjLst = new List<Transform>();

	void Start() {
		originalAnimateSpeed = animateSpeed;
	}

	// Update is called once per frame
	void Update () {
		if (animating)
		{
			animateTimer += Time.deltaTime;
			if (settingUpNPCText)	//first phase
			{
				if (animateTimer >= animateSpeed)
				{
					animateIndex++;
					animateTimer -= animateSpeed;
					if (puncString.Contains(""+npcText[animateIndex-1]))
						animateSpeed = originalAnimateSpeed + punctPauseMargin;
					else
						animateSpeed = originalAnimateSpeed;
					if (animateIndex <= npcText.Length)
						npcTextObj.GetComponent<TextMesh>().text = npcName + ":\n" + npcText.Substring(0, animateIndex);
					else
						npcTextObj.GetComponent<TextMesh>().text = npcName + ":\n" + npcText;
				}
				if (animateIndex >= npcText.Length)
				{
					settingUpNPCText = false;
					if (playerContainer != null) {
						if (playerTextObjLst.Count == 1) {
							settingUpPlayerText = true;
						}
						else
							waitingForResponse = true;
					}
					else
						animating = false;
					animateTimer = 0f;
					animateIndex = 0;
					animateSpeed = originalAnimateSpeed;
				}
			}
			else if (settingUpPlayerText) 
			{
				Transform playerTextObj = playerTextObjLst[0];
				if (!playerContainer.activeSelf) {
					playerContainer.SetActive(true);
					playerTextObj.GetComponent<TextMesh>().text = "";
				}
				if (animateTimer > animateSpeed)
				{
					animateIndex++;
					animateTimer -= animateSpeed;
					if (puncString.Contains(""+playerText[animateIndex-1]))
						animateSpeed = originalAnimateSpeed + punctPauseMargin;
					else
						animateSpeed = originalAnimateSpeed;

					if (animateIndex <= playerText.Length)
						playerTextObj.GetComponent<TextMesh>().text = playerText.Substring(0, animateIndex);
					else
						playerTextObj.GetComponent<TextMesh>().text = playerText;
				}
				if (animateIndex >= playerText.Length)
				{
					settingUpPlayerText = false;
					animating = false;
					//waitingForResponse = true;
					animateTimer = 0f;
					animateSpeed = originalAnimateSpeed;
				}

			}
			else if (waitingForResponse)	//second phase
			{
				if (animateTimer >= responseSpeed)
				{
					playerContainer.SetActive(true);
					animateTimer = 0f;
					animating = false;
					waitingForResponse = false;
				}
			}
		}
	}

	public void PushNPCText(string text, Vector2 position, string name = "NPC")
	{
		//destroy the existing dialogue box
		if (npcContainer != null)
			Destroy(npcContainer);
		if (text.Length == 0) {
			npcTextBounds = new Rect(0f, 1e11f, 0f, 0f);	//hack to get the player text to be placed high
			return;	//don't do anything for empty string
		}
		//Draw the text
		npcName = name;
		npcText = text;
		npcTextObj = Instantiate (textPrefab) as Transform;
		int lineCount = 1;
		FormatText (ref npcText, ref lineCount); 
		npcTextObj.GetComponent<TextMesh> ().text = npcName + ":\n" + npcText;
		npcTextObj.GetComponent<TextMesh> ().color = npcTextColor;
		npcTextObj.GetComponent<TextMesh> ().anchor = TextAnchor.UpperLeft;
		Vector2 textSize = new Vector2 (npcTextObj.GetComponent<MeshRenderer> ().bounds.size.x,
                               			npcTextObj.GetComponent<MeshRenderer> ().bounds.size.y);
		npcTextObj.position = position + npcOffset + new Vector2(0, +textSize.y / 2);
		npcContainer = CreateTextBox (npcTextObj.position + new Vector3(textSize.x / 2, -textSize.y / 2), 
		                              textSize, npcBgColor);
		npcTextObj.parent = npcContainer.transform;
		if (newParent != null) {
			npcContainer.transform.parent = newParent;
			newParent = null;
		} 
		else
			npcContainer.transform.parent = this.transform;
		npcTextBounds = new Rect(npcTextObj.position.x - textSize.x,
		                         npcTextObj.position.y - textSize.y,
		                         textSize.x, textSize.y);

		//indicate animation can start
		npcTextObj.GetComponent<TextMesh> ().text = npcName + ":\n";
		animating = true;
		animateIndex = 0;
		settingUpNPCText = true;
	}

	//to make NPC text move with the NPC on ONLY the next frame -- limited use
	public void ReparentNPCText(Transform nParent) {
		newParent = nParent;
	}

	public void PushPlayerText(List<string> textLst, Vector2 position)
	{
		//destroy the existing dialogue box
		if (playerContainer != null)
		{
			Destroy (playerContainer);
			playerTextObjLst.Clear();//remove dangling references
		}
		if (textLst.Count == 0)
			return;

		playerTextObjLst.Clear();
		Vector2 textSize = Vector2.zero;
		int lineCount = 0;
		foreach (string text in textLst)
		{
			lineCount++;
			string cpy = text.Trim();
			if (textLst.Count == 1) {
				FormatText(ref cpy, ref lineCount);
				playerText = cpy;
			}
			Transform playerTextObj = Instantiate(textPrefab) as Transform;
			playerTextObj.GetComponent<TextMesh>().text = cpy;
			playerTextObj.GetComponent<TextMesh>().color = playerRespNormalColor;
			textSize.y += playerTextObj.GetComponent<MeshRenderer>().bounds.size.y;
			textSize.x = Mathf.Max(textSize.x, playerTextObj.GetComponent<MeshRenderer>().bounds.size.x);
			playerTextObjLst.Add(playerTextObj);
		}
		position = position + playerOffset + new Vector2(textSize.x / 2, +textSize.y / 2);
		if (npcContainer != null)
			position.y = Mathf.Min (position.y, npcTextBounds.yMin - bgMargin - frameThickness / 2 - textSize.y / 2);//prevent overlap
		playerContainer = CreateTextBox(position, textSize, playerBgColor);
		int i = 0;
		foreach (Transform textObj in playerTextObjLst)
		{
			textObj.parent = playerContainer.transform;
			textObj.GetComponent<TextMesh>().anchor = TextAnchor.UpperLeft;
			textObj.position = new Vector3(position.x - textSize.x / 2, 
			                               position.y + textSize.y / 2 - (i * textSize.y / playerTextObjLst.Count), 0f);
			i++;
		}
		playerContainer.transform.parent = this.transform;

		//set up the animation
		cursor = 0;
		if (playerTextObjLst.Count > 1)
			playerTextObjLst[0].GetComponent<TextMesh>().color = playerRespHighlightColor;	//current selection
		playerContainer.SetActive(false);
		if (!settingUpNPCText) {
			animating = true;
			animateIndex = 0;
			settingUpPlayerText = true;	//if no NPC text for this interaction, jump straight to player text
		}
	}

	public int GetCursor()
	{
		return cursor;
	}

	public void MoveCursor(bool up)
	{
		int numChoices = playerTextObjLst.Count;
		if (numChoices <= 1)
			return;
		playerTextObjLst[cursor].GetComponent<TextMesh>().color = playerRespNormalColor;
		if (up)
		{
			cursor--;
			if (cursor < 0)
				cursor = numChoices - 1;
		}
		else //otherwise it's down!
		{
			cursor = (cursor + 1) % numChoices;
		}
		playerTextObjLst[cursor].GetComponent<TextMesh>().color = playerRespHighlightColor;
	}

	public bool IsAnimating()
	{
		return animating;
	}

	public void StepAnimation()
	{
		//finish the animation manually
		if (npcTextObj != null)
			npcTextObj.GetComponent<TextMesh>().text = npcName + ":\n" + npcText;
		if (playerContainer != null) {
			playerContainer.SetActive(true);
			if (playerTextObjLst.Count == 1)
				playerTextObjLst[0].GetComponent<TextMesh>().text = playerText;
		}
		//reset all the flags
		animating = false;
		animateTimer = 0f;
		animateIndex = 0;
		settingUpNPCText = false;
		settingUpPlayerText = false;
		waitingForResponse = false;
	}
	
	public void EndConversation()
	{
		if (playerContainer != null)
			Destroy(playerContainer);
		if (npcContainer != null)
			Destroy (npcContainer);
		playerTextObjLst.Clear();
		animating = false;
		animateTimer = 0f;
		animateIndex = 0;
		settingUpNPCText = false;
		settingUpPlayerText = false;
		waitingForResponse = false;
	}

	private GameObject CreateTextBox(Vector3 textPosition, Vector2 textDimensions, Color color)
	{
		//Draw the text box
		Transform bg = Instantiate (backgroundPrefab) as Transform;
		bg.position = textPosition;
		bg.GetComponent<SpriteRenderer>().color = color;
		Vector2 bgSpriteSize = new Vector2 (bg.GetComponent<SpriteRenderer> ().bounds.size.x / bg.localScale.x,
		                                  bg.GetComponent<SpriteRenderer> ().bounds.size.y / bg.localScale.y);
		bg.localScale = new Vector3 ((textDimensions.x + bgMargin) / bgSpriteSize.x,
		                             (textDimensions.y + bgMargin) / bgSpriteSize.y, 1f);

		//Draw the frame for the box
		Transform leftBorder = Instantiate (framePrefab) as Transform;
		Transform rightBorder = Instantiate (framePrefab) as Transform;
		Transform upBorder = Instantiate (framePrefab) as Transform;
		Transform downBorder = Instantiate (framePrefab) as Transform;
		Vector2 borderSpriteSize = new Vector2 (leftBorder.GetComponent<SpriteRenderer> ().bounds.size.x / leftBorder.localScale.x,
		                                        leftBorder.GetComponent<SpriteRenderer> ().bounds.size.y / leftBorder.localScale.y);

		leftBorder.position = new Vector2(bg.position.x - textDimensions.x / 2 - bgMargin / 2, bg.position.y);
		rightBorder.position = new Vector2(bg.position.x + textDimensions.x / 2 + bgMargin / 2, bg.position.y);
		upBorder.position = new Vector2(bg.position.x, bg.position.y + textDimensions.y / 2 + bgMargin / 2);
		downBorder.position = new Vector2(bg.position.x, bg.position.y - textDimensions.y / 2 - bgMargin / 2);
		leftBorder.localScale = new Vector3(frameThickness, (textDimensions.y + bgMargin) / borderSpriteSize.y, 1f);
		rightBorder.localScale = new Vector3(frameThickness, (textDimensions.y + bgMargin) / borderSpriteSize.y, 1f);
		upBorder.localScale = new Vector3((textDimensions.x + bgMargin) / borderSpriteSize.x, frameThickness, 1f);
		downBorder.localScale = new Vector3((textDimensions.x + bgMargin) / borderSpriteSize.x, frameThickness, 1f);
		leftBorder.GetComponent<SpriteRenderer>().color = frameColor;
		rightBorder.GetComponent<SpriteRenderer>().color = frameColor;
		upBorder.GetComponent<SpriteRenderer>().color = frameColor;
		downBorder.GetComponent<SpriteRenderer>().color = frameColor;
		leftBorder.GetComponent<SpriteRenderer>().sortingOrder = bg.GetComponent<SpriteRenderer>().sortingOrder + 1;
		rightBorder.GetComponent<SpriteRenderer>().sortingOrder = bg.GetComponent<SpriteRenderer>().sortingOrder + 1;
		upBorder.GetComponent<SpriteRenderer>().sortingOrder = bg.GetComponent<SpriteRenderer>().sortingOrder + 1;
		downBorder.GetComponent<SpriteRenderer>().sortingOrder = bg.GetComponent<SpriteRenderer>().sortingOrder + 1;

		//now make a container for all this
		GameObject container = new GameObject("DialogueBox");	
		bg.parent = container.transform;
		leftBorder.parent = container.transform;
		rightBorder.parent = container.transform;
		upBorder.parent = container.transform;
		downBorder.parent = container.transform;

		return container;
	}

	private void FormatText(ref string text, ref int lineCount){
		text = text.Trim();
		//Only line breaks if the string is large enough
		if (text.Length > wordWrapCharCount){
			//Starts the loop to begin scanning at wrap count to mitigate unrequiered scanning
			int index = wordWrapCharCount;
			//Starts to scan the entire string for spaces after
			while (index < (text.Length)){
				//Finds only a space to insert the line break
				if (text[index] == ' '){

					char[] carray = text.ToCharArray();
					carray[index] = '\n';
					text = new string(carray);
					lineCount++;

					//Prepares a new line to be added after another 15 characters
					index += wordWrapCharCount;
				}
				index += 1;
			}
		}
	}
}