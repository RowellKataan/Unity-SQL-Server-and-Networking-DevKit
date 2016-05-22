// ===========================================================================================================
//
// Class/Library: DatabaseManager (Singleton Class)
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Mar 26, 2016
//	
// VERS 1.0.000 : Mar 26, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

#if UNITY_EDITOR
	#define		IS_DEBUGGING
#else 
	#undef		IS_DEBUGGING
#endif

#define	USES_APPLICATIONMANAGER		// #define = Scene has an ApplicationManager Prefab,	#undef = Scene does not have an ApplicationManager Prefab
#define	USES_STATUSMANAGER				// #define = Scene has a  StatusManager Prefab,				#undef = Scene does not have a  StatusManager Prefab

using UnityEngine;
using System.Data;
using System.Collections;
using System.Collections.Specialized;

[DisallowMultipleComponent]
public class DatabaseManager : MonoBehaviour 
{

	#region "PRIVATE CONSTANTS"

		private float								CONNECTION_TIMEOUT		= 10.00f;

	#endregion

	#region "PUBLIC CONSTANTS"

		public	float								QUERY_TIMEOUT					=  5.00f;

	#endregion

	#region "PRIVATE VARIABLES"

		// DEFINE THE SINGLETONE INSTANCE VARAIABLE
		static	DatabaseManager			_instance								= null;

		#if USES_STATUSMANAGER
		private	StatusManager				_stm										= null;
		#endif
		#if USES_APPLICATIONMANAGER
		private	ApplicationManager	_app										= null;
		#endif

		[SerializeField]
		private bool								_blnClientCanUse				= false;
		[SerializeField]
		private bool								_blnKeepConnectionOpen	= false;

		// DEFINE CLASS STATUS VARIABLES
		private bool								_blnInitialized					= false;
		private bool								_blnProcessing					= false;
		private ClsDAL							_DAL										= null;

		// DEFINE RESPONSE VARIABLES
		private string							_strDBresponse					= "";
		private DataTable						_dtDBresponse;

	#endregion

	#region "PRIVATE PROPERTIES"

		#if USES_STATUSMANAGER
		private StatusManager							Status
		{
			get
			{
				if (_stm == null)
						_stm = StatusManager.Instance;
				return _stm;
			}
		}
		#endif
		#if USES_APPLICATIONMANAGER
		private ApplicationManager				App
		{
			get
			{
				if (_app == null)
						_app = ApplicationManager.Instance;
				return _app;
			}
		}
		#endif

		private	bool											IsServer
		{
			get
			{
				#if USES_APPLICATIONMANAGER
				if (App != null)
					return App.IsServer;
				else
					return true;
				#else
				return true;
				#endif
			}
		}

	#endregion

	#region "PUBLIC PROPERTIES"

		public	static		DatabaseManager	Instance
		{
			get
			{
				return GetInstance();
			}
		}
		public	static		DatabaseManager	GetInstance()
		{
			if (_instance == null)
					_instance = (DatabaseManager)GameObject.FindObjectOfType(typeof(DatabaseManager));
			return _instance;
		}

		public	string										DBserver;
		public	int												DBport;
		public	string										DBdatabase;
		public	string										DBuser;
		public	string										DBpassword;
		public	bool											DBuseWindowsAccount	= false;
		public	TextAsset									DBtextFile	= null;

		public	ClsDAL										DAL
		{
			get
			{
				if (_DAL == null)
				{
					_DAL = new ClsDAL();
					_DAL.KeepConnectionOpen = this.KeepConnectionOpen;
				}
				return _DAL;
			}
		}
		public	bool											KeepConnectionOpen
		{
			get
			{
				return _blnKeepConnectionOpen;
			}
			set
			{
				_blnKeepConnectionOpen = value;
				if (_DAL != null)
						_DAL.KeepConnectionOpen = this.KeepConnectionOpen;
			}
		}
		public	string										DBresponseString
		{
			get
			{
				return _strDBresponse.Trim();
			}
		}
		public	DataTable									DBresponseTable
		{
			get
			{
				return _dtDBresponse;
			}
		}

		public	bool											IsInitialized
		{
			get
			{
				return _blnInitialized;
			}
		}
		public	bool											IsConnected
		{
			get
			{
				try { return DAL != null && DAL.IsConnected; } catch { return false; }
			}
		}
		public	bool											IsConnectedCheck
		{
			get
			{
				try { return DAL != null && DAL.IsConnectedCheck; } catch { return false; }
			}
		}
		public	bool											IsConnecting
		{
			get
			{
				return DAL != null && DAL.IsConnecting;		// false
			}

		}
		public	bool											IsProcessing
		{
			get
			{
				return _blnProcessing;
			}
		}
		public	bool											ClientsCanUse
		{
			get
			{
				#if UNITY_WEBGL
				return false;
				#endif
				return _blnClientCanUse;
			}
			set
			{
				_blnClientCanUse = value;
			}
		}

