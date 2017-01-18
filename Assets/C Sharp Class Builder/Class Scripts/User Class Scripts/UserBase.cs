//		AUTO-GENERATED FILE: UserBase.cs
//		GENERATED ON       : Wed Jan 18 2017 - 12:41:34 PM
//		
//		This is the Base Class file.  It is not intended to be modified.
//		If you need to make changes or additions, please modify the User.cs File.


using UnityEngine;
using UnityEngine.Networking;
using System.Data;

public class UserBase : NetworkBehaviour
{

	#region "PRIVATE VARIABLES"

		// INDEX VARIABLE
		[SerializeField]	protected	int		_intUserID	=	0;
		[SerializeField]	protected	System.DateTime		_dtDateCreated = System.DateTime.Parse("01/01/1900");
		[SerializeField]	protected	System.DateTime		_dtDateUpdated = System.DateTime.Parse("01/01/1900");
		[SerializeField]	protected	bool		_blnIsActive	=	true;
		[SerializeField]	protected	string		_strUsername	=	"";
		[SerializeField]	protected	string		_strPassword	=	"";
		[SerializeField]	protected	string		_strFirstName	=	"";
		[SerializeField]	protected	string		_strLastName	=	"";
		[SerializeField]	protected	string		_strEmailAddress	=	"";
		[SerializeField]	protected	UserTypes		_enUserType = (UserTypes) 1;
		[SerializeField]	protected	int		_intWarnings	=	0;
		[SerializeField]	protected	System.DateTime		_dtBanDate = System.DateTime.Parse("01/01/1900");
		[SerializeField]	protected	int		_intBanDays	=	0;
		[SerializeField]	protected	int		_intConnectionCount	=	0;
		private NetworkConnection		_netConn;

	#endregion

	#region "PRIVATE PROPERTIES"

		protected	ApplicationManager		_app	= null;
		protected	ApplicationManager		App
		{
			get {
				if (_app == null)		_app = ApplicationManager.Instance;
					return _app;
			}
		}
		protected	AppNetworkManager			_net	= null;
		protected	AppNetworkManager			Net
		{
			get {
				if (_net == null)		_net = AppNetworkManager.Instance;
					return _net;
			}
		}
		protected	DatabaseManager				_dbm	= null;
		protected	DatabaseManager				Database
		{
			get {
				if (_dbm == null)		_dbm = DatabaseManager.Instance;
					return _dbm;
			}
		}

	#endregion

	#region "PUBLIC PROPERTIES"

		public	int		NetID
		{
			get { try { return (int) this.netId.Value; } catch { return 0; } }
		}
		public	NetworkConnection	NetConnection
		{
			get { return _netConn; }
			set { _netConn = value; }
		}
		// INDEX VARIABLE
		public	int		ID
		{
			get { return _intUserID; }
		}
		// INDEX VARIABLE
		public	int		UserID
		{
			get { return _intUserID; }
		}
		public	System.DateTime		DateCreated
		{
			get { return _dtDateCreated; }
			set { _dtDateCreated = value; }
		}
		public	System.DateTime		DateUpdated
		{
			get { return _dtDateUpdated; }
			set { _dtDateUpdated = value; }
		}
		public	bool		IsActive
		{
			get { return _blnIsActive; }
			set { _blnIsActive = value; }
		}
		public	string		Username
		{
			get { return _strUsername; }
			set { _strUsername = value; }
		}
		public	string		Password
		{
			get { return _strPassword; }
			set { _strPassword = value; }
		}
		public	string		FirstName
		{
			get { return _strFirstName; }
			set { _strFirstName = value; }
		}
		public	string		LastName
		{
			get { return _strLastName; }
			set { _strLastName = value; }
		}
		public	string		EmailAddress
		{
			get { return _strEmailAddress; }
			set { _strEmailAddress = value; }
		}
		public	UserTypes		UserType
		{
			get { return _enUserType; }
			set { _enUserType = value; }
		}
		public	int		UserTypeInt
		{
			get { return (int) _enUserType; }
			set { _enUserType = (UserTypes) value; }
		}
		// ENUM DECLARATION
		public	enum	UserTypes : int	{ NONE=0, Normal=1, Parole=2, Preferred=3, Enhanced=4, Moderator=5, AsstAdmin=6, Admin=7 };
		public	int		Warnings
		{
			get { return _intWarnings; }
			set { _intWarnings = value; }
		}
		public	System.DateTime		BanDate
		{
			get { return _dtBanDate; }
			set { _dtBanDate = value; }
		}
		public	int		BanDays
		{
			get { return _intBanDays; }
			set { _intBanDays = value; }
		}
		public	int		ConnectionCount
		{
			get { return _intConnectionCount; }
			set { _intConnectionCount = value; }
		}

	#endregion

	#region "PUBLIC SEARCH FUNCTIONS"

