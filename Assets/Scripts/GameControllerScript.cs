using UnityEngine;
using System.Collections;
using System.Linq;
using LitJson;
using ZXing;
using System.Collections.Generic;
using ZXing.QrCode;
using System.IO;
using UnityEngine.UI;

public class GameControllerScript : MonoBehaviour {

	public GameObject puzzlePiecePrefab;
	public TwoDBoundaries boundaries1;
    public TwoDBoundaries boundaries2;
	public float snapDistance;
    public NetworkController networkController;
    public GameObject uiController;
    public Text winText;

    public QRCodePanel qrCodePanel;
    public GameObject testPiece;

	private GameObject[] puzzlePieces1;
    private GameObject[] puzzlePieces2;
	private GameObject[] gridColliders;
    private GameObject[] gridColliders2;
	private bool notEnoughColliders;

    private bool started;

	private bool puzzleOver;

	private float time_Team1;
	private float time_Team2;

    private string team1_clientID;
    private string team2_clientID;
    private string team1_qrCode;
    private string team2_qrCode;
    private string team1_name;
    private string team2_name;
    private int imageCount;
    private int team1_images_received;

    private BarcodeWriter qrWriter;
    private int registeredCount;
    private int gameStateCount;

    private Texture2D[] orderPuzzleImages;
    private LinkedList<JsonData> receivedData;
    private Dictionary<int, GameObject> puzzleGOs1;
    private Dictionary<int, GameObject> puzzleGOs2;
    private int[] orderIDMapping;

    string log = "";
    private bool leftHandLifted;

    public bool paused = false;
    
	
	void Start() {

        started = false;
		puzzleOver = false;
        winText.gameObject.SetActive(false);

		time_Team1 = 0.0f;
		time_Team2 = 0.0f;
        
        qrWriter = new BarcodeWriter { Format = BarcodeFormat.QR_CODE, Options = new QrCodeEncodingOptions { Height = 256, Width = 256 } };
        qrCodePanel.disablePanel();
        registeredCount = 0;

        receivedData = new LinkedList<JsonData>();
        orderPuzzleImages = new Texture2D[9];
        orderIDMapping = new int[9];
        puzzleGOs1 = new Dictionary<int, GameObject>();
        puzzleGOs2 = new Dictionary<int, GameObject>();
        //startGame();
        asdf();
        
	}

    public void asdf()
    {
        gridColliders = GameObject.FindGameObjectsWithTag("GridCollider").OrderBy(go => go.name).ToArray();
        gridColliders2 = GameObject.FindGameObjectsWithTag("GridCollider2").OrderBy(go => go.name).ToArray();

        testPiece.GetComponent<TileController>().setSolutionGridCollider(gridColliders[0]);
        puzzlePieces1 = new GameObject[1];
        puzzlePieces1[0] = testPiece;

        puzzlePieces2 = new GameObject[0];
    }

