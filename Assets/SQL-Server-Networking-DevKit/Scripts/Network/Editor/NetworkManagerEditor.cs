using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AppNetworkManager))]
public class NetworkManagerEditor : Editor 
{

	#region "PRIVATE VARIABLES"

		private string[]	strSvrOpt					= { "Client", "Server", "Editor=Server, App=Client", "Editor=Client, App=Server", "Offline Mode" } ;
		private string[]	strSvrIP					= { "Use External IP", "Use Internal IP", "Use Local IP (127.0.0.1)", "Use Custom IP" } ;

		private bool			blnShowNetwork		= true;
		private bool			blnNWMnetInfo			= false;
		private string		strExternalIP			= "";

	#endregion

	public override void OnInspectorGUI()
	{
		AppNetworkManager myTarget = null;
		try { myTarget = (AppNetworkManager)target; } catch { }

		if (myTarget != null)
		{
			GUI.changed = false;

			EditorStyles.foldout.fontStyle = FontStyle.Bold;
			blnShowNetwork = EditorGUILayout.Foldout(blnShowNetwork, "GAME SERVER INFORMATION");
			EditorStyles.foldout.fontStyle = FontStyle.Normal;
			if (blnShowNetwork)
			{
				myTarget.ServerName				= EditorGUILayout.TextField("Server Name",				myTarget.ServerName);
				myTarget.ServerDesc				= EditorGUILayout.TextField("Description",				myTarget.ServerDesc);
				myTarget.ServerPassword		= EditorGUILayout.PasswordField("Password",				myTarget.ServerPassword).Trim();
				myTarget.ServerMode				= EditorGUILayout.Popup(		"Network Mode",				myTarget.ServerMode - 1, strSvrOpt) + 1;
				if (myTarget.ServerMode != 2)
					myTarget.UserIPaddress	= EditorGUILayout.Popup(		"IP Address",					myTarget.UserIPaddress - 1, strSvrIP) + 1;
				switch (myTarget.UserIPaddress)
				{
					case 1:	
						if (strExternalIP == "")
							strExternalIP = AppNetworkManager.GetExternalIP();
						try { EditorGUILayout.LabelField("External IP", strExternalIP); } catch { }
						break;
					case 2:
						try { EditorGUILayout.LabelField("Internal IP", AppNetworkManager.GetInternalIP()); } catch { }
						break;
				}
				if (myTarget.UserIPaddress == 4)
						myTarget.ServerIPaddress	=	EditorGUILayout.TextField(	"Server IP Address",					myTarget.ServerIPaddress);
				myTarget.networkAddress				=	myTarget.ServerIPaddress;
				myTarget.ServerPort						= EditorGUILayout.IntField(		"Server Port",								myTarget.ServerPort);
				myTarget.ServerPort						= Mathf.Clamp(myTarget.ServerPort, 1, 65500);
				myTarget.networkPort					= myTarget.ServerPort;
				myTarget.MaxConnections				=	EditorGUILayout.IntField(		"Max Connections",						myTarget.MaxConnections);
				myTarget.maxConnections				= myTarget.MaxConnections;
				if (myTarget.ServerMode == 1 || myTarget.ServerMode == 3 || myTarget.ServerMode == 4)
					myTarget.UsesMatchMaking		= EditorGUILayout.Toggle(			"Uses MatchMaking",						myTarget.UsesMatchMaking);
				if (myTarget.UsesMatchMaking)
				{
					myTarget.AutoClientConnect			= false;
				} else {
					if (myTarget.ServerMode == 1 || myTarget.ServerMode == 3 || myTarget.ServerMode == 4)
							myTarget.AutoClientConnect	= EditorGUILayout.Toggle("Client Auto Connects",				myTarget.AutoClientConnect);
					if (myTarget.ServerMode == 2 || myTarget.ServerMode == 3 || myTarget.ServerMode == 4)
							myTarget.ServerAlsoPlays		= EditorGUILayout.Toggle("Server is also a Player",			myTarget.ServerAlsoPlays);
				}
				GUILayout.Space(10);


				// MASTER SERVER CONFIGURATION -- DEPRECATED
				myTarget.UseMasterServer				= false;
				/*
				EditorStyles.label.fontStyle = FontStyle.Bold;
				EditorGUILayout.LabelField("MASTER SERVER INFORMATION  (Deprecated)");
				EditorStyles.label.fontStyle = FontStyle.Normal;
				myTarget.UseMasterServer				= EditorGUILayout.Toggle(						"Use Master Server",				myTarget.UseMasterServer);
				if (myTarget.UseMasterServer)
				{
				myTarget.MasterServerIPaddress	= EditorGUILayout.TextField(				"Master Server IP Address",	myTarget.MasterServerIPaddress);
				myTarget.MasterServerPort				= EditorGUILayout.IntField(					"Master Server Port",				myTarget.MasterServerPort);
				} else {
				myTarget.MasterServerIPaddress	= "";
				myTarget.MasterServerPort				= 0;
				}
				GUILayout.Space(10);
				*/

				// PROXY CONFIGURATION -- DEPRECATED
				myTarget.UseProxy								= false;
				/*
				EditorStyles.label.fontStyle = FontStyle.Bold;
				EditorGUILayout.LabelField("PROXY SERVER INFORMATION  (Deprecated)");
				EditorStyles.label.fontStyle = FontStyle.Normal;
				myTarget.UseProxy								= EditorGUILayout.Toggle(						"Use Proxy Server",					myTarget.UseProxy);
				if (myTarget.UseProxy)
				{
				myTarget.ProxyIPaddress					= EditorGUILayout.TextField(				"Proxy Server IP Address",	myTarget.ProxyIPaddress);
				myTarget.ProxyPort							= EditorGUILayout.IntField(					"Proxy Server Port",				myTarget.ProxyPort);
				myTarget.ProxyPassword					= EditorGUILayout.PasswordField(		"Proxy Server Password",		myTarget.ProxyPassword);
				} else {
				myTarget.ProxyIPaddress					= "";
				myTarget.ProxyPort							= 0;
				myTarget.ProxyPassword					= "";
				}
				GUILayout.Space(10);
				*/

				// NAT CONFIGURATION -- DEPRECATED
				myTarget.UseNAT									= false;
				/*
				EditorStyles.label.fontStyle = FontStyle.Bold;
				EditorGUILayout.LabelField("NAT CONFIGURATION  (Deprecated)");
				EditorStyles.label.fontStyle = FontStyle.Normal;
				myTarget.UseNAT									= EditorGUILayout.Toggle(						"Use NAT",									myTarget.UseNAT);
				if (myTarget.UseNAT)
				{
				myTarget.NATFacilitatorIP				= EditorGUILayout.TextField(				"NAT IP Address",						myTarget.NATFacilitatorIP);
				myTarget.NATFacilitatorPort			= EditorGUILayout.IntField(					"NAT Port",									myTarget.NATFacilitatorPort);
				} else {
				myTarget.NATFacilitatorIP				= "";
				myTarget.NATFacilitatorPort			= 0;
				}
				GUILayout.Space(10);
				*/
			}

			// UNET NETWORKMANAGER CONFIGURATION
			EditorStyles.foldout.fontStyle = FontStyle.Bold;
			blnNWMnetInfo = EditorGUILayout.Foldout(blnNWMnetInfo, "UNET NETWORK MANAGER INFORMATION");
			EditorStyles.foldout.fontStyle = FontStyle.Normal;
			if (blnNWMnetInfo)
					DrawDefaultInspector();
			GUILayout.Space(10);

			// CONNECTION STATUS
			#if UNITY_EDITOR
			if (UnityEditor.EditorApplication.isPlaying)
			{
				EditorStyles.label.fontStyle = FontStyle.Bold;
				EditorGUILayout.LabelField("CONNECTION STATUS");
				EditorStyles.label.fontStyle = FontStyle.Normal;

				EditorGUILayout.LabelField("Is Server",			myTarget.IsServer.ToString());
				EditorGUILayout.LabelField("Is Host",				myTarget.IsHost.ToString());
				EditorGUILayout.LabelField("Is Client",			myTarget.IsClient.ToString());
				EditorGUILayout.LabelField("Forced Host",		myTarget.ForceHostMode.ToString());
				EditorGUILayout.LabelField("Forced Offln",	myTarget.ForceOffline.ToString());
				EditorGUILayout.LabelField("Is Connected",	myTarget.IsConnected.ToString());
				EditorGUILayout.LabelField("Is Logged In",	myTarget.IsLoggedIn.ToString());
				EditorGUILayout.LabelField("Match Making",	myTarget.UsesMatchMaking.ToString());
				if (myTarget.UsesMatchMaking)
				{ 
				EditorGUILayout.LabelField("-- Connected",	myTarget.IsMatchMakingConnected.ToString());
				EditorGUILayout.LabelField("-- Host",				myTarget.matchHost.ToString());
				EditorGUILayout.LabelField("-- Name",				myTarget.matchName.ToString());
				EditorGUILayout.LabelField("-- Port",				myTarget.matchPort.ToString());
				EditorGUILayout.LabelField("-- Max Plrs",		myTarget.matchSize.ToString());
				if (myTarget.matchInfo != null)
				{ 
				EditorGUILayout.LabelField("-- Relay?",			myTarget.matchInfo.usingRelay.ToString());
				EditorGUILayout.LabelField("---- IP Addr",	myTarget.matchInfo.address.ToString());
				EditorGUILayout.LabelField("---- Port",			myTarget.matchInfo.port.ToString());
				}
				}
				if ((myTarget.IsServer || myTarget.IsHost) && myTarget.IsConnected)
				EditorGUILayout.LabelField("Users Online",	myTarget.PlayerCount.ToString() + "/" + myTarget.MaxConnections.ToString());
			}
			#endif

			if (GUI.changed)
				EditorUtility.SetDirty(myTarget);
		}
	}
}
