using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class CharacterMovement : NetworkBehaviour 
{

	#region "PRIVATE VARIABLES"

		private float			fMovementSpeed	= 10.0f;
		private float			fRotationSpeed	= 100.0f;
		private bool			blnColliding		= false;
		private Rigidbody	rb							= null;

	#endregion

	#region "PRIVATE PROPERTIES"

		private ApplicationManager		_app							= null;
		private AppNetworkManager			_net							= null;

		private ApplicationManager		App
		{
			get
			{
				if (_app == null)
						_app = ApplicationManager.Instance;
				return _app;
			}
		}
		private AppNetworkManager			Net
		{
			get
			{
				if (_net == null)
						_net = AppNetworkManager.Instance;
				return _net;
			}
		}

		private bool									IsLocalPlayer
		{
			get
			{
				return isLocalPlayer || App.IsWorkingOffline;
			}
		}

	#endregion

	#region "PUBLIC EDITOR PROPERTIES"

		public	GameObject			PlayerModel;

	#endregion

	#region "START FUNCTION"

		private void			Start () 
		{
			GetComponent<CharacterMovement>().enabled = IsLocalPlayer;
			if (!IsLocalPlayer)
			{
				transform.GetChild(0).GetComponent<MeshRenderer>().material.color = Color.red; 
				return;
			}

			if (GetComponent<Rigidbody>() != null)
				rb = GetComponent<Rigidbody>();

			// ATTACH THE CAMERA TO THE PLAYER OBJECT
			Transform cam = Camera.main.gameObject.transform;
			cam.SetParent(this.transform);
			cam.localPosition = new Vector3(0, 3, -4);
			cam.localEulerAngles = new Vector3(10, 0, 0);
		}

	#endregion

	#region "UPDATE FUNCTION"

		private void			Update()
		{
			if (!IsLocalPlayer)
					return;
			if (!App.IsLoggedIn && !Net.IsHost)
					return;

			float translation	= CrossPlatformInputManager.GetAxis("Vertical")		* fMovementSpeed * ((Input.GetKey(KeyCode.LeftShift)) ? 2 : 1);
			float rotation		= CrossPlatformInputManager.GetAxis("Horizontal")	* fRotationSpeed;

			translation	*= Time.deltaTime;
			rotation		*= Time.deltaTime;

			if (rb != null)
			{
				rb.isKinematic = true;
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
				rb.isKinematic = false;
			}

			transform.Translate(0, 0, translation);
			transform.Rotate(		0, rotation, 0);

			Quaternion trot = transform.rotation;
			trot[0] = 0; //null rotation X
			trot[2] = 0; //null rotation Z
			transform.rotation = trot;
		}

	#endregion

	#region "EVENT FUNCTIONS"

		public	override	void	OnStartLocalPlayer()
		{
			if (PlayerModel == null)
					PlayerModel = transform.GetChild(0).gameObject;
			PlayerModel.GetComponent<MeshRenderer>().material.color = Color.yellow;
			App.UserObj = this.gameObject;
			if (Net.IsClient)
			{

			}
		}

		private						void	OnCollisionEnter(	Collision collision)
		{
			if (collision.gameObject.tag.ToLower() == "ground")
					return;
//		Debug.Log("Collision Enter " + collision.gameObject.name);
			blnColliding = true;
		}
		private						void	OnCollisionExit(	Collision collision)
		{
//		Debug.Log("Collision Exit " + collision.gameObject.name);
			blnColliding = false;
		}
		private						void	OnTriggerEnter(		Collider collision)
		{
			if (collision.gameObject.tag.ToLower() == "ground")
					return;
//		Debug.Log("Collider Enter " + collision.gameObject.name);
			blnColliding = true;
		}
		private						void	OnTriggerExit(		Collider collision)
		{
//		Debug.Log("Collider Exit " + collision.gameObject.name);
			blnColliding = false;
		}
		private						void	OnControllerColliderHit(ControllerColliderHit hit)
		{
			if (hit.gameObject.tag.ToLower() == "ground")
					return;
//		Debug.Log("OnControllerColliderHit "  + hit.gameObject.name);
			blnColliding = true;
		}

	#endregion

}
