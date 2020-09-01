

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
	public class SustainCurve : Curve {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/
		private const string SUSTAIN_FORMULA = "0";

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/


		/***************************************/
		/*               METHODS               */
		/***************************************/
		private new void Awake() {
			formula = SUSTAIN_FORMULA;
			nextCurve = null;
			base.Awake();
		}

		protected override void UpdateCurve() {
			UpdateCurveValue();
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}