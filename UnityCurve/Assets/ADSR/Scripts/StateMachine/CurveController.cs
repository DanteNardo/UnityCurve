

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
	public class CurveController : MonoBehaviour {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/
		public bool controllerActive;
		public Curve currentCurve;
		public Curve remainCurve;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/
		public double Value { get; private set; }

		/***************************************/
		/*               METHODS               */
		/***************************************/
		private void Awake() {

		}

		private void Update() {
			if (controllerActive) {
				currentCurve.UpdateState(this);
			}
		}

		public void AddValue(double delta) {
			Value += delta;
		}

		public void SetValue(double value) {
			Value = value;
		}

		public void TransitionToCurve(Curve nextCurve) {
			if (nextCurve != remainCurve) {
				currentCurve = nextCurve;
			}
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}