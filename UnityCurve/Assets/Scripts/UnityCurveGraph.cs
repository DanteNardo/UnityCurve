

/*******************************************/
/*                  INCLUDES               */
/*******************************************/
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*******************************************/
/*                   CLASS                 */
/*******************************************/
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
		/// If true, the Graph will visualize the 
		/// live ADSR envelope as it modulates.
		/// If false, the Graph will visualize a 
		/// still of the entire ADSR envelope.
		/// </summary>
		public bool realtime = true;

		public float simulationInputTime = 0.5f;

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
		/// The color of the Attack portion of the line.
		/// </summary>
		public Color color;

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
		private CurvePoints Line { get; set; }

		/// <summary>
		/// A necessary variable for normalizing the y values of the graph.
		/// </summary>
		private float MaxYHeight { get { return (float)(HighestY - LowestY); } }

		private double HighestY { get; set; }
		private double LowestY { get; set; }

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

			// If this is a static graph, set all data now
			if (realtime == false) {
				Line = y.Simulate(simulationInputTime);
				StaticTimeAnalysis();
				UpdateColorPoints();
				UpdateRenderer();
			}
		}

		/// <summary>
		/// Updates the graph if the graph is a realtime graph.
		/// </summary>
		private void FixedUpdate() {
			if (realtime) {
				AddPoint();
			}
		}

		/// <summary>
		/// Clears all graph data.
		/// </summary>
		public void Clear() {
			// Clear line data
			Line.Clear();
			lineRenderer.Line.Clear();
			lineRenderer.ColorPoints.Clear();
		}

		/// <summary>
		/// Adds a point to the graph based on current UnityCurve parameter status.
		/// </summary>
		public void AddPoint() {
			if (y.Active) {
				Line.Add(new CurvePoint(y.Value, y.TotalCurveTime, y.CurrentCurveTime));
				//attackDurationText.text = y.StateTime.ToString("0.##") + "s";
				//attackTotalTimeText.text = y.TotalTime.ToString("0.##") + "s";

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
			//// Prepare variables for iteration and add initial color point
			//CURVE_STATE lastState = CURVE_STATE.NONE;
			//lineRenderer.ColorPoints.Clear();
			//lineRenderer.ColorPoints.Add(new UIColorPoint(attackColor, 0));

			//// Iterate and create GradientKeys when state changes
			//for (int i = 0; i < Line.Points.Count; i++) {
			//	if (Line.Points[i].State != lastState) {
			//		switch (Line.Points[i].State) {
			//			case CURVE_STATE.DECAY:
			//				lineRenderer.ColorPoints.Add(new UIColorPoint(attackColor, i - 1));
			//				lineRenderer.ColorPoints.Add(new UIColorPoint(decayColor, i));
			//				break;
			//			case CURVE_STATE.SUSTAIN:
			//				lineRenderer.ColorPoints.Add(new UIColorPoint(decayColor, i - 1));
			//				lineRenderer.ColorPoints.Add(new UIColorPoint(sustainColor, i));
			//				break;
			//			case CURVE_STATE.RELEASE:
			//				lineRenderer.ColorPoints.Add(new UIColorPoint(sustainColor, i - 1));
			//				lineRenderer.ColorPoints.Add(new UIColorPoint(releaseColor, i));
			//				break;
			//			case CURVE_STATE.NONE:
			//				lineRenderer.ColorPoints.Add(new UIColorPoint(releaseColor, i));
			//				break;
			//		}
			//	}

			//	// Update last state
			//	lastState = Line.Points[i].State;
			//}
		}

		/// <summary>
		/// Sets the correct duration and total time values in the UI for a static UnityCurve Graph.
		/// </summary>
		private void StaticTimeAnalysis() {
			foreach (var point in Line.Points) {
				UpdateYAxisValues(point);
			}

			UpdateAxises();
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
			return (float)(y - this.y.defaultValue) / MaxYHeight * gridRenderer.gridSize.y;
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