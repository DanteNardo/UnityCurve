

/*******************************************/
/*                  INCLUDES               */
/*******************************************/
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/*******************************************/
/*                   CLASS                 */
/*******************************************/
namespace UnityCurve {
	/// <summary>
	/// 
	/// </summary>
	public class InputCurve : Curve {

		/***************************************/
		/*                ENUM               */
		/***************************************/
		public enum InputActionCallbackType {
			STARTED,
			PERFORMED,
			CANCELED
		}

		/***************************************/
		/*               MEMBERS               */
		/***************************************/
		public InputActionCallbackType callbackType;
		public InputAction input;
		public UnityEvent inputEvent;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/


		/***************************************/
		/*               METHODS               */
		/***************************************/
		private void OnEnable() {
			switch (callbackType) {
				case InputActionCallbackType.STARTED:
					input.started += Invocation;
					input.Enable();
					break;
				case InputActionCallbackType.PERFORMED:
					input.performed += Invocation;
					input.Enable();
					break;
				case InputActionCallbackType.CANCELED:
					input.canceled += Invocation;
					input.Enable();
					break;
			}
		}

		private void OnDisable() {
			switch (callbackType) {
				case InputActionCallbackType.STARTED:
					input.started -= Invocation;
					input.Disable();
					break;
				case InputActionCallbackType.PERFORMED:
					input.performed -= Invocation;
					input.Disable();
					break;
				case InputActionCallbackType.CANCELED:
					input.canceled -= Invocation;
					input.Disable();
					break;
			}
		}

		private void Invocation(InputAction.CallbackContext context) {
			inputEvent.Invoke();
		}

		protected override void UpdateCurve() {
			UpdateCurveValue();
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}