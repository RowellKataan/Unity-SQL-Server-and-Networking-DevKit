//		AUTO-GENERATED FILE: User.cs
//		GENERATED ON       : Sun Apr 24 2016 - 06:02:31 PM
//		
//		This is the Class file.  This is the file that you can modify.
//		It will not automatically be changed by the system going forward.


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Data;

public class User : UserBase
{

	#region "PRIVATE VARIABLES"

		protected bool					_blnIsAdmin			= false;

		// MANDATORY - GAME VARIABLES
		protected bool					_blnInitialized	= false;
		protected bool					_blnChangeMade	= false;
		protected string				_strCryptoKey		= "ABCDEFGHabcdefgh1234!@#$";
		protected GameObject		_gameObject			= null;

//	private	GameManager			_gmm						= null;
		private LogInPanel			_loginManager		= null;
		private StatusManager		_statusManager	= null;
		private UserManager			_usm						= null;

		private int							_netConnID			= -1;

	#endregion

	#region "PRIVATE PROPERTIES"

/*
		protected GameManager						Game
		{
			get
			{
				if (_gmm == null)
					_gmm = GameManager.Instance;
				return _gmm;
			}
		}
*/
		protected UserManager						Users
		{
			get
			{
				if (_usm == null)
						_usm = UserManager.Instance;
				return _usm;
			}
		}

		protected	LogInPanel						LoginManager
		{
			get
			{
				if (_loginManager == null)
						_loginManager = GameObject.FindObjectOfType<LogInPanel>();
				return _loginManager;
			}
		}
		protected	StatusManager					Status
		{
			get
			{
				if (_statusManager == null)
						_statusManager = StatusManager.Instance;
				return _statusManager;
			}
		}

	#endregion

	#region "PUBLIC PROPERTIES"

		public		bool					IsAdmin
		{
			get
			{
				return _blnIsAdmin;
			}
		}
		public		string				RealName
		{
			get
			{
				return (FirstName.Trim() + " " + LastName.Trim()).Trim();
			}
		}

	#endregion

	#region "PUBLIC SEARCH FUNCTIONS"

		public	static	User		FindByID(int _intFind)
		{
			if (UserManager.Instance != null)
				return UserManager.Instance.Users.Find(x => x.UserID == _intFind);
			else
				return null;
		}
		public	static	User		FindByUserID(int _intFind)
		{
			if (UserManager.Instance != null)
				return UserManager.Instance.Users.Find(x => x.UserID == _intFind);
			else
				return null;
		}
		public	static	User		FindByUsername(string _strFind)
		{
			if (UserManager.Instance != null)
				return UserManager.Instance.Users.Find(x => x.Username.ToLower() == _strFind.ToLower());
			else
				return null;
		}
		public	static	User		FindByEmailAddress(string _strFind)
		{
			if (UserManager.Instance != null)
				return UserManager.Instance.Users.Find(x => x.EmailAddress.ToLower() == _strFind.ToLower());
			else
				return null;
		}
		public	static	User		FindByUserType(UserTypes _enFind)
		{
			if (UserManager.Instance != null)
				return UserManager.Instance.Users.Find(x => x.UserType == _enFind);
			else
				return null;
		}

	#endregion

	#region "PRIVATE FUNCTIONS"

		private string			CreateNewPassword()
		{
			int			intPassword			= (Random.Range(1, 9999) + 10000) + System.DateTime.Now.Minute;
			string	strNewPassword	= intPassword.ToString();
			for (int i = 0; i < 10; i++)
			{
				intPassword			= Random.Range(0, 74) + 48;
				while ((intPassword > 90 && intPassword < 97) ||
							 (intPassword > 57 && intPassword < 64))
					intPassword		= Random.Range(0, 74) + 48;
				strNewPassword	= System.Convert.ToChar(intPassword) + strNewPassword;
			}
			return strNewPassword;
		}

	#endregion

	#region "PUBLIC FUNCTIONS"

