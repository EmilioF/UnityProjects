using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Engine : MonoBehaviour
{
	public Vector2 scrollPosition;
	public GUISkin style;
	
	public Color currentPlayer;
	
	public List<Color> turnRotation;
	public int turnIndex;
	public int Turns;

	public Color r;
	public Color g;
	public Color y;
	public Color n;
	public bool gameGoing;
	public GameObject board;
	public GameObject Chat;
	public bool threepl;
	public bool twopl;
	
	public GameObject Instructions;
	private bool _chatting;
	private bool _instructions;
	
	// Use this for initialization
	void Start()
	{
		Turns = 0;
		//y = new Color(.163f, .174f, .008f);
		//g = new Color(.043f, .151f, .011f);
		//r = new Color(.174f, .025f, .008f);
		gameGoing = false;
		Chat = GameObject.Find ("Chat");
		_chatting = true;
		_instructions = true;
		currentPlayer = Color.green;
	}
	
	
	
	
	// Update is called once per frame
	void Update()
	{
		if (gameGoing)
		{
			if (currentPlayer == Color.green)
				Camera.main.backgroundColor = g;
			if (currentPlayer == Color.red)
				Camera.main.backgroundColor = r;
			if (currentPlayer == Color.yellow)
				Camera.main.backgroundColor = y;
			board.SetActive(true);
		}
		else
		{
			Camera.main.backgroundColor = n;
			board.SetActive(false);
		}
		if (_instructions)
			Instructions.SetActive(true);
		else
			Instructions.SetActive(false);
	}
	
	void OnGUI()
	{
		GUI.skin = style;
		
		GUI.BeginGroup(new Rect(Screen.width - 100, 20.0f, 100.0f, 500.0f));
		if (GUILayout.Button("SAVE", GUILayout.Width(90)))
		{
			SaveLoad.Save ();
		}
		
		if (_chatting && GUILayout.Button("Hide Chat", GUILayout.Width(90)))
		{
			Chat.GetComponent<Chat>().Display = false;
			_chatting =  false;
		}
		else if (!_chatting && GUILayout.Button("Show Chat", GUILayout.Width(90)))
		{
			Chat.GetComponent<Chat>().Display = true;
			_chatting = true;
		}
		if (!_instructions && GUILayout.Button("Show Tips", GUILayout.Width(90)))
		{
			_chatting = false;
			_instructions = true;
		}
		else if (_instructions && GUILayout.Button("Close Tips", GUILayout.Width(90)))
		{
			_instructions =  false;
		}
		
		
		if (gameGoing)
		{
			if (currentPlayer == GameObject.Find("Player(Clone)").GetComponent<Player>().yourColor)
			{
				if (GUILayout.Button("End Turn", GUILayout.Width(90)))
				{
					GetComponent<NetworkView>().RPC("nextTurn", RPCMode.All);
				}
			}
		}
		GUI.EndGroup();
	}
	
	[RPC]
	public void nextTurn()
	{
		if (threepl)
		{
			if (currentPlayer == Color.green)
			{
				currentPlayer = Color.yellow;
				GameObject.Find("HexGenerator(Clone)").GetComponent<HexGenerator>().newTurn(Color.yellow);
				Chat.GetComponent<Chat>().AddMessage("Yellow's Turn");
			}
			else if (currentPlayer == Color.yellow)
			{
				currentPlayer = Color.red;
				GameObject.Find("HexGenerator(Clone)").GetComponent<HexGenerator>().newTurn(Color.red);
				Chat.GetComponent<Chat>().AddMessage("Red's Turn");
			}
			else if (currentPlayer == Color.red)
			{
				GameObject.Find("HexGenerator(Clone)").GetComponent<HexGenerator>().growTrees();
				GameObject.Find("HexGenerator(Clone)").GetComponent<HexGenerator>().newTurn(Color.green);
				currentPlayer = Color.green;
				Chat.GetComponent<Chat>().AddMessage("Green's Turn");
				GetComponent<NetworkView>().RPC ("updateTurns", RPCMode.AllBuffered);
			}
			else
				print("Error");
		}
		else if (twopl)
		{
			if (currentPlayer == Color.green)
			{
				currentPlayer = Color.yellow;
				GameObject.Find("HexGenerator(Clone)").GetComponent<HexGenerator>().newTurn(Color.yellow);
				Chat.GetComponent<Chat>().AddMessage("Yellow's Turn");
			}
			else if (currentPlayer == Color.yellow)
			{
				GameObject.Find("HexGenerator(Clone)").GetComponent<HexGenerator>().growTrees();
				GameObject.Find("HexGenerator(Clone)").GetComponent<HexGenerator>().newTurn(Color.green);
				currentPlayer = Color.green;
				Chat.GetComponent<Chat>().AddMessage("Green's Turn");
				GetComponent<NetworkView>().RPC ("updateTurns", RPCMode.AllBuffered);
			}
			else
			{
				print("Error");
				//                GameObject.Find("HexGenerator(Clone)").GetComponent<HexGenerator>().newTurn(Color.green);
				//                currentPlayer = Color.green;
				//				Chat.GetComponent<Chat>().AddMessage("Yellow's Turn");
			}
		}
	}
	[RPC]
	public void updatePl(string text, bool state)
	{
		print("Testing turn RPC");
		if (text == "threePl")
			this.threepl = state;
		if (text == "twoPl")
			this.twopl = state;
	}
	[RPC]
	public void updateTurns()
	{
		Turns++;
	}
}