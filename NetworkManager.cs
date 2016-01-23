using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour
{
	
	public string registerHostName = "The Ultimate Medieval Warfare Game";
	public string gameName = "Xoey's Game";//"142.157.34.81";
	public int portNumber = 8436;
	
	public GUISkin style;
	
	public GameObject hex;
	public GameObject playerPrefab;
	public GameObject myPlayer;
	public bool _gameOngoing;
	public int connectedPlayers;
	
	public bool greenChosen;
	public bool redChosen;
	public bool yellowChosen;
	public bool hasChosen;
	bool isRefreshing = false;
	float refreshRequestLength = 3.0f;
	HostData[] hostData;
	
	public GameObject Chat;
	
	public NetworkView nView;
	
	// Use this for initialization
	void Start()
	{
		myPlayer = GameObject.Find ("Player(Clone)");
		if (myPlayer == null)
		{
			myPlayer = (GameObject)Instantiate(playerPrefab);
			myPlayer.GetComponent<Player>().name = "NO_NAME";
		}
		Chat = GameObject.Find ("Chat");
		greenChosen = false;
		yellowChosen = false;
		redChosen = false;
		hasChosen = false;
		connectedPlayers = 0;
		nView = GetComponent<NetworkView>();
		_gameOngoing = false;
	}
	
	// Update is called once per frame
	void Update() { 
		
		if (_gameOngoing && connectedPlayers == 1)
		{
			GameObject.Find("Player(Clone)").GetComponent<Player>().WonGame = true;
		}
	}
	
	private void StartServer()
	{
		Network.InitializeServer(4, portNumber, false);
		MasterServer.RegisterHost(registerHostName,gameName, "Test Implementation of Servr Code");
	}
	
	void OnServerInitialized()
	{
		Debug.Log("Server has been initialized");
	}
	
	void OnMasterServerEvent(MasterServerEvent masterServerEvent)
	{
		if (masterServerEvent == MasterServerEvent.RegistrationSucceeded)
			Debug.Log("Registration Successful");
	}
	
	void OnDisconnectedFromServer()
	{
		nView.RPC("Message", RPCMode.AllBuffered, "A Player Disconnected");
		_gameOngoing = false;
		Application.LoadLevel("hexmap");
	}
	
	public IEnumerator RefreshHostList()
	{
		Debug.Log("Refreshing...");
		MasterServer.RequestHostList(registerHostName);
		float timeStarted = Time.time;
		float timeEnd = Time.time + refreshRequestLength;
		
		while (Time.time < timeEnd)
		{
			hostData = MasterServer.PollHostList();
			yield return new WaitForEndOfFrame();
		}
		
		if (hostData == null || hostData.Length == 0)
			Debug.Log("No active servers have been found.");
		else
			Debug.Log("Has been found");
	}
	void OnGUI()
	{
		GUI.skin = style;
		// Displays no matter what
		if (!_gameOngoing)
		{
			GUI.BeginGroup( new Rect(Screen.width*0.25f, 10f, Screen.width*0.5f, 30f));
			GUI.Label(new Rect(0, 0, Screen.width*0.5f, 30f), "<size=20> Welcome " + myPlayer.GetComponent<Player>().name + "!  </size>");
			GUI.EndGroup ();
		}
		if (!Network.isClient && !Network.isServer)
		{
			
			gameName = GUILayout.TextField(gameName);
			int.TryParse(GUILayout.TextField(portNumber.ToString()), out portNumber);
			
			
			
			if (GUILayout.Button("Host"))
				StartServer();
			if (GUILayout.Button("Refresh Host List"))
				StartCoroutine("RefreshHostList");
			if (hostData != null)
			{
				foreach (HostData hostDatum in hostData)
				{
					if (GUILayout.Button(hostDatum.gameName))
					{
						Network.Connect(hostDatum);
					}
				}
			}
		}
		
		else
		{
			if (!_gameOngoing)
				GUILayout.Label("Connections: " + (Network.connections.Length+1).ToString());
			
			if (!_gameOngoing && !hasChosen)
			{
				if (!greenChosen )
				{
					if (GUILayout.Button("GreenPlayer"))
					{
						nView.RPC("updateBool", RPCMode.AllBuffered, "green", true);
						hasChosen = true;
						myPlayer.GetComponent<Player>().yourColor = Color.green;
						Chat.GetComponent<Chat>().htmlcolor = "#D3FFAD";
						nView.RPC("updateConnected", RPCMode.AllBuffered);
					}
				}
				if (!yellowChosen && greenChosen)
				{
					if (GUILayout.Button("YellowPlayer"))
					{
						nView.RPC("updateBool", RPCMode.AllBuffered, "yellow", true);
						hasChosen = true;
						myPlayer.GetComponent<Player>().yourColor = Color.yellow;
						Chat.GetComponent<Chat>().htmlcolor = "#FFF8C7";
						nView.RPC("updateConnected", RPCMode.AllBuffered);
					}
				}
				//				if (Network.connections.Length > 1)
				//				{
				if (!redChosen && greenChosen && yellowChosen)
				{
					if (GUILayout.Button("RedPlayer"))
					{
						myPlayer.GetComponent<Player>().yourColor = Color.red;
						nView.RPC("updateBool", RPCMode.AllBuffered, "red", true);
						Chat.GetComponent<Chat>().htmlcolor = "#FFC7DC";
						hasChosen = true;
						nView.RPC("updateConnected", RPCMode.AllBuffered);
					}
				}
				//				}
			}
			
			if (Network.isServer && !_gameOngoing) // && connectedPlayers == Network.connections.Length+1)
			{
				if (Network.connections.Length == 1 && (connectedPlayers == Network.connections.Length+1))
				{
					if (GUILayout.Button("Start Game (2 Players)"))
					{
						Chat.GetComponent<Chat>().AddMessage("Game Started");
						nView.RPC("Message", RPCMode.AllBuffered, "Game Begins");
						Network.Instantiate(hex, Vector3.zero, Quaternion.identity, 1);
						nView.RPC("updateBool", RPCMode.AllBuffered, "game", true);
						GameObject.Find("Engine").GetComponent<Engine>().Instructions.SetActive(false);
						GameObject.Find("Engine").GetComponent<Engine>().Instructions.SetActive(false);
						GameObject.Find("Engine").GetComponent<NetworkView>().RPC("updatePl", RPCMode.AllBuffered, "threePl", false);
						GameObject.Find("Engine").GetComponent<NetworkView>().RPC("updatePl", RPCMode.AllBuffered, "twoPl", true);
						
						//GameObject.Find("Engine").GetComponent<Engine>().GetComponent<NetworkView>().RPC("initTurnRotation", RPCMode.AllBuffered);
						
					}
				}
				else if (Network.connections.Length == 2 && (connectedPlayers == Network.connections.Length + 1))
				{
					if (GUILayout.Button("Start Game (3 Players)"))
					{
						Chat.GetComponent<Chat>().AddMessage("Game Started");
						nView.RPC("Message", RPCMode.AllBuffered, "Game Begins");
						Network.Instantiate(hex, Vector3.zero, Quaternion.identity, 1);
						nView.RPC("updateBool", RPCMode.AllBuffered, "game", true);
						GameObject.Find("Engine").GetComponent<Engine>().Instructions.SetActive(false);
						GameObject.Find("Engine").GetComponent<Engine>().Instructions.SetActive(false);
						GameObject.Find("Engine").GetComponent<NetworkView>().RPC("updatePl", RPCMode.AllBuffered, "threePl", true);
						GameObject.Find("Engine").GetComponent<NetworkView>().RPC("updatePl", RPCMode.AllBuffered, "twoPl", false);
						
						//GameObject.Find("Engine").GetComponent<Engine>().GetComponent<NetworkView>().RPC("initTurnRotation", RPCMode.AllBuffered);
					}
				}
			}
		}
	}
	
	[RPC]
	public void updateBool(string color, bool state){
		if (color == "red")
			this.redChosen = state;
		if (color == "yellow")
			this.yellowChosen = state;
		if (color == "green")
			this.greenChosen = state;
		if (color == "game")
		{
			this._gameOngoing = state;
			GameObject.Find("Engine").GetComponent<Engine>().gameGoing = state;
		}
	}
	
	[RPC]
	public void updateConnected()
	{
		connectedPlayers++;
	}
	
}


