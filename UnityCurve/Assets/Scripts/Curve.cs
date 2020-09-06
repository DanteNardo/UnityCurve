

/***********************************************/
/*                   INCLUDES                  */
/***********************************************/
using CalcEngine;
using UnityEngine;

/***********************************************/
/*                     CLASS                   */
/***********************************************/
namespace UnityCurve {

	/// <summary>
	/// The abstract class that all curves inherit
	/// from. A curve is a continuous function
	/// that makes up a part of a UnityCurve.
	/// </summary>
	[RequireComponent(typeof(UnityCurve))]
	public abstract class Curve : MonoBehaviour {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/

		/// <summary>
		/// An excel formula that is compiled 
		/// into an Expression that a Calculation
		/// Engine can use to calculate a Y
		/// value for a UnityCurve.
		/// </summary>
		public string formula;

		/// <summary>
		/// Most curves will call UnityCurve's
		/// ChangeToNextCurve function and when
		/// they do UnityCurve will switch to 
		/// this curve.
		/// </summary>
		public Curve nextCurve;

		/// <summary>
		/// This is used to render the curve
		/// in the debug or demo UI.
		/// </summary>
		public Color curveColor;

		/// <summary>
		/// A reference to the UnityCurve that
		/// this curve is a part of.
		/// </summary>
		protected UnityCurve unityCurve;

		/// <summary>
		/// Whether or not this Curve is the
		/// active curve on the UnityCurve.
		/// </summary>
		private bool active = false;

		/// <summary>
		/// A private variable for storing the
		/// faster-to-process Expression instead
		/// of the slower formula.
		/// </summary>
		private Expression expression;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/


		/***************************************/
		/*               METHODS               */
		/***************************************/

		/// <summary>
		/// Gets parent UnityCurve and pareses
		/// formula into a faster to calculate
		/// expression variable.
		/// </summary>
		public void Awake() {
			unityCurve = GetComponent<UnityCurve>();
			expression = unityCurve.CalculationEngine.Parse(formula);
		}

		/// <summary>
		/// Updates this curve based on parent
		/// UnityCurve type.
		/// </summary>
		private void Update() {
			if (active && unityCurve.UpdateType == UPDATE_TYPES.UPDATE) {
				UpdateCurve();
			}
		}

		/// <summary>
		/// Updates this curve based on parent
		/// UnityCurve type.
		/// </summary>
		private void FixedUpdate() {
			if (active && unityCurve.UpdateType == UPDATE_TYPES.FIXED_UPDATE) {
				UpdateCurve();
			}
		}

		/// <summary>
		/// Updates this curve based on parent
		/// UnityCurve type.
		/// </summary>
		private void LateUpdate() {
			if (active && unityCurve.UpdateType == UPDATE_TYPES.LATE_UPDATE) {
				UpdateCurve();
			}
		}

		/// <summary>
		/// Activates this curve.
		/// </summary>
		public void Activate() {
			active = true;
		}

		/// <summary>
		/// Deactivates this curve.
		/// </summary>
		public void Deactivate() {
			active = false;
		}

		/// <summary>
		/// A method that is unique to all 
		/// children. Generally, this updates the 
		/// curve value and also calls the
		/// UnityCurve.ChangeToNextCurve() 
		/// function when its appropriate.
		/// </summary>
		protected abstract void UpdateCurve();

		/// <summary>
		/// Updates the current curve value for
		/// this curve's UnityCurve parent.
		/// </summary>
		protected void UpdateCurveValue() {
			unityCurve.UpdateCurrentCurveValue(expression);
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}