// ===========================================================================================================
//
// Class/Library: Class Builder Unity Database Editor
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Mar 26, 2016
//	
// VERS 1.0.000 : Mar 26, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace CBT
{ 

	public partial class ClassBuilderDatabaseEditor : BaseDatabaseEditor<ClassBuilderDatabaseEditor, ClassBuilderDatabase, ClassBuilder>
	{

		#region "PRIVATE/PROTECTED CONSTANTS"

			protected const			int			MINIMUM_WIDTH						= 825;
			protected const			int			MINIMUM_HEIGHT					= 600;
			private 	const			string	WINDOW_TITLE						= "Class Builder";
			private		const			string	MENU_NAME								= "Tools/Class Builder";
			private		const			string	dDATABASE_FILE_NAME			= @"ClassScriptDatabase.asset";	

		#endregion

		#region "PRIVATE VARIABLES"

			private		int							_intSelectedClass			= -1;

			[System.NonSerialized]
			private		string					_strNewVar						= "";
			[System.NonSerialized]
			private		string					_strNewType						= "";
			[System.NonSerialized]
			private		string					_strStart							= "";
			[System.NonSerialized]
			private		int							_intMaxLen						= 20;
			[System.NonSerialized]
			private		bool						_blnNewFind						= false;
			[System.NonSerialized]
			private		bool						_blnSyncVar						= false;
			[System.NonSerialized]
			private		bool						_blnIsName						= false;
			[System.NonSerialized]
			private		int							_intSelected					= 0;
			[System.NonSerialized]
			private		string[]				TypeArray							= { "int", "string", "bool", "float", "enum", "DateTime", "Vector2", "Vector3", "Quaternion", "Sprite", "Color" };

			[System.NonSerialized]
			private		string					strTemp								= "";
			[System.NonSerialized]
			private		int							intTemp								= 0;
			[System.NonSerialized]
			private		float						fTemp									= 0;
			[System.NonSerialized]
			private		bool						blnTemp								= false;
			[System.NonSerialized]
			private		Vector2					v2Temp								= Vector2.zero;
			[System.NonSerialized]
			private		Vector3					v3Temp								= Vector3.zero;
			[System.NonSerialized]
			private		Vector4					v4Temp								= Vector4.zero;
			[System.NonSerialized]
			private		string					dtTemp								= "";

		#endregion

		#region "EVENT FUNCTIONS"

			[MenuItem(MENU_NAME, false, 10)]
			public		static		void		Init()
			{
				ClassBuilderDatabaseEditor window = EditorWindow.GetWindow<ClassBuilderDatabaseEditor>();
				window.minSize = new Vector2(MINIMUM_WIDTH, MINIMUM_HEIGHT);
				window.titleContent.text = WINDOW_TITLE;
				window.Show();
			}

			protected override	void		Initialize()
			{
				if (IsInitialized || IsInitializing)
					return;

				_blnIsInitializing = true;
				DATABASE_FILE_NAME = dDATABASE_FILE_NAME;
				LoadDatabase();
				selected = null;
				IsInitialized = true;
			}
			protected override	void		DisplayEditorWindow()
			{
				GUILayout.BeginVertical();
				DisplayEditorCommands();
				GUILayout.BeginHorizontal();
				ListView();
				if (selected != null)
					DisplayEditor();
				GUILayout.EndHorizontal();
				GUILayout.EndVertical();
				DisplayCount();
			}

		#endregion

		#region "PRIVATE FUNCTIONS"

			private							void							DisplayEditorCommands()
			{
				return;

				// DISPLAY ANY FILTERS ON THE DATA AVAILABLE IN THE LISTVIEW
				GUILayout.BeginVertical("box");
				EditorGUILayout.LabelField("FILTERS: ");

				GUILayout.EndVertical();
			}
			protected override	void							DisplayEditor()
			{
				DisplayEditorTop();
				ClassBuilder theObject = ((ClassBuilder)(object)selected);

				if (_intSelectedClass != theObject.ID)
						_intSelectedClass	 = theObject.ID;

				// DISPLAY THE UI
				if (_intSelected < 0 || !theObject.IsInitialized) _intSelected = 0;			//GetIndex(TypeArray, DBitemSlot.GetByID(theObject.SlotID).Name);

				// DESCRIPTION
				EditorStyles.label.wordWrap = true;
				EditorGUILayout.LabelField("This Control allows you to create custom C# classes, " +
																	 "then builds all the necessary files (script, " + 
																	 "editor and SQL Database scripts). The scripts can " +
																	 "then be found in the Directory below.");
				EditorGUILayout.Separator();

				_v2ScrollEPosition = EditorGUILayout.BeginScrollView(_v2ScrollEPosition, GUILayout.ExpandHeight(true));


				// CLASS NAME AND NAMESPACE
																				EditorGUILayout.LabelField("Directory: ",								"Assets/" + theObject.Script_Directory);
				theObject.ClassName						= EditorGUILayout.TextField("Class Name: ",								theObject.ClassName);

				if (!theObject.IsInitialized && theObject.ClassName != "" && GUILayout.Button("INITIALIZE CLASS"))
						theObject.ResetVariables();

				// DEFINE THE CLASS SETTINGS
				if (theObject.IsInitialized)
				{
					GUI.changed = false;
					theObject.Namespace					= EditorGUILayout.TextField("Namespace: ",								theObject.Namespace);

					EditorGUILayout.Separator();
					EditorGUILayout.Space();

					EditorStyles.label.fontStyle = FontStyle.Bold;
					EditorGUILayout.LabelField("CONFIGURATION");
					EditorStyles.label.fontStyle = FontStyle.Normal;

					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.BeginVertical();

					theObject.UseUnity							= EditorGUILayout.Toggle("Uses Unity",											theObject.UseUnity,						GUILayout.Width(200));
					if (theObject.UseUnity)
					{
						theObject.UseUnityUI					= EditorGUILayout.Toggle("-- Uses Unity UI",							theObject.UseUnityUI,						GUILayout.Width(200));
						if (!theObject.UseUnityUI)
							theObject.UseUnityDatabase	= false;
						theObject.IsANetworkObject		= EditorGUILayout.Toggle("-- Is Networked Object",				theObject.IsANetworkObject,			GUILayout.Width(200));
						if (theObject.IsANetworkObject)
							theObject.HasNetworkTransform	= EditorGUILayout.Toggle("-- Has Net Transform",					theObject.HasNetworkTransform, GUILayout.Width(200));
						else
							theObject.HasNetworkTransform = false;
						if (!theObject.UseUnityDatabase)
						{
							theObject.UseEditor						= EditorGUILayout.Toggle("-- Create Inspector",						theObject.UseEditor,		GUILayout.Width(200));
							theObject.UseClassMgr					= EditorGUILayout.Toggle("-- Create Class Manager",				theObject.UseClassMgr,	GUILayout.Width(200));
							if (theObject.UseEditor || theObject.UseClassMgr)
								theObject.UseUnityDatabase = false;
						}
					} else { 
						theObject.UseUnityUI					= false;
						theObject.IsANetworkObject		= false;
						theObject.HasNetworkTransform	= false;
						theObject.UseEditor						= false;
						theObject.UseClassMgr					= false;
						theObject.UseUnityDatabase		= false;
					}
					theObject.UseSerialization			= EditorGUILayout.Toggle("-- Serializer/Deserializer",		theObject.UseSerialization, GUILayout.Width(200));
					if (!theObject.UseSerialization)
						theObject.UseUnityDatabase		= false;

					if (theObject.UseUnity)
					{
						EditorGUILayout.LabelField("Uses Other Managers (Singletons)");
						theObject.UseAppMgr						= EditorGUILayout.Toggle("-- ApplicationManager?",				theObject.UseAppMgr,	GUILayout.Width(200));
						theObject.UseNetMgr						= EditorGUILayout.Toggle("-- NetworkManager?",						theObject.UseNetMgr,	GUILayout.Width(200));
					} else {
						theObject.UseAppMgr						= false;
						theObject.UseNetMgr						= false;
					}

					EditorGUILayout.EndVertical();

					EditorGUILayout.BeginVertical();
					if (theObject.UseUnity)
					{
						bool b		= EditorGUILayout.Toggle("Uses Unity Database",						theObject.UseUnityDatabase, GUILayout.Width(200));
						if (b != theObject.UseUnityDatabase && b)
						{
							theObject.UseUnityDatabase	= true;
							theObject.UseUnityUI				= true;
							theObject.UseSerialization	= true;
							theObject.UseSQLDatabase		= false;
							theObject.UseEditor					= false;
							theObject.UseClassMgr				= false;
						}
					}

					theObject.UseSQLDatabase			= EditorGUILayout.Toggle("Uses SQL Database",							theObject.UseSQLDatabase, GUILayout.Width(200));
					if (theObject.UseSQLDatabase)
							theObject.UseUnityDatabase = false;
			
					if (theObject.UseUnity)
					{ 
						if (theObject.UseSQLDatabase)
							theObject.UseDBmgr				= EditorGUILayout.Toggle("-- DatabaseManager?",						theObject.UseDBmgr, GUILayout.Width(200));
						else
							theObject.UseDBmgr				= false;
					} else {
						theObject.UseDBmgr					= false;
					}
					if (theObject.UseSQLDatabase)
					{ 
						theObject.UseDBload					= EditorGUILayout.Toggle("-- Handle Loads",								theObject.UseDBload, GUILayout.MinWidth(200));
						theObject.UseDBsave					= EditorGUILayout.Toggle("-- Handle Saves",								theObject.UseDBsave, GUILayout.MinWidth(200));
					} else {
						theObject.UseDBload					= false;
						theObject.UseDBsave					= false;
					}
					EditorGUILayout.Separator();

					if (!theObject.UseDBmgr && theObject.UseSQLDatabase)
					{
						EditorGUILayout.LabelField("MS-SQL SERVER SET UP");
						theObject.DBserver					= EditorGUILayout.TextField("Server",											theObject.DBserver);
						theObject.DBdatabase				= EditorGUILayout.TextField("Database",										theObject.DBdatabase);
						theObject.DBuseWinAccount		= EditorGUILayout.Toggle("Use Windows User Acct",					theObject.DBuseWinAccount);
						if (!theObject.DBuseWinAccount)
						{ 
							theObject.DBuser					= EditorGUILayout.TextField("Username",										theObject.DBuser);
							theObject.DBpassword			= EditorGUILayout.PasswordField("Password",								theObject.DBpassword);
						} else {
							theObject.DBuser					= "";
							theObject.DBpassword			= "";
						}
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.EndHorizontal();
					EditorGUILayout.Separator();
					EditorGUILayout.Space();

					// ADD A NEW PROPERTY
					// -- CONTENT
					EditorGUILayout.BeginHorizontal();
					EditorStyles.label.fontStyle = FontStyle.Bold;
					EditorGUILayout.LabelField("CREATE NEW CLASS FIELD");
					EditorStyles.label.fontStyle = FontStyle.Normal;
					if (GUILayout.Button("OPEN ENUM EDITOR"))
						EnumBuilderDatabaseEditor.Init();
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginHorizontal();
					// ---- FIELD NAME
					EditorGUILayout.BeginVertical();
					EditorStyles.label.richText = true;
					EditorStyles.label.stretchWidth = false;
					EditorGUILayout.LabelField("\n<color=yellow>Field Name</color>", GUILayout.Width(100), GUILayout.Height(32));
					EditorStyles.label.richText = false;
					EditorStyles.label.stretchWidth = true;
					_strNewVar = EditorGUILayout.TextField("", _strNewVar, GUILayout.Width(100));
					_strNewVar = Regex.Replace(_strNewVar, @"[^a-zA-Z0-9_]", "");
					EditorGUILayout.EndVertical();

					// ---- FIELD TYPE
					EditorGUILayout.BeginVertical();
					EditorStyles.label.richText = true;
					EditorStyles.label.stretchWidth = false;
					EditorGUILayout.LabelField("\n<color=yellow>Type</color>", GUILayout.Width(75), GUILayout.Height(32));
					EditorStyles.label.richText = false;
					EditorStyles.label.stretchWidth = true;
					int iSlot							= EditorGUILayout.Popup(		"", _intSelected, TypeArray, GUILayout.Width(75));
					if (iSlot != _intSelected)
					{
						_strNewType		= TypeArray[iSlot];
						_intSelected	= iSlot;
					}
					EditorGUILayout.EndVertical();

					// ---- MAXIMUM LENGTH (FOR STRING ONLY)
					EditorGUILayout.BeginVertical();
					EditorStyles.label.richText = true;
					EditorStyles.label.stretchWidth = false;
					EditorGUILayout.LabelField("\n<color=yellow>MaxLn</color>", GUILayout.Width(40), GUILayout.Height(32));
					EditorStyles.label.richText = false;
					EditorStyles.label.stretchWidth = true;
					if (_strNewType == "string")
						_intMaxLen					= EditorGUILayout.IntField(	"", _intMaxLen, GUILayout.Width(40));
					else
						EditorGUILayout.LabelField(" ", GUILayout.Width(40));
					EditorGUILayout.EndVertical();

					// ---- DEFAULT STARTING VALUE
					EditorGUILayout.BeginVertical();
					EditorStyles.label.richText = true;
					EditorStyles.label.stretchWidth = false;
					if (_intSelected == 4)
					EditorGUILayout.LabelField("\n<color=yellow>Enum Type</color>", GUILayout.Width(150), GUILayout.Height(32));
					else
					EditorGUILayout.LabelField("\n<color=yellow>Default Value</color>", GUILayout.Width(150), GUILayout.Height(32));
					EditorStyles.label.richText = false;
					EditorStyles.label.stretchWidth = true;
					switch (_intSelected)
					{
						case 0:		// "int":
							int x = EditorGUILayout.IntField("", intTemp, GUILayout.Width(150));
							if (x != intTemp)
							{
								_strStart = x.ToString();
								intTemp = x;
							}
							break;
						case 1:		// "string":
							string s = EditorGUILayout.TextField("", strTemp, GUILayout.Width(150));
							if (s != strTemp)
							{
								_strStart = s.ToString();
								strTemp = s;
							}
							break;
						case 2:		// "bool":
							bool bln = EditorGUILayout.Toggle("", blnTemp, GUILayout.Width(150));
							if (bln != blnTemp)
							{
								_strStart = bln.ToString().ToLower();
								blnTemp = bln;
							}
							break;
						case 3:		// "float":
							float f = EditorGUILayout.FloatField("", fTemp, GUILayout.Width(150));
							if (f != fTemp)
							{
								_strStart = f.ToString();
								fTemp = f;
							}
							break;
						case 4:		// "enum"
							int iEnum = EditorGUILayout.Popup("", _intSelectedEnum, EnumArray, GUILayout.Width(150));
							if (iEnum != _intSelectedEnum)
							{
								_strNewType = "enum" + EnumArray[iEnum];
								_intSelectedEnum = iEnum;
								_strEnumDefList = CreateEnumDefaultPopUpListByID(DBenums, iEnum);
							}
							if (_intSelectedEnum >= 0 && EnumDefaultArray != null && EnumDefaultArray.Length > 0)
							{ 
								int e = EditorGUILayout.Popup("", intTemp, EnumDefaultArray, GUILayout.Width(150));
								if (e != intTemp)
								{
									_strStart = e.ToString();
									intTemp = e;
								}
							}
							break;
						case 5:		// "date" "datetime"
							string d = EditorGUILayout.TextField("", dtTemp, GUILayout.Width(150));
							try
							{
								if (Util.IsDate(d))
								{ 
									System.DateTime dx = Util.ConvertToDate(d);
									if (dx.ToString("MM/dd/yyyy HH:mm:ss") != dtTemp)
									{
										_strStart = dx.ToString("MM/dd/yyyy HH:mm:ss");
										dtTemp = _strStart;
									}
								} else {
									dtTemp = "";
								}
							} catch {
								Debug.LogError("DateTime Error");
								dtTemp = "";
							}
							break;
						case 6:		// "vector2":
							Vector2 v2 = EditorGUILayout.Vector2Field("", v2Temp, GUILayout.Width(150));
							if (v2 != v2Temp)
							{
								_strStart = v2.ToString();
								v2Temp = v2;
							}
							break;
						case 7:		// "vector3":
							Vector3 v3 = EditorGUILayout.Vector3Field("", v3Temp, GUILayout.Width(150));
							if (v3 != v3Temp)
							{
								_strStart = v3.ToString();
								v3Temp = v3;
							}
							break;
						case 8:		// "quaternion":
							Vector4 v4 = EditorGUILayout.Vector4Field("", v4Temp, GUILayout.Width(150));
							if (v4 != v4Temp)
							{
								Quaternion q = Quaternion.identity;
								q.x = v4.x;
								q.y = v4.y;
								q.z = v4.z;
								q.w = v4.w;
								_strStart = q.ToString();
								v4Temp = v4;
							}
							break;
						case 9:		// "sprite"
							break;
					}
					EditorGUILayout.EndVertical();

					// ---- CAN SEARCH ON
					EditorGUILayout.BeginVertical();
					EditorStyles.label.richText = true;
					EditorStyles.label.stretchWidth = false;
					EditorGUILayout.LabelField("<color=yellow>Can\nFind</color>", GUILayout.Width(40), GUILayout.Height(32));
					EditorStyles.label.richText = false;
					EditorStyles.label.stretchWidth = true;
					_blnNewFind					= EditorGUILayout.Toggle(	"", _blnNewFind, GUILayout.Width(40)) && !(_intSelected==9);
					EditorGUILayout.EndVertical();

					// ---- IS NETWORKED OBJECT -- SYNCVAR
					if (theObject.IsANetworkObject)
					{ 
					EditorGUILayout.BeginVertical();
					EditorStyles.label.richText = true;
					EditorStyles.label.stretchWidth = false;
					EditorGUILayout.LabelField("<color=yellow>Sync\nVar</color>", GUILayout.Width(40), GUILayout.Height(32));
					EditorStyles.label.richText = false;
					EditorStyles.label.stretchWidth = true;
					_blnSyncVar					= EditorGUILayout.Toggle(	"", _blnSyncVar, GUILayout.Width(40)) && !(_intSelected==9);
					EditorGUILayout.EndVertical();
					}

					// ---- IS NAME OF THE OBJECT
					EditorGUILayout.BeginVertical();
					EditorStyles.label.richText = true;
					EditorStyles.label.stretchWidth = false;
					EditorGUILayout.LabelField("<color=yellow>Obj\nName</color>", GUILayout.Width(40), GUILayout.Height(32));
					EditorStyles.label.richText = false;
					EditorStyles.label.stretchWidth = true;
					if (theObject.HasNamedVariable)
					{
						_blnIsName = false;
						EditorGUILayout.Toggle(	"", false, GUILayout.Width(40));
					} else 
						_blnIsName					= EditorGUILayout.Toggle(	"", _blnIsName, GUILayout.Width(40)) && !(_intSelected==9);
					EditorGUILayout.EndVertical();

					// ---- ADD BUTTON
					EditorGUILayout.BeginVertical();
					EditorStyles.label.richText			= true;
					EditorStyles.label.stretchWidth = false;
					EditorGUILayout.LabelField("", GUILayout.Width(75), GUILayout.Height(32));
					EditorStyles.label.richText			= false;
					EditorStyles.label.stretchWidth	= true;
					int			intVariableExists				= theObject.Variables.FindIndex(x => x.Name.ToLower() == _strNewVar.Trim().ToLower());
					bool		blnVariableExists				= false;
					string	strAddVarBtnName				= "ADD";
					if (theObject.Variables.Count > 0 && intVariableExists >= 0 &&
							!theObject.Variables[intVariableExists].IsIndex && !theObject.Variables[intVariableExists].IsMandatory)
					{
						strAddVarBtnName = "UPDATE";
						blnVariableExists	= true;
					}
					if (GUILayout.Button(strAddVarBtnName, GUILayout.Width(75)) && _strNewVar != "" && 
							(_intSelected != 4 || ( _intSelected == 4 && _intSelectedEnum >= 0)) && 
							(blnVariableExists || (!blnVariableExists && theObject.Variables.FindAll(x => x.Name.ToLower() == _strNewVar.ToLower()).Count < 1)) &&
							DBenums.GetByName(_strNewVar) == null)
					{
						if (_strNewType == "")
								_strNewType = "int";
						ClassBuilder.ClassProperty prop = new ClassBuilder.ClassProperty();
						prop.Name						= _strNewVar;
						prop.MaxLength			= _intMaxLen;
						prop.VarType				= _strNewType;
						prop.StartingValue	= _strStart;
						prop.IsSearchable		= _blnNewFind;
						prop.IsSynchVar			= _blnSyncVar;
						prop.IsNameVar			= _blnIsName;
						if (blnVariableExists)
							theObject.Variables[intVariableExists].Deserialize(prop.Serialize());
						else
							theObject.AddProperty(prop);
						blnVariableExists		= false;
						_strNewVar					= "";
						_strNewType					= "int";
						_strStart						= "";
						_intMaxLen					= 20;
						_intSelected				= -1;
						_intSelectedEnum		= -1;
						_blnSyncVar					= false;

						strTemp							= "";
						intTemp							= 0;
						fTemp								= 0;
						blnTemp							= false;
						v2Temp							= Vector2.zero;
						v3Temp							= Vector3.zero;
						v4Temp							= Vector4.zero;
						dtTemp							= "";
						GUI.FocusControl(null);
					}
					EditorGUILayout.EndVertical();

					EditorGUILayout.EndHorizontal();
					EditorGUILayout.Separator();
					EditorGUILayout.Space();
			
					// LIST CLASS PROPERTIES
					// -- HEADER
					EditorStyles.label.fontStyle = FontStyle.Bold;
					EditorGUILayout.LabelField("CURRENT CLASS FIELDS");
					EditorStyles.label.fontStyle = FontStyle.Normal;
					EditorGUILayout.BeginHorizontal();
					EditorStyles.label.richText = true;
					EditorStyles.label.stretchWidth = false;
								GUILayout.Button(" ", GUILayout.Width(20), GUILayout.MaxWidth(20));
					EditorGUILayout.LabelField("\n<color=yellow>Field Name</color>",		GUILayout.Width(125), GUILayout.Height(32));
					EditorGUILayout.LabelField("\n<color=yellow>Type</color>",					GUILayout.Width(75),	GUILayout.Height(32));
					EditorGUILayout.LabelField("\n<color=yellow>MaxLn</color>",					GUILayout.Width(40),	GUILayout.Height(32));
					EditorGUILayout.LabelField("<color=yellow>Can\nFind</color>",				GUILayout.Width(40),	GUILayout.Height(32));
					if (theObject.IsANetworkObject)
					EditorGUILayout.LabelField("<color=yellow>Sync\nVar</color>",				GUILayout.Width(40),	GUILayout.Height(32));
					EditorGUILayout.LabelField("<color=yellow>Obj\nName</color>",				GUILayout.Width(40),	GUILayout.Height(32));
					EditorGUILayout.LabelField("\n<color=yellow>Default Value</color>",	GUILayout.Width(125),	GUILayout.Height(32));
					EditorStyles.label.richText = false;
					EditorStyles.label.stretchWidth = true;
					EditorGUILayout.EndHorizontal();

					// -- CONTENT
					EditorGUILayout.BeginVertical();
					Color defaultColor		= GUI.color;
					Color defaultBGcolor	= GUI.backgroundColor;
					for (int i = 0; i < theObject.Variables.Count; i++)
					{
						bool blnHighlight = blnVariableExists && _strNewVar.Trim().ToLower() == theObject.Variables[i].Name.ToLower();
						if (!theObject.Variables[i].IsDeleted)
						{
							EditorGUILayout.BeginHorizontal();

							if (blnHighlight)
							{ 
								GUI.color = Color.green; 
								GUI.backgroundColor = Color.cyan;
							}

							if (!theObject.Variables[i].IsIndex && !theObject.Variables[i].IsMandatory)
							{
								if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.MaxWidth(20)))
									if (EditorUtility.DisplayDialog("Delete this Property?", "Are you sure that you want to delete \"" + theObject.Variables[i].Name + "\"?", "Delete", "Cancel"))
										theObject.Variables[i].IsDeleted = true;
							} else 
								GUILayout.Button(" ", GUILayout.Width(20), GUILayout.MaxWidth(20));
							EditorGUILayout.LabelField(theObject.Variables[i].Name,	GUILayout.Width(125));

							if (theObject.Variables[i].VarType.ToLower().StartsWith("enum"))
								EditorGUILayout.LabelField(theObject.Variables[i].VarType.Substring(4), GUILayout.Width(75));
							else
								EditorGUILayout.LabelField(theObject.Variables[i].VarType,							GUILayout.Width(75));

							if (theObject.Variables[i].VarType.ToLower() == "string")
								EditorGUILayout.LabelField(theObject.Variables[i].MaxLength.ToString(), GUILayout.Width(40));
							else
								EditorGUILayout.LabelField("", GUILayout.Width(40));

							EditorGUILayout.Toggle(theObject.Variables[i].IsSearchable,								GUILayout.Width(40));

							if (theObject.IsANetworkObject)
								EditorGUILayout.Toggle(theObject.Variables[i].IsSynchVar,								GUILayout.Width(40));

							EditorGUILayout.Toggle(theObject.Variables[i].IsNameVar,									GUILayout.Width(40));

							if (theObject.Variables[i].VarType.ToLower().StartsWith("enum"))
							{
								string sV = theObject.Variables[i].VarType.Substring(4).ToLower();
								int w = DBenums.database.FindIndex(x => x.Name.ToLower() == sV);
								if (w >= 0)
								EditorGUILayout.LabelField(CreateEnumDefaultPopUpListByID(DBenums, w)[Util.ConvertToInt(theObject.Variables[i].StartingValue)],	GUILayout.Width(125));
							} else
								EditorGUILayout.LabelField(theObject.Variables[i].StartingValue, GUILayout.Width(125));

							GUI.color						= defaultColor;
							GUI.backgroundColor	= defaultBGcolor;

							EditorGUILayout.EndHorizontal();
						}
					}
					EditorGUILayout.EndVertical();

					// CREATE BUTTON
					EditorGUILayout.Space();

					if (GUI.changed)
						_blnNote = false;

					if (Application.isPlaying && false)
					{ 
						EditorGUILayout.Space();
						if (theObject.UseSQLDatabase && theObject.Variables.Count > 0 && GUILayout.Button("MODIFY DATABASE"))
						{ 
							_blnNote = theObject.CreateSQLscriptsIntoSQLdatabase();
						}
					}
				}
				EditorGUILayout.EndScrollView();

				GUILayout.EndVertical();
				GUILayout.Space(10);

				if (_blnNote)
				{ 
					EditorGUILayout.LabelField("Files have been successfully Generated.");
					GUILayout.Space(5);
				}

				if (theObject.Variables.Count > 0 && GUILayout.Button("GENERATE CLASS FILES"))
				{
					theObject.GenerateClass();
					_blnNote = true;
				}
				GUILayout.Space(10);

				GUILayout.BeginHorizontal();
				if (GUILayout.Button("SAVE"))
				{
					if (selected != null && selected.Name.Trim() != "")
					{
						((BaseDatabase<ClassBuilder>)(object)editorDB).Save(theObject);
						selected = null;
						GUI.FocusControl("");
					}
				}

				DisplayEditorBottom();
			}
			protected override	void							DisplayCount()
			{
				GUILayout.BeginHorizontal("Box", GUILayout.ExpandWidth(true));

				GUILayout.Label("Record Count: " + ((BaseDatabase<ClassBuilder>)(object)editorDB).Count.ToString() + " - (Hold CTRL to Delete a Record)");
				if (selected == null && GUILayout.Button("Add New"))
					selected = (ClassBuilder)(object)new ClassBuilder();

				GUILayout.EndHorizontal();
			}

		#endregion

	}

}
