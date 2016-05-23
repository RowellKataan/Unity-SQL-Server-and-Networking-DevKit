// ===========================================================================================================
//
// Class/Library: DAL - Data Access Library Class
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Aug 15, 2007
//	
// VERS 1.0.000 : Aug 15, 2007 : Original File Created.
//      1.1.001 : Mar 26, 2016 : Released for Unity 3D.
//			1.1.002 : Apr 16, 2016 : Resolved bug where "localhost" was being translated to an IPv6 address.
//
// ===========================================================================================================

#if UNITY_EDITOR
	#define		IS_DEBUGGING
#else 
	#undef		IS_DEBUGGING
#endif

// REFERENCE DECLARATIONS
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Security;

public class ClsDAL
{

	#region "PRIVATE CONSTANTS"

		private int			CONNECTION_TIMEOUT		= 8;
		private int			QUERY_TIMEOUT					= 5;

	#endregion

	#region "PRIVATE VARIABLES"

		// SQL CONNECTION VARIABLES
		private SqlConnection						_sqlConn;
		private SqlCommand							_sqlComm;
		private SqlParameterCollection	_SQLSPparams;

		// SQL SERVER CONNECTION STATES
		private bool		_blnIsOnline						= false;
		private bool		_blnIsConnecting				= false;
		private bool		_blnIsProcessing				= false;
		private bool		_blnIsDisposed					= false;

		// SQL SERVER/DATABASE INFORMATION
		private bool		_blnUseWindowsAccount		= false;
		private bool		_blnKeepConnectionOpen	= false;
		private	string	_strDBserver;
		private	string	_strDBdatabase;
		private	string	_strDBuser;
		private	string	_strDBpassword;
		private int			_intDBport;
		private string	_strServerIPaddress			= "";

		// SQL REPORTING
		private string		  _strErrors					= "";
		private int				  _intErrorCount			= 0;
		private string		  _strSQLqueries			= "";
		private Util.Timer	_queryTimer					= null;
		private int					_intQueryCount			= 0;
		private float				_fQueryAverage			= 0;
		private float				_fQueryLast					= 0;

	#endregion

	#region "PUBLIC PROPERTIES"

		public	string	DBserver
		{
			get
			{
				return _strDBserver;
			}
			set
			{
				_strDBserver = value.Trim();
			}
		}
		public	int			DBport
		{
			get
			{
				return _intDBport;
			}
			set
			{
				_intDBport = value;
				if (_intDBport < 0)
						_intDBport = 0;
			}
		}
		public	string	DBdatabase
		{
			get
			{
				return _strDBdatabase;
			}
			set
			{
				_strDBdatabase = value.Trim();
			}
		}
		public	string	DBuser
		{
			get
			{
				return _strDBuser;
			}
			set
			{
				_strDBuser = value.Trim();
			}
		}
		public	string	DBpassword
		{
			get
			{
				return _strDBpassword;
			}
			set
			{
				_strDBpassword = value.Trim();
			}
		}
		public	bool		UseWindowsAccount
		{
			get
			{
				return _blnUseWindowsAccount;
			}
			set
			{
				_blnUseWindowsAccount = value;
			}
		}
		public	bool		KeepConnectionOpen
		{
			get
			{
				return _blnKeepConnectionOpen;
			}
			set
			{
				_blnKeepConnectionOpen = value;
			}
		}

