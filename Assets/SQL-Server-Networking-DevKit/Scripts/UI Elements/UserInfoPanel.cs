using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UserInfoPanel : MonoBehaviour 
{

	#region "PRIVATE VARIABLES"

		private User				_user				= null;

		private Text				_username		= null;
		private Text				_realname		= null;
		private Text				_network		= null;
		private GameObject	_btnDisco		= null;

	#endregion

	#region "PRIVATE PROPERTIES"

		private	string			Username
		{
			get
			{
				if (_username == null)
						_username = transform.GetChild(0).GetComponent<Text>();
				return _username.text;
			}
			set
			{
				if (_username == null)
					_username = transform.GetChild(0).GetComponent<Text>();
				_username.text = value.Trim();
			}
		}
		private	string			Realname
		{
			get
			{
				if (_realname == null)
						_realname = transform.GetChild(1).GetComponent<Text>();
				return _realname.text;
			}
			set
			{
				if (_realname == null)
						_realname = transform.GetChild(1).GetComponent<Text>();
				_realname.text = value.Trim();
			}
		}
		private	string			NetworkInfo
		{
			get
			{
				if (_network == null)
						_network = transform.GetChild(2).GetComponent<Text>();
				return _network.text;
			}
			set
			{
				if (_network == null)
						_network = transform.GetChild(2).GetComponent<Text>();
				_network.text = value.Trim();
			}
		}
		private	GameObject	DisconnectButton
		{
			get
			{
				if (_btnDisco == null)
						_btnDisco = transform.GetChild(3).gameObject;
				return _btnDisco;
			}
		}

	#endregion

	#region "PUBLIC PROPERTIES"

		public	User				User
		{
			get
			{
				return _user;
			}
			set
			{
				_user = value;
				UpdatePanel();
			}
		}

	#endregion

	#region "PUBLIC FUNCTIONS"

		public	void				UpdatePanel()
		{
			if (User != null && User.UserID > 0)
			{
				DisconnectButton.SetActive(true);
				Username		= User.Username;
				Realname		= User.RealName;
				NetworkInfo	=	User.NetConnection.address + "  (" + User.NetID.ToString() + ")";
			} else {
				DisconnectButton.SetActive(false);
			}
		}
		public	void				DisconnectButtonClick()
		{
			AppNetworkManager.Instance.ServerKickUser(User.NetConnection);
		}

	#endregion

}
