

/*******************************************/
/*                  INCLUDES               */
/*******************************************/
using CalcEngine;
using UnityEngine;

/*******************************************/
/*                   CLASS                 */
/*******************************************/
namespace UnityCurve {
	/// <summary>
	/// 
	/// </summary>
	[CreateAssetMenu(menuName = "UnityCurve/Actions/CalculateAction")]
	public class CalculateAction : Action {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/
		public Calculator calculator;
		public Curve curve;
		private Expression expression;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/


		/***************************************/
		/*               METHODS               */
		/***************************************/
		private void Awake() {
			if (calculator != null && curve != null) {
				expression = calculator.Parse(curve.formula);
			}
			else {
				if (calculator == null) {
					Debug.LogError("Calculator Reference Is Null: create calculator or assign reference in inspector.");
				}
				if (curve == null) {
					Debug.LogError("Curve Reference Is Null: assign reference in inspector.");
				}
			}
		}

		public override void Act(CurveController controller) {
			Calculation(controller);
		}

		private void Calculation(CurveController controller) {
			controller.AddValue(calculator.Evaluate(expression));
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}