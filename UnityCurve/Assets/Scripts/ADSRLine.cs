

/*******************************************/
/*                  INCLUDES               */
/*******************************************/
using System.Collections.Generic;

/*******************************************/
/*                    CLASS                */
/*******************************************/
/// <summary>
/// A  wrapper around a List<ADSRPoint>.
/// The added functionality is an event that
/// triggers every time the List<> is
/// modified in some way.
/// </summary>
public class ADSRLine {

	/***************************************/
	/*               MEMBERS               */
	/***************************************/
	public delegate void OnLineChangeDelegate();
	public event OnLineChangeDelegate OnLineChange;

	/***************************************/
	/*              PROPERTIES             */
	/***************************************/
	/// <summary>
	/// The ADSR points that represent the line.
	/// </summary>
	public List<ADSRPoint> Points { get; private set; }

	/***************************************/
	/*               METHODS               */
	/***************************************/
	/// <summary>
	/// Constructor initializes points list.
	/// </summary>
	public ADSRLine() {
		Points = new List<ADSRPoint>();
	}

	/// <summary>
	/// Adds a point to the line and alerts listeners to change.
	/// </summary>
	public void Add(ADSRPoint point) {
		Points.Add(point);
		OnLineChange?.Invoke();
	}

	/// <summary>
	/// Clears the line data and alerts listeners to change.
	/// </summary>
	public void Clear() {
		Points.Clear();
		OnLineChange?.Invoke();
	}

}


/*******************************************/
/*                   STRUCT                */
/*******************************************/
/// <summary>
/// A simple struct that records all of the 
/// important data in an ADSR envelope at a 
/// specific point in time.
/// </summary>
public struct ADSRPoint {

	/***************************************/
	/*              PROPERTIES             */
	/***************************************/
	/// <summary>
	/// The ADSR state at this point in the envelope.
	/// </summary>
	public ADSR_STATE State { get; private set; }

	/// <summary>
	/// The ADSR value at this point in the envelope.
	/// </summary>
	public float Value { get; private set; }

	/// <summary>
	/// The ADSR total state time at this point in the envelope.
	/// </summary>
	public float TotalTime { get; private set; }

	/// <summary>
	/// The ADSR time at this point since the start of the most recent state.
	/// </summary>
	public float StateTime { get; private set; }

	/***************************************/
	/*               METHODS               */
	/***************************************/
	/// <summary>
	/// Constructor that stores all ADSR values at a point in time.
	/// Makes it easier to not cast parameters to float every time constructor is called.
	/// </summary>
	/// <param name="state">The ADSR state at this point in the envelope.</param>
	/// <param name="value">The ADSR value at this point in the envelope.</param>
	/// <param name="totalTime">The ADSR total state time at this point in the envelope.</param>
	/// <param name="stateTime">The ADSR time at this point since the start of the most recent state.</param>
	public ADSRPoint(ADSR_STATE state, double value, double totalTime, double stateTime) {
		State = state;
		Value = (float)value;
		TotalTime = (float)totalTime;
		StateTime = (float)stateTime;
	}

	/// <summary>
	/// Constructor that stores all ADSR values at a point in time.
	/// </summary>
	/// <param name="state">The ADSR state at this point in the envelope.</param>
	/// <param name="value">The ADSR value at this point in the envelope.</param>
	/// <param name="totalTime">The ADSR total state time at this point in the envelope.</param>
	/// <param name="stateTime">The ADSR time at this point since the start of the most recent state.</param>
	public ADSRPoint(ADSR_STATE state, float value, float totalTime, float stateTime) {
		State = state;
		Value = value;
		TotalTime = totalTime;
		StateTime = stateTime;
	}
}