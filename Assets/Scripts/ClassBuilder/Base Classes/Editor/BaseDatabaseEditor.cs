// ===========================================================================================================
//
// Class/Library: Unity Database Editor Base
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Mar 26, 2016
//	
// VERS 1.0.000 : Mar 26, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

using UnityEngine;
using UnityEditor;
using System.Collections;

namespace CBT
{ 

	public partial class BaseDatabaseEditor<T, D, R> : EditorWindow where T: ScriptableObject where D: ScriptableObject where R: class
	{

		#region "PRIVATE/PROTECTED CONSTANTS"

			protected const		int				MINIMUM_WIDTH						= 270;
			protected const		int				MINIMUM_HEIGHT					= 300;
			protected	const		string		DATABASE_FILE_DIRECTORY	= @"Scripts/ClassBuilder/Database";
			protected					string		DATABASE_FILE_NAME			= @"XXX.asset";

		#endregion

		#region "PRIVATE VARIABLES"

			protected	D											editorDB							= null;
			protected	R											selected							= null;
			protected	bool									_blnIsInitializing		= false;
			private		bool									_blnIsInitialized			= false;
			protected	bool									_blnNote							= false;

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
			}

		#endregion

		#region "PRIVATE FUNCTIONS"

			protected virtual	void		DisplayEditor()
			{
				DisplayEditorTop();
/*
				BaseDescriptor theObject = ((BaseDescriptor)(object)selected);

				if (editorDB is BaseDatabase<BaseDescriptor>	&& selected is BaseDescriptor)
				{
																			EditorGUILayout.LabelField(						"ID: ",				theObject.ID.ToString());
					theObject.Name						= EditorGUILayout.TextField(						"Name: ",			theObject.Name);
					theObject.Icon						= (Sprite) EditorGUILayout.ObjectField(	"Icon: ",			theObject.Icon, typeof(Sprite));
																			EditorGUILayout.TextField(						"Path: ",			theObject.IconPath);
					theObject.DescColor				= (Color)	 EditorGUILayout.ColorField(	"Color: ",		theObject.DescColor);
					theObject.Modifier				= EditorGUILayout.FloatField(						"Modifier: ",	theObject.Modifier);

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
			protected virtual void		DisplayCount()
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

			protected					void		DisplayEditorTop()
			{
				GUILayout.BeginVertical();
				GUILayout.BeginVertical(GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
			}
			protected					void		DisplayEditorBottom()
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
