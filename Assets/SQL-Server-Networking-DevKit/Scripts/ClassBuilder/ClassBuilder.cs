// ===========================================================================================================
//
// Class/Library: Class Builder - Main Script
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Mar 26, 2016
//	
// VERS 1.0.000 : Mar 26, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace CBT
{ 
	[System.Serializable]
	public class ClassBuilder 
	{

		[System.Serializable]
		public	class		ClassProperty
		{
			[SerializeField]	public	bool				IsIndex				= false;
			[SerializeField]	public	string			Name					= "";
			[SerializeField]	public	int					MaxLength			= 0;
			[SerializeField]	public	bool				IsDeleted			= false;
			[SerializeField]	public	bool				IsMandatory		= false;
			[SerializeField]	public	bool				IsSearchable	= false;
			[SerializeField]	public	bool				IsSynchVar		= false;
			[SerializeField]	private	string			_varType			= "";
			[SerializeField]	private string			_strStart			= "";

			public	string			VarType
			{
				get
				{
					return _varType;
				}
				set
				{
					_varType = value;
					if (_varType.ToLower() == "vector2" || _varType.ToLower() == "vector3" || _varType.ToLower() == "quaternion")
						MaxLength = 50;
				}
			}
			public	string			StartingValue
			{
				get
				{
					if (_strStart == "")
					{
						switch (_varType.ToLower())
						{
							case "int":
								return "0";
							case "float":
							case "double":
							case "decimal":
								return "0.00";
							case "vector2":
								return Vector2.zero.ToString();
							case "vector3":
								return Vector3.zero.ToString();
							case "quaternion":
								return Quaternion.identity.ToString();
							case "date":
							case "datetime":
	//						return "01/01/1900";
								break;
							case "bool":
								return "false";
						}
						if (_varType.ToLower().StartsWith("enum"))
							return _varType.Substring(4) + ".NONE";
					}
					return _strStart;
				}
				set
				{
					_strStart = value.Trim();
					if (_varType.ToLower() == "bool")
						_strStart = _strStart.ToLower();
				}
			}
			public	string			GetPrivatePrefix
			{
				get
				{
					switch (VarType.ToLower())
					{
						case "string":
							return "_str";
						case "int":
							return "_int";
						case "bool":
							return "_bln";
						case "float":
							return "_f";
						case "decimal":
							return "_dec";
						case "double":
							return "_dbl";
						case "datetime":
							return "_dt";
						case "vector2":
							return "_v2";
						case "vector3":
							return "_v3";
						case "quaternion":
							return "_q3";
					}
					if (VarType.ToLower().StartsWith("enum"))
						return "_en";
					return "_str";
				}
			}
			public	string			GetStartingValue
			{
				get
				{
					switch (VarType.ToLower())
					{
						case "string":
							return "\"\"";
						case "int":
							return "0";
						case "bool":
							return "true";
						case "float":
							return "0";
						case "decimal":
							return "0";
						case "double":
							return "0";
						case "datetime":
							return "System.DateTime.Parse(\"01/01/1900\")";
					}
					if (VarType.ToLower().StartsWith("enum"))
						return VarType.Substring(4) + ".NONE";
					return "\"\"";
				}
			}
			public	string			GetConversionFromDataRow
			{
				get
				{
					switch (VarType.ToLower())
					{
						case "string":
							return "dr[\"" + Name.ToUpper() + "\"].ToString()" ;
						case "int":
							return "Util.ConvertToInt(dr[\"" + Name.ToUpper() + "\"].ToString())";
						case "bool":
							return "Util.ConvertToBoolean(dr[\"" + Name.ToUpper() + "\"].ToString())";
						case "float":
						case "decimal":
						case "double":
							return "Util.ConvertToFloat(dr[\"" + Name.ToUpper() + "\"].ToString())";
						case "datetime":
							return "Util.ConvertToDate(dr[\"" + Name.ToUpper() + "\"].ToString())";
						case "vector2":
							return "Util.ConvertToVector2(dr[\"" + Name.ToUpper() + "\"].ToString())";
						case "vector3":
							return "Util.ConvertToVector3(dr[\"" + Name.ToUpper() + "\"].ToString())";
						case "quaternion":
							return "Util.ConvertToQuaternion(dr[\"" + Name.ToUpper() + "\"].ToString())";
					}
					if (VarType.ToLower().StartsWith("enum"))
						return "((" + VarType.Substring(4) + ") Util.ConvertToInt(dr[\"" + Name.ToUpper() + "\"].ToString()))";

					return "dr[\"" + Name.ToUpper() + "\"].ToString()";
				}
			}
			public	string			GetConversionToParam
			{
				get
				{
					if (VarType.ToLower().StartsWith("enum"))
						return Name + "Int";
					else
						switch (VarType.ToLower())
						{ 
							case "vector2":
							case "vector3":
							case "quaternion":
								return Name + ".ToString()";
							default:
								return Name;
						}
				}
			}
			public	string			GetConversionToType
			{
				get
				{
					if (VarType.ToLower().StartsWith("enum"))
						return Name + "Int";
					switch (VarType.ToLower())
					{ 
						case "datetime":
							return Name + ".ToString(\"MM/dd/yyyy HH:mm:ss\")";
						case "bool":
							return "Util.ConvertToInt(" + Name + ")";
						case "":
							return "";
						default:
							return Name + ".ToString()";
					}
				}
			}
			public	string			GetSQLtype
			{
				get
				{
					if (VarType.ToLower().StartsWith("enum"))
						return "INT";
					switch (VarType.ToLower())
					{
						case "vector2":
						case "vector3":
						case "quaternion":
							return "VARCHAR(50)";
						case "string":
						default:
							return "VARCHAR(" + MaxLength.ToString() + ")";
						case "int":
							return "INT";
						case "bool":
							return "BIT";
						case "float":
						case "decimal":
						case "double":
							return "DECIMAL(18, 6)";
						case "datetime":
							return "DATETIME";
					}
				}
			}
			public	string			GetSQLtype2
			{
				get
				{
					if (VarType.ToLower().StartsWith("enum"))
						return "INT";
					switch (VarType.ToLower())
					{
						case "vector2":
						case "vector3":
						case "quaternion":
						case "string":
						default:
							return "VARCHAR";
						case "int":
							return "INT";
						case "bool":
							return "BIT";
						case "float":
						case "decimal":
						case "double":
							return "DECIMAL";
						case "datetime":
							return "DATETIME";
					}
				}
			}

			public	string			Serialize()
			{
				string strOut = "";

				strOut += Name			+ "^";
				strOut += _varType	+ "^";
				strOut += _strStart	+ "^";
				strOut += MaxLength.ToString() + "^";
				strOut += Util.ConvertToInt(IsIndex).ToString()				+ "^";
				strOut += Util.ConvertToInt(IsDeleted).ToString()			+ "^";
				strOut += Util.ConvertToInt(IsMandatory).ToString()		+ "^";
				strOut += Util.ConvertToInt(IsSearchable).ToString()	+ "^";
				strOut += Util.ConvertToInt(IsSynchVar).ToString();

				return strOut;
			}
			public	void				Deserialize(string strInput)
			{
				string[] strSpl = strInput.Split('^');

				Name								= strSpl[0];
				try { _varType			= strSpl[1];	} catch { _varType	= "string";	}
				try { _strStart			= strSpl[2];	} catch { _strStart = "";				}
				try { MaxLength			= Util.ConvertToInt(strSpl[3]);			} catch { MaxLength			= 0;			}
				try { IsIndex				= Util.ConvertToBoolean(strSpl[4]);	} catch { IsIndex				= false;	}
				try { IsDeleted			= Util.ConvertToBoolean(strSpl[5]); } catch { IsDeleted			= false;	}
				try { IsMandatory		= Util.ConvertToBoolean(strSpl[6]); } catch { IsMandatory		= false;	}
				try { IsSearchable	= Util.ConvertToBoolean(strSpl[7]); } catch { IsSearchable	= false;	}
				try { IsSynchVar		=	Util.ConvertToBoolean(strSpl[8]); } catch { IsSynchVar		= false;	}
			}
		}

		#region "PRIVATE/PROTECTED CONSTANTS"

			protected	static	string		APP_ROOT_DIRECTORY			= "SQL-Server-Networking-DevKit/";
			protected static	string		SCRIPT_DIRECTORY				= APP_ROOT_DIRECTORY + "Class Scripts/";									// WHERE THE CREATED SCRIPT FILES ARE WRITTEN
			protected	static	string		DATABASE_FILE_DIRECTORY	= APP_ROOT_DIRECTORY + "Scripts/ClassBuilder/Database";		// WHERE THE UNITY DATABASE FILE IS STORED FOR THE CLASSES WE CREATE
			protected	static	string		DATABASE_FILE_NAME			= "ClassScriptDatabase.asset";														// THE NAME OF THE DATABASE FILE FOR THE CLASSES WE CREATE

		#endregion

		#region "PRIVATE VARIABLES"
			
			// CLASS VARIABLES -- USED BY SYSTEM
			[SerializeField, HideInInspector]	private	int					_intID									= 0;				
			[SerializeField, HideInInspector]	private int					_intIndex								= -1;				
			[SerializeField, HideInInspector]	private bool				_blnIsActive						= true;		

			// CLASS VARIABLES -- SET BY USER
			[SerializeField, HideInInspector]	private string			_strNamespace						= "";
			[SerializeField, HideInInspector]	private string			_strClassName						= "";
			[SerializeField, HideInInspector]	private bool				_blnUseUnity						= true;
			[SerializeField, HideInInspector]	private bool				_blnUseUnityUI					= false;
			[SerializeField, HideInInspector]	private bool				_blnIsNetworkObject			= false;
			[SerializeField, HideInInspector]	private bool				_blnHasNetworkTransform	= false;
			[SerializeField, HideInInspector]	private bool				_blnUseEditor						= true;
			[SerializeField, HideInInspector]	private bool				_blnUseDatabase					= true;
			[SerializeField, HideInInspector]	private bool				_blnUseClassMgr					= true;
			[SerializeField, HideInInspector]	private bool				_blnUseAppMgr						= false;
			[SerializeField, HideInInspector]	private bool				_blnUseDBmgr						= true;
			[SerializeField, HideInInspector]	private bool				_blnUseNetMgr						= false;
			[SerializeField, HideInInspector]	private bool				_blnUseDBload						= true;
			[SerializeField, HideInInspector]	private bool				_blnUseDBsave						= true;
			[SerializeField, HideInInspector]	private bool				_blnUseSerialization		= true;
			[SerializeField, HideInInspector]	private	string			_DBserver								= "";
			[SerializeField, HideInInspector]	private	string			_DBdatabase							= "";
			[SerializeField, HideInInspector]	private	string			_DBuser									= "";
			[SerializeField, HideInInspector]	private	string			_DBpassword							= "";
			[SerializeField, HideInInspector]	private bool				_DBuseWinAccount				= false;
			[SerializeField, HideInInspector]	private	List<ClassProperty>		_vars					= new List<ClassProperty>();

			// INTERNAL PROCESS VARIABLES
			[System.NonSerialized]
												private bool				_blnInitialized				= false;
			[System.NonSerialized]
												private string			_strFileData					= "";

		#endregion

		#region "PRIVATE PROPERTIES"

			private string			CLASS_SCRIPT_DIRECTORY
			{
				get
				{
					return SCRIPT_DIRECTORY + Name + " Class Scripts";
				}
			}
			private	string			GetPropertyType(ClassProperty prop, bool blnIncludeSrc = false)
			{
				if (prop.VarType.ToLower().StartsWith("enum"))
					return ((blnIncludeSrc) ? ((Namespace != "") ? Namespace + "." : "") + ClassName + "." : "") + prop.VarType.Substring(4);
				else if (prop.VarType.ToLower().StartsWith("date"))
					return "System.DateTime";
				else
					return prop.VarType;
			}

			private string			GetIndexPrivateVariable
			{
				get
				{
					for (int i = 0; i < Variables.Count; i++)
					{
						if (Variables[i].IsIndex)
							return Variables[i].GetPrivatePrefix + Variables[i].Name;
					}
					return "";
				}
			}
			private string			GetIndexVariable
			{
				get
				{
					for (int i = 0; i < Variables.Count; i++)
					{
						if (Variables[i].IsIndex)
							return Variables[i].Name;
					}
					return "";
				}
			}
		
			private string			Serialize()
			{
				string strOut = "";

				// SERIALIZE BASE CLASS PORTION
				strOut += _strClassName	+ "^";
				strOut += _strNamespace	+ "^";
				strOut += _DBserver			+ "^";
				strOut += _DBdatabase		+ "^";
				strOut += _DBuser				+ "^";
				strOut += _DBpassword		+ "^";
				strOut += Util.ConvertToInt(_blnUseUnity).ToString()						+ "^";
				strOut += Util.ConvertToInt(_blnUseUnityUI).ToString()					+ "^";
				strOut += Util.ConvertToInt(_blnIsNetworkObject).ToString()			+ "^";
				strOut += Util.ConvertToInt(_blnHasNetworkTransform).ToString()	+ "^";
				strOut += Util.ConvertToInt(_blnUseEditor).ToString()						+ "^";
				strOut += Util.ConvertToInt(_blnUseDatabase).ToString()					+ "^";
				strOut += Util.ConvertToInt(_blnUseClassMgr).ToString()					+ "^";
				strOut += Util.ConvertToInt(_blnUseAppMgr).ToString()						+ "^";
				strOut += Util.ConvertToInt(_blnUseDBmgr).ToString()						+ "^";
				strOut += Util.ConvertToInt(_blnUseNetMgr).ToString()						+ "^";
				strOut += Util.ConvertToInt(_blnUseDBload).ToString()						+ "^";
				strOut += Util.ConvertToInt(_blnUseDBsave).ToString()						+ "^";
				strOut += Util.ConvertToInt(_blnUseSerialization).ToString()		+ "^";
				strOut += Util.ConvertToInt(_DBuseWinAccount).ToString()				+ "|";

				// SERIALIZE PROPERTIES PORTION
				bool bln = false;
				for (int i = 0; i < Variables.Count; i++)
				{
					if (!Variables[i].IsDeleted)
					{
						if (!bln)
							bln = true;
						else
							strOut += "©";
						strOut += Variables[i].Serialize();
					}
				}
				return strOut;
			}
			private void				Deserialize(string strInput)
			{
				string[] strSpl1 = strInput.Split('|');
				string[] strSpl2 = strSpl1[0].Split('^');
				string[] strSpl3 = strSpl1[1].Split('©');

				// DESERIALIZE BASE CLASS PORTION
				_strClassName = strSpl2[0];
				_strNamespace = strSpl2[1];
				_DBserver			= strSpl2[2];
				_DBdatabase		= strSpl2[3];
				_DBuser				= strSpl2[4];
				_DBpassword		= strSpl2[5];
				_blnUseUnity						= Util.ConvertToBoolean(strSpl2[6]);
				_blnUseUnityUI					= Util.ConvertToBoolean(strSpl2[7]);
				_blnIsNetworkObject			= Util.ConvertToBoolean(strSpl2[8]);
				_blnHasNetworkTransform	= Util.ConvertToBoolean(strSpl2[9]);
				_blnUseEditor						= Util.ConvertToBoolean(strSpl2[10]);
				_blnUseDatabase					= Util.ConvertToBoolean(strSpl2[11]);
				_blnUseClassMgr					= Util.ConvertToBoolean(strSpl2[12]);
				_blnUseAppMgr						= Util.ConvertToBoolean(strSpl2[13]);
				_blnUseDBmgr						= Util.ConvertToBoolean(strSpl2[14]);
				_blnUseNetMgr						= Util.ConvertToBoolean(strSpl2[15]);
				_blnUseDBload						= Util.ConvertToBoolean(strSpl2[16]);
				_blnUseDBsave						= Util.ConvertToBoolean(strSpl2[17]);
				_blnUseSerialization		= Util.ConvertToBoolean(strSpl2[18]);
				_DBuseWinAccount				= Util.ConvertToBoolean(strSpl2[19]);

				// DESERIALIZE PROPERTIES PORTION
				_vars = new List<ClassProperty>();
				foreach (string st in strSpl3)
				{
					ClassProperty prop = new ClassProperty();
					prop.Deserialize(st);
					_vars.Add(prop);
				}

				_blnInitialized = true;
			}

		#endregion

		#region "PUBLIC PROPERTIES"

			public	int					ID
			{
				get
				{
					return _intID;
				}
				set
				{
					_intID = value;
				}
			}
			public	int					Index
			{
				get
				{
					return _intIndex;
				}
				set
				{
					_intIndex = value;
				}
			}
			public	bool				IsActive
			{
				get
				{
					return _blnIsActive;
				}
				set
				{
					_blnIsActive = value;
				}
			}

			public	string			Name								{ get { return _strClassName;						} set { _strClassName = value.Trim(); } }
			public	string			ClassName						{ get { return _strClassName;						} set { _strClassName = value.Trim(); } }
			public	string			Namespace						{ get { return _strNamespace;						} set { _strNamespace = value.Trim(); } }
			public	bool				IsInitialized				{ get { return _blnInitialized;					} set { _blnInitialized = value; } }
			public	bool				UseUnity						{ get { return _blnUseUnity;						} set { _blnUseUnity = value; } }
			public	bool				IsANetworkObject		{ get { return _blnIsNetworkObject;			} set { _blnIsNetworkObject = value; } }
			public	bool				HasNetworkTransform	{	get { return _blnHasNetworkTransform;	} set { _blnHasNetworkTransform = value; } }
			public	bool				UseUnityUI					{ get { return _blnUseUnityUI;					} set { _blnUseUnityUI = value; } }
			public	bool				UseEditor						{ get { return _blnUseEditor;						} set { _blnUseEditor = value; } }
			public	bool				UseDatabase					{ get { return _blnUseDatabase;					} set { _blnUseDatabase = value; } }
			public	bool				UseClassMgr					{ get { return _blnUseClassMgr;					} set { _blnUseClassMgr = value; } }
			public	bool				UseAppMgr						{ get { return _blnUseAppMgr;						} set { _blnUseAppMgr = value; } }
			public	bool				UseDBmgr						{ get { return _blnUseDBmgr;						} set { _blnUseDBmgr = value; } }
			public	bool				UseNetMgr						{ get { return _blnUseNetMgr;						} set { _blnUseNetMgr = value; } }
			public	bool				UseDBload						{ get { return _blnUseDBload;						} set { _blnUseDBload = value; } }
			public	bool				UseDBsave						{ get { return _blnUseDBsave;						} set { _blnUseDBsave = value; } }
			public	bool				UseSerialization		{ get { return _blnUseSerialization;		} set { _blnUseSerialization = value; } }
		
			public	string			DBserver						{ get { return _DBserver;								} set { _DBserver = value.Trim(); } }
			public	string			DBdatabase					{ get { return _DBdatabase;							} set { _DBdatabase = value.Trim(); } }
			public	string			DBuser							{ get { return _DBuser;									} set { _DBuser = value.Trim(); } }
			public	string			DBpassword					{ get { return _DBpassword;							} set { _DBpassword = value.Trim(); } }
			public	bool				DBuseWinAccount			{ get { return _DBuseWinAccount;				} set { _DBuseWinAccount = value; } }

			public	List<ClassProperty>		Variables	{ get { return _vars; } }
			public	bool				HasVariableType(string strType)
			{
				List<ClassProperty> li = Variables.FindAll(x => x.VarType.ToLower() == strType.ToLower());
				return (li != null && li.Count > 0);
			}

			public	string			Script_Directory
			{
				get
				{
					return CLASS_SCRIPT_DIRECTORY;
				}
			}
			public	string			DatabaseCall
			{
				get
				{
					if (UseDatabase)
					{ 
						if (UseUnity && UseDBmgr)
							return "Database.DAL.";
						else
							return "DAL.";
					} else
						return "";
				}
			}

		#endregion

		#region "PUBLIC STATIC FUNCTIONS"

			public	static	BaseDatabase<ClassBuilder>			LoadDatabase()
			{
				BaseDatabase<ClassBuilder> db = null;
				string strDBfullPath = @"Assets/" + DATABASE_FILE_DIRECTORY + "/" + DATABASE_FILE_NAME;

				try
				{
					// db = ScriptableObject.CreateInstance<BaseDatabase<BaseEffect>>();
					if (!System.IO.Directory.Exists(@"Assets/" + DATABASE_FILE_DIRECTORY))
					{
						System.IO.Directory.CreateDirectory(@"Assets/" + DATABASE_FILE_DIRECTORY);
					} else {
						#if UNITY_EDITOR
						db = AssetDatabase.LoadAssetAtPath(strDBfullPath, typeof(BaseDatabase<ClassBuilder>)) as BaseDatabase<ClassBuilder>;
						#else
							db = Resources.GetBuiltinResource(typeof(BaseDatabase<ClassBuilder>), strDBfullPath) as BaseDatabase<ClassBuilder>;
						#endif
					}

					#if UNITY_EDITOR
					if (db == null)
					{
						db = ScriptableObject.CreateInstance<BaseDatabase<ClassBuilder>>();
						AssetDatabase.CreateAsset(db, strDBfullPath);
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();
					}
					#endif
					db.IsDatabaseLoaded = (db != null);

					// INITIALIZE INDEX
					if (db != null && db.database != null && db.database.Count > 0)
					{
						for (int i = 0; i < db.database.Count; i++)
						{
							db.database[i].Index = 0;
						}
					}
				} catch {
					Debug.LogError("Error Loading " + db.name.ToString() + " \"" + DATABASE_FILE_NAME + "\"  (" + strDBfullPath + ")");
					return null;
				}
				return db;
			}

		#endregion

		#region "PRIVATE FUNCTIONS"

			// SQL BUILDING SUB-FUNCTIONS
			private string			DropConstraint(string strVar)
			{
				string strSQL		= "";
				string strTable	= "tbl" + this.ClassName;

				strSQL += "IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_" + strTable + "_" + strVar + "]') AND type = 'D')\n";
				strSQL += "BEGIN\n";
				strSQL += "ALTER TABLE [dbo].[" + strTable + "] DROP CONSTRAINT [DF_" + strTable + "_" + strVar + "]\n";
				strSQL += "END\nGO\n\n";

				return strSQL;
			}
			private string			AddConstraint(ClassProperty prop)
			{
				string strSQL = "";

				strSQL += "ALTER TABLE [dbo].[tbl" + this.ClassName + "] ADD CONSTRAINT [DF_tbl" + this.ClassName + "_" + prop.Name + "] DEFAULT ";
				if (prop.VarType.ToLower().StartsWith("enum"))
					strSQL += "0";
				else
					switch (prop.VarType.ToLower())
					{
						case "string":
						default:
							strSQL += "('" + prop.StartingValue + "')";
							break;
						case "int":
						case "float":
						case "decimal":
						case "double":
							strSQL += prop.StartingValue;
							break;
						case "bool":
							strSQL += "((" +  Util.ConvertToInt(prop.StartingValue.ToLower() == "true") + "))";
							break;
						case "vector2":
						case "vector3":
						case "quaternion":
							strSQL += "('" + prop.StartingValue + "')";
							break;
						case "date":
						case "datetime":
							if (prop.StartingValue == "")
								strSQL += "(getdate())";
							else
								strSQL += "('" + prop.StartingValue + "')";
							break;
					}
				strSQL += " FOR [" + prop.Name + "]\n";
				strSQL += "GO\n\n";

				return strSQL;
			}

			// SQL BUILDING FUNCTIONS
			private void				BuildDropConstraints()
			{
				for (int i = 0; i < Variables.Count; i++)
				{
					_strFileData += DropConstraint(Variables[i].Name);
				}
			}
			private void				BuildDropStoredProcedures()
			{
				_strFileData += "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spGet" + this.ClassName + "ByID]') AND type in (N'P', N'PC'))\n";
				_strFileData += "DROP PROCEDURE [dbo].[spGet" + this.ClassName + "ByID];\n";
				_strFileData += "GO\n\n";
				_strFileData += "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spUpdate" + this.ClassName + "]') AND type in (N'P', N'PC'))\n";
				_strFileData += "DROP PROCEDURE [dbo].[spUpdate" + this.ClassName + "];\n";
				_strFileData += "GO\n\n";
				for (int i = 0; i < Variables.Count; i++)
				{
					if (Variables[i].IsSearchable)
					{ 
						_strFileData += "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spGet" + this.ClassName + "By" + Variables[i].Name + "]') AND type in (N'P', N'PC'))\n";
						_strFileData += "DROP PROCEDURE [dbo].[spGet" + this.ClassName + "By" + Variables[i].Name + "];\n";
						_strFileData += "GO\n\n";
					}
				}
			}
			private void				BuildDropTable()
			{
				_strFileData += "IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tbl" + this.ClassName + "]') AND type in (N'U'))\n";
				_strFileData += "DROP TABLE [dbo].[tbl" + this.ClassName + "]\n";
				_strFileData += "GO\n\n";
			}
			private void				BuildCreateTable()
			{
				_strFileData += "SET ANSI_NULLS ON\n";
				_strFileData += "GO\n\n";
				_strFileData += "SET QUOTED_IDENTIFIER ON\n";
				_strFileData += "GO\n\n";
				_strFileData += "SET ANSI_PADDING ON\n";
				_strFileData += "GO\n\n";
				_strFileData += "CREATE TABLE [dbo].[tbl" + this.ClassName + "](\n";
				for (int i = 0; i < Variables.Count; i++)
				{
					if (!Variables[i].IsDeleted)
					{
						_strFileData += "[" + Variables[i].Name + "] [" + Variables[i].GetSQLtype2 + "]";
						if (Variables[i].VarType.ToLower() == "string" ||
								Variables[i].VarType.ToLower() == "vector2" ||
								Variables[i].VarType.ToLower() == "vector3" ||
								Variables[i].VarType.ToLower() == "quaternion")
							_strFileData += "(" + Variables[i].MaxLength.ToString() + ")";
						else if (Variables[i].VarType.ToLower() == "float" ||
										 Variables[i].VarType.ToLower() == "decimal" ||
										 Variables[i].VarType.ToLower() == "double")
							_strFileData += "(18, 6)";
						_strFileData += " NOT NULL";
						if (i < Variables.Count - 1)
							_strFileData += ",\n";
					}
				}
				_strFileData += ") ON [PRIMARY]\n\n";
				_strFileData += "GO\n\n";
				_strFileData += "SET ANSI_PADDING OFF\n";
				_strFileData += "GO\n\n";
			}
			private void				BuildAddConstraints()
			{
				for (int i = 0; i < Variables.Count; i++)
				{
					if (!Variables[i].IsDeleted)
						_strFileData += AddConstraint(Variables[i]);
				}
			}
			private void				BuildLoadingStoredProcedures()
			{
				if (UseDBload)
				{ 
					// -- PRIMARY LOAD SCRIPT (BY THE ID)
					_strFileData += "SET ANSI_NULLS ON\n";
					_strFileData += "GO\n\n";
					_strFileData += "SET QUOTED_IDENTIFIER ON\n";
					_strFileData += "GO\n\n";
//				_strFileData += "-- =============================================\n";
//				_strFileData += "-- Create date: " + System.DateTime.Now.ToString("MMM dd yyyy - hh:mm:ss tt") + "\n";
//				_strFileData += "-- Description: spGet" + this.ClassName + "ByID\n";
//				_strFileData += "-- =============================================\n";
					_strFileData += "CREATE PROCEDURE [dbo].[spGet" + this.ClassName + "ByID] (\n";
 					_strFileData += "	@ID					INT = 0,\n";
					_strFileData += "	@ACTIVEONLY	BIT = FALSE\n";
					_strFileData += ") AS\n";
					_strFileData += "BEGIN\n\n";
					_strFileData += "	IF (@ID = 0)\n	BEGIN\n\n";
					_strFileData += "		SELECT * \n";
					_strFileData += "		  FROM tbl" + this.ClassName + "\n";
					_strFileData += "		 WHERE ((@ACTIVEONLY = ((1)) AND IsActive = ((1)) ) OR @ACTIVEONLY = ((0)) )\n";
					_strFileData += "		 ORDER BY " + GetIndexVariable + ";\n";
					_strFileData += "\n	END\n	ELSE\n	BEGIN\n\n";
					_strFileData += "		SELECT TOP 1 * \n";
					_strFileData += "		  FROM tbl" + this.ClassName + "\n";
					_strFileData += "		 WHERE " + GetIndexVariable + " = @ID\n";
					_strFileData += "		   AND ((@ACTIVEONLY = ((1)) AND IsActive = ((1)) ) OR @ACTIVEONLY = ((0)) );\n";
					_strFileData += "\n	END\n";
					_strFileData += "\nEND\n\n";
					_strFileData += "GO\n\n";

					// -- FIND BY FIELD SCRIPTS
					for (int i = 0; i < Variables.Count; i++)
					{
						if (!Variables[i].IsDeleted && Variables[i].IsSearchable)
						{
							_strFileData += "SET ANSI_NULLS ON\n";
							_strFileData += "GO\n\n";
							_strFileData += "SET QUOTED_IDENTIFIER ON\n";
							_strFileData += "GO\n\n";
//						_strFileData += "-- =============================================\n";
//						_strFileData += "-- Create date: " + System.DateTime.Now.ToString("MMM dd yyyy - hh:mm:ss tt") + "\n";
//						_strFileData += "-- Description: spGet" + this.ClassName + "By" + Variables[i].Name + "\n";
//						_strFileData += "-- =============================================\n";
							_strFileData += "CREATE PROCEDURE [dbo].[spGet" + this.ClassName + "By" + Variables[i].Name + "] (\n";
							_strFileData += "	@FIND				" + Variables[i].GetSQLtype + ",\n";
							_strFileData += "	@ACTIVEONLY	BIT = TRUE\n";
							_strFileData += ") AS\n";
							_strFileData += "BEGIN\n\n";
							_strFileData += "	SELECT TOP 1 * \n";
							_strFileData += "	  FROM tbl" + this.ClassName + "\n";
							if (Variables[i].VarType.ToLower() == "string")
								_strFileData += "	 WHERE LOWER(" + Variables[i].Name + ") = LOWER(@FIND)\n";
							else
								_strFileData += "	 WHERE " + Variables[i].Name + " = @FIND\n";
							_strFileData	 +="		AND ((@ACTIVEONLY = ((1)) AND IsActive = ((1))) OR @ACTIVEONLY = ((0)) );\n";
							_strFileData += "\nEND\n\n";
							_strFileData += "GO\n\n";
						}
					}
				}			
			}
			private void				BuildSavingStoredProcedure()
			{
				if (UseDBsave)
				{ 
					string strIndex = GetIndexVariable;
					_strFileData += "SET ANSI_NULLS ON\n";
					_strFileData += "GO\n\n";
					_strFileData += "SET QUOTED_IDENTIFIER ON\n";
					_strFileData += "GO\n\n";
//				_strFileData += "-- =============================================\n";
//				_strFileData += "-- Create date: " + System.DateTime.Now.ToString("MMM dd yyyy - hh:mm:ss tt") + "\n";
//				_strFileData += "-- Description: spUpdate" + this.ClassName + "\n";
//				_strFileData += "-- =============================================\n";
					_strFileData += "CREATE PROCEDURE [dbo].[spUpdate" + this.ClassName + "]( \n";
					for (int i = 0; i < Variables.Count; i++)
					{
						if (!Variables[i].IsDeleted && 
								 Variables[i].Name.ToLower() != "datecreated" && 
								 Variables[i].Name.ToLower() != "dateupdated")
								_strFileData += "	@" + Variables[i].Name.ToUpper() + "		" + Variables[i].GetSQLtype + ",\n";
					}
					_strFileData += "	@NEWID				INT OUT\n";
					_strFileData += ")\n";
					_strFileData += "AS\n";
					_strFileData += "BEGIN\n\n";
					_strFileData += "	SET NOCOUNT ON;\n";
					_strFileData += "	DECLARE @Find AS INT;\n";
					_strFileData += "	DECLARE @Max  AS INT;\n\n";
					_strFileData += "	IF (@" + strIndex.ToUpper() + " = 0)\n";
					_strFileData += "	BEGIN\n\n";
					_strFileData += "		SELECT TOP 1 @" + strIndex.ToUpper() + " = " + strIndex + "\n";
					_strFileData += "		  FROM tbl" + this.ClassName + "\n";
					_strFileData += "		 WHERE " + strIndex + " = @" + strIndex.ToUpper() + "\n\n";
					_strFileData += "	END;\n\n";
					_strFileData += "	SELECT @Find=COUNT(" + strIndex + ") FROM tbl" + this.ClassName + " WHERE " + strIndex + " = @" + strIndex.ToUpper() + ";\n\n";
					_strFileData += "	IF (@Find > 0 AND @" + strIndex.ToUpper() + " > 0)\n";
					_strFileData += "	BEGIN\n\n";
					_strFileData += "		UPDATE tbl" + this.ClassName + " SET\n";
					bool blnStarted = false;
					for (int i = 0; i < Variables.Count; i++)
					{
						if (!Variables[i].IsDeleted && !Variables[i].IsIndex && Variables[i].Name.ToLower() != "datecreated" && Variables[i].Name.ToLower() != "dateupdated")
						{ 
							if (blnStarted)
								_strFileData += ",\n";
							blnStarted = true;
							_strFileData += "							" + Variables[i].Name + "	= @" + Variables[i].Name.ToUpper();
						}
					}
					_strFileData += "\n				WHERE " + strIndex + " = @" + strIndex.ToUpper() + ";\n";
					_strFileData += "			SET @NEWID = @" + strIndex.ToUpper() + ";\n\n";
					_strFileData += "	END\n";
					_strFileData += "	ELSE\n";
					_strFileData += "	BEGIN\n\n";
					_strFileData += "		SELECT @Max = MAX(" + strIndex + ")+1 FROM tbl" + this.ClassName + ";\n\n";
					_strFileData += "		IF (@Max IS NULL OR @Max < 1)\n";
					_strFileData += "		BEGIN\n";
					_strFileData += "			SELECT @Max=COUNT(" + strIndex + ")+1 FROM tbl" + this.ClassName + ";\n";
					_strFileData += "		END\n\n";
					_strFileData += "		INSERT INTO tbl" + this.ClassName + " \n";
					_strFileData += "		(" + strIndex;
					for (int i = 0; i < Variables.Count; i++)
					{
						if (!Variables[i].IsDeleted && !Variables[i].IsIndex)
							_strFileData += ", " + Variables[i].Name;
					}
					_strFileData += ")\n		VALUES \n";
					_strFileData += "		(@Max";
					for (int i = 0; i < Variables.Count; i++)
					{
						if (!Variables[i].IsDeleted && !Variables[i].IsIndex)
						{
							if (Variables[i].Name.ToLower() == "dateupdated" || Variables[i].Name.ToLower() == "datecreated")
								_strFileData += ", GETDATE()";
							else 
								_strFileData += ", @" + Variables[i].Name.ToUpper();
						}
					}
					_strFileData += ");\n\n		SET @NEWID = @Max \n\n";
					_strFileData += "	END\n\n";
					_strFileData += "END\n\n";
					_strFileData += "GO\n\n";
				}			
			}

			// SCRIPT BUILDING FUNCTIONS
			private	void				CreateCSbaseFile()
			{
				BaseDatabase<EnumBuilder> dbEnums = EnumBuilder.LoadDatabase();
				EnumBuilder eb = null;

				string strFileName = this.ClassName + "Base";
				string strSer				= "";
				string strSync			= "[SyncVar] ";
				if (UseUnity && UseSerialization)
				{ 
					strSer	= "[SerializeField]	";
					strSync	= "[SerializeField, SyncVar]	";
				}
				_strFileData = "";

				// COMMENTED HEADER
				_strFileData += "//		AUTO-GENERATED FILE: " + strFileName + ".cs\n";
				_strFileData += "//		GENERATED ON       : " + System.DateTime.Now.ToString("ddd MMM dd yyyy - hh:mm:ss tt") + "\n";
				_strFileData += "//		\n";
				_strFileData += "//		This is the Base Class file.  It is not intended to be modified.\n";
				_strFileData += "//		If you need to make changes or additions, please modify the " + this.ClassName + ".cs File.\n";
				_strFileData += "\n\n";


				if (UseUnity)
					_strFileData	+= "using UnityEngine;\n";
				if (UseUnity && UseUnityUI)
					_strFileData	+= "using UnityEngine.UI;\n";
				if (IsANetworkObject)
					_strFileData	+= "using UnityEngine.Networking;\n";
				_strFileData		+= "using System.Collections;\n";
				_strFileData		+= "using System.Collections.Generic;\n";
				if (UseDatabase)
					_strFileData	+= "using System.Data;\n";
				_strFileData		+= "\n";

				if (Namespace != "")
					_strFileData	+= "namespace " + this.Namespace + "\n{\n\n";
				if (UseUnity)
				{
					if (IsANetworkObject)
					{
						if (HasNetworkTransform)
						_strFileData	+= "[RequireComponent(typeof(NetworkTransform))]\n";
						_strFileData	+= "public class " + strFileName + " : NetworkBehaviour\n{\n\n";
					} else
						_strFileData	+= "public class " + strFileName + " : MonoBehaviour\n{\n\n";
				} else
					_strFileData	+= "public class " + strFileName + "\n{\n\n";

				// BUILD PRIVATE VARIABLE REGION		-----------------------------------------------
				_strFileData		+= "	#region \"PRIVATE VARIABLES\"\n\n";
				for (int i = 0; i < Variables.Count; i++)
				{
					if (!Variables[i].IsDeleted)
					{ 
						if (Variables[i].IsIndex)
							_strFileData += "		// INDEX VARIABLE\n";
						if (IsANetworkObject && Variables[i].IsSynchVar)
						_strFileData += "		" + strSync + "protected	" + GetPropertyType(Variables[i]) + "		" + Variables[i].GetPrivatePrefix + Variables[i].Name;
						else
						_strFileData += "		" + strSer + "protected	" + GetPropertyType(Variables[i]) + "		" + Variables[i].GetPrivatePrefix + Variables[i].Name;
						if (Variables[i].VarType.ToLower() == "vector2" ||
								Variables[i].VarType.ToLower() == "vector2" ||
								Variables[i].VarType.ToLower() == "vector2" ||
								Variables[i].VarType.ToLower() == "vector3" ||
								Variables[i].VarType.ToLower() == "quaternion")
							_strFileData += ";\n";
						else if ((Variables[i].VarType.ToLower() == "date" ||
											Variables[i].VarType.ToLower() == "datetime"))
						{
							if (Variables[i].StartingValue == "")
								_strFileData += " = System.DateTime.Parse(\"01/01/1900\");\n";
							else
								_strFileData += " = System.DateTime.Parse(\"" + Variables[i].StartingValue + "\");\n";
						}
						else if (Variables[i].VarType.ToLower() == "string")
							_strFileData += "	=	\"" + Variables[i].StartingValue + "\";\n";
						else if (Variables[i].VarType.ToLower() == "float")
							_strFileData += "	=	" + Variables[i].StartingValue + "f;\n";
						else
							_strFileData += "	=	" + Variables[i].StartingValue + ";\n";
					}
				}
				if (IsANetworkObject)
				_strFileData += "		private NetworkConnection		_netConn;\n";

				_strFileData += "\n	#endregion\n\n";

				// BUILD PRIVATE PROPERTY REGION		-----------------------------------------------
				_strFileData += "	#region \"PRIVATE PROPERTIES\"\n\n";
				if (UseAppMgr)
				{
					_strFileData += "		protected	ApplicationManager		_app	= null;\n";
					_strFileData += "		protected	ApplicationManager		App\n		{\n";
					_strFileData += "			get {\n";
					_strFileData += "				if (_app == null)		_app = ApplicationManager.Instance;\n";
					_strFileData += "					return _app;\n			}\n";
					_strFileData += "		}\n";
				}
				if (UseNetMgr)
				{
					_strFileData += "		protected	AppNetworkManager			_net	= null;\n";
					_strFileData += "		protected	AppNetworkManager			Net\n		{\n";
					_strFileData += "			get {\n";
					_strFileData += "				if (_net == null)		_net = AppNetworkManager.Instance;\n";
					_strFileData += "					return _net;\n			}\n";
					_strFileData += "		}\n";
				}
				if (UseDBmgr)
				{
					_strFileData += "		protected	DatabaseManager				_dbm	= null;\n";
					_strFileData += "		protected	DatabaseManager				Database\n		{\n";
					_strFileData += "			get {\n";
					_strFileData += "				if (_dbm == null)		_dbm = DatabaseManager.Instance;\n";
					_strFileData += "					return _dbm;\n			}\n";
					_strFileData += "		}\n";
				} else if (UseDatabase) {
					_strFileData += "		private	ClsDAL				_DAL				= null;\n";
					_strFileData += "		private string				_DBserver		= \"" + DBserver.Replace("\\", "\\\\") + "\";\n";
					_strFileData += "		private string				_DBdatabase	= \"" + DBdatabase + "\";\n";
					if (!DBuseWinAccount)
					{ 
					_strFileData += "		private string				_DBuser			= \"" + DBuser + "\";\n";
					_strFileData += "		private string				_DBpassword	= \"" + DBpassword + "\";\n";
					}
					_strFileData += "		protected ClsDAL			DAL\n		{\n";
					_strFileData += "			get {\n";
					_strFileData += "				if (_DAL == null)\n					_DAL = new ClsDAL();\n";
					_strFileData += "				if (_DAL != null && !_DAL.IsConnected)\n";
					if (DBuseWinAccount)
					_strFileData += "					_DAL.OpenConnection(_DBserver, _DBdatabase);\n";
					else
					_strFileData += "					_DAL.OpenConnection(_DBserver, _DBdatabase, _DBuser, _DBpassword);\n";
					_strFileData += "				return _DAL;\n			}\n";
					_strFileData += "		}\n";
				}
				_strFileData += "\n	#endregion\n\n";

				// BUILD PUBLIC PROPERTY REGION			-----------------------------------------------
				_strFileData += "	#region \"PUBLIC PROPERTIES\"\n\n";
				if (IsANetworkObject)
				{
					_strFileData += "		public	int		NetID\n		{\n";
					_strFileData += "			get { try { return (int) this.netId.Value; } catch { return 0; } }\n";
					_strFileData += "		}\n";

					_strFileData += "		public	NetworkConnection	NetConnection\n		{\n";
					_strFileData += "			get { return _netConn; }\n";
					_strFileData += "			set { _netConn = value; }\n";
					_strFileData += "		}\n";
				}
				for (int i = 0; i < Variables.Count; i++)
				{
					if (!Variables[i].IsDeleted)
					{ 
						if (Variables[i].IsIndex)
						{ 
							_strFileData += "		// INDEX VARIABLE\n";
							_strFileData		+= "		public	" + GetPropertyType(Variables[i]) + "		ID\n		{\n";
							_strFileData		+= "			get { return " + Variables[i].GetPrivatePrefix + Variables[i].Name + "; }\n";
							_strFileData		+= "		}\n";
							_strFileData += "		// INDEX VARIABLE\n";
						}
						_strFileData		+= "		public	" + GetPropertyType(Variables[i]) + "		" + Variables[i].Name + "\n		{\n";
						_strFileData		+= "			get { return " + Variables[i].GetPrivatePrefix + Variables[i].Name + "; }\n";
						if (!Variables[i].IsIndex)
							_strFileData	+= "			set { " + Variables[i].GetPrivatePrefix + Variables[i].Name + " = value; }\n";
						_strFileData		+= "		}\n";


						if (Variables[i].VarType.ToLower().StartsWith("enum"))
						{
							_strFileData	+= "		public	int		" + Variables[i].Name + "Int\n		{\n";
							_strFileData	+= "			get { return (int) " + Variables[i].GetPrivatePrefix + Variables[i].Name + "; }\n";
							_strFileData	+= "			set { " + Variables[i].GetPrivatePrefix + Variables[i].Name + " = (" + Variables[i].VarType.Substring(4) + ") value; }\n";
							_strFileData	+= "		}\n";
							_strFileData	+= "		// ENUM DECLARATION\n";
							_strFileData	+= "		public	enum	" + Variables[i].VarType.Substring(4) + " : int	{ ";
							eb = dbEnums.GetByName(Variables[i].VarType.Substring(4));
							for (int e = 0; e < eb.Variables.Count; e++)
								_strFileData += eb.Variables[e].Name + "=" + e.ToString() + ", ";
							_strFileData = _strFileData.Substring(0, _strFileData.Length - 2);
							_strFileData	+= " };\n";
						}

					}
				}
				_strFileData += "\n	#endregion\n\n";

				// BUILD PUBLIC SEARCH FUNCTIONS REGION			-----------------------------------------------
				_strFileData += "	#region \"PUBLIC SEARCH FUNCTIONS\"\n\n";
				if (IsANetworkObject)
				{
					_strFileData		+= "		public	static	" + this.ClassName + "Base		FindByNetID(int intFind)\n		{\n";
					_strFileData		+= "			if (" + this.ClassName + "Manager.Instance != null)\n";
					_strFileData	+= "				return " + this.ClassName + "Manager.Instance." + this.ClassName + "s.Find(x => x.NetID == intFind);\n";
					_strFileData		+= "			else\n				return null;\n";
					_strFileData		+= "		}\n";
				}
				for (int i = 0; i < Variables.Count; i++)
				{
					if (!Variables[i].IsDeleted)
					{
						if (UseClassMgr && UseDatabase)
						{
							if (Variables[i].IsIndex)
							{
								_strFileData		+= "		public	static	" + this.ClassName + "Base		FindByID(" + GetPropertyType(Variables[i]) + " " + Variables[i].GetPrivatePrefix + "Find)\n		{\n";
								_strFileData		+= "			if (" + this.ClassName + "Manager.Instance != null)\n";
								if (Variables[i].VarType.ToLower() == "string")
									_strFileData	+= "				return " + this.ClassName + "Manager.Instance." + this.ClassName + "s.Find(x => x." + Variables[i].Name + ".ToLower() == " + Variables[i].GetPrivatePrefix + "Find.ToLower());\n";
								else
									_strFileData	+= "				return " + this.ClassName + "Manager.Instance." + this.ClassName + "s.Find(x => x." + Variables[i].Name + " == " + Variables[i].GetPrivatePrefix + "Find);\n";
								_strFileData		+= "			else\n				return null;\n";
								_strFileData		+= "		}\n";
							}
							if (Variables[i].IsIndex || Variables[i].IsSearchable)
							{
								_strFileData		+= "		public	static	" + this.ClassName + "Base		FindBy" + Variables[i].Name + "(" + GetPropertyType(Variables[i]) + " " + Variables[i].GetPrivatePrefix + "Find)\n		{\n";
								_strFileData		+= "			if (" + this.ClassName + "Manager.Instance != null)\n";
								if (Variables[i].VarType.ToLower() == "string")
									_strFileData	+= "				return " + this.ClassName + "Manager.Instance." + this.ClassName + "s.Find(x => x." + Variables[i].Name + ".ToLower() == " + Variables[i].GetPrivatePrefix + "Find.ToLower());\n";
								else
									_strFileData	+= "				return " + this.ClassName + "Manager.Instance." + this.ClassName + "s.Find(x => x." + Variables[i].Name + " == " + Variables[i].GetPrivatePrefix + "Find);\n";
								_strFileData		+= "			else\n				return null;\n";
								_strFileData		+= "		}\n";
							}
						} else {
							if (Variables[i].IsIndex)
							{
								_strFileData		+= "		public	static	" + this.ClassName + "Base		FindByID(List<" + this.ClassName + "Base> lib, " + GetPropertyType(Variables[i]) + " " + Variables[i].GetPrivatePrefix + "Find)\n		{\n";
								_strFileData		+= "			if (" + this.ClassName + "Manager.Instance != null)\n";
								if (Variables[i].VarType.ToLower() == "string")
									_strFileData	+= "				return lib.Find(x => x." + Variables[i].Name + ".ToLower() == " + Variables[i].GetPrivatePrefix + "Find.ToLower());\n";
								else
									_strFileData	+= "				return lib.Find(x => x." + Variables[i].Name + " == " + Variables[i].GetPrivatePrefix + "Find);\n";
								_strFileData		+= "			else\n				return null;\n";
								_strFileData		+= "		}\n";
							}
							if (Variables[i].IsIndex || Variables[i].IsSearchable)
							{
								_strFileData		+= "		public	static	" + this.ClassName + "Base		FindBy" + Variables[i].Name + "(List<" + this.ClassName + "Base> lib, " + GetPropertyType(Variables[i]) + " " + Variables[i].GetPrivatePrefix + "Find)\n		{\n";
								_strFileData		+= "			if (" + this.ClassName + "Manager.Instance != null)\n";
								if (Variables[i].VarType.ToLower() == "string")
									_strFileData	+= "				return lib.Find(x => x." + Variables[i].Name + ".ToLower() == " + Variables[i].GetPrivatePrefix + "Find.ToLower());\n";
								else
									_strFileData	+= "				return lib.Find(x => x." + Variables[i].Name + " == " + Variables[i].GetPrivatePrefix + "Find);\n";
								_strFileData		+= "			else\n				return null;\n";
								_strFileData		+= "		}\n";
							}
						}
					}
				}
				_strFileData += "\n	#endregion\n\n";

				// BUILD PRIVATE FUNCTIONS REGION		-----------------------------------------------
				_strFileData += "	#region \"PRIVATE FUNCTIONS\"\n\n";
				_strFileData += "\n	#endregion\n\n";

				// BUILD PUBLIC FUNCTIONS REGION		-----------------------------------------------
				_strFileData += "	#region \"PUBLIC FUNCTIONS\"\n\n";
				_strFileData += "		public	void			Clone(" + ClassName + "Base c)\n		{\n";
				for (int i = 0; i < Variables.Count; i++)
				{
					if (!Variables[i].IsDeleted)
					{
						_strFileData += "			" + Variables[i].GetPrivatePrefix + Variables[i].Name + "	= c." + Variables[i].Name + ";\n";
					}
				}
				_strFileData += "		}\n";
				_strFileData += "		public	void			Reset()\n		{\n";

				for (int i = 0; i < Variables.Count; i++)
				{
					if (!Variables[i].IsDeleted)
					{ 
						_strFileData += "			" + Variables[i].GetPrivatePrefix + Variables[i].Name;
						if (Variables[i].VarType.ToLower() == "vector2")
							_strFileData += "	= Vector2.zero";
						else if (Variables[i].VarType.ToLower() == "vector3")
							_strFileData += "	= Vector3.zero";
						else if (Variables[i].VarType.ToLower() == "quaternion")
							_strFileData += "	= Quaternion.identity";
						else if ((Variables[i].VarType.ToLower() == "date" ||
											Variables[i].VarType.ToLower() == "datetime"))
						{
							if (Variables[i].StartingValue == "")
								_strFileData += " = System.DateTime.Parse(\"01/01/1900\")";
							else
								_strFileData += " = System.DateTime.Parse(\"" + Variables[i].StartingValue + "\")";
						}
						else if (Variables[i].VarType.ToLower() == "string")
							_strFileData += "	=	\"" + Variables[i].StartingValue + "\"";
						else if (Variables[i].VarType.ToLower() == "float")
							_strFileData += "	=	" + Variables[i].StartingValue + "f";
						else if (Variables[i].VarType.ToLower().StartsWith("enum"))
							_strFileData += " = (" + Variables[i].VarType.Substring(4) + ") " + Util.ConvertToInt(Variables[i].StartingValue).ToString();
						else
							_strFileData += "	=	" + Variables[i].StartingValue;
						_strFileData += ";\n";
					}
				}
				_strFileData += "		}\n";
				_strFileData += "\n	#endregion\n\n";

				// BUILD DATABASE REGION		-------------------------------------------------------
				if (UseDatabase)
				{ 
					_strFileData += "	#region \"DATABASE FUNCTIONS\"\n\n";

					// BUILD LOADING FUNCTIONS
					if (UseDBload)
					{ 
						_strFileData += "		public	virtual	void		PopulateClass(DataRow dr)\n		{\n";
						for (int i = 0; i < Variables.Count; i++)
						{
							if (!Variables[i].IsDeleted)
								_strFileData += "			" + Variables[i].GetPrivatePrefix + Variables[i].Name + " = " + Variables[i].GetConversionFromDataRow + ";\n";
						}
						_strFileData += "		}\n";

						// BUILD LOAD ALL RECORDS FUNCTION  (RETURNS A LIST)
						if (!UseClassMgr)
						{ 
							string strDAL = DatabaseCall.Substring(0, DatabaseCall.Length - 1);
							_strFileData += "		public	static		List<" + this.ClassName + ">	LoadAll(bool blnActiveOnly = true)\n		{\n";
							if (UseDBmgr)
							{
								strDAL = "_" + strDAL;
								_strFileData += "			DatabaseManager _Database = DatabaseManager.Instance;\n";
								strDAL += ".";
							} else { 
								strDAL = "_" + strDAL;
								_strFileData += "			ClsDAL	" + strDAL + " = new ClsDAL();\n";
								strDAL += ".";
								if (!DBuseWinAccount)
									_strFileData += "			" + strDAL + "OpenConnection(\"" + DBserver + "\", \"" + DBdatabase + "\", \"" + DBuser + "\", \"" + DBpassword + "\");\n";
								else
									_strFileData += "			" + strDAL + "OpenConnection(\"" + DBserver + "\", \"" + DBdatabase + "\");\n";
							}
							_strFileData += "			if (" + strDAL + "IsConnected)\n";
							_strFileData += "			{\n";
							_strFileData += "				// LOAD FROM THE DATABASE TABLE\n";
							_strFileData += "				" + strDAL + "ClearParams();\n";
							_strFileData += "				" + strDAL + "AddParam(\"ID\",					0);\n";
							_strFileData += "				" + strDAL + "AddParam(\"ACTIVEONLY\",	blnActiveOnly);\n";
							_strFileData += "				DataTable dt = " + strDAL + "GetSPDataTable(\"spGet" + this.ClassName + "ByID\");\n";
							_strFileData += "				List<" + this.ClassName + "> lib = new List<" + this.ClassName + ">();\n";
							_strFileData += "				if (dt != null && dt.Rows.Count > 0)\n				{\n";
							_strFileData += "					foreach (DataRow dr in dt.Rows)\n					{\n";
							_strFileData += "						" + this.ClassName + "	temp = new " + this.ClassName + "();\n";
							_strFileData += "						temp.PopulateClass(dr);\n";
							_strFileData += "						lib.Add(temp);\n";
							_strFileData += "					}\n";
							_strFileData += "				}\n";
							if (!UseDBmgr)
							{
								_strFileData += "				" + strDAL + "CloseConnection();\n";
								strDAL = strDAL.Substring(0, strDAL.Length - 1);
								_strFileData += "				" + strDAL + " = null;\n";
							}
							_strFileData += "				return lib;\n";
							_strFileData += "			}\n";
							_strFileData += "			return null;\n";
							_strFileData += "		}\n";
						}

						// BUILD LOAD ONE RECORD (BY ID) FUNCTION
						_strFileData += "		public	virtual		bool	LoadByID(int intID, bool blnActiveOnly = false)\n		{\n";
						_strFileData += "			if (intID < 1)\n";
						_strFileData += "					return false;\n\n";
						_strFileData += "			if (" + DatabaseCall + "IsConnected)\n";
						_strFileData += "			{\n";
						_strFileData += "				// LOAD FROM THE DATABASE TABLE\n";
						_strFileData += "				" + DatabaseCall + "ClearParams();\n";
						_strFileData += "				" + DatabaseCall + "AddParam(\"ID\",					intID);\n";
						_strFileData += "				" + DatabaseCall + "AddParam(\"ACTIVEONLY\",	blnActiveOnly);\n";
						_strFileData += "				DataTable dt = " + DatabaseCall + "GetSPDataTable(\"spGet" + this.ClassName + "ByID\");\n";
						_strFileData += "				if (dt != null && dt.Rows.Count > 0)\n				{\n";
						_strFileData += "					PopulateClass(dt.Rows[0]);\n";
						_strFileData += "					return true;\n";
						_strFileData += "				}\n";
						_strFileData += "			}\n";
						_strFileData += "			return false;\n";
						_strFileData += "		}\n";

						// BUILD FIND BY PROPERTY FUNCTIONS
						for (int i = 0; i < Variables.Count; i++)
						{
							if (!Variables[i].IsDeleted && Variables[i].IsSearchable)
							{
								_strFileData += "		public	virtual		bool	LoadBy" + Variables[i].Name + "(" + GetPropertyType(Variables[i]) + " xFind, bool blnActiveOnly = true)\n		{\n";
								_strFileData += "			if (" + DatabaseCall + "IsConnected)\n";
								_strFileData += "			{\n";
								_strFileData += "				// LOAD FROM THE DATABASE TABLE\n";
								_strFileData += "				" + DatabaseCall + "ClearParams();\n";
								if (Variables[i].VarType.ToLower().StartsWith("enum"))
								_strFileData += "				" + DatabaseCall + "AddParam(\"FIND\",				((int)xFind));\n";
								else
								_strFileData += "				" + DatabaseCall + "AddParam(\"FIND\",				xFind);\n";
								_strFileData += "				" + DatabaseCall + "AddParam(\"ACTIVEONLY\",	blnActiveOnly);\n";
								_strFileData += "				DataTable dt = " + DatabaseCall + "GetSPDataTable(\"spGet" + this.ClassName + "By" + Variables[i].Name + "\");\n";
								_strFileData += "				if (dt != null && dt.Rows.Count > 0)\n				{\n";
								_strFileData += "					PopulateClass(dt.Rows[0]);\n";
								_strFileData += "					return true;\n";
								_strFileData += "				}\n";
								_strFileData += "			}\n";
								_strFileData += "			return false;\n";
								_strFileData += "		}\n";
							}
						}
					}
				
					// BUILD SAVING FUNCTIONS
					if (UseDBsave)
					{ 
						_strFileData += "		public	virtual		void	Save()\n		{\n";
						_strFileData += "			if (" + DatabaseCall + "IsConnected)\n";
						_strFileData += "			{\n";
						_strFileData += "				// UPDATE TO THE DATABASE TABLE\n";
						_strFileData += "				_dtDateUpdated = System.DateTime.Now;\n";
						_strFileData += "				" + DatabaseCall + "ClearParams();\n";
						for (int i = 0; i < Variables.Count; i++)
						{
							if (!Variables[i].IsDeleted &&
									Variables[i].Name.ToLower() != "datecreated" && 
									Variables[i].Name.ToLower() != "dateupdated")
								_strFileData += "				" + DatabaseCall + "AddParam(\"" + Variables[i].Name.ToUpper() + "\", " + Variables[i].GetConversionToParam + ");\n";
						}
						_strFileData += "				" + DatabaseCall + "AddParam(\"NEWID\", DbType.Int32);\n";
						_strFileData += "				int n = " + DatabaseCall + "GetSPInt(\"spUpdate" + this.ClassName + "\");\n";
						string st = GetIndexPrivateVariable;
						if (st != "")
						{
							_strFileData += "				if (" + st + " != n)\n";
							_strFileData += "					" + st + " = n;\n";
						}
						_strFileData += "			}\n";
						_strFileData += "		}\n";
					}
					_strFileData += "\n	#endregion\n\n";
				}

				// BUILD SERIALIZATION REGION		---------------------------------------------------
				if (_blnUseSerialization)
				{ 
					_strFileData += "	#region \"SERIALIZATION FUNCTIONS\"\n\n";
					_strFileData += "		public	virtual	string					Serialize()\n		{\n";
					_strFileData += "			string strOut = \"\";\n";
					for (int i = 0; i < Variables.Count; i++)
					{
						if (!Variables[i].IsDeleted)
							if (Variables[i].VarType.ToLower() == "date" || Variables[i].VarType.ToLower() == "datetime")
								_strFileData += "			strOut += " + Variables[i].Name + ".ToString(\"MM/dd/yyyy HH:mm:ss\") + \"|\";\n";
							else if (Variables[i].VarType.ToLower().StartsWith("enum"))
								_strFileData += "			strOut += " + Variables[i].Name + "Int.ToString() + \"|\";\n";
							else
								_strFileData += "			strOut += " + Variables[i].Name + ".ToString() + \"|\";\n";
					}
					_strFileData += "			strOut = strOut.Substring(0, strOut.Length - 1);\n";
					_strFileData += "			return strOut;\n";
					_strFileData += "		}\n";
			
					_strFileData += "		public	virtual	void						Deserialize(string strText)\n		{\n";
					_strFileData += "			string[] strSpl = strText.Split('|');\n";
					int x = 0;
					for (int i = 0; i < Variables.Count; i++)
					{
						if (!Variables[i].IsDeleted)
						{
							if (Variables[i].VarType.ToLower().StartsWith("enum"))
								_strFileData += "			" + Variables[i].Name + "Int = Util.ConvertToInt(strSpl[" + x.ToString() + "]);\n";
							else
								switch (Variables[i].VarType.ToLower())
								{
									case "string": 
										_strFileData += "			" + Variables[i].GetPrivatePrefix + Variables[i].Name + " = strSpl[" + x.ToString() + "];\n";
										break;
									case "int":
										_strFileData += "			" + Variables[i].GetPrivatePrefix + Variables[i].Name + " = Util.ConvertToInt(strSpl[" + x.ToString() + "]);\n";
										break;
									case "float":
										_strFileData += "			" + Variables[i].GetPrivatePrefix + Variables[i].Name + " = Util.ConvertToFloat(strSpl[" + x.ToString() + "]);\n";
										break;
									case "bool":
										_strFileData += "			" + Variables[i].GetPrivatePrefix + Variables[i].Name + " = Util.ConvertToBoolean(strSpl[" + x.ToString() + "]);\n";
										break;
									case "date":
									case "datetime":
										_strFileData += "			" + Variables[i].GetPrivatePrefix + Variables[i].Name + " = Util.ConvertToDate(strSpl[" + x.ToString() + "]);\n";
										break;
									case "vector2":
										_strFileData += "			" + Variables[i].GetPrivatePrefix + Variables[i].Name + " = Util.ConvertToVector2(strSpl[" + x.ToString() + "]);\n";
										break;
									case "vector3":
										_strFileData += "			" + Variables[i].GetPrivatePrefix + Variables[i].Name + " = Util.ConvertToVector3(strSpl[" + x.ToString() + "]);\n";
										break;
									case "quaternion":
										_strFileData += "			" + Variables[i].GetPrivatePrefix + Variables[i].Name + " = Util.ConvertToQuaternion(strSpl[" + x.ToString() + "]);\n";
										break;
								}
							x++;
						}
					}
					_strFileData += "		}\n";
					_strFileData += "\n	#endregion\n\n";
				}

				// BUILD NETWORK FUNCTIONS REGION		-----------------------------------------------
				if (IsANetworkObject)
				{ 
					_strFileData += "	#region \"NETWORK FUNCTIONS\"\n\n";
					_strFileData += "\n	#endregion\n\n";
				}

				_strFileData += "}\n\n";		// END CLASS

				if (Namespace != "")
					_strFileData += "}\n\n";	// END NAMESPACE

				// WRITE THE FILE
				strFileName += ".cs";
				Util.WriteTextFile(CLASS_SCRIPT_DIRECTORY, strFileName, _strFileData);
			}
			private void				CreateCSclassFile()
			{
				string strFileName = this.ClassName + ".cs";
				_strFileData = "";

				// COMMENTED HEADER
				_strFileData += "//		AUTO-GENERATED FILE: " + strFileName + "\n";
				_strFileData += "//		GENERATED ON       : " + System.DateTime.Now.ToString("ddd MMM dd yyyy - hh:mm:ss tt") + "\n";
				_strFileData += "//		\n";
				_strFileData += "//		This is the Class file.  This is the file that you can modify.\n";
				_strFileData += "//		It will not automatically be changed by the system going forward.\n";
				_strFileData += "\n\n";

				if (UseUnity)
					_strFileData	+= "using UnityEngine;\n";
				if (UseUnity && UseUnityUI)
					_strFileData	+= "using UnityEngine.UI;\n";
				if (IsANetworkObject)
					_strFileData	+= "using UnityEngine.Networking;\n";
				_strFileData		+= "using System.Collections;\n";
				_strFileData		+= "using System.Collections.Generic;\n";
				if (UseDatabase)
					_strFileData	+= "using System.Data;\n";
				_strFileData		+= "\n";

				if (Namespace		!= "")
					_strFileData	+= "namespace " + this.Namespace + "\n{\n\n";
				_strFileData		+= "public class " + this.ClassName + " : " + this.ClassName + "Base\n{\n\n";

				// BUILD PRIVATE VARIABLE REGION		-----------------------------------------------
				_strFileData += "	#region \"PRIVATE VARIABLES\"\n\n";
				_strFileData += "\n	#endregion\n\n";

				// BUILD PRIVATE PROPERTY REGION		-----------------------------------------------
				_strFileData += "	#region \"PRIVATE PROPERTIES\"\n\n";
				_strFileData += "\n	#endregion\n\n";

				// BUILD PUBLIC PROPERTY REGION			-----------------------------------------------
				_strFileData += "	#region \"PUBLIC PROPERTIES\"\n\n";
				_strFileData += "\n	#endregion\n\n";

				// BUILD PUBLIC SEARCH FUNCTIONS REGION			---------------------------------------
				_strFileData += "	#region \"PUBLIC SEARCH FUNCTIONS\"\n\n";
				if (IsANetworkObject)
				{
					_strFileData		+= "		public	static	" + this.ClassName + "		FindByNetID(int intFind)\n		{\n";
					_strFileData		+= "			if (" + this.ClassName + "Manager.Instance != null)\n";
					_strFileData	+= "				return " + this.ClassName + "Manager.Instance." + this.ClassName + "s.Find(x => x.NetID == intFind);\n";
					_strFileData		+= "			else\n				return null;\n";
					_strFileData		+= "		}\n";
				}
				for (int i = 0; i < Variables.Count; i++)
				{
					if (!Variables[i].IsDeleted)
					{
						if (UseClassMgr && UseDatabase)
						{
							if (Variables[i].IsIndex)
							{
								_strFileData		+= "		public	static	" + this.ClassName + "		FindByID(" + GetPropertyType(Variables[i]) + " " + Variables[i].GetPrivatePrefix + "Find)\n		{\n";
								_strFileData		+= "			if (" + this.ClassName + "Manager.Instance != null)\n";
								if (Variables[i].VarType.ToLower() == "string")
									_strFileData	+= "				return " + this.ClassName + "Manager.Instance." + this.ClassName + "s.Find(x => x." + Variables[i].Name + ".ToLower() == " + Variables[i].GetPrivatePrefix + "Find.ToLower());\n";
								else
									_strFileData	+= "				return " + this.ClassName + "Manager.Instance." + this.ClassName + "s.Find(x => x." + Variables[i].Name + " == " + Variables[i].GetPrivatePrefix + "Find);\n";
								_strFileData		+= "			else\n				return null;\n";
								_strFileData		+= "		}\n";
							}
							if (Variables[i].IsIndex || Variables[i].IsSearchable)
							{
								_strFileData		+= "		public	static	" + this.ClassName + "		FindBy" + Variables[i].Name + "(" + GetPropertyType(Variables[i]) + " " + Variables[i].GetPrivatePrefix + "Find)\n		{\n";
								_strFileData		+= "			if (" + this.ClassName + "Manager.Instance != null)\n";
								if (Variables[i].VarType.ToLower() == "string")
									_strFileData	+= "				return " + this.ClassName + "Manager.Instance." + this.ClassName + "s.Find(x => x." + Variables[i].Name + ".ToLower() == " + Variables[i].GetPrivatePrefix + "Find.ToLower());\n";
								else
									_strFileData	+= "				return " + this.ClassName + "Manager.Instance." + this.ClassName + "s.Find(x => x." + Variables[i].Name + " == " + Variables[i].GetPrivatePrefix + "Find);\n";
								_strFileData		+= "		}\n";
								_strFileData		+= "			else\n				return null;\n";
							}
						} else {
							if (Variables[i].IsIndex)
							{
								_strFileData		+= "		public	static	" + this.ClassName + "		FindByID(List<" + this.ClassName + "> lib, " + GetPropertyType(Variables[i]) + " " + Variables[i].GetPrivatePrefix + "Find)\n		{\n";
								_strFileData		+= "			if (" + this.ClassName + "Manager.Instance != null)\n";
								if (Variables[i].VarType.ToLower() == "string")
									_strFileData	+= "				return lib.Find(x => x." + Variables[i].Name + ".ToLower() == " + Variables[i].GetPrivatePrefix + "Find.ToLower());\n";
								else
									_strFileData	+= "				return lib.Find(x => x." + Variables[i].Name + " == " + Variables[i].GetPrivatePrefix + "Find);\n";
								_strFileData		+= "			else\n				return null;\n";
								_strFileData		+= "		}\n";
							}
							if (Variables[i].IsIndex || Variables[i].IsSearchable)
							{
								_strFileData		+= "		public	static	" + this.ClassName + "		FindBy" + Variables[i].Name + "(List<" + this.ClassName + "> lib, " + GetPropertyType(Variables[i]) + " " + Variables[i].GetPrivatePrefix + "Find)\n		{\n";
								_strFileData		+= "			if (" + this.ClassName + "Manager.Instance != null)\n";
								if (Variables[i].VarType.ToLower() == "string")
									_strFileData	+= "				return lib.Find(x => x." + Variables[i].Name + ".ToLower() == " + Variables[i].GetPrivatePrefix + "Find.ToLower());\n";
								else
									_strFileData	+= "				return lib.Find(x => x." + Variables[i].Name + " == " + Variables[i].GetPrivatePrefix + "Find);\n";
								_strFileData		+= "			else\n				return null;\n";
								_strFileData		+= "		}\n";
							}
						}
					}
				}				
				_strFileData += "\n	#endregion\n\n";

				// BUILD PRIVATE FUNCTIONS REGION		-----------------------------------------------
				_strFileData += "	#region \"PRIVATE FUNCTIONS\"\n\n";
				_strFileData += "\n	#endregion\n\n";

				// BUILD PUBLIC FUNCTIONS REGION		-----------------------------------------------
				_strFileData += "	#region \"PUBLIC FUNCTIONS\"\n\n";
				_strFileData += "\n	#endregion\n\n";

				// BUILD DATABASE REGION		-------------------------------------------------------
				if (UseDatabase)
				{ 
					_strFileData += "	#region \"DATABASE FUNCTIONS\"\n\n";
					_strFileData += "		public	override	void	PopulateClass(DataRow dr)\n		{\n";
					_strFileData += "			// YOU CAN COMMENT THIS LINE OUT, AND BUILD YOUR OWN CUSTOM POPULATE CLASS FUNCTION\n";
					_strFileData += "			base.PopulateClass(dr);\n";
					_strFileData += "		}\n";

					if (UseDBload)
					{ 
						_strFileData += "		public	override	bool	LoadByID(int intID, bool blnActiveOnly = false)\n		{\n";
						_strFileData += "			// YOU CAN COMMENT THIS LINE OUT, AND BUILD YOUR OWN CUSTOM LOADBYID FUNCTION\n";
						_strFileData += "			return base.LoadByID(intID, blnActiveOnly);\n";
						_strFileData += "		}\n";

						for (int i = 0; i < Variables.Count; i++)
						{
							if (!Variables[i].IsDeleted && Variables[i].IsSearchable)
							{
								_strFileData += "		public	override	bool	LoadBy" + Variables[i].Name + "(" + GetPropertyType(Variables[i]) + " xFind, bool blnActiveOnly = true)\n		{\n";
								_strFileData += "			// YOU CAN COMMENT THIS LINE OUT, AND BUILD YOUR OWN CUSTOM LoadBy" + Variables[i].Name + " FUNCTION\n";
								_strFileData += "			return base.LoadBy" + Variables[i].Name + "(xFind, blnActiveOnly);\n";
								_strFileData += "		}\n";
							}
						}
					}

					if (UseDBsave)
					{ 
						_strFileData += "		public	override	void	Save()\n		{\n";
						_strFileData += "			// YOU CAN COMMENT THIS LINE OUT, AND BUILD YOUR OWN CUSTOM SAVE FUNCTION\n";
						_strFileData += "			base.Save();\n";
						_strFileData += "		}\n";
					}

					_strFileData += "\n	#endregion\n\n";
				}
				

				// BUILD SERIALIZATION REGION		---------------------------------------------------
				if (_blnUseSerialization)
				{ 
					_strFileData += "	#region \"SERIALIZATION FUNCTIONS\"\n\n";
					_strFileData += "\n	#endregion\n\n";
				}

				_strFileData += "}\n\n";		// END CLASS

				if (Namespace != "")
					_strFileData += "}\n\n";	// END NAMESPACE

				// WRITE THE FILE
				if (!Util.FileExists(CLASS_SCRIPT_DIRECTORY, strFileName))
						Util.WriteTextFile(CLASS_SCRIPT_DIRECTORY, strFileName, _strFileData);
			}
			private void				CreateCSclassManager()
			{
				if (!UseClassMgr || !UseDatabase)
					return;

				string strClassName	= this.ClassName + "Manager";
				string strFileName	= strClassName + ".cs";
				_strFileData = "";

				// COMMENTED HEADER
				_strFileData += "//		AUTO-GENERATED FILE: " + strFileName + "\n";
				_strFileData += "//		GENERATED ON       : " + System.DateTime.Now.ToString("ddd MMM dd yyyy - hh:mm:ss tt") + "\n";
				_strFileData += "//		\n";
				_strFileData += "//		This is the Class Manager file.  This is the file that you can modify.\n";
				_strFileData += "//		It will not automatically be changed by the system going forward.\n";
				_strFileData += "\n\n";

				if (UseUnity)
					_strFileData	+= "using UnityEngine;\n";
				if (UseUnity && UseUnityUI)
					_strFileData	+= "using UnityEngine.UI;\n";
				_strFileData		+= "using System.Collections;\n";
				_strFileData		+= "using System.Collections.Generic;\n";
				if (UseDatabase)
					_strFileData	+= "using System.Data;\n";
				_strFileData		+= "\n";

				if (Namespace		!= "")
					_strFileData	+= "namespace " + this.Namespace + "\n{\n\n";
				if (UseUnity)
					_strFileData	+= "public class " + strClassName + " : MonoBehaviour \n{\n\n";
				else
					_strFileData	+= "public class " + strClassName + " \n{\n\n";

				// BUILD PRIVATE VARIABLE REGION		-----------------------------------------------
				_strFileData += "	#region \"PRIVATE VARIABLES\"\n\n";
				_strFileData += "		private	static	" + strClassName + "									_instance = null;\n\n";
				_strFileData += "		private bool		_blnInitialized		= false;\n\n";
				_strFileData += "		// _blnLoadAllAtStart:  true=Load all Records from Database into Class, false=Start with Empty List, allow developer to add/remove to List.\n";
				_strFileData += "		[SerializeField]				private bool				_blnLoadAllAtStart	= false;\n";
				_strFileData += "		[System.NonSerialized]	private	List<" + this.ClassName + ">	_" + this.ClassName.ToLower() + "s	= new List<" + this.ClassName + ">();\n";
				_strFileData += "\n	#endregion\n\n";

				// BUILD PRIVATE PROPERTY REGION		-----------------------------------------------
				_strFileData += "	#region \"PRIVATE PROPERTIES\"\n\n";
				if (UseAppMgr)
				{
					_strFileData += "		private	ApplicationManager		_app	= null;\n";
					_strFileData += "		private	ApplicationManager		App\n		{\n";
					_strFileData += "			get {\n";
					_strFileData += "				if (_app == null)		_app = ApplicationManager.Instance;\n";
					_strFileData += "					return _app;\n			}\n";
					_strFileData += "		}\n";
				}
				if (UseNetMgr)
				{
					_strFileData += "		private	AppNetworkManager			_net	= null;\n";
					_strFileData += "		private	AppNetworkManager			Net\n		{\n";
					_strFileData += "			get {\n";
					_strFileData += "				if (_net == null)		_net = AppNetworkManager.Instance;\n";
					_strFileData += "					return _net;\n			}\n";
					_strFileData += "		}\n";
				}
				if (UseDBmgr)
				{
					_strFileData += "		private	DatabaseManager				_dbm	= null;\n";
					_strFileData += "		private	DatabaseManager				Database\n		{\n";
					_strFileData += "			get {\n";
					_strFileData += "				if (_dbm == null)		_dbm = DatabaseManager.Instance;\n";
					_strFileData += "					return _dbm;\n			}\n";
					_strFileData += "		}\n";
				} else if (UseDatabase) {
					_strFileData += "		private	ClsDAL				_DAL				= null;\n";
					_strFileData += "		private string				_DBserver		= \"" + DBserver.Replace("\\", "\\\\") + "\";\n";
					_strFileData += "		private string				_DBdatabase	= \"" + DBdatabase + "\";\n";
					if (!DBuseWinAccount)
					{ 
					_strFileData += "		private string				_DBuser			= \"" + DBuser + "\";\n";
					_strFileData += "		private string				_DBpassword	= \"" + DBpassword + "\";\n";
					}
					_strFileData += "		private ClsDAL				DAL\n		{\n";
					_strFileData += "			get {\n";
					_strFileData += "				if (_DAL == null)\n					_DAL = new ClsDAL();\n";
					_strFileData += "				if (_DAL != null && !_DAL.IsConnectedCheck)\n";
					if (DBuseWinAccount)
					_strFileData += "					_DAL.OpenConnection(_DBserver, _DBdatabase);\n";
					else
					_strFileData += "					_DAL.OpenConnection(_DBserver, _DBdatabase, _DBuser, _DBpassword);\n";
					_strFileData += "				return _DAL;\n			}\n";
					_strFileData += "		}\n";
				}
				_strFileData += "\n	#endregion\n\n";

				// BUILD PUBLIC PROPERTY REGION			-----------------------------------------------
				_strFileData += "	#region \"PUBLIC PROPERTIES\"\n\n";
				_strFileData += "		public	static		" + strClassName + "		Instance\n";
				_strFileData += "		{\n";
				_strFileData += "			get\n";
				_strFileData += "			{\n";
				_strFileData += "				return GetInstance();\n";
				_strFileData += "			}\n";
				_strFileData += "		}\n";
				_strFileData += "		public	static		" + strClassName + "		GetInstance()\n";
				_strFileData += "		{\n";
				_strFileData += "			if (_instance == null)\n";
				_strFileData += "					_instance = (" + strClassName + ")GameObject.FindObjectOfType(typeof(" + strClassName + "));\n";
				_strFileData += "			return _instance;\n";
				_strFileData += "		}\n\n";
				_strFileData += "		public						List<" + this.ClassName + ">		" + this.ClassName + "s\n";
				_strFileData += "		{\n";
				_strFileData += "			get\n";
				_strFileData += "			{\n";
				_strFileData += "				if (_" + this.ClassName.ToLower() + "s == null)\n";
				_strFileData += "						_" + this.ClassName.ToLower() + "s = new List<" + this.ClassName + ">();\n";
				_strFileData += "				return _" + this.ClassName.ToLower() + "s;\n";
				_strFileData += "			}\n";
				_strFileData += "		}\n";
				_strFileData += "\n	#endregion\n\n";

				// BUILD PRIVATE FUNCTIONS REGION		-----------------------------------------------
				_strFileData += "	#region \"PRIVATE FUNCTIONS\"\n\n";
				_strFileData += "		private	void				Start()\n		{\n";
				_strFileData += "			_" + this.ClassName.ToLower() + "s = new List<" + this.ClassName + ">();\n";
				if (UseDatabase)
				{
				_strFileData += "			StartCoroutine(DoStart());\n";
				_strFileData += "		}\n";
				_strFileData += "		private void				OnEnable()\n		{\n";
				_strFileData += "			if (!_blnInitialized)\n";
				_strFileData += "				StartCoroutine(DoStart());\n";
				_strFileData += "		}\n";
				_strFileData += "		private IEnumerator	DoStart()\n		{\n";
				_strFileData += "			yield return new WaitForSeconds(1.0f);\n";
				_strFileData += "			if (!_blnLoadAllAtStart)\n			{\n";
				_strFileData += "				_blnInitialized = true;\n";
				_strFileData += "				yield break;\n";
				_strFileData += "			}\n";
				_strFileData += "			Util.Timer	clock = new Util.Timer();\n";
				_strFileData += "			clock.StartTimer();\n";
				_strFileData += "			int i = 0;\n";
				_strFileData += "			while (!" + ((UseDBmgr) ? "Database" : "DAL") + ".IsConnected && clock.GetTime < 10)\n";
				_strFileData += "			{\n";
				_strFileData += "				i++;\n";
				_strFileData += "				yield return null;\n";
				_strFileData += "			}\n";
				_strFileData += "			clock.StopTimer();\n";
				_strFileData += "			if (clock.GetTime > 10)\n";
				_strFileData += "			{\n";
				_strFileData += "				// DO NOTHING\n";
				_strFileData += "			} else if (" + ((UseDBmgr) ? "Database" : "DAL") + ".IsConnectedCheck)\n";
				_strFileData += "				LoadAll(false);\n";
				_strFileData += "			clock = null;\n";
				_strFileData += "			_blnInitialized = true;\n";
				_strFileData += "		}\n";
				} else {
				_strFileData += "			_blnInitialized = true;\n";
				_strFileData += "		}\n";
				}
				_strFileData += "\n	#endregion\n\n";

				// BUILD PUBLIC FUNCTIONS REGION		-----------------------------------------------
				_strFileData += "	#region \"PUBLIC FUNCTIONS\"\n\n";
				_strFileData += "		public	void						Add(" + this.ClassName + " cls" + this.ClassName.ToLower() + ")\n";
				_strFileData += "		{\n";
				_strFileData += "			if (cls" + this.ClassName.ToLower() + "." + this.ClassName + "ID == 0)\n";
				_strFileData += "				return;\n";
				_strFileData += "			if (_" + this.ClassName.ToLower() + "s.Exists(x => x." + this.ClassName + "ID == cls" + this.ClassName.ToLower() + "." + this.ClassName + "ID))\n";
				_strFileData += "				_" + this.ClassName.ToLower() + "s.Remove(cls" + this.ClassName.ToLower() + ");\n";
				_strFileData += "			_" + this.ClassName.ToLower() + "s.Add(cls" + this.ClassName.ToLower() + ");\n";
				_strFileData += "		}\n";
				_strFileData += "		public	void						Remove(" + this.ClassName + " cls" + this.ClassName.ToLower() + ")\n";
				_strFileData += "		{\n";
				_strFileData += "			_" + this.ClassName.ToLower() + "s.Remove(cls" + this.ClassName.ToLower() + ");\n";
				_strFileData += "		}\n";
				_strFileData += "\n	#endregion\n\n";

				// BUILD PUBLIC SEARCH FUNCTIONS REGION			---------------------------------------
				_strFileData += "	#region \"PUBLIC SEARCH FUNCTIONS\"\n\n";
				for (int i = 0; i < Variables.Count; i++)
				{
					if (!Variables[i].IsDeleted)
					{
						if (Variables[i].IsIndex)
						{
							_strFileData		+= "		public	" + this.ClassName + "		FindByID(" + GetPropertyType(Variables[i], true) + " " + Variables[i].GetPrivatePrefix + "Find)\n		{\n";
							if (Variables[i].VarType.ToLower() == "string")
								_strFileData	+= "			return _" + this.ClassName.ToLower() + "s.Find(x => x." + Variables[i].Name + ".ToLower() == " + Variables[i].GetPrivatePrefix + "Find.ToLower());\n";
							else
								_strFileData	+= "			return _" + this.ClassName.ToLower() + "s.Find(x => x." + Variables[i].Name + " == " + Variables[i].GetPrivatePrefix + "Find);\n";
							_strFileData		+= "		}\n";
						}
						if (Variables[i].IsIndex || Variables[i].IsSearchable)
						{
							_strFileData		+= "		public	" + this.ClassName + "		FindBy" + Variables[i].Name + "(" + GetPropertyType(Variables[i], true) + " " + Variables[i].GetPrivatePrefix + "Find)\n		{\n";
							if (Variables[i].VarType.ToLower() == "string")
								_strFileData	+= "			return _" + this.ClassName.ToLower() + "s.Find(x => x." + Variables[i].Name + ".ToLower() == " + Variables[i].GetPrivatePrefix + "Find.ToLower());\n";
							else
								_strFileData	+= "			return _" + this.ClassName.ToLower() + "s.Find(x => x." + Variables[i].Name + " == " + Variables[i].GetPrivatePrefix + "Find);\n";
							_strFileData		+= "		}\n";
						}
					}
				}
				_strFileData += "\n	#endregion\n\n";

				// BUILD DATABASE REGION		-------------------------------------------------------
				if (UseDatabase)
				{ 
					_strFileData += "	#region \"DATABASE FUNCTIONS\"\n\n";

					if (UseDBload)
					{ 
						// BUILD LOAD ALL RECORDS FUNCTION  (RETURNS A LIST)
						string strDAL = DatabaseCall.Substring(0, DatabaseCall.Length - 1);
						_strFileData += "		public	void	LoadAll(bool blnActiveOnly = true)\n		{\n";
						_strFileData += "			_" + this.ClassName.ToLower() + "s = new List<" + this.ClassName + ">();\n\n";
						if (!UseDBmgr)
						{
							strDAL = "_" + strDAL;
							_strFileData += "			ClsDAL	" + strDAL + " = new ClsDAL();\n";
							strDAL += ".";
							if (!DBuseWinAccount)
								_strFileData += "			" + strDAL + "OpenConnection(\"" + DBserver.Replace("\\", "\\\\") + "\", \"" + DBdatabase + "\", \"" + DBuser + "\", \"" + DBpassword + "\");\n";
							else
								_strFileData += "			" + strDAL + "OpenConnection(\"" + DBserver.Replace("\\", "\\\\") + "\", \"" + DBdatabase + "\");\n";
						} else
							strDAL += ".";

						_strFileData += "			_blnInitialized = false;\n";
						_strFileData += "			if (" + strDAL + "IsConnected)\n";
						_strFileData += "			{\n";
						_strFileData += "				// LOAD FROM THE DATABASE TABLE\n";
						_strFileData += "				" + strDAL + "ClearParams();\n";
						_strFileData += "				" + strDAL + "AddParam(\"ID\",					0);\n";
						_strFileData += "				" + strDAL + "AddParam(\"ACTIVEONLY\",	blnActiveOnly);\n";
						_strFileData += "				DataTable dt = " + strDAL + "GetSPDataTable(\"spGet" + this.ClassName + "ByID\");\n";
						_strFileData += "				if (dt != null && dt.Rows.Count > 0)\n				{\n";
						_strFileData += "					foreach (DataRow dr in dt.Rows)\n					{\n";
						_strFileData += "						" + this.ClassName + "	temp = new " + this.ClassName + "();\n";
						_strFileData += "						temp.PopulateClass(dr);\n";
						_strFileData += "						_" + this.ClassName.ToLower() + "s.Add(temp);\n";
						_strFileData += "					}\n";
						_strFileData += "					_blnInitialized = true;\n";
						_strFileData += "				}\n";
						if (!UseDBmgr)
						{
							_strFileData += "				" + strDAL + "CloseConnection();\n";
							strDAL = strDAL.Substring(0, strDAL.Length - 1);
							_strFileData += "				" + strDAL + " = null;\n";
						}
						_strFileData += "			}\n";
						_strFileData += "		}\n";
					}
					_strFileData += "\n	#endregion\n\n";
				}

				// BUILD SERIALIZATION REGION		---------------------------------------------------
				if (_blnUseSerialization)
				{ 
					_strFileData += "	#region \"SERIALIZATION FUNCTIONS\"\n\n";
					_strFileData += "\n	#endregion\n\n";
				}

				_strFileData += "}\n\n";		// END CLASS

				if (Namespace != "")
					_strFileData += "}\n\n";	// END NAMESPACE

				// WRITE THE FILE
				if (!Util.FileExists(CLASS_SCRIPT_DIRECTORY, strFileName))
						Util.WriteTextFile(CLASS_SCRIPT_DIRECTORY, strFileName, _strFileData);
			}
			private void				CreateSQLfiles()
			{
				if (!UseDatabase)
					return;

				string strFileName = this.ClassName + "_Class_Setup.SQL";
				_strFileData = "";

				// DROP CONSTRAINTS
				BuildDropConstraints();

				// DROP STORED PROCEDURES
				BuildDropStoredProcedures();

				// DROP THE TABLE
				BuildDropTable();

				// ADD THE TABLE
				BuildCreateTable();

				// ADD CONSTRAINTS
				BuildAddConstraints();

				// CREATE THE SQL LOAD/GET SCRIPTS
				BuildLoadingStoredProcedures();

				// CREATE THE SQL UPDATE SCRIPT
				BuildSavingStoredProcedure();

				// WRITE THE FILE
				Util.WriteTextFile(CLASS_SCRIPT_DIRECTORY, strFileName, _strFileData);
			}
			private void				CreateEditorFile(bool blnCreateBase)
			{
				if (!UseUnity || !UseEditor)
					return;

				string strClassName	= this.ClassName;
				if (blnCreateBase)
							 strClassName += "Base";
				string strFileName	= strClassName + "Editor.cs";

				_strFileData = "";
				int intCnt = 0;

				// COMMENTED HEADER
				_strFileData += "//		AUTO-GENERATED FILE: " + strFileName + "\n";
				_strFileData += "//		GENERATED ON       : " + System.DateTime.Now.ToString("ddd MMM dd yyyy - hh:mm:ss tt") + "\n";
				_strFileData += "//		\n";
				if (blnCreateBase)
				{
					_strFileData += "//		This is the Editor for the Base Class file.  It is not intended to be modified.\n";
					_strFileData += "//		If you need to make changes or additions, please modify the " + this.ClassName + "Editor.cs File.\n";
				} else {
					_strFileData += "//		This is the Editor for the Class file.  This is the file that you can modify.\n";
					_strFileData += "//		It will not automatically be changed by the system going forward.\n";
				}
				_strFileData += "\n\n";

				_strFileData += "using UnityEngine;\n";
				_strFileData += "using UnityEditor;\n";
				_strFileData += "using System.Collections;\n\n";
				_strFileData += "[CustomEditor(typeof(" + strClassName + "))]\n";
				_strFileData += "public class " + strClassName + "Editor : Editor\n";
				_strFileData += "{\n\n";
				_strFileData += "	private Vector4[] v4		 = new Vector4[" + Variables.Count.ToString() + "];\n";
				_strFileData += "	private Vector4[] v4Temp = new Vector4[" + Variables.Count.ToString() + "];\n\n";
				_strFileData += "	public override void OnInspectorGUI()\n";
				_strFileData += "	{\n";
				_strFileData += "		" + strClassName + "	myTarget = null;\n";
				_strFileData += "		try { myTarget = (" + strClassName + ")target; } catch { }\n\n";
				_strFileData += "		if (myTarget != null)\n";
				_strFileData += "		{\n";
				_strFileData += "			Quaternion q3 = Quaternion.identity;\n";
				_strFileData += "			GUI.changed = false;\n\n";
				if (IsANetworkObject)
				_strFileData += "			EditorGUILayout.LabelField(\"Net ID\", myTarget.NetID.ToString());\n";

				for (int i = 0; i < Variables.Count; i++)
				{
					if (!Variables[i].IsDeleted)
					{ 
						switch (Variables[i].VarType.ToLower())
						{
							case "string":
								if (Variables[i].IsIndex)
									_strFileData += "			EditorGUILayout.TextField(\"" + Variables[i].Name + "\", myTarget." + Variables[i].Name + ");\n";
								else
									_strFileData += "			myTarget." + Variables[i].Name + "	= EditorGUILayout.TextField(\"" + Variables[i].Name + "\", myTarget." + Variables[i].Name + ");\n";
								break;
							case "int":
								if (Variables[i].IsIndex)
									_strFileData += "			EditorGUILayout.IntField(\"" + Variables[i].Name + "\", myTarget." + Variables[i].Name + ");\n";
								else
									_strFileData += "			myTarget." + Variables[i].Name + "	= EditorGUILayout.IntField(\"" + Variables[i].Name + "\", myTarget." + Variables[i].Name + ");\n";
								break;
							case "bool":
								if (Variables[i].IsIndex)
									_strFileData += "			EditorGUILayout.Toggle(\"" + Variables[i].Name + "\", myTarget." + Variables[i].Name + ");\n";
								else
									_strFileData += "			myTarget." + Variables[i].Name + "	= EditorGUILayout.Toggle(\"" + Variables[i].Name + "\", myTarget." + Variables[i].Name + ");\n";
								break;
							case "float":
								if (Variables[i].IsIndex)
									_strFileData += "			EditorGUILayout.FloatField(\"" + Variables[i].Name + "\", myTarget." + Variables[i].Name + ");\n";
								else
									_strFileData += "			myTarget." + Variables[i].Name + "	= EditorGUILayout.FloatField(\"" + Variables[i].Name + "\", myTarget." + Variables[i].Name + ");\n";
								break;
							case "vector2":
								if (Variables[i].IsIndex)
									_strFileData += "			EditorGUILayout.Vector2Field(\"" + Variables[i].Name + "\", myTarget." + Variables[i].Name + ");\n";
								else
									_strFileData += "			myTarget." + Variables[i].Name + "	= EditorGUILayout.Vector2Field(\"" + Variables[i].Name + "\", myTarget." + Variables[i].Name + ");\n";
								break;
							case "vector3":
								if (Variables[i].IsIndex)
									_strFileData += "			EditorGUILayout.Vector3Field(\"" + Variables[i].Name + "\", myTarget." + Variables[i].Name + ");\n";
								else
									_strFileData += "			myTarget." + Variables[i].Name + "	= EditorGUILayout.Vector3Field(\"" + Variables[i].Name + "\", myTarget." + Variables[i].Name + ");\n";
								break;
							case "quaternion":
								_strFileData += "\n";
								_strFileData += "			if (v4Temp[" + intCnt.ToString() + "]	== null)\n			{\n";
								_strFileData += "				v4Temp[" + intCnt.ToString() + "] = Vector4.zero;\n";
								_strFileData += "				v4Temp[" + intCnt.ToString() + "].x = myTarget." + Variables[i].Name + ".x;\n";
								_strFileData += "				v4Temp[" + intCnt.ToString() + "].y = myTarget." + Variables[i].Name + ".y;\n";
								_strFileData += "				v4Temp[" + intCnt.ToString() + "].z = myTarget." + Variables[i].Name + ".z;\n";
								_strFileData += "				v4Temp[" + intCnt.ToString() + "].w = myTarget." + Variables[i].Name + ".w;\n";
								_strFileData += "			}\n";
								_strFileData += "			if (v4[" + intCnt.ToString() + "]			== null)		v4[" + intCnt.ToString() + "]	= v4Temp[" + intCnt.ToString() + "];\n";
								_strFileData += "			v4[" + intCnt.ToString() + "] = EditorGUILayout.Vector4Field(\"" + Variables[i].Name + "\", v4Temp[" + intCnt.ToString() + "]);\n";
								_strFileData += "			if (v4[" + intCnt.ToString() + "] != v4Temp[" + intCnt.ToString() + "])\n			{\n";
								_strFileData += "				q3 = Quaternion.identity;\n";
								_strFileData += "				q3.x = v4[" + intCnt.ToString() + "].x;\n";
								_strFileData += "				q3.y = v4[" + intCnt.ToString() + "].y;\n";
								_strFileData += "				q3.z = v4[" + intCnt.ToString() + "].z;\n";
								_strFileData += "				q3.w = v4[" + intCnt.ToString() + "].w;\n";
								_strFileData += "				myTarget." + Variables[i].Name + " = q3;\n";
								_strFileData += "				v4Temp[" + intCnt.ToString() + "] = v4[" + intCnt.ToString() + "];\n";
								_strFileData += "			}\n\n";
								intCnt++;
								break;
							case "date":
							case "datetime":
								_strFileData += "			EditorGUILayout.TextField(\"" + Variables[i].Name + "\", myTarget." + Variables[i].Name + ".ToString(\"MM/dd/yyyy HH:mm:ss\"));\n";
								break;
						}
					}
				}
				_strFileData += "\n			if (GUI.changed)\n";
				_strFileData += "				EditorUtility.SetDirty(myTarget);\n";
				_strFileData += "		}\n";
				_strFileData += "	}\n";
				_strFileData += "}\n\n";

				// WRITE THE FILE
				if (!blnCreateBase && !Util.FileExists(CLASS_SCRIPT_DIRECTORY + "/Editor", strFileName))
					Util.WriteTextFile(CLASS_SCRIPT_DIRECTORY + "/Editor", strFileName, _strFileData);
			}
			private void				CreateManagerEditorFile()
			{
				if (!UseUnity || !UseEditor || !UseDatabase)
					return;

				string strClassName	= this.ClassName + "Manager";
				string strFileName	= strClassName + "Editor.cs";

				_strFileData = "";
				int intCnt = 0;

				// COMMENTED HEADER
				_strFileData += "//		AUTO-GENERATED FILE: " + strFileName + "\n";
				_strFileData += "//		GENERATED ON       : " + System.DateTime.Now.ToString("ddd MMM dd yyyy - hh:mm:ss tt") + "\n";
				_strFileData += "//		\n";
					_strFileData += "//		This is the Editor for the Class Manager file.  This is the file that you can modify.\n";
					_strFileData += "//		It will not automatically be changed by the system going forward.\n";
				_strFileData += "\n\n";
				_strFileData += "using UnityEngine;\n";
				_strFileData += "using UnityEditor;\n";
				if (IsANetworkObject)
				_strFileData += "using UnityEngine.Networking;\n";
				_strFileData += "using System.Collections;\n\n";
				_strFileData += "[CustomEditor(typeof(" + strClassName + "))]\n";
				_strFileData += "public class " + strClassName + "Editor : Editor\n";
				_strFileData += "{\n\n";
				_strFileData += "	private int		intSelected = 0;\n";
				_strFileData += "	private bool	blnSave			= false;\n";
				_strFileData += "	private Vector4[] v4			= new Vector4[" + Variables.Count.ToString() + "];\n";
				_strFileData += "	private Vector4[] v4Temp	= new Vector4[" + Variables.Count.ToString() + "];\n\n";
				_strFileData += "	public override void OnInspectorGUI()\n";
				_strFileData += "	{\n";
				_strFileData += "		" + strClassName + "	myTarget = null;\n";
				_strFileData += "		try { myTarget = (" + strClassName + ")target; } catch { }\n\n";
				_strFileData += "		if (myTarget != null)\n";
				_strFileData += "		{\n";
				_strFileData += "			if (myTarget." + this.ClassName + "s.Count > 0)\n			{\n";
				_strFileData += "				" + this.ClassName + " selected	= myTarget." + this.ClassName + "s[intSelected];\n";
				_strFileData += "				Quaternion q3 = Quaternion.identity;\n";
				_strFileData += "				EditorStyles.label.richText = true;\n";
				_strFileData += "				EditorGUILayout.LabelField(\" \", (intSelected + 1).ToString() + \" / \" + myTarget." + this.ClassName + "s.Count.ToString());\n";
				_strFileData += "				EditorStyles.label.richText = false;\n";
				_strFileData += "				EditorGUILayout.Separator();\n";
				_strFileData += "				EditorGUILayout.Space();\n";
				_strFileData += "				GUI.changed = false;\n\n";
				for (int i = 0; i < Variables.Count; i++)
				{
					if (!Variables[i].IsDeleted)
					{ 
						if (Variables[i].VarType.ToLower().StartsWith("enum"))
							_strFileData += "				EditorGUILayout.LabelField(\"" + Variables[i].Name + "\", selected." + Variables[i].Name + ".ToString());\n";
						else
							switch (Variables[i].VarType.ToLower())
							{
								case "string":
									if (Variables[i].IsIndex)
										_strFileData += "				EditorGUILayout.TextField(\"" + Variables[i].Name + "\", selected." + Variables[i].Name + ");\n";
									else
										_strFileData += "				selected." + Variables[i].Name + "	= EditorGUILayout.TextField(\"" + Variables[i].Name + "\", selected." + Variables[i].Name + ");\n";
									break;
								case "int":
									if (Variables[i].IsIndex)
										_strFileData += "				EditorGUILayout.IntField(\"" + Variables[i].Name + "\", selected." + Variables[i].Name + ");\n";
									else
										_strFileData += "				selected." + Variables[i].Name + "	= EditorGUILayout.IntField(\"" + Variables[i].Name + "\", selected." + Variables[i].Name + ");\n";
									break;
								case "bool":
									if (Variables[i].IsIndex)
										_strFileData += "				EditorGUILayout.Toggle(\"" + Variables[i].Name + "\", selected." + Variables[i].Name + ");\n";
									else
										_strFileData += "				selected." + Variables[i].Name + "	= EditorGUILayout.Toggle(\"" + Variables[i].Name + "\", selected." + Variables[i].Name + ");\n";
									break;
								case "float":
									if (Variables[i].IsIndex)
										_strFileData += "				EditorGUILayout.FloatField(\"" + Variables[i].Name + "\", selected." + Variables[i].Name + ");\n";
									else
										_strFileData += "				selected." + Variables[i].Name + "	= EditorGUILayout.FloatField(\"" + Variables[i].Name + "\", selected." + Variables[i].Name + ");\n";
									break;
								case "vector2":
									if (Variables[i].IsIndex)
										_strFileData += "				EditorGUILayout.Vector2Field(\"" + Variables[i].Name + "\", selected." + Variables[i].Name + ");\n";
									else
										_strFileData += "				selected." + Variables[i].Name + "	= EditorGUILayout.Vector2Field(\"" + Variables[i].Name + "\", selected." + Variables[i].Name + ");\n";
									break;
								case "vector3":
									if (Variables[i].IsIndex)
										_strFileData += "				EditorGUILayout.Vector3Field(\"" + Variables[i].Name + "\", selected." + Variables[i].Name + ");\n";
									else
										_strFileData += "				selected." + Variables[i].Name + "	= EditorGUILayout.Vector3Field(\"" + Variables[i].Name + "\", selected." + Variables[i].Name + ");\n";
									break;
								case "quaternion":
									_strFileData += "\n";
									_strFileData += "				if (v4Temp[" + intCnt.ToString() + "]	== null)\n			{\n";
									_strFileData += "					v4Temp[" + intCnt.ToString() + "] = Vector4.zero;\n";
									_strFileData += "					v4Temp[" + intCnt.ToString() + "].x = selected." + Variables[i].Name + ".x;\n";
									_strFileData += "					v4Temp[" + intCnt.ToString() + "].y = selected." + Variables[i].Name + ".y;\n";
									_strFileData += "					v4Temp[" + intCnt.ToString() + "].z = selected." + Variables[i].Name + ".z;\n";
									_strFileData += "					v4Temp[" + intCnt.ToString() + "].w = selected." + Variables[i].Name + ".w;\n";
									_strFileData += "				}\n";
									_strFileData += "				if (v4[" + intCnt.ToString() + "]			== null)		v4[" + intCnt.ToString() + "]	= v4Temp[" + intCnt.ToString() + "];\n";
									_strFileData += "				v4[" + intCnt.ToString() + "] = EditorGUILayout.Vector4Field(\"" + Variables[i].Name + "\", v4Temp[" + intCnt.ToString() + "]);\n";
									_strFileData += "				if (v4[" + intCnt.ToString() + "] != v4Temp[" + intCnt.ToString() + "])\n			{\n";
									_strFileData += "					q3 = Quaternion.identity;\n";
									_strFileData += "					q3.x = v4[" + intCnt.ToString() + "].x;\n";
									_strFileData += "					q3.y = v4[" + intCnt.ToString() + "].y;\n";
									_strFileData += "					q3.z = v4[" + intCnt.ToString() + "].z;\n";
									_strFileData += "					q3.w = v4[" + intCnt.ToString() + "].w;\n";
									_strFileData += "					selected." + Variables[i].Name + " = q3;\n";
									_strFileData += "					v4Temp[" + intCnt.ToString() + "] = v4[" + intCnt.ToString() + "];\n";
									_strFileData += "				}\n\n";
									intCnt++;
									break;
								case "date":
								case "datetime":
									_strFileData += "				EditorGUILayout.TextField(\"" + Variables[i].Name + "\", selected." + Variables[i].Name + ".ToString(\"MM/dd/yyyy HH:mm:ss\"));\n";
									break;
							}
					}
				}

				_strFileData += "				EditorGUILayout.Separator();\n";
				_strFileData += "				EditorGUILayout.BeginHorizontal();\n";
				_strFileData += "				if (GUILayout.Button(\"<--\"))\n				{\n";
				_strFileData += "					if (intSelected > 0)\n";
				_strFileData += "						intSelected--;\n";
				_strFileData += "					else\n";
				_strFileData += "						intSelected	= myTarget." + this.ClassName + "s.Count - 1;\n";
				_strFileData += "					selected			= myTarget." + this.ClassName + "s[intSelected];\n";
				_strFileData += "					blnSave				= false;\n";
				_strFileData += "					GUI.changed		= false;\n\n";
				_strFileData += "					GUI.SetNextControlName(\"\");\n";
				_strFileData += "					GUI.FocusControl (\"\");\n";
				_strFileData += "				}\n";
				_strFileData += "				if (GUILayout.Button(\"-->\"))\n				{\n";
				_strFileData += "					if (intSelected < myTarget." + this.ClassName + "s.Count - 1)\n";
				_strFileData += "						intSelected++;\n";
				_strFileData += "					else\n";
				_strFileData += "						intSelected	= 0;\n";
				_strFileData += "					selected			= myTarget." + this.ClassName + "s[intSelected];\n";
				_strFileData += "					blnSave				= false;\n";
				_strFileData += "					GUI.changed		= false;\n\n";
				_strFileData += "					GUI.SetNextControlName(\"\");\n";
				_strFileData += "					GUI.FocusControl (\"\");\n";
				_strFileData += "				}\n";
				_strFileData += "				EditorGUILayout.EndHorizontal();\n\n";
				_strFileData += "				if (GUI.changed)\n				{\n";
				_strFileData += "					EditorUtility.SetDirty(myTarget);\n";
				_strFileData += "					blnSave = true;\n";
				_strFileData += "				}\n";
				_strFileData += "				if (blnSave && GUILayout.Button(\"SAVE\"))\n				{\n";
				_strFileData += "					GUI.SetNextControlName(\"\");\n";
				_strFileData += "					GUI.FocusControl (\"\");\n";
				_strFileData += "					selected.Save();\n";
				_strFileData += "					blnSave = false;\n";
				_strFileData += "				}\n";
				_strFileData += "			} else {\n";
				_strFileData += "				EditorGUILayout.LabelField(\"There are No Records.\");\n";
//			_strFileData += "				if (GUILayout.Button(\"ADD NEW\"))\n";
//			_strFileData += "					myTarget." + this.ClassName + "s.Add(new " + this.ClassName + "());\n";
				_strFileData += "			}\n";
				_strFileData += "		}\n";
				_strFileData += "	}\n";
				_strFileData += "}\n\n";

				// WRITE THE FILE
				if (!Util.FileExists(CLASS_SCRIPT_DIRECTORY + "/Editor", strFileName))
					Util.WriteTextFile(CLASS_SCRIPT_DIRECTORY + "/Editor", strFileName, _strFileData);
			}

		#endregion

		#region "PUBLIC FUNCTIONS"

			public							ClassBuilder(ClassBuilder CB = null)
			{
				if (CB != null)
					Clone(CB);
			}
			public	void				Clone(ClassBuilder eff)
			{
				_intID									= eff.ID;
				_intIndex								= eff.Index;
				_blnIsActive						= eff.IsActive;
				_strClassName						= eff.ClassName;
				_strNamespace						= eff.Namespace;
				_blnUseUnity						= eff.UseUnity;
				_blnUseUnityUI					= eff.UseUnityUI;
				_blnIsNetworkObject			= eff.IsANetworkObject;
				_blnHasNetworkTransform	= eff.HasNetworkTransform;
				_blnUseEditor						= eff.UseEditor;
				_blnUseDatabase					= eff.UseDatabase;
				_blnUseAppMgr						= eff.UseAppMgr;
				_blnUseDBmgr						= eff.UseDBmgr;
				_blnUseNetMgr						= eff.UseNetMgr;
				_blnUseDBload						= eff.UseDBload;
				_blnUseDBsave						= eff.UseDBsave;
				_blnUseSerialization		= eff.UseSerialization;
				_DBserver								= eff.DBserver;
				_DBdatabase							= eff.DBdatabase;
				_DBuser									= eff.DBuser;
				_DBpassword							= eff.DBpassword;
				_DBuseWinAccount				= eff.DBuseWinAccount;

				_vars = new List<ClassProperty>();
				for (int i = 0; i < eff.Variables.Count; i++)
				{
					_vars.Add(eff.Variables[i]);
				}
				_blnInitialized				= true;
			}
			public	void				AddProperty(ClassProperty prop)
			{
				// REMOVE PROPERTY IF THE NAME ALREADY EXISTS, AND IT IS DELETED
				int i = Variables.FindIndex(x => x.Name.ToLower() == prop.Name.ToLower() && x.IsDeleted);
				if (i >= 0)
					Variables.RemoveAt(i);

				// ADD THE PROPERTY TO THE LIST
				Variables.Add(prop);
			}

			public	void				ResetClass()
			{
				_strClassName						= "";
				_strNamespace						= "";
				_blnInitialized					= false;
				_blnUseUnity						= true;
				_blnUseUnityUI					= false;
				_blnIsNetworkObject			= false;
				_blnHasNetworkTransform	= false;
				_blnUseEditor						= true;
				_blnUseDatabase					= true;
				_blnUseClassMgr					= true;
				_blnUseAppMgr						= false;
				_blnUseNetMgr						= false;
				_blnUseDBmgr						= true;
				_blnUseDBload						= true;
				_blnUseDBsave						= true;
				_blnUseSerialization		= true;
				_vars = new List<ClassProperty>();
			}
			public	void				ResetVariables()
			{
				if (ClassName.Trim() == "")
					return;

				_vars = new List<ClassProperty>();
				ClassProperty prop = null;
			
				// CREATE MANDATORY FIELDS
				// CREATE INDEX VARIABLE
				prop								= new ClassProperty();
				prop.IsIndex				= true;
				prop.Name						= ClassName + "ID";
				prop.VarType				= "int";
				prop.StartingValue	= "0";
				prop.IsMandatory		= true;
				prop.IsSearchable		= true;
				_vars.Add(prop);

				// CREATE DATE CREATED VARIABLE
				prop								= new ClassProperty();
				prop.Name						= "DateCreated";
				prop.VarType				= "DateTime";
				prop.IsMandatory		= true;
				_vars.Add(prop);

				// CREATE DATE UPDATED VARIABLE
				prop								= new ClassProperty();
				prop.Name						= "DateUpdated";
				prop.VarType				= "DateTime";
				prop.IsMandatory		= true;
				_vars.Add(prop);

				// CREATE ISACTIVE VARIABLE
				prop								= new ClassProperty();
				prop.Name						= "IsActive";
				prop.VarType				= "bool";
				prop.StartingValue	= "true";
				prop.IsMandatory		= true;
				_vars.Add(prop);

				// SET CLASS AS INITIALIZED
				_blnInitialized = true;
			}
			public	void				GenerateClass()
			{
				// BUILD THE CS BASE CLASS FILE SCRIPT
				CreateCSbaseFile();

				// BUILD THE CS CLASS FILE SCRIPT
				CreateCSclassFile();

				// BUILD THE CS CLASS MANAGER FILE
				CreateCSclassManager();

				// BUILD THE SQL QUERIES
				CreateSQLfiles();

				// BUILD THE CUSTOM EDITORS
				CreateEditorFile(true);			// EDITOR FOR BASE CLASS
				CreateEditorFile(false);		// EDITOR FOR USER-MODIFIED CLASS
				CreateManagerEditorFile();	// EDITOR FOR CLASS MANAGER

				// REMOVE DELETED VARIABLES
				for (int i = Variables.Count - 1; i >=0; i--)
				{
					if (Variables[i].IsDeleted)
						Variables.RemoveAt(i);
				}
			}
			public	bool				CreateSQLscriptsIntoSQLdatabase()
			{
				// BUILD THE SQL QUERIES
				// THIS FUNCTION ONLY WORKS IF THE DATABASE USER 
				// HAS APPROPRIATE PERMISSIONS FOR ALTER CONTRAINTS,
				// DROP/CREATE TABLES, AND DROP/CREATE STORED PROCEDURES.
				// OTHERWISE, THIS WILL NOT WORK...
				// INSTEAD, OPEN THE SQL FILE AND EXECUTE IT INSIDE
				// YOU MICROSOFT SQL SERVER MANAGERMENT STUDIO.
				bool bln = false;
				DatabaseManager.Instance.DAL.ClearParams();

				// DROP CONSTRAINTS
				_strFileData = "";
				BuildDropConstraints();
				_strFileData = _strFileData.Replace("GO\n\n", "");
				bln = DatabaseManager.Instance.DAL.DoSQLUpdateDelete(_strFileData);
				if (!bln)
					Debug.LogError(DatabaseManager.Instance.DAL.Errors);

				// DROP STORED PROCEDURES
				_strFileData = "";
				BuildDropStoredProcedures();
				_strFileData = _strFileData.Replace("GO\n\n", "");
				bln = DatabaseManager.Instance.DAL.DoSQLUpdateDelete(_strFileData);
				if (!bln)
					Debug.LogError(DatabaseManager.Instance.DAL.Errors);

				// DROP THE TABLE
				_strFileData = "";
				BuildDropTable();
				_strFileData = _strFileData.Replace("GO\n\n", "");
				bln = DatabaseManager.Instance.DAL.DoSQLUpdateDelete(_strFileData);
				if (!bln)
					Debug.LogError(DatabaseManager.Instance.DAL.Errors);

				// ADD THE TABLE
				_strFileData = "";
				BuildCreateTable();
				_strFileData = _strFileData.Replace("GO\n\n", "");
				bln = DatabaseManager.Instance.DAL.DoSQLUpdateDelete(_strFileData);
				if (!bln)
					Debug.LogError(DatabaseManager.Instance.DAL.Errors);

				// ADD CONSTRAINTS
				_strFileData = "";
				BuildAddConstraints();
				_strFileData = _strFileData.Replace("GO\n\n", "");
				bln = DatabaseManager.Instance.DAL.DoSQLUpdateDelete(_strFileData);
				if (!bln)
					Debug.LogError(DatabaseManager.Instance.DAL.Errors);

				// CREATE THE SQL LOAD/GET SCRIPTS
				_strFileData = "";
				BuildLoadingStoredProcedures();
				_strFileData = _strFileData.Replace("GO\n\n", "");
				bln = DatabaseManager.Instance.DAL.DoSQLUpdateDelete(_strFileData);
				if (!bln)
					Debug.LogError(DatabaseManager.Instance.DAL.Errors);

				// CREATE THE SQL UPDATE SCRIPT
				_strFileData = "";
				BuildSavingStoredProcedure();
				_strFileData = _strFileData.Replace("GO\n\n", "");
				bln = DatabaseManager.Instance.DAL.DoSQLUpdateDelete(_strFileData);
				if (!bln)
					Debug.LogError(DatabaseManager.Instance.DAL.Errors);

				return bln;
			}

		#endregion

	}
}