		public						void	LogUserIntoDatabase(string strUsername, string strPassword)
		{
			if (Database.IsConnected)
			{
				this.LoadByUsername(strUsername, false);
				if (this.UserID > 0)
				{
					if (!this.IsActive)
					{
						LoginManager.StatusMessage	= "User Account is Inactive.";
						LoginManager.DisplayPanel(true);
						return;
					} else if (this.BanDate.AddDays(this.BanDays) > System.DateTime.Now) {
						LoginManager.StatusMessage	= "User Account is Banned until " + this.BanDate.AddDays(this.BanDays).ToString("ddd MMM dd yyyy") + " 11:59:59 PM";
						LoginManager.DisplayPanel(true);
						return;
					} else if ((App.UserLogInType == 1 && this.Password == strPassword) || App.UserLoginType == 2) {
						this.SetUpUserObject();
						App.UserGameObject					= this.gameObject;
						LoginManager.StatusMessage	= "Log In Successful";
						App.IsLoggedIn							= true;
					} else {
						LoginManager.StatusMessage	= "Invalid Password.";
						return;
					}
				} else {
					if (App.AutoCreateAccount && App.UserLoginType == 2)
					{
						// AUTOMATICALLY CREATE THE ACCOUNT
						this.Reset();
						this.Username								= strUsername;
						this.EmailAddress						= strUsername;
						this.Save();
						this.gameObject.name				= this.Username + " (-1)";
						App.UserGameObject					= this.gameObject;
						LoginManager.StatusMessage	= "Log In Successful";
						App.IsLoggedIn							= true;
						return;
					} else {
						LoginManager.StatusMessage	= "User Account does not Exist.";
					}
				}
			}
		}
		public						void	SetUpUserObject()
		{
			// CHANGE THE NAME OF THE OBJECT
			if (this.Username != "")
				gameObject.name = this.Username;
			else 
				gameObject.name = "Player";
			if (!App.IsWorkingOffline && Net.IsConnected)
				gameObject.name += " (" + this.netId.ToString() + ")";
			else
				gameObject.name += " (Local)";

			// MOVE THE PLAYER OBJECT INTO THE APPROPRIATE PLAYER CONTAINER
			gameObject.transform.SetParent(Net.PlayerContainer.transform);
		}

	#endregion

	#region "EVENT FUNCTIONS"

		private		void					OnDisable()
		{
			// IF THE CAMERA IS ATTACHED TO THE USER OBJECT, DETACH IT BEFORE DISABLING/DESTROYING THE USER OBJECT
			if (!isLocalPlayer)
				return;

			Camera cam = Camera.main;
			if (cam == null)
					cam = GameObject.FindObjectOfType<Camera>();
			if (cam != null && cam.transform.parent.gameObject == this.gameObject)
					cam.transform.parent = null;
		}

	#endregion

	#region "DATABASE FUNCTIONS"

		public	override	void	PopulateClass(DataRow dr)
		{
			// YOU CAN COMMENT THIS LINE OUT, AND BUILD YOUR OWN CUSTOM POPULATE CLASS FUNCTION
			base.PopulateClass(dr);
		}
		public	override	bool	LoadByID(int intID, bool blnActiveOnly = false)
		{
			// YOU CAN COMMENT THIS LINE OUT, AND BUILD YOUR OWN CUSTOM LOADBYID FUNCTION
			return base.LoadByID(intID, blnActiveOnly);
		}
		public	override	bool	LoadByUserID(int xFind, bool blnActiveOnly = true)
		{
			// YOU CAN COMMENT THIS LINE OUT, AND BUILD YOUR OWN CUSTOM LoadByUserID FUNCTION
			return base.LoadByUserID(xFind, blnActiveOnly);
		}
		public	override	bool	LoadByUsername(string xFind, bool blnActiveOnly = true)
		{
			// YOU CAN COMMENT THIS LINE OUT, AND BUILD YOUR OWN CUSTOM LoadByUsername FUNCTION
			return base.LoadByUsername(xFind, blnActiveOnly);
		}
		public	override	bool	LoadByEmailAddress(string xFind, bool blnActiveOnly = true)
		{
			// YOU CAN COMMENT THIS LINE OUT, AND BUILD YOUR OWN CUSTOM LoadByEmailAddress FUNCTION
			return base.LoadByEmailAddress(xFind, blnActiveOnly);
		}
		public	override	bool	LoadByUserType(UserTypes xFind, bool blnActiveOnly = true)
		{
			// YOU CAN COMMENT THIS LINE OUT, AND BUILD YOUR OWN CUSTOM LoadByUserType FUNCTION
			return base.LoadByUserType(xFind, blnActiveOnly);
		}
		public	override	bool	Save()
		{
			// YOU CAN COMMENT THIS LINE OUT, AND BUILD YOUR OWN CUSTOM SAVE FUNCTION
			return base.Save();
		}

	#endregion

	#region "NETWORK FUNCTIONS -- SERVER COMMANDS & CLIENT RPC'S"

		// BASICALLY, THIS CLASS HANDLES ALL COMMUNICATIONS FROM THE USER TO THE SERVER
		// THESE COMMUNICATIONS ARE HANDLED BELOW

