

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
	public ADSRInput xInput;
	public ADSRInput yInput;
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
		if (keyboard.aKey.wasPressedThisFrame) {
			xInput.InputAttack();
			yInput.InputAttack();
		}
		if (keyboard.aKey.wasReleasedThisFrame) {
			xInput.InputRelease();
			yInput.InputRelease();
		}

		// Update position for demonstration
		transform.position = new Vector3((float)x.Value, (float)y.Value, 0);
	}

	/***************************************/
	/*              COROUTINES             */
	/***************************************/
}