    public void startGame()
    {
        if (!started)
        {
            gridColliders = GameObject.FindGameObjectsWithTag("GridCollider").OrderBy(go => go.name).ToArray();
            gridColliders2 = GameObject.FindGameObjectsWithTag("GridCollider2").OrderBy(go => go.name).ToArray();

            if (gridColliders.Length < orderPuzzleImages.Length || gridColliders2.Length < orderPuzzleImages.Length)
            {
                notEnoughColliders = true;
                Debug.LogError("Not enough grid colliders in the scene");
            }
            else
            {

                puzzlePieces1 = new GameObject[orderPuzzleImages.Length];
                for (int i = 0; i < orderPuzzleImages.Length; i++)
                {

                    Vector3 createdPos = new Vector3(Random.Range(boundaries1.minX, boundaries1.maxX), Random.Range(boundaries1.minY, boundaries1.maxY), 0.0f);
                    puzzlePieces1[i] = Instantiate(puzzlePiecePrefab, createdPos, Quaternion.identity) as GameObject;
                    puzzlePieces1[i].GetComponent<SpriteRenderer>().sprite = Sprite.Create(orderPuzzleImages[i], new Rect(0, 0, orderPuzzleImages[i].width, orderPuzzleImages[i].height), new Vector2(0.5f, 0.5f));
                    puzzlePieces1[i].renderer.sortingOrder = i;

                    if (!notEnoughColliders)
                    {
                        puzzlePieces1[i].GetComponent<TileController>().setSolutionGridCollider(gridColliders[i]);
                    }

                    puzzlePieces1[i].SetActive(false);
                    puzzleGOs1.Add(orderIDMapping[i], puzzlePieces1[i]);
                }

                puzzlePieces2 = new GameObject[orderPuzzleImages.Length];
                for (int i = 0; i < orderPuzzleImages.Length; i++)
                {
                    Vector3 createdPos = new Vector3(Random.Range(boundaries2.minX, boundaries2.maxX), Random.Range(boundaries2.minY, boundaries2.maxY), 0.0f);
                    puzzlePieces2[i] = Instantiate(puzzlePiecePrefab, createdPos, Quaternion.identity) as GameObject;
                    puzzlePieces2[i].GetComponent<SpriteRenderer>().sprite = Sprite.Create(orderPuzzleImages[i], new Rect(0, 0, orderPuzzleImages[i].width, orderPuzzleImages[i].height), new Vector2(0.5f, 0.5f));
                    puzzlePieces2[i].renderer.sortingOrder = i;

                    if (!notEnoughColliders)
                    {
                        puzzlePieces2[i].GetComponent<TileController>().setSolutionGridCollider(gridColliders2[i]);
                    }

                    puzzlePieces2[i].SetActive(false);
                    puzzleGOs2.Add(orderIDMapping[i], puzzlePieces2[i]);

                }



                started = true;
            }
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
                        team1_qrCode = "[protocols]eth::http://" + networkController.ip + ":" + networkController.port + "[clientID]" + appMsg["msgData"]["qrCode"].ToString();
                        Debug.Log("QR Code from Team 1 set");
                    }
                    else if (appMsg["clientID"].ToString() == team2_clientID)
                    {
                        team2_qrCode = "[protocols]eth::http://" + networkController.ip + ":" + networkController.port + "[clientID]" + appMsg["msgData"]["qrCode"].ToString();
                        Debug.Log("QR Code from Team 2 set");
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

                    if (appMsg["msgData"]["gameName"] != null)
                    {
                        gameStateCount++;
                    }

                    string usedClientID = "";
                    string teamName = appMsg["msgData"]["teamName"].ToString();

                    if (gameStateCount == 1)
                    {
                        usedClientID = team1_clientID;
                        uiController.GetComponent<UIController>().setTeam1Name(teamName);
                        qrCodePanel.setTeam1Name(teamName);
                        team1_name = teamName;
                    }
                    else if (gameStateCount == 2)
                    {
                        usedClientID = team2_clientID;
                        uiController.GetComponent<UIController>().setTeam2Name(teamName);
                        qrCodePanel.setTeam2Name(teamName);
                        team2_name = teamName;
                    }

                    JsonData imageIDs = appMsg["msgData"]["imageIDs"];
                    if (imageIDs.IsArray && gameStateCount == 1)
                    {
                        imageCount = imageIDs.Count;
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
                case Command.GetImageResponse:

                    Debug.Log("Image received: " + team1_images_received);
                    if(appMsg["clientID"].ToString() == team1_clientID) {
                        team1_images_received++;

                        string base64 = appMsg["msgData"]["base64Image"].ToString();
                        int imageID = int.Parse(appMsg["msgData"]["imageID"].ToString());
                        int order = int.Parse(appMsg["msgData"]["order"].ToString());

                        orderIDMapping[order] = imageID;

                        log += base64 + "\r\n\r\n";
                        
                        File.WriteAllText("C:\\Users\\faisstm\\Desktop\\log.txt", log);
                        byte[] imgData = System.Convert.FromBase64String(base64);

                        Texture2D newTexture = new Texture2D(256, 240);
                        if (newTexture.LoadImage(imgData))
                        {
                            Debug.Log("Base64 converted to Texture2D succesfully");
                        }
                        else
                        {
                            Debug.Log("Error in converting base64 to texture2d");
                        }

                        orderPuzzleImages[order] = newTexture;

                        if (team1_images_received == imageCount)
                        {
                            Debug.Log("IMAGE COUNT REACHED");
                            SimpleParameterTransferObject readyCommand = new SimpleParameterTransferObject(Command.Ready, team1_clientID, null);
                            networkController.sendConn(readyCommand);

                            SimpleParameterTransferObject readyCommand2 = new SimpleParameterTransferObject(Command.Ready, team2_clientID, null);
                            networkController.sendConn(readyCommand2);
                        }
                    }
                    
                    break;
                case Command.GameStart:
                    Debug.Log("GAME START YEEEEEEEEEEEEEEEEEEEEEAAAAAH!");
                    qrCodePanel.disablePanel();
                    Time.timeScale = 1;

                    if (!started)
                    {
                        startGame();
                    }
                    break;
                case Command.Pause:
                    Time.timeScale = 0;
                    qrCodePanel.enablePanel();
                    paused = true;
                    break;
                case Command.Ready:
                    
                    break;
                case Command.GameFinished:

                    winText.gameObject.SetActive(true);
                    if (appMsg["clientID"].ToString() == team1_clientID)
                    {
                        if(bool.Parse(appMsg["msgData"]["isWinning"].ToString())) {
                            winText.text = team1_name + " have won!! YEAH!";                            
                        }
                        else
                        {
                            winText.text = team2_name + " have won!! YIPPIIII!";
                        }
                    }
                    
                    

                    break;
                case Command.PenaltyTimeAdd:

                    if (appMsg["clientID"].ToString() == team1_clientID)
                    {
                        addToTime(1, float.Parse(appMsg["msgData"]["penaltyTime"].ToString()));
                    } else if(appMsg["clientID"].ToString() == team2_clientID) {
                        addToTime(2, float.Parse(appMsg["msgData"]["penaltyTime"].ToString()));
                    }

                    break;
                case Command.PieceScanned:
                    int pieceID = 0;
                    if (appMsg["clientID"].ToString() == team1_clientID)
                    {
                        pieceID = int.Parse(appMsg["msgData"]["unlockedImage"].ToString());
                        puzzleGOs1[pieceID].SetActive(true);
                    }
                    else if (appMsg["clientID"].ToString() == team2_clientID)
                    {
                        pieceID = int.Parse(appMsg["msgData"]["unlockedImage"].ToString());
                        puzzleGOs2[pieceID].SetActive(true);
                    }

                    break;
            }

        }

	}

