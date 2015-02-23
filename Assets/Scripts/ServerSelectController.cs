using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ServerSelectController : MonoBehaviour {

    public GameObject serverList;
    public GameObject listEntry;
    public int yOffset = 30;
    public InputField ipInput;
    public NetworkController networkController;

    private LinkedList<GameObject> entries;

	// Use this for initialization
	void Start () {
        entries = new LinkedList<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void addEntry(string ip)
    {
        GameObject newLine = Instantiate(listEntry) as GameObject;
        Text text = newLine.GetComponent<Text>();
        text.text = ip;
        RectTransform rectTransform = newLine.GetComponent<RectTransform>();
        newLine.transform.parent = serverList.transform;
        rectTransform.anchoredPosition = new Vector2(30, -20 - entries.Count * yOffset);
        entries.AddLast(newLine);
    }

    public void connect()
    {
        string ip = ipInput.text;
        this.gameObject.SetActive(false);

        networkController.ip = ip;
        networkController.openConn();
    }
}