		#region "MANDATORY NETWORKING FUNCTIONS  -- DO NOT REMOVE"

			#region "CONNECTION FUNCTIONS"
		
				#region "GAME VERSION CHECK"

					// CLIENT-TO-SERVER: CHECK CLIENT'S GAME VERSION
					[Command]
					public	void			CmdGameVersionCheck(string strVersion)
					{
						strVersion = Crypto.Decrypt(strVersion);
						if (strVersion == App.GameCodeString)
							RpcGameVersionCheck(true, "");
						else
							RpcGameVersionCheck(false, App.GameVersion);
					}
					// SERVER-TO-CLIENT: SERVER RESPONSE WITH PROPER VERSION
					[ClientRpc]
					public	void			RpcGameVersionCheck(bool blnSuccess, string strVersion)
					{
						if (!isLocalPlayer)
							return;
						LoginManager.LoginIsActive = blnSuccess;
						if (!blnSuccess)
							LoginManager.StatusMessage = "Your Game Version is Invalid.\nCurrent Version is: " + strVersion;
						LoginManager.DisplayPanel(true);
					}

				#endregion

			#endregion

			#region "LOG IN PANEL NETWORK FUNCTIONS"

				#region "LOG IN PANEL: USER IS LOGGING IN (USERNAME/PASSWORD)"

					// CLIENT-TO-SERVER: THIS IS EXECUTED BY THE SERVER
					[Command]
					public	void			CmdLogUserIn(string strUsername, string strPassword)
					{ 
						strUsername = Crypto.Decrypt(strUsername);
						strPassword = Crypto.Decrypt(strPassword);
						this.LoadByUsername(strUsername, false);
						if (this.UserID > 0)
						{
							// USER ACCOUNT EXISTS.  MAKE SURE THAT IT IS ACTIVE AND NOT BANNED
							if (!this.IsActive)
								RpcLogUserIn(false, "User Account is Inactive.");
							else if (this.BanDate.AddDays(this.BanDays) > System.DateTime.Now)
								RpcLogUserIn(false, "User Account is Banned until " + this.BanDate.AddDays(this.BanDays).ToString("ddd MMM dd yyyy") + " 11:59:59 PM");
							else if ((App.UserLogInType == 1 && this.Password == strPassword) || App.UserLoginType == 2)
							{
								// SUCCESS: LOG THE USER IN
								Status.Status = this.Username + " has Logged On.";
								User temp = new User();
								temp.Clone(this);
								temp.Password			= "1";
								temp.EmailAddress	= "2";
								temp.FirstName		= "3";
								temp.LastName			= "4";
								this.gameObject.name = this.Username + " (" + this.netId.ToString() + ")";

								// ADD THE USER TO THE USERMANAGER LIST
								if (UserManager.Instance != null)
										UserManager.Instance.Add(this);
								else
										Debug.Log("UserManager does not Exist.");

								// SEND THE USERS A MODIFIED VERSION OF THE USER DATA THAT DOES NOT INCLUDE PERSONAL OR PASSWORD INFORMATION
								RpcLogUserIn(true, Crypto.Encrypt(temp.Serialize()));
							}
							else
								RpcLogUserIn(false, "Invalid Password.");
						} else {
							// IF USER DOES NOT EXIST, AND THE LOG IN TYPE IS BY WINDOWS USERNAME,
							// AND APPLICATIONMANAGER IS SET UP TO AUTOCREATE ACCOUNTS:
							// CREATE THE NEW USER ACCOUNT IN THE DATABASE AND LOG THE USER IN
							if (App.AutoCreateAccount && App.UserLoginType == 2)
							{
								// AUTOMATICALLY CREATE THE ACCOUNT
								Status.Status = "Auto-Create Account for " + this.Username + ".";
								this.Reset();
								this.Username			= strUsername;
								this.EmailAddress = strUsername;
								this.Save();
								this.gameObject.name = this.Username + " (" + this.netId.ToString() + ")";

								// ADD THE USER TO THE USERMANAGER LIST
								if (UserManager.Instance != null)
									UserManager.Instance.Add(this);

								// SEND THE USERS A MODIFIED VERSION OF THE USER DATA THAT DOES NOT INCLUDE PERSONAL OR PASSWORD INFORMATION
								RpcLogUserIn(true, Crypto.Encrypt(this.Serialize()));
							} else
								RpcLogUserIn(false, "User Account does not Exist.");
						}
					}
					// SERVER-TO-CLIENT: THIS IS EXECUTED BY THE CLIENTS
					[ClientRpc]
					public	void			RpcLogUserIn(bool blnSuccess, string strMessage)
					{
						if (blnSuccess)
						{
							// LOG IN WAS SUCCESSFUL.
							// DESERIALIZE THE INCOMING DATA TO POPULATE THE USER CLASS
							// MARK THE USER AS LOGGED IN
							this.Deserialize(Crypto.Decrypt(strMessage));
							this.SetUpUserObject();
							if (isLocalPlayer)
							{
								App.IsLoggedIn = true;
								LoginManager.StatusMessage = "Log In is Successful";
							}
						} else {
							// LOG IN WAS UNSUCCESSFUL. DISPLAY THE MESSAGE AND CLEAR THE FIELDS.
							if (isLocalPlayer)
							{
								LoginManager.StatusMessage = strMessage;
								LoginManager.DisplayPanel(true);
							}
						}
					}

