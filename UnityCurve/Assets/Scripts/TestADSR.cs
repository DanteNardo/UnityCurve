

/*******************************************/
/*                  INCLUDES               */
/*******************************************/
using UnityEngine;
using UnityEngine.InputSystem;

/*******************************************/
/*                   CLASS                 */
/*******************************************/
public class TestADSR : MonoBehaviour {

	/***************************************/
	/*               MEMBERS               */
	/***************************************/
	public ADSR x;
	public ADSR y;

	/***************************************/
	/*              PROPERTIES             */
	/***************************************/


	/***************************************/
	/*               METHODS               */
	/***************************************/
	private void Update() {
		Keyboard keyboard = InputSystem.GetDevice<Keyboard>();

		/* 
		 * DEFAULT BEHAVIOR:
		 * The object moves linearly along the
		 * x plane while there are complex
		 * curves to the y height. This is 
		 * used to show contrast between the
		 * two formula setups.
		 */

		// Update position for demonstration
		transform.position = new Vector3((float)x.Value, (float)y.Value, 0);
	}

	/***************************************/
	/*              COROUTINES             */
	/***************************************/
}
