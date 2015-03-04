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

    private int playerID;
    

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
			//Vector3 newPos = Vector3.Lerp (transform.position, triggerPos, lag * Time.deltaTime);
			//transform.position = new Vector3 (newPos.x, newPos.y, 0.0f);
            transform.position = triggerPos;
		}

        if (triggered)
        {
            //Debug.Log("is triggered, " + isDraged + " / " + gameController.isLeftHandLiftet());
            if(!isDraged) {
                if ((playerID == 1 && gameController.isLeftHandLiftet1()) || (playerID == 2 && gameController.isLeftHAndLiftet2()))
                {
                    //Debug.Log("move tile start");
                    moveTileStart();

                    if (playerID == 1)
                    {
                        gameController.setPlayer1Dragging(true);
                    }
                    else
                    {
                        gameController.setPlayer2Dragging(true);
                    }
                }
            }

            if (isDraged)
            {
                if ((playerID == 1 && !gameController.isLeftHandLiftet1()) || (playerID == 2 && !gameController.isLeftHAndLiftet2()))
                {
                    moveTileEnd();

                    if (playerID == 1)
                    {
                        gameController.setPlayer1Dragging(false);
                    }
                    else
                    {
                        gameController.setPlayer2Dragging(false);
                    }
                }
            }
        }

	}

	void moveTileStart() {
		GameObject hitGameObject = getObjectWithHighestSortingLayer ();

		if (hitGameObject != null) {
			if (hitGameObject == this.gameObject) {
                if (isDraged == false)
                {
                    isDraged = true;
                    gameController.setHigherSortOrder(this.gameObject);
                    this.renderer.material = selected;

                    if (currentGridCollider != null)
                    {
                        currentGridCollider.GetComponent<GridColliderController>().setCurrentTile(null);
                        currentGridCollider = null;
                    }
                }
			} else {
				TileController otherC = hitGameObject.GetComponent<TileController> ();
                //Debug.Log("And the bitch is: " + hitGameObject.name);
                if (!otherC.isDraged)
                {
                    otherC.setDragged(true);
                    gameController.setHigherSortOrder(hitGameObject);
                    otherC.renderer.material = selected;

                    if (otherC.currentGridCollider != null)
                    {
                        otherC.currentGridCollider.GetComponent<GridColliderController>().setCurrentTile(null);
                        otherC.currentGridCollider = null;
                    }
                }
			}
		}
	}

	void moveTileEnd() {
		GameObject hitGameObject = getObjectWithHighestSortingLayer ();

		if (hitGameObject != null) {
			if (hitGameObject == this.gameObject) {
                if (isDraged)
                {
                    isDraged = false;
                    this.renderer.material = notSelected;
                    setCurrentGridCollider(gameController.snapToNextGridCollider(this.gameObject));
                }
			} else {
                if (hitGameObject.GetComponent<TileController>().isDraged)
                {
                    hitGameObject.renderer.material = notSelected;
                    hitGameObject.GetComponent<TileController>().setDragged(false);
                    hitGameObject.GetComponent<TileController>().setCurrentGridCollider(gameController.snapToNextGridCollider(hitGameObject));
                }
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
        if (other.tag == "Player1Sphere")
        {
            playerID = 1;
            if (gameController.getPlayer1Dragging())
            {
                triggered = false;
            }
            else
            {
                triggered = true;
            }
        }
        else if (other.tag == "Player2Sphere")
        {
            playerID = 2;
            if (gameController.getPlayer2Dragging())
            {
                triggered = false;
            }
            else
            {
                triggered = true;
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        triggerPos = other.transform.position;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        triggered = false;

        if (isDraged == true)
        {
            isDraged = false;
            this.renderer.material = notSelected;

            if (playerID == 1)
            {
                gameController.setLeftHandLiftet1(false);
            }
            else if (playerID == 2)
            {
                gameController.setLeftHandLiftet2(false);
            }
        }
    }
	
}

