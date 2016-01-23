using UnityEngine;
using System.Collections;

[System.Serializable] 
public class Player : MonoBehaviour {
	
	public string name;
	public Color yourColor;
	public GUISkin style;
	public int currentVillages;
	public bool WonGame;
	
	// Use this for initialization
	void Start () {
		yourColor = new Color(0, 0, 0, 1);
		WonGame = false;
	}
	
	void Awake() {
		DontDestroyOnLoad(transform.gameObject);
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	void OnGUI()
	{
		GUI.skin = style;
//		GUI.BeginGroup(new Rect(Screen.width * - 250f, Screen.height -35f, 250f, 35f));
		GUI.Label(new Rect(900, 3, 200f, 30f), "<size=14> Turns: " + GameObject.Find ("Engine").GetComponent<Engine>().Turns +"</size>");

		if (yourColor == Color.green)
			GUI.Label(new Rect(900, 50, 200f, 20f), "<size=14> YOU ARE GREEN</size>");
		else if (yourColor == Color.yellow)
			GUI.Label(new Rect(900, 50, 200f, 30f), "<size=14> YOU ARE YELLOW</size>");
		else if (yourColor == Color.red)
			GUI.Label(new Rect(900, 50, 200f, 30f), "<size=14> YOU ARE RED</size>");
//		GUI.EndGroup();
//		if (currentVillages < 0 && GameObject.Find("Engine").GetComponent<Engine>().gameGoing)
//		{
//			GUI.BeginGroup( new Rect(Screen.width*0.5f, Screen.height*0.5f, Screen.width*0.5f, 30f));
//			GUI.Label(new Rect(0, 0, Screen.width*0.5f, 30f), "<size=20> YOU LOSE!  </size>");
//
//			if (Network.isClient && GUI.Button(new Rect (0,30,80,30), "Back to Lobby"))
//			{
//				Application.LoadLevel("hexmap");
//			}
//			else if(Network.isServer && GUI.Button(new Rect (0,30,80,30), "Observe Game"))
//			{
//				yourColor = new Color(0, 0, 0, 1);
//			}
//			GUI.EndGroup ();
//		}
//		else if (WonGame)
//		{
//			GUI.BeginGroup( new Rect(Screen.width*0.5f, Screen.height*0.5f, Screen.width*0.5f, 30f));
//			GUI.Label(new Rect(0, 0, Screen.width*0.5f, 30f), "<size=20> YOU WIN! </size>");
//			
//			if (GUI.Button(new Rect (0,30,80,30), "Back to Lobby"))
//			{
//				Application.LoadLevel("hexmap");
//			}
//
//			GUI.EndGroup ();
//		}
		//name = GUILayout.TextField(name);
		
		//if(name == null)
		//{
		//    if (GUILayout.Button("Name"))
		//        name = name;
		//}
	}
	
	public void updateNumberOfVillages(Color color, bool state)
	{
		if (color == yourColor)
		{
			if (state)
				currentVillages++;
			else
				currentVillages--;
		}
	}
}
