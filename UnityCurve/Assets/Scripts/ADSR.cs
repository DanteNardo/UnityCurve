

/*******************************************/
/*                  INCLUDES               */
/*******************************************/
using CalcEngine;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/*******************************************/
/*                    ENUM                 */
/*******************************************/

/// <summary>
/// Used to easily track which ADSR envelope
/// phase this variable is currently in.
/// </summary>
public enum ADSR_STATE {
    NONE,
    ATTACK,
    DECAY,
    SUSTAIN,
    RELEASE
}

/// <summary>
/// Used to easily track and change 
/// Monobehavior update method.
/// </summary>
public enum UPDATE_TYPES {
    UPDATE = 0,
    LATE_UPDATE = 1,
    FIXED_UPDATE = 2
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
/// ----------------------------------------
/// The point of this class is to easily
/// add curves of varying shapes to the
/// modulation of a value based on input.
/// Purely linear increases and decreases
/// in a car's driving force is not great
/// game feel. This gives developers fast
/// and easy control over the curves of
/// a value instead of having to create
/// whole complicated and mismanaged
/// state machines and input systems.
/// </summary>
public abstract class ADSR : MonoBehaviour {

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
    /// This determines which update method 
    /// this variable uses. Consequently, it 
    /// also determines which type of 
    /// deltaTime to use for our time 
    /// recordings.
    /// </summary>
    public UPDATE_TYPES UpdateType = UPDATE_TYPES.UPDATE;

    /// <summary>
    /// This is the default value for the ADSR
    /// variable. This is also the minimum 
    /// value and Value can never be lower 
    /// than it.
    /// </summary>
    public double defaultValue = 0;

    /// <summary>
    /// This is the target value for the
    /// attack phase. Once Value reaches this
    /// target the decay phase begins. This
    /// is also the highest value for the 
    /// ADSR envelope so it must be higher
    /// than defaultValue and sustainTarget.
    /// </summary>
    public double attackTarget;

    /// <summary>
    /// This is the target value for the
    /// decay phase. Once Value reaches this
    /// target the sustain phase begins. This
    /// value must be higher than defaultValue 
    /// and less than or equal to attackTarget.
    /// </summary>
    public double sustainTarget;

    /// <summary>
    /// This is a string representation of
    /// an excel formula. Value will increase
    /// at the rate of this formula during
    /// the attack phase.
    /// </summary>
    public string attackFormula;

    /// <summary>
    /// This is a string representation of
    /// an excel formula. Value will decrease
    /// at the rate of this formula during
    /// the decay phase.
    /// </summary>
    public string decayFormula;

    /// <summary>
    /// This is a string representation of
    /// an excel formula. Value will decrease
    /// at the rate of this formula during
    /// the release phase.
    /// </summary>
    public string releaseFormula;

    /// <summary>
    /// Invoked on first frame of Attack state.
    /// </summary>
    [Space(10)]
    public UnityEvent ADSRAttack;

    /// <summary>
    /// Invoked on first frame of Decay state.
    /// </summary>
    public UnityEvent ADSRDecay;

    /// <summary>
    /// Invoked on first frame of Sustain state.
    /// </summary>
    public UnityEvent ADSRSustain;

    /// <summary>
    /// Invoked on first frame of Release state.
    /// </summary>
    public UnityEvent ADSRRelease;

    /// <summary>
    /// Invoked on first frame of None state.
    /// </summary>
    public UnityEvent ADSREnd;

    /// <summary>
    /// Invoked on first frame of every state.
    /// </summary>
    public UnityEvent ADSRStateChange;

    /***************************************/
    /*              PROPERTIES             */
    /***************************************/

    /// <summary>
    /// This is the ADSR envelope state at
    /// any point in time.
    /// </summary>
    public ADSR_STATE State { get; private set; } = ADSR_STATE.NONE;

    /// <summary>
    /// This is the value that modulates over
    /// the four ADSR phases.
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// This is the value that modulates over
    /// the four ADSR phases, but returned
    /// as a float.
    /// </summary>
    public float ValueAsFloat { get { return (float)Value; } }

    /// <summary>
    /// This is the value that modulates over
    /// the four ADSR phases, but returned
    /// as a float.
    /// </summary>
    public float ValueAsInt { get { return (int)Value; } }

