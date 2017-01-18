// ===========================================================================================================
//
// Class/Library: ApplicationManager (Singleton Class)
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Apr 21, 2016
//	
// VERS 1.0.000 : Apr 21, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

#if UNITY_EDITOR
	#define		IS_DEBUGGING
#else
	#undef		IS_DEBUGGING
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;

public partial class ApplicationManager : MonoBehaviour
{

	#region "PRIVATE VARIABLES"

		private System.DateTime		_dtTimeOutLast = System.DateTime.Now;

	#endregion

	#region "PUBLIC EDITOR PROPERTIES"

	public GameObject					WorldObject;

	#endregion

	#region "START FUNCTION"

		private void						GameStart()
		{
			// INSTANTIATE THE GAME OBJECT
/*
			GameObject	gu = (GameObject)GameObject.Instantiate(GameObjectPrefab, Vector3.zero, Quaternion.identity);
									gu.name = gu.name.Replace("(Clone)", "").Trim();
									gu.GetComponent<GameManager>().GameName = this.GameName;
*/
			// CREATE A USERMANAGER COMPONENT ON THIS OBJECT IF THIS IS THE SERVER
			if (!Net.IsServer)
				gameObject.AddComponent<UserManager>();
		}

	#endregion

	#region "UPDATE FUNCTION"

		private void						Update()
		{
			bool blnActed = false;

			// -----------------------------------------------------------------------------------------------------------------------
			// HANDLE USER KEY PRESSES	----------------------------------------------------------------------------------------------
			// -----------------------------------------------------------------------------------------------------------------------
			//	-- V: DISPLAY APPLICATION VERSION
			if (Input.GetKeyUp(KeyCode.V) &&
				 (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) )
			{
				blnActed = true;
				Status.Status = "Application Version: " + GameVersion + " "  + GameBuildName;
			}

			//	--	CTRL-D: DISPLAY WHICH DATABASE THE APPLICATION IS USING/CONNECTED TO
			if (Input.GetKeyUp(KeyCode.D) && GameUser != null && GameUser.IsAdmin &&
				 (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) )
			{
				blnActed = true;
				Status.Status = "Connected to Database: <b>" + Database.DBdatabase + "</b>";
			}

			//	--	CTRL-M: OPEN SEND SYSTEM MESSAGE DIALOG IN STATUS MANAGER
			if ( Input.GetKeyUp(KeyCode.M) && Net.IsConnected &&
					(Input.GetKey(KeyCode.RightControl)	|| Input.GetKey(KeyCode.LeftControl)) )
			{
				blnActed = true;
				Status.ShowStatusMessage(true);
			}

			//	--	ENTER:  (ONLY WHEN STATUS SEND MESSAGE PANEL IS OPEN)
			if ((Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter)) && Status.IsStatusMessageActive)
			{
				blnActed = true;
				Status.SendMessageButtonOnClick();
			}

			//	--	CTRL-ALT-C: GET COUNT OF NUMBER OF PLAYERS CONNECTED TO SERVER
			if ( Input.GetKeyDown(KeyCode.C) && 
					(Input.GetKey(KeyCode.RightControl)	|| Input.GetKey(KeyCode.LeftControl)) &&
					(Input.GetKey(KeyCode.RightAlt)			|| Input.GetKey(KeyCode.LeftAlt)) )
			{
				blnActed = true;
				GameUser.RequestPlayerCount();
			}

			//	--	CTRL-ALT-K: KICK ALL PLAYERS OFF
			if ( Input.GetKeyDown(KeyCode.K) && 
					(Input.GetKey(KeyCode.RightControl)	|| Input.GetKey(KeyCode.LeftControl)) &&
					(Input.GetKey(KeyCode.RightAlt)			|| Input.GetKey(KeyCode.LeftAlt)) )
			{
				blnActed = true;
				GameUser.RequestPlayerKick();
			}

			//	--	CTRL-ALT-K: KICK ALL PLAYERS OFF
			if ( Input.GetKeyDown(KeyCode.Q) && 
					(Input.GetKey(KeyCode.RightControl)	|| Input.GetKey(KeyCode.LeftControl)) &&
					(Input.GetKey(KeyCode.RightAlt)			|| Input.GetKey(KeyCode.LeftAlt)) )
			{
				blnActed = true;
				GameUser.RequestServerShutdown();
			}


			// -----------------------------------------------------------------------------------------------------------------------
			// HANDLE APPLICATION TIMEOUT
			// -----------------------------------------------------------------------------------------------------------------------
			if (blnActed)
				ResetAppTimer();

			if (Mathf.Abs(Util.DateDiff(Util.DateInterval.Second, _dtTimeOutLast, System.DateTime.Now)) == (DEFAULT_APP_TIMEOUT - 5) * 60)
				Status.Status = "ATTENTION! Application will shut down in 5 Minutes due to inactivity.";
			if (Mathf.Abs(Util.DateDiff(Util.DateInterval.Second, _dtTimeOutLast, System.DateTime.Now)) == (DEFAULT_APP_TIMEOUT - 1) * 60)
				Status.Status = "ATTENTION! Application will shut down in 1 Minute due to inactivity.";
			if (Mathf.Abs(Util.DateDiff(Util.DateInterval.Second, _dtTimeOutLast, System.DateTime.Now)) == (DEFAULT_APP_TIMEOUT * 60) - 10)
				Status.Status = "ATTENTION! Application will shut down in 10 Seconds due to inactivity.";
			if (Mathf.Abs(Util.DateDiff(Util.DateInterval.Second, _dtTimeOutLast, System.DateTime.Now)) >= DEFAULT_APP_TIMEOUT * 60)
			{
				Status.Status = "Application shutting down due to inactivity (" + DEFAULT_APP_TIMEOUT.ToString() + " Minutes).";
				QuitApplication();
			}
		}

	#endregion

	#region "PUBLIC FUNCTIONS"

		public	void						ResetAppTimer()
		{
			_dtTimeOutLast = System.DateTime.Now;
		}

	#endregion

}
