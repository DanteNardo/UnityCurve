

/*******************************************/
/*                  INCLUDES               */
/*******************************************/
using System.Collections.Generic;
using UnityEngine;

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
	public MultiTargetCamera multiTargetCamera;
	public LineRenderer xAxisRenderer;
	public LineRenderer yAxisRenderer;
	public LineRenderer lineRenderer;

	public Color attackColor;
	public Color decayColor;
	public Color sustainColor;
	public Color releaseColor;

	public TextMesh attackDurationText;
	public TextMesh decayDurationText;
	public TextMesh sustainDurationText;
	public TextMesh releaseDurationText;
	public TextMesh attackTotalTimeText;
	public TextMesh decayTotalTimeText;
	public TextMesh sustainTotalTimeText;
	public TextMesh releaseTotalTimeText;

	/***************************************/
	/*              PROPERTIES             */
	/***************************************/
	private ADSRGraphLine Line { get; set; }
	private int AttackStart = 0;
	private int AttackEnd = -1;
	private int DecayStart = -1;
	private int DecayEnd = -1;
	private int SustainStart = -1;
	private int SustainEnd = -1;
	private int ReleaseStart = -1;
	private int ReleaseEnd = -1;
	private float MaxYHeight { get; set; }

	/***************************************/
	/*               METHODS               */
	/***************************************/
	private void Start() {
		// Instantiate new graph line variables
		Line = new ADSRGraphLine();
		Line.OnLineChange += UpdateRenderer;

		// Clear and set all data to empty
		Clear();

		// Calculate the maximum height for the graph
		MaxYHeight = (float)(y.attackTarget - y.defaultValue);

		// Update X and Y Axis Lines based on transform
		InitializeXAndYAxis();

		// If this is a static graph, set all data now
		if (realtime == false) {
			Line = y.Simulate(simulationSustainTime);
			UpdateLineColorIndices();
			UpdateRenderer();
		}
	}

	private void FixedUpdate() {
		if (realtime) {
			AddPoint();
		}
	}

	private void InitializeXAndYAxis() {
		// Prepare vector arrays (necessary for GetPositions call)
		Vector3[] xAxisPoints = new Vector3[xAxisRenderer.positionCount];
		Vector3[] yAxisPoints = new Vector3[yAxisRenderer.positionCount];

		// Store vector arrays (using ref)
		xAxisRenderer.GetPositions(xAxisPoints);
		yAxisRenderer.GetPositions(yAxisPoints);

		// Transform vector arrays
		TransformPoints(ref xAxisPoints);
		TransformPoints(ref yAxisPoints);

		// Set camera to track the axises
		foreach (var xPoint in xAxisPoints) {
			multiTargetCamera.AddTarget(xPoint);
		}
		foreach (var yPoint in yAxisPoints) {
			multiTargetCamera.AddTarget(yPoint);
		}

		// Set renderer positions
		xAxisRenderer.SetPositions(xAxisPoints);
		yAxisRenderer.SetPositions(yAxisPoints);
	}

	public void Clear() {
		// Clear line data
		Line.Clear();

		// Clear text data
		attackDurationText.text = "";
		decayDurationText.text = "";
		sustainDurationText.text = "";
		releaseDurationText.text = "";
		attackTotalTimeText.text = "";
		decayTotalTimeText.text = "";
		sustainTotalTimeText.text = "";
		releaseTotalTimeText.text = "";

		// Clear color indices
		AttackStart = 0;
		AttackEnd = -1;
		DecayStart = -1;
		DecayEnd = -1;
		SustainStart = -1;
		SustainEnd = -1;
		ReleaseStart = -1;
		ReleaseEnd = -1;
}

	public void AddPoint() {
		switch (y.State) {
			case ADSR_STATE.ATTACK:
				Line.Add(new ADSRGraphPoint(y.State, y.Value, y.TotalTime));
				attackDurationText.text = y.StateTime.ToString("0.##") + "s";
				attackTotalTimeText.text = y.TotalTime.ToString("0.##") + "s";
				break;
			case ADSR_STATE.DECAY:
				Line.Add(new ADSRGraphPoint(y.State, y.Value, y.TotalTime));
				decayDurationText.text = y.StateTime.ToString("0.##") + "s";
				decayTotalTimeText.text = y.TotalTime.ToString("0.##") + "s";
				break;
			case ADSR_STATE.SUSTAIN:
				Line.Add(new ADSRGraphPoint(y.State, y.Value, y.TotalTime));
				sustainDurationText.text = y.StateTime.ToString("0.##") + "s";
				sustainTotalTimeText.text = y.TotalTime.ToString("0.##") + "s";
				break;
			case ADSR_STATE.RELEASE:
				Line.Add(new ADSRGraphPoint(y.State, y.Value, y.TotalTime));
				releaseDurationText.text = y.StateTime.ToString("0.##") + "s";
				releaseTotalTimeText.text = y.TotalTime.ToString("0.##") + "s";
				break;
		}

		// A new point means we might need to update the color indices
		UpdateLineColorIndices();
	}

	private void UpdateRenderer() {
		Vector3[] renderPoints = GetGraphPoints();
		lineRenderer.positionCount = renderPoints.Length;
		lineRenderer.SetPositions(renderPoints);
		lineRenderer.colorGradient = GetColorGradient();
	}

	private Vector3[] GetGraphPoints() {
		// Create graph points object
		Vector3[] graphPoints = new Vector3[Line.Points.Count];

		// Generate graph points based on time and value and then normalize them.
		// - Adjust each point's x value based on previous lines in the graph
		// - Normalize x values with normalizer
		// - Normalize y values by subtracting default value and dividing by maxY
		for (int i = 0; i < graphPoints.Length; i++) {
			graphPoints[i] = new Vector3(
				NormalizeIndex(i),
				(float)(Line.Points[i].Value - y.defaultValue) / MaxYHeight,
				0
			);
		}

		// Apply parent transformations
		TransformPoints(ref graphPoints);

		// Return value
		return graphPoints;
	}

	private Gradient GetColorGradient() {
		// Initialize gradient
		Gradient gradient = new Gradient();

		// Count how many gradient keys need to exist based on saved indices
		int keyCount = 0;
		keyCount = AttackStart  == -1 ? keyCount : keyCount + 1;
		keyCount = AttackEnd    == -1 ? keyCount : keyCount + 1;
		keyCount = DecayStart   == -1 ? keyCount : keyCount + 1;
		keyCount = DecayEnd     == -1 ? keyCount : keyCount + 1;
		keyCount = SustainStart == -1 ? keyCount : keyCount + 1;
		keyCount = SustainEnd   == -1 ? keyCount : keyCount + 1;
		keyCount = ReleaseStart == -1 ? keyCount : keyCount + 1;
		keyCount = ReleaseEnd   == -1 ? keyCount : keyCount + 1;

		// Create gradient key arrays
		GradientColorKey[] colorKeys = new GradientColorKey[keyCount];
		GradientAlphaKey[] alphaKeys = new GradientAlphaKey[keyCount];
		for (int i = 0; i < keyCount; i++) {
			switch (i) {
				case 0:
					colorKeys[i] = new GradientColorKey(attackColor, NormalizeIndex(AttackStart));
					alphaKeys[i] = new GradientAlphaKey(attackColor.a, NormalizeIndex(AttackStart));
					break;
				case 1:
					colorKeys[i] = new GradientColorKey(attackColor, NormalizeIndex(AttackEnd));
					alphaKeys[i] = new GradientAlphaKey(attackColor.a, NormalizeIndex(AttackEnd));
					break;
				case 2:
					colorKeys[i] = new GradientColorKey(decayColor, NormalizeIndex(DecayStart));
					alphaKeys[i] = new GradientAlphaKey(decayColor.a, NormalizeIndex(DecayStart));
					break;
				case 3:
					colorKeys[i] = new GradientColorKey(decayColor, NormalizeIndex(DecayEnd));
					alphaKeys[i] = new GradientAlphaKey(decayColor.a, NormalizeIndex(DecayEnd));
					break;
				case 4:
					colorKeys[i] = new GradientColorKey(sustainColor, NormalizeIndex(SustainStart));
					alphaKeys[i] = new GradientAlphaKey(sustainColor.a, NormalizeIndex(SustainStart));
					break;
				case 5:
					colorKeys[i] = new GradientColorKey(sustainColor, NormalizeIndex(SustainEnd));
					alphaKeys[i] = new GradientAlphaKey(sustainColor.a, NormalizeIndex(SustainEnd));
					break;
				case 6:
					colorKeys[i] = new GradientColorKey(releaseColor, NormalizeIndex(ReleaseStart));
					alphaKeys[i] = new GradientAlphaKey(releaseColor.a, NormalizeIndex(ReleaseStart));
					break;
				case 7:
					colorKeys[i] = new GradientColorKey(releaseColor, NormalizeIndex(ReleaseEnd));
					alphaKeys[i] = new GradientAlphaKey(releaseColor.a, NormalizeIndex(ReleaseEnd));
					break;
			}
		}

		// Set gradient data and return
		gradient.SetKeys(colorKeys, alphaKeys);
		gradient.mode = GradientMode.Fixed;

		// FOR DEBUGGING: TODO REMOVE
		// FOR DEBUGGING: TODO REMOVE
		// FOR DEBUGGING: TODO REMOVE
		Debug.Log("AttackStart:" + AttackStart);
		Debug.Log("AttackEnd:" + AttackEnd);
		Debug.Log("DecayStart:" + DecayStart);
		Debug.Log("DecayEnd:" + DecayEnd);
		Debug.Log("SustainStart:" + SustainStart);
		Debug.Log("SustainEnd:" + SustainEnd);
		Debug.Log("ReleaseStart:" + ReleaseStart);
		Debug.Log("ReleaseEnd:" + ReleaseEnd);
		for (int i = 0; i < gradient.colorKeys.Length; i++) {
			Debug.Log("Gradient[" + i + "] Color:{" + gradient.colorKeys[i].color + "," + gradient.colorKeys[i].time + "}");
		}
		// FOR DEBUGGING: TODO REMOVE
		// FOR DEBUGGING: TODO REMOVE
		// FOR DEBUGGING: TODO REMOVE

		return gradient;
	}

	private void UpdateLineColorIndices() {
		// Iterate and create GradientKeys when state changes
		ADSR_STATE lastState = ADSR_STATE.NONE;
		for (int i = 0; i < Line.Points.Count; i++) {
			if (Line.Points[i].State != lastState) {
				switch (Line.Points[i].State) {
					case ADSR_STATE.DECAY:
						AttackEnd = i - 1;
						DecayStart = i;
						break;
					case ADSR_STATE.SUSTAIN:
						DecayEnd = i - 1;
						SustainStart = i;
						break;
					case ADSR_STATE.RELEASE:
						SustainEnd = i - 1;
						ReleaseStart = i;
						break;
					case ADSR_STATE.NONE:
						ReleaseEnd = i;
						break;
				}
			}

			// Update last state
			lastState = Line.Points[i].State;
		}
	}

	private void TransformPoints(ref Vector3[] points) {
		for (int j = 0; j < points.Length; j++) {
			points[j] = new Vector3(
				(points[j].x * transform.localScale.x) + transform.position.x,
				(points[j].y * transform.localScale.y) + transform.position.y,
				(points[j].z * transform.localScale.z) + transform.position.z
			);
		}
	}

	private float NormalizeIndex(int index) {
		if (Line.Points.Count == 0) return index;
		return (1.0f / Line.Points.Count) * index;
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

	/***************************************/
	/*               METHODS               */
	/***************************************/
	public ADSRGraphPoint(ADSR_STATE state, double value, double timestamp) {
		State = state;
		Value = (float)value;
		TotalTime = (float)timestamp;
	}

	public ADSRGraphPoint(ADSR_STATE state, float value, float timestamp) {
		State = state;
		Value = value;
		TotalTime = timestamp;
	}
}