    /// <summary>
    /// This represents the time that has
    /// occurred over the course of all 
    /// phases. It resets to 0 once Value
    /// returns to defaultValue.
    /// </summary>
    public double TotalTime { get; private set; }

    /// <summary>
    /// This represents the time that has
    /// occurred over the course of this 
    /// phase. It resets to 0 at the start
    /// of each phase. Think of this as
    /// the time axis on a 2D graph for your
    /// formulas.
    /// </summary>
    public double StateTime { get; private set; }

    /// <summary>
    /// Used to calculate Value during 
    /// Attack phase. CalcEngine's custom 
    /// type for processing excel formulas.
    /// Converted from strings and saved to 
    /// improve processing.
    /// </summary>
    private Expression AttackExpression { get; set; }

    /// <summary>
    /// Used to calculate Value during 
    /// Decay phase. CalcEngine's custom 
    /// type for processing excel formulas.
    /// Converted from strings and saved to 
    /// improve processing.
    /// </summary>
    private Expression DecayExpression { get; set; }

    /// <summary>
    /// Used to calculate Value during 
    /// Release phase. CalcEngine's custom 
    /// type for processing excel formulas.
    /// Converted from strings and saved to 
    /// improve processing.
    /// </summary>
    private Expression ReleaseExpression { get; set; }

	/***************************************/
	/*               METHODS               */
	/***************************************/

    /// <summary>
    /// Initialize calculation engine and calculation expressions.
    /// </summary>
	private void Awake() {
        // Check for errors with public inspector properties
        CheckForValidInspectorVariables();

        // Initialize calculation engine
        calcEngine = new CalcEngine.CalcEngine();
        calcEngine.Variables[PARAMETER_NAME] = StateTime;

        // Initialize expressions
        // Sustain does not require an expression as Value remains constant
        AttackExpression  = calcEngine.Parse(attackFormula);
        DecayExpression   = calcEngine.Parse(decayFormula);
        ReleaseExpression = calcEngine.Parse(releaseFormula);

        // Initialize Value
        Value = defaultValue;
    }

    /// <summary>
    /// Updates the parameter if UpdateType is UPDATE.
    /// </summary>
	private void Update() {
		if (UpdateType == UPDATE_TYPES.UPDATE) {
            UpdateADSR();
		}
	}

    /// <summary>
    /// Updates the parameter if UpdateType is LATE_UPDATE.
    /// </summary>
    private void LateUpdate() {
        if (UpdateType == UPDATE_TYPES.LATE_UPDATE) {
            UpdateADSR();
        }
    }

    /// <summary>
    /// Updates the parameter if UpdateType is FIXED_UPDATE.
    /// </summary>
    private void FixedUpdate() {
        if (UpdateType == UPDATE_TYPES.FIXED_UPDATE) {
            UpdateADSR();
        }
    }

    /// <summary>
    /// Enables input to trigger ADSR envelope.
    /// </summary>
    private void OnEnable() {
        EnableInput();
    }

    /// <summary>
    /// Disables input to trigger ADSR envelope.
    /// </summary>
    private void OnDisable() {
        DisableInput();
    }

    /// <summary>
    /// These are the abstract methods that must be implemented when
    /// inherited by a child. These customize the input that triggers
    /// the ADSR envelope and handles when the parameter is updated.
    /// </summary>
    protected abstract void EnableInput();
    protected abstract void DisableInput();

    /// <summary>
    /// Updates Value, StateTime, and TotalTime based on current state.
    /// This should be called in Update/LateUpdate/FixedUpdate depending
    /// on how your own update methods are setup and what the ADSR
    /// variable is for.
    /// </summary>
    protected void UpdateADSR() {
        // Update Value based on state
        switch (State) {
            case ADSR_STATE.ATTACK:
                UpdateValue(AttackExpression);
                break;
            case ADSR_STATE.DECAY:
                UpdateValue(DecayExpression);
                break;
            case ADSR_STATE.SUSTAIN:
                // Sustain does not require any changes to value.
                // We literally want to keep it constant.
                // Just update time.
                StateTime += DeltaTime();
                TotalTime += DeltaTime();
                break;
            case ADSR_STATE.RELEASE:
                UpdateValue(ReleaseExpression);
                break;
        }
    }

