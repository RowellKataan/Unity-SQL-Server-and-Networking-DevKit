// ===========================================================================================================
//
// Class/Library: Unity Database ListView Base
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

		#region "PRIVATE CONSTANTS"

			protected const int MINIMUM_LIST_WIDTH = 200;

		#endregion

		#region "PRIVATE VARIABLES"

			private		Vector2		_v2ScrollPosition;
			protected	bool			_blnSaveCtrlKey				= false;

		#endregion

		#region "PRIVATE/PROTECTED FUNCTIONS"

			protected						void			ListView()
			{
				_v2ScrollPosition = EditorGUILayout.BeginScrollView(_v2ScrollPosition, GUILayout.ExpandHeight(true), GUILayout.Width(MINIMUM_LIST_WIDTH));
				DisplayList();
				EditorGUILayout.EndScrollView();
			}
			protected	virtual		void			DisplayList()
			{
/*
				if (editorDB is BaseDatabase<BaseDescriptor>)
				{
					BaseDatabase<BaseDescriptor> db = (BaseDatabase<BaseDescriptor>)(object)editorDB;
					for (int i = 0; i < db.Count; i++)
					{
						GUILayout.BeginHorizontal("Box");
						bool blnDel = (Event.current.control); 
						bool blnRep = (blnDel != _blnSaveCtrlKey);
						_blnSaveCtrlKey	= blnDel;

						if (blnDel)
						{
							GUIStyle style = new GUIStyle(GUI.skin.button);
							style.normal.textColor = Color.yellow;
							if (GUILayout.Button("X", style, GUILayout.Width(16), GUILayout.Height(16)) && blnDel)
							{
								if (EditorUtility.DisplayDialog("Delete this Record?", "Are you sure that you want to delete \"" + db.database[i].Name + "\"?", "Delete", "Cancel"))
								{
									((BaseDatabase<BaseDescriptor>)(object)editorDB).Delete(db.database[i]);
									GUI.FocusControl("");
									selected = null;
								}
								if (blnRep)
									Repaint();
							}
						} else {
							if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(16)))
							{
								GUI.FocusControl("");
//							selected = (R)(object) db.GetByName(db.database[i].Name);
								selected = (R)(object)(new BaseDescriptor(db.GetByName(db.database[i].Name)));
							}
							if (blnRep)
								Repaint();
						}

						try 
						{
							string st = db.database[i].Name;
							if (GUILayout.Button(st, "Label", GUILayout.ExpandWidth(true)))
							{
								GUI.FocusControl("");
								selected = (R)(object)(new BaseDescriptor(db.GetByName(db.database[i].Name)));
							}
							if (blnRep)
								Repaint();
						} catch { 
							GUILayout.Label("No Item #" + i.ToString() + " of " + db.Count.ToString() ); 
						}

						GUILayout.EndHorizontal();
					}
				}
*/
			}

		#endregion

	}
}
