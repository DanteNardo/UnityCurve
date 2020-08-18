

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
/// elements and rendering an ADSRLine that
/// is created by an ADSR parameter. This 
/// graph is primarily for debugging.
/// </summary>
public class ADSRGraph : MonoBehaviour {

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

	/// <summary>
	/// If realtime is false, then this is 
	/// the amount of sustain time that is 
	/// used in the static simulation of 
	/// the ADSR envelope.
	/// </summary>
	public float simulationSustainTime = 0.5f;

	/// <summary>
	/// The y axis of the Graph is the value of the ADSR parameter at a point in time (X).
	/// </summary>
	public ADSR y;

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
	public Color attackColor;

	/// <summary>
	/// The color of the Decay portion of the line.
	/// </summary>
	public Color decayColor;

	/// <summary>
	/// The color of the Sustain portion of the line.
	/// </summary>
	public Color sustainColor;

	/// <summary>
	/// The color of the Release portion of the line.
	/// </summary>
	public Color releaseColor;

	/// <summary>
	/// The UI element that renders the duration of the attack phase.
	/// </summary>
	public TMP_Text attackDurationText;

	/// <summary>
	/// The UI element that renders the duration of the decay phase.
	/// </summary>
	public TMP_Text decayDurationText;

	/// <summary>
	/// The UI element that renders the duration of the sustain phase.
	/// </summary>
	public TMP_Text sustainDurationText;

	/// <summary>
	/// The UI element that renders the duration of the release phase.
	/// </summary>
	public TMP_Text releaseDurationText;

	/// <summary>
	/// The UI element that renders the total time of the ADSR envelope at the end of the attack phase.
	/// </summary>
	public TMP_Text attackTotalTimeText;

	/// <summary>
	/// The UI element that renders the total time of the ADSR envelope at the end of the decay phase.
	/// </summary>
	public TMP_Text decayTotalTimeText;

	/// <summary>
	/// The UI element that renders the total time of the ADSR envelope at the end of the sustain phase.
	/// </summary>
	public TMP_Text sustainTotalTimeText;

	/// <summary>
	/// The UI element that renders the total time of the ADSR envelope at the end of the release phase.
	/// </summary>
	public TMP_Text releaseTotalTimeText;

	/***************************************/
	/*              PROPERTIES             */
	/***************************************/

	/// <summary>
	/// The line data of all the points in the ADSR envelope. Rendered by UILineRenderer.
	/// </summary>
	private ADSRLine Line { get; set; }

	/// <summary>
	/// A necessary variable for normalizing the y values of the graph.
	/// </summary>
	private float MaxYHeight { get; set; }

	/***************************************/
	/*               METHODS               */
	/***************************************/