				#endregion

				#region "LOG IN PANEL: FORGOT/RESET PASSWORD"

					// CLIENT-TO-SERVER: THIS IS EXECUTED BY THE SERVER
					[Command]
					public	void			CmdResetPassword(string strEmail)
					{
						strEmail = Crypto.Decrypt(strEmail);
						this.LoadByEmailAddress(strEmail);
						if (this.UserID > 0)
						{
							if (!this.IsActive)
								RpcResetPassword(false, "User Account is Inactive.");
							else if (this.BanDate.AddDays(this.BanDays) > System.DateTime.Now)
								RpcResetPassword(false, "User Account is Banned until " + this.BanDate.AddDays(this.BanDays).ToString("DDD MMM dd yyyy"));
							else
							{
								this.Password = CreateNewPassword();
								this.Save();
								Status.Status = this.Username + " has Reset their Password.";
								RpcResetPassword(true, "Password has been Reset. (An Email has been sent to you).");
							}
						} else
							RpcResetPassword(false, "User Account does not Exist.");
					}
					// SERVER-TO-CLIENT: THIS IS EXECUTED BY THE CLIENTS
					[ClientRpc]
					public	void			RpcResetPassword(bool blnSuccess, string strMessage)
					{
						if (isLocalPlayer)
						{
							if (blnSuccess)
							{
								LoginManager.StatusMessage = strMessage;
								LoginManager.DisplayPanel(true);
							} else {
								LoginManager.StatusMessage = strMessage;
								LoginManager.DisplayPanel(true);
							}
						}
					}

				#endregion

				#region "LOG IN PANEL: USER IS SIGNING UP (USERNAME/PASSWORD/EMAIL ADDRESS)"

					// CLIENT-TO-SERVER: THIS IS EXECUTED BY THE SERVER
					[Command]
					public	void			CmdNewUserSignUp(string strUsername, string strPassword, string strEmail)
					{
						strUsername = Crypto.Decrypt(strUsername);
						strPassword = Crypto.Decrypt(strPassword);
						strEmail		= Crypto.Decrypt(strEmail);
						User temp		= new User();
						int intFound = 0;
						intFound += Util.ConvertToInt(temp.LoadByUsername(strUsername, false));
						intFound += Util.ConvertToInt(temp.LoadByEmailAddress(strEmail, false)) * 2;
						switch(intFound)
						{
							case 0:	// ACCOUNT DOES NOT EXIST. OKAY TO CREATE
								Status.Status = strUsername + " has Created a New Account.";
								this.Reset();
								this.Username			= strUsername;
								this.Password			= strPassword;
								this.EmailAddress = strEmail;
								this.UserType			= UserTypes.Normal;
		//					this.IsActive			= false;
								this.Save();
								RpcNewUserSignUp(true, "Account Created. (Check your Email to Activate the Account.)");
								break;
							case 1:	// USERNAME ALREADY EXISTS.
								RpcNewUserSignUp(false, "Username already Exists.");
								break;
							case 2:	// EMAIL ADDRESS ALREADY EXISTS.
								RpcNewUserSignUp(false, "Email Address already Exists.");
								break;
						}
						temp = null;
					}
					// SERVER-TO-CLIENT: THIS IS EXECUTED BY THE CLIENTS
					[ClientRpc]
					public	void			RpcNewUserSignUp(bool blnSuccess, string strMessage)
					{
						if (isLocalPlayer)
						{
							LoginManager.StatusMessage = strMessage;
							LoginManager.DisplayPanel(true);
						}
					}

				#endregion

			#endregion

