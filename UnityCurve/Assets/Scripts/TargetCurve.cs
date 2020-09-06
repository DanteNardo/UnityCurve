

/***********************************************/
/*                   INCLUDES                  */
/***********************************************/
using UnityEngine;

/***********************************************/
/*                     CLASS                   */
/***********************************************/
namespace UnityCurve {

	/// <summary>
	/// This Curve will transition to the next
	/// curve once the UnityCurve's value is
	/// equal to or very close to the target.
	/// </summary>
	public class TargetCurve : Curve {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/

		/// <summary>
		/// The UnityCurve Value that must occur
		/// when this curve is activated in order
		/// to change to the next curve.
		/// </summary>
		public double target;

		/// <summary>
		/// This variable is necessary in case
		/// the UnityCurve Value oversteps the
		/// target value.
		/// </summary>
		private double lastValue;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/


		/***************************************/
		/*               METHODS               */
		/***************************************/

		/// <summary>
		/// A workaround to prevent an initial duration overstep.
		/// </summary>
		private void Start() {
			// This is needed to make sure we don't 
			// accidentally trigger the overstep
			// or understep in DurationHit.
			lastValue = target;
		}

		/// <summary>
		/// Updates the curve value and triggers the ChangeToNextCurve function once target value is hit.
		/// </summary>
		protected override void UpdateCurve() {
			UpdateCurveValue();

			if (TargetHit()) {
				unityCurve.ChangeToNextCurve();
			}
		}

		/// <summary>
		/// Determines if the target value was hit.
		/// </summary>
		/// <returns>True if Value is equal to or passed target, else false.</returns>
		public bool TargetHit() {
			if (Mathf.Approximately((float)target, (float)unityCurve.Value)) {
				unityCurve.SetValue(target);
				return true;
			}

			// Check for overstepping target
			if (lastValue < target &&  unityCurve.Value > target) {
				unityCurve.SetValue(target);
				return true;
			}
			// Check for understepping target
			if (lastValue > target && unityCurve.Value < target) {
				unityCurve.SetValue(target);
				return true;
			}

			// Default return and update lastValue
			lastValue = unityCurve.Value;
			return false;
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}