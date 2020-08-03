﻿

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
	public MultiTargetCamera multiTargetCamera;
	public LineRenderer xAxisRenderer;
	public LineRenderer yAxisRenderer;
	public LineRenderer attackRenderer;
	public LineRenderer decayRenderer;
	public LineRenderer sustainRenderer;
	public LineRenderer releaseRenderer;
	public ADSR y;
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
	private ADSRGraphLine AttackLine { get; set; }
	private ADSRGraphLine DecayLine { get; set; }
	private ADSRGraphLine SustainLine { get; set; }
	private ADSRGraphLine ReleaseLine { get; set; }
	private float MaxYHeight { get; set; }

	/***************************************/
	/*               METHODS               */
	/***************************************/
	private void Start() {
		// Instantiate new graph line variables
		AttackLine = new ADSRGraphLine();
		DecayLine = new ADSRGraphLine();
		SustainLine = new ADSRGraphLine();
		ReleaseLine = new ADSRGraphLine();

		// Initialize line events
		AttackLine.OnLineChange += UpdateRenderers;
		DecayLine.OnLineChange += UpdateRenderers;
		SustainLine.OnLineChange += UpdateRenderers;
		ReleaseLine.OnLineChange += UpdateRenderers;

		// Clear and set all data to empty
		Clear();

		// Calculate the maximum height for the graph
		MaxYHeight = (float)(y.attackTarget - y.defaultValue);

		// Update X and Y Axis Lines based on transform
		InitializeXAndYAxis();
	}

	private void FixedUpdate() {
		AddPoint();
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
		AttackLine.Clear();
		DecayLine.Clear();
		SustainLine.Clear();
		ReleaseLine.Clear();

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
				AttackLine.Add(new ADSRGraphPoint(y.TotalTime, y.Value));
				attackDurationText.text = y.StateTime.ToString("0.##") + "s";
				attackTotalTimeText.text = y.TotalTime.ToString("0.##") + "s";
				break;
			case ADSR_STATE.DECAY:
				DecayLine.Add(new ADSRGraphPoint(y.TotalTime, y.Value));
				decayDurationText.text = y.StateTime.ToString("0.##") + "s";
				decayTotalTimeText.text = y.TotalTime.ToString("0.##") + "s";
				break;
			case ADSR_STATE.SUSTAIN:
				SustainLine.Add(new ADSRGraphPoint(y.TotalTime, y.Value));
				sustainDurationText.text = y.StateTime.ToString("0.##") + "s";
				sustainTotalTimeText.text = y.TotalTime.ToString("0.##") + "s";
				break;
			case ADSR_STATE.RELEASE:
				ReleaseLine.Add(new ADSRGraphPoint(y.TotalTime, y.Value));
				releaseDurationText.text = y.StateTime.ToString("0.##") + "s";
				releaseTotalTimeText.text = y.TotalTime.ToString("0.##") + "s";
				break;
		}
	}

	public void ConnectLines() {
		switch (y.State) {
			case ADSR_STATE.DECAY:
				AttackLine.Add(new ADSRGraphPoint(y.TotalTime, y.Value));
				break;
			case ADSR_STATE.SUSTAIN:
				DecayLine.Add(new ADSRGraphPoint(y.TotalTime, y.Value));
				break;
			case ADSR_STATE.RELEASE:
				SustainLine.Add(new ADSRGraphPoint(y.TotalTime, y.Value));
				break;
			case ADSR_STATE.NONE:
				ReleaseLine.Add(new ADSRGraphPoint(y.TotalTime, y.Value));
				break;
		}
	}

	private void UpdateRenderers() {
		// Keep track of starting x position
		float startX = 0;

		// Update attack line and increment positionCount
		UpdateRenderer(attackRenderer, AttackLine, 0);
		startX += attackRenderer.positionCount;

		// Update decay line and increment positionCount
		UpdateRenderer(decayRenderer, DecayLine, startX);
		startX += decayRenderer.positionCount;

		// Update sustain line and increment positionCount
		UpdateRenderer(sustainRenderer, SustainLine, startX);
		startX += sustainRenderer.positionCount;

		// Update release line
		UpdateRenderer(releaseRenderer, ReleaseLine, startX);
	}

	private void UpdateRenderer(LineRenderer lineRenderer, ADSRGraphLine line, float startX) {
		float totalPoints =
			attackRenderer.positionCount +
			decayRenderer.positionCount +
			sustainRenderer.positionCount +
			releaseRenderer.positionCount;
		Vector3[] renderPoints = GetGraphPoints(line, totalPoints, startX);
		lineRenderer.positionCount = renderPoints.Length;
		lineRenderer.SetPositions(renderPoints);
	}

	private Vector3[] GetGraphPoints(ADSRGraphLine line, float totalPoints, float startX) {
		// Create graph points object
		Vector3[] graphPoints = new Vector3[line.Points.Count];

		// Create normalization multiplier (default value is 1)
		float xNormalizer = totalPoints == 0 ? 1.0f : 1.0f / totalPoints;

		// Generate graph points based on time and value and then normalize them.
		// - Adjust each point's x value based on previous lines in the graph
		// - Normalize x values with normalizer
		// - Normalize y values by subtracting default value and dividing by maxY
		for (int i = 0; i < graphPoints.Length; i++) {
			graphPoints[i] = new Vector3(
				(startX + i) * xNormalizer,
				(float)(line.Points[i].Value - y.defaultValue) / MaxYHeight,
				0
			);
		}

		// Apply parent transformations
		TransformPoints(ref graphPoints);

		// Return value
		return graphPoints;
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
	public float TotalTime { get; private set; }
	public float Value { get; private set; }

	/***************************************/
	/*               METHODS               */
	/***************************************/
	public ADSRGraphPoint(double timestamp, double value) {
		TotalTime = (float)timestamp;
		Value = (float)value;
	}

	public ADSRGraphPoint(float timestamp, float value) {
		TotalTime = timestamp;
		Value = value;
	}
}