	#endregion

	#region "PRIVATE FUNCTIONS"

		private	void								Awake()
		{
			GetInstance();
			if ((IsServer || ClientsCanUse) && !_blnInitialized)
				StartCoroutine(InitializeDatabase());
		}
		private void								Start()
		{
		}
		private IEnumerator					InitializeDatabase()
		{	
			// DO NOT RE-INITIALIZE IF ALREADY OPEN
			if (!_blnInitialized)
			{ 
				// CONNECT TO DATABASE.  IF NOT CONNECTED IN TIMEOUT PERIOD, CLOSE APPLICATION
				Util.Timer clock = new Util.Timer();
				OpenDatabase();
				bool blnCont = IsConnected;
				clock.StartTimer();
				while (clock.GetTime < CONNECTION_TIMEOUT && !blnCont)
				{
					blnCont = IsConnected;
					yield return null;
				}
				clock.StopTimer();
				clock = null;

				if (!blnCont)
				{
					#if IS_DEBUGGING
					Debug.LogError("Unable to Connect to Database.");
					#endif
					#if USES_STATUSMANAGER
					Status.Status = "Unable to Connect to Database.";
					#endif
				} else 
					_blnInitialized = true;
			}
			yield return null;
		}

	#endregion

	#region "EVENT FUNCTIONS"

		private void		OnApplicationQuit()
		{
			DisposeDatabase();
		}

	#endregion

	#region "PUBLIC FUNCTIONS"

		public	void										OpenDatabase()
		{
				if (!IsServer && !ClientsCanUse)
					return;

				if (IsInitialized && (IsConnectedCheck || IsConnecting))
					return;

				// CHECK FOR SETTINGS FROM TEXT FILE
				if (DBtextFile != null)
				{
					DBport = 0;
					string[] strLines = DBtextFile.text.Split('\n');
					if (strLines.Length > 0)
					{
						foreach(string st in strLines)
						{
							if (!st.StartsWith("//") && st.Trim() != "" && st.Contains("="))
							{ 
								string[] s = st.Trim().Split('=');
								switch (s[0].Trim().ToLower())
								{
									case "server":
										DBserver = s[1].Trim();
										break;
									case "database":
										DBdatabase = s[1].Trim();
										break;
									case "username":
										DBuser = s[1].Trim();
										break;
									case "password":
										DBpassword = s[1].Trim();
										break;
									case "port":
										try { DBport = int.Parse(s[1].Trim()); } catch { DBport = 0; }
										break;
								}
							}
						}
					}
					DBuseWindowsAccount = (DBuser == "" && DBpassword == "");
				}

				if (DBserver.Trim() != "" && DBdatabase.Trim() != "" && ((DBuser.Trim() != "" && DBpassword.Trim() != "") || DBuseWindowsAccount))
				{
					if (DBuseWindowsAccount)
						DAL.OpenConnection(DBserver, DBdatabase, DBport);
					else
						DAL.OpenConnection(DBserver, DBdatabase, DBuser, DBpassword, DBport);
					_blnInitialized = true;
				} else {
					#if IS_DEBUGGING
					Debug.LogWarning("-- Missing Database Connection Information.");	
					#endif
				}
		}
		public	void										CloseDatabase()
		{
			if (_DAL != null && _DAL.IsConnectedCheck)
					_DAL.CloseConnection();
		}
		public	void										DisposeDatabase()
		{
			if (_DAL != null)
			{
				_DAL.CloseConnection();
				_DAL.Dispose();
				_DAL = null;
			}
		}

	#endregion

	#region "SQL DATABASE IENUMERATOR FUNCTIONS"
	
		public	void							ResetIDB()
		{
			_strDBresponse	= "";
			_dtDBresponse		= null;
			if (DAL != null)
					DAL.ClearParams();
		}

