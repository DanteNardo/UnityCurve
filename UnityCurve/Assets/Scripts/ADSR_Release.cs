

/*******************************************/
/*                  INCLUDES               */
/*******************************************/
using UnityEngine;
using UnityEngine.InputSystem;

/*******************************************/
/*                   CLASS                 */
/*******************************************/
public class ADSR_Release : ADSR {

	/***************************************/
	/*               MEMBERS               */
	/***************************************/

	/// <summary>
	/// This is a reference to an asset
	/// that is created as a part of the
	/// 2019+ Unity Input system. Most games
	/// will have a custom one that you use.
	/// HOWEVER, the default version of this
	/// script simply looks for a given 
	/// InputAction. You may modify it to 
	/// use an asset if that is how your 
	/// game is set up.
	/// </summary>
	//public InputActions inputActions;

	/// <summary>
	/// The code assumes that this reads
	/// input from some kind of Release
	/// button. See Enable/Disable Input.
	/// </summary>
	[Space(10)]
	public InputAction inputAction;

	/***************************************/
	/*              PROPERTIES             */
	/***************************************/


	/***************************************/
	/*               METHODS               */
	/***************************************/
	protected override void EnableInput() {
		/* 
         * =============================================================
         * CUSTOMIZE YOUR INPUT IN EnableInput() TO SUIT YOUR NEEDS
         * =============================================================
		 * Default Behavior: This script assumes that ACTION.started is 
		 * triggered on the first frame of input only and that
		 * ACTION.performed is triggered when the input is released.
         * 
		 * SETUP WITH INPUT ACTIONS ASSET:
         * inputActions.ACTION_MAP.ACTION_NAME.started += Attack;
         * inputActions.ACTION_MAP.ACTION_NAME.performed += Release;
         * inputActions.ACTION_MAP.ACTION_NAME.Enable();
         * 
         * SETUP WITH SINGLE INPUT ACTION:
         * inputAction.started += Attack;
         * inputAction.performed += Release;
         * inputAction.Enable();
         */
		Debug.Log("EnableInput");
		inputAction.started += Attack;
		inputAction.performed += Release;
		inputAction.Enable();
	}

	protected override void DisableInput() {
		/* 
         * =============================================================
         * CUSTOMIZE YOUR INPUT IN DisableInput() TO SUIT YOUR NEEDS
         * =============================================================
         * 
		 * SETUP WITH INPUT ACTIONS ASSET:
         * inputActions.ACTION_MAP.ACTION_NAME.started -= Attack;
         * inputActions.ACTION_MAP.ACTION_NAME.performed -= Release;
         * inputActions.ACTION_MAP.ACTION_NAME.Disable();
         * 
         * SETUP WITH SINGLE INPUT ACTION:
         * inputAction.started -= Attack;
         * inputAction.performed -= Release;
         * inputAction.Disable();
         */
		Debug.Log("EnableInput");
		inputAction.started -= Attack;
		inputAction.performed -= Release;
		inputAction.Disable();
	}

	/***************************************/
	/*              COROUTINES             */
	/***************************************/
}