	/// <summary>
	/// Initialies graph data. If the graph is not realtime, then performs all calculations necessary to display static graph.
	/// </summary>
	private void Start() {
		// Instantiate new graph line variables
		Line = new ADSRLine();
		Line.OnLineChange += UpdateColorPoints;
		Line.OnLineChange += UpdateRenderer;

		// Clear and set all data to empty
		Clear();

		// Calculate the maximum height for the graph
		MaxYHeight = (float)(y.attackTarget - y.defaultValue);

		// If this is a static graph, set all data now
		if (realtime == false) {
			Line = y.Simulate(simulationSustainTime);
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

		// Clear text data
		attackDurationText.text = "";
		decayDurationText.text = "";
		sustainDurationText.text = "";
		releaseDurationText.text = "";
		attackTotalTimeText.text = "";
		decayTotalTimeText.text = "";
		sustainTotalTimeText.text = "";
		releaseTotalTimeText.text = "";
	}

	/// <summary>
	/// Adds a point to the graph based on current ADSR parameter status.
	/// </summary>
	public void AddPoint() {
		switch (y.State) {
			case ADSR_STATE.ATTACK:
				Line.Add(new ADSRPoint(y.State, y.ExternalValue, y.TotalTime, y.StateTime));
				attackDurationText.text = y.StateTime.ToString("0.##") + "s";
				attackTotalTimeText.text = y.TotalTime.ToString("0.##") + "s";
				break;
			case ADSR_STATE.DECAY:
				Line.Add(new ADSRPoint(y.State, y.ExternalValue, y.TotalTime, y.StateTime));
				decayDurationText.text = y.StateTime.ToString("0.##") + "s";
				decayTotalTimeText.text = y.TotalTime.ToString("0.##") + "s";
				break;
			case ADSR_STATE.SUSTAIN:
				Line.Add(new ADSRPoint(y.State, y.ExternalValue, y.TotalTime, y.StateTime));
				sustainDurationText.text = y.StateTime.ToString("0.##") + "s";
				sustainTotalTimeText.text = y.TotalTime.ToString("0.##") + "s";
				break;
			case ADSR_STATE.RELEASE:
				Line.Add(new ADSRPoint(y.State, y.ExternalValue, y.TotalTime, y.StateTime));
				releaseDurationText.text = y.StateTime.ToString("0.##") + "s";
				releaseTotalTimeText.text = y.TotalTime.ToString("0.##") + "s";
				break;
		}
	}

	/// <summary>
	/// Updates the UILineRenderer's points and forces re-render.
	/// </summary>
	private void UpdateRenderer() {
		lineRenderer.Line.SetEqual(GetGraphPoints());
	}

	/// <summary>
	/// Converts the Line.Points values to a normalized structure for the UILineRenderer.
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
	/// Determines color transition points between different ADSR states.
	/// </summary>
	private void UpdateColorPoints() {
		// Prepare variables for iteration and add initial color point
		ADSR_STATE lastState = ADSR_STATE.NONE;
		lineRenderer.ColorPoints.Clear();
		lineRenderer.ColorPoints.Add(new UIColorPoint(attackColor, 0));

		// Iterate and create GradientKeys when state changes
		for (int i = 0; i < Line.Points.Count; i++) {
			if (Line.Points[i].State != lastState) {
				switch (Line.Points[i].State) {
					case ADSR_STATE.DECAY:
						lineRenderer.ColorPoints.Add(new UIColorPoint(attackColor, i-1));
						lineRenderer.ColorPoints.Add(new UIColorPoint(decayColor, i));
						break;
					case ADSR_STATE.SUSTAIN:
						lineRenderer.ColorPoints.Add(new UIColorPoint(decayColor, i - 1));
						lineRenderer.ColorPoints.Add(new UIColorPoint(sustainColor, i));
						break;
					case ADSR_STATE.RELEASE:
						lineRenderer.ColorPoints.Add(new UIColorPoint(sustainColor, i - 1));
						lineRenderer.ColorPoints.Add(new UIColorPoint(releaseColor, i));
						break;
					case ADSR_STATE.NONE:
						lineRenderer.ColorPoints.Add(new UIColorPoint(releaseColor, i));
						break;
				}
			}

			// Update last state
			lastState = Line.Points[i].State;
		}
	}

	/// <summary>
	/// Sets the correct duration and total time values in the UI for a static ADSR Graph.
	/// </summary>
	private void StaticTimeAnalysis() {
		// Set up last state variable to detect transitions and update statetime.
		foreach (var point in Line.Points) {
			switch (point.State) {
				case ADSR_STATE.ATTACK:
					attackDurationText.text = point.StateTime.ToString("0.##") + "s";
					attackTotalTimeText.text = point.TotalTime.ToString("0.##") + "s";
					break;
				case ADSR_STATE.DECAY:
					decayDurationText.text = point.StateTime.ToString("0.##") + "s";
					decayTotalTimeText.text = point.TotalTime.ToString("0.##") + "s";
					break;
				case ADSR_STATE.SUSTAIN:
					sustainDurationText.text = point.StateTime.ToString("0.##") + "s";
					sustainTotalTimeText.text = point.TotalTime.ToString("0.##") + "s";
					break;
				case ADSR_STATE.RELEASE:
					releaseDurationText.text = point.StateTime.ToString("0.##") + "s";
					releaseTotalTimeText.text = point.TotalTime.ToString("0.##") + "s";
					break;
			}
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
		return (float)(y - this.y.defaultValue) / MaxYHeight * gridRenderer.gridSize.y;
	}

	/***************************************/
	/*              COROUTINES             */
	/***************************************/
}