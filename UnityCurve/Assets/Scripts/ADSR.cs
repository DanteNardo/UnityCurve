

/*******************************************/
/*                  INCLUDES               */
/*******************************************/
using CalcEngine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

/*******************************************/
/*                    ENUM                 */
/*******************************************/
public enum ADSR_STATE {
    ATTACK,
    DECAY,
    SUSTAIN,
    RELEASE,
    NONE
}

/*******************************************/
/*                   CLASS                 */
/*******************************************/
/// <summary>
/// From Steve Swink's 'Game Feel' paradigm
/// ----------------------------------------
/// "An ADSR envelope describes the 
/// modulation of a parameter over time, in 
/// four distinct phases."
/// ----------------------------------------
/// 
/// ATTACK:  The phase where the parameter
///          moves from the default value to
///          the highest point.
/// DECAY:   The phase where the parameter
///          moves from the highest point to
///          a sustainable point.
/// SUSTAIN: The phase where the parameter 
///          stays at a constant value, most
///          commonly the longest phase.
/// RELEASE: The phase where the parameter
///          moves from the sustained value 
///          back to the default value.
/// </summary>
[RequireComponent(typeof(ADSRInput))]
public class ADSR : MonoBehaviour {

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
    private CalcEngine.CalcEngine calcEngine;

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

	/// <summary>
	/// This is a reference to an asset
	/// that is created as a part of the
	/// 2019+ Unity Input system. Most games
	/// will have a custom one that you use.
	/// HOWEVER, the default script simply 
	/// looks for a given InputAction. You
	/// may modify it to use an asset if
	/// that is how your game is set up.
	/// </summary>
	//public InputActions inputActions;

	/// <summary>
	/// The code assumes that this reads
	/// input from some kind of Release
	/// button. See OnEnable and OnDisable.
	/// </summary>
	public InputAction inputAction;

    // Inspector variables
    public ADSRInput input;
    public double defaultValue = 0;
    public double attackTarget;
    public double sustainTarget;
    public string attackFormula;
    public string decayFormula;
    public string sustainFormula;
    public string releaseFormula;

    /***************************************/
    /*              PROPERTIES             */
    /***************************************/
    public ADSR_STATE State { get; private set; } = ADSR_STATE.NONE;
    public double Value { get; private set; }
    public double StateTime { get; private set; }

    private Expression AttackExpression { get; set; }
    private Expression DecayExpression { get; set; }
    private Expression SustainExpression { get; set; }
    private Expression ReleaseExpression { get; set; }

	/***************************************/
	/*               METHODS               */
	/***************************************/
	private void Start() {
        // Initialize calculation engine
        calcEngine = new CalcEngine.CalcEngine();
        calcEngine.Variables[PARAMETER_NAME] = StateTime;

        // Initialize expressions
        AttackExpression  = calcEngine.Parse(attackFormula);
        DecayExpression   = calcEngine.Parse(decayFormula);
        SustainExpression = calcEngine.Parse(sustainFormula);
        ReleaseExpression = calcEngine.Parse(releaseFormula);
    }

    private void OnEnable() {
        if (inputActions == null) {
            inputActions = new InputActions();

			/* 
             * =============================================================
             * CUSTOMIZE YOUR INPUT HERE FOR TRIGGERING ATTACK AND RELEASE
             * =============================================================
			 * Default Behavior: This script assumes that ACTION.started is 
			 * triggered on the first frame of input only and that
			 * ACTION.performed is triggered when the input is released.
             * 
			 * SETUP WITH INPUT ACTIONS ASSET:
             * inputActions.ACTION_MAP.ACTION_NAME.started += Attack;
             * inputActions.ACTION_MAP.ACTION_NAME.performed += Release;
             * inputActions.ACTION_MAP.ACTION_NAME.Enable();
             */

			inputAction.started += Attack;
			inputAction.performed += Release;
			inputAction.Enable();
		}
    }

    private void OnDisable() {
        if (inputActions == null) {
            inputActions = new InputActions();

			/* 
             * =============================================================
             * CUSTOMIZE YOUR INPUT HERE FOR TRIGGERING ATTACK AND RELEASE
             * =============================================================
             * **** This should call the same functions as above, but   ****
             * **** with -= instead of += and Disable instead of Enable ****
             * 
			 * SETUP WITH INPUT ACTIONS ASSET:
             * inputActions.ACTION_MAP.ACTION_NAME.started -= Attack;
             * inputActions.ACTION_MAP.ACTION_NAME.performed -= Release;
             * inputActions.ACTION_MAP.ACTION_NAME.Disable();
             */

			inputAction.started -= Attack;
			inputAction.performed -= Release;
			inputAction.Disable();
		}
    }

    /// <summary>
    /// Updates the parameter's value based on state.
    /// </summary>
	private void FixedUpdate() {
        // Update Value based on state
        switch (State) {
            case ADSR_STATE.ATTACK:
                UpdateValue(AttackExpression);
                break;
            case ADSR_STATE.DECAY:
                UpdateValue(DecayExpression);
                break;
            case ADSR_STATE.SUSTAIN:
                UpdateValue(SustainExpression);
                break;
            case ADSR_STATE.RELEASE:
                UpdateValue(ReleaseExpression);
                break;
            default:
                Value = defaultValue;
                break;
        }
    }

