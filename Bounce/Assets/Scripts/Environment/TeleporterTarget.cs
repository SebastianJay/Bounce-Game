using UnityEngine;
using System.Collections;

public class TeleporterTarget : MonoBehaviour {

	public int targetID;

	void Awake()
	{
		if (!LevelTeleporter.teleporterTargetTable.ContainsKey (targetID)) {
			LevelTeleporter.teleporterTargetTable.Add (targetID, this.transform.position);
		}
	}
}
