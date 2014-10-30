using UnityEngine;
using System.Collections;

public class Webcam : MonoBehaviour {

	private WebCamTexture webcamTexture;

	// Use this for initialization
	void Start () {
		webcamTexture = new WebCamTexture ("Microsoft LifeCam HD-3000");

		for(int i = 0; i < WebCamTexture.devices.Length; i++) {
			Debug.Log(WebCamTexture.devices[i].name);
		}

		GetComponent<MeshRenderer> ().material.mainTexture = webcamTexture;
		webcamTexture.Play ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
