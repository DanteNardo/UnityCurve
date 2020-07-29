

/*******************************************/
/*                  INCLUDES               */
/*******************************************/
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

/*******************************************/
/*                   CLASS                 */
/*******************************************/
public class ADSRInput : MonoBehaviour {

	/***************************************/
	/*               MEMBERS               */
	/***************************************/
	public InputActions inputActions;

	/***************************************/
	/*              PROPERTIES             */
	/***************************************/


	/***************************************/
	/*               METHODS               */
	/***************************************/
	private void OnEnable() {
		if (inputActions == null) {
			inputActions = new InputActions();

			inputActions.Player.Debug.started += Attack;
			inputActions.Player.Debug.performed += Release;
			inputActions.Player.Debug.Enable();
		}
	}

	private void OnDisable() {
		if (inputActions == null) {
			inputActions = new InputActions();

			inputActions.Player.Debug.started -= Attack;
			inputActions.Player.Debug.performed -= Release;
			inputActions.Player.Debug.Enable();
		}
	}

	private void Attack(InputAction.CallbackContext callbackContext) {

	}

	private void Release(InputAction.CallbackContext callbackContext) {

	}

	/***************************************/
	/*              COROUTINES             */
	/***************************************/
}