    /// <summary>
    /// Triggered by an input event. Changes state to Attack.
    /// </summary>
    /// <param name="callbackContext">The input callback context.</param>
    private void Attack(InputAction.CallbackContext callbackContext) {
        ChangeToNextState(ADSR_STATE.ATTACK);
    }

    /// <summary>
    /// Triggered by an input event. Changes state to Attack.
    /// </summary>
    /// <param name="callbackContext">The input callback context.</param>
    private void Release(InputAction.CallbackContext callbackContext) {
        ChangeToNextState(ADSR_STATE.RELEASE);
    }

    /// <summary>
    /// Updates the time in this state and the value based on expression and time.
    /// Automatically triggers transition to next state when we hit target value.
    /// </summary>
    /// <param name="expression">The formula to use to calculate the changes to Value.</param>
    private void UpdateValue(Expression expression) {
        StateTime += Time.fixedDeltaTime;
        Value += CalculateDelta(expression);

        // Change to the next state once we hit target value or release input.
        // To change from sustain to release, we need to check for a release.
        if (HitStateTarget()) {
            ChangeToNextState(GetNextState());
        }
    }

    /// <summary>
    /// Handles changing the state from one to another. 
    /// Checks for errors along the way.
    /// </summary>
    /// <param name="toState">The state we want to be in</param>
    public void ChangeToNextState(ADSR_STATE toState) {
        // Immediately change state and reset time
        Debug.Log("ChangeToNextState --- State:" + toState + ", Time:" + StateTime);
        State = toState;
        StateTime = 0.0f;

        // Check if everything is set up to skip this state (for example we want no decay and have hit decayTarget)
        if (HitStateTarget()) {
            ChangeToNextState(GetNextState());
            return;
		}
    }

    /// <summary>
    /// Checks if we hit the target value for this state.
    /// Sustain state requires another check called CheckForRelease().
    /// </summary>
    /// <returns>True if hit state's target, else false</returns>
    private bool HitStateTarget() {
        switch (State) {
            // We wish to increase the value until we hit the peak
            case ADSR_STATE.ATTACK: 
                return HitTarget((float)attackTarget, true);

            // We wish to decay until we hit the sustainable value
            case ADSR_STATE.DECAY: 
                return HitTarget((float)sustainTarget, false);

            // There is no target for sustain
            case ADSR_STATE.SUSTAIN: 
                return false;

            // Once input is released, we wish to go back to default
            case ADSR_STATE.RELEASE: 
                return HitTarget((float)defaultValue, true);

            // There is no target
            default: return false;
        }
    }

    /// <summary>
    /// Checks if the target has been hit. Keeps in mind that Value can overstep the target.
    /// </summary>
    /// <param name="target">What we want Value to be.</param>
    /// <param name="increasingValue">Whether or not Value increases towards target or decreases.</param>
    /// <returns>True if Value is at target or has crossed past it, else false.</returns>
    private bool HitTarget(float target, bool increasingValue) {
        // Value is close to target
        if (Mathf.Approximately((float)Value, (float)target))
            return true;

        // These are the cases where the value oversteps the target
        if (increasingValue && Value > target)
            return true;
        if (!increasingValue && Value < target)
            return true;

        // Default return
        return false;
    }

    /// <summary>
    /// Gets the next state based on current state.
    /// </summary>
    /// <returns>The next state.</returns>
    private ADSR_STATE GetNextState() {
        switch (State) {
            case ADSR_STATE.NONE:    return ADSR_STATE.ATTACK;
            case ADSR_STATE.ATTACK:  return ADSR_STATE.DECAY;
            case ADSR_STATE.DECAY:   return ADSR_STATE.SUSTAIN;
            case ADSR_STATE.SUSTAIN: return ADSR_STATE.RELEASE;
            case ADSR_STATE.RELEASE: return ADSR_STATE.NONE;
            default:                 return ADSR_STATE.NONE;
        }
    }

    /// <summary>
    /// Halts state transitions and resets to default value.
    /// This "cancels" the phase approach to the parameter.
    /// </summary>
    public void StopSignal() {
        ChangeToNextState(ADSR_STATE.NONE);
    }

    /// <summary>
    /// Uses CalcEngine to calculate an expression derived from an inspector formula.
    /// </summary>
    /// <param name="expression">The expression used to calculate.</param>
    /// <returns>A new value based on the expression and time in this phase.</returns>
    private double CalculateDelta(Expression expression) {
        // Must update the Value variable stored in the CalcEngine
        calcEngine.Variables[PARAMETER_NAME] = StateTime;

        // Calculate the result of the expression
        var result = calcEngine.Evaluate(expression);

        // For debugging, TODO: REMOVE
        Debug.Log("RESULT: '" + result + "', Type: " + result.GetType().Name);

        // Return delta
        return (double)result;
    }

    /// <summary>
    /// A simpler way to log errors in Unity.
    /// Also halts execution.
    /// </summary>
    /// <param name="error">The error text to present to the user.</param>
    private void Error(string error) {
        Debug.LogError(error);
        StopSignal();
    }

    /***************************************/
    /*              COROUTINES             */
    /***************************************/
}
