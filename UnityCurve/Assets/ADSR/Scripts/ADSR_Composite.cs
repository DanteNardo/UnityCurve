

/***********************************************/
/*                  INCLUDES                   */
/***********************************************/
using UnityEngine;

/***********************************************/
/*                   CLASS                     */
/***********************************************/
namespace UnityCurve {
	/// <summary>
	/// A very simple component that returns a
	/// single Value that represents the sum of
	/// all current Values from the ADSR 
	/// variables this composite component
	/// listens to. This has a lot of uses:
	/// ----------------------------------------
	/// Bind one 
	/// </summary>
	public class ADSR_Composite : MonoBehaviour {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/
		public ADSR[] variables;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/
		public double Value { 
			get {
				double composite = 0;
				foreach (var v in variables) {
					composite += v.Value;
				}
				return composite;
			}
		}

		/***************************************/
		/*               METHODS               */
		/***************************************/


		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}
