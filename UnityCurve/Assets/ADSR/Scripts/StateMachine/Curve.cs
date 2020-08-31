

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
	[CreateAssetMenu(menuName = "UnityCurve/Curve")]
	public class Curve : ScriptableObject {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/
		public string formula;
		public Action[] actions;
		public Transition[] transitions;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/


		/***************************************/
		/*               METHODS               */
		/***************************************/
		public void UpdateState(CurveController controller) {
			DoActions(controller);
			CheckTransitions(controller);
		}

		private void DoActions(CurveController controller) {
			foreach (Action action in actions) {
				action.Act(controller);
			}
		}

		private void CheckTransitions(CurveController controller) {
			foreach (Transition transition in transitions) {
				bool decision = transition.decision.Decide(controller);
				switch (decision) {
					case true: 
						controller.TransitionToCurve(transition.trueCurve);
						break;
					case false:
						controller.TransitionToCurve(transition.falseCurve);
						break;
				}
			}
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}
