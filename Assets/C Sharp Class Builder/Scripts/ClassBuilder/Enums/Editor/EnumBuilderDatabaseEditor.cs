// ===========================================================================================================
//
// Class/Library: Enum Builder Unity Database Editor
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Apr 19, 2016
//	
// VERS 1.0.000 : Apr 19, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace CBT
{ 

	public partial class EnumBuilderDatabaseEditor : BaseDatabaseEditor<EnumBuilderDatabaseEditor, EnumDatabase, EnumBuilder>
	{

		#region "PRIVATE/PROTECTED CONSTANTS"

			private 	const			string	WINDOW_TITLE						= "Enum Builder";
			private		const			string	MENU_NAME								= "Tools/Enum Builder";
			private		const			string	dDATABASE_FILE_NAME			= @"EnumDatabase.asset";	

		#endregion

		#region "PRIVATE VARIABLES"

			[System.NonSerialized]
			private		string					_strNewName				= "";
			[System.NonSerialized]
			private		string					_strNewVar				=	"";
			[System.NonSerialized]
			private		int							_intNewValue			= -1;

			[System.NonSerialized]
			private		int							_intSelected			= 0;

		#endregion

		#region "EVENT FUNCTIONS"

			[MenuItem(MENU_NAME, false, 11)]
			public		static		void		Init()
			{
				EnumBuilderDatabaseEditor window = EditorWindow.GetWindow<EnumBuilderDatabaseEditor>();
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

			protected	bool												EnumExistsInClass(string strVar)
			{
				BaseDatabase<ClassBuilder> cb = ClassBuilder.LoadDatabase();
				foreach (ClassBuilder c in cb.database)
				{
					if (c.HasVariableType(strVar))
						return true;
				}
				return false;
			}

			private							void							DisplayEditorCommands()
			{
				return;

				// DISPLAY ANY FILTERS ON THE DATA AVAILABLE IN THE LISTVIEW
				GUILayout.BeginVertical("box");
				EditorGUILayout.LabelField("FILTERS: ");

				GUILayout.EndVertical();
			}
			protected	override	void							DisplayEditor()
			{
				DisplayEditorTop();
				EnumBuilder theObject = ((EnumBuilder)(object)selected);

				if (_intSelectedEnum	!= theObject.ID)
						_intSelectedEnum	 = theObject.ID;

				// DISPLAY THE UI
				if (_intSelected < 0 || !theObject.IsInitialized) _intSelected = 0;			//GetIndex(TypeArray, DBitemSlot.GetByID(theObject.SlotID).Name);

				// DESCRIPTION
				EditorStyles.label.wordWrap = true;
				EditorGUILayout.LabelField("This Control allows you to create custom enums.");
				EditorGUILayout.Separator();

				_v2ScrollEPosition = EditorGUILayout.BeginScrollView(_v2ScrollEPosition, GUILayout.ExpandHeight(true));

				if (!theObject.IsInitialized && theObject.Name != "" && GUILayout.Button("INITIALIZE ENUM"))
						theObject.ResetVariables();

				// DEFINE THE CLASS SETTINGS
				if (theObject.IsInitialized)
				{
					GUI.changed = false;

					EditorStyles.label.fontStyle = FontStyle.Bold;
					EditorGUILayout.LabelField("ENUMERATION CONFIGURATION");
					EditorStyles.label.fontStyle = FontStyle.Normal;

					theObject.Name = EditorGUILayout.TextField("Enum Name: ", theObject.Name);

					EditorGUILayout.Separator();
					EditorGUILayout.Space();


					// ADD A NEW PROPERTY
					// -- CONTENT
					EditorStyles.label.fontStyle = FontStyle.Bold;
					EditorGUILayout.LabelField("ADD ENUM MEMBER");
					EditorStyles.label.fontStyle = FontStyle.Normal;

					EditorGUILayout.BeginVertical();

					// ---- FIELD NAME
					_strNewVar = EditorGUILayout.TextField("Member Name: ", _strNewVar);
					_strNewVar = Regex.Replace(_strNewVar, @"[^a-zA-Z0-9_]", "");

					// ---- INT VALUE (IF APPLICABLE)
					_intNewValue = EditorGUILayout.IntField("Int Value: ", _intNewValue);
												 EditorGUILayout.LabelField(" ", "(-1 = auto-numbering)");

					// ---- ADD BUTTON
					if (GUILayout.Button("ADD", GUILayout.Width(75)))
					{
						EnumBuilder.EnumProperty prop = new EnumBuilder.EnumProperty();
						prop.Name						= _strNewVar;
						prop.IntValue				= _intNewValue;
						theObject.AddProperty(prop);
						_strNewVar					= "";
						_intNewValue				= -1;
						_intSelected				= -1;
						GUI.FocusControl(null);
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.Separator();
					EditorGUILayout.Space();
			
					// LIST CLASS PROPERTIES
					// -- HEADER
					EditorStyles.label.fontStyle = FontStyle.Bold;
					EditorGUILayout.LabelField("CURRENT ENUM MEMBERS");
					EditorStyles.label.fontStyle = FontStyle.Normal;

					// -- CONTENT
					EditorGUILayout.BeginVertical();
					for (int i = 0; i < theObject.Variables.Count; i++)
					{
						if (!theObject.Variables[i].IsDeleted)
						{ 
							EditorGUILayout.BeginHorizontal();
							if (i == 0)
							GUILayout.Button(" ", GUILayout.Width(20), GUILayout.MaxWidth(20));
							else
							if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.MaxWidth(20)) && i > 0)
								if (EditorUtility.DisplayDialog("Delete this Item?", "Are you sure that you want to delete \"" + theObject.Variables[i].Name + "\"?", "Delete", "Cancel"))
									theObject.Variables[i].IsDeleted = true;

							EditorGUILayout.LabelField(theObject.Variables[i].Name, GUILayout.Width(150));
							EditorGUILayout.LabelField(theObject.Variables[i].IntValue.ToString(), GUILayout.Width(30));

							EditorGUILayout.EndHorizontal();
						}
					}
					EditorGUILayout.EndVertical();

					// CREATE BUTTON
					EditorGUILayout.Space();

					if (_blnNote)
						EditorGUILayout.LabelField("Enum has been successfully Generated.");
					
					if (GUI.changed)
						_blnNote = false;

				}
				
				EditorGUILayout.EndScrollView();

				GUILayout.EndVertical();

				GUILayout.Space(10);

				GUILayout.BeginHorizontal();
				if (GUILayout.Button("SAVE"))
				{
					if (selected != null && selected.Name.Trim() != "")
					{
						((BaseDatabase<EnumBuilder>)(object)editorDB).Save(theObject);
						selected = null;
						_blnNote = true;
						GUI.FocusControl("");
					}
				}

				DisplayEditorBottom();
			}
			protected override	void							DisplayCount()
			{
				GUILayout.BeginHorizontal("Box", GUILayout.ExpandWidth(true));

				GUILayout.Label("Record Count: " + ((BaseDatabase<EnumBuilder>)(object)editorDB).Count.ToString() + " - (Hold CTRL to Delete a Record)");
				if (selected == null && GUILayout.Button("Add New"))
						selected = (EnumBuilder)(object)new EnumBuilder();

				GUILayout.EndHorizontal();
			}

		#endregion

	}
}
