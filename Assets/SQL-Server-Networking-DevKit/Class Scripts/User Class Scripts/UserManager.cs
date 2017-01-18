//		AUTO-GENERATED FILE: UserManager.cs
//		GENERATED ON       : Fri May 13 2016 - 12:18:46 PM
//		
//		This is the Class Manager file.  This is the file that you can modify.
//		It will not automatically be changed by the system going forward.


using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Data;

public class UserManager : MonoBehaviour 
{

	#region "PRIVATE VARIABLES"

		private	static	UserManager				_instance = null;

		private bool		_blnInitialized		= false;

		// _blnLoadAllAtStart:  true=Load all Records from Database into Class, false=Start with Empty List, allow developer to add/remove to List.
		[SerializeField]				private bool				_blnLoadAllAtStart	= false;
		[System.NonSerialized]	private	List<User>	_users							= new List<User>();
		[System.NonSerialized]	private	User[]			_guObj;
		[System.NonSerialized]	private int					_intUserCnt					= 0;

	#endregion

	#region "PRIVATE PROPERTIES"

		private	ApplicationManager		_app	= null;
		private	ApplicationManager		App
		{
			get {
				if (_app == null)		_app = ApplicationManager.Instance;
					return _app;
			}
		}
		private	AppNetworkManager			_net	= null;
		private	AppNetworkManager			Net
		{
			get {
				if (_net == null)		_net = AppNetworkManager.Instance;
					return _net;
			}
		}
		private	DatabaseManager				_dbm	= null;
		private	DatabaseManager				Database
		{
			get {
				if (_dbm == null)		_dbm = DatabaseManager.Instance;
					return _dbm;
			}
		}

	#endregion

	#region "PUBLIC EDITOR PROPERTIES"

		[SerializeField]
		public	GameObject			UserInfoPrefab		= null;
		[SerializeField]
		public	GameObject			UserListContainer	= null;
		[SerializeField]
		public	Scrollbar				UserListScrollbar	= null;

	#endregion

	#region "PUBLIC PROPERTIES"

		public	static		UserManager		Instance
		{
			get
			{
				return GetInstance();
			}
		}
		public	static		UserManager		GetInstance()
		{
			if (_instance == null)
					_instance = (UserManager)GameObject.FindObjectOfType(typeof(UserManager));
			return _instance;
		}

		public						List<User>		Users
		{
			get
			{
				if (_users == null)
						_users = new List<User>();
				return _users;
			}
		}

	#endregion

	#region "PRIVATE FUNCTIONS"

		private void				Awake()
		{
			_instance = this;
		}
		private	void				Start()
		{
			_users = new List<User>();
			_intUserCnt	= 0;
			StartCoroutine(DoStart());
		}
		private void				OnEnable()
		{
			if (!_blnInitialized)
				StartCoroutine(DoStart());
		}
		private IEnumerator	DoStart()
		{
			yield return new WaitForSeconds(1.0f);
			if (!_blnLoadAllAtStart)
			{
				_blnInitialized = true;
				yield break;
			}
			Util.Timer	clock = new Util.Timer();
			clock.StartTimer();
			int i = 0;
			while (!Database.IsConnected && clock.GetTime < 10)
			{
				i++;
				yield return null;
			}
			clock.StopTimer();
			if (clock.GetTime > 10)
			{
				// DO NOTHING
			} else if (Database.IsConnectedCheck)
				LoadAll(false);
			clock = null;
			_blnInitialized = true;
		}

	#endregion

	#region "PUBLIC FUNCTIONS"

		public	void						Add(User clsuser)
		{
			if (clsuser.UserID == 0)
				return;
			if (_users.Exists(x => x.UserID == clsuser.UserID))
				_users.Remove(clsuser);
			_users.Add(clsuser);
		}
		public	void						Remove(User clsuser)
		{
			_users.Remove(clsuser);
		}