			#region "GET PLAYER COUNT (CONNECTED TO SERVER)"

				public	void				RequestPlayerCount()
				{
					if (isLocalPlayer)
						StartCoroutine(DelayedRequestPlayerCount());
				}
				private IEnumerator	DelayedRequestPlayerCount()
				{
					yield return new WaitForSeconds(0.50f);
					CmdGetPlayerCount(this.NetID, this.UserID);
				}

				[Command]
				public	void				CmdGetPlayerCount(int intConnID, int intUserID)
				{
					AppNetworkManager.Instance.ServerLog(this.Username + " requested Connected Player Count.");
					RpcDisplayPlayerCount(intConnID, intUserID, AppNetworkManager.Instance.PlayerCount);
				}
				[ClientRpc]
				public	void				RpcDisplayPlayerCount(int intConnID, int intUserID, int intPlayerCount)
				{
					Status.Status = "There are " + intPlayerCount.ToString() + " Player(s) Connected.";
				}

			#endregion

			#region "KICK ALL PLAYERS OFF OF SERVER"

				public	void				RequestPlayerKick()
				{
					if (isLocalPlayer && this.IsAdmin)
						StartCoroutine(DelayedRequestPlayerKick());
				}
				private IEnumerator	DelayedRequestPlayerKick()
				{
					yield return new WaitForSeconds(0.50f);
					CmdPlayerKick(this.NetID, this.UserID);
				}
				private IEnumerator	ResetServer()
				{
					yield return new WaitForSeconds(0.50f);
					AppNetworkManager.Instance.ServerDisconnect();
					AppNetworkManager.Instance.ServerLog("Server will restart in 5 seconds...");

					yield return new WaitForSeconds(5.00f);
					StartCoroutine(App.DoServerStart()); 
				}

				[Command]
				public	void				CmdPlayerKick(int intConnID, int intUserID)
				{
					if (this.IsAdmin)
					{
						AppNetworkManager.Instance.ServerLog(this.Username + " requested All Players Kicked.");
						StartCoroutine(ResetServer());
					}
				}

			#endregion

			#region "SHUT DOWN ALL APPLICATIONS (INCLUDING THE SERVER)"

				public	void				RequestServerShutdown()
				{
					if (isLocalPlayer && this.IsAdmin)
						StartCoroutine(DelayedRequestServerShutdown());
				}
				private IEnumerator DelayedRequestServerShutdown()
				{
					yield return new WaitForSeconds(0.50f);
					CmdRequestServerShutdown(this.NetID, this.UserID);
				}

				[Command]
				public	void				CmdRequestServerShutdown(int intConnID, int intUserID)
				{
					AppNetworkManager.Instance.ServerLog(this.Username + " requested Shutdown of Game Network.");
					Net.GracefulServerShutdown();
				}
				[ClientRpc]
				public	void				RpcRequestServerShutdown(int intConnID, int intUserID)
				{
					Net.IsQuitting = true;
					Status.Status = "Server requested that Applications Quit (for maintenance).";
					Net.ClientDisconnect();
					App.QuitApplication();
				}

			#endregion

			#region "SEND SYSTEM-WIDE MESSAGE"

				public	void				SendSystemMessage(string strMessage)
				{
					if (isLocalPlayer)
						StartCoroutine(DelayedSendSystemMessage(strMessage));
				}
				private IEnumerator DelayedSendSystemMessage(string strMessage)
				{
					yield return new WaitForSeconds(0.50f);
					CmdSystemMessage(this.NetID, this.UserID, strMessage);
				}

				[Command]
				public	void				CmdSystemMessage(int intConnID, int intUserID, string strMessage)
				{
					if (strMessage.Trim() != "")
					{
						AppNetworkManager.Instance.ServerLog(this.Username + ": " + strMessage);
						strMessage = "[" + this.Username + "] : " + strMessage;

						if (Users.Users.Count > 0)
						{
							for (int i = 0; i < Users.Users.Count; i++)
							{
								Users.Users[i].RpcSystemMessage(Users.Users[i].NetConnection.connectionId, intUserID, strMessage);
							}
						}
					}
				}
				[ClientRpc]
				public	void				RpcSystemMessage(int intConnID, int intUserID, string strMessage)
				{
					if (strMessage.Trim() != "")
					{
						Status.Status = strMessage;
					}
				}

			#endregion

		#endregion

		#region "GAME SPECIFIC FUNCTIONS -- YOU CAN ADD/MODIFY ANYTHING IN HERE"


		#endregion

	#endregion

}

