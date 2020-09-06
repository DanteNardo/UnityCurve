

/***********************************************/
/*                    INCLUDES                 */
/***********************************************/
using System.Collections.Generic;

/***********************************************/
/*                     CLASS                   */
/***********************************************/
namespace UnityCurve {

	/// <summary>
	/// A  wrapper around a List<CurvePoint>.
	/// The added functionality is an event that
	/// triggers every time the List<> is
	/// modified in some way.
	/// </summary>
	public class CurvePoints {

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
		public List<CurvePoint> Points { get; private set; }

		/// <summary>
		/// A property that makes it easy to get the last point in the line.
		/// </summary>
		public CurvePoint? LastPoint {
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
		public CurvePoints() {
			Points = new List<CurvePoint>();
		}

		/// <summary>
		/// Adds a point to the line and alerts listeners to change.
		/// </summary>
		public void Add(CurvePoint point) {
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

/***********************************************/
/*                     STRUCT                  */
/***********************************************/
namespace UnityCurve {

	/// <summary>
	/// A simple struct that records all of the 
	/// important data in a Curve at a specific 
	/// point in time.
	/// </summary>
	public struct CurvePoint {

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/

		/// <summary>
		/// The active Curve at this point in the UnityCurve.
		/// </summary>
		public Curve CurveAtPoint { get; private set; }

		/// <summary>
		/// The Curve value at this point in the envelope.
		/// </summary>
		public float Value { get; private set; }

		/// <summary>
		/// The total time at this point in the UnityCurve.
		/// </summary>
		public float TotalTime { get; private set; }

		/// <summary>
		/// The time at this point since the start of the most recent curve.
		/// </summary>
		public float CurveTime { get; private set; }

		/***************************************/
		/*               METHODS               */
		/***************************************/
		/// <summary>
		/// Constructor that stores all Curve values at a point in time.
		/// Makes it easier to not cast parameters to float every time constructor is called.
		/// </summary>
		/// <param name="curveAtPoint">The active Curve at this point in the UnityCurve.</param>
		/// <param name="value">The Curve value at this point in the envelope.</param>
		/// <param name="totalTime">The Curve total state time at this point in the envelope.</param>
		/// <param name="stateTime">The Curve time at this point since the start of the most recent state.</param>
		public CurvePoint(Curve curveAtPoint, double value, double totalTime, double stateTime) {
			CurveAtPoint = curveAtPoint;
			Value = (float)value;
			TotalTime = (float)totalTime;
			CurveTime = (float)stateTime;
		}

		/// <summary>
		/// Constructor that stores all Curve values at a point in time.
		/// </summary>
		/// <param name="curveAtPoint">The active Curve at this point in the UnityCurve.</param>
		/// <param name="value">The Curve value at this point in the envelope.</param>
		/// <param name="totalTime">The Curve total state time at this point in the envelope.</param>
		/// <param name="stateTime">The Curve time at this point since the start of the most recent state.</param>
		public CurvePoint(Curve curveAtPoint, float value, float totalTime, float stateTime) {
			CurveAtPoint = curveAtPoint;
			Value = value;
			TotalTime = totalTime;
			CurveTime = stateTime;
		}
	}
}