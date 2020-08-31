

/***********************************************/
/*                  INCLUDES                   */
/***********************************************/
using System.Collections.Generic;

/***********************************************/
/*                   CLASS                     */
/***********************************************/
namespace UnityCurve.UI {
	/// <summary>
	/// A  wrapper around a List<ADSR_Point>.
	/// The added functionality is an event that
	/// triggers every time the List<> is
	/// modified in some way.
	/// </summary>
	public class ADSR_Line {

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
		public List<ADSR_Point> Points { get; private set; }

		/// <summary>
		/// A property that makes it easy to get the last point in the line.
		/// </summary>
		public ADSR_Point? LastPoint {
			get {
				if (Points.Count == 0) return null;
				return Points[Points.Count - 1];
			}
		}

		/***************************************/
		/*               METHODS               */
		/***************************************/
		/// <summary>
		/// Constructor initializes points list.
		/// </summary>
		public ADSR_Line() {
			Points = new List<ADSR_Point>();
		}

		/// <summary>
		/// Adds a point to the line and alerts listeners to change.
		/// </summary>
		public void Add(ADSR_Point point) {
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
}


/*******************************************/
/*                   STRUCT                */
/*******************************************/
namespace UnityCurve {
	/// <summary>
	/// A simple struct that records all of the 
	/// important data in an ADSR envelope at a 
	/// specific point in time.
	/// </summary>
	public struct ADSR_Point {

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
		public ADSR_Point(ADSR_STATE state, double value, double totalTime, double stateTime) {
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
		public ADSR_Point(ADSR_STATE state, float value, float totalTime, float stateTime) {
			State = state;
			Value = value;
			TotalTime = totalTime;
			StateTime = stateTime;
		}
	}
}