

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
	/// curve once a number of seconds equal
	/// to the public duration value have passed.
	/// </summary>
	public class DurationCurve : Curve {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/

		/// <summary>
		/// The time in seconds that must pass
		/// once this curve is activated in order
		/// to change to the next curve.
		/// </summary>
		public double duration;

		/// <summary>
		/// This variable is necessary in case
		/// the total duration oversteps the
		/// target duration.
		/// </summary>
		private double lastDuration;

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
			lastDuration = duration;
		}

		/// <summary>
		/// Updates the curve value and triggers the ChangeToNextCurve function once duration time has passed.
		/// </summary>
		protected override void UpdateCurve() {
			UpdateCurveValue();

			if (DurationHit()) {
				unityCurve.ChangeToNextCurve();
			}
		}

		/// <summary>
		/// Determines if the duration in seconds has passed.
		/// </summary>
		/// <returns>True if duration seconds has passed, else false.</returns>
		public bool DurationHit() {
			if (Mathf.Approximately((float)duration, (float)unityCurve.CurrentCurveTime)) {
				return true;
			}

			// Check for overstepping duration
			if (lastDuration < duration && unityCurve.CurrentCurveTime > duration) {
				return true;
			}
			// Check for understepping duration
			if (lastDuration > duration && unityCurve.CurrentCurveTime < duration) {
				return true;
			}

			// Default return and update lastDuration
			lastDuration = unityCurve.CurrentCurveTime;
			return false;
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}