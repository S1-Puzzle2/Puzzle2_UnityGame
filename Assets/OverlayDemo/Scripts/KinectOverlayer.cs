using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class KinectOverlayer : MonoBehaviour 
{
//	public Vector3 TopLeft;
//	public Vector3 TopRight;
//	public Vector3 BottomRight;
//	public Vector3 BottomLeft;

    public NetworkController networkController;


	public GUITexture backgroundImage;
	public KinectWrapper.NuiSkeletonPositionIndex TrackedJoint = KinectWrapper.NuiSkeletonPositionIndex.HandRight;
	public GameObject OverlayObject;
    public GameObject OverlayObject2;
	public float smoothFactor = 5f;
    public bool twoPlayers;

    public GameObject player1PlayArea;
    public GameObject player2PlayArea;
    public Text posText;
	public GUIText debugText;
    public float calibrationTime;

	private float distanceToCamera = 10f;
    private bool calibratedMin;
    private bool calibratedMax;
    private bool calibratingMinStarted;
    private bool calibratingMaxStarted;
    private Vector2 minPositionsPlayer1;
    private Vector2 maxPositionsPlayer1;
    private Vector2 minPositionsPlayer2;
    private Vector2 maxPositionsPlayer2;

    private KinectManager manager;

    private bool startCalibrating;
    private bool readySent;

    private string team1_id;
    private string team2_id;

	void Start()
	{
        //Debug.Log(player1PlayArea.collider.bounds.min.x + " / " + player1PlayArea.collider.bounds.max.x);
        //Debug.Log(player2PlayArea.collider.bounds.min.x + " / " + player2PlayArea.collider.bounds.max.x);
        calibratedMin = false;
        calibratedMax = false;
        calibratingMinStarted = false;
        calibratingMaxStarted = false;
        minPositionsPlayer1 = new Vector2();
        maxPositionsPlayer1 = new Vector2();

        minPositionsPlayer2 = new Vector2();
        maxPositionsPlayer2 = new Vector2();

        OverlayObject.renderer.sortingOrder = 70000;
        OverlayObject2.renderer.sortingOrder = 70000;

        manager = KinectManager.Instance;
        readySent = false;
        startCalibrating = false;

		if(OverlayObject)
		{
			distanceToCamera = (OverlayObject.transform.position - Camera.main.transform.position).magnitude;
		}

        InvokeRepeating("UpdateKinect", 0.0f, 0.02f);
	}
	
	void UpdateKinect() 
	{
		
		if(manager && manager.IsInitialized() && startCalibrating)
		{
			//backgroundImage.renderer.material.mainTexture = manager.GetUsersClrTex();
			if(backgroundImage && (backgroundImage.texture == null))
			{
				backgroundImage.texture = manager.GetUsersClrTex();
			}
			
//			Vector3 vRight = BottomRight - BottomLeft;
//			Vector3 vUp = TopLeft - BottomLeft;
			
			int iJointIndex = (int)TrackedJoint;
			
			if(manager.IsUserDetected() && manager.TwoUsers && manager.AllPlayersCalibrated)
			{
				uint userId = manager.GetPlayer1ID();
                uint userId2 = manager.GetPlayer2ID();
                

                if (!calibratedMin || !calibratedMax)
                {
                    if (!calibratedMin && !calibratingMinStarted)
                    {
                        posText.text = "Calibrating min...";
                        //Debug.Log("start calibrating min");
                        StartCoroutine("calibrateMin");
                    }

                    if (calibratedMin && !calibratedMax && !calibratingMaxStarted)
                    {
                        posText.text = "Calibrating max...";
                        //Debug.Log("start calibrating max");
                        StartCoroutine("calibrateMax");
                    }
                }

				if(manager.IsJointTracked(userId, iJointIndex) && calibratedMin && calibratedMax)
				{

                    if (!readySent)
                    {
                        Debug.Log("sending ready");
                        SimpleParameterTransferObject readyCommand = new SimpleParameterTransferObject(Command.Ready, team1_id, null);
                        networkController.sendConn(readyCommand);

                        SimpleParameterTransferObject readyCommand2 = new SimpleParameterTransferObject(Command.Ready, team2_id, null);
                        networkController.sendConn(readyCommand2);

                        readySent = true;
                        posText.text = "";
                    }

					Vector3 posJoint = manager.GetRawSkeletonJointPos(userId, iJointIndex);
                    Vector3 posJoint2 = Vector3.zero;

                    if (twoPlayers)
                    {
                        posJoint2 = manager.GetRawSkeletonJointPos(userId2, iJointIndex);
                    }

					if(posJoint != Vector3.zero)
					{
						// 3d position to depth
						Vector2 posDepth = manager.GetDepthMapPosForJointPos(posJoint);
						
						// depth pos to color pos
						Vector2 posColor = manager.GetColorMapPosForDepthPos(posDepth);
                        //posText.text = posColor.ToString();
						
                        
						//float scaleX = (float)posColor.x / KinectWrapper.Constants.ColorImageWidth;
                        
						//float scaleY = 1.0f - (float)posColor.y / KinectWrapper.Constants.ColorImageHeight;

                        float scaleX = map(posColor.x, minPositionsPlayer1.x, maxPositionsPlayer1.x, player1PlayArea.collider.bounds.min.x, player1PlayArea.collider.bounds.max.x);
                        float scaleY = map(posColor.y, minPositionsPlayer1.y, maxPositionsPlayer1.y, player1PlayArea.collider.bounds.min.y, player1PlayArea.collider.bounds.max.y);
//						Vector3 localPos = new Vector3(scaleX * 10f - 5f, 0f, scaleY * 10f - 5f); // 5f is 1/2 of 10f - size of the plane
//						Vector3 vPosOverlay = backgroundImage.transform.TransformPoint(localPos);
						//Vector3 vPosOverlay = BottomLeft + ((vRight * scaleX) + (vUp * scaleY));

						if(debugText)
						{
							debugText.guiText.text = "Tracked user ID: " + userId;  // new Vector2(scaleX, scaleY).ToString();
						}
						
						if(OverlayObject)
						{
							//Vector3 vPosOverlay = Camera.main.ViewportToWorldPoint(new Vector3(scaleX, scaleY, distanceToCamera));

                            if (scaleX < player1PlayArea.collider.bounds.min.x)
                            {
                                scaleX = player1PlayArea.collider.bounds.min.x;
                            }
                            else if (scaleX > player1PlayArea.collider.bounds.max.x)
                            {
                                scaleX = player1PlayArea.collider.bounds.max.x;
                            }


                            if(scaleY < player1PlayArea.collider.bounds.min.y) 
                            {
                                scaleY = player1PlayArea.collider.bounds.min.y;
                            }
                            else if(scaleY > player1PlayArea.collider.bounds.max.y) 
                            {
                                scaleY = player1PlayArea.collider.bounds.max.y;
                            }

                            Vector3 vPosOverlay = new Vector3(scaleX, scaleY, OverlayObject.transform.position.z);
                            OverlayObject.transform.position = Vector3.Lerp(OverlayObject.transform.position, vPosOverlay, smoothFactor * Time.deltaTime);
						}
					}

                    if (twoPlayers && posJoint2 != Vector3.zero)
                    {
                        // 3d position to depth
                        Vector2 posDepth = manager.GetDepthMapPosForJointPos(posJoint2);

                        // depth pos to color pos
                        Vector2 posColor = manager.GetColorMapPosForDepthPos(posDepth);
                        //posText.text = posColor.ToString();


                        //float scaleX = (float)posColor.x / KinectWrapper.Constants.ColorImageWidth;

                        //float scaleY = 1.0f - (float)posColor.y / KinectWrapper.Constants.ColorImageHeight;

                        float scaleX = map(posColor.x, minPositionsPlayer2.x, maxPositionsPlayer2.x, player2PlayArea.collider.bounds.min.x, player2PlayArea.collider.bounds.max.x);
                        float scaleY = map(posColor.y, minPositionsPlayer2.y, maxPositionsPlayer2.y, player2PlayArea.collider.bounds.min.y, player2PlayArea.collider.bounds.max.y);
                        //						Vector3 localPos = new Vector3(scaleX * 10f - 5f, 0f, scaleY * 10f - 5f); // 5f is 1/2 of 10f - size of the plane
                        //						Vector3 vPosOverlay = backgroundImage.transform.TransformPoint(localPos);
                        //Vector3 vPosOverlay = BottomLeft + ((vRight * scaleX) + (vUp * scaleY));

                        if (debugText)
                        {
                            debugText.guiText.text = "Tracked user ID: " + userId;  // new Vector2(scaleX, scaleY).ToString();
                        }

                        if (OverlayObject2)
                        {
                            if (scaleX < player2PlayArea.collider.bounds.min.x)
                            {
                                scaleX = player2PlayArea.collider.bounds.min.x;
                            }
                            else if (scaleX > player2PlayArea.collider.bounds.max.x)
                            {
                                scaleX = player2PlayArea.collider.bounds.max.x;
                            }


                            if (scaleY < player2PlayArea.collider.bounds.min.y)
                            {
                                scaleY = player2PlayArea.collider.bounds.min.y;
                            }
                            else if (scaleY > player2PlayArea.collider.bounds.max.y)
                            {
                                scaleY = player2PlayArea.collider.bounds.max.y;
                            }

                            //Vector3 vPosOverlay = Camera.main.ViewportToWorldPoint(new Vector3(scaleX, scaleY, distanceToCamera));
                            Vector3 vPosOverlay = new Vector3(scaleX, scaleY, OverlayObject2.transform.position.z);
                            OverlayObject2.transform.position = Vector3.Lerp(OverlayObject2.transform.position, vPosOverlay, smoothFactor * Time.deltaTime);
                        }
                    }
				}
				
			}
			
		}
	}

    float map(float x, float in_min, float in_max, float out_min, float out_max)
    {
        return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
    }

    private IEnumerator calibrateMin()
    {
        calibratingMinStarted = true;
        int functionEvals = (int)(calibrationTime / 0.1f);
        float minX = 0.0f;
        float minY = 0.0f;

        float minX2 = 0.0f;
        float minY2 = 0.0f;
        int iJointIndex = (int)TrackedJoint;

        yield return new WaitForSeconds(1);
        for (int i = 0; i < functionEvals; i++)
        {
            if (manager.IsJointTracked(manager.GetPlayer1ID(), iJointIndex))
            {
                Vector3 posJoint = manager.GetRawSkeletonJointPos(manager.GetPlayer1ID(), iJointIndex);
                Vector3 posJoint2 = Vector3.zero;

                if (twoPlayers)
                {
                    posJoint2 = manager.GetRawSkeletonJointPos(manager.GetPlayer2ID(), iJointIndex);
                }

                if (posJoint != Vector3.zero && (!twoPlayers || posJoint2 != Vector3.zero))
                {
                    Vector2 posDepth = manager.GetDepthMapPosForJointPos(posJoint);
                    Vector2 posColor = manager.GetColorMapPosForDepthPos(posDepth);
                    minX += posColor.x;
                    minY += posColor.y;

                    if (twoPlayers)
                    {
                        Vector2 posDepth2 = manager.GetDepthMapPosForJointPos(posJoint2);
                        Vector2 posColor2 = manager.GetColorMapPosForDepthPos(posDepth2);
                        minX2 += posColor2.x;
                        minY2 += posColor2.y;
                    }
                }
                else
                {
                    i--;
                }
            }
            else
            {
                i--;
            }

            yield return new WaitForSeconds(0.1f);
        }

        //Debug.Log(minX / functionEvals + " / " + minY / functionEvals);
        minPositionsPlayer1.x = minX / functionEvals;
        minPositionsPlayer1.y = minY / functionEvals;

        if (twoPlayers)
        {
            minPositionsPlayer2.x = minX2 / functionEvals;
            minPositionsPlayer2.y = minY2 / functionEvals;
        }
        calibratedMin = true;
    }

    private IEnumerator calibrateMax()
    {
        calibratingMaxStarted = true;
        int functionEvals = (int)(calibrationTime / 0.1f);
        float maxX = 0.0f;
        float maxY = 0.0f;

        float maxX2 = 0.0f;
        float maxY2 = 0.0f;

        int iJointIndex = (int)TrackedJoint;

        yield return new WaitForSeconds(1);
        for (int i = 0; i < functionEvals; i++)
        {
            if (manager.IsJointTracked(manager.GetPlayer1ID(), iJointIndex))
            {
                Vector3 posJoint = manager.GetRawSkeletonJointPos(manager.GetPlayer1ID(), iJointIndex);
                Vector3 posJoint2 = Vector3.zero;

                if (twoPlayers)
                {
                    posJoint2 = manager.GetRawSkeletonJointPos(manager.GetPlayer2ID(), iJointIndex);
                }

                if (posJoint != Vector3.zero && (!twoPlayers || posJoint2 != Vector3.zero))
                {
                    Vector2 posDepth = manager.GetDepthMapPosForJointPos(posJoint);
                    Vector2 posColor = manager.GetColorMapPosForDepthPos(posDepth);
                    maxX += posColor.x;
                    maxY += posColor.y;

                    if (twoPlayers)
                    {
                        Vector2 posDepth2 = manager.GetDepthMapPosForJointPos(posJoint2);
                        Vector2 posColor2 = manager.GetColorMapPosForDepthPos(posDepth2);
                        maxX2 += posColor2.x;
                        maxY2 += posColor2.y;
                    }
                }
                else
                {
                    i--;
                }
            }
            else
            {
                i--;
            }

            yield return new WaitForSeconds(0.1f);
        }

        //Debug.Log(maxX / functionEvals + " / " + maxY / functionEvals);
        maxPositionsPlayer1.x = maxX / functionEvals;
        maxPositionsPlayer1.y = maxY / functionEvals;

        if (twoPlayers)
        {
            maxPositionsPlayer2.x = maxX2 / functionEvals;
            maxPositionsPlayer2.y = maxY2 / functionEvals;
        }
        calibratedMax = true;
    }

    public void setStartCalibration(bool value)
    {
        this.startCalibrating = value;
    }

    public void setTeam1ID(string id)
    {
        this.team1_id = id;
    }

    public void setTeam2ID(string id)
    {
        this.team2_id = id;
    }
}
