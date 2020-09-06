

/***********************************************/
/*                   INCLUDES                  */
/***********************************************/
using UnityEngine;
using System.Collections.Generic;

/***********************************************/
/*                     CLASS                   */
/***********************************************/
namespace UnityCurve.UI {

	/// <summary>
	/// A wrapper around a List<Vector2>.
	/// The added functionality is an event that
	/// triggers every time the List<> is
	/// modified in some way.
	/// </summary>
	public class UILine {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/
		public delegate void OnLineChangeDelegate();
		public event OnLineChangeDelegate OnLineChange;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/

		/// <summary>
		/// The Vector2 points that represent the
		/// UI line.
		/// </summary>
		public List<Vector2> Points { get; private set; }

		/***************************************/
		/*               METHODS               */
		/***************************************/

		/// <summary>
		/// Constructor initializes points list.
		/// </summary>
		public UILine() {
			Points = new List<Vector2>();
		}

		/// <summary>
		/// Sets all points in the line and alerts listeners to change.
		/// </summary>
		public void SetEqual(List<Vector2> points) {
			Points = points;
			OnLineChange?.Invoke();
		}

		/// <summary>
		/// Adds multiple points to the line and alerts listeners to change.
		/// </summary>
		public void AddRange(List<Vector2> points) {
			Points.AddRange(points);
			OnLineChange?.Invoke();
		}

		/// <summary>
		/// Adds a point to the line and alerts listeners to change.
		/// </summary>
		public void Add(Vector2 point) {
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