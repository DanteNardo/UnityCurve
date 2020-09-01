

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
	[RequireComponent(typeof(UnityCurve))]
	public class CurveTransition : MonoBehaviour {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/
		public InputActionCallbackType callbackType;
		public InputAction input;
		public Curve curveToTransitionTo;
		private UnityCurve unityCurve;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/


		/***************************************/
		/*               METHODS               */
		/***************************************/
		public void Awake() {
			unityCurve = GetComponent<UnityCurve>();
		}

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
			Debug.Log("Phase:" + context.phase);
			unityCurve.ChangeToCurve(curveToTransitionTo);
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}