using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

	public GameObject timeTeam1;
	public GameObject timeTeam2;

    public Text team1Name;
    public Text team2Name;

	private Text text_timeTeam1;
	private Text text_timeTeam2;

	private GameControllerScript gameController;

	// Use this for initialization
	void Start () {
		text_timeTeam1 = timeTeam1.GetComponent<Text> ();
		text_timeTeam2 = timeTeam2.GetComponent<Text> ();

		gameController = GameObject.FindGameObjectWithTag ("GameController").GetComponent<GameControllerScript> ();
	}
	
	// Update is called once per frame
	void Update () {

		if(!gameController.getPuzzleOver()) {
			text_timeTeam1.text = gameController.getTime (1).ToString("000.00");
			text_timeTeam2.text = gameController.getTime (2).ToString("000.00");
		}
	}

    public void setTeam1Name(string name)
    {
        team1Name.text = name;
    }

    public void setTeam2Name(string name)
    {
        team2Name.text = name;
    }
}
