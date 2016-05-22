// ===========================================================================================================
//
// Class/Library: Network Connect Panel (UI)
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

// APPLICATIONMANAGER IS MANDATORY
// APPNETWORKMANAGER  IS MANDATORY

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkConnectPanel : MonoBehaviour 
{

	#region "PRIVATE VARIABLES"

		private ApplicationManager		_app							= null;
		private AppNetworkManager			_nwm							= null;

		private ProgressBar						_progBar					= null;
		private bool									_blnAutoConnect		= false;
		private bool									_blnIsConnecting	= false;

	#endregion

	#region "PRIVATE PROPERTIES"
	
		private ApplicationManager		App
		{
			get
			{
				if (_app == null)
						_app = ApplicationManager.Instance;
				return _app;
			}
		}
		private AppNetworkManager			Net
		{
			get
			{
				if (_nwm == null)
						_nwm = AppNetworkManager.Instance;
				return _nwm;
			}
		}

		private ProgressBar						ConnectionProgressBar
		{
			get
			{
				if (_progBar == null && ProgressBarObject != null)
						_progBar = ProgressBarObject.GetComponent<ProgressBar>();
				return _progBar;					
			}
		}
		private string								GameVersion
		{
			set
			{
				transform.GetChild(0).GetChild(3).GetComponent<Text>().text = value;
			}
		}

	#endregion

	#region "PUBLIC EDITOR FUNCTIONS"

		public	GameObject			ProgressBarObject	= null;

	#endregion

	#region "PRIVATE FUNCTIONS"

		private void						Start()
		{
			GameVersion = App.GameDisplayString;
			_blnIsConnecting = false;
			if (Net != null)
			{
				_blnAutoConnect = Net.AutoClientConnect;
				Display();				

				if (Net.IsClient && Net.AutoClientConnect)
					StartCoroutine(StartConnection());
			}
		}
		private void						OnDisable()
		{
			_blnIsConnecting = false;
		}

		private void						Display(bool blnHideButtons = false)
		{
			// SHOW THE SERVER TEXT
			transform.GetChild(0).GetChild(0).gameObject.SetActive(Net.IsServer);

			// SHOW THE BUTTONS IF GAME IS IN CLIENT MODE
			transform.GetChild(0).GetChild(1).gameObject.SetActive(!Net.IsServer);
			transform.GetChild(0).GetChild(1).GetChild(0).gameObject.SetActive(!Net.ForceOffline && !_blnIsConnecting);				// CONNECT BUTTON
			transform.GetChild(0).GetChild(1).GetChild(1).gameObject.SetActive(ApplicationManager.Instance.CanWorkOffline);		// WORK OFFLINE BUTTON

			// SHOW THE PROGRESS BAR IF GAME IS IN CLIENT MODE
			if (ProgressBarObject == null)
					ProgressBarObject = transform.GetChild(0).GetChild(2).gameObject;
			ProgressBarObject.SetActive(!Net.IsServer && _blnIsConnecting);
		}

		private IEnumerator			DelayedReconnect()
		{
			yield return new WaitForSeconds(0.1f);
			Start();
		}
		private IEnumerator			StartConnection()
		{
			yield return new WaitForSeconds(0.1f);

			int i = 0;
			ConnectionProgressBar.Progress = 0;
			ConnectionProgressBar.Caption = "Connecting...";
			Util.Timer connClock = new Util.Timer();
			_blnIsConnecting = true;
			Display(true);
			connClock.StartTimer();
			Net.ClientConnect();
			while (!Net.IsConnected && connClock.GetTime <= Net.CONNECTION_TIMEOUT)
			{
				yield return null;
				i++;
				ConnectionProgressBar.SetProgress(connClock.GetFloatTime, Net.CONNECTION_TIMEOUT);
				yield return null;
			}
			connClock.StopTimer();
			_blnIsConnecting	= false;
			_blnAutoConnect		= false;
			ConnectionProgressBar.Caption = "";
			if (!Net.IsConnected && connClock.GetFloatTime > Net.CONNECTION_TIMEOUT)
				Display();
			connClock = null;
		}

	#endregion

	#region "PUBLIC FUNCTIONS"

		public	void			Reconnect()
		{
			Net.ClientDisconnect();
			this.gameObject.SetActive(true);

			ConnectionProgressBar.Progress = 0;
			ConnectionProgressBar.Caption = "Connecting...";
			Display(true);
			StartCoroutine(DelayedReconnect());
		}

	#endregion

	#region "BUTTON FUNCTIONS"

		public	void			OnClientConnectClick()
		{
			if (Net != null)
				StartCoroutine(StartConnection());
		}
		public	void			OnClientOfflineClick()
		{
			// INSTANTIATE AN UNCONNECTED USER/PLAYER OBJECT
			Net.ClientDisconnect();
			App.GameUserObject = (GameObject)GameObject.Instantiate(Net.playerPrefab, Vector3.zero, Quaternion.identity);
			App.GameUser.SetUpUserObject();
			App.IsWorkingOffline = true;
			PanelManager.Instance.ShowLogInPanel();
		}

	#endregion

}