		// STORED PROCEDURES
		public	IEnumerator				GetIDBSPstring(			string strSP, string   strParamList = "")				// PIPE ("|") SEPARATED STRING
		{
			bool blnDone			= false;
			_blnProcessing		= true;
			Util.Timer clock	= new Util.Timer();
			_strDBresponse		= "";

			// SPLIT PARAMETER LIST STRING ON PIPE CHARACTER
			string[] strParams = strParamList.Split('|');

			// OPEN THE CONNECTION TO THE DATABASE
			if (DAL.IsConnected)
			{
				// ADD THE PARAMETERS
				DAL.ClearParams();
				foreach (string st in strParams)
				{
					DAL.AddParam(st.Split('=')[0], st.Split('=')[1]);
				}

				// MAKE THE STORED PROCEDURE CALL
				_strDBresponse = DAL.GetSPString(strSP);
				clock.StartTimer();

				// WAIT FOR THE RESPONSE
				while (!blnDone && clock.GetTime < QUERY_TIMEOUT)
				{
					blnDone = (_strDBresponse != "");
					yield return null;
				}

				// DISPOSE OF TIMEOUT TIMER
				clock.StopTimer();
				clock = null;

				// IF NO RESULTS RETURNED ON TIME, SEND BACK EMPTY
				if (!blnDone)
						_strDBresponse = "";

				// RETURN VALUE
				_blnProcessing = false;
				yield return _strDBresponse;
			} else {
				#if IS_DEBUGGING
				Debug.LogWarning("Database not connected");
				Debug.LogWarning("Queries:\n" + DAL.SQLqueries);
				Debug.LogWarning("Errors:\n" + DAL.Errors);
				#endif
				_strDBresponse = "";
			}
			_blnProcessing = false;
		}
		public	IEnumerator				GetIDBSPstring(			string strSP, string[] strParamList = null)
		{
			bool blnDone			= false;
			_blnProcessing		= true;
			Util.Timer clock	= new Util.Timer();
			_strDBresponse		= "";

			// OPEN THE CONNECTION TO THE DATABASE
			if (DAL.IsConnected)
			{
				// ADD THE PARAMETERS
				DAL.ClearParams();
				foreach(string st in strParamList)
				{
					DAL.AddParam(st.Split('=')[0], st.Split('=')[1]);
				}

				// MAKE THE STORED PROCEDURE CALL
				_strDBresponse = DAL.GetSPString(strSP);
				clock.StartTimer();

				// WAIT FOR THE RESPONSE
				while (!blnDone && clock.GetTime < QUERY_TIMEOUT)
				{
					blnDone = (_strDBresponse != "");
					yield return null;
				}

				// DISPOSE OF TIMEOUT TIMER
				clock.StopTimer();
				clock = null;

				// IF NO RESULTS RETURNED ON TIME, SEND BACK EMPTY
				if (!blnDone)
						_strDBresponse = "";

				// RETURN VALUE
				_blnProcessing = false;
				yield return _strDBresponse;
			} else {
				#if IS_DEBUGGING
				Debug.LogWarning("Database not connected");
				Debug.LogWarning("Queries:\n" + DAL.SQLqueries);
				Debug.LogWarning("Errors:\n" + DAL.Errors);
				#endif
				_strDBresponse = "";
			}
			_blnProcessing = false;
		}
		public	IEnumerator				GetIDBSPdataTable(	string strSP, string[] strParamList = null)
		{
			bool blnDone			= false;
			_blnProcessing		= true;
			Util.Timer clock	= new Util.Timer();
			_strDBresponse		= "";
			_dtDBresponse			= null;

			// OPEN THE CONNECTION TO THE DATABASE
			if (DAL.IsConnected)
			{
				// ADD THE PARAMETERS
				DAL.ClearParams();
				foreach(string st in strParamList)
				{
					DAL.AddParam(st.Split('=')[0], st.Split('=')[1]);
				}

				// MAKE THE STORED PROCEDURE CALL
				_dtDBresponse = DAL.GetSPDataTable(strSP);
				clock.StartTimer();

				// WAIT FOR THE RESPONSE
				while (!blnDone && clock.GetTime < QUERY_TIMEOUT)
				{
					blnDone = (_dtDBresponse != null);
					yield return null;
				}

				// DISPOSE OF TIMEOUT TIMER
				clock.StopTimer();
				clock = null;

				// IF NO RESULTS RETURNED ON TIME, SEND BACK EMPTY
				if (!blnDone)
					_dtDBresponse = null;

				// RETURN VALUE
				_blnProcessing = false;
				yield return _dtDBresponse;
			} else {
				#if IS_DEBUGGING
				Debug.LogWarning("Database not connected");
				Debug.LogWarning("Queries:\n" + DAL.SQLqueries);
				Debug.LogWarning("Errors:\n" + DAL.Errors);
				#endif
				_strDBresponse	= "";
				_dtDBresponse		= null;
			}
			_blnProcessing = false;
		}