		public	User			CheckUserLogin(string strUsername, string strPassword, ref string strResult)
		{
			if (!AppNetworkManager.Instance.IsServer)
				return null;

			int intResult = 0;
			DataTable dt	= null;
			 
			if (Database.IsConnected)
			{ 
				Database.DAL.ClearParams();
				Database.DAL.AddParam("USERNAME", strUsername);
				switch (ApplicationManager.Instance.UserLoginType)
				{
					case 1:		// USERNAME/PASSWORD LOGIN
						Database.DAL.AddParam("PASSWORD", strPassword);
						dt = Database.DAL.GetSPDataTable("spGetUserByLogin");
						if (dt != null && dt.Rows.Count > 0)
							intResult = Util.ConvertToInt(dt.Rows[0]["TOTAL"].ToString()) + 1;			// REMOVE THE +1 FOR NON-NWC PROJECTS
						break;
					case 2:		// WINDOWS USERNAME LOGIN
						dt = Database.DAL.GetSPDataTable("spGetUserByLogin");
						if (dt != null && dt.Rows.Count > 0)
							intResult = Util.ConvertToInt(dt.Rows[0]["TOTAL"].ToString()) + 1;
						break;
					default:
						intResult = 0;
						break;
				}
				if (dt != null && dt.Rows.Count > 0)
				{
/*
					if (!Util.ConvertToBoolean(dt.Rows[0]["ISACTIVE"].ToString()))
						intResult = 5;
					if (Util.ConvertToBoolean(dt.Rows[0]["ISDELETED"].ToString()))
						intResult = 6;
					if (Util.ConvertToBoolean(dt.Rows[0]["ISBANNED"].ToString()))
						intResult = 7;
*/
				}
			} else
				intResult = -1;

			User temp = new User();
			switch (intResult)
			{
				default:		// DATABASE IS OFFLINE/INACCESSIBLE
					strResult = "-- Database Unavailable  (" + intResult.ToString() + ")";
					return null;
				case 0:			// INVALID USERNAME/PASSWORD
					switch (ApplicationManager.Instance.UserLoginType)
					{
						case 1:		// USERNAME/PASSWORD -- USERNAME DOES NOT EXIST
							strResult = "-- Invalid Username/Password";
							return null;
						case 2:		// WINDOWS USERNAME  -- USERNAME DOES NOT EXIST
							if (ApplicationManager.Instance.AutoCreateAccount)
							{
								// CREATE THE ACCOUNT IN THE USER TABLE
								temp = new User();
								temp.Username = strUsername;
								if (temp.Save())
								{ 
//								temp.NetworkPlayer = player;
									AddUser(temp);
									strResult = "Load";
									return temp;
								} else {
									strResult = "-- Unable to Create New User Account";
									return null;
								}
							} else {
								strResult = "-- Invalid Username/Password";
								return null;
							}
					}
					break;
				case 1:			// INVALID USERNAME/PASSWORD
					strResult = "-- Invalid Username/Password";
					return null;
				case 2:			// SUCCESS -- LOG THE USER IN
				case 3:
					if (dt != null && dt.Rows.Count > 0)
					{
						temp = new User();
						temp.LoadUserFromDataRow(dt.Rows[0]);
//					temp.NetworkPlayer = player;
						AddUser(temp);
						strResult = "Load";
						return temp;
					} else {
						strResult = "-- Unable to Initialize User Account";
						return null;
					}
				case 5:			// USER ACCOUNT IS INACTIVE
					strResult = "-- User Account is Inactive";
					return null;
				case 6:			// USER ACCOUNT DELETED
					strResult = "-- User Account Deleted";
					return null;
				case 7:			// USER ACCOUNT BANNED
					strResult = "-- User Account Banned";
					return null;
			}
			return null;
		}

