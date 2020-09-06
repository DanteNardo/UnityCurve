

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

    /// <summary>
    /// Determines how frequently a UnityCurve 
    /// is updated by the game engine.
    /// </summary>
    public enum UPDATE_TYPES {
        UPDATE,
        FIXED_UPDATE,
        LATE_UPDATE
    }

    /// <summary>
    /// Determines which input callback is used
    /// when invocating functions inside of
    /// InputCurves and CurveTransitions.
    /// </summary>
    public enum INPUT_CALLBACK {
        STARTED,
        PERFORMED,
        CANCELED
    }
}

/***********************************************/
/*                     CLASS                   */
/***********************************************/
namespace UnityCurve {

    /// <summary>
    /// The point of this class is to easily add
    /// continuous curves of varying shapes to
    /// the modulation of a value based on input.
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
        /// Named like a constant to enforce not
        /// changing, but accessible in inspector.
        /// 
        /// YOU MUST CHANGE THE STRING IN YOUR 
        /// CURVE FORMULAS TO MATCH IF YOU CHANGE
        /// THIS VARIABLE'S VALUE.
        /// 
        /// This parameter cannot be a name
        /// for any built-in Excel function. If
        /// it is, you will get a CalcEngine
        /// error saying there are too few
        /// parameters.
        /// </summary>
        public string PARAMETER_NAME = "X";

        /// <summary>
        /// This determines which update method 
        /// this variable uses. Consequently, it 
        /// also determines which type of 
        /// deltaTime to use for our time changes.
        /// </summary>
        public UPDATE_TYPES UpdateType = UPDATE_TYPES.UPDATE;

        /// <summary>
        /// This is the default Y value for this 
        /// UnityCurve. This is also the minimum 
        /// value and Value can never be lower 
        /// than it.
        /// </summary>
        public double defaultValue = 0;

        /// <summary>
        /// A utility unityEvent that is invoked
        /// when this UnityCurve is activated.
        /// </summary>
        public UnityEvent activationEvent;

        /// <summary>
        /// A utility unityEvent that is invoked
        /// when this UnityCurve is deactivated.
        /// </summary>
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

        /// <summary>
        /// Determines whether or not this 
        /// UnityCurve is actively updating or
        /// is the default value.
        /// </summary>
        public bool Active { get { return CurrentCurve != null; } }

        /// <summary>
        /// Null, unless this UnityCurve is active
        /// in which case it is the current
        /// active curve on this UnityCurve.
        /// </summary>
        public Curve CurrentCurve { get; private set; } = null;

        /// <summary>
        /// Null, unless this UnityCurve is active
        /// in which case it is the curve that will
        /// be transitioned to next if the current
        /// curve's transition condition is met.
        /// </summary>
        public Curve NextCurve { get { return CurrentCurve == null ? null : CurrentCurve.nextCurve; } }

        /// <summary>
        /// The current Value of this UnityCurve 
        /// based on any previously active curves
        /// and the current active curve.
        /// </summary>
        public double Value { get { return PreviousCurveValue + CurrentCurveValue; } }

        /// <summary>
        /// A necessary variable to make Value
        /// continous across Curve changes.
        /// </summary>
        private double PreviousCurveValue { get; set; }

        /// <summary>
        /// A necessary variable to make Value
        /// continous across Curve changes.
        /// </summary>
        private double CurrentCurveValue { get; set; }

        /// <summary>
        /// The total time that this UnityCurve 
        /// has been active. Resets to 0 once
        /// Value = defaultY.
        /// </summary>
        public double TotalCurveTime { get; private set; }

        /// <summary>
        /// The total time that CurrentCurve 
        /// has been active. Resets to 0 when
        /// the CurrentCurve changes to NextCurve.
        /// </summary>
        public double CurrentCurveTime { get; private set; }

		/***************************************/
		/*               METHODS               */
		/***************************************/

        /// <summary>
        /// Initialize the calculation engine and UnityCurve values.
        /// </summary>
		private void Awake() {
            // Initialize calculation engine
            CalculationEngine = new CalcEngine.CalcEngine();
            CalculationEngine.Variables[PARAMETER_NAME] = 0;

            // Initialize Value
            PreviousCurveValue = defaultValue;
            CurrentCurveValue = 0;
        }

        /// <summary>
        /// Activates this UnityCurve and begins curve modulation.
        /// Initializes variables.
        /// </summary>
        /// <param name="firstCurve">The starting curve for this UnityCurve.</param>
        public void Activate(Curve firstCurve) {
            CurrentCurve = firstCurve;
            CurrentCurve.Activate();
            CurrentCurveTime = 0;
            TotalCurveTime = 0;
            PreviousCurveValue = defaultValue;
            CurrentCurveValue = 0;
            activationEvent.Invoke();
		}

        /// <summary>
        /// Dectivates this UnityCurve and ends curve modulation.
        /// Resets all variables.
        /// </summary>
        public void Deactivate() {
            CurrentCurve.Deactivate();
            CurrentCurve = null;
            CurrentCurveTime = 0;
            TotalCurveTime = 0;
            PreviousCurveValue = defaultValue;
            CurrentCurveValue = 0;
            deactivationEvent.Invoke();
        }

        /// <summary>
        /// Syntactic sugar wrapped around ChangeToCurve.
        /// </summary>
        public void ChangeToNextCurve() {
            ChangeToCurve(NextCurve);
        }

        /// <summary>
        /// Based on current state, either deactivates this UnityCurve, activates this UnityCurve, or switches to a curve while already active.
        /// </summary>
        /// <param name="curveToChangeTo">The desired curve to change to. If null, deactivates this UnityCurve.</param>
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

        /// <summary>
        /// A function that overrides the CurrentValue. Usually used to set a value to be perfectly equal to a target instead of approximately equal.
        /// </summary>
        /// <param name="value">The desired value.</param>
        public void SetValue(double value) {
            CurrentCurveValue = value - PreviousCurveValue;
        }

        /// <summary>
        /// Updates the times and current curve value based on given expression.
        /// </summary>
        /// <param name="expression">The expression to use with CalcEngine to set CurrentCurveValue.</param>
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

        /// <summary>
        /// A function that returns the correct DeltaTime based on this UnityCurve's UpdateType.
        /// </summary>
        /// <returns>Time.deltaTime or Time.fixedDeltaTime depending on UpdateType.</returns>
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
