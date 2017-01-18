// ===========================================================================================================
//
// Class/Library: Loading Next Scene / Connecting Panel  (UI)
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Apr 29, 2016
//	
// VERS 1.0.000 : Apr 29, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

#if UNITY_EDITOR
	#define		IS_DEBUGGING
#else
	#undef		IS_DEBUGGING
#endif

// APPNETWORKMANAGER  IS MANDATORY

using UnityEngine;
using System.Collections;

public class LoadingPanel : MonoBehaviour 
{

	#region "PRIVATE VARIABLES"

		private bool								_blnForceConnection		= false;

		private ProgressBar					_connProgBar					= null;
		private AppNetworkManager		_nwm									= null;
	
	#endregion

	#region "PRIVATE PROPERTIES"

		private ProgressBar					ConnectionProgressBar
		{
			get
			{
				if (_connProgBar == null)
						_connProgBar = transform.GetChild(0).GetChild(1).GetComponent<ProgressBar>();
				return _connProgBar;
			}
		}
		private AppNetworkManager		Net
		{
			get
			{
				if (_nwm == null)
						_nwm = AppNetworkManager.Instance;
				return _nwm;
			}
		}

	#endregion

	#region "PRIVATE FUNCTIONS"

		private void						OnEnable()
		{
			StartCoroutine(StartConnection());
		}
		private IEnumerator			StartConnection()
		{
			int i = 0;
			ConnectionProgressBar.Progress = 0;
			ConnectionProgressBar.Caption = "Loading...";
			Util.Timer connClock = new Util.Timer();
			connClock.StartTimer();
			if (_blnForceConnection)
				Net.ClientConnect();
			while (!Net.IsConnected && connClock.GetTime <= Net.CONNECTION_TIMEOUT)
			{
				yield return null;
				i++;
				ConnectionProgressBar.SetProgress(connClock.GetFloatTime, Net.CONNECTION_TIMEOUT);
				yield return null;
			}
			connClock.StopTimer();
			ConnectionProgressBar.Caption = "";
			if (!Net.IsConnected && connClock.GetFloatTime > Net.CONNECTION_TIMEOUT)
			{ 
				ConnectionProgressBar.Caption = "Unable to Load.";
				if (Net.IsClient)
				{ 
					Net.ClientDisconnect();
//				if (Net.UsesMatchMaking)
//				{
//					PanelManager.Instance.ShowMatchMakingPanel();
//					Net.StartMatchMaking();
//				} else
						PanelManager.Instance.ShowConnectPanel();
				}
			}
			connClock = null;
		}

	#endregion

}
