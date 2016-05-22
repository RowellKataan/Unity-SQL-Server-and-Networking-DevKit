// ===========================================================================================================
//
// Class/Library: PanelManager (Singleton Class)
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Apr 29, 2016
//	
// VERS 1.0.000 : Apr 29, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

#if UNITY_EDITOR
	#define		IS_DEBUGGING
#else
	#undef		IS_DEBUGGING
#endif

using UnityEngine;
using System.Collections;

public class PanelManager : MonoBehaviour 
{

	#region "PRIVATE VARIABLES"

		private	static	PanelManager		_instance					= null;

	#endregion	

	#region "PRIVATE PROPERTIES"

		private GameObject			theConnectPanel
		{
			get
			{
				if (ConnectPanelObject == null)
						ConnectPanelObject = (GameObject) GameObject.FindObjectOfType(typeof(NetworkConnectPanel));
				return ConnectPanelObject;
			}
		}
		private GameObject			theMatchMakingPanel
		{
			get
			{
				if (MatchMakingPanelObject == null)
						MatchMakingPanelObject = (GameObject) GameObject.FindObjectOfType(typeof(MatchMakingPanel));
				return MatchMakingPanelObject;
			}
		}
		private GameObject			theLogInPanel
		{
			get
			{
				if (LogInPanelObject == null)
						LogInPanelObject = (GameObject) GameObject.FindObjectOfType(typeof(LogInPanel));
				return LogInPanelObject;
			}
		}
		private GameObject			theLoadingPanel
		{
			get
			{
				if (LoadingPanelObject == null)
						LoadingPanelObject = (GameObject) GameObject.FindObjectOfType(typeof(LoadingPanel));
				return LoadingPanelObject;
			}
		}

	#endregion

	#region "PUBLIC PROPERTIES"

		public	static	PanelManager		Instance
		{
			get
			{
				return GetInstance();
			}
		}
		public	static	PanelManager		GetInstance()
		{
			if (_instance == null)
					_instance = (PanelManager)GameObject.FindObjectOfType(typeof(PanelManager));
			return _instance;
		}

	#endregion

	#region "PUBLIC EDITOR PROPERTIES"

		[SerializeField]
		public	GameObject			ConnectPanelObject			= null;
		[SerializeField]
		public	GameObject			MatchMakingPanelObject	= null;
		[SerializeField]
		public	GameObject			LogInPanelObject				= null;
		[SerializeField]
		public	GameObject			LoadingPanelObject			= null;

	#endregion

	#region "PRIVATE VARIABLES"

		private	void		HideAll()
		{
			if (theConnectPanel != null)
					theConnectPanel.SetActive(false);
			if (theMatchMakingPanel != null)
					theMatchMakingPanel.SetActive(false);
			if (theLogInPanel != null)
					theLogInPanel.SetActive(false);
			if (theLoadingPanel != null)
					theLoadingPanel.SetActive(false);
		}

	#endregion

	#region "PUBLIC PROPERTIES"

		public	void		ShowConnectPanel()
		{
			HideAll();
			if (theConnectPanel != null)
					theConnectPanel.SetActive(true);
		}
		public	void		ShowMatchMakingPanel()
		{
			HideAll();
			if (theMatchMakingPanel != null)
					theMatchMakingPanel.SetActive(true);
		}
		public	void		ShowLogInPanel()
		{
			HideAll();
			if (theLogInPanel != null)
					theLogInPanel.SetActive(true);
		}
		public	void		ShowLoadingPanel()
		{
			HideAll();
			if (theLoadingPanel != null)
					theLoadingPanel.SetActive(true);
		}
		public	void		ShowGame()
		{
			HideAll();
		}

	#endregion

}