    /// <summary>
    /// Triggered by an input event. Changes state to Attack.
    /// This should be triggered by input setup in EnableInput.
    /// </summary>
    /// <param name="callbackContext">The input callback context.</param>
    protected void Attack(InputAction.CallbackContext callbackContext) {
        ChangeToNextState(ADSR_STATE.ATTACK);
    }

    /// <summary>
    /// Triggered by an input event. Changes state to Attack.
    /// This should be triggered by input setup in EnableInput.
    /// </summary>
    /// <param name="callbackContext">The input callback context.</param>
    protected void Release(InputAction.CallbackContext callbackContext) {
        ChangeToNextState(ADSR_STATE.RELEASE);
    }

    /// <summary>
    /// Halts state transitions and resets to default value.
    /// This "cancels" the phase approach to the parameter.
    /// This grants outside scripts some control over state, but only
    /// setting the state back to nothing.
    /// </summary>
    public void StopSignal() {
        ChangeToNextState(ADSR_STATE.NONE);
    }

    /// <summary>
    /// This function simulates and stores the ADSR envelope.
    /// Every Value and corresponding timestamp is saved in an ADSRGraphLine
    /// object as we quickly run through all of the frames necessary to
    /// compute Value.
    /// </summary>
    /// <param name="sustainTime">This simulates how long the input is held down for before it is released.</param>
    /// <returns>Every Value/TimeStamp pair that occurs in the envelope in a line format.</returns>
    public ADSRLine Simulate(float sustainTime) {
        // Prepare simulated line and start attack
        ADSRLine simulatedLine = new ADSRLine();
        ChangeToNextState(ADSR_STATE.ATTACK, false);

        /*
         * State will always equal ATTACK at the start of this while loop.
         * The ADSR envelope will continue to transition to decay and on
         * to sustain and so on until the envelope finishes release.
         * Value, StateTime, and TotalTime will be saved as this occurs
         * and returned to the caller in a line format.
         */
        while (State != ADSR_STATE.NONE) {
            /* 
             * Update Value, StateTime, and TotalTime.
             * This also takes care of all state transitions (except for
             * the sustain -> release transition) and saves the Value at
             * this point in time in the simulated line.
             */
            UpdateADSR();
            simulatedLine.Add(new ADSRPoint(State, Value, TotalTime, StateTime));

            /* 
             * UpdateADSR automatically handles most state transitions,
             * but SUSTAIN normally ends once an input is released. This
             * simulate function "releases" once sustain has occurred for
             * an amount of time determined by the function call parameter.
             */
            if (State == ADSR_STATE.SUSTAIN && StateTime >= sustainTime) {
                ChangeToNextState(ADSR_STATE.RELEASE, false);
			}
        }

        // Return all of the Values and timestamps for this ADSR envelope in a line format
        return simulatedLine;
    }

    /// <summary>
    /// Updates the time in this state and the value based on expression and time.
    /// Automatically triggers transition to next state when we hit target value.
    /// </summary>
    /// <param name="expression">The formula to use to calculate the changes to Value.</param>
    private void UpdateValue(Expression expression) {
        StateTime += DeltaTime();
        TotalTime += DeltaTime();
        Value += CalculateDelta(expression);

        // Change to the next state once we hit target value or release input.
        // To change from sustain to release, we need to check for a release.
        if (HitStateTarget()) {
            ChangeToNextState(GetNextState());
        }
    }

    /// <summary>
    /// This returns the correct delta time.
    /// Each ADSR variable must have a selected update method type.
    /// </summary>
    /// <returns>The time since last update</returns>
    private float DeltaTime() {
        switch (UpdateType) {
            case UPDATE_TYPES.UPDATE: 
                return Time.deltaTime;

            case UPDATE_TYPES.LATE_UPDATE: 
                return Time.deltaTime;

            case UPDATE_TYPES.FIXED_UPDATE:
                return Time.fixedDeltaTime;

            default:
                Error("DeltaTime default occurred. UpdateType has invalid value.");
                return 0;
        }
	}

