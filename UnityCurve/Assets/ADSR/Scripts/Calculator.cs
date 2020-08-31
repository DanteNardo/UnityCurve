

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
	[CreateAssetMenu(menuName = "UnityCurve/Calculator")]
	public class Calculator : ScriptableObject {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/

		/// <summary>
		/// https://github.com/Bernardo-Castilho/CalcEngine
		/// This is an Open-Source excel formula 
		/// calculation engine developed by 
		/// Bernardo-Castilho and licensed under 
		/// the MIT license.
		/// </summary>
		private CalcEngine.CalcEngine calculationEngine;

		/// <summary>
		/// Named like a constant to enforce not
		/// changing, but accessible in inspector.
		/// 
		/// YOU MUST CHANGE THE STRING IN YOUR 
		/// ADSR FORMULAS TO MATCH IF YOU CHANGE
		/// THIS VARIABLE'S VALUE.
		/// 
		/// AND THIS PARAMETER CANNOT BE A NAME
		/// FOR ANY BUILT-IN EXCEL FUNCTION. IF
		/// IT IS, YOU WILL GET A CALCENGINE
		/// ERROR SAYING THERE ARE TOO FEW 
		/// PARAMETERS.
		/// </summary>
		public string PARAMETER_NAME = "X";

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/


		/***************************************/
		/*               METHODS               */
		/***************************************/
		private void Awake() {
			calculationEngine = new CalcEngine.CalcEngine();
			UpdateVariable(0);
		}

		public void UpdateVariable(object o) {
			calculationEngine.Variables[PARAMETER_NAME] = o;
		}

		public Expression Parse(string expression) {
			return calculationEngine.Parse(expression);
		}

		public double Evaluate(Expression expression) {
			return (double)calculationEngine.Evaluate(expression);
		}

		public double Evaluate(string expression) {
			return (double)calculationEngine.Evaluate(expression);
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}