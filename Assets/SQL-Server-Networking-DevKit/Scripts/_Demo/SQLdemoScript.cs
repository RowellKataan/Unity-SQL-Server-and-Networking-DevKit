// ===========================================================================================================
//
// Class/Library: Demo Script - Shows how to access the SQL Database
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Mar 26, 2016
//	
// VERS 1.0.000 : Mar 26, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SQLdemoScript : MonoBehaviour 
{

	#region "PRIVATE PROPERTIES"

		private DatabaseManager		_dbm		= null;
		private DatabaseManager		Database
		{
			get
			{
				if (_dbm == null)
						_dbm = DatabaseManager.Instance;
				return _dbm;
			}
		}

		private GameObject				_canvas	= null;
		private GameObject				Canvas
		{
			get
			{
				if (_canvas == null)
						_canvas = GameObject.Find("Canvas");
				return _canvas;
			}
		}

		private string						ResultText
		{
			get
			{
				return Canvas.transform.GetChild(3).GetComponent<Text>().text;
			}
			set
			{
				Canvas.transform.GetChild(3).GetComponent<Text>().text = value.Trim();
			}
		}

	#endregion

	#region "START FUNCTIONS"

		private void					Start()
		{
			StartCoroutine(DoStart());
		}
		private IEnumerator		DoStart()
		{
			// WE'RE GOING TO TEST IF WE CAN CONNECT TO THE DATABASE BY OPENING A CONNECTION THEN CLOSE IT
			int i = 0;
			Util.Timer clock = new Util.Timer();
			clock.StartTimer();
			while (!Database.IsConnected && clock.GetTime < 4)
			{
				yield return null;
				i++;
			}
			ResultText = "Database Connected = " + Database.IsConnectedCheck.ToString() + " (After " + clock.GetTime + " seconds, " + i.ToString() + ")";
			if (!Database.KeepConnectionOpen)
					Database.CloseDatabase();
			clock.StopTimer();
		}

	#endregion

	#region "PUBLIC BUTTON FUNCTIONS"

		public	void							PressTestButton1()
		{
			// MAKE SURE THE SQL SERVER IS CONNECTED
			// (IF IT ISN'T, A CONNECTION WILL BE OPENED)
			if (Database.DAL.IsConnected)
			{ 
				// QUERY THE DATABASE -- COUNT THE NUMBER OF RECORDS IN OUR TEST TABLE
				Database.DAL.ClearParams();
				int i = Database.DAL.GetSQLSelectInt("SELECT COUNT(*) FROM tblTest");

				// DISPLAY THE RESULTS (OR ERRORS) 
				ResultText = (Database.DAL.Errors == "") ? "Success!  " : "Failed!  ";
				if (Database.DAL.Errors == "")
					ResultText += "There are " + i.ToString() + " Records.";
				else
					ResultText += "\n" + Database.DAL.SQLqueries + "\n\n" + Database.DAL.Errors;
			} else
				ResultText = "Not Connected to the Database.";
		}
		public	void							PressTestButton2()
		{
			// MAKE SURE THE SQL SERVER IS CONNECTED
			// (IF IT ISN'T, A CONNECTION WILL BE OPENED)
			if (Database.DAL.IsConnected)
			{ 
				// QUERY THE DATABASE -- COUNT THE NUMBER OF RECORDS IN OUR TEST TABLE
				Database.DAL.ClearParams();
				int i = Database.DAL.GetSQLSelectInt("SELECT COUNT(*) FROM tblTest WHERE ID < 3");

				// DISPLAY THE RESULTS (OR ERRORS) 
				ResultText = (Database.DAL.Errors == "") ? "Success!  " : "Failed!  ";
				if (Database.DAL.Errors == "")
					ResultText += "There are " + i.ToString() + " Records.";
				else
					ResultText += "\n" + Database.DAL.SQLqueries + "\n\n" + Database.DAL.Errors;
			} else
				ResultText = "Not Connected to the Database.";
		}

	#endregion

}
