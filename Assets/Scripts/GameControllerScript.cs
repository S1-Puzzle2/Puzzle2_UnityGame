using UnityEngine;
using System.Collections;
using System.Linq;

public class GameControllerScript : MonoBehaviour {

	public Sprite[] puzzleTextures;
	public GameObject puzzlePiecePrefab;
	public TwoDBoundaries boundaries;
	public float snapDistance;

	private GameObject[] puzzlePieces;
	private GameObject[] gridColliders;
	private bool notEnoughColliders;

	private bool puzzleOver;

	void Start() {

		puzzleOver = false;
		gridColliders = GameObject.FindGameObjectsWithTag ("GridCollider").OrderBy (go => go.name).ToArray ();

		if (gridColliders.Length < puzzleTextures.Length) {
			notEnoughColliders = true;
			Debug.LogError("Not enough grid colliders in the scene");
		}

		puzzlePieces = new GameObject[puzzleTextures.Length];
		for(int i = 0; i < puzzleTextures.Length; i++) {

			Vector3 createdPos = new Vector3(Random.Range(boundaries.minX, boundaries.maxX), Random.Range(boundaries.minY, boundaries.maxY), 0.0f); 
			puzzlePieces[i] = Instantiate(puzzlePiecePrefab, createdPos, Quaternion.identity) as GameObject;
			puzzlePieces[i].GetComponent<SpriteRenderer>().sprite = puzzleTextures[i];
			puzzlePieces[i].renderer.sortingOrder = i;

			if(!notEnoughColliders) {
				puzzlePieces[i].GetComponent<TileController>().setSolutionGridCollider(gridColliders[i]);
			}
		}

	}

	public void setHigherSortOrder(GameObject tile) {
		if (puzzlePieces == null || puzzlePieces.Length == 0) {
			return;
		}

		int startSortingLayer = tile.renderer.sortingOrder;

		for(int i = tile.renderer.sortingOrder + 1; i < puzzlePieces.Length; i++) {
			if(puzzlePieces[i] != tile) {
				puzzlePieces[i].renderer.sortingOrder--;
				puzzlePieces[i - 1] = puzzlePieces[i];
			}
		}

		tile.renderer.sortingOrder = puzzlePieces.Length - 1;
		puzzlePieces [puzzlePieces.Length - 1] = tile;
	}

	public GameObject snapToNextGridCollider(GameObject tile) {

		GameObject closestCollider = null;
		float minDistance = float.MaxValue;

		foreach (GameObject collider in gridColliders) {
			float currDistance = Vector3.Distance(tile.transform.position, collider.transform.position);
			if(currDistance < minDistance) {
				minDistance = currDistance;
				closestCollider = collider;
			}
		}

		if (minDistance < snapDistance && !closestCollider.GetComponent<GridColliderController> ().isGridColliderOccupied ()) {
			tile.transform.position = closestCollider.transform.position;
			closestCollider.GetComponent<GridColliderController> ().setCurrentTile (tile);
		} else {
			closestCollider = null;
		}

		return closestCollider;
	}

	public void checkIfPuzzleFinished() {

		foreach (GameObject tile in puzzlePieces) {
			TileController tileC = tile.GetComponent<TileController> ();

			if (!tileC.isOnCorrectCollider ()) {
				puzzleOver = false;
				return;
			}
		}

		Debug.Log ("PUZZLE DONE");

		foreach(GameObject tile in puzzlePieces) {
			tile.GetComponent<TileController>().enabled = false;
		}

		puzzleOver = true;
	}

	public bool getPuzzleOver() {
		return puzzleOver;
	}


}
