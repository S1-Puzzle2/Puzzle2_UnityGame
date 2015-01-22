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
    public NetworkController networkController;

    public QRCodePanel qrCodePanel;

	private GameObject[] puzzlePieces;
	private GameObject[] gridColliders;
	private bool notEnoughColliders;

    private bool started;

	private bool puzzleOver;

	private float time_Team1;
	private float time_Team2;

    private string team1_clientID;
    private string team2_clientID;
    private string team1_qrCode;
    private string team2_qrCode;

    private BarcodeWriter qrWriter;
    private int registeredCount;
    private int gameStateCount;

    private LinkedList<JsonData> receivedData;

    public bool paused = false;
    
	
	void Start() {

        started = false;
		puzzleOver = false;

		time_Team1 = 0.0f;
		time_Team2 = 0.0f;

        qrWriter = new BarcodeWriter { Format = BarcodeFormat.QR_CODE, Options = new QrCodeEncodingOptions { Height = 256, Width = 256 } };
        qrCodePanel.disablePanel();
        registeredCount = 0;

        receivedData = new LinkedList<JsonData>();
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

        if (receivedData.Count > 0)
        {
            JsonData data = receivedData.First.Value;
            receivedData.RemoveFirst();
            string dataNull = data == null ? "yes : (" : "No";
            Debug.Log("Is data null?" + dataNull);

            JsonData appMsg = JsonMapper.ToObject(Base64.Base64Decode(data["appMsg"].ToString()));
            Command receivedCommand = CommandMethods.getCommand(appMsg["msgType"].ToString());
            Debug.Log("received Command: " + receivedCommand);

            switch (receivedCommand)
            {
                case Command.QrCodeSend:

                    if (appMsg["clientID"].ToString() == team1_clientID)
                    {
                        team1_qrCode = appMsg["msgData"]["qrCode"].ToString();
                    }
                    else if (appMsg["clientID"].ToString() == team2_clientID)
                    {
                        team2_qrCode = appMsg["msgData"]["qrCode"].ToString();
                    }

                    if(team1_qrCode != null && team2_qrCode != null && !string.IsNullOrEmpty(team1_qrCode) && !string.IsNullOrEmpty(team2_qrCode)) {
                        Color32[] qr1 = qrWriter.Write(team1_qrCode);
                        Color32[] qr2 = qrWriter.Write(team2_qrCode);
                        qrCodePanel.enablePanel();
                        qrCodePanel.setQRCodes(qr1, qr2);
                    }

                    break;
                case Command.Registered:
                    registeredCount++;

                    if (registeredCount == 1)
                    {
                        team1_clientID = appMsg["clientID"].ToString();
                        SimpleParameterTransferObject registerPackage = new SimpleParameterTransferObject(Command.Register, null, null);
                        networkController.sendConn(registerPackage);
                        Debug.Log("Team 1 registered complete");
                    }
                    else if (registeredCount == 2)
                    {
                        team2_clientID = appMsg["clientID"].ToString();
                        Debug.Log("Team 2 registered complete");
                        SimpleParameterTransferObject gameStatePackage = new SimpleParameterTransferObject(Command.GetGameState, team1_clientID, null);
                        networkController.sendConn(gameStatePackage);
                    }

                    break;
                case Command.GameStateResponse:
                    // TODO: save puzzle pieces etc.

                    gameStateCount++;

                    string usedClientID = "";
                    if (gameStateCount == 1)
                    {
                        usedClientID = team1_clientID;
                    }
                    else if (gameStateCount == 2)
                    {
                        usedClientID = team2_clientID;
                    }

                    JsonData imageIDs = appMsg["msgData"]["imageIDs"];
                    if (imageIDs.IsArray)
                    {
                        int count = imageIDs.Count;
                        for (int i = 0; i < count; i++)
                        {
                            Dictionary<string, object> paramsDict = new Dictionary<string, object>();
                            paramsDict.Add("imageID", (int)imageIDs[i]);
                            SimpleParameterTransferObject getImagePackage = new SimpleParameterTransferObject(Command.GetImage, usedClientID, paramsDict);
                            networkController.sendConn(getImagePackage);
                        }
                    }


                    if (gameStateCount == 1)
                    {
                        SimpleParameterTransferObject gameStatePackage = new SimpleParameterTransferObject(Command.GetGameState, team2_clientID, null);
                        networkController.sendConn(gameStatePackage);
                    }
                    
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
        receivedData.AddLast(data);
    }


}
