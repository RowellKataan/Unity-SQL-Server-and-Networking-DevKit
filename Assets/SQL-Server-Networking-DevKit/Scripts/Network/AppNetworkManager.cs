// ===========================================================================================================
//
// Class/Library: NetworkManager (Singleton Class)
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Apr 21, 2016
//	
// VERS 1.0.000 : Apr 21, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

#if UNITY_EDITOR
	#define		IS_DEBUGGING
#else
	#undef		IS_DEBUGGING
#endif

#define	USES_APPLICATIONMANAGER		// #define = Scene has an ApplicationManager Prefab,	#undef = Scene does not have an ApplicationManager Prefab
#define	USES_STATUSMANAGER				// #define = Scene has a  StatusManager Prefab,				#undef = Scene does not have a  StatusManager Prefab

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class AppNetworkManager : NetworkManager
{

	#region "PUBLIC CONSTANTS"

		[HideInInspector]
		public	int													MAX_SERVER_LOG_LENGTH		= 4000;		// CHARACTERS
		[HideInInspector]
		public	int													MM_SERVERS_PER_PAGE			= 100;

	#endregion

	#region "PRIVATE VARIABLES"

		private	static	AppNetworkManager		_instance					= null;

		#if USES_APPLICATIONMANAGER
		private					ApplicationManager	_app							= null;
		#endif

		#if USES_STATUSMANAGER
		private					StatusManager				_stm							= null;
		#endif

		private bool												_blnIsReady				= false;
		private bool												_blnForceOffline	= false;
		private bool												_blnForceHostMode	= false;
		private bool												_blnInitializing	= false;
		private bool												_blnInitialized		= false;
		private bool												_blnIsConnected		= false;
		private bool												_blnWasConnected	= false;
		private string											_strNetError			= "";
		private int													_intNetConnectTmr	= 0;

		private NetworkClient								_networkClient		= null;
		private string											_strClientID			= "";

		private GameObject									_canvas						= null;
		private GameObject									_pgc							= null;

	#endregion

	#region "PRIVATE PROPERTIES"

		#if USES_APPLICATIONMANAGER
		private ApplicationManager					App
		{
			get
			{
				if (_app == null)
						_app = ApplicationManager.Instance;
				return _app;
			}
		}
		#endif
		#if USES_STATUSMANAGER
		private StatusManager								Status
		{
			get
			{
				if (_stm == null)
						_stm = StatusManager.Instance;
				return _stm;
			}
		}
		#endif

		private GameObject									Canvas
		{
			get
			{
				if (_canvas == null)
						_canvas = GameObject.Find("Canvas").gameObject;
				return _canvas;
			}
		}

		private bool												IsWinRT
		{
			get
			{
				#if UNITY_4_6
					return	Application.platform == RuntimePlatform.MetroPlayerARM ||
									Application.platform == RuntimePlatform.MetroPlayerX86 ||
									Application.platform == RuntimePlatform.MetroPlayerX64;
				#else
					return	Application.platform == RuntimePlatform.WSAPlayerARM ||
									Application.platform == RuntimePlatform.WSAPlayerX86 ||
									Application.platform == RuntimePlatform.WSAPlayerX64;
				#endif
			}
		}

	#endregion

	#region "PUBLIC PROPERTIES"

		public	static	AppNetworkManager	Instance
		{
			get
			{
				return GetInstance();
			}
		}
		public	static	AppNetworkManager	GetInstance()
		{
			if (_instance == null)
					_instance = (AppNetworkManager)GameObject.FindObjectOfType(typeof(AppNetworkManager));
			return _instance;
		}

		public	float										CONNECTION_TIMEOUT
		{
			get
			{
				return ((float)this.connectionConfig.ConnectTimeout * (this.connectionConfig.MaxConnectionAttempt - 1)) / 1000f;
			}
		}

		public	NetworkClient						MyClient
		{ 
			get
			{
				return _networkClient;
			}
		}
		public	string									NetworkClientID
		{
			get
			{
				if (_strClientID == "")
				{
					if (PlayerPrefs.GetString("cliendId") == string.Empty || PlayerPrefs.GetString("cliendId") == "")
					{
						_strClientID	= string.Empty;
						string pool		= "1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
						for(int i = 0; i < 100; i++)
						{
							_strClientID += pool[Random.Range(0, pool.Length)];
						}
						PlayerPrefs.SetString("cliendId", _strClientID);
					} else
						_strClientID = PlayerPrefs.GetString("cliendId");
				}
				return _strClientID;
			}
		}

		public	bool										ForceOffline
		{
			get
			{
				return _blnForceOffline;
			}
			set
			{
				_blnForceOffline = value;
			}
		}
		public	bool										ForceHostMode
		{
			get
			{
				return _blnForceHostMode;
			}
			set
			{
				_blnForceHostMode = value;
			}
		}
		public	bool										IsWorkingOffline
		{
			get
			{
				return _blnForceOffline || App.IsWorkingOffline;
			}
			set
			{ 
				App.IsWorkingOffline = value;
			}
		}
		public	bool										IsServer
		{
			get
			{
				if (UsesMatchMaking)
					return false;
				switch (ServerMode)
				{
					case 1:		// CLIENT MODE
						_blnForceOffline = false;
						return false;
					case 2:		// SERVER MODE
						_blnForceOffline = false;
						return true;
					case 3:		// EDITOR=SERVER, COMPILE=CLIENT
						#if UNITY_EDITOR
							_blnForceOffline = false;
							return true;
						#else
							_blnForceOffline = false;
							return false;
						#endif
					case 4:		// EDITOR=CLIENT, COMPILE=SERVER
						#if !UNITY_EDITOR
							_blnForceOffline = false;
							return true;
						#else
							_blnForceOffline = false;
							return false;
						#endif
					default:
						_blnForceOffline = true;
						return false;
				}
			}
		}
		public	bool										IsHost
		{
			get
			{
				return (IsServer && ServerAlsoPlays) || _blnForceHostMode;
			}
		}
		public	bool										IsClient
		{
			get
			{
				return !IsServer  && !_blnForceHostMode;				// (!IsServer || (IsServer && ServerAlsoPlays)) && !_blnForceHostMode
			}
		}
		public	bool										IsReady
		{
			get
			{
				return _blnIsReady;
			}
		}
		public	bool										IsConnected
		{
			get
			{
				return _blnIsConnected || IsWorkingOffline;
			}
		}
		public	bool										IsMatchMakingConnected
		{
			get
			{
				if (!UsesMatchMaking)
					return false;
				return NetworkManager.singleton != null && NetworkManager.singleton.matchMaker != null;
			}
		}
		public	bool										IsLoggedIn
		{
			get
			{
				#if USES_APPLICATIONMANAGER
				return (App != null && App.IsLoggedIn) || IsServer;
				#else
				return IsConnected || IsServer;
				#endif
			}
			#if USES_APPLICATIONMANAGER
			set
			{
				App.IsLoggedIn = value;
			}
			#endif
		}
		public	int											PlayerCount
		{
			get
			{
				return this.numPlayers;
			}
		}
		public	string									NetErrorMessage
		{
			get
			{
				return _strNetError;
			}
		}
		public	int											NetConnectTimer
		{
			get
			{
				return _intNetConnectTmr;
			}
		}

		public	void										ResetErrorMessage()
		{
			_intNetConnectTmr = 0;
			_strNetError = "";
		}

		public	GameObject							PlayerContainer
		{
			get
			{
				if (_pgc == null)
						_pgc = GameObject.Find("Player List");
				return _pgc;
			}
		}

	#endregion

	#region "PUBLIC EDITOR PROPERTIES"

		// THIS SERVER INFORMATION
		[SerializeField, HideInInspector]
		public	string			ServerName								= "Server-001";
		[SerializeField, HideInInspector]
		public	string			ServerDesc								= "Demo Game Server";
		[SerializeField, HideInInspector]
		public	string			ServerPassword						= "";
		[SerializeField, HideInInspector]
		public	int					ServerMode								= 1;
		[SerializeField, HideInInspector]
		public	int					UserIPaddr								= 3;
		[SerializeField, HideInInspector]
		public	string			ServerIPaddress						= "127.0.0.1";
		[SerializeField, HideInInspector]
		public	int					ServerPort								= 80;
		[SerializeField, HideInInspector]
		public	int					MaxConnections						= 32;
		[SerializeField, HideInInspector]
		public	bool				AutoClientConnect					= false;
		[SerializeField, HideInInspector]
		public	bool				ServerAlsoPlays						= true;
		[SerializeField, HideInInspector]
		public	bool				UsesMatchMaking						= false;

		// MASTER SERVER INFORMATION	-- DEPRECATED
		[SerializeField, HideInInspector]
		public	bool				UseMasterServer						= false;
		[SerializeField, HideInInspector]
		public	string			MasterServerIPaddress			= "";
		[SerializeField, HideInInspector]
		public	int					MasterServerPort					= 0;

		// PROXY INFORMATION	-- DEPRECATED
		[SerializeField, HideInInspector]
		public	bool				UseProxy									= false;
		[SerializeField, HideInInspector]
		public	string			ProxyIPaddress						= "";
		[SerializeField, HideInInspector]
		public	string			ProxyPassword							= "";
		[SerializeField, HideInInspector]
		public	int					ProxyPort									= 0;

		// NAT INFORMATION	-- DEPRECATED
		[SerializeField, HideInInspector]
		public	bool				UseNAT										= false;
		[SerializeField, HideInInspector]
		public	string			NATFacilitatorIP					= "";
		[SerializeField, HideInInspector]
		public	int					NATFacilitatorPort				= 0;

		#if UNITY_STANDALONE_LINUX
		public bool autoStartServer = true;
		#endif

		public	int											UserIPaddress
		{
			get
			{
				return UserIPaddr;
			}
			set
			{
				UserIPaddr = value;
			}
		}

	#endregion
	
	#region "PUBLIC FUNCTIONS"

		#region "NETWORKING IDENTITY FUNCTIONS"

			public	static string GetInternalIP()
			{
				IPHostEntry host;
				string localIP = "";
				host = Dns.GetHostEntry(Dns.GetHostName());
				foreach (IPAddress ip in host.AddressList)
				{
					if (ip.AddressFamily == AddressFamily.InterNetwork)
					{
						localIP = ip.ToString();
						break;
					}
				}
				return localIP;
			}
			public	static string GetExternalIP()
			{
				try
				{
					string direction = "";
					WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
					using (WebResponse response = request.GetResponse())
					using (StreamReader stream = new StreamReader(response.GetResponseStream()))
					{
						direction = stream.ReadToEnd();
					}
					int first = direction.IndexOf("Address: ") + 9;
					int last = direction.LastIndexOf("</body>");
					direction = direction.Substring(first, last - first);
					return direction;
				} catch {
					return "N/A";
				}
			}
			public	string				GetServerIPaddress()
			{
				string strIP = ServerIPaddress;
				switch (UserIPaddress)
				{
					case 1:		// USE EXTERNAL IP ADDRESS
						strIP = GetExternalIP();
						break;
					case 2:		// USE INTERNAL IP ADDRESS
						strIP = GetInternalIP();
						break;
					case 3:		// USE LOCAL IP ADDRESS
						strIP = "127.0.0.1";
						break;
					default:
						if (strIP == "")
							strIP = GetExternalIP();
						break;
				}
				if (strIP == "N/A")
					strIP = "127.0.0.1";
				return strIP;
			}

		#endregion

		#region "SERVER FUNCTIONS"

			// STAND-ALONE SERVER
			public	void					ServerStart()
			{
				if (!IsServer)
					return;
				if (UsesMatchMaking)
				{ 
					StartMatchMaker();
					return;
				}

				_blnInitializing	= true;
				_blnInitialized		= false;
				_blnIsConnected		= false;
				IsLoggedIn				= false;
				#if USES_STATUSMANAGER
				Status.Status			= "Starting Server...";
				Status.UpdateStatus();
				#endif

				NetworkManager.singleton.StartServer();
			}
			public	void					ServerDisconnect()
			{
				#if USES_STATUSMANAGER
				Status.Status			= "Stopping Server...";
				Status.UpdateStatus();
				#endif
				NetworkManager.singleton.StopServer();
			}
			public	void					ServerKickUser(NetworkConnection conn)
			{
				conn.Disconnect();
			}

			// HOST = SERVER+CLIENT
			public	void					HostStart()
			{
				if (!IsHost)
					return;
				if (UsesMatchMaking)
				{ 
					StartMatchMaker();
					return;
				} else
					PanelManager.Instance.ShowLoadingPanel();

				_blnInitializing	= true;
				_blnInitialized		= false;
				_blnIsConnected		= false;
				IsLoggedIn				= false;
				#if USES_STATUSMANAGER
				Status.Status			= "Starting Host...";
				Status.UpdateStatus();
				#endif

				_networkClient		= NetworkManager.singleton.StartHost();
			}
			public	void					HostDisconnect()
			{
				#if USES_STATUSMANAGER
				Status.Status			= "Stopping Host...";
				Status.UpdateStatus();
				#endif
				NetworkManager.singleton.StopHost();
			}

		#endregion

		#region "CLIENT FUNCTIONS"

			public	void					ClientConnect()
			{
				if (!IsClient)
					return;
				if (UsesMatchMaking)
				{ 
					StartMatchMaker();
					return;
				}

				_blnInitializing	= true;
				_blnInitialized		= false;
				_blnIsConnected		= false;
				IsLoggedIn				= false;
				#if USES_STATUSMANAGER
				Status.Status			= "Starting Client...";
				Status.UpdateStatus();
				#endif

				_networkClient		= NetworkManager.singleton.StartClient();
			}
			public	void					ClientConnect(MatchInfo matchInfo)
			{
				if (!UsesMatchMaking)
				{ 
					ClientConnect();
					return;
				}

				_blnInitializing	= true;
				_blnInitialized		= false;
				_blnIsConnected		= false;
				IsLoggedIn				= false;
				#if USES_STATUSMANAGER
				Status.Status			= "Starting MatchMaking...";
				Status.UpdateStatus();
				#endif

				_networkClient		= NetworkManager.singleton.StartClient(matchInfo);
			}
			public	void					ClientDisconnect()
			{
				#if USES_STATUSMANAGER
				Status.Status			= "Stopping Client...";
				Status.UpdateStatus();
				#endif
				NetworkManager.singleton.StopClient();
				_blnWasConnected = false;
			}

		#endregion

		#region "MATCH MAKING FUNCTIONS"

			public	void					StartMatchMaking()
			{
				if (!UsesMatchMaking)
					return;

				PanelManager.Instance.ShowMatchMakingPanel();
				NetworkManager.singleton.StartMatchMaker();
			}
			public	void					StartMatchHost(string strServerName, string strPassword)
			{
				_blnInitializing	= true;
				_blnInitialized		= false;
				_blnIsConnected		= false;
				_blnForceHostMode	= true;
				IsLoggedIn				= false;

				NetworkManager.singleton.matchMaker.CreateMatch(strServerName, (uint)this.MaxConnections, true, strPassword, OnMatchCreate);
			}
			public	void					GetHostList()
			{
				NetworkManager.singleton.matchMaker.ListMatches(0, MM_SERVERS_PER_PAGE, "", OnMatchList);
			}
			public	void					UpdateMatchList(List<MatchDesc> matches)
			{
				if (PanelManager.Instance.MatchMakingPanelObject == null)
					return;

				MatchMakingPanel	mmp = PanelManager.Instance.MatchMakingPanelObject.GetComponent<MatchMakingPanel>();
				mmp.ClearServerList();
				for(int i = 0; i < matches.Count; i++)
				{
					mmp.AddToServerList(matches[i].name, matches[i].networkId, matches[i].isPrivate, matches[i].maxSize, matches[i].currentSize);
				}
			}
			public	void					JoinMatch(NetworkID networkID, string strPassword)
			{
				PanelManager.Instance.ShowLoadingPanel();
				NetworkManager.singleton.matchMaker.JoinMatch(networkID, strPassword, OnJoinMatch);
			}

		#endregion

	#endregion

	#region "EVENT FUNCTIONS"

		#region "APPLICATION EVENTS"

			protected void					Awake ()
			{
				// INITIALIZE SINGLETON INSTANCES
				this.dontDestroyOnLoad		= true;
				NetworkManager.singleton	= GetInstance();
				#if USES_APPLICATIONMANAGER
				_app = ApplicationManager.Instance;
				#endif
				#if USES_STATUSMANAGER
				_stm = StatusManager.Instance;
				#endif

				// SET THE NETWORKMANAGER AS READY FOR USE
				_blnIsReady = true;
				#if USES_STATUSMANAGER
				Status.UpdateStatus();
				#endif
			}

			private		void					OnApplicationQuit()
			{
				_blnInitializing	= false;
				_blnInitialized		= false;
				_blnIsConnected		= false;
				IsLoggedIn				= false;
				#if USES_STATUSMANAGER
				Status.UpdateStatus();
				#endif
			}

		#endregion

		#region "SERVER EVENTS"

			public	override	void	OnServerConnect(NetworkConnection conn)
			{
				base.OnServerConnect(conn);
				#if USES_STATUSMANAGER
				Status.Status = "Player has Connected (" + conn.address + " / " + conn.connectionId.ToString() + ")";
				Status.UpdateStatus();
				#endif
			}
			public	override	void	OnServerDisconnect(NetworkConnection conn)
			{
				#if USES_STATUSMANAGER
				if (conn != null)
				{
					if (conn.address != "")
						Status.Status = "Player has Disconnected (" + conn.address + " / " + conn.connectionId.ToString() + ")";
					else
						Status.Status = "Player has Disconnected (" + conn.connectionId.ToString() + ")";

					if (UserManager.Instance != null)
					{
						User us = UserManager.Instance.Users.Find(x => x.NetConnection == conn);
						if (us != null)
							UserManager.Instance.Remove(us);
					}
				} else
					Status.Status = "Player has Disconnected";
				Status.UpdateStatus();
				#endif

				base.OnServerDisconnect(conn);
			}
			public	override	void	OnServerReady(NetworkConnection conn)
			{
				base.OnServerReady(conn);
				#if USES_STATUSMANAGER
				Status.Status = "Client #" + conn.connectionId.ToString() + " is Ready";
				Status.UpdateStatus();
				#endif
			}
			public	override	void	OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
			{ 
				GameObject player = (GameObject)GameObject.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
				NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
				player.name				= "Player-" + playerControllerId.ToString();
				player.GetComponent<User>().NetConnection = conn;
				player.transform.SetParent(this.PlayerContainer.transform);
				#if USES_STATUSMANAGER
				Status.Status			= "Adding Player ( " + conn.address + " / " + conn.connectionId.ToString() + ") to List";
				Status.UpdateStatus();
				#endif
			}
			public						void	OnServerRemovePlayer(NetworkConnection conn, short playerControllerId)
			{
				return;

				PlayerController player;
				player = conn.playerControllers.Find(x => x.playerControllerId == playerControllerId);
				base.OnServerRemovePlayer(conn, player);
			}
			public	override	void	OnServerError(NetworkConnection conn, int errorCode)
			{
				base.OnServerError(conn, errorCode);
				#if USES_STATUSMANAGER
				Status.Status = "Server Error (CODE: " + errorCode.ToString() + ")";
				Status.UpdateStatus();
				#endif
			}
			public	override	void	OnServerSceneChanged(string strSceneName)
			{
				base.OnServerSceneChanged(strSceneName);
				#if USES_STATUSMANAGER
				Status.Status = "Server Changing Scene to: " + strSceneName;
				Status.UpdateStatus();
				#endif
			}

			public	override	void	OnStartHost()
			{
				base.OnStartHost();
				_blnInitializing	= false;
				_blnInitialized		= true;
				_blnIsConnected		= true;
				IsLoggedIn				= true;
				#if USES_STATUSMANAGER
				Status.Status			= "Host has Started.";
				Status.UpdateStatus();
				#endif
				PanelManager.Instance.ShowLogInPanel();
			}
			public	override	void	OnStopHost()
			{
				base.OnStopHost();
				_blnInitializing	= false;
				_blnInitialized		= false;
				_blnIsConnected		= false;
				IsLoggedIn				= false;
				#if USES_STATUSMANAGER
				Status.Status = "Host has Stopped.";
				Status.UpdateStatus();
				#endif
			}
			public	override	void	OnStartServer()
			{
				base.OnStartServer();
				_blnInitializing	= false;
				_blnInitialized		= true;
				_blnIsConnected		= true;
				IsLoggedIn				= true;
				#if USES_STATUSMANAGER
				Status.Status = "Server has Started.";
				Status.UpdateStatus();
				#endif
				PanelManager.Instance.ShowConnectPanel();
			}
			public	override	void	OnStopServer()
			{
				base.OnStopServer();
				_blnInitializing	= false;
				_blnInitialized		= false;
				_blnIsConnected		= false;
				IsLoggedIn				= false;
				#if USES_STATUSMANAGER
				Status.Status = "Server has Stopped.";
				Status.UpdateStatus();
				#endif
			}


		#endregion

		#region "CLIENT EVENTS"

			public	override	void	OnClientConnect(NetworkConnection conn)
			{
				base.OnClientConnect(conn);
				_blnInitializing	= false;
				_blnInitialized		= true;
				_blnIsConnected		= true;
				IsLoggedIn				= false;
				#if USES_STATUSMANAGER
				Status.Status			= "Connected!";
				Status.UpdateStatus();
				#endif
				PanelManager.Instance.ShowLogInPanel();
			}
			public	override	void	OnClientDisconnect(NetworkConnection conn)
			{
				base.OnClientDisconnect(conn);
				if (!App.IsWorkingOffline)
				{
					_blnForceHostMode	= false;
					_blnInitializing	= false;
					_blnInitialized		= false;
					_blnIsConnected		= false;
					IsLoggedIn				= false;
					#if USES_STATUSMANAGER
					Status.Status			= "Disconnected from Server";
					Status.UpdateStatus();
					#endif

					// ATTEMPT TO RE-CONNECT / RE-DISPLAY THE NETWORK CONNECTION PANEL
					if (_blnWasConnected)
					{ 
						PanelManager.Instance.ShowConnectPanel();
						PanelManager.Instance.ConnectPanelObject.GetComponent<NetworkConnectPanel>().Reconnect();
					}
				}
			}
			#if !UNITY_WEBGL
			private						void	OnDisconnectedFromServer(NetworkDisconnection info)
			{
				#if USES_STATUSMANAGER
				if (Network.isServer)
				{
					Status.Status = "Local server connection disconnected";
        } else {
					if (info == NetworkDisconnection.LostConnection)
					{
						Status.Status = "Lost connection to the server";
					} else {
						Status.Status			= "Successfully diconnected from the server";
					}
				}
				Status.UpdateStatus();
				#endif
			}
			#endif
			public	override	void	OnClientError(NetworkConnection conn, int errorCode)
			{
				base.OnClientError(conn, errorCode);
				#if USES_STATUSMANAGER
				Status.Status = "Client Error: CODE(" + errorCode.ToString() + ")";
				Status.UpdateStatus();
				#endif
			}
			public	override	void	OnClientNotReady(NetworkConnection conn)
			{
				base.OnClientNotReady(conn);
				#if USES_STATUSMANAGER
				Status.Status = "Client Not Ready";
				Status.UpdateStatus();
				#endif
			}
			public	override	void	OnClientSceneChanged(NetworkConnection conn)
			{
				ClientScene.Ready(conn);
				base.OnClientSceneChanged(conn);
				#if USES_STATUSMANAGER
				Status.Status = "Changing Scene...";
				Status.UpdateStatus();
				#endif
			}
			public	override	void	OnStartClient(NetworkClient client)
			{
				base.OnStartClient(client);
				if (!App.IsWorkingOffline)
				{
					_blnInitializing	= false;
					_blnInitialized		= true;
					_blnIsConnected		= false;
					IsLoggedIn				= false;
					#if USES_STATUSMANAGER
					Status.Status = "Client has Started.";
					Status.UpdateStatus();
					#endif
				}
			}
			public	override	void	OnStopClient()
			{
				base.OnStopClient();
				if (!App.IsWorkingOffline)
				{
					_blnWasConnected	= _blnIsConnected || IsLoggedIn;
					_blnInitializing	= false;
					_blnInitialized		= false;
					_blnIsConnected		= false;
					IsLoggedIn				= false;
					#if USES_STATUSMANAGER
					Status.Status = "Client is Stopped.";
					Status.UpdateStatus();
					#endif
				}
			}

		#endregion
 
		#region "MATCH MAKING EVENTS"

			public	void					OnMatchCreate(CreateMatchResponse matchResponse)
			{
				if(matchResponse.success)
				{
					MatchInfo info =  new MatchInfo(matchResponse);
					PanelManager.Instance.ShowLoadingPanel();
					Utility.SetAccessTokenForNetwork(matchResponse.networkId, new NetworkAccessToken(matchResponse.accessTokenString));
					_networkClient = NetworkManager.singleton.StartHost(info);
					ForceHostMode = true;
				}
			}
			public	void					OnMatchList(ListMatchResponse res)
			{
				if (res.success)
					UpdateMatchList(res.matches);
				else
				{
					#if IS_DEBUGGING
					Debug.Log("Unable to obtain Server List");
					#endif
				}
			}
			public	void					OnJoinMatch(JoinMatchResponse matchJoin)
			{
				if(matchJoin.success)
				{
					PanelManager.Instance.ShowLoadingPanel();
					Utility.SetAccessTokenForNetwork(matchJoin.networkId, new NetworkAccessToken(matchJoin.accessTokenString));
					_networkClient = NetworkManager.singleton.StartClient(new MatchInfo(matchJoin));
				}
			}

		#endregion

	#endregion 
	
}
