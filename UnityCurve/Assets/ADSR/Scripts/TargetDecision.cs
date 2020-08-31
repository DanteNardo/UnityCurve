

/*******************************************/
/*                  INCLUDES               */
/*******************************************/
using UnityEngine;

/*******************************************/
/*                   CLASS                 */
/*******************************************/
namespace UnityCurve {
	/// <summary>
	/// 
	/// </summary>
	[CreateAssetMenu(menuName = "UnityCurve/Decisions/TargetDecision")]
	public class TargetDecision : Decision {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/
		public float target;
		private double lastValue;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/


		/***************************************/
		/*               METHODS               */
		/***************************************/
		public override bool Decide(CurveController controller) {
			bool outcome = ValueHitTarget(controller);
			lastValue = controller.Value;
			return outcome;
		}

		private bool ValueHitTarget(CurveController controller) {
			// Value is close to target
			if (Mathf.Approximately((float)controller.Value, target)) {
				controller.SetValue(target);
				return true;
			}

			// These are the cases where the controller.Value oversteps the target
			if (lastValue < target && controller.Value > target) {
				controller.SetValue(target);
				return true;
			}
			if (lastValue > target && controller.Value < target) {
				controller.SetValue(target);
				return true;
			}

			// Default return
			return false;
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}
