
#define USES_NETWORKMANAGER

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(ApplicationManager))]
public class ApplicationManagerEditor : Editor 
{

	#region "PRIVATE VARIABLES"

		private bool		blnShowGame				= true;
		private bool		blnShowUser				= false;
		private bool		blnShowPlayerObjs	= false;
		private int			intSaveCls				= 0;
		private int			intSaveDev				= 0;

	#endregion

	#region "PRIVATE FUNCTIONS"

		private	void		IncreaseVersion(ApplicationManager app,  bool blnMajor, bool blnMinor, bool blnBuild)
		{
			string[]	strVer = app.GameVersion.Split('.');
			int[]			intVer = new int[strVer.Length];

			for (int i = 0; i < strVer.Length; i++)
			{
				intVer[i] = Util.ConvertToInt(strVer[i]);
			}

			// INCREASE MAJOR BUILD VERSION, RESET MINOR BUILD VERSION
			if (blnMajor)
			{
				intVer[0] = intVer[0]++;
				intVer[1] = 0;
			}

			// INCREASE MINOR VERSION
			if (blnMinor)
				intVer[1]++;

			// ALWAYS INCREASE BUILD
			intVer[2]++;

			app.GameVersion = intVer[0].ToString() + "." + intVer[1].ToString().PadLeft(2, '0') + "." + intVer[2].ToString().PadLeft(5, '0');
		}

	#endregion

	public override void OnInspectorGUI()
	{
		ApplicationManager myTarget = null;
		try { myTarget = (ApplicationManager)target; } catch { }

		// DECLARE A STYLE
		GUIStyle centerStyle = new GUIStyle();
		centerStyle.alignment = TextAnchor.MiddleCenter;
		centerStyle.normal.textColor = Color.white;

		if (myTarget != null)
		{
			GUI.changed = false;

			blnShowGame = EditorGUILayout.Foldout(blnShowGame, "GAME INFORMATION");
			if (blnShowGame)
			{
				string[] strBldType					= { "Experimental", "Development", "Internal Alpha", "Closed Alpha", "Closed Beta", "Open Beta", "Testing", "Release" };
				string[] strAppSec					= System.Enum.GetNames(typeof(ApplicationManager.Classifications));
				myTarget.GameName						= EditorGUILayout.TextField("Application Name",	myTarget.GameName);
				myTarget.GameCode						= EditorGUILayout.TextField("Game Code",	myTarget.GameCode);
				myTarget.AppClassification = (ApplicationManager.Classifications)EditorGUILayout.Popup("Classification", (int)myTarget.AppClassification, strAppSec);
				myTarget.GameBuildType			= EditorGUILayout.Popup(		"Build Type",	myTarget.GameBuildType - 1, strBldType) + 1;
				myTarget.GameVersion				= EditorGUILayout.TextField("Version",		myTarget.GameVersion);

				GUILayout.Label("--- INCREASE VERSION ---", centerStyle);
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Major"))
					IncreaseVersion(myTarget, true, false, false);
				if (GUILayout.Button("Minor"))
					IncreaseVersion(myTarget, false, true, false);
				if (GUILayout.Button("Build"))
					IncreaseVersion(myTarget, false, false, true);
				EditorGUILayout.EndHorizontal();

				myTarget.MaxFPS							= EditorGUILayout.IntField("Max FPS", myTarget.MaxFPS);
				myTarget.MaxFPS							= Mathf.Clamp(myTarget.MaxFPS, 10, 240);
				myTarget.GameObjectPrefab		= (GameObject)EditorGUILayout.ObjectField("Game Prefab", myTarget.GameObjectPrefab, typeof(GameObject), true);

				if (intSaveCls != (int) myTarget.AppClassification || intSaveDev != myTarget.GameBuildType)
				{
					intSaveCls = (int) myTarget.AppClassification;
					intSaveDev = myTarget.GameBuildType;
//				GUImanager.Instance.SetLogInBackgroundImage(myTarget.ApplicationClassifiedBackground);
				}
			}

			blnShowUser = EditorGUILayout.Foldout(blnShowUser, "USER LOG-IN INFORMATION");
			if (blnShowUser)
			{
				string[] strUsrOpt = { "Username-Password", "Windows Username" } ;
				myTarget.UserLogInType			= EditorGUILayout.Popup( "Log In Mode", myTarget.UserLogInType - 1, strUsrOpt) + 1;
				myTarget.AllowSignUp				= myTarget.UserLogInType == 1 && EditorGUILayout.Toggle("Allow User Sign Up?",	myTarget.AllowSignUp);
				myTarget.AutoCreateAccount	= myTarget.UserLogInType == 2 && EditorGUILayout.Toggle("Auto-Create Account?", myTarget.AutoCreateAccount);
				myTarget.CanWorkOffline			= DatabaseManager.Instance.ClientsCanUse && EditorGUILayout.Toggle("Allow Offline Use?", myTarget.CanWorkOffline);
			}

			blnShowPlayerObjs = EditorGUILayout.Foldout(blnShowPlayerObjs, "WORLD/PLAYER/UI OBJECTS");
			if (blnShowPlayerObjs)
			{
				myTarget.PlayerPrefab			= (GameObject)EditorGUILayout.ObjectField("Player Prefab",			myTarget.PlayerPrefab,		typeof(GameObject), true);
				#if USES_NETWORKMANAGER
				if (!Application.isPlaying && AppNetworkManager.Instance != null)
				AppNetworkManager.Instance.playerPrefab = myTarget.PlayerPrefab;
				#endif
			}

			if (GUI.changed)
				EditorUtility.SetDirty(myTarget);

		}
	}

}
