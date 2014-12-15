using UnityEngine;
using System.Collections.Generic;

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

	private int cursor;
	private Rect npcTextBounds;
	private List<Transform> textObjLst = new List<Transform>();
	private GameObject npcLine;
	private GameObject playerLine;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void PushNPCText(string text, Vector2 position)
	{
		//destroy the existing dialogue box
		if (npcLine != null)
			Destroy(npcLine);

		//Draw the text
		Transform npcText = Instantiate (textPrefab) as Transform;
		int lineCount = 1;
		FormatText (ref text, ref lineCount); 
		npcText.GetComponent<TextMesh> ().text = text;
		npcText.GetComponent<TextMesh> ().color = npcTextColor;
		npcText.GetComponent<TextMesh> ().anchor = TextAnchor.MiddleCenter;
		Vector2 textSize = new Vector2 (npcText.GetComponent<MeshRenderer> ().bounds.size.x,
                               			npcText.GetComponent<MeshRenderer> ().bounds.size.y);
		npcText.position = position + npcOffset + new Vector2(textSize.x / 2, -textSize.y / 2);
		npcLine = CreateTextBox (npcText.position, textSize, npcBgColor);
		npcText.parent = npcLine.transform;
		npcLine.transform.parent = this.transform;
		npcTextBounds = new Rect(npcText.position.x - textSize.x / 2,
		                         npcText.position.y - textSize.y / 2,
		                         textSize.x, textSize.y);
	}

	public void PushPlayerText(List<string> textLst, Vector2 position)
	{
		//destroy the existing dialogue box
		if (playerLine != null)
		{
			Destroy (playerLine);
			textObjLst.Clear();//remove dangling references
		}
		if (textLst.Count == 0)
			return;

		textObjLst.Clear();
		Vector2 textSize = Vector2.zero;
		int lineCount = 0;
		foreach (string text in textLst)
		{
			lineCount++;
			Transform playerText = Instantiate(textPrefab) as Transform;
			playerText.GetComponent<TextMesh>().text = text.Trim();
			playerText.GetComponent<TextMesh>().color = playerRespNormalColor;
			textSize.y += playerText.GetComponent<MeshRenderer>().bounds.size.y;
			textSize.x = Mathf.Max(textSize.x, playerText.GetComponent<MeshRenderer>().bounds.size.x);
			textObjLst.Add(playerText);
		}
		position = position + playerOffset + new Vector2(textSize.x / 2, -textSize.y / 2);
		if (npcLine != null)
			position.y = Mathf.Min (position.y, npcTextBounds.yMin - bgMargin - frameThickness / 2 - textSize.y / 2);//prevent overlap
		playerLine = CreateTextBox(position, textSize, playerBgColor);
		int i = 0;
		foreach (Transform textObj in textObjLst)
		{
			textObjLst[i].parent = playerLine.transform;
			textObjLst[i].GetComponent<TextMesh>().anchor = TextAnchor.UpperLeft;
			textObjLst[i].position = new Vector3(position.x - textSize.x / 2, 
			                                     position.y + textSize.y / 2 - (i * textSize.y / textObjLst.Count), 0f);
			i++;
		}
		playerLine.transform.parent = this.transform;

		cursor = 0;
		if (textObjLst.Count > 1)
			textObjLst[0].GetComponent<TextMesh>().color = playerRespHighlightColor;	//current selection
	}

	public int GetCursor()
	{
		return cursor;
	}

	public void MoveCursor(bool up)
	{
		int numChoices = textObjLst.Count;
		if (numChoices <= 1)
			return;
		textObjLst[cursor].GetComponent<TextMesh>().color = Color.black;
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
		textObjLst[cursor].GetComponent<TextMesh>().color = Color.yellow;
	}

	public void EndConversation()
	{
		if (playerLine != null)
			Destroy(playerLine);
		if (npcLine != null)
			Destroy (npcLine);
		textObjLst.Clear();
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