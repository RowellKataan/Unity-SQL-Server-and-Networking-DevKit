


#undef IS_DEBUGGING

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class TabNavigation : MonoBehaviour
{

	#region "PRIVATE VARIABLES"

		private bool						_blnInitialized	= false;

	#endregion

	#region "PUBLIC PROPERTIES"

		public enum DirectionEnum
		{
			UpDown,
			LeftRight,
		}

		public	DirectionEnum		Direction	= DirectionEnum.UpDown;
		public	Selectable			Default;
		public	Button					DefaultButton;
		public	bool						AutoFocus	= true;

	#endregion

	#region "START FUNCTION"

		private void	Start()
		{
			StartCoroutine(DoStart());
		}
		public	IEnumerator		DoStart()
		{
			yield return new WaitForSeconds(0.1f);
			if (AutoFocus && Default is InputField)
			{
				EventSystem system = EventSystem.current;
				InputField inputfield = (InputField)Default;
				if (inputfield.interactable)
				{ 
					inputfield.OnPointerClick(new PointerEventData(system));
					system.SetSelectedGameObject(inputfield.gameObject);
//				inputfield.ActivateInputField();
				}
				_blnInitialized = true;
			}
		}

	#endregion

	#region "UPDATE FUNCTION"

		private void	LateUpdate()
		{
			if (!gameObject.activeSelf)
				return;

			if (Input.GetKeyUp(KeyCode.Tab))
			{
				if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
					SelectPrevious();
				else
					SelectNext();
			} else if ((Input.GetKeyUp(KeyCode.Return) || Input.GetKeyUp(KeyCode.KeypadEnter)) && DefaultButton != null) {
				#if IS_DEBUGGING
				Debug.Log("Invoking OnClick for " + DefaultButton.name);
				#endif
				DefaultButton.Select();
				DefaultButton.onClick.Invoke();
			}
		}

	#endregion

	#region "PRIVATE FUNCTIONS"

		Selectable Next(Selectable current)
		{
			switch (Direction)
			{
				case DirectionEnum.UpDown:
							return current.FindSelectableOnDown();
				case DirectionEnum.LeftRight:
							return current.FindSelectableOnRight();
			}
			return current.FindSelectableOnDown();
		}
		Selectable Previous(Selectable current)
		{
			switch (Direction)
			{
				case DirectionEnum.UpDown:
							return current.FindSelectableOnUp();
				case DirectionEnum.LeftRight:
							return current.FindSelectableOnLeft();
			}
			return current.FindSelectableOnUp();
		}

	#endregion

	#region "PUBLIC FUNCTIONS"

		public	void			SelectNext()
		{
			EventSystem	system	= EventSystem.current;
			Selectable	next		= null;
			
			try { next = Next(system.currentSelectedGameObject.GetComponent<Selectable>()); } catch { next = null; }

			if (next == null)
				next = Default;

			if (next != null)
			{
				InputField inputfield = next.GetComponent<InputField>();
				if (inputfield != null && inputfield.interactable)
				{
					inputfield.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret
					system.SetSelectedGameObject(next.gameObject);
					inputfield.Select();
					inputfield.ActivateInputField();
					inputfield.MoveTextStart(false);
					inputfield.MoveTextEnd(true);
				}
			}
		}
		public	void			SelectPrevious()
		{
			EventSystem system	= EventSystem.current;
			Selectable	prev		= null;
			
			try { prev = Previous(system.currentSelectedGameObject.GetComponent<Selectable>()); } catch { prev = null; }

			if (prev == null)
				prev = Default;

			if (prev != null)
			{
				InputField inputfield = prev.GetComponent<InputField>();
				if (inputfield != null && inputfield.interactable)
				{
					inputfield.OnPointerClick(new PointerEventData(system));  //if it's an input field, also set the text caret
					system.SetSelectedGameObject(prev.gameObject);
					inputfield.Select();
					inputfield.ActivateInputField();
					inputfield.MoveTextStart(false);
					inputfield.MoveTextEnd(true);
				}
			}
		}

	#endregion

}
