using UnityEngine;
using System.Collections.Generic;

public class TeleporterTarget : MonoBehaviour {

	public int targetID;
	public static Dictionary<int,Vector3> teleporterTargetTable = new Dictionary<int, Vector3>();
	public static int teleportTarget = 0;

	void Awake()
	{
		if (!teleporterTargetTable.ContainsKey (targetID)) {
			teleporterTargetTable.Add (targetID, this.transform.position);
		}
	}
}