		public	static	UserBase		FindByNetID(int intFind)
		{
			if (UserManager.Instance != null)
				return UserManager.Instance.Users.Find(x => x.NetID == intFind);
			else
				return null;
		}
		public	static	UserBase		FindByID(int _intFind)
		{
			if (UserManager.Instance != null)
				return UserManager.Instance.Users.Find(x => x.UserID == _intFind);
			else
				return null;
		}
		public	static	UserBase		FindByUserID(int _intFind)
		{
			if (UserManager.Instance != null)
				return UserManager.Instance.Users.Find(x => x.UserID == _intFind);
			else
				return null;
		}
		public	static	UserBase		FindByUsername(string _strFind)
		{
			if (UserManager.Instance != null)
				return UserManager.Instance.Users.Find(x => x.Username.ToLower() == _strFind.ToLower());
			else
				return null;
		}
		public	static	UserBase		FindByEmailAddress(string _strFind)
		{
			if (UserManager.Instance != null)
				return UserManager.Instance.Users.Find(x => x.EmailAddress.ToLower() == _strFind.ToLower());
			else
				return null;
		}

	#endregion

	#region "PRIVATE FUNCTIONS"


	#endregion

	#region "PUBLIC FUNCTIONS"

		public	void			Clone(UserBase c)
		{
			_intUserID	= c.UserID;
			_dtDateCreated	= c.DateCreated;
			_dtDateUpdated	= c.DateUpdated;
			_blnIsActive	= c.IsActive;
			_strUsername	= c.Username;
			_strPassword	= c.Password;
			_strFirstName	= c.FirstName;
			_strLastName	= c.LastName;
			_strEmailAddress	= c.EmailAddress;
			_enUserType	= c.UserType;
			_intWarnings	= c.Warnings;
			_dtBanDate	= c.BanDate;
			_intBanDays	= c.BanDays;
			_intConnectionCount	= c.ConnectionCount;
		}
		public	void			Reset()
		{
			_intUserID	=	0;
			_dtDateCreated = System.DateTime.Parse("01/01/1900");
			_dtDateUpdated = System.DateTime.Parse("01/01/1900");
			_blnIsActive	=	true;
			_strUsername	=	"";
			_strPassword	=	"";
			_strFirstName	=	"";
			_strLastName	=	"";
			_strEmailAddress	=	"";
			_enUserType = (UserTypes) 1;
			_intWarnings	=	0;
			_dtBanDate = System.DateTime.Parse("01/01/1900");
			_intBanDays	=	0;
			_intConnectionCount	=	0;
		}

	#endregion

	#region "DATABASE FUNCTIONS"

