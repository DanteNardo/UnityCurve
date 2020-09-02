

/*******************************************/
/*                  INCLUDES               */
/*******************************************/
using System;
using UnityEngine;
using UnityEngine.InputSystem;

/*******************************************/
/*                   CLASS                 */
/*******************************************/
namespace UnityCurve {
	/// <summary>
	/// 
	/// </summary>
	public class DurationCurve : Curve {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/
		public double duration;
		private double lastDuration;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/


		/***************************************/
		/*               METHODS               */
		/***************************************/
		private void Start() {
			// This is needed to make sure we don't 
			// accidentally trigger the overstep
			// or understep in DurationHit.
			lastDuration = duration;
		}

		protected override void UpdateCurve() {
			UpdateCurveValue();

			if (DurationHit()) {
				unityCurve.ChangeToNextCurve();
			}
		}

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