		// DIRECT SQL QUERIES
		public	IEnumerator				GetIDBSQLstring(		string strSQL)
		{
			bool blnDone			= false;
			Util.Timer clock	= new Util.Timer();
			_strDBresponse		= "";
			_blnProcessing		= true;

			// OPEN THE CONNECTION TO THE DATABASE
			if (DAL.IsConnected)
			{
				DAL.ClearParams();

				// MAKE THE SQL CALL
				_strDBresponse = DAL.GetSQLSelectString(strSQL);
				clock.StartTimer();

				// WAIT FOR THE RESPONSE
				while (!blnDone && clock.GetTime < QUERY_TIMEOUT)
				{
					blnDone = (_strDBresponse != "");
					yield return null;
				}

				// DISPOSE OF TIMEOUT TIMER
				clock.StopTimer();
				clock = null;

				// IF NO RESULTS RETURNED ON TIME, SEND BACK EMPTY
				if (!blnDone)
						_strDBresponse = "";

				// TURN OFF PROCESSING FLAG
				_blnProcessing	= false;

				// RETURN VALUE
				yield return _strDBresponse;
			} else {
				#if IS_DEBUGGING
				Debug.LogWarning("Database not connected");
				Debug.LogWarning("Queries:\n" + DAL.SQLqueries);
				Debug.LogWarning("Errors:\n" + DAL.Errors);
				#endif
				_strDBresponse = "";
			}
			_blnProcessing = false;
		}
		public	IEnumerator				GetIDBSQLint(				string strSQL)
		{
			_blnProcessing = true;
			Util.CoroutineWithData cd = new Util.CoroutineWithData(this, GetIDBSQLstring(strSQL));
			yield return cd.result.ToString();		// Result is: cd.result
		}
		public	IEnumerator				GetIDBSQLdataTable(	string strSQL)
		{
			bool blnDone			= false;
			Util.Timer clock	= new Util.Timer();
			_strDBresponse		= "";
			_blnProcessing		= true;
			_dtDBresponse			= null;

			// OPEN THE CONNECTION TO THE DATABASE
			if (DAL.IsConnected)
			{
				DAL.ClearParams();

				// MAKE THE SQL CALL
				_dtDBresponse = DAL.GetSQLSelectDataTable(strSQL);
				clock.StartTimer();

				// WAIT FOR THE RESPONSE
				while (!blnDone && clock.GetTime < QUERY_TIMEOUT)
				{
					blnDone = (_dtDBresponse != null);
					yield return null;
				}

				// DISPOSE OF TIMEOUT TIMER
				clock.StopTimer();
				clock = null;

				// IF NO RESULTS RETURNED ON TIME, SEND BACK EMPTY
				if (!blnDone)
					_dtDBresponse = null;

				// TURN OFF PROCESSING FLAG
				_blnProcessing = false;

				// RETURN VALUE
				yield return _dtDBresponse;
			} else {
				#if IS_DEBUGGING
				Debug.LogWarning("Database not connected");
				Debug.LogWarning("Queries:\n" + DAL.SQLqueries);
				Debug.LogWarning("Errors:\n" + DAL.Errors);
				#endif
				_strDBresponse	= "";
				_dtDBresponse		= null;
			}
			_blnProcessing = false;
		}

	#endregion

}


/*
			// GET BY SQL STATEMENT
			Util.Timer clock	= new Util.Timer();
			bool bln = false;
			ResetIDB();
			StartCoroutine(GetIDBSQLstring("SELECT COUNT(*) FROM tblUser"));
			clock.StartTimer();
			while (!bln && clock.GetTime < QUERY_TIMEOUT)
			{
				bln = (_strDBresponse != "");
				yield return null;
			}
			clock.StopTimer();
			clock = null;
			if (!blnDone)
					_strDBresponse = "";
			Debug.Log("My 1 Total Users=" + _strDBresponse);

 
			// STORED PROCEDURE
			Util.Timer clock	= new Util.Timer();
			bln = false;
			ResetIDB();
			StartCoroutine(GetIDBSPdataTable("spGetUserByUserID", "UserID=1"));
			clock.StartTimer();
			while (!bln && clock.GetTime < QUERY_TIMEOUT)
			{
				bln = (_dtDBresponse != null);
				yield return null;			
			}
			clock.StopTimer();
			clock = null;
			if (!blnDone)
					_dtDBresponse = null;
			if (_dtDBresponse != null)
					Debug.Log("My 2 Total Users=" + _dtDBresponse.Rows.Count.ToString());

 
			// DIRECT APPROACH
			ClsDAL DAL = new ClsDAL();
			DAL.OpenConnection();
			if (DAL.IsConnected())
			{
				DAL.ClearParams();
				string st = DAL.GetSQLSelectInt("SELECT COUNT(*) FROM tblUser").ToString();
				Debug.Log("Total Users = " + st);
			} else {
				Debug.Log("Not connected");
				Debug.Log("Queries:\n" + DAL.SQLqueries);
				Debug.Log("Errors:\n" + DAL.Errors);
			}
			DAL.CloseConnection();
*/
