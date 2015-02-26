using UnityEngine;
using System.Collections;

public class TileController : MonoBehaviour {

	public float lag;
    public Material notSelected;
    public Material selected;

	GameControllerScript gameController;
	bool isDraged;
	GameObject solutionGridCollider;
	GameObject currentGridCollider;
    private int imageID;

    private bool triggered;
    private Vector3 triggerPos;
    

	// Use this for initialization
	void Start () {
		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameControllerScript> ();
		isDraged = false;
        triggered = false;
        this.renderer.material = notSelected;
	}
	
	// Update is called once per frame
	void Update () {

		if (isDraged) {
			//Vector3 screenPos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
			Vector3 newPos = Vector3.Lerp (transform.position, triggerPos, lag * Time.deltaTime);
			transform.position = new Vector3 (newPos.x, newPos.y, 0.0f);
		}

        if (triggered)
        {
            //Debug.Log("is triggered, " + isDraged + " / " + gameController.isLeftHandLiftet());
            if(!isDraged && gameController.isLeftHandLiftet()) {
                //Debug.Log("move tile start");
                moveTileStart();
            }

            if (isDraged && !gameController.isLeftHandLiftet())
            {
                moveTileEnd();
            }
        }

	}

	void moveTileStart() {
		GameObject hitGameObject = getObjectWithHighestSortingLayer ();

		if (hitGameObject != null) {
			if (hitGameObject == this.gameObject) {
				isDraged = true;
				gameController.setHigherSortOrder (this.gameObject);
                this.renderer.material = selected;

				if(currentGridCollider != null) {
					currentGridCollider.GetComponent<GridColliderController> ().setCurrentTile (null);
					currentGridCollider = null;
				}
			} else {
				TileController otherC = hitGameObject.GetComponent<TileController> ();
                //Debug.Log("And the bitch is: " + hitGameObject.name);

				otherC.setDragged (true);
				gameController.setHigherSortOrder (hitGameObject);
                otherC.renderer.material = selected;

				if(otherC.currentGridCollider != null) {
					otherC.currentGridCollider.GetComponent<GridColliderController> ().setCurrentTile (null);
					otherC.currentGridCollider = null;
				}
			}
		}
	}

	void moveTileEnd() {
		GameObject hitGameObject = getObjectWithHighestSortingLayer ();

		if (hitGameObject != null) {
			if (hitGameObject == this.gameObject) {
				isDraged = false;
                this.renderer.material = notSelected;
				setCurrentGridCollider(gameController.snapToNextGridCollider(this.gameObject));
			} else {
                hitGameObject.renderer.material = notSelected;
				hitGameObject.GetComponent<TileController>().setDragged (false);
				hitGameObject.GetComponent<TileController>().setCurrentGridCollider(gameController.snapToNextGridCollider(hitGameObject));
			}
		}

		if (currentGridCollider != null) {
			//gameController.checkIfPuzzleFinished();
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
		RaycastHit2D[] hits = Physics2D.LinecastAll (triggerPos, triggerPos);
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

    public bool isOnAnyCollider()
    {
        if (this.currentGridCollider != null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void setImageID(int imageID)
    {
        this.imageID = imageID;
    }

    public int getImageID()
    {
        return this.imageID;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        triggered = true;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        triggerPos = other.transform.position;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        triggered = false;
    }
	
}