	public void setHigherSortOrder(GameObject tile) {
		if (puzzlePieces1 == null || puzzlePieces1.Length == 0) {
			return;
		}

		int startSortingLayer = tile.renderer.sortingOrder;

		for(int i = tile.renderer.sortingOrder + 1; i < puzzlePieces1.Length; i++) {
			if(puzzlePieces1[i] != tile) {
				puzzlePieces1[i].renderer.sortingOrder--;
				puzzlePieces1[i - 1] = puzzlePieces1[i];
			}
		}

		tile.renderer.sortingOrder = puzzlePieces1.Length - 1;
		puzzlePieces1 [puzzlePieces1.Length - 1] = tile;
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

        foreach (GameObject collider in gridColliders2)
        {
            float currDistance = Vector3.Distance(tile.transform.position, collider.transform.position);
            if (currDistance < minDistance)
            {
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

        bool temp1 = true;
		foreach (GameObject tile in puzzlePieces1) {
			TileController tileC = tile.GetComponent<TileController> ();

			if (!tileC.isOnCorrectCollider ()) {
                temp1 = false;
                break;
			}
		}

        bool temp2 = true;
        foreach (GameObject tile in puzzlePieces2)
        {
            TileController tileC = tile.GetComponent<TileController>();

            if (!tileC.isOnCorrectCollider())
            {
                temp2 = false;
                break;
            }
        }

        if (temp1 || temp2)
        {
            Debug.Log("PUZZLE DONE");

            foreach (GameObject tile in puzzlePieces1)
            {
                tile.GetComponent<TileController>().enabled = false;
            }

            SimpleParameterTransferObject puzzleFinishedPackage;
            if (temp1)
            {
                puzzleFinishedPackage = new SimpleParameterTransferObject(Command.PuzzleFinished, team1_clientID, null);
            }
            else
            {
                puzzleFinishedPackage = new SimpleParameterTransferObject(Command.PuzzleFinished, team2_clientID, null);
            }
            networkController.sendConn(puzzleFinishedPackage);

            puzzleOver = true;
        }
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

    public void setLeftHandLiftet(bool value)
    {
        this.leftHandLifted = value;
    }

    public bool isLeftHandLiftet()
    {
        return this.leftHandLifted;
    }
}
