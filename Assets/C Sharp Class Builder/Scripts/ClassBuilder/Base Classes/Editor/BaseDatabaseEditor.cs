// ===========================================================================================================
//
// Class/Library: Unity Database Editor Base
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Mar 26, 2016
//	
// VERS 1.0.000 : Mar 26, 2016 :	Original File Created. Released for Unity 3D.
//			1.0.001 : May 04, 2017 :	Added separate variables to track ListView scroll position and
//																editor scroll position (_v2ScrollLPosition and _v2ScrollEPosition).
//
// ===========================================================================================================

using UnityEngine;
using UnityEditor;

namespace CBT
{ 

	public partial class BaseDatabaseEditor<T, D, R> : EditorWindow where T: ScriptableObject where D: ScriptableObject where R: class
	{

		#region "PRIVATE/PROTECTED CONSTANTS"

			protected const		int				MINIMUM_WIDTH						= 500;
			protected const		int				MINIMUM_HEIGHT					= 600;
			protected	const		string		APP_ROOT_DIRECTORY			= "C Sharp Class Builder/";
			protected					string		DATABASE_FILE_DIRECTORY	= APP_ROOT_DIRECTORY + "Scripts/ClassBuilder/Database";
			protected					string		DATABASE_FILE_NAME			= @"XXX.asset";

		#endregion

		#region "PRIVATE VARIABLES"

			protected	D											editorDB							= null;
			protected	R											selected							= null;
			protected	bool									_blnIsInitializing		= false;
			private		bool									_blnIsInitialized			= false;
			protected	bool									_blnNote							= false;
			protected	Vector2								_v2ScrollEPosition		= Vector2.zero;

			[System.NonSerialized]
			protected	BaseDatabase<EnumBuilder>	_dbEnums		= null;
			[System.NonSerialized]
			protected	string[]				_strEnumList					= null;
			[System.NonSerialized]
			protected	string[]				_strEnumDefList				= null;
			[System.NonSerialized]
			protected	int							_intSelectedEnum			= -1;

		#endregion

		#region "PRIVATE PROPERTIES"

			protected		BaseDatabase<EnumBuilder>		DBenums
			{
				get
				{
					if (_dbEnums == null)
							_dbEnums = EnumBuilder.LoadDatabase();
					return _dbEnums;
				}
			}
			protected		string[]										EnumArray
			{
				get
				{
					if (_strEnumList == null)
							_strEnumList = CreateEnumPopUpList(DBenums);
					return _strEnumList;
				}
			}
			protected		string[]										EnumDefaultArray
			{
				get
				{
					return _strEnumDefList;
				}
			}

		#endregion

		#region "PUBLIC PROPERTIES"

			public	bool									IsInitialized
			{
				get
				{
					return _blnIsInitialized;
				}
				set
				{
					_blnIsInitialized = value;
				}
			}
			public	bool									IsInitializing
			{
				get
				{
					return _blnIsInitializing;
				}
			}

		#endregion

		#region "EVENT FUNCTIONS"

			private							void	OnEnable()
			{
				if (!IsInitialized && !_blnIsInitializing)
						Initialize();

				selected = null;
			}
			protected	virtual		void	OnDisable()
			{
				selected = null;
				editorDB = null;
				_blnIsInitialized		= false;
				_blnIsInitializing	= false;
			}
			private							void	OnGUI()
			{
				if (IsInitialized)
				{
					if (editorDB == null)
					{
						LoadDatabase();
						return;
					} else if (!((BaseDatabase<R>)(object)editorDB).IsDatabaseLoaded) {
						LoadDatabase();
						return;
					}

					DisplayEditorWindow();

				} else
					if (!_blnIsInitializing)
						Initialize();
			}

			protected virtual		void	Initialize()
			{
				if (_blnIsInitializing)
					return;
				_blnIsInitializing = true;
				LoadDatabase();
				IsInitialized = true;
			}
			protected virtual		void	DisplayEditorWindow()
			{
				GUILayout.BeginHorizontal();
				ListView();
				if (selected != null)
					DisplayEditor();
				GUILayout.EndHorizontal();
				DisplayCount();
			}
			protected						void	LoadDatabase()
			{
				try
				{
					if (editorDB != null && ((BaseDatabase<R>)(object)editorDB).IsDatabaseLoaded)
						return;

					string strDBfullPath = @"Assets/" + DATABASE_FILE_DIRECTORY + "/" + DATABASE_FILE_NAME;
					editorDB = ScriptableObject.CreateInstance<D>();

					if(!System.IO.Directory.Exists(@"Assets/" + DATABASE_FILE_DIRECTORY))
					{
						System.IO.Directory.CreateDirectory(@"Assets/" + DATABASE_FILE_DIRECTORY);
					} else {
						#if UNITY_EDITOR
							editorDB = AssetDatabase.LoadAssetAtPath(strDBfullPath, typeof(D)) as D;
						#else
							editorDB = Resources.GetBuiltinResource(typeof(D), strDBfullPath) as D;
						#endif

					}

					#if UNITY_EDITOR
					if (editorDB == null)
					{
						editorDB = ScriptableObject.CreateInstance<D>();
						AssetDatabase.CreateAsset(editorDB, strDBfullPath);
						AssetDatabase.SaveAssets();
						AssetDatabase.Refresh();
					}
					#endif
					((BaseDatabase<R>)(object)editorDB).IsDatabaseLoaded = (editorDB != null);
				} catch (System.Exception ex) {
					Debug.LogError("Error: " + ex.Message + "\n" + ex.InnerException); 
				}
			}