		public	void			ResetUsers()
		{
			_users = new List<User>();
		}
		public	void			AddUser(User user)
		{
			if (user == null)
			{
				Debug.LogError("User is NULL");
				return;
			}
			if (user.UserID < 0)
			{
				Debug.LogError("UserID is -1.  Cannot Save Server TempUser.");
				return;
			}

			if (_users == null)
					_users = new List<User>();

			// ADD THE USER INTO THE LIST
			Users.Add(user);

			if (!AppNetworkManager.Instance.IsServer)
				return;

			// CREATE THE USER INFO PANEL FOR THE USER
			if (UserListContainer != null && UserInfoPrefab != null)
			{
				GameObject go = (GameObject) GameObject.Instantiate(UserInfoPrefab);
				go.name = user.Username;
				go.transform.SetParent(UserListContainer.transform);
				go.GetComponent<UserInfoPanel>().User = user;
				float		f		= ((UserListContainer.transform.childCount - 1) * go.GetComponent<RectTransform>().rect.height) + 2;
				Vector2 v2	= new Vector2(0, f * -1);
				go.GetComponent<RectTransform>().localPosition = v2;
				v2		= UserListContainer.GetComponent<RectTransform>().sizeDelta;
				v2.y	= f + (go.GetComponent<RectTransform>().rect.height + 2);
				UserListContainer.GetComponent<RectTransform>().sizeDelta = v2;
				if (UserListScrollbar != null)
					UserListScrollbar.value = 1;
			}
		}
		public	void			RemoveUserByUser(User user)
		{
			// DELETE THE USER INFO PANEL FROM THE LIST
			if (AppNetworkManager.Instance.IsServer && UserListContainer != null)
			{
				float fh		= 0;
				float rh		= 0;
				int		m			= UserListContainer.transform.childCount;

				for (int i = 0; i < m; i++)
				{
					GameObject		go = UserListContainer.transform.GetChild(i).gameObject;
					UserInfoPanel ip = go.GetComponent<UserInfoPanel>();
					RectTransform rt = go.GetComponent<RectTransform>();
					rh = rt.rect.height;
					if (ip.User.NetID == user.NetID)
					{
						DestroyImmediate(go);
						m--;
						i--;
					} else {
						Vector2 v2 = rt.localPosition;
						v2.y = ((fh * rh) + 2) * -1;
						go.GetComponent<RectTransform>().localPosition = v2;
						fh++;
					}
				}

				// RESCALE THE CONTAINER
				RectTransform rx = UserListContainer.GetComponent<RectTransform>();
				Vector2				vx = rx.sizeDelta;
				vx.y = ((UserListContainer.transform.childCount * rh) + 4);
				UserListContainer.GetComponent<RectTransform>().sizeDelta = vx;

				if (UserListScrollbar != null)
					UserListScrollbar.value = 1;
			} else 
				AppNetworkManager.Instance.ServerLog("UserListContainer is NULL. Cannot cull Disconnected User.");

			// REMOVE THE USER FROM THE LIST
			Users.Remove(user);
		}
		public	void			RemoveUserByNetworkConnection(NetworkConnection conn)
		{
			User temp = (User) Users.Find(x => x.NetID == conn.connectionId);
			if (temp != null)
				RemoveUserByUser(temp);
		}

		public	User			FindUserByID(int intID)
		{
			if (Users.Count < 1)
				return	null;
			User temp = null;
			try { temp = Users.Find(x => x.UserID == intID); } catch { return null; }
			if (temp != null)
				return temp;
			return null;
		}
		public	User			FindUserByUsername(string username)
		{
			if (Users.Count < 1)
				return	null;
			User temp = null;
			try { temp = Users.Find(x => x.Username.ToLower() == username.ToLower()); } catch { return null; }
			if (temp != null)
				return temp;
			return null;
		}

	#endregion

	#region "PUBLIC SEARCH FUNCTIONS"

		public	User		FindByID(int _intFind)
		{
			return _users.Find(x => x.UserID == _intFind);
		}
		public	User		FindByUserID(int _intFind)
		{
			return _users.Find(x => x.UserID == _intFind);
		}
		public	User		FindByUsername(string _strFind)
		{
			return _users.Find(x => x.Username.ToLower() == _strFind.ToLower());
		}
		public	User		FindByEmailAddress(string _strFind)
		{
			return _users.Find(x => x.EmailAddress.ToLower() == _strFind.ToLower());
		}
		public	User		FindByUserType(User.UserTypes _enFind)
		{
			return _users.Find(x => x.UserType == _enFind);
		}

	#endregion

	#region "DATABASE FUNCTIONS"

		public	void	LoadAll(bool blnActiveOnly = true)
		{
			_users = new List<User>();

			_blnInitialized = false;
			if (Database.DAL.IsConnected)
			{
				// LOAD FROM THE DATABASE TABLE
				Database.DAL.ClearParams();
				Database.DAL.AddParam("ID",					0);
				Database.DAL.AddParam("ACTIVEONLY",	blnActiveOnly);
				DataTable dt = Database.DAL.GetSPDataTable("spGetUserByID");
				if (dt != null && dt.Rows.Count > 0)
				{
					foreach (DataRow dr in dt.Rows)
					{
						User	temp = new User();
						temp.PopulateClass(dr);
						_users.Add(temp);
					}
					_blnInitialized = true;
				}
			}
		}

	#endregion

	#region "SERIALIZATION FUNCTIONS"


	#endregion

}

