using UnityEngine;
using System.Collections;

public class MeshSortingLayer : MonoBehaviour {
	
	public string sortingLayer;
	public int sortingOrder;
	// Use this for initialization
	void Start () {
		GetComponent<MeshRenderer> ().sortingLayerName = sortingLayer;
		GetComponent<MeshRenderer> ().sortingOrder = sortingOrder;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
