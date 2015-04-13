using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Actions : MonoBehaviour {

	GameObject menu;
	GameObject paint;
	GameObject cam;
	GameObject go;
	// Use this for initialization
	void Start () {
		menu=GameObject.Find ("Menu");
		paint=GameObject.Find ("PaintCanvas");
		cam = GameObject.Find ("Camera");
		cam.SetActive (false);
		go = GameObject.Find ("Gameover");
		go.SetActive (false);
		Canvas paintCanvas = paint.GetComponent<Canvas> ();
		paintCanvas.enabled = false;	

	}

	public void Exit()
	{
		Application.Quit ();
	}
	public void LetsStartIt()
	{
		Canvas menuCanvas = menu.GetComponent<Canvas> ();
		Canvas paintCanvas = paint.GetComponent<Canvas> ();
		menuCanvas.enabled = false;
		paintCanvas.enabled = true;
		cam.SetActive (true);

	}
	// Update is called once per frame
	void Update () {
	
	}
}