		public	virtual	void		PopulateClass(DataRow dr)
		{
			_intUserID = Util.ConvertToInt(dr["USERID"].ToString());
			_dtDateCreated = Util.ConvertToDate(dr["DATECREATED"].ToString());
			_dtDateUpdated = Util.ConvertToDate(dr["DATEUPDATED"].ToString());
			_blnIsActive = Util.ConvertToBoolean(dr["ISACTIVE"].ToString());
			_strUsername = dr["USERNAME"].ToString();
			_strPassword = dr["PASSWORD"].ToString();
			_strFirstName = dr["FIRSTNAME"].ToString();
			_strLastName = dr["LASTNAME"].ToString();
			_strEmailAddress = dr["EMAILADDRESS"].ToString();
			_enUserType = ((UserTypes) Util.ConvertToInt(dr["USERTYPE"].ToString()));
			_intWarnings = Util.ConvertToInt(dr["WARNINGS"].ToString());
			_dtBanDate = Util.ConvertToDate(dr["BANDATE"].ToString());
			_intBanDays = Util.ConvertToInt(dr["BANDAYS"].ToString());
			_intConnectionCount = Util.ConvertToInt(dr["CONNECTIONCOUNT"].ToString());
		}
		public	virtual		bool	LoadByID(int intID, bool blnActiveOnly = false)
		{
			if (intID < 1)
					return false;

			if (Database.DAL.IsConnected)
			{
				// LOAD FROM THE DATABASE TABLE
				Database.DAL.ClearParams();
				Database.DAL.AddParam("ID",					intID);
				Database.DAL.AddParam("ACTIVEONLY",	blnActiveOnly);
				DataTable dt = Database.DAL.GetSPDataTable("spUserGetByID");
				if (dt != null && dt.Rows.Count > 0)
				{
					PopulateClass(dt.Rows[0]);
					return true;
				}
			}
			return false;
		}
		public	virtual		bool	LoadByUserID(int xFind, bool blnActiveOnly = true)
		{
			if (Database.DAL.IsConnected)
			{
				// LOAD FROM THE DATABASE TABLE
				Database.DAL.ClearParams();
				Database.DAL.AddParam("FIND",				xFind);
				Database.DAL.AddParam("ACTIVEONLY",	blnActiveOnly);
				DataTable dt = Database.DAL.GetSPDataTable("spUserGetByUserID");
				if (dt != null && dt.Rows.Count > 0)
				{
					PopulateClass(dt.Rows[0]);
					return true;
				}
			}
			return false;
		}
		public	virtual		bool	LoadByUsername(string xFind, bool blnActiveOnly = true)
		{
			if (Database.DAL.IsConnected)
			{
				// LOAD FROM THE DATABASE TABLE
				Database.DAL.ClearParams();
				Database.DAL.AddParam("FIND",				xFind);
				Database.DAL.AddParam("ACTIVEONLY",	blnActiveOnly);
				DataTable dt = Database.DAL.GetSPDataTable("spUserGetByUsername");
				if (dt != null && dt.Rows.Count > 0)
				{
					PopulateClass(dt.Rows[0]);
					return true;
				}
			}
			return false;
		}
		public	virtual		bool	LoadByEmailAddress(string xFind, bool blnActiveOnly = true)
		{
			if (Database.DAL.IsConnected)
			{
				// LOAD FROM THE DATABASE TABLE
				Database.DAL.ClearParams();
				Database.DAL.AddParam("FIND",				xFind);
				Database.DAL.AddParam("ACTIVEONLY",	blnActiveOnly);
				DataTable dt = Database.DAL.GetSPDataTable("spUserGetByEmailAddress");
				if (dt != null && dt.Rows.Count > 0)
				{
					PopulateClass(dt.Rows[0]);
					return true;
				}
			}
			return false;
		}
		public	virtual		bool	Save()
		{
			if (Database.DAL.IsConnected)
			{
				// UPDATE TO THE DATABASE TABLE
				_dtDateUpdated = System.DateTime.Now;
				Database.DAL.ClearParams();
				Database.DAL.AddParam("USERID", UserID);
				Database.DAL.AddParam("ISACTIVE", IsActive);
				Database.DAL.AddParam("USERNAME", Username);
				Database.DAL.AddParam("PASSWORD", Password);
				Database.DAL.AddParam("FIRSTNAME", FirstName);
				Database.DAL.AddParam("LASTNAME", LastName);
				Database.DAL.AddParam("EMAILADDRESS", EmailAddress);
				Database.DAL.AddParam("USERTYPE", UserTypeInt);
				Database.DAL.AddParam("WARNINGS", Warnings);
				Database.DAL.AddParam("BANDATE", BanDate);
				Database.DAL.AddParam("BANDAYS", BanDays);
				Database.DAL.AddParam("CONNECTIONCOUNT", ConnectionCount);
				Database.DAL.AddParam("NEWID", DbType.Int32);
				int n = Database.DAL.GetSPInt("spUserUpdate");
				if (_intUserID != n)
					_intUserID = n;
				if (_intUserID > 0)
					return true;
			}
			return false;
		}

	#endregion

	#region "SERIALIZATION FUNCTIONS"

		public	virtual	string					Serialize()
		{
			string strOut = "";
			strOut += UserID.ToString() + "|";
			strOut += DateCreated.ToString("MM/dd/yyyy HH:mm:ss") + "|";
			strOut += DateUpdated.ToString("MM/dd/yyyy HH:mm:ss") + "|";
			strOut += IsActive.ToString() + "|";
			strOut += Username.ToString() + "|";
			strOut += Password.ToString() + "|";
			strOut += FirstName.ToString() + "|";
			strOut += LastName.ToString() + "|";
			strOut += EmailAddress.ToString() + "|";
			strOut += UserTypeInt.ToString() + "|";
			strOut += Warnings.ToString() + "|";
			strOut += BanDate.ToString("MM/dd/yyyy HH:mm:ss") + "|";
			strOut += BanDays.ToString() + "|";
			strOut += ConnectionCount.ToString() + "|";
			strOut = strOut.Substring(0, strOut.Length - 1);
			return strOut;
		}
		public	virtual	void						Deserialize(string strText)
		{
			string[] strSpl = strText.Split('|');
			_intUserID = Util.ConvertToInt(strSpl[0]);
			_dtDateCreated = Util.ConvertToDate(strSpl[1]);
			_dtDateUpdated = Util.ConvertToDate(strSpl[2]);
			_blnIsActive = Util.ConvertToBoolean(strSpl[3]);
			_strUsername = strSpl[4];
			_strPassword = strSpl[5];
			_strFirstName = strSpl[6];
			_strLastName = strSpl[7];
			_strEmailAddress = strSpl[8];
			UserTypeInt = Util.ConvertToInt(strSpl[9]);
			_intWarnings = Util.ConvertToInt(strSpl[10]);
			_dtBanDate = Util.ConvertToDate(strSpl[11]);
			_intBanDays = Util.ConvertToInt(strSpl[12]);
			_intConnectionCount = Util.ConvertToInt(strSpl[13]);
		}

	#endregion

	#region "NETWORK FUNCTIONS"


	#endregion

}

