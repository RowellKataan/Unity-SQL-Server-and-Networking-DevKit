// ===========================================================================================================
//
// Class/Library: Network/Database Log In Panel (UI)
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Apr 24, 2016
//	
// VERS 1.0.000 : Apr 24, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

#if UNITY_EDITOR
	#define		IS_DEBUGGING
#else
	#undef		IS_DEBUGGING
#endif

// APPLICATIONMANAGER IS MANDATORY
// APPNETWORKMANAGER  IS MANDATORY
// DATABASEMANAGER    IS OPTIONAL
// STATUSMANAGER			IS OPTIONAL

#define		USES_DATABASEMANAGER
#define		USES_STATUSMANAGER

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using System.Collections;

public class LogInPanel : NetworkBehaviour
{

	#region "PRIVATE VARIABLES"

		private bool									_blnIsSigningUp				= false;
		private bool									_blnIsForgetting			= false;
		private bool									_blnConnected					= false;
		private bool									_blnLoginIsActive			= false;
		private bool									_blnCheckGameVersion	= false;

	#endregion

	#region "PRIVATE PROPERTIES"

		private ApplicationManager		_app							= null;
		private AppNetworkManager			_nwm							= null;
		#if USES_DATABASEMANAGER
		private	DatabaseManager				_dbm							= null;
		#endif
		#if USES_STATUSMANAGER
		private StatusManager					_stm							= null;
		#endif

		private ApplicationManager		App
		{
			get
			{
				if (_app == null)
						_app = ApplicationManager.Instance;
				#if IS_DEBUGGING
				if (_app == null)
					Debug.LogError("ApplicationManager Instance is Missing");
				#endif
				return _app;
			}
		}
		private AppNetworkManager			Net
		{
			get
			{
				if (_nwm == null)
						_nwm = AppNetworkManager.Instance;
				#if IS_DEBUGGING
				if (_nwm == null)
					Debug.LogError("AppNetworkManager Instance is Missing");
				#endif
				return _nwm;
			}
		}
		#if USES_DATABASEMANAGER
		private DatabaseManager				Database
		{
			get
			{
				if (_dbm == null)
						_dbm = DatabaseManager.Instance;
				#if IS_DEBUGGING
				if (_dbm == null)
					Debug.LogError("DatabaseManager Instance is Missing");
				#endif
				return _dbm;
			}
		}
		#endif
		#if USES_STATUSMANAGER
		private StatusManager					Status
		{
			get
			{
				if (_stm == null)
						_stm = StatusManager.Instance;
				#if IS_DEBUGGING
				if (_stm == null)
					Debug.LogError("StatusManager Instance is Missing");
				#endif
				return _stm;
			}
		}
		#endif

		private GameObject						UsernameContainer
		{
			get
			{
				return transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
			}
		}
		private GameObject						PasswordContainer
		{
			get
			{
				return transform.GetChild(0).GetChild(1).GetChild(1).gameObject;
			}
		}
		private GameObject						EmailAddressContainer
		{
			get
			{
				return transform.GetChild(0).GetChild(1).GetChild(3).gameObject;
			}
		}
		private GameObject						ForgotPasswordContainer
		{
			get
			{
				return transform.GetChild(0).GetChild(1).GetChild(4).gameObject;
			}
		}

