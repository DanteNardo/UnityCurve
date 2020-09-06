

/***********************************************/
/*                   INCLUDES                  */
/***********************************************/
using UnityEngine.Events;
using UnityEngine.InputSystem;

/***********************************************/
/*                     CLASS                   */
/***********************************************/
namespace UnityCurve {
	/// <summary>
	/// 
	/// </summary>
	public class InputCurve : Curve {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/
		public INPUT_CALLBACK callbackType;
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
				case INPUT_CALLBACK.STARTED:
					input.started += Invocation;
					input.Enable();
					break;
				case INPUT_CALLBACK.PERFORMED:
					input.performed += Invocation;
					input.Enable();
					break;
				case INPUT_CALLBACK.CANCELED:
					input.canceled += Invocation;
					input.Enable();
					break;
			}
		}

		private void OnDisable() {
			switch (callbackType) {
				case INPUT_CALLBACK.STARTED:
					input.started -= Invocation;
					input.Disable();
					break;
				case INPUT_CALLBACK.PERFORMED:
					input.performed -= Invocation;
					input.Disable();
					break;
				case INPUT_CALLBACK.CANCELED:
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