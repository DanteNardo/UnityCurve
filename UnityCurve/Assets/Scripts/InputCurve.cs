

/***********************************************/
/*                   INCLUDES                  */
/***********************************************/
using UnityEngine.InputSystem;

/***********************************************/
/*                     CLASS                   */
/***********************************************/
namespace UnityCurve {

	/// <summary>
	/// This Curve will transition to the next
	/// curve once a specific input action's
	/// callback is triggered.
	/// </summary>
	public class InputCurve : Curve {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/

		/// <summary>
		/// A workaround for detecting which 
		/// inputAction callback should trigger
		/// this InputCurve.
		/// </summary>
		public INPUT_CALLBACK callbackType;

		/// <summary>
		/// The input action we use to trigger
		/// this InputCurve transition.
		/// </summary>
		public InputAction input;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/


		/***************************************/
		/*               METHODS               */
		/***************************************/

		/// <summary>
		/// Add the Invocation call to the correct
		/// inputAction callback based on public
		/// inspector values.
		/// </summary>
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

		/// <summary>
		/// Remove the Invocation call to the correct
		/// inputAction callback based on public
		/// inspector values.
		/// </summary>
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

		/// <summary>
		/// This invocation transitions the
		/// UnityCurve to the desired curve
		/// regardless of whatever current curve
		/// the UnityCurve is on.
		/// </summary>
		/// <param name="context">The inputAction context. This is required, but unused.</param>
		private void Invocation(InputAction.CallbackContext context) {
			unityCurve.ChangeToNextCurve();
		}

		/// <summary>
		/// Solely updates the curve value. 
		/// InputAction listens and triggers the transition instead of UpdateCurve.
		/// </summary>
		protected override void UpdateCurve() {
			UpdateCurveValue();
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}