

/***********************************************/
/*                   INCLUDES                  */
/***********************************************/
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/***********************************************/
/*                     CLASS                   */
/***********************************************/
/// <summary>
/// A robust script for controlling UI graph
/// elements and rendering an CurvePoints that
/// is created by a UnityCurve. This 
/// graph is primarily for debugging.
/// </summary>
namespace UnityCurve {
	public class UnityCurveGraph : MonoBehaviour {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/

		/// <summary>
		/// The y axis of the Graph is the value of the UnityCurve at a point in time (X).
		/// </summary>
		public UnityCurve y;

		/// <summary>
		/// The grid renderer for this graph.
		/// </summary>
		public UIGridRenderer gridRenderer;

		/// <summary>
		/// The line renderer for this graph.
		/// </summary>
		public UILineRenderer lineRenderer;

		/// <summary>
		/// The color array for this graph.
		/// </summary>
		public Color[] colors;

		/// <summary>
		/// The UI element that renders the minimum value on the x axis.
		/// </summary>
		public TMP_Text xAxisMinimumText;

		/// <summary>
		/// The UI element that renders the maximum value on the x axis.
		/// </summary>
		public TMP_Text xAxisMaximumText;

		/// <summary>
		/// The UI element that renders the minimum value on the y axis.
		/// </summary>
		public TMP_Text yAxisMinimumText;

		/// <summary>
		/// The UI element that renders the maximum value on the y axis.
		/// </summary>
		public TMP_Text yAxisMaximumText;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/

		/// <summary>
		/// The line data of all the points in the ADSR envelope. Rendered by UILineRenderer.
		/// </summary>
		public CurvePoints Line { get; private set; }

		/// <summary>
		/// A necessary variable for normalizing the y values of the graph.
		/// </summary>
		private float MaxYHeight { get { return (float)(HighestY - LowestY); } }

		/// <summary>
		/// The highest Y value in the graph's line. Used for normalizing Y values.
		/// </summary>
		private double HighestY { get; set; }

		/// <summary>
		/// The lowest Y value in the graph's line. Used for normalizing Y values.
		/// </summary>
		private double LowestY { get; set; }

		/// <summary>
		/// The current color of the line when processing colors.
		/// </summary>
		private Color CurrentColor { get { return colors[CurrentColorIndex]; } }

		/// <summary>
		/// The next color along the line when processing colors.
		/// </summary>
		private Color NextColor { get { return colors[NextColorIndex]; } }

		/// <summary>
		/// Keeps track of the current color index when processing colors.
		/// </summary>
		private int CurrentColorIndex { get; set; }

		/// <summary>
		/// Keeps track of the next color index when processing colors.
		/// </summary>
		private int NextColorIndex { get { return CurrentColorIndex + 1 >= colors.Length ? 0 : CurrentColorIndex + 1; } }

		/***************************************/
		/*               METHODS               */
		/***************************************/

		/// <summary>
		/// Initialies graph data. If the graph is not realtime, then performs all calculations necessary to display static graph.
		/// </summary>
		private void Start() {
			// Instantiate new graph line variables
			Line = new CurvePoints();
			Line.OnLineChange += UpdateColorPoints;
			Line.OnLineChange += UpdateRenderer;

			// Clear and set all data to empty
			Clear();

			// Prepare the Y axis variables
			LowestY = y.defaultValue;
			HighestY = y.defaultValue;
		}

		/// <summary>
		/// Updates the graph if the graph is a realtime graph.
		/// </summary>
		private void FixedUpdate() {
			AddPoint();
		}

		/// <summary>
		/// Clears all graph data.
		/// </summary>
		public void Clear() {
			Line.Clear();
			lineRenderer.Line.Clear();
			lineRenderer.ColorPoints.Clear();
		}

		/// <summary>
		/// Adds a point to the graph based on current UnityCurve parameter status.
		/// </summary>
		public void AddPoint() {
			if (y.Active) {
				Line.Add(new CurvePoint(y.CurrentCurve, y.Value, y.TotalCurveTime, y.CurrentCurveTime));

				// Update Axises
				UpdateYAxisValues();
				UpdateAxises();
			}
		}

		/// <summary>
		/// Updates the UILineRenderer's points and forces re-render.
		/// </summary>
		private void UpdateRenderer() {
			lineRenderer.Line.SetEqual(GetGraphPoints());
		}

		/// <summary>
		/// Converts the CurvePoints values to a normalized structure for the UILineRenderer.
		/// </summary>
		/// <returns>A list of Vector2 points for the UILineRenderer.</returns>
		private List<Vector2> GetGraphPoints() {
			// Create graph points object
			List<Vector2> graphPoints = new List<Vector2>();

			// Generate graph points based on time and value and then normalize them.
			// - Adjust each point's x value based on previous lines in the graph
			// - Normalize x values with normalization function
			// - Normalize y values by subtracting default value and dividing by maxY
			for (int i = 0; i < Line.Points.Count; i++) {
				graphPoints.Add(new Vector2(NormalizeX(i), NormalizeY(Line.Points[i].Value)));
			}

			// Return value
			return graphPoints;
		}

		/// <summary>
		/// Records points where the line color should change on the graph.
		/// Determines color transition points between different curves.
		/// </summary>
		private void UpdateColorPoints() {
			// Prepare variables for iteration and add initial color point
			Curve lastCurve = null;
			CurrentColorIndex = colors.Length-1;
			lineRenderer.ColorPoints.Clear();
			lineRenderer.ColorPoints.Add(new UIColorPoint(CurrentColor, 0));

			// Create GradientKeys when the Curve along a UnityCurve changes
			for (int i = 0; i < Line.Points.Count; i++) {
				if (Line.Points[i].CurveAtPoint != lastCurve) {
					lineRenderer.ColorPoints.Add(new UIColorPoint(CurrentColor, i - 1));
					lineRenderer.ColorPoints.Add(new UIColorPoint(NextColor, i));
					CurrentColorIndex = NextColorIndex;
				}

				// Update last curve
				lastCurve = Line.Points[i].CurveAtPoint;
			}
		}

		/// <summary>
		/// Normalizes the X value of the graph.
		/// </summary>
		/// <param name="x">The x value to normalize.</param>
		/// <returns>A normalized version of x relative to the graph width and total points in current line.</returns>
		private float NormalizeX(int x) {
			if (Line.Points.Count == 0) return x * gridRenderer.gridSize.x;
			return (1.0f / Line.Points.Count) * x * gridRenderer.gridSize.x;
		}

		/// <summary>
		/// Normalizes the Y value of the graph.
		/// </summary>
		/// <param name="y">The y value to normalize.</param>
		/// <returns>A normalized version of y relative to the graph height and max/min ADSR values.</returns>
		private float NormalizeY(float y) {
			if (MaxYHeight == 0) return y * gridRenderer.gridSize.y;
			return (float)(y - LowestY) / MaxYHeight * gridRenderer.gridSize.y;
		}

		private void UpdateYAxisValues() {
			if (y.Value < LowestY) LowestY = y.Value;
			if (y.Value > HighestY) HighestY = y.Value;
		}

		private void UpdateYAxisValues(CurvePoint point) {
			if (point.Value < LowestY) LowestY = point.Value;
			if (point.Value > HighestY) HighestY = point.Value;
		}

		private void UpdateAxises() {
			xAxisMinimumText.text = "0.0s";
			xAxisMaximumText.text = Line.LastPoint?.TotalTime.ToString("0.##") + "s";
			yAxisMinimumText.text = LowestY.ToString("0.##");
			yAxisMaximumText.text = HighestY.ToString("0.##");
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}