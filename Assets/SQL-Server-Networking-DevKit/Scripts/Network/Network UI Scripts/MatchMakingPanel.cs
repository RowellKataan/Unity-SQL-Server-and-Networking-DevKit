// ===========================================================================================================
//
// Class/Library: Network MatchMaking Panel (UI)
//        Author: Michael Marzilli   ( http://www.linkedin.com/in/michaelmarzilli )
//       Created: Apr 28, 2016
//	
// VERS 1.0.000 : Apr 28, 2016 : Original File Created. Released for Unity 3D.
//
// ===========================================================================================================

#if UNITY_EDITOR
	#define		IS_DEBUGGING
#else
	#undef		IS_DEBUGGING
#endif

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using System.Collections;

public class MatchMakingPanel : MonoBehaviour 
{

	#region "PRIVATE PROPERTIES"

		private AppNetworkManager		_nwm			= null;
		private AppNetworkManager		Net
		{
			get
			{
				if (_nwm == null)
						_nwm = AppNetworkManager.Instance;
				return _nwm;
			}
		}

		private GameObject					ListContainer
		{
			get
			{
				return transform.GetChild(0).GetChild(1).GetChild(0).gameObject;
			}
		}
		private string							ServerNameInput
		{
			get
			{
				return transform.GetChild(0).GetChild(5).GetComponent<InputField>().text;
			}
			set
			{
				transform.GetChild(0).GetChild(5).GetComponent<InputField>().text = value.Trim();
			}
		}
		private string							ServerPasswordInput
		{
			get
			{
				return transform.GetChild(0).GetChild(6).GetComponent<InputField>().text;
			}
			set
			{
				transform.GetChild(0).GetChild(6).GetComponent<InputField>().text = value.Trim();
			}
		}
		private GameObject					StartHostButton
		{
			get
			{
				return transform.GetChild(0).GetChild(7).gameObject;
			}
		}
		private string							ServerIPaddress
		{
			get
			{
				return transform.GetChild(0).GetChild(11).GetComponent<InputField>().text;
			}
			set
			{
				transform.GetChild(0).GetChild(11).GetComponent<InputField>().text = value.Trim();
			}
		}
		private int									ServerPort
		{
			get
			{
				return Util.ConvertToInt(transform.GetChild(0).GetChild(12).GetComponent<InputField>().text);
			}
			set
			{
				transform.GetChild(0).GetChild(12).GetComponent<InputField>().text = value.ToString();
			}
		}

	#endregion

	#region "PUBLIC EDITOR PROPERTIES"

		[SerializeField]
		public	GameObject		ServerInformationUIprefab		= null;

	#endregion

	#region "PRIVATE FUNCTIONS"

		private	void					OnEnable()
		{
			StartCoroutine(GetServerList());
		}
		private void					Update()
		{
			StartHostButton.GetComponent<Button>().interactable = !Net.IsConnected;
//		if (Net.IsConnected)
//			PanelManager.Instance.ShowLogInPanel();
		}

		private IEnumerator		GetServerList(float fDelay = 0.2f)
		{
			yield return new WaitForSeconds(fDelay);
			int i = 0;
			while (!Net.IsMatchMakingConnected && i < 100)
			{
				i++;
				yield return null;
			}
/*
			if (i > 99)
			{
				Net.StartMatchMaking();
				#if IS_DEBUGGING
				Debug.LogError("MatchMaking is not Connected");
				#endif
			}
			Net.GetHostList();
*/
		}

	#endregion

	#region "PUBLIC FUNCTIONS"

		public	void					ClearServerList()
		{
			if (ListContainer.transform.childCount > 0)
				for (int i = ListContainer.transform.childCount - 1; i >= 0; i--)
				{
					try { DestroyImmediate(ListContainer.transform.GetChild(i).gameObject); } catch { }
				}
		}
		public	void					AddToServerList(string strServerName, NetworkID networkID, bool blnPrivate, int intMax, int intCur)
		{
			int c = ListContainer.transform.childCount;

			// CREATE THE NEW SERVER INFORMATION UI PREFAB.  POPULATE IT WITH THE SERVER INFORMATION
			GameObject		go			= (GameObject) Instantiate(ServerInformationUIprefab);
			ServerInfoUI	sui			= go.GetComponent<ServerInfoUI>();
			sui.ServerName				= strServerName;
			sui.ServerNetworkID		= (ulong)networkID;
			sui.PasswordRequired	= blnPrivate;
			sui.MaxPlayers				= intMax;
			sui.CurPlayers				= intCur;

			// ADD THE UI ELEMENT TO THE LIST CONTAINER. PLACE IT IN THE RIGHT Y-POSITION PLACE.
			go.name = strServerName;
			go.transform.SetParent(ListContainer.transform);
			Vector3 v3 = Vector3.zero;
			v3.x = 0;
			v3.y = (c * 83) + 3;
			go.GetComponent<RectTransform>().localPosition = v3;

			// RESCALE THE LISTCONTAINER TO ENCOMPASS ALL THE SERVER ELEMENTS
			v3 = ListContainer.transform.GetComponent<RectTransform>().sizeDelta;
			v3.y = ((c + 1) * 86) + 3;
			ListContainer.transform.GetComponent<RectTransform>().sizeDelta = v3;
		}

	#endregion

	#region "BUTTON FUNCTIONS"

		public	void					CreateServerButtonOnClick()
		{
//		if (ServerNameInput != "")
//			Net.StartMatchHost(ServerNameInput, ServerPasswordInput);
		}
		public	void					ReListServersButtonOnClick()
		{
			StartCoroutine(GetServerList());
		}
		public	void					ManualServerConnectButtonOnClick()
		{
			if (ServerIPaddress != "" && ServerPort > 0)
			{
				Net.StopMatchMaker();
				Net.UsesMatchMaking	= false;
				Net.ForceHostMode		= false;
				Net.ServerIPaddress	= this.ServerIPaddress;
				Net.ServerPort			= this.ServerPort;
				Net.ClientConnect();
				PanelManager.Instance.ShowConnectPanel();
			}
		}

	#endregion

}
