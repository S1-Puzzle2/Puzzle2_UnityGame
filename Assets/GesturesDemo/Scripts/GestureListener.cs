using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class GestureListener : MonoBehaviour, KinectGestures.GestureListenerInterface
{
	// GUI Text to display the gesture messages.
	public Text GestureInfo;
	
	private bool leftHandLift1;
    private bool leftHandLift2;
    private GameControllerScript gameController;
    private KinectManager manager;

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        leftHandLift1 = false;
        leftHandLift2 = false;
        manager = KinectManager.Instance;
    }
	
	public bool IsHandLiftet1()
	{
        if (leftHandLift1)
		{
            leftHandLift1 = false;
			return true;
		}
		
		return false;
	}

    public bool IsHandLiftet2()
    {
        if (leftHandLift2)
        {
            leftHandLift2 = false;
            return true;
        }

        return false;
    }
	
    public void UserDetected(uint userId, int userIndex)
	{
		// detect these user specific gestures
		KinectManager manager = KinectManager.Instance;
        //GestureInfo.text = "User detected";
        //Debug.Log("Listener: User detected");
        manager.DetectGesture(userId, KinectGestures.Gestures.Click);
	}
	
	public void UserLost(uint userId, int userIndex)
	{
	}

	public void GestureInProgress(uint userId, int userIndex, KinectGestures.Gestures gesture, 
	                              float progress, KinectWrapper.NuiSkeletonPositionIndex joint, Vector3 screenPos)
	{
		// don't do anything here
	}

	public bool GestureCompleted (uint userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectWrapper.NuiSkeletonPositionIndex joint, Vector3 screenPos)
	{
        
        if (gesture == KinectGestures.Gestures.Click)
        {

            if (userId == manager.GetPlayer1ID())
            {
                leftHandLift1 = !leftHandLift1;
                //GestureInfo.text = "Push";
                gameController.setLeftHandLiftet1(leftHandLift1);
            }

            if (userId == manager.GetPlayer2ID())
            {
                leftHandLift2 = !leftHandLift2;
                gameController.setLeftHandLiftet2(leftHandLift2);
            }
        }

        

		return true;
	}

	public bool GestureCancelled (uint userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectWrapper.NuiSkeletonPositionIndex joint)
	{
        return true;
	}
	
}