		public	string	DBserverString
		{
			get
			{
				// GET THE IPv4 OF THE LOCALHOST (IF APPLICABLE)
				// THIS CONVERTS THE SERVER NAME INTO AN IP ADDRESS
				// THIS IS DONE TO AVOID THE SERVER NAME BEING CONVERTED TO AN IPv6 ADDRESS
				if (_strDBserver.ToLower().Contains("localhost"))
				{ 
					if (_strServerIPaddress == "")
					{
						string strTempServer	= "";
						string strTempDB			= "";
						if (_strDBserver.Contains("\\"))
						{
							strTempServer = _strDBserver.Split('\\')[0];
							strTempDB			= _strDBserver.Split('\\')[1];
						} else
							strTempServer = _strDBserver;

						System.Net.NetworkInformation.NetworkInterface[] networkInterfaces = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces();
						foreach (System.Net.NetworkInformation.NetworkInterface network in networkInterfaces)
						{
							// Read the IP configuration for each network 
							System.Net.NetworkInformation.IPInterfaceProperties properties = network.GetIPProperties();

							// Each network interface may have multiple IP addresses 
							foreach (System.Net.NetworkInformation.IPAddressInformation address in properties.UnicastAddresses)
							{
								// We're only interested in IPv4 addresses for now 
								if (address.Address.AddressFamily != System.Net.Sockets.AddressFamily.InterNetwork)
									continue;

								// Ignore loopback addresses (e.g., 127.0.0.1) 
								if (System.Net.IPAddress.IsLoopback(address.Address))
								{
									_strServerIPaddress = address.Address.ToString();
									if (strTempDB != "")
										_strServerIPaddress += "\\" + strTempDB;
									break;
								}
							}
						} 

						if (_strServerIPaddress == "")
						{ 
							System.Net.IPAddress[] ips = System.Net.Dns.GetHostAddresses(strTempServer);
							foreach (System.Net.IPAddress a in ips)
							{
								if (a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
								{
									_strServerIPaddress = a.ToString();
									if (strTempDB != "")
										_strServerIPaddress += "\\" + strTempDB;
									break;
								}
							}
						}
					}
				} else 
					_strServerIPaddress = _strDBserver;

				// ADD ON THE PORT (IF APPLICABLE)
				if (_intDBport == 0 || _intDBport == 1433)
					return _strServerIPaddress;
				else
					return _strServerIPaddress + "," + _intDBport.ToString();
			}
		}

	#endregion

	#region "SQL CODE"

		#region "CONNECTION FUNCTIONS"
					
					private string				SQLConnectionString
					{
						get
						{
							if (UseWindowsAccount)
								return "Server=" + DBserverString + ";Database=" + DBdatabase + ";Integrated Security=True;Trusted_Connection=True;";
							else
								return "Server=" + DBserverString + ";Database=" + DBdatabase + ";User ID=" + DBuser + ";Password=" + DBpassword + ";";
						}
					}

					private void					SQLOpenConnection()
					{
						try
						{
							StartQueryTimer();
							Job.make(SQLOpenConnectionEnum(), true);
						} catch {
							StopQueryTimer();
							_blnIsConnecting	= false;
							_blnIsOnline			= false;
						}
					}
					private IEnumerator		SQLOpenConnectionEnum()
					{
						_blnIsOnline			= false;
						_blnIsConnecting	= true;

						if (_sqlConn == null) 
						{
							_sqlConn = new SqlConnection(SQLConnectionString);
							_sqlConn.Open();
						} else {
							switch (_sqlConn.State) 
							{
								case ConnectionState.Open:
									_blnIsOnline = true;
									break;
								case ConnectionState.Closed:
									_sqlConn.Dispose();
									_sqlConn = new SqlConnection(SQLConnectionString);
									try {  _sqlConn.Open(); } catch (System.Exception ex) { 
										#if IS_DEBUGGING
										UnityEngine.Debug.LogError("The Database does not Exist. (" + SQLConnectionString + ")\n" + _strErrors);
										#endif
										ReportError("ClsDAL", "SQLOpenConnectionEnum", "The Database does not Exist", ex);
									}
									break;
								case ConnectionState.Broken:
									_sqlConn.Close();
									_sqlConn.Open();
									break;
							}
						}
						Util.Timer clock = new Util.Timer();
						clock.StartTimer();
						while (!_blnIsOnline && clock.GetTime < CONNECTION_TIMEOUT)
						{
							yield return null;
							_blnIsOnline = (_sqlConn.State != ConnectionState.Broken && _sqlConn.State != ConnectionState.Closed);
						}
						clock.StopTimer();
						_sqlConn.StateChange += OnSQLstateChanged;
						_blnIsConnecting	= false;
					}
					private void					SQLOpenConnection(string strServer, string strDB, int intPort = 0)
					{
						DBserver		= strServer;
						DBdatabase	= strDB;
						DBuser			= "";
						DBpassword	= "";
						DBport			= intPort;
						UseWindowsAccount = true;
						SQLOpenConnection();
					}
					private void					SQLOpenConnection(string strServer, string strDB, string strUser, string strPwd, int intPort = 0)
					{
						DBserver		= strServer;
						DBdatabase	= strDB;
						DBuser			= strUser;
						DBpassword	= strPwd;
						DBport			= intPort;
						UseWindowsAccount = false;
						SQLOpenConnection();
					}

					private void					SQLCloseConnection()
					{
						StopQueryTimer();
						_blnIsConnecting	= false;
						_blnIsOnline			= false;
						_blnIsProcessing	= false;

						if (_sqlConn == null || !_blnIsOnline)
							return;
						else
						{
							if (_sqlComm != null)
									_sqlComm.Dispose();
							_sqlConn.Close();
							_sqlConn.Dispose();
						}
					}
					private bool					SQLisConnected
					{
						get
						{
							if (!_blnIsConnecting)
							{ 
								if (_sqlConn == null)
								{
									_sqlConn = new SqlConnection(SQLConnectionString);
									_sqlConn.Open();
								} else if (_sqlConn.State == ConnectionState.Broken || _sqlConn.State == ConnectionState.Closed) {
									_sqlConn.Close();
									_sqlConn.Dispose();
									_sqlConn = new SqlConnection(SQLConnectionString);
									_sqlConn.Open();
								}
							}
							_blnIsOnline = (_sqlConn != null && _sqlConn.State != ConnectionState.Broken && _sqlConn.State != ConnectionState.Closed && !_blnIsConnecting);
							if (_blnIsOnline)
								StartQueryTimer();
							return _blnIsOnline;
						}
					}
					private bool					SQLisConnecting
					{
						get
						{
							return  _blnIsConnecting;
						}
					}

					private void					OnSQLstateChanged(object conn, System.Data.StateChangeEventArgs e)
					{
						_blnIsOnline = (_sqlConn != null && _sqlConn.State != ConnectionState.Broken && _sqlConn.State != ConnectionState.Closed && !_blnIsConnecting);
						if (StatusManager.Instance != null)
								StatusManager.Instance.UpdateStatus();
					}

		#endregion

		#region "PARAMETER DEFINITION FUNCTIONS"

					private void SQLAddParam(string strParamName, string    strParamValue)
					{
						if (!SQLisConnected)
							return;
							 
						try
						{
							SqlParameter sParam		= new SqlParameter();
							sParam.ParameterName	= strParamName;
							sParam.SqlDbType			= SqlDbType.VarChar;
							sParam.SqlValue				= strParamValue;
							sParam.Direction			= ParameterDirection.Input;
							_SQLSPparams.Add(sParam);
						} catch (Exception ex) { ReportError("ClsDAL.cs", "SQLAddParam(STRING)", strParamValue, ex); }
					}
					private void SQLAddParam(string strParamName, int       intParamValue)
					{
						if (!SQLisConnected)
							return;

						try
						{
							SqlParameter sParam		= new SqlParameter();
							sParam.ParameterName	= strParamName;
							sParam.SqlDbType			= SqlDbType.Int;
							sParam.SqlValue				= intParamValue;
							sParam.Direction			= ParameterDirection.Input;
							_SQLSPparams.Add(sParam);
						} catch (Exception ex) { ReportError("ClsDAL.cs", "SQLAddParam(INT)", intParamValue.ToString(), ex); }
					}
					private void SQLAddParam(string strParamName, decimal   decParamValue)
					{
						if (!SQLisConnected)
							return;

						try 
						{
							SqlParameter sParam  = new SqlParameter();
							sParam.ParameterName = strParamName;
							sParam.SqlDbType     = SqlDbType.Decimal;
							sParam.SqlValue      = decParamValue;
							sParam.Direction     = ParameterDirection.Input;
							_SQLSPparams.Add(sParam);
						} catch (Exception ex) { ReportError("ClsDAL.cs", "SQLAddParam(DECIMAL)", decParamValue.ToString(), ex); }
					}
					private void SQLAddParam(string strParamName, float     fParamValue)
					{
						if (!SQLisConnected)
							return;

						try 
						{
							SqlParameter sParam  = new SqlParameter();
							sParam.ParameterName = strParamName;
							sParam.SqlDbType     = SqlDbType.Float;
							sParam.SqlValue      = fParamValue;
							sParam.Direction     = ParameterDirection.Input;
							_SQLSPparams.Add(sParam);
						} catch (Exception ex) { ReportError("ClsDAL.cs", "SQLAddParam(FLOAT)", fParamValue.ToString(), ex); }
					}
					private void SQLAddParam(string strParamName, double    decParamValue)
					{
						if (!SQLisConnected)
							return;

						try
						{
							SqlParameter  sParam = new SqlParameter();
							sParam.ParameterName = strParamName;
							sParam.SqlDbType     = SqlDbType.Decimal;
							sParam.SqlValue      = decParamValue;
							sParam.Direction     = ParameterDirection.Input;
							_SQLSPparams.Add(sParam);
						} catch (Exception ex) { ReportError("ClsDAL.cs", "SQLAddParam(DOUBLE)", decParamValue.ToString(), ex); }
					}
					private void SQLAddParam(string strParamName, long      lngParamValue)
					{
						if (!SQLisConnected)
							return;

						try
						{
							SqlParameter sParam  = new SqlParameter();
							sParam.ParameterName = strParamName;
							sParam.SqlDbType     = SqlDbType.Float;
							sParam.SqlValue      = lngParamValue;
							sParam.Direction     = ParameterDirection.Input;
							_SQLSPparams.Add(sParam);
						} catch (Exception ex) { ReportError("ClsDAL.cs", "SQLAddParam(LONG)", lngParamValue.ToString(), ex); }
					}
					private void SQLAddParam(string strParamName, DateTime  dateParamValue)
					{
						if (!SQLisConnected)
							return;

						try
						{
							SqlParameter sParam = new SqlParameter();
							sParam.ParameterName = strParamName;
							sParam.SqlDbType = SqlDbType.DateTime;
							sParam.SqlValue = dateParamValue;
							sParam.Direction = ParameterDirection.Input;
							_SQLSPparams.Add(sParam);
						} catch (Exception ex) { ReportError("ClsDAL.cs", "SQLAddParam(DATE)", dateParamValue.ToString(), ex); }
					}
					private void SQLAddParam(string strParamName, bool      blnParamValue)
					{
						if (!SQLisConnected)
							return;

						try
						{
							SqlParameter sParam  = new SqlParameter();
							sParam.ParameterName = strParamName;
							sParam.SqlDbType     = SqlDbType.Bit;
							sParam.SqlValue      = blnParamValue;
							sParam.Direction     = ParameterDirection.Input;
							_SQLSPparams.Add(sParam);
						} catch (Exception ex) { ReportError("ClsDAL.cs", "SQLAddParam(BOOLEAN)", blnParamValue.ToString(), ex); }
					}
					private void SQLAddParam(string strParamName, byte[]		buffer)
					{
						if (!SQLisConnected)
							return;

						try
						{
							SqlParameter sParam		= new SqlParameter();
							sParam.ParameterName	= strParamName;
							sParam.SqlDbType			= SqlDbType.VarBinary;
							sParam.SqlValue				= buffer;
							sParam.Direction			= ParameterDirection.Input;
							_SQLSPparams.Add(sParam);
						} catch (Exception ex) { ReportError("ClsDAL.cs", "SQLAddParam(BYTE[])", "...", ex); }
					}
					private void SQLAddParam(string strParamName, SqlDbType sType)
					{
						if (!SQLisConnected)
							return;

						try
						{
							SqlParameter sParam		= new SqlParameter();
							sParam.ParameterName	= strParamName;
							sParam.SqlDbType			= sType;
							sParam.Value					= null;
							sParam.Direction			= ParameterDirection.Output;
							_SQLSPparams.Add(sParam);
						} catch (Exception ex) { ReportError("ClsDAL.cs", "SQLAddParam(OUTPUT)", "", ex); }
					}
					private void SQLAddParam(string strParamName, SqlDbType sType, int varSize)
					{
						if (!SQLisConnected)
							return;

						try
						{
							SqlParameter sParam		= new SqlParameter();
							sParam.ParameterName	= strParamName;
							sParam.SqlDbType			= sType;
							sParam.Size						= varSize;
							sParam.Value					= null;
							sParam.Direction			= ParameterDirection.Output;
							_SQLSPparams.Add(sParam);
						} catch (Exception ex) { ReportError("ClsDAL.cs", "SQLAddParam(OUTPUT,SIZE)", "", ex); }
					}
					private void SQLClearParams()
					{
						if (!SQLisConnected && !_blnIsConnecting)
						{
							SQLOpenConnection();
							if (!SQLisConnected)
							{
								_strErrors += "Connection to Database Lost.\n"; 
								return;
							}
						}

						try
						{
							if (_SQLSPparams != null)
								_SQLSPparams.Clear();
						} catch (Exception ex) { ReportError("ClsDAL.cs", "SQLClearParams", "", ex); }
						_sqlComm = new SqlCommand();
						_SQLSPparams = _sqlComm.Parameters;
						try
						{
							if (_SQLSPparams != null)
								_SQLSPparams.Clear();
						} catch (Exception ex) { ReportError("ClsDAL.cs", "SQLClearParams", "", ex); }
					}
					private SqlParameter SQLCopyParameter(SqlParameter aPa)
					{
						SqlParameter	olt = new SqlParameter();
						olt.DbType				= aPa.DbType;
						olt.Direction			= aPa.Direction;
						olt.DbType				= aPa.DbType;
						olt.ParameterName = aPa.ParameterName;
						olt.Size					= aPa.Size;
						olt.Value					= aPa.Value;
						return olt;
					}

		#endregion

		#region "DIRECT SQL SELECT FUNCTIONS"

					private DataTable SQLGetSQLSelectDataTable(	string strSQL)
					{
						DataTable dtRet = new DataTable();

						if (!SQLisConnected)
							return null;
						
						try
						{
							SqlDataAdapter dtaDataAdapter = new SqlDataAdapter(strSQL, _sqlConn);
							dtaDataAdapter.Fill(dtRet);
							dtaDataAdapter.Dispose();
						} catch (Exception ex) { ReportError("ClsDAL.cs", "SQLGetSQLSelectDataTable", strSQL, ex); dtRet = null; }

						if (KeepConnectionOpen)
							StopQueryTimer();
						else
							SQLCloseConnection();

						return dtRet;
					}
					private string    SQLGetSQLSelectString(		string strSQL)
					{
						string strRet = "";
						_sqlComm = null;

						if (!SQLisConnected)
							return strRet;

						try
						{
							while (_sqlComm == null)
							{
								_sqlComm = new SqlCommand(strSQL, _sqlConn);
							}
							_sqlComm.CommandTimeout = 10;
							try 
							{
								strRet = _sqlComm.ExecuteScalar().ToString(); 
							} catch (Exception ex) { ReportError("ClsDAL.cs", "SQLGetSQLSelectString", "Unable to Extract Scalar", ex); strRet = ""; }
						} catch (Exception ex) {
							ReportError("ClsDAL.cs", "SQLGetSQLSelectString", strSQL, ex);
							strRet = "";
						}

						if (KeepConnectionOpen)
							StopQueryTimer();
						else
							SQLCloseConnection();

						return strRet;
					}
					private int       SQLGetSQLSelectInt(				string strSQL)
					{
						int intRet = 0;

						if (!SQLisConnected)
							return intRet;

						string strTemp = SQLGetSQLSelectString(strSQL);
						try
						{
							intRet = int.Parse(strTemp);
						} catch {
							intRet = 0;
						}

						return intRet;
					}
					private decimal   SQLGetSQLSelectDecimal(		string strSQL)
					{
						decimal decRet = 0;

						if (!SQLisConnected)
							return decRet;

						string strTemp = SQLGetSQLSelectString(strSQL);
						try
						{
							decRet = decimal.Parse(strTemp);
						} catch {
							decRet = 0;
						}

						return decRet;
					}
					private float			SQLGetSQLSelectFloat(			string strSQL)
					{
						float fRet = 0;

						if (!SQLisConnected)
							return fRet;

						string strTemp = SQLGetSQLSelectString(strSQL);
						try
						{
							fRet = float.Parse(strTemp);
						} catch {
							fRet = 0;
						}

						return fRet;
					}
					private double    SQLGetSQLSelectDouble(		string strSQL)
					{
						double dblRet = 0;

						if (!SQLisConnected)
							return dblRet;

						string strTemp = SQLGetSQLSelectString(strSQL);
						try
						{
							dblRet = double.Parse(strTemp);
						} catch {
							dblRet = 0;
						}

						return dblRet;
					}
					private bool      SQLDoSQLUpdateDelete(			string strSQL)
					{
						bool blnRet = false;

						if (!SQLisConnected)
							return blnRet;

						_sqlComm = _sqlConn.CreateCommand();
						_sqlComm.CommandText = strSQL;
						try
						{
							blnRet = (_sqlComm.ExecuteNonQuery() > 0);
						} catch (Exception ex) {
							ReportError("ClsDAL.cs", "SQLDoSQLUpdateDelete", strSQL, ex);
							blnRet = false; 
						}

						if (KeepConnectionOpen)
							StopQueryTimer();
						else
							SQLCloseConnection();

						return blnRet;
					}

		#endregion

		#region "STORED PROCEDURE SELECT FUNCTIONS"

					private DataTable SQLGetSPDataTable(string strSP)
					{
						DataTable dtRet = new DataTable();

						if (!SQLisConnected)
							return null;

						try
						{
							SqlCommand command = new SqlCommand(strSP, _sqlConn);
							using (_sqlConn)
							{
								command.CommandType = CommandType.StoredProcedure;
								command.CommandText = strSP;

								foreach (SqlParameter aPa in _SQLSPparams)
								{
									command.Parameters.Add(SQLCopyParameter(aPa));
								}
								SqlDataAdapter dtaDataAdapter = new SqlDataAdapter(command);
								dtaDataAdapter.Fill(dtRet);
								dtaDataAdapter.Dispose();
							}
						} catch (Exception ex) { 
							ReportError("ClsDAL.cs", "SQLGetSPDataTable", strSP, ex); dtRet = null; 
						}

						if (KeepConnectionOpen)
							StopQueryTimer();
						else
							SQLCloseConnection();

						return dtRet;
					}
					private string    SQLGetSPString(string strSP)
					{
						string strRet = "";

						if (!SQLisConnected)
							return strRet;

						try
						{
							_sqlComm = new SqlCommand(strSP, _sqlConn);
							using (_sqlConn)
							{
								string strParamName = "";
								_sqlComm.CommandType = CommandType.StoredProcedure;
								_sqlComm.CommandText = strSP;

								foreach (SqlParameter sqlpar in _SQLSPparams)
								{
									_sqlComm.Parameters.Add(SQLCopyParameter(sqlpar));
									if (sqlpar.Direction == ParameterDirection.Output)
										strParamName = sqlpar.ParameterName.ToString();
								}
								if (strParamName != "")
								{
									_sqlComm.ExecuteNonQuery();
									strRet = _sqlComm.Parameters[strParamName].Value.ToString();
								} else {
									DataTable dtRet = new DataTable();
									SqlDataAdapter dtaDataAdapter = new SqlDataAdapter(_sqlComm);
									dtaDataAdapter.Fill(dtRet);
									dtaDataAdapter.Dispose();
									try { strRet = dtRet.Rows[0][0].ToString(); } catch { strRet = ""; }
								}
							}
						} catch (Exception ex) { ReportError("ClsDAL.cs", "SQLGetSPString", strSP, ex); }

						if (KeepConnectionOpen)
							StopQueryTimer();
						else
							SQLCloseConnection();

						return strRet;
					}
					private int       SQLGetSPInt(string strSP)
					{
						int intRet = 0;

						if (!SQLisConnected)
							return intRet;

						string strTemp = SQLGetSPString(strSP);
						try
						{
							intRet = int.Parse(strTemp);
						} catch {
							intRet = 0;
						}

						return intRet;
					}
					private long      SQLGetSPLong(string strSP)
					{
						long lngRet = 0;

						if (!SQLisConnected)
							return lngRet;

						string strTemp = SQLGetSPString(strSP);
						try
						{
							lngRet = long.Parse(strTemp);
						} catch {
							lngRet = 0;
						}

						return lngRet;
					}
					private decimal   SQLGetSPDecimal(string strSP)
					{
						decimal decRet = 0;

						if (!SQLisConnected)
							return decRet;

						string strTemp = SQLGetSPString(strSP);
						try
						{
							decRet = decimal.Parse(strTemp);
						} catch {
							decRet = 0;
						}

						return decRet;
					}
					private float		  SQLGetSPFloat(string strSP)
					{
						float		fRet = 0;

						if (!SQLisConnected)
							return fRet;

						string strTemp = SQLGetSPString(strSP);
						try
						{
							fRet = float.Parse(strTemp);
						} catch {
							fRet = 0;
						}

						return fRet;
					}
					private byte[]    SQLGetSPBinary(string strSP)
					{
						byte[]    binRet       = null;
						DataTable dtRet        = new DataTable();

						if (!SQLisConnected)
							return binRet;

						try
						{
							_sqlComm = new SqlCommand(strSP, _sqlConn);
							using (_sqlConn)
							{
								_sqlComm.CommandType = CommandType.StoredProcedure;
								_sqlComm.CommandText = strSP;

								foreach (SqlParameter aPa in _SQLSPparams)
								{
									_sqlComm.Parameters.Add(SQLCopyParameter(aPa));
								}
								SqlDataAdapter dtaDataAdapter = new SqlDataAdapter(_sqlComm);
								dtaDataAdapter.Fill(dtRet);
								dtaDataAdapter.Dispose();

								binRet = dtRet.Rows[0][0] as byte[];
							}
						} catch (Exception ex) { 
							ReportError("ClsDAL.cs", "SQLGetSPDataTable", strSP, ex); dtRet = null; 
						}

						if (KeepConnectionOpen)
							StopQueryTimer();
						else
							SQLCloseConnection();

						return binRet;
					}
					private void      SQLExecuteSP(string strSP)
					{
						if (!SQLisConnected)
							return;

						try
						{
							_sqlComm = new SqlCommand(strSP, _sqlConn);
							using (_sqlConn)
							{
								_sqlComm.CommandType = CommandType.StoredProcedure;
								_sqlComm.CommandText = strSP;

								foreach (SqlParameter aPa in _SQLSPparams)
								{
									_sqlComm.Parameters.Add(SQLCopyParameter(aPa));
								}
								_sqlComm.ExecuteNonQuery();
							}
						} catch (Exception ex) { 
							ReportError("ClsDAL.cs", "SQLExecuteSP", strSP, ex); 
						}

						if (KeepConnectionOpen)
							StopQueryTimer();
						else
							SQLCloseConnection();
					}

		#endregion

		#region "UPDATE STORED PROCEDURE"

					private string  SQLUpdateSPDataTable(string strSP, string  strPass)
					{
						string strRet = "";
						string strParamName = "";

						if (!SQLisConnected)
							return strRet;

						try
						{
							_sqlComm = new SqlCommand(strSP, _sqlConn);
							using (_sqlConn)
							{
								_sqlComm.CommandType = CommandType.StoredProcedure;
								_sqlComm.CommandText = strSP;

								foreach (SqlParameter aPa in _SQLSPparams)
								{
									_sqlComm.Parameters.Add(SQLCopyParameter(aPa));
									if (aPa.Direction == ParameterDirection.Output)
										strParamName = aPa.ParameterName.ToString();
								}
								_sqlComm.ExecuteNonQuery();
								if (strParamName != "")
									strRet = _sqlComm.Parameters[strParamName].Value.ToString();
								else
									strRet = "";
							}
						} catch (Exception ex) { ReportError("ClsDAL.cs", "SQLUpdateSPDataTable(STRING)", strSP, ex); strRet = ""; }

						if (KeepConnectionOpen)
							StopQueryTimer();
						else
							SQLCloseConnection();

						return strRet;
					}
					private int     SQLUpdateSPDataTable(string strSP, int     intPass)
					{
						string strParamName = "";
						string strRet = "";
						int intRet = 0;

						if (!SQLisConnected)
							return intRet;

						try
						{
							_sqlComm = new SqlCommand(strSP, _sqlConn);
							using (_sqlConn)
							{
								_sqlComm.CommandType = CommandType.StoredProcedure;
								_sqlComm.CommandText = strSP;

								foreach (SqlParameter aPa in _SQLSPparams)
								{
									_sqlComm.Parameters.Add(SQLCopyParameter(aPa));
									if (aPa.Direction == ParameterDirection.Output)
										strParamName = aPa.ParameterName.ToString();
								}
								_sqlComm.ExecuteNonQuery();
								if (strParamName != "")
									strRet = _sqlComm.Parameters[strParamName].Value.ToString();
								else
									strRet = "0";
								intRet = int.Parse(strRet);
							}
						} catch (Exception ex) { ReportError("ClsDAL.cs", "SQLUpdateSPDataTable(INT)", strSP, ex); intRet = 0; }

						if (KeepConnectionOpen)
							StopQueryTimer();
						else
							SQLCloseConnection();

						return intRet;
					}
					private decimal SQLUpdateSPDataTable(string strSP, decimal decPass)
					{
						string strParamName = "";
						string strRet = "";
						decimal decRet = 0;

						if (!SQLisConnected)
							return decRet;

						try
						{
							_sqlComm = new SqlCommand(strSP, _sqlConn);
							using (_sqlConn)
							{
								_sqlComm.CommandType = CommandType.StoredProcedure;
								_sqlComm.CommandText = strSP;

								foreach (SqlParameter aPa in _SQLSPparams)
								{
									_sqlComm.Parameters.Add(SQLCopyParameter(aPa));
									if (aPa.Direction == ParameterDirection.Output)
										strParamName = aPa.ParameterName.ToString();
								}
								_sqlComm.ExecuteNonQuery();
								if (strParamName != "")
									strRet = _sqlComm.Parameters[strParamName].Value.ToString();
								else
									strRet = "0";
								decRet = decimal.Parse(strRet);
							}
						} catch (Exception ex) { ReportError("ClsDAL.cs", "SQLUpdateSPDataTable(DECIMAL)", strSP, ex); decRet = 0; }

						if (KeepConnectionOpen)
							StopQueryTimer();
						else
							SQLCloseConnection();

						return decRet;
					}

		#endregion

		#region "BULKCOPY / INSERT"

					private bool		SQLBulkCopier(string strTableName, DataTable dtTable)
					{
						if (!SQLisConnected)
							return false;

						try
						{ 
							using (var tran = _sqlConn.BeginTransaction(IsolationLevel.ReadCommitted))
							{
								using (var bulkCopy = new SqlBulkCopy(_sqlConn, SqlBulkCopyOptions.Default, tran))
								{
									bulkCopy.DestinationTableName = strTableName;
									bulkCopy.BulkCopyTimeout = 1200; // in seconds
//								bulkCopy.ColumnMappings.Add("...", "...");
									bulkCopy.WriteToServer(dtTable);
									tran.Commit();
									return true;

								}
							}

						} catch (Exception ex) {
							_strErrors = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
						}

/*
						SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConn);
//												(
//												sqlConn,
//												SqlBulkCopyOptions.TableLock |
//												SqlBulkCopyOptions.FireTriggers |
//												SqlBulkCopyOptions.UseInternalTransaction,
//												null
//												);
						bulkCopy.BulkCopyTimeout = 1200; // in seconds
						bulkCopy.DestinationTableName = "dbo." + strTableName;
						try
						{ 
							bulkCopy.WriteToServer(dtTable);
							return true;
						} catch (Exception ex) {
							strErrors = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
							return false;
						}
*/

						if (KeepConnectionOpen)
							StopQueryTimer();
						else
							SQLCloseConnection();

						return false;
					}
					private bool		SQLInsertFromDataTable(string strSP, string strDataType, DataTable dtTable)
					{
						if (!SQLisConnected)
							return false;

						try
						{
							_sqlComm = new SqlCommand(strSP, _sqlConn);
							using (_sqlConn)
							{
								_sqlComm.CommandType = CommandType.StoredProcedure;
								_sqlComm.CommandText = strSP;

								SqlParameter parameter = new SqlParameter();
								parameter.ParameterName = "@" + strDataType;
//							parameter.SqlDbType = System.Data.SqlDbType.Structured;
								parameter.Value = dtTable;
//							parameter.TypeName = strDataType;
								_sqlComm.Parameters.Add(parameter);

								_sqlComm.ExecuteNonQuery();
								return true;
							}
						} catch (Exception ex) {
							ReportError("ClsDAL.cs", "SQLInsertFromDataTable", strSP, ex);
							_strErrors = ex.Message + "\n" + ex.InnerException + "\n" + ex.StackTrace;
						}

						if (KeepConnectionOpen)
							StopQueryTimer();
						else
							SQLCloseConnection();

						return false;
					}

		#endregion

		#region "DATABASE BACKUP FUNCTIONS"

			private bool    SQLBackupDatabase(string strDatabase, string strBackupPath)
			{
				if (!SQLisConnected)
					return false;

				// ABORT IF THERE IS NO DATABASE FOR THIS WEB SITE
				if (strDatabase == "" || strBackupPath == "") 
					return false;

				try
				{
					ClearParams();
					AddParam("DBNAME",  strDatabase);
					AddParam("DIRNAME", strBackupPath);
					ExecuteSP("spCmdBackupDatabase");

					/*
					// BUILD THE BACKUP COMMAND MANUALLY, NOT THROUGH THE ABOVE STORED PROCEDURE
					SqlCommand   sCom  = new SqlCommand();
					string strCommand  = "BACKUP DATABASE ";
								 strCommand += strDatabase + " TO DISK = '" + strBackupPath + "' WITH NOFORMAT, NOINIT, NAME = 'Full " + strDatabase + " Database Backup', SKIP, NOREWIND, NOUNLOAD,STATS = 10";

					// PERFORM THE SQL COMMAND TO BACKUP THE DATABASE TO THE FILE
					SQLOpenConnection();
					sCom  = new SqlCommand(strCommand, sqlConn);
					sCom.CommandType = CommandType.Text;

					sCom.ExecuteNonQuery();
					sCom.Dispose();
					*/

					return true;
				} catch (System.Exception ex) {
					ReportError("ClsDAL.cs", "SQLBackupDatabase", "DB: " + strDatabase + ", PATH: " + strBackupPath, ex);
					return false;
				}
			} 
		
		#endregion
				
	#endregion


	#region "MAIN CODE"

		#region "CLASS CONSTRUCTOR"
			
			public        ClsDAL()
			{
				ResetErrors();
			}
			public string SQLqueries
			{
				get
				{
					return _strSQLqueries;
				}
			}
			public void   Dispose()
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			protected virtual void Dispose(bool disposing)
			{
				if (!_blnIsDisposed)
				{
					if (disposing)
					{
						if (_blnIsOnline || _blnIsConnecting)
							this.CloseConnection();
					}
					_blnIsDisposed = true;
				}
			}

		#endregion

		#region "HTML ENCODE AND DECODE"
		
			private string HTMLencode(string strInput)
			{
				return strInput;
			}
			private string HTMLdecode(string strInput)
			{
				return strInput;
			}
			
		#endregion
		
		#region "TIMER FUNCTIONS"

			private void		StartQueryTimer()
			{
				ResetErrors();
				if (_queryTimer == null)
						_queryTimer = new Util.Timer();
				_queryTimer.StartTimer();
			}
			private void		StopQueryTimer()
			{
				if (_queryTimer != null)
				{
					if (_queryTimer.IsRunning)
						_queryTimer.StopTimer();
					_fQueryLast			= _queryTimer.GetFloatTime;
					_fQueryAverage += _fQueryLast;
					_intQueryCount++;
				}
			}
			public	void		ResetAverage()
			{
				_fQueryAverage = 0;
				_intQueryCount = 0;
			}

			public	float		AverageQueryTime
			{
				get
				{
					if (_intQueryCount == 0 || _fQueryAverage == 0)
						return 0;
					else
						return _fQueryAverage / ((float)_intQueryCount);
				}
			}
			public	float		LastQueryTime
			{ 
				get
				{
					return _fQueryLast;
				}
			}

		#endregion

		#region "CONNECTION FUNCTIONS"

			public void OpenConnection()
			{
				if (_blnIsOnline || _blnIsConnecting)
					return;

				#if IS_DEBUGGING
				_strSQLqueries += "OPEN CONNECTION\n";
				#endif

				SQLOpenConnection();
			}
			public void OpenConnection(string strServer, string strDB, int intPort = 0)
			{
				SQLOpenConnection(strServer, strDB, intPort);
			}
			public void OpenConnection(string strServer, string strDB, string strUser, string strPwd, int intPort = 0)
			{
				SQLOpenConnection(strServer, strDB, strUser, strPwd, intPort);
			}

			public void CloseConnection()
			{
				#if IS_DEBUGGING
				_strSQLqueries += "CLOSE CONNECTION\n";
				#endif

				SQLCloseConnection();
			}
			public bool IsConnected
			{
				get
				{
					return SQLisConnected;
				}
			}
			public bool IsConnectedCheck
			{
				get
				{
					return _blnIsOnline;
				}
			}
			public bool IsConnecting
			{
				get
				{
					return SQLisConnecting;
				}
			}

		#endregion

		#region "PARAMETER FUNCTIONS"

			public void AddParam(string strParamName, string   strParamValue)
			{
				SQLAddParam(strParamName, strParamValue);
			}
			public void AddParam(string strParamName, int      intParamValue)
			{
				SQLAddParam(strParamName, intParamValue);
			}
			public void AddParam(string strParamName, decimal  decParamValue)
			{
				SQLAddParam(strParamName, decParamValue);
			}
			public void AddParam(string strParamName, float    fParamValue)
			{
				SQLAddParam(strParamName, fParamValue);
			}
			public void AddParam(string strParamName, double   decParamValue)
			{
				SQLAddParam(strParamName, decParamValue);
			}
			public void AddParam(string strParamName, long     lngParamValue)
			{
				SQLAddParam(strParamName,    lngParamValue);
			}
			public void AddParam(string strParamName, DateTime dateParamValue)
			{
				SQLAddParam(strParamName, dateParamValue);
			}
			public void AddParam(string strParamName, bool     blnParamValue)
			{
				SQLAddParam(strParamName, blnParamValue);
			}
			public void AddParam(string strParamName, byte[]	 buffer)
			{
				SQLAddParam(strParamName, buffer);
			}
			public void AddParam(string strParamName, DbType   sType)
			{
				SqlDbType st = new SqlDbType();
				switch (sType.ToString().ToLower())
				{
					case "int":
					case "int16":
					case "int32":
					case "int64":
						st = SqlDbType.Int;
						break;
					case "float":
					case "double":
					case "decimal":
						st = SqlDbType.Decimal;
						break;
					case "string":
					case "stringfixedlength":
						st = SqlDbType.VarChar;
						break;
					case "date":
					case "time":
					case "datetime":
					case "datetime2":
						st = SqlDbType.DateTime;
						break;
					case "bool":
					case "boolean":
						st = SqlDbType.Bit;
						break;
				}
				AddParam(strParamName, st);
			}
			public void AddParam(string strParamName, SqlDbType  sType)
			{
				SQLAddParam(strParamName, sType);
			}
			public void AddParam(string strParamName, SqlDbType  sType, int varSize)
			{
				SQLAddParam(strParamName, sType, varSize);
			}

			public void ClearParams()
			{
				SQLClearParams();

				_strSQLqueries = "";

				#if IS_DEBUGGING
				_strSQLqueries = "ClearParams()\n";
				#endif
			}

		#endregion

		#region "DIRECT SQL FUNCTIONS"

			public DataTable GetSQLSelectDataTable(	string strSQL)
			{
				DataTable dtRet = new DataTable();

				#if IS_DEBUGGING
				_strSQLqueries += "-- GetSQLSelectDataTable(" + strSQL + ")\n";
				#endif

				dtRet = SQLGetSQLSelectDataTable(strSQL);

				return dtRet;
			}
			public string    GetSQLSelectString(		string strSQL)
			{
				string strRet = "";

				#if IS_DEBUGGING
				_strSQLqueries += "-- GetSQLSelectString(" + strSQL + ")\n";
				#endif
								
				strRet = SQLGetSQLSelectString(strSQL);
				
				return strRet;
			}
			public int       GetSQLSelectInt(				string strSQL)
			{
				int intRet = 0;

				#if IS_DEBUGGING
				_strSQLqueries += "-- GetSQLSelectInt(" + strSQL + ")\n";
				#endif
								
				intRet = SQLGetSQLSelectInt(strSQL);

				return intRet;
			}
			public decimal   GetSQLSelectDecimal(		string strSQL)
			{
				decimal decRet = 0;

				#if IS_DEBUGGING
				_strSQLqueries += "-- GetSQLSelectDecimal(" + strSQL + ")\n";
				#endif
								
				decRet = SQLGetSQLSelectDecimal(strSQL);

				return decRet;
			}
			public float     GetSQLSelectFloat(			string strSQL)
			{
				float fRet = 0;

				#if IS_DEBUGGING
				_strSQLqueries += "-- GetSQLSelectFloat(" + strSQL + ")\n";
				#endif

				fRet = SQLGetSQLSelectFloat(strSQL);

				return fRet;
			}
			public double    GetSQLSelectDouble(		string strSQL)
			{
				double dblRet = 0;

				#if IS_DEBUGGING
				_strSQLqueries += "-- GetSQLSelectDecimal(" + strSQL + ")\n";
				#endif

				dblRet = SQLGetSQLSelectDouble(strSQL);

				return dblRet;
			}
			public bool      DoSQLUpdateDelete(			string strSQL)
			{
				bool blnRet = false;

				#if IS_DEBUGGING
				_strSQLqueries += "-- DoSQLUpdateDelete(" + strSQL + ")\n";
				#endif
								
				blnRet = SQLDoSQLUpdateDelete(strSQL);

				return blnRet;
			}

		#endregion

		#region "STORED PROCEDURE SELECT FUNCTIONS"

			public DataTable GetSPDataTable(string strSP)
			{
				DataTable dtRet = new DataTable();

				#if IS_DEBUGGING
				_strSQLqueries += "-- GetSPDataTable(" + strSP + ")\n";
				#endif
								
				dtRet = SQLGetSPDataTable(strSP);

				return dtRet;								
			}
			public string    GetSPString(string strSP)
			{
				string strRet = "";

				#if IS_DEBUGGING
				_strSQLqueries += "-- GetSPString(" + strSP + ")\n";
				#endif
								
				strRet = SQLGetSPString(strSP);
				
				return strRet;
			}
			public int       GetSPInt(string strSP)
			{
				int intRet = 0;

				#if IS_DEBUGGING
				_strSQLqueries += "-- GetSPInt(" + strSP + ")\n";
				#endif

				intRet = SQLGetSPInt(strSP);

				return intRet;
			}
			public long      GetSPLong(string strSP)
			{
				long lngRet = 0;

				#if IS_DEBUGGING
				_strSQLqueries += "-- GetSPInt(" + strSP + ")\n";
				#endif

				lngRet = SQLGetSPLong(strSP);

				return lngRet;
			}
			public decimal   GetSPDecimal(string strSP)
			{
				decimal decRet = 0;

				#if IS_DEBUGGING
				_strSQLqueries += "-- GetSPDecimal(" + strSP + ")\n";
				#endif

				decRet = SQLGetSPDecimal(strSP);

				return decRet;
			}
			public float     GetSPFloat(string strSP)
			{
				float fRet = 0;

				#if IS_DEBUGGING
				_strSQLqueries += "-- GetSPFloat(" + strSP + ")\n";
				#endif

				fRet = SQLGetSPFloat(strSP);

				return fRet;
			}
			public byte[]    GetSPBinary(string strSP)
			{
				byte[] binRet = null;

				#if IS_DEBUGGING
				_strSQLqueries += "-- GetSPBinary(" + strSP + ")\n";
				#endif

				binRet = SQLGetSPBinary(strSP);

				return binRet;
			}
			public void      ExecuteSP(string strSP)
			{
				#if IS_DEBUGGING
				_strSQLqueries += "-- ExecuteSP(" + strSP + ")\n";
				#endif

				SQLExecuteSP(strSP);
			}

		#endregion

		#region "STORED PROCEDURE UPDATE FUNCTIONS"

			public string  UpdateSPDataTable(string strSP, string  strPass)
			{
				string strRet = "";

				#if IS_DEBUGGING
				_strSQLqueries += "-- UpdateSPDataTable(" + strSP + ")\n";
				#endif

				strRet = SQLUpdateSPDataTable(strSP, strPass);

				return strRet;
			}
			public int     UpdateSPDataTable(string strSP, int     intPass)
			{
				int intRet = 0;

				#if IS_DEBUGGING
				_strSQLqueries += "-- UpdateSPDataTable(" + strSP + ")\n";
				#endif

				intRet = SQLUpdateSPDataTable(strSP, intPass);

				return intRet;
			}
			public decimal UpdateSPDataTable(string strSP, decimal decPass)
			{
				decimal decRet = 0;

				#if IS_DEBUGGING
				_strSQLqueries += "-- UpdateSPDataTable(" + strSP + ")\n";
				#endif

				decRet = SQLUpdateSPDataTable(strSP, decPass);

				return decRet;
			}

		#endregion

		#region "BULKCOPY / INSERT"

			public bool    BulkCopy(string strTableName, DataTable dtTable)
			{
				#if IS_DEBUGGING
				_strSQLqueries += "-- BulkCopy(" + strTableName + ")\n";
				#endif

				return SQLBulkCopier(strTableName, dtTable);
			}
			public bool		 InsertFromDataTable(string strSP, string strDataType, DataTable dtTable)
			{
				return SQLInsertFromDataTable(strSP, strDataType, dtTable);
			}

		#endregion

		#region "ERROR HANDLING"

			public void   ResetErrors()
			{
				_strErrors = "";
				_intErrorCount = 0;
			}
			public string Errors
			{
				get
				{
					return _strErrors;
				}
			}
			public void   ReportError(string strFile, string strFunc, string strPass, Exception ex)
			{
				string strLine = "";
				// ADD ERROR TO THE ERROR MESSAGE LIST
				string strError = "";
				strError		+= "\n";
				strError		+= "--- <b>ERROR in " + strFile + "." + strFunc + "</b> ------------- \n";
				if (strPass != "")
					strError	+= "<b>  Inputs:</b> " + strPass + "\n\n";
				if (strLine != "")
					strError	+= "<b> At Line:</b> " + strLine + "\n";
				strError		+= "<b> Message:</b> " + ex.Message + "\n";
				strError		+= "<b>  Target:</b> " + ex.TargetSite + "\n";
				if (ex.InnerException != null && ex.InnerException.ToString() != "")
					strError	+= "<b>   Inner:</b> " + ex.InnerException + "\n";
				strError		+= "\n";
				_strErrors		+= strError;
				UnityEngine.Debug.LogError(strError);
			}
			public void   ReportError(string strFile, string strFunc, string strPass, Exception ex, int intUserID)
			{
				string strLine = "";

				// ADD ERROR TO THE ERROR MESSAGE LIST
				string strError = "";
				strError		+= "\n";
				strError		+= "--- <b>ERROR in " + strFile + "." + strFunc + "</b> -------------\n";
				if (strPass != "")
					strError	+= "<b>  Inputs:</b> " + strPass + "\n\n";
				if (strLine != "")
					strError	+= "<b> At Line:</b> " + strLine + "\n";
				strError		+= "<b> Message:</b> " + ex.Message + "\n";
				strError		+= "<b>  Target:</b> " + ex.TargetSite + "\n";
				if (ex.InnerException != null && ex.InnerException.ToString() != "")
					strError	+= "<b>   Inner:</b> " + ex.InnerException + "\n";
				strError		+= "\n";
				_strErrors		+= strError;
				UnityEngine.Debug.LogError(strError);
			}

		#endregion
		
		#region "DATABASE BACKUP FUNCTIONS"
/*
			public Boolean BackupDatabase(string strBackupPath)
			{
				if (!blnDALonline)
					return false;
					
				try
				{
					// DETERMINE DATABASE NAME
					string strDatabase = "";
					if (ConfigurationManager.AppSettings["db_DB"] != null && ConfigurationManager.AppSettings["db_DB"] != "")
						strDatabase = ConfigurationManager.AppSettings["db_DB"];
					if (strDatabase == "")
						strDatabase = ConfigurationManager.AppSettings["app_name"];

					// SET BACKUP FILE TO DEFAULT IF PASSED STRING IS EMPTY
					if (strBackupPath == "")
						strBackupPath = "~/Database/";
					if (!strBackupPath.EndsWith("/"))
						strBackupPath += "/";

					// MAKE SURE THE DIRECTORY EXISTS
					if (!System.IO.Directory.Exists(strBackupPath))
						if (!System.IO.Directory.Exists(System.Web.HttpContext.Current.Server.MapPath(strBackupPath)))
							return false;
											
					// APPEND THE BACKUP FILENAME ONTO THE DIRECTORY STRUCTURE
					strBackupPath += strDatabase + DateTime.Now.AddHours(2).ToString("_yyyyMMdd-HHmm") + ".bak";
					strBackupPath =  System.Web.HttpContext.Current.Server.MapPath(strBackupPath);

					// IF THE BACKUP FILE ALREADY EXISTS, DELETE IT.
					//	OTHERWISE, THE SQL COMMAND WILL JUST APPEND TO THE FILE
					if (File.Exists(strBackupPath))
						System.IO.File.Delete(strBackupPath);

					// PERFORM THE BACKUP
					if (_DALType == DALtypes.MSSQL)
						return SQLBackupDatabase(strDatabase, strBackupPath);
				} catch (Exception ex) { 
//				this.ReportError("ClsDAL", "BackupDatabase", strBackupPath, ex);
				}

				return false;
			}
*/		
		#endregion
		
	#endregion

}