		private GameObject						UsernameLabel
		{
			get
			{
				return transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).gameObject;
			}
		}
		private GameObject						PasswordLabel
		{
			get
			{
				return transform.GetChild(0).GetChild(1).GetChild(1).GetChild(0).gameObject;
			}
		}
		private GameObject						EmailAddressLabel
		{
			get
			{
				return transform.GetChild(0).GetChild(1).GetChild(3).GetChild(0).gameObject;
			}
		}
		private GameObject						UsernameInput
		{
			get
			{
				return transform.GetChild(0).GetChild(1).GetChild(0).GetChild(1).gameObject;
			}
		}
		private GameObject						PasswordInput
		{
			get
			{
				return transform.GetChild(0).GetChild(1).GetChild(1).GetChild(1).gameObject;
			}
		}
		private GameObject						EmailAddressInput
		{
			get
			{
				return transform.GetChild(0).GetChild(1).GetChild(3).GetChild(1).gameObject;
			}
		}
		private GameObject						ForgotPasswordButton
		{
			get
			{
				return transform.GetChild(0).GetChild(1).GetChild(4).GetChild(0).gameObject;
			}
		}
		private GameObject						LoginButton
		{
			get
			{
				return transform.GetChild(0).GetChild(1).GetChild(5).GetChild(0).GetChild(0).gameObject;
			}
		}
		private GameObject						SignUpButton
		{
			get
			{
				return transform.GetChild(0).GetChild(1).GetChild(5).GetChild(0).GetChild(1).gameObject;
			}
		}
		private GameObject						OfflineButton
		{
			get
			{
				return transform.GetChild(0).GetChild(1).GetChild(5).GetChild(0).GetChild(2).gameObject;
			}
		}
		private GameObject						QuitButton
		{
			get
			{
				return transform.GetChild(0).GetChild(1).GetChild(5).GetChild(0).GetChild(3).gameObject;
			}
		}
		private GameObject						StatusMessageLabel
		{
			get
			{
				return transform.GetChild(0).GetChild(2).gameObject;
			}
		}
		private GameObject						ResetPasswordLabel
		{
			get
			{
				return transform.GetChild(0).GetChild(1).GetChild(2).gameObject;
			}
		}
		
		private string								GameVersion
		{
			set
			{
				transform.GetChild(1).GetComponent<Text>().text = value;
			}
		}
		
	#endregion

	#region "PUBLIC PROPERTIES"

		public	string		StatusMessage
		{
			set
			{
				StatusMessageLabel.GetComponent<Text>().text = value.Trim();
			}
		}
		public	bool			LoginIsActive
		{
			get
			{
				return _blnLoginIsActive;
			}
			set
			{
				_blnLoginIsActive = value;
			}
		}

	#endregion

	#region "PRIVATE FUNCTIONS"

		private void			OnEnable()
		{
			GameVersion		= App.GameDisplayString;
			StatusMessage = "";
			LoginIsActive = Net.IsWorkingOffline;
			if (App.IsLoggedIn || (Net.IsServer && !Net.IsHost))
				this.gameObject.SetActive(false);
		}
		private void			Update()
		{
			// IF SERVER OR LOGGED IN, SHUT THIS PANEL DOWN
			if (App.IsLoggedIn || (Net.IsServer && !Net.IsHost))
				this.gameObject.SetActive(false);

			// IF THERE'S A CHANGE IN CONNECTION STATE (CONNECTED to DISCONNECTED), DISPLAY THE PANEL
			if (_blnConnected != Net.IsConnected)
			{
				this.gameObject.SetActive(true);
				_blnConnected = Net.IsConnected;
				DisplayPanel();
			}

			// FIRST TIME THE PANEL IS DISPLAYED AND A CONNECTION EXISTS, CHECK GAME VERSION WITH SERVER
			if (_blnConnected && !Net.IsWorkingOffline && !_blnCheckGameVersion)
			{
				if (App.GameUser != null)
				{
					_blnCheckGameVersion = true;
					App.GameUser.CmdGameVersionCheck(Crypto.Encrypt(App.GameCodeString));
				}
			}
		}

		private void			LogInViaNetwork()
		{
			// ENCRYPT THE USERNAME AND PASSWORD, SEND TO THE SERVER TO LOG IN
			if (Net.IsConnected)
			{
				if (App.GameUser != null)
				{
					string strUsername = UsernameInput.GetComponent<InputField>().text.Trim();
					string strPassword = PasswordInput.GetComponent<InputField>().text.Trim();

					if (strUsername == "" || (App.UserLoginType == 1 && strPassword == ""))
						StatusMessage = "All Fields must be Completed.";
					else
					{ 
						strUsername = Crypto.Encrypt(strUsername);
						strPassword = Crypto.Encrypt(strPassword);
						App.GameUser.CmdLogUserIn(strUsername, strPassword);
					}
				} else
					StatusMessage = "Unable to Communicate with the Server";
			} else
				StatusMessage = "Not Connected to the Server";
		}
		private void			LogInViaDatabase()
		{
			// IF ABLE TO (SETTINGS IN APPLICATIONMANAGER AND DATABASEMANAGER),
			// TRY LOGGING IN BY CHECKING USERNAME AND PASSWORD AGAINST THE DATABASE.
			#if USES_DATABASEMANAGER

			if (Database.ClientsCanUse && App.CanWorkOffline)
			{
				if (App.GameUser != null)
				{
					string strUsername = UsernameInput.GetComponent<InputField>().text.Trim();
					string strPassword = PasswordInput.GetComponent<InputField>().text.Trim();

					if (strUsername == "" || (App.UserLoginType == 1 && strPassword == ""))
						StatusMessage = "All Fields must be Completed.";
					else
						App.GameUser.LogUserIntoDatabase(strUsername, strPassword);
				} else
					StatusMessage = "Unable to Communicate with the Database";
			} else
				StatusMessage = "Clients are not authorized to Connect in Offline Mode.";
			
			#else

			StatusMessage = "Database is not Available";
			
			#endif
		}
		private void			SignUpViaNetwork()
		{
			// ENCRYPT THE USERNAME, PASSWORD AND EMAIL ADDRESS.
			// SEND TO THE SERVER TO CREATE A NEW USER ACCOUNT.
			if (Net.IsConnected)
			{
				if (App.GameUser != null)
				{
					string strUsername	= UsernameInput.GetComponent<InputField>().text.Trim();
					string strPassword	= PasswordInput.GetComponent<InputField>().text.Trim();
					string strEmail			= EmailAddressInput.GetComponent<InputField>().text.Trim();

					if (strUsername == "" || strPassword == "" || strEmail == "")
						StatusMessage = "All Fields must be Completed.";
					else
					{ 
						strUsername = Crypto.Encrypt(strUsername);
						strPassword = Crypto.Encrypt(strPassword);
						strEmail		= Crypto.Encrypt(strEmail);
						App.GameUser.CmdNewUserSignUp(strUsername, strPassword, strEmail);
					}
				} else
					StatusMessage = "Unable to Communicate with the Server";
			} else
				StatusMessage = "Not Connected to the Server";
		}
		private void			ResetPasswordViaNetwork()
		{
			// ENCRYPT THE EMAIL ADDRESS AND SEND TO THE SERVER TO RESET THE USER'S PASSWORD.
			if (Net.IsConnected)
			{
				if (App.GameUser != null)
				{
					string strEmail			= EmailAddressInput.GetComponent<InputField>().text.Trim();

					if (strEmail == "")
						StatusMessage = "All Fields must be Completed.";
					else
					{ 
						strEmail		= Crypto.Encrypt(strEmail);
						App.GameUser.CmdResetPassword(strEmail);
					}
				} else
					StatusMessage = "Unable to Communicate with the Server";
			} else
				StatusMessage = "Not Connected to the Server";

		}

	#endregion

	#region "PUBLIC FUNCTIONS"

		public	void			DisplayPanel(bool blnClearFields = false)
		{
			#if USES_STATUSMANAGER
			Status.UpdateStatus();
			#endif

			if (blnClearFields)
			{
				if (UsernameInput.GetComponent<InputField>().interactable)
						UsernameInput.GetComponent<InputField>().text = "";
				PasswordInput.GetComponent<InputField>().text = "";
				EmailAddressInput.GetComponent<InputField>().text = "";
			}

			if (Net == null || !Net.IsConnected || App.IsLoggedIn)
			{
				// TURN EVERYTHING OFF
				this.GetComponent<Image>().enabled = false;
				for (int i = 0; i < this.transform.childCount; i++)
				{
					this.transform.GetChild(0).GetChild(i).gameObject.SetActive(false);
				}
			} else {
				// TURN EVERYTHING ON
				this.GetComponent<Image>().enabled = true;
				for (int i = 0; i < this.transform.childCount - 1; i++)
				{
					this.transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
				}

				// MODIFY WHICH BUTTONS/INPUTFIELDS ARE ACTIVE BASED ON THE STATE
				if (App.UserLoginType == 2)		// WINDOWS USERNAME LOGIN TYPE
				{
					UsernameInput.GetComponent<InputField>().interactable = false;
					PasswordContainer.SetActive(false);
					EmailAddressContainer.SetActive(false);
					ResetPasswordLabel.SetActive(false);
					ForgotPasswordContainer.SetActive(false);
					SignUpButton.SetActive(false);
					LoginButton.SetActive(!App.IsWorkingOffline && !Net.ForceOffline);
					UsernameInput.GetComponent<InputField>().text = App.GetWindowsUsername();
					UsernameInput.GetComponent<InputField>().MoveTextEnd(false);
					UsernameInput.GetComponent<InputField>().DeactivateInputField();
					EventSystem.current.SetSelectedGameObject(UsernameInput.GetComponent<InputField>().gameObject, null);
					UsernameInput.GetComponent<InputField>().OnPointerClick(new PointerEventData(EventSystem.current)); 
				} else {
					UsernameInput.GetComponent<InputField>().interactable = true;
					UsernameContainer.SetActive(!_blnIsForgetting);
					PasswordContainer.SetActive(!_blnIsForgetting);
					EmailAddressContainer.SetActive(_blnIsSigningUp || _blnIsForgetting);
					ForgotPasswordContainer.SetActive(!_blnIsForgetting && !_blnIsSigningUp);
					ResetPasswordLabel.SetActive(_blnIsForgetting);
					LoginButton.SetActive(!App.IsWorkingOffline && !Net.ForceOffline);
					SignUpButton.SetActive(App.AllowSignUp || _blnIsForgetting || _blnIsSigningUp);
				}

				bool blnCanOffline = (App != null && App.CanWorkOffline && App.IsWorkingOffline && !_blnIsForgetting && !_blnIsSigningUp);
				#if USES_DATABASEMANAGER
				blnCanOffline = blnCanOffline && Database != null && Database.ClientsCanUse;
				#else
				blnCanOffline = false;
				#endif
				OfflineButton.SetActive(blnCanOffline);

				// ACTIVATE/DEACTIVATE BUTTONS BASED ON ACTIVE LOGIN  (IE, GAME VERSION IS UP TO DATE)
				LoginButton.GetComponent<Button>().interactable						= LoginIsActive && !Net.IsWorkingOffline;
				ForgotPasswordButton.GetComponent<Button>().interactable	= LoginIsActive && !Net.IsWorkingOffline;
				SignUpButton.GetComponent<Button>().interactable					= LoginIsActive && !Net.IsWorkingOffline && (App.AllowSignUp || _blnIsForgetting || _blnIsSigningUp);
				OfflineButton.GetComponent<Button>().interactable					= LoginIsActive;
			}
		}

	#endregion

	#region "BUTTON FUNCTIONS"

		public	void			OnLoginButtonClick()
		{
			if (_blnIsSigningUp)		
			{
				// SUBMIT NEW SIGN UP
				_blnIsSigningUp = false;
				SignUpButton.transform.GetChild(0).GetComponent<Text>().text	= "Sign Up";
				LoginButton.transform.GetChild(0).GetComponent<Text>().text		= "Log In";
				SignUpViaNetwork();
				DisplayPanel();
			} else if (_blnIsForgetting) {		
				// SUBMIT EMAIL ADDRESS FOR FORGOTTEN PASSWORD
				_blnIsForgetting = false;
				SignUpButton.transform.GetChild(0).GetComponent<Text>().text	= "Sign Up";
				LoginButton.transform.GetChild(0).GetComponent<Text>().text		= "Log In";
				ResetPasswordViaNetwork();
				DisplayPanel();
			} else {
				// SEND THE ENCRYPTED USERNAME AND PASSWORD TO THE SERVER
				// SERVER RESPONDS BY DENYING THE LOGIN OR ACCEPTING
				LogInViaNetwork();
			}
		}
		public	void			OnSignUpButtonClick()
		{
			if (_blnIsSigningUp)
			{
				// CANCEL SIGNING UP
				_blnIsSigningUp = false;
				SignUpButton.transform.GetChild(0).GetComponent<Text>().text	= "Sign Up";
				LoginButton.transform.GetChild(0).GetComponent<Text>().text		= "Log In";
				DisplayPanel();
			} else if (_blnIsForgetting) {
				// CANCEL RESETTING PASSWORD
				_blnIsForgetting = false;
				SignUpButton.transform.GetChild(0).GetComponent<Text>().text	= "Sign Up";
				LoginButton.transform.GetChild(0).GetComponent<Text>().text		= "Log In";
				DisplayPanel();
			} else {
				// START TO SIGN UP FOR NEW ACCOUNT
				_blnIsSigningUp = true;
				SignUpButton.transform.GetChild(0).GetComponent<Text>().text	= "Cancel";
				LoginButton.transform.GetChild(0).GetComponent<Text>().text		= "Submit";
				DisplayPanel();
			}
		}
		public	void			OnOfflineButtonClick()
		{
			LogInViaDatabase();
		}
		public	void			OnQuitButtonClick()
		{
			App.QuitApplication();
		}
		public	void			OnForgotButtonClick()
		{
			_blnIsForgetting = true;
			SignUpButton.transform.GetChild(0).GetComponent<Text>().text	= "Cancel";
			LoginButton.transform.GetChild(0).GetComponent<Text>().text		= "Submit";
			DisplayPanel();
		}

	#endregion

}
