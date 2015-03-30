using UnityEngine;
using System.Collections;

public class AccessoryManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	// as of now, only one accessory can be equipped
	// we can change this if we want later
	public void SetAccessory(ItemType item) {
		GameObject obj;
		if (transform.childCount > 0) {
			obj = transform.GetChild(0).gameObject;
		}
		else {
			obj = new GameObject("accessory");
			SpriteRenderer rend = obj.AddComponent<SpriteRenderer>();
			rend.sortingLayerName = "Character";
			rend.sortingOrder = 1;
			obj.transform.parent = transform;
		}

		obj.GetComponent<SpriteRenderer>().sprite = ImmutableData.GetItemData()[item].image;
		Vector2 pos = ImmutableData.GetItemData()[item].localPosition;
		Quaternion rot = ImmutableData.GetItemData()[item].localRotation;
		obj.transform.localPosition = new Vector3(pos.x, pos.y, 0f);
		obj.transform.localScale = new Vector3(1f, 1f, 1f);
		obj.transform.localRotation = rot;
		PlayerDataManager.itemEquipped = item;
	}

	public void RemoveAccessory() {
		if (transform.childCount > 0) {
			Destroy (transform.GetChild(0).gameObject);
		}
		PlayerDataManager.itemEquipped = ItemType.None;
	}
}
