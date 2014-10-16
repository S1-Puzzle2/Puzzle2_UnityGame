using UnityEngine;
using System.Collections;
using System.Net;

public class Rotate : MonoBehaviour {

	public float rotationSpeed;

	bool isPressed = false;
	bool newPressed = true;
	Quaternion newRot;


	// Update is called once per frame
	void LateUpdate () {

		if (isPressed && newPressed) {
			newPressed = false;
			newRot = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + 90.0f);
		}

		if (newRot != null && transform.rotation != newRot) {
			transform.rotation = Quaternion.Lerp(transform.rotation, newRot, rotationSpeed * Time.time);
		}
	}

	void OnMouseOver() {
		if (Input.GetMouseButton (1)) {
			Debug.Log("On Mouse over" + this.gameObject.ToString());
			isPressed = true;
		} else {
			isPressed = false;
			newPressed = true;
		}
	}	
	
}
