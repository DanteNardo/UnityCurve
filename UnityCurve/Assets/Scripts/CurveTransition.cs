

/***********************************************/
/*                   INCLUDES                  */
/***********************************************/
using UnityEngine;
using UnityEngine.InputSystem;

/***********************************************/
/*                     CLASS                   */
/***********************************************/
namespace UnityCurve {

	/// <summary>
	/// This class is useful for when you want to
	/// transition a UnityCurve from one curve to 
	/// any other curve based on input. It is 
	/// also perfect for when you want a state
	/// transition that occurs regardless of
	/// what the current state is, such as 
	/// when you start or exit a UnityCurve.
	/// </summary>
	[RequireComponent(typeof(UnityCurve))]
	public class CurveTransition : MonoBehaviour {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/

		/// <summary>
		/// A workaround for detecting which 
		/// inputAction callback should trigger
		/// this CurveTransition.
		/// </summary>
		public INPUT_CALLBACK callbackType;

		/// <summary>
		/// The input action we use to trigger
		/// this UnityCurve transition.
		/// </summary>
		public InputAction input;

		/// <summary>
		/// This is the Curve that will become
		/// active when the InputAction's
		/// callback is triggered.
		/// </summary>
		public Curve curveToTransitionTo;

		/// <summary>
		/// A reference to the parent UnityCurve.
		/// </summary>
		private UnityCurve unityCurve;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/


		/***************************************/
		/*               METHODS               */
		/***************************************/

		/// <summary>
		/// Gets the parent UnityCurve.
		/// </summary>
		public void Awake() {
			unityCurve = GetComponent<UnityCurve>();
		}

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
			unityCurve.ChangeToCurve(curveToTransitionTo);
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}