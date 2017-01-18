//		AUTO-GENERATED FILE: UserManagerEditor.cs
//		GENERATED ON       : Wed Jan 18 2017 - 12:41:34 PM
//		
//		This is the Editor for the Class Manager file.  This is the file that you can modify.
//		It will not automatically be changed by the system going forward.


using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using UnityEditor.SceneManagement;
using System.Collections;

[CustomEditor(typeof(UserManager))]
public class UserManagerEditor : Editor
{

	private int		intSelected = 0;
	private bool	blnSave			= false;
	private Vector4[] v4			= new Vector4[14];
	private Vector4[] v4Temp	= new Vector4[14];

	public override void OnInspectorGUI()
	{
		UserManager	myTarget = null;
		try { myTarget = (UserManager)target; } catch { }

		if (myTarget != null)
		{
			if (myTarget.Users.Count > 0)
			{
				User selected	= myTarget.Users[intSelected];
				Quaternion q3 = Quaternion.identity;
				EditorStyles.label.richText = true;
				EditorGUILayout.LabelField(" ", (intSelected + 1).ToString() + " / " + myTarget.Users.Count.ToString());
				EditorStyles.label.richText = false;
				EditorGUILayout.Separator();
				EditorGUILayout.Space();
				GUI.changed = false;

				EditorGUILayout.IntField("UserID", selected.UserID);
				EditorGUILayout.TextField("DateCreated", selected.DateCreated.ToString("MM/dd/yyyy HH:mm:ss"));
				EditorGUILayout.TextField("DateUpdated", selected.DateUpdated.ToString("MM/dd/yyyy HH:mm:ss"));
				selected.IsActive	= EditorGUILayout.Toggle("IsActive", selected.IsActive);
				selected.Username	= EditorGUILayout.TextField("Username", selected.Username);
				selected.Password	= EditorGUILayout.TextField("Password", selected.Password);
				selected.FirstName	= EditorGUILayout.TextField("FirstName", selected.FirstName);
				selected.LastName	= EditorGUILayout.TextField("LastName", selected.LastName);
				selected.EmailAddress	= EditorGUILayout.TextField("EmailAddress", selected.EmailAddress);
				EditorGUILayout.LabelField("UserType", selected.UserType.ToString());
				selected.Warnings	= EditorGUILayout.IntField("Warnings", selected.Warnings);
				EditorGUILayout.TextField("BanDate", selected.BanDate.ToString("MM/dd/yyyy HH:mm:ss"));
				selected.BanDays	= EditorGUILayout.IntField("BanDays", selected.BanDays);
				selected.ConnectionCount	= EditorGUILayout.IntField("ConnectionCount", selected.ConnectionCount);
				EditorGUILayout.Separator();
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("<--"))
				{
					if (intSelected > 0)
						intSelected--;
					else
						intSelected	= myTarget.Users.Count - 1;
					selected			= myTarget.Users[intSelected];
					blnSave				= false;
					GUI.changed		= false;

					GUI.SetNextControlName("");
					GUI.FocusControl ("");
				}
				if (GUILayout.Button("-->"))
				{
					if (intSelected < myTarget.Users.Count - 1)
						intSelected++;
					else
						intSelected	= 0;
					selected			= myTarget.Users[intSelected];
					blnSave				= false;
					GUI.changed		= false;

					GUI.SetNextControlName("");
					GUI.FocusControl ("");
				}
				EditorGUILayout.EndHorizontal();

				if (GUI.changed)
				{
					EditorUtility.SetDirty(myTarget);
					if (!Application.isPlaying)
						EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
					blnSave = true;
				}
				if (blnSave && GUILayout.Button("SAVE"))
				{
					GUI.SetNextControlName("");
					GUI.FocusControl ("");
					selected.Save();
					blnSave = false;
				}
			} else {
				EditorGUILayout.LabelField("There are No Records.");
			}
		}
	}
}

