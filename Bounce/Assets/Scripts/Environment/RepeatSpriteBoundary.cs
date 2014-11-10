using UnityEngine;
using System.Collections;

// Strech out the image as you need in the sprite render, the following script will auto-correct it when rendered in the game
[RequireComponent (typeof (SpriteRenderer))]

// Generates a nice set of repeated sprites inside a streched sprite renderer
public class RepeatSpriteBoundary : MonoBehaviour {
	public int tileX = 1;
	public int tileY = 1;
	public float scaleX = 1f;
	public float scaleY = 1f;
	public bool useScaleAsTile = true;

	SpriteRenderer sprite;

	Vector2 GetAbsoluteScale()
	{
		Vector2 retVal = Vector2.one;
		Transform trans = this.transform;
		while (trans != null)
		{
			retVal.x *= trans.localScale.x;
			retVal.y *= trans.localScale.y;
			trans = trans.parent;
		}
		return retVal;
	}

	void Awake () {
		// Get the current sprite with an unscaled size
		sprite = GetComponent<SpriteRenderer>();
		Vector2 absScale = GetAbsoluteScale();
		Vector2 spriteSize = new Vector2(sprite.bounds.size.x / absScale.x, sprite.bounds.size.y / absScale.y);
		/*Debug.Log (gameObject.name + " " + sprite.bounds.size.x + " " + sprite.bounds.size.y + " " + 
		           absScale.x + " " + absScale.y + " " +
		           spriteSize.x + " " + spriteSize.y);*/
		
		// Generate a child prefab of the sprite renderer
		GameObject childPrefab = new GameObject();
		SpriteRenderer childSprite = childPrefab.AddComponent<SpriteRenderer>();
		float startX = transform.position.x - (sprite.bounds.size.x / 2) + (spriteSize.x / 2);
		float startY = transform.position.y - (sprite.bounds.size.y / 2) + (spriteSize.y / 2);
		childPrefab.transform.position = new Vector3(startX, startY, transform.position.z);
		if (useScaleAsTile)
			childPrefab.transform.localScale = new Vector3(1, 1, 1);
		else
			childPrefab.transform.localScale = new Vector3(scaleX, scaleY, 1);
		childSprite.sprite = sprite.sprite;
		childSprite.sortingLayerName = sprite.sortingLayerName;
		
		// Loop through and spit out repeated tiles
		GameObject child;
		for (int i = 0; i < tileX; i++) 
		{
			for (int j = 0; j < tileY; j++)
			{
				//if (i == 0 && j == 0) continue;
				child = Instantiate(childPrefab) as GameObject;
				child.transform.position = new Vector3(startX + spriteSize.x * i, 
				                                       startY + spriteSize.y * j, 
				                                       transform.position.z);
				child.transform.parent = transform;
			}
		}

		// Set the parent last on the prefab to prevent transform displacement
		childPrefab.transform.parent = transform;
		
		// Disable the currently existing sprite component since it's now a repeated image
		sprite.enabled = false;
	}
}