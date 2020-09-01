

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
	public class TargetCurve : Curve {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/
		public double target;
		private double lastValue;

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
			lastValue = target;
		}

		protected override void UpdateCurve() {
			UpdateCurveValue();

			if (TargetHit()) {
				unityCurve.ChangeToNextCurve();
			}
		}

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