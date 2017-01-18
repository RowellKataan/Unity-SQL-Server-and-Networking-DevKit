// ===========================================================================================================
//
// Class/Library: Demo Script - Shows how to access the Network
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Jan 09, 2017
//	
// VERS 1.0.000 : Jan 09, 2017 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NetworkDemoScript : MonoBehaviour 
{

	#region "PRIVATE PROPERTIES"

		private AppNetworkManager		_nwm		= null;	
		private AppNetworkManager		Net
		{
			get
			{
				if (_nwm == null)
						_nwm = AppNetworkManager.Instance;
				return _nwm;
			}
		}

		private GameObject				_canvas	= null;
		private GameObject				Canvas
		{
			get
			{
				if (_canvas == null)
						_canvas = GameObject.Find("Canvas");
				return _canvas;
			}
		}

		private string						ResultText
		{
			get
			{
				return Canvas.transform.GetChild(3).GetComponent<Text>().text;
			}
			set
			{
				Canvas.transform.GetChild(3).GetComponent<Text>().text = value.Trim();
			}
		}

	#endregion

	#region "PRIVATE FUNCTIONS"

		private void							DisconnectFromNetwork()
		{
			if (Net.IsConnected)
			{
				if (Net.IsClient)
				{
					Debug.Log("Disconnect Client");
					Net.ClientDisconnect();
				} else {
					if (Net.IsHost)
						Debug.Log("Disconnect Host");
					else
						Debug.Log("Disconnect Server");
					Net.ServerDisconnect();
				}
			} else
				Debug.Log("Not Connected");

			transform.GetChild(0).GetComponent<Text>().text = "Connect";
		}
		private IEnumerator				ConnectToNetwork()
		{
			yield return new WaitForSeconds(0.5f);
			if (Net.IsClient)
				PanelManager.Instance.ShowConnectPanel();
			else if (Net.ServerAlsoPlays)
				Net.HostStart();
			else	
				Net.ServerStart();
			transform.GetChild(0).GetComponent<Text>().text = "Disconnect";
		}

	#endregion
		
	#region "PUBLIC FUNCTIONS"

		public	void							DisconnectButton()
		{
			if (Net.IsConnected)
				DisconnectFromNetwork();
			else
				StartCoroutine(ConnectToNetwork());
		}
		public	void							WriteLogButton()
		{
			ApplicationManager.Instance.WriteDebugLog();
		}

	#endregion

}
