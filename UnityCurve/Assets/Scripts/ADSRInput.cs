

/*******************************************/
/*                  INCLUDES               */
/*******************************************/
using UnityEngine;

/*******************************************/
/*                   CLASS                 */
/*******************************************/
public class ADSRInput : MonoBehaviour {

    /***************************************/
    /*               MEMBERS               */
    /***************************************/


    /***************************************/
    /*              PROPERTIES             */
    /***************************************/
    public bool Attack { get; private set; } = false;
    public bool Release { get; private set; } = false;

	/***************************************/
	/*               METHODS               */
	/***************************************/
	private void LateUpdate() {
		if (Attack) {
			Attack = false;
		}

		if (Release) {
			Release = false;
		}
	}

	public void InputAttack() {
		Attack = true;
	}

	public void InputRelease() {
		Release = true;
	}

	/***************************************/
	/*              COROUTINES             */
	/***************************************/
}