		#endregion

		#region "PRIVATE FUNCTIONS"

			protected virtual	int				GetIndex(string[] strArray, string strSelected)
			{
				for (int i = 0; i < strArray.Length; i++)
				{
					if (strSelected == strArray[i])
						return i;
				}
				return 0;
			}
			protected virtual	string[]	CreateEnumPopUpList(BaseDatabase<EnumBuilder>	db, bool blnIncludeAll = false, bool blnIncludeNone = false)
			{
				if (db == null || db.Count < 1)
					if (blnIncludeAll)
						return new string[] { "-- All -- "};
					else
						return new string[] {  };

				string[] st = new string[] { };

				if (blnIncludeAll)
				{
					st = new string[db.Count + 1];
					st[0] = "-- All --";
					for (int i = 0; i < db.Count; i++)
					{
						st[i + 1] = db.database[i].Name;
					}
				} else if (blnIncludeAll)
				{
					st = new string[db.Count + 1];
					st[0] = "-- None --";
					for (int i = 0; i < db.Count; i++)
					{
						st[i + 1] = db.database[i].Name;
					}
				} else {
					st = new string[db.Count];
					for (int i = 0; i < db.Count; i++)
					{
						st[i] = db.database[i].Name;
					}
				}

				return st;
			}
			protected virtual	string[]	CreateEnumDefaultPopUpListByID(BaseDatabase<EnumBuilder>	db, int intField)
			{
				if (db == null || db.Count < 1)
					return new string[] { };

				if (intField < 0 || intField > db.Count - 1)
					return new string[] { };

				string[] st = new string[] { };

				st = new string[db.database[intField].Variables.Count];
				for (int i = 0; i < db.database[intField].Variables.Count; i++)
				{
					st[i] = db.database[intField].Variables[i].Name;
				}

				return st;
			}
			protected virtual	string[]	CreateEnumDefaultPopUpListByName(BaseDatabase<EnumBuilder>	db, string strEnumName)
			{
				strEnumName = strEnumName.Replace("enum", "");
				if (db == null || db.Count < 1 || strEnumName == "")
					return new string[] { };
				
				int intField = -1;
				for (int i = 0; i < db.Count; i++)
					if (db.database[i].Name == strEnumName)
					{
						intField = i;
						break;
					}

				if (intField < 0 || intField > db.Count - 1)
					return new string[] { };

				string[] st = new string[] { };

				st = new string[db.database[intField].Variables.Count];
				for (int i = 0; i < db.database[intField].Variables.Count; i++)
				{
					st[i] = db.database[intField].Variables[i].Name;
				}

				return st;
			}

			protected virtual	void			DisplayEditor()
			{
				DisplayEditorTop();
/*
				BaseDescriptor theObject = ((BaseDescriptor)(object)selected);

				if (editorDB is BaseDatabase<BaseDescriptor>	&& selected is BaseDescriptor)
				{
					EditorGUILayout.LabelField(						"ID: ",				theObject.ID.ToString());

					// ADD IN YOU OWN CUSTOM DATA INPUTS IN SOME OTHER CLASS-SPECIFIC FILE
					// DON'T CHANGE THIS CODE/FILE AT ALL!!!

					GUILayout.EndVertical();
					GUILayout.Space(10);
					GUILayout.BeginHorizontal();

					if (GUILayout.Button("SAVE"))
					{
						if (selected != null && theObject.Name.Trim() != "")
						{
							((BaseDatabase<BaseDescriptor>)(object)editorDB).Save(theObject);
							selected = null;
							GUI.FocusControl("");
						}
					}
				}
*/
				DisplayEditorBottom();
			}
			protected virtual void			DisplayCount()
			{
				GUILayout.BeginHorizontal("Box", GUILayout.ExpandWidth(true));

				// HANDLE DESCRIPTOR DATABASE TYPES
/*
				if (editorDB is BaseDatabase<BaseDescriptor>)
				{
					GUILayout.Label("Record Count: " + ((BaseDatabase<BaseDescriptor>)(object)editorDB).Count.ToString() + " - (Hold CTRL to Delete a Record)");
					if (selected == null && GUILayout.Button("Add New"))
						selected = (R)(object)new BaseDescriptor();
				}
*/
				GUILayout.EndHorizontal();
			}

			protected					void			DisplayEditorTop()
			{
				GUILayout.BeginVertical();
				GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			}
			protected					void			DisplayEditorBottom()
			{
				GUILayout.Space(10);
				if (GUILayout.Button("CANCEL"))
				{
					_blnNote = false;
					selected = null;
					GUI.FocusControl("");
				}
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
			}

		#endregion

	}
}
