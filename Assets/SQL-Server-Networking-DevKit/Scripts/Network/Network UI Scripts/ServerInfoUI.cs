// ===========================================================================================================
//
// Class/Library: Network MatchMaking Panel (UI)
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Apr 28, 2016
//	
// VERS 1.0.000 : Apr 28, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

#if UNITY_EDITOR
	#define		IS_DEBUGGING
#else
	#undef		IS_DEBUGGING
#endif

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using System.Collections;

public class ServerInfoUI : MonoBehaviour 
{

	#region "PRIVATE VARIABLES"

		private string		_strServerName				= "Sample Server";
		private int				_intMaxPlayers				= 2;
		private int				_intCurPlayers				= 0;
		private bool			_blnPasswordRequired	= false;
		private ulong			_networkID;

	#endregion

	#region "PRIVATE PROPERTIES"

		private AppNetworkManager		_nwm			= null;
		private AppNetworkManager		Net
		{
			get
			{
				if (_nwm == null)
						_nwm = AppNetworkManager.Instance;
				return _nwm;
			}
		}

		private GameObject	ServerNameObject
		{
			get
			{
				return transform.GetChild(0).gameObject;
			}
		}
		private GameObject	ServerPlayerObject
		{
			get
			{
				return transform.GetChild(1).gameObject;
			}
		}
		private GameObject	ServerPasswordContainer
		{
			get
			{
				return transform.GetChild(2).gameObject;
			}
		}
		private GameObject	ServerPasswordInputField
		{
			get
			{
				return transform.GetChild(2).GetChild(1).gameObject;
			}
		}
		private GameObject	ServerConnectButton
		{
			get
			{
				return transform.GetChild(3).gameObject;
			}
		}

	#endregion

	#region "PUBLIC PROPERTIES"

		public	string		ServerName
		{
			get
			{
				return _strServerName;
			}
			set
			{
				_strServerName = value.Trim();
			}
		}
		public	int				MaxPlayers
		{
			get
			{
				return _intMaxPlayers;
			}
			set
			{
				_intMaxPlayers = value;
			}
		}
		public	int				CurPlayers
		{
			get
			{
				return _intCurPlayers;
			}
			set
			{
				_intCurPlayers = value;
			}
		}
		public	bool			PasswordRequired
		{
			get
			{
				return _blnPasswordRequired;
			}
			set
			{
				_blnPasswordRequired = value;
			}
		}
		public	ulong			ServerNetworkID
		{
			get
			{
				return (ulong) _networkID;
			}
			set
			{
				_networkID = (ulong)value;
			}
		}

	#endregion

	#region "PRIVATE FUNCTIONS"

		private void			Start()
		{
			Display(true);
		}

	#endregion

	#region "PUBLIC FUNCTIONS"

		public	void			Display(bool blnRest = false)
		{
			ServerNameObject.GetComponent<Text>().text = ServerName;
			ServerPlayerObject.GetComponent<Text>().text = CurPlayers.ToString() + "/" + MaxPlayers.ToString();
			ServerPasswordContainer.SetActive(PasswordRequired);
			ServerPasswordInputField.GetComponent<InputField>().text = "";
			ServerConnectButton.GetComponent<Button>().interactable = CurPlayers < MaxPlayers;
		}

	#endregion

	#region "BUTTON FUNCTIONS"

		public	void			ServerConnectButtonOnClick()
		{
//		if (Net != null && !Net.IsConnected)
//			Net.JoinMatch(  (NetworkID)this.ServerNetworkID, ServerPasswordInputField.GetComponent<InputField>().text);
		}

	#endregion

}
