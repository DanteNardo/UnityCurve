

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
	private KeyControl keyControl;
	public Key key;

    /***************************************/
    /*              PROPERTIES             */
    /***************************************/
    public bool Attack {
		get {
			if (keyControl != null) return keyControl.wasPressedThisFrame;
			
			// Default return
			return false;
		}
	}
    public bool Release {
		get {
			if (keyControl != null) return keyControl.wasReleasedThisFrame;

			// Default return
			return false;
		}
	}

	/***************************************/
	/*               METHODS               */
	/***************************************/
	private void FixedUpdate() {
		Keyboard keyboard = InputSystem.GetDevice<Keyboard>();
		keyControl = keyboard[key];
	}

	/***************************************/
	/*              COROUTINES             */
	/***************************************/
}
