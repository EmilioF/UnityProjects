using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable] 
public class Chat : MonoBehaviour {
	public Vector2 scrollPosition;
	public List<string> chatHistory = new List<string>();
	public GUISkin style;
	private bool _enter;
	public bool Display;
	public GameObject myPlayer;
	public GameObject playerPrefab;
	public string htmlcolor;

	private string currentMessage = string.Empty;
	void Start()
	{
		myPlayer = GameObject.Find ("Player(Clone)");
		if (myPlayer == null)
		{
			myPlayer = (GameObject)Instantiate(playerPrefab);
		}
		htmlcolor = "#D6D6D6";
		Display = true;
	}
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return))
		{
			print(Display);
		}
	}
	void OnGUI()
	{
		GUI.skin = style;
		_enter = false;
		Event e = Event.current;        
		if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return)        
			_enter = true;

		if (Display)
		{
			GUI.BeginGroup( new Rect(20, Screen.height * 0.5f, 300, Screen.height * 0.5f));
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(250), GUILayout.Height(Screen.height * 0.4f));
			foreach (string c in chatHistory)
			{
				GUILayout.Label(c);
			}
			GUILayout.EndScrollView();
			GUILayout.BeginHorizontal(GUILayout.Width(250));
			currentMessage = GUILayout.TextField(currentMessage, GUILayout.Width (200));
			
			if (GUILayout.Button("Send", GUILayout.Width(50)) || _enter)
			{
				if (!string.IsNullOrEmpty(currentMessage.Trim()))
				{
					GetComponent<NetworkView>().RPC("ChatMessage", RPCMode.AllBuffered, 
					                                new object[] { "<color="+ htmlcolor +">"+ myPlayer.GetComponent<Player>().name +" says:</color> \n   " + currentMessage });
					scrollPosition.y = Mathf.Infinity;
					currentMessage = string.Empty;
				}
			}
			GUILayout.EndHorizontal();
			GUI.EndGroup();
		}
	}
	
	public void AddMessage(string message)
	{
		chatHistory.Add("<color=#ADF8FF> <size=14>"+ message +"</size></color>");
		//		GetComponent<NetworkView>().RPC("ChatMessage", RPCMode.AllBuffered, new object[] { "<color=#ADF8FF> <size=14>"+ message +"</size></color>" });
		scrollPosition.y = Mathf.Infinity;
		currentMessage = string.Empty;
	}
	
	[RPC]
	public void ChatMessage(string message)
	{
		chatHistory.Add(message);
	}
	
}
