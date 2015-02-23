using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class GestureListener : MonoBehaviour, KinectGestures.GestureListenerInterface
{
	// GUI Text to display the gesture messages.
	public Text GestureInfo;
	
	private bool leftHandLift;
    private GameControllerScript gameController;

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
        leftHandLift = false;
    }
	
	public bool IsHandLiftet()
	{
        if (leftHandLift)
		{
            leftHandLift = false;
			return true;
		}
		
		return false;
	}
	
    public void UserDetected(uint userId, int userIndex)
	{
		// detect these user specific gestures
		KinectManager manager = KinectManager.Instance;
        GestureInfo.text = "User detected";
        Debug.Log("Listener: User detected");
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
            leftHandLift = !leftHandLift;
            GestureInfo.text = "Push";
            gameController.setLeftHandLiftet(leftHandLift);
        }

		return true;
	}

	public bool GestureCancelled (uint userId, int userIndex, KinectGestures.Gestures gesture, 
	                              KinectWrapper.NuiSkeletonPositionIndex joint)
	{
		// don't do anything here, just reset the gesture state
		return true;
	}
	
}
