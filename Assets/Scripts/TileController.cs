using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour {

	public float lag;

	GameControllerScript gameController;
	bool isDraged;
	GameObject solutionGridCollider;
	GameObject currentGridCollider;

	// Use this for initialization
	void Start () {
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameControllerScript> ();
		isDraged = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (isDraged) {
			Vector3 screenPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector3 newPos = Vector3.Lerp (transform.position, screenPos, lag * Time.deltaTime);
			transform.position = new Vector3 (newPos.x, newPos.y, 0.0f);
		}

	}

	void OnMouseDown() {
		GameObject hitGameObject = getObjectWithHighestSortingLayer ();

		if (hitGameObject != null) {
			if (hitGameObject == this.gameObject) {
				isDraged = true;
				gameController.setHigherSortOrder (this.gameObject);

				if(currentGridCollider != null) {
					currentGridCollider.GetComponent<GridColliderController> ().setCurrentTile (null);
					currentGridCollider = null;
				}
			} else {
				TileController otherC = hitGameObject.GetComponent<TileController> ();
				otherC.setDragged (true);
				gameController.setHigherSortOrder (hitGameObject);
				
				if(otherC.currentGridCollider != null) {
					otherC.currentGridCollider.GetComponent<GridColliderController> ().setCurrentTile (null);
					otherC.currentGridCollider = null;
				}
			}
		}
	}

	void OnMouseUp() {
		GameObject hitGameObject = getObjectWithHighestSortingLayer ();

		if (hitGameObject != null) {
			if (hitGameObject == this.gameObject) {
				isDraged = false;
				setCurrentGridCollider(gameController.snapToNextGridCollider(this.gameObject));
			} else {
				hitGameObject.GetComponent<TileController> ().setDragged (false);
				hitGameObject.GetComponent<TileController>().setCurrentGridCollider(gameController.snapToNextGridCollider(hitGameObject));
			}
		}

		if (currentGridCollider != null) {
			gameController.checkIfPuzzleFinished();
		}
	}

	void setDragged(bool value) {
		this.isDraged = value;
	}

	public void setSolutionGridCollider(GameObject collider) {
		this.solutionGridCollider = collider;
	}

	GameObject getObjectWithHighestSortingLayer() {

		Vector3 clickPosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		RaycastHit2D[] hits = Physics2D.LinecastAll (clickPosition, clickPosition);
		GameObject selectedObject = null;

		if (hits.Length != 0) {
			selectedObject = hits [0].collider.gameObject;
			for (int i = 1; i < hits.Length; i++) {
				if (hits [i].collider.gameObject.renderer.sortingOrder >= selectedObject.renderer.sortingOrder) {
					selectedObject = hits [i].collider.gameObject;
				}
			}
		}

		return selectedObject;
	}

	public bool isOnCorrectCollider() {
		return solutionGridCollider == currentGridCollider;
	}

	private void setCurrentGridCollider(GameObject collider) {
		this.currentGridCollider = collider;
		if (this.currentGridCollider != null) {
			audio.Play();
		}
	}
	
}

