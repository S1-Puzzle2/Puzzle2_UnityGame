using UnityEngine;
using System.Collections;

public class GridColliderController : MonoBehaviour {

	GameObject currentTile;

	public GameObject getCurrentTile() {
		return currentTile;
	}

	public void setCurrentTile(GameObject tile) {
		this.currentTile = tile;
	}

	public bool isGridColliderOccupied() {
		return currentTile != null;
	}

}
