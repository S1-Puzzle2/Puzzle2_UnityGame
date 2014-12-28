using UnityEngine;
using System.Collections;
using System.Linq;
using LitJson;

public class GameControllerScript : MonoBehaviour {

	public Sprite[] puzzleTextures;
	public GameObject puzzlePiecePrefab;
	public TwoDBoundaries boundaries;
	public float snapDistance;

	private GameObject[] puzzlePieces;
	private GameObject[] gridColliders;
	private bool notEnoughColliders;

    private bool started;

	private bool puzzleOver;

	private float time_Team1;
	private float time_Team2;
	
	void Start() {

        started = false;
		puzzleOver = false;

		time_Team1 = 0.0f;
		time_Team2 = 0.0f;
	}

    public void startGame()
    {
        gridColliders = GameObject.FindGameObjectsWithTag("GridCollider").OrderBy(go => go.name).ToArray();

        if (gridColliders.Length < puzzleTextures.Length)
        {
            notEnoughColliders = true;
            Debug.LogError("Not enough grid colliders in the scene");
        }

        puzzlePieces = new GameObject[puzzleTextures.Length];
        for (int i = 0; i < puzzleTextures.Length; i++)
        {

            Vector3 createdPos = new Vector3(Random.Range(boundaries.minX, boundaries.maxX), Random.Range(boundaries.minY, boundaries.maxY), 0.0f);
            puzzlePieces[i] = Instantiate(puzzlePiecePrefab, createdPos, Quaternion.identity) as GameObject;
            puzzlePieces[i].GetComponent<SpriteRenderer>().sprite = puzzleTextures[i];
            puzzlePieces[i].renderer.sortingOrder = i;

            if (!notEnoughColliders)
            {
                puzzlePieces[i].GetComponent<TileController>().setSolutionGridCollider(gridColliders[i]);
            }
        }
    }

	void Update() {

        if (started)
        {
            time_Team1 += Time.deltaTime;
            time_Team2 += Time.deltaTime;
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

	public void addToTime(int team, float amount) {
		switch (team) {
			case 1:
			time_Team1 += amount;
			break;
			case 2:
			time_Team2 += amount;
			break;
		}
	}

	public float getTime(int team) {
		if (team == 1) {
			return time_Team1;
		}

		return time_Team2;
	}

    public void updateFromNetwork(JsonData data)
    {
        int cmdId = (int)data["appMsg"]["msgType"];
        Command cmd = (Command) cmdId;

        switch (cmd)
        {
            case Command.Pause:
                break;
            case Command.QrCodeSend:
                break;
            case Command.PenaltyTimeAdd:
                break;
            case Command.GameDataSend:
                break;
            case Command.PieceScanned:
                break;
        }
    }


}
