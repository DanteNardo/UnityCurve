

/***********************************************/
/*                   INCLUDES                  */
/***********************************************/
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/***********************************************/
/*                     CLASS                   */
/***********************************************/
namespace UnityCurve.UI {

	/// <summary>
	/// A robust script for controlling UI graph
	/// elements and rendering CurvePoints that
	/// are created by a UnityCurve. This graph 
	/// is primarily for debugging or visual
	/// feedback while creating curves.
	/// </summary>
	public class UIGraph : MonoBehaviour {

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
		/// The container for the UnityCurve graph fields.
		/// </summary>
		public UIGraphFields graphFields;

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

		/***************************************/
		/*               METHODS               */
		/***************************************/

		/// <summary>
		/// Initialies graph data. If the graph is not realtime, then performs all calculations necessary to display static graph.
		/// </summary>
		private void Start() {
			// Instantiate new graph line variables
			Line = new CurvePoints();
			Line.OnLineChange += UpdateColors;
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
			graphFields.Clear();
		}

		/// <summary>
		/// Adds a point to the graph based on current UnityCurve parameter status and updates axises.
		/// </summary>
		public void AddPoint() {
			if (y.Active) {
				Line.Add(new CurvePoint(y.CurrentCurve, y.Value, y.TotalCurveTime, y.CurrentCurveTime));
				UpdateYAxisValues();
				UpdateAxisText();
			}
		}

		/// <summary>
		/// Updates the UILineRenderer's points and forces re-render.
		/// </summary>
		private void UpdateRenderer() {
			lineRenderer.Line.SetEqual(GetGraphPoints());
		}

		/// <summary>
		/// Records points where the line color should change on the graph.
		/// Determines color transition points between different curves.
		/// </summary>
		private void UpdateColors() {
			// Don't update colors if less than two points
			if (Line.Points.Count < 2) {
				return;
			}

			// Prepare variables for iteration and add initial color point
			int curveCount = 0;
			Curve lastCurve = null;
			UIGraphField field = null;
			lineRenderer.ColorPoints.Clear();
			lineRenderer.ColorPoints.Add(new UIColorPoint(Line.Points[0].CurveAtPoint.curveColor, 0));

			// Create GradientKeys when the Curve along a UnityCurve changes
			for (int i = 1; i < Line.Points.Count; i++) {
				if (Line.Points[i].CurveAtPoint != lastCurve) {
					curveCount++;
					lineRenderer.ColorPoints.Add(new UIColorPoint(Line.Points[i - 1].CurveAtPoint.curveColor, i - 1));
					lineRenderer.ColorPoints.Add(new UIColorPoint(Line.Points[i].CurveAtPoint.curveColor, i));
					field = graphFields.GetCurveField(Line.Points[i].CurveAtPoint, curveCount);
					field.SetDuration(Line.Points[i].CurveTime.ToString("0.##") + "s");
					field.SetTotalTime(Line.Points[i].TotalTime.ToString("0.##") + "s");
				}
				else {
					field.SetDuration(Line.Points[i].CurveTime.ToString("0.##") + "s");
				}

				// Update last curve
				lastCurve = Line.Points[i].CurveAtPoint;
			}
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

		/// <summary>
		/// Updates lowest and highest Y values if necessary whenever the graph receives a new Point.
		/// </summary>
		private void UpdateYAxisValues() {
			if (y.Value < LowestY) LowestY = y.Value;
			if (y.Value > HighestY) HighestY = y.Value;
		}

		/// <summary>
		/// Updates the UI text that displays the X and Y axis min and max values.
		/// </summary>
		private void UpdateAxisText() {
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