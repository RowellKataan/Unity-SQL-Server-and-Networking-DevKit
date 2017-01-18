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

	#region "PUBLIC EDITOR PROPERTIES"

		public	GameObject					WorldObject;

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

}
