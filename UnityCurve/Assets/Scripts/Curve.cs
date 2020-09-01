

/*******************************************/
/*                  INCLUDES               */
/*******************************************/
using CalcEngine;
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
	[RequireComponent(typeof(UnityCurve))]
	public abstract class Curve : MonoBehaviour {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/
		public string formula;
		public Curve nextCurve;
		protected UnityCurve unityCurve;
		private bool active = false;
		private Expression expression;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/


		/***************************************/
		/*               METHODS               */
		/***************************************/
		public void Awake() {
			unityCurve = GetComponent<UnityCurve>();
			expression = unityCurve.CalculationEngine.Parse(formula);
		}

		private void Update() {
			if (active && unityCurve.UpdateType == UPDATE_TYPES.UPDATE) {
				UpdateCurve();
			}
		}

		private void FixedUpdate() {
			if (active && unityCurve.UpdateType == UPDATE_TYPES.FIXED_UPDATE) {
				UpdateCurve();
			}
		}

		private void LateUpdate() {
			if (active && unityCurve.UpdateType == UPDATE_TYPES.LATE_UPDATE) {
				UpdateCurve();
			}
		}

		public void Activate() {
			active = true;
		}

		public void Deactivate() {
			active = false;
		}

		protected abstract void UpdateCurve();

		protected void UpdateCurveValue() {
			unityCurve.UpdateValue(expression);
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}