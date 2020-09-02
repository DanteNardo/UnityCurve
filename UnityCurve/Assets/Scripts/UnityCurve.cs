

/***********************************************/
/*                   INCLUDES                  */
/***********************************************/
using CalcEngine;
using UnityEngine;
using UnityEngine.Events;

/***********************************************/
/*                     ENUM                    */
/***********************************************/
namespace UnityCurve {
    public enum UPDATE_TYPES {
        UPDATE,
        FIXED_UPDATE,
        LATE_UPDATE
    }

    public enum VALUE_TYPE {
        ADDITIVE,
        EQUALITIVE
    }
}

/***********************************************/
/*                     CLASS                   */
/***********************************************/
namespace UnityCurve {
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
    public class UnityCurve : MonoBehaviour {

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
        /// This is the default Y value for this 
        /// UnityCurve. This is also the minimum 
        /// value and Value can never be lower 
        /// than it.
        /// </summary>
        public double defaultValue = 0;

        public UnityEvent activationEvent;
        public UnityEvent deactivationEvent;

        /***************************************/
        /*              PROPERTIES             */
        /***************************************/

        /// <summary>
        /// https://github.com/Bernardo-Castilho/CalcEngine
        /// This is an Open-Source excel formula 
        /// calculation engine developed by 
        /// Bernardo-Castilho and licensed under 
        /// the MIT license.
        /// </summary>
        public CalcEngine.CalcEngine CalculationEngine { get; private set; }

        public bool Active { get { return CurrentCurve != null; } }

        public Curve CurrentCurve { get; private set; } = null;

        public Curve NextCurve { get { return CurrentCurve == null ? null : CurrentCurve.nextCurve; } }

        public double Value { get { return PreviousCurveValue + CurrentCurveValue; } }

        private double PreviousCurveValue { get; set; }

        private double CurrentCurveValue { get; set; }

        /// <summary>
        /// The total time that this UnityCurve 
        /// has been active. Resets to 0 once
        /// Value = defaultY.
        /// </summary>
        public double TotalCurveTime { get; private set; }

        public double CurrentCurveTime { get; private set; }

		/***************************************/
		/*               METHODS               */
		/***************************************/

		private void Awake() {
            // Initialize calculation engine
            CalculationEngine = new CalcEngine.CalcEngine();
            CalculationEngine.Variables[PARAMETER_NAME] = 0;

            // Initialize Value
            PreviousCurveValue = defaultValue;
            CurrentCurveValue = 0;
        }

        public void Activate(Curve firstCurve) {
            CurrentCurve = firstCurve;
            CurrentCurve.Activate();
            CurrentCurveTime = 0;
            TotalCurveTime = 0;
            PreviousCurveValue = defaultValue;
            CurrentCurveValue = 0;
            activationEvent.Invoke();
		}

        public void Deactivate() {
            CurrentCurve.Deactivate();
            CurrentCurve = null;
            CurrentCurveTime = 0;
            TotalCurveTime = 0;
            PreviousCurveValue = defaultValue;
            CurrentCurveValue = 0;
            deactivationEvent.Invoke();
        }

        public void ChangeToNextCurve() {
            ChangeToCurve(NextCurve);
        }

        public void ChangeToCurve(Curve curveToChangeTo) {
            // Deactivate this UnityCurve once we hit start index
            if (curveToChangeTo == null && Active) {
                Deactivate();
            }

            // Check if this UnityCurve has not been activated yet
            if (curveToChangeTo != null && !Active) {
                Activate(curveToChangeTo);
			}

            // Normal case: deactivate current curve and activate next
            if (curveToChangeTo != null && Active) {
                PreviousCurveValue = Value;
                CurrentCurve.Deactivate();
                CurrentCurve = curveToChangeTo;
                CurrentCurve.Activate();
                CurrentCurveValue = 0;
                CurrentCurveTime = 0;
            }
        }

        public void SetValue(double value) {
            CurrentCurveValue = value - PreviousCurveValue;
        }

        public void UpdateCurrentCurveValue(Expression expression) {
            CurrentCurveTime += DeltaTime();
            TotalCurveTime += DeltaTime();
            CurrentCurveValue = Calculate(expression);
        }

		/// <summary>
		/// Uses CalcEngine to calculate an expression derived from an inspector formula.
		/// </summary>
		/// <param name="expression">The expression used to calculate.</param>
		/// <returns>A new value based on the expression and time in this phase.</returns>
		private double Calculate(Expression expression) {
			// Must update the Value variable stored in the CalcEngine
			CalculationEngine.Variables[PARAMETER_NAME] = CurrentCurveTime;

			// Calculate the result of the expression
			return (double)CalculationEngine.Evaluate(expression);
		}

		private float DeltaTime() {
            switch (UpdateType) {
                case UPDATE_TYPES.UPDATE: return Time.deltaTime;
                case UPDATE_TYPES.LATE_UPDATE: return Time.deltaTime;
                case UPDATE_TYPES.FIXED_UPDATE: return Time.fixedDeltaTime;

                default:
                    Debug.LogError("DeltaTime default occurred. UpdateType has invalid value.");
                    return 0;
            }
        }
    }
}
