using UnityEngine;
using System.Collections;
using System.Linq;
using LitJson;
using ZXing;
using System.Collections.Generic;
using ZXing.QrCode;

public class GameControllerScript : MonoBehaviour {

	public Sprite[] puzzleTextures;
	public GameObject puzzlePiecePrefab;
	public TwoDBoundaries boundaries;
	public float snapDistance;

    public QRCodePanel qrCodePanel;

	private GameObject[] puzzlePieces;
	private GameObject[] gridColliders;
	private bool notEnoughColliders;

    private bool started;

	private bool puzzleOver;

	private float time_Team1;
	private float time_Team2;

    private BarcodeWriter qrWriter;

    private bool commandReceived = false;
    private Command receivedCommand;
    private JsonData data;

    public bool paused = false;
    
	
	void Start() {

        started = false;
		puzzleOver = false;

		time_Team1 = 0.0f;
		time_Team2 = 0.0f;

        qrWriter = new BarcodeWriter { Format = BarcodeFormat.QR_CODE, Options = new QrCodeEncodingOptions { Height = 256, Width = 256 } };
        qrCodePanel.disablePanel();
	}

    public void startGame()
    {
        gridColliders = GameObject.FindGameObjectsWithTag("GridCollider").OrderBy(go => go.name).ToArray();

        if (gridColliders.Length < puzzleTextures.Length)
        {
            notEnoughColliders = true;
            Debug.LogError("Not enough grid colliders in the scene");
        }
        else
        {

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

            started = true;
        }
    }

	void Update() {

        if (started)
        {
            time_Team1 += Time.deltaTime;
            time_Team2 += Time.deltaTime;
        }

        if (commandReceived)
        {
            Debug.Log("Command received! BAM");
            string dataNull = data == null ? "yes : (" : "No";
            Debug.Log("Is data null?" + dataNull);

            int cmdId = (int)data["appMsg"]["msgType"];
            receivedCommand = (Command)cmdId;
            
            switch (receivedCommand)
            {
                case Command.QrCodeSend:
                    qrCodePanel.enablePanel();

                    string uid1 = (string) data["appMsg"]["msgData"]["uid1"];
                    string uid2 = (string) data["appMsg"]["msgData"]["uid2"];

                    Debug.Log("QR Contents: " + uid1 + " / " + uid2);

                    Color32[] qr1 = qrWriter.Write(uid1);
                    Color32[] qr2 = qrWriter.Write(uid2);

                    qrCodePanel.setQRCodes(qr1, qr2);
                    break;
                case Command.GameDataSend:
                    //TODO: write
                    break;
                case Command.Pause:
                    Time.timeScale = 0;
                    paused = true;
                    break;
                case Command.Ready:
                    startGame();
                    break;
                case Command.PenaltyTimeAdd:
                    //TODO: write
                    break;
                case Command.PieceScanned:
                    //TODO: write
                    break;
            }

            commandReceived = false;
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
        this.data = data;
        commandReceived = true;
        /*

        Debug.Log("Received Command: " + receivedCommand);

        switch (receivedCommand)
        {
            case Command.Pause:
                break;
            case Command.QrCodeSend:
                
                string uid1 = (string) data["appMsg"]["msgData"]["uid1"];
                string uid2 = (string) data["appMsg"]["msgData"]["uid2"];

                Color32[] qr1 = qrWriter.Write(uid1);
                Color32[] qr2 = qrWriter.Write(uid2);

                commandArgs = new System.Object[] { qr1, qr2 };
                break;
            case Command.PenaltyTimeAdd:
                break;
            case Command.GameDataSend:
                break;
            case Command.PieceScanned:
                break;
        }

        commandReceived = true;
         * */
    }


}
