//		AUTO-GENERATED FILE: UserEditor.cs
//		GENERATED ON       : Wed Jan 18 2017 - 12:41:34 PM
//		
//		This is the Editor for the Class file.  This is the file that you can modify.
//		It will not automatically be changed by the system going forward.


using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(User))]
public class UserEditor : Editor
{

	private Vector4[] v4		 = new Vector4[14];
	private Vector4[] v4Temp = new Vector4[14];

	public override void OnInspectorGUI()
	{
		User	myTarget = null;
		try { myTarget = (User)target; } catch { }

		if (myTarget != null)
		{
			Quaternion q3 = Quaternion.identity;
			GUI.changed = false;

			EditorGUILayout.LabelField("Net ID", myTarget.NetID.ToString());
			EditorGUILayout.IntField("UserID", myTarget.UserID);
			EditorGUILayout.TextField("DateCreated", myTarget.DateCreated.ToString("MM/dd/yyyy HH:mm:ss"));
			EditorGUILayout.TextField("DateUpdated", myTarget.DateUpdated.ToString("MM/dd/yyyy HH:mm:ss"));
			myTarget.IsActive	= EditorGUILayout.Toggle("IsActive", myTarget.IsActive);
			myTarget.Username	= EditorGUILayout.TextField("Username", myTarget.Username);
			myTarget.Password	= EditorGUILayout.TextField("Password", myTarget.Password);
			myTarget.FirstName	= EditorGUILayout.TextField("FirstName", myTarget.FirstName);
			myTarget.LastName	= EditorGUILayout.TextField("LastName", myTarget.LastName);
			myTarget.EmailAddress	= EditorGUILayout.TextField("EmailAddress", myTarget.EmailAddress);
			EditorGUILayout.TextField("User Type", myTarget.UserType.ToString());
			myTarget.Warnings	= EditorGUILayout.IntField("Warnings", myTarget.Warnings);
			EditorGUILayout.TextField("BanDate", myTarget.BanDate.ToString("MM/dd/yyyy HH:mm:ss"));
			myTarget.BanDays	= EditorGUILayout.IntField("BanDays", myTarget.BanDays);
			myTarget.ConnectionCount	= EditorGUILayout.IntField("ConnectionCount", myTarget.ConnectionCount);

			if (GUI.changed)
				{
				EditorUtility.SetDirty(myTarget);
				if (!Application.isPlaying)
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}
		}
	}
}

