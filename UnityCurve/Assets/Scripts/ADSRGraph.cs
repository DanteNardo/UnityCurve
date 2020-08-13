

/*******************************************/
/*                  INCLUDES               */
/*******************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*******************************************/
/*                   CLASS                 */
/*******************************************/
public class ADSRGraph : MonoBehaviour {

	/***************************************/
	/*               MEMBERS               */
	/***************************************/
	public bool realtime = true;
	public float simulationSustainTime = 0.5f;
	public ADSR y;
	public UIGridRenderer gridRenderer;
	public UILineRenderer lineRenderer;

	public Color attackColor;
	public Color decayColor;
	public Color sustainColor;
	public Color releaseColor;

	public TMP_Text attackDurationText;
	public TMP_Text decayDurationText;
	public TMP_Text sustainDurationText;
	public TMP_Text releaseDurationText;
	public TMP_Text attackTotalTimeText;
	public TMP_Text decayTotalTimeText;
	public TMP_Text sustainTotalTimeText;
	public TMP_Text releaseTotalTimeText;

	/***************************************/
	/*              PROPERTIES             */
	/***************************************/
	private ADSRGraphLine Line { get; set; }
	private float MaxYHeight { get; set; }

	/***************************************/
	/*               METHODS               */
	/***************************************/
	private void Start() {
		// Instantiate new graph line variables
		Line = new ADSRGraphLine();
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

	private void FixedUpdate() {
		if (realtime) {
			AddPoint();
		}
	}

	public void Clear() {
		// Clear line data
		Line.Clear();
		lineRenderer.points.Clear();
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

	public void AddPoint() {
		switch (y.State) {
			case ADSR_STATE.ATTACK:
				Line.Add(new ADSRGraphPoint(y.State, y.Value, y.TotalTime, y.StateTime));
				attackDurationText.text = y.StateTime.ToString("0.##") + "s";
				attackTotalTimeText.text = y.TotalTime.ToString("0.##") + "s";
				break;
			case ADSR_STATE.DECAY:
				Line.Add(new ADSRGraphPoint(y.State, y.Value, y.TotalTime, y.StateTime));
				decayDurationText.text = y.StateTime.ToString("0.##") + "s";
				decayTotalTimeText.text = y.TotalTime.ToString("0.##") + "s";
				break;
			case ADSR_STATE.SUSTAIN:
				Line.Add(new ADSRGraphPoint(y.State, y.Value, y.TotalTime, y.StateTime));
				sustainDurationText.text = y.StateTime.ToString("0.##") + "s";
				sustainTotalTimeText.text = y.TotalTime.ToString("0.##") + "s";
				break;
			case ADSR_STATE.RELEASE:
				releaseDurationText.text = y.StateTime.ToString("0.##") + "s";
				releaseTotalTimeText.text = y.TotalTime.ToString("0.##") + "s";
				break;
		}
	}
	
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

	private void UpdateRenderer() {
		lineRenderer.points = GetGraphPoints();
	}

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

	private float NormalizeX(int x) {
		if (Line.Points.Count == 0) return x * gridRenderer.gridSize.x;
		return (1.0f / Line.Points.Count) * x * gridRenderer.gridSize.x;
	}

	private float NormalizeY(float y) {
		return (float)(y - this.y.defaultValue) / MaxYHeight * gridRenderer.gridSize.y;
	}

	/***************************************/
	/*              COROUTINES             */
	/***************************************/
}


/*******************************************/
/*                    CLASS                */
/*******************************************/
public class ADSRGraphLine {

	/***************************************/
	/*               MEMBERS               */
	/***************************************/
	public delegate void OnLineChangeDelegate();
	public event OnLineChangeDelegate OnLineChange;

	/***************************************/
	/*              PROPERTIES             */
	/***************************************/
	public List<ADSRGraphPoint> Points { get; private set; }

	/***************************************/
	/*               METHODS               */
	/***************************************/
	public ADSRGraphLine() {
		Points = new List<ADSRGraphPoint>();
	}

	public void Add(ADSRGraphPoint point) {
		Points.Add(point);
		OnLineChange?.Invoke();
	}

	public void Clear() {
		Points.Clear();
		OnLineChange?.Invoke();
	}
}


/*******************************************/
/*                   STRUCT                */
/*******************************************/
public struct ADSRGraphPoint {

	/***************************************/
	/*              PROPERTIES             */
	/***************************************/
	public ADSR_STATE State { get; private set; }
	public float Value { get; private set; }
	public float TotalTime { get; private set; }
	public float StateTime { get; private set; }

	/***************************************/
	/*               METHODS               */
	/***************************************/
	public ADSRGraphPoint(ADSR_STATE state, double value, double totalTime, double stateTime) {
		State = state;
		Value = (float)value;
		TotalTime = (float)totalTime;
		StateTime = (float)stateTime;
	}

	public ADSRGraphPoint(ADSR_STATE state, float value, float totalTime, float stateTime) {
		State = state;
		Value = value;
		TotalTime = totalTime;
		StateTime = (float)stateTime;
	}
}