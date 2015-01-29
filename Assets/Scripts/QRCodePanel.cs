using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QRCodePanel : MonoBehaviour {

    public RawImage qr1Raw;
    public RawImage qr2Raw;

    public Text team1_name;
    public Text team2_name;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void setQRCodes(Color32[] qr1, Color32[] qr2)
    {
        Texture2D qr1Tex = new Texture2D(256, 256);
        qr1Tex.SetPixels32(qr1);
        qr1Tex.Apply();
        qr1Raw.texture = qr1Tex;

        Texture2D qr2Tex = new Texture2D(256, 256);
        qr2Tex.SetPixels32(qr2);
        qr2Tex.Apply();
        qr2Raw.texture = qr2Tex;
        
    }

    public void enablePanel()
    {
        this.gameObject.SetActive(true);
    }

    public void disablePanel()
    {
        this.gameObject.SetActive(false);
    }

    public void setTeam1Name(string name) {
        team1_name.text = name;
    }

    public void setTeam2Name(string name) {
        team2_name.text = name;
    }
}
