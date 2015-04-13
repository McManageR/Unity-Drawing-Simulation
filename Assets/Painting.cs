using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Painting : MonoBehaviour {

	public LineRenderer trail;
	public LineRenderer line;
	public LineRenderer pattern;
	
	GameObject go;

	bool drawing=false;
	bool Game_Over=false;
	int trailLength=0;
	public static int trailLimit = 25;
	public static int anchorsLimit = 6;
	public static float maxTime = 10;

	int pointsCount=0;
	int staticPointsCount=0;
	int patternPointsCount=0;


	float angle1,angle2;

	Vector3[]  trailPoints = new Vector3[trailLimit+1];
	Vector3[]  staticPoints = new Vector3[anchorsLimit];
	Vector3[]  patternPoints = new Vector3[anchorsLimit];

	int score=0;
	float time_left=maxTime;

	Text score_state;
	Text time_state;

	// Use this for initialization
	void Start () {
		go=GameObject.Find ("Gameover");
		CreatePattern ();
	}
	
	// Update is called once per frame
	void Update () {


		if (!Game_Over) {
			TimeProceed ();
			SetCurrentScore ();
			if (!drawing && Input.GetMouseButtonDown (0)) {
				SetStateDrawing ();
			}

			if (drawing && Input.GetMouseButtonUp (0)) {
				SetStateIdle ();
				CreatePattern ();
			}

			if (drawing) {
				DrawProceed ();
			}
		}
		if (time_left < 0) {
			line.SetVertexCount(0);
			pattern.SetVertexCount(0);
			trail.SetVertexCount(0);

			Game_Over=true;
			go.SetActive(true);
			Text finalscore = GameObject.Find ("FinalScore").GetComponent<Text>();
			finalscore.text="ВАШ СЧЕТ: "+score;

		}
	}

	public void ResetGame()
	{
		CreatePattern ();
		go.SetActive(false);
		Game_Over = false;
		score = 0;
		time_left = maxTime;
	}

	private void SetCurrentScore()
	{
		score_state = GameObject.Find ("Score").GetComponent<Text>();
		score_state.text = "СЧЕТ: " + score;
	}

	private void SetStateDrawing()
	{
		
		staticPointsCount=0;
		staticPoints[staticPointsCount]=Input.mousePosition;
		staticPoints[staticPointsCount].z=1;
		
		
		
		drawing=true;
		Debug.Log ("Started Drawing");
		
		
		pointsCount=0;
		trailLength = 0;
		staticPointsCount = 0;
		line.SetVertexCount (pointsCount);
		trail.SetVertexCount(trailLength);
	}

	private void SetStateIdle()
	{
		drawing=false;
		Debug.Log ("Ended Drawing");
		Debug.Log (pointsCount+"Ended");

		staticPointsCount = 0;
		pointsCount=0;
		trailLength = 0;
		line.SetVertexCount (pointsCount);
		trail.SetVertexCount(trailLength);

	}

	public void DrawProceed()
	{
		Vector3 msPos=Input.mousePosition;
		msPos.z = 1;
		if(msPos.y>490){msPos.y=490;}
		
		pointsCount++;
		Vector3 wP = GetComponent<Camera>().ScreenToWorldPoint(msPos);
		line.SetVertexCount (pointsCount);
		line.SetPosition (pointsCount - 1, wP);

		if (patternPointsCount >= staticPointsCount)
			for (int i=0; i<=patternPointsCount; i++) {
				bool unable = false;
				if (GetDistance2_3D (msPos, patternPoints [i]) <= 20) {
					for (int j=0; j<=staticPointsCount; j++) {
						if (staticPoints [j] == patternPoints [i]) {
							unable = true;

							if (j == 0 && patternPointsCount == staticPointsCount) {
								unable = false;
							}
						}

					}
					if (!unable) {
						staticPoints [staticPointsCount] = patternPoints [i];
						Vector3 toWorld = GetComponent<Camera> ().ScreenToWorldPoint (staticPoints [staticPointsCount]);
						staticPointsCount++;
						line.SetVertexCount (staticPointsCount);
						line.SetPosition (staticPointsCount - 1, toWorld);
						pointsCount = staticPointsCount;
					}
				}
			}
		else 
		{
			Goal ();
			CreatePattern();
			SetStateIdle ();
		}


		if (trailLength >= trailLimit) 
		{
			for(int i=0; i<trailLength; i++)
			{
				trailPoints[i]=trailPoints[i+1];
				trailPoints [trailLength] = msPos;
				Vector3 toWorld = GetComponent<Camera>().ScreenToWorldPoint(trailPoints[i]);
				trail.SetPosition(i,toWorld);
			}
		} 
		else 
		{
			trailPoints [trailLength] = msPos;
			trailLength++;
			trail.SetVertexCount(trailLength);
			trail.SetPosition(trailLength-1,wP);
		}
		 
		//Попытка выставления целых отрезков в случаях сильного отклонения в углах
		/*float dX = msPos.x - staticPoints[staticPointsCount].x;
		float dY = msPos.y - staticPoints[staticPointsCount].y;
		angle1 = Mathf.Atan2(dY, dX) * (180 / Mathf.PI);
		Debug.Log (staticPointsCount+" "+angle1);
		if(pointsCount<=10){angle2=angle1;}
			else if(Mathf.Abs (angle2-angle1)>5)
			{
				staticPointsCount++; 
				pointsCount=staticPointsCount;
				staticPoints[staticPointsCount]=msPos;
				liner.SetVertexCount(staticPointsCount);
				liner.SetPosition(staticPointsCount,wP);
			}*/

		//Выставление целых отрезков по нажатию ПКМ(не пулемета, кнопки мыши)
		/*
		if(Input.GetMouseButtonDown(1))
		{
			//staticPointsCount++; 
			//pointsCount=staticPointsCount;
			//staticPoints[staticPointsCount]=msPos;
			//liner.SetVertexCount(staticPointsCount+1);
			//liner.SetPosition(staticPointsCount,wP);
		}
		else if(Input.GetMouseButtonUp(1))
		{
			staticPointsCount++; 
			pointsCount=staticPointsCount;
			staticPoints[staticPointsCount]=msPos;
			liner.SetVertexCount(staticPointsCount);
			liner.SetPosition(staticPointsCount,wP);
			
		}
		*/
	}

	public float GetDistance2_3D(Vector3 a, Vector3 b)
	{
		return Mathf.Abs (a.x - b.x) + Mathf.Abs (a.y - b.y);
	}

	public void CreatePattern()
	{
		patternPointsCount = Random.Range (3, anchorsLimit);
		Debug.Log (patternPointsCount);
		float getAverageAngle = 360 / patternPointsCount;
		patternPoints [0].x = 500; 		patternPoints [0].y = 360;
		for (int i=1; i<=patternPointsCount; i++) 
		{
			patternPoints[i].x = Random.Range(30,800);
			patternPoints[i].y = Random.Range(0,490);
		}

		for (int i=0; i<=patternPointsCount; i++) 
		{
			patternPoints[i].z=2;
			Vector3 toWorld = GetComponent<Camera>().ScreenToWorldPoint(patternPoints[i]);
			pattern.SetVertexCount(i+1);
			pattern.SetPosition(i,toWorld);
		}
		Vector3 wP = GetComponent<Camera>().ScreenToWorldPoint(patternPoints[0]);
		pattern.SetPosition(patternPointsCount,wP);

	}
	public void Goal()
	{
		score++;
		SetCurrentScore ();
		time_left = 20 - score;
		if (time_left < 2) 
		{
			time_left = 2;
			SetCurrentTime();
		}
	}

	public void SetCurrentTime()
	{
		time_state = GameObject.Find ("Timer").GetComponent<Text>();
		time_state.text = "ВРЕМЯ: " + time_left + "с";
	}
	public void TimeProceed()
	{
		time_left -= Time.deltaTime;
		SetCurrentTime ();
	}
	/*
	public bool CheckSimilarity()
	{
		int similar = 0;
		for (int i=1; i<staticPointsCount; i++)
			for (int j=1; j<patternPointsCount; j++) 
			{
			}
		return true/false;
	}
	*/
}
