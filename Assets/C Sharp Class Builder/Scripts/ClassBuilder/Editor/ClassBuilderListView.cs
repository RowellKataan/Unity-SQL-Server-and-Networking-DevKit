// ===========================================================================================================
//
// Class/Library: Class Builder Unity Database ListView
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Mar 26, 2016
//	
// VERS 1.0.000 : Mar 26, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

using UnityEngine;
using UnityEditor;

namespace CBT
{ 
	public partial class ClassBuilderDatabaseEditor : BaseDatabaseEditor<ClassBuilderDatabaseEditor, ClassBuilderDatabase, ClassBuilder>
	{

		#region "PRIVATE FUNCTIONS"
	
			protected	override	void	DisplayList()
			{
				for (int i = 0; i < editorDB.Count; i++)
				{
						GUILayout.BeginHorizontal("Box");
						bool blnDel = (Event.current.control); 
						bool blnRep = (blnDel != _blnSaveCtrlKey);
						_blnSaveCtrlKey	= blnDel;

						if (!blnDel)
						{
							if (GUILayout.Button("?", GUILayout.Width(16), GUILayout.Height(16)))
							{
								GUI.FocusControl("");
								selected = new ClassBuilder(editorDB.GetByIndex(i));
							}
							if (blnRep)
									Repaint();
						} else {
							GUIStyle style = new GUIStyle(GUI.skin.button);
							style.normal.textColor = Color.yellow;
							if (GUILayout.Button("X", style, GUILayout.Width(16), GUILayout.Height(16)) && blnDel)
							{
								if (EditorUtility.DisplayDialog("Delete this Record?", "Are you sure that you want to delete \"" + editorDB.GetByIndex(i).Name + "\"?", "Delete", "Cancel"))
								{
									selected = new ClassBuilder(editorDB.GetByName(editorDB.database[i].Name));
									editorDB.Delete(selected.Index);
									GUI.FocusControl("");
									selected = null;
								}
							}
							if (blnRep)
									Repaint();
						}

						try 
						{
							string st = editorDB.GetByIndex(i).Name;
							if (GUILayout.Button(st, "Label", GUILayout.ExpandWidth(true)))
							{
								GUI.FocusControl("");
								selected = new ClassBuilder(editorDB.GetByIndex(i));
							}
							if (blnRep)
								Repaint();
						} catch {
							GUILayout.Label("No Class #" + i.ToString() + " of " + editorDB.Count.ToString()); 
						}

						GUILayout.EndHorizontal();
				}
			}

		#endregion

	}
}
