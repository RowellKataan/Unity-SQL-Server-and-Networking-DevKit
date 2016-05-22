//		AUTO-GENERATED FILE: UserManager.cs
//		GENERATED ON       : Fri May 13 2016 - 12:18:46 PM
//		
//		This is the Class Manager file.  This is the file that you can modify.
//		It will not automatically be changed by the system going forward.


using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Data;

public class UserManager : MonoBehaviour 
{

	#region "PRIVATE VARIABLES"

		private	static	UserManager									_instance = null;

		private bool		_blnInitialized		= false;

		// _blnLoadAllAtStart:  true=Load all Records from Database into Class, false=Start with Empty List, allow developer to add/remove to List.
		[SerializeField]				private bool				_blnLoadAllAtStart	= false;
		[System.NonSerialized]	private	List<User>	_users	= new List<User>();

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

		private	void				Start()
		{
			_users = new List<User>();
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

