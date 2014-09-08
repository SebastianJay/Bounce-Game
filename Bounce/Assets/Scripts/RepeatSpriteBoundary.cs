using UnityEngine;
using System.Collections;

// @NOTE the attached sprite's position should be "top left" or the children will not align properly
// Strech out the image as you need in the sprite render, the following script will auto-correct it when rendered in the game
[RequireComponent (typeof (SpriteRenderer))]

// Generates a nice set of repeated sprites inside a streched sprite renderer
// @NOTE Vertical only, you can easily expand this to horizontal with a little tweaking
public class RepeatSpriteBoundary : MonoBehaviour {
	SpriteRenderer sprite;
	
	void Awake () {
		// Get the current sprite with an unscaled size
		sprite = GetComponent<SpriteRenderer>();
		Vector2 spriteSize = new Vector2(sprite.bounds.size.x / transform.localScale.x, sprite.bounds.size.y / transform.localScale.y);
		
		// Generate a child prefab of the sprite renderer
		GameObject childPrefab = new GameObject();
		SpriteRenderer childSprite = childPrefab.AddComponent<SpriteRenderer>();
		float startX = transform.position.x - (sprite.bounds.size.x / 2) + (spriteSize.x / 2);
		childPrefab.transform.position = new Vector3(startX, transform.position.y, transform.position.z);
		childPrefab.transform.localScale = new Vector3(1, transform.localScale.y, 1);
		childSprite.sprite = sprite.sprite;
		
		// Loop through and spit out repeated tiles
		GameObject child;
		//print (sprite.bounds.size.x);
		//print (spriteSize.x + " " + spriteSize.y);
		for (int i = 1, l = (int)Mathf.Round(transform.localScale.x); i < l; i++) {
			child = Instantiate(childPrefab) as GameObject;
			child.transform.position = new Vector3(startX +  spriteSize.x * i, transform.position.y, transform.position.z);
			child.transform.parent = transform;
		}

		// Set the parent last on the prefab to prevent transform displacement
		childPrefab.transform.parent = transform;
		
		// Disable the currently existing sprite component since its now a repeated image
		sprite.enabled = false;
	}
}