using UnityEngine;
using System.Collections;

// Strech out the image as you need in the sprite render, the following script will auto-correct it when rendered in the game
[RequireComponent (typeof (SpriteRenderer))]

// Generates a nice set of repeated sprites inside a streched sprite renderer
public class TileSprite : MonoBehaviour {
	public int tileX = 1;
	public int tileY = 1;
	public float scaleX = 1f;
	public float scaleY = 1f;
	public bool useScaleAsTile = true;	//if true, scale will be (1, 1) and scale of object will be used as tile factor
										//if false, the other fields in the script will be used
	public float zrot = 0f;

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
		//Quaternion prevRot = transform.localRotation;
		//transform.rotation = Quaternion.identity;
		//Vector3 prevPos = transform.position;
		//transform.RotateAround (transform.position, new Vector3 (0f, 0f, 1f), -transform.rotation.eulerAngles.z);
		//transform.position = prevPos;
		/*Debug.Log (gameObject.name + " " + sprite.bounds.size.x + " " + sprite.bounds.size.y + " " + 
		           absScale.x + " " + absScale.y + " " +
		           spriteSize.x + " " + spriteSize.y);*/
		
		// Generate a child prefab of the sprite renderer
		GameObject childPrefab = new GameObject();
		SpriteRenderer childSprite = childPrefab.AddComponent<SpriteRenderer>();
		Vector3 desiredScale = Vector3.one;
		if (useScaleAsTile)
		{
			tileX = Mathf.RoundToInt(absScale.x);
			tileY = Mathf.RoundToInt(absScale.y);
			desiredScale = new Vector3(1, 1, 1);
		}
		else
			desiredScale = new Vector3(scaleX, scaleY, 1);
		float startX = transform.position.x - (sprite.bounds.size.x / 2) + (spriteSize.x * desiredScale.x) / 2;
		float startY = transform.position.y - (sprite.bounds.size.y / 2) + (spriteSize.y * desiredScale.y) / 2;

		childPrefab.transform.position = new Vector3(startX, startY, transform.position.z);
		childPrefab.transform.rotation = transform.rotation;
		childPrefab.transform.localScale = desiredScale;
		//childPrefab.transform.localRotation = prevRot;
		childSprite.sprite = sprite.sprite;
		childSprite.sortingLayerName = sprite.sortingLayerName;
		childSprite.sortingOrder = sprite.sortingOrder;
		childSprite.color = sprite.color;

		//Debug.Log (tileX + " " + tileY);
		// Loop through and spit out repeated tiles
		GameObject child;
		for (int i = 0; i < tileX; i++) 
		{
			for (int j = 0; j < tileY; j++)
			{
				if (i == 0 && j == 0) continue;
				child = Instantiate(childPrefab) as GameObject;
				child.transform.position = new Vector3(startX + spriteSize.x * desiredScale.x * i, 
				                                       startY + spriteSize.y * desiredScale.y * j, 
				                                       transform.position.z);
				child.transform.parent = transform;
			}
		}

		// Set the parent last on the prefab to prevent transform displacement
		childPrefab.transform.parent = transform;

		//transform.localRotation = prevRot;
		transform.RotateAround(transform.position, new Vector3 (0f, 0f, 1f), zrot);

		// Disable the currently existing sprite component since it's now a repeated image
		sprite.enabled = false;
	}
}