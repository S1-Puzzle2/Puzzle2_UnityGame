using UnityEngine;
using System.Collections;

public class Webcam : MonoBehaviour {

	private WebCamTexture webcamTexture;

	// Use this for initialization
	void Start () {
		webcamTexture = new WebCamTexture ("Integrated Webcam");

		for(int i = 0; i < WebCamTexture.devices.Length; i++) {
			Debug.Log(WebCamTexture.devices[i].name);
		}

		GetComponent<MeshRenderer> ().material.mainTexture = webcamTexture;
		webcamTexture.Play ();
	}
	
	// Update is called once per frame
	void Update () {
	
		Color[] colors = webcamTexture.GetPixels();
		int tempBrightness = 0;
		int maxBrightness = 0;
		int highestX = 0;
		int highestY = 0;
		
		for(int i = 0; i < webcamTexture.height; i++) {
			for(int j = 0; j < webcamTexture.width; j++) {
				Color color = colors[i * webcamTexture.height + j];

				tempBrightness = (int)((299 * color.r * 255 + 587 * color.g * 255 + 114 * color.b * 255) / 1000);

				if(tempBrightness > maxBrightness) {
					maxBrightness = tempBrightness;
					highestX = j;
					highestY = i;
				}
									
			}
		}
		
		Debug.Log("X: " + highestX + ", Y: " + highestY + ", Max Brightness: " + maxBrightness);
	
	}
}