    /// <summary>
    /// Handles changing the state from one to another. 
    /// Checks for errors along the way and invokes callbacks.
    /// </summary>
    /// <param name="toState">The state we want to be in</param>
    /// <param name="triggerCallbacks">An optional parameter that invokes UnityEvents if callbacks are enabled. Default true.</param>
    private void ChangeToNextState(ADSR_STATE toState, bool triggerCallbacks = true) {
        // Immediately change state and reset state time
        Debug.Log("ChangeToNextState --- FromState:" + State + ", ToState:" + toState + ", TotalTime:" + TotalTime + ", StateTime:" + StateTime);
        State = toState;
        StateTime = 0.0f;

        // Reset total time if we have returned to default state
        if (State == ADSR_STATE.NONE) {
            TotalTime = 0.0f;
		}

        // Check if everything is set up to skip this state
        // This is used if we want no decay and have hit decayTarget
        // CANNOT SKIP THE SUSTAIN STATE
        if (State != ADSR_STATE.SUSTAIN && HitStateTarget()) {
            ChangeToNextState(GetNextState(), triggerCallbacks);
            return;
        }
        // Callback functions if we don't skip the state
        else if (triggerCallbacks) {
            // Always invoke this callback no matter what state is changed
            ADSRStateChange.Invoke();

            // State-specific callbacks
            switch (toState) {
                case ADSR_STATE.ATTACK:
                    ADSRAttack.Invoke();
                    break;
                case ADSR_STATE.DECAY:
                    ADSRDecay.Invoke();
                    break;
                case ADSR_STATE.SUSTAIN:
                    ADSRSustain.Invoke();
                    break;
                case ADSR_STATE.RELEASE:
                    ADSRRelease.Invoke();
                    break;
                case ADSR_STATE.NONE:
                    ADSREnd.Invoke();
                    break;
            }
		}
    }

    /// <summary>
    /// Checks if we hit the target value for this state.
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
                return HitTarget((float)defaultValue, false);

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
        if (increasingValue && Value > target) {
            Value = target;
            return true;
        }
        if (!increasingValue && Value < target) {
            Value = target;
            return true;
        }

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
    /// Uses CalcEngine to calculate an expression derived from an inspector formula.
    /// </summary>
    /// <param name="expression">The expression used to calculate.</param>
    /// <returns>A new value based on the expression and time in this phase.</returns>
    private double CalculateDelta(Expression expression) {
        // Must update the Value variable stored in the CalcEngine
        calcEngine.Variables[PARAMETER_NAME] = StateTime;

        // Calculate the result of the expression
        var result = calcEngine.Evaluate(expression);

        // Return delta
        return (double)result;
    }

    /// <summary>
    /// Runs some fixed checks to see if user error occured while setting up the Inspector variables.
    /// </summary>
    private void CheckForValidInspectorVariables() {
        // Errors
        if (attackTarget <= defaultValue) {
            Error("attackTarget(" + attackTarget + ") is less than or equal to defaultValue(" + defaultValue + "). Increase attackTarget or decrease defaultValue.");
        }
        if (attackTarget < sustainTarget) {
            Error("attackTarget(" + attackTarget + ") is less than sustainTarget(" + sustainTarget + "). Increase attackTarget or decrease sustainTarget.");
        }
        if (sustainTarget <= defaultValue) {
            Error("sustainTarget(" + sustainTarget + ") is less than or equal to defaultValue(" + defaultValue + "). Increase sustainTarget or decrease defaultValue.");
        }

        // Warnings
        if (attackTarget == sustainTarget) {
            Warning("attackTarget(" + attackTarget + ") is equal to sustainTarget(" + sustainTarget + "). Decay phase will be skipped. This might not be intended.");
        }
    }

    /// <summary>
    /// A simpler way to log errors in Unity.
    /// Also halts ADSR envelope updates.
    /// </summary>
    /// <param name="error">The error text to present to the user.</param>
    protected void Error(string error) {
        Debug.LogError(error);
        StopSignal();
    }

    /// <summary>
    /// A simpler way to log warnings in Unity.
    /// </summary>
    /// <param name="warning">The warning text to present to the user.</param>
    protected void Warning(string warning) {
        Debug.LogWarning(warning);
	}

    /***************************************/
    /*              COROUTINES             */
    /***************************************/
}
