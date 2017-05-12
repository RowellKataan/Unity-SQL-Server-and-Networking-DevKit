// ===========================================================================================================
//
// Class/Library: Enum Builder - Database Script  (non-SQL database)
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Apr 19, 2016
//	
// VERS 1.0.000 : Apr 19, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace CBT
{	
	[System.Serializable]
	public class EnumBuilder
	{

		[System.Serializable]
		public	class		EnumProperty
		{
			[SerializeField]	public	int					Index					= 0;
			[SerializeField]	public	string			Name					= "";
			[SerializeField]	public	int					IntValue			= -1;
			[SerializeField]	public	bool				IsDeleted			= false;	

			public						EnumProperty(string strName = "", int newValue = -1)
			{
				Index			= 0;
				Name			= strName;
				IntValue	= newValue;
				IsDeleted = false;
			}
			public	string		Serialize()
			{
				string strOut = "";

				strOut += Index.ToString()		+ "^";
				strOut += Name.ToString()			+ "^";
				strOut += IntValue.ToString()	+ "^";
				strOut += Util.ConvertToInt(IsDeleted).ToString();

				return strOut;
			}
			public	void			Deserialize(string strInput)
			{
				string[] strSpl = strInput.Split('^');

					Index			=	Util.ConvertToInt(strSpl[0]);
					Name			= strSpl[1];
					IntValue	=	Util.ConvertToInt(strSpl[2]);
					IsDeleted	= Util.ConvertToBoolean(strSpl[3]);
			}
		}

		#region "PRIVATE/PROTECTED CONSTANTS"

			protected	static	string		APP_ROOT_DIRECTORY			= "C Sharp Class Builder/";
			protected	static	string		DATABASE_FILE_DIRECTORY	= APP_ROOT_DIRECTORY + "Scripts/ClassBuilder/Database";		// WHERE THE UNITY DATABASE FILE IS STORED FOR THE ENUMS WE CREATE
			protected	static	string		DATABASE_FILE_NAME			= "EnumDatabase.asset";																		// THE NAME OF THE DATABASE FILE FOR THE ENUMS WE CREATE

		#endregion

		#region "PRIVATE VARIABLES"

			// CLASS VARIABLES -- USED BY SYSTEM
			[SerializeField]	private	int					_intID								= 0;				
			[SerializeField]	private int					_intIndex							= -1;
			[SerializeField]	private bool				_blnIsActive					= true;		

			// CLASS VARIABLES -- SET BY USER
			[SerializeField]	private string			_strEnumName					= "";
			[SerializeField]	private	List<EnumProperty>		_vars				= new List<EnumProperty>();

			// INTERNAL PROCESS VARIABLES
			[System.NonSerialized]
												private bool				_blnInitialized				= false;

		#endregion

		#region "PRIVATE PROPERTIES"
		
			private string			Serialize()
			{
				string strOut = "";

				// SERIALIZE BASE CLASS PORTION
				strOut += _strEnumName + "|";

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
				string[] strSpl2 = strSpl1[1].Split('©');

				// DESERIALIZE BASE CLASS PORTION
				_strEnumName	= strSpl1[0];

				// DESERIALIZE PROPERTIES PORTION
				_vars = new List<EnumProperty>();
				foreach (string st in strSpl2)
				{
					EnumProperty prop = new EnumProperty();
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

			public	string			Name							{ get { return _strEnumName;					} set { _strEnumName = value.Trim(); } }
			public	List<EnumProperty>	Variables	{ get { return _vars; } }

			public	bool				IsInitialized
			{
				get
				{
					return _blnInitialized;
				}
			}

		#endregion

		#region "PUBLIC STATIC FUNCTIONS"

			public	static	BaseDatabase<EnumBuilder>			LoadDatabase()
			{
				BaseDatabase<EnumBuilder> db = null;
				string strDBfullPath = @"Assets/" + DATABASE_FILE_DIRECTORY + "/" + DATABASE_FILE_NAME;

				try
				{
					// db = ScriptableObject.CreateInstance<BaseDatabase<BaseEffect>>();
					if (!System.IO.Directory.Exists(@"Assets/" + DATABASE_FILE_DIRECTORY))
					{
						System.IO.Directory.CreateDirectory(@"Assets/" + DATABASE_FILE_DIRECTORY);
					} else {
						#if UNITY_EDITOR
							db = AssetDatabase.LoadAssetAtPath(strDBfullPath, typeof(BaseDatabase<EnumBuilder>)) as BaseDatabase<EnumBuilder>;
						#else
							db = Resources.GetBuiltinResource(typeof(BaseDatabase<EnumBuilder>), strDBfullPath) as BaseDatabase<EnumBuilder>;
						#endif
					}

					#if UNITY_EDITOR
					if (db == null)
					{
						db = ScriptableObject.CreateInstance<BaseDatabase<EnumBuilder>>();
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

		#region "PUBLIC FUNCTIONS"

			public							EnumBuilder(EnumBuilder CB = null)
			{
				if (CB != null)
					Clone(CB);
				else 
					ResetClass();
			}
			public	void				Clone(EnumBuilder eff)
			{
				_intID								= eff.ID;
				_intIndex							= eff.Index;
				_blnIsActive					= eff.IsActive;
				_strEnumName					= eff.Name;

				_vars = new List<EnumProperty>();
				for (int i = 0; i < eff.Variables.Count; i++)
				{
					_vars.Add(eff.Variables[i]);
				}
				_blnInitialized				= true;
			}
			public	void				AddProperty(EnumProperty prop)
			{
				// DETERMINE IF THE INTVALUE ALREADY EXISTS UNDER A DIFFERENT ENUM NAME
				int i = Variables.FindIndex(x => x.IntValue == prop.IntValue && x.Name.ToLower() != prop.Name.ToLower());
				if (i >= 0)
				{
					Debug.LogError("The IntValue (" + prop.IntValue.ToString() + ") already exists for Enum (" + Variables[i].Name + ").");
					return;
				}

				// DETERMINE INDEX
				int intIndex = 0;
				if (Variables.Count > 0)
						intIndex = Variables.Max(x => x.Index) + 1;

				// REMOVE PROPERTY IF THE NAME ALREADY EXISTS, AND IT IS DELETED
				i = Variables.FindIndex(x => x.Name.ToLower() == prop.Name.ToLower() );		// && x.IsDeleted
				if (i >= 0)
				{
					if (prop.IntValue < 0)
							prop.IntValue = Variables[i].IntValue; 
					Variables.RemoveAt(i);
					intIndex = i;
				} else {
					if (prop.IntValue < 0)
					{
						try {  i = Variables.Max(x => x.IntValue) + 1; } catch { i = 0; }
						prop.IntValue = i;
					}
				}

				// ADD THE PROPERTY TO THE LIST
				prop.Index = intIndex;
				Variables.Add(prop);
			}

			public	void				ResetClass()
			{
				_strEnumName					= "";
				_vars = new List<EnumProperty>();
				_vars.Add(new EnumProperty("NONE", 0));
				_blnInitialized = true;
			}
			public	void				ResetVariables()
			{
				if (this.Name.Trim() == "")
					return;

				// INIT THE VAR LIST
				_vars = new List<EnumProperty>();
				_vars.Add(new EnumProperty("NONE", 0));

				// SET CLASS AS INITIALIZED
				_blnInitialized = true;
			}

		#endregion

	}
}
