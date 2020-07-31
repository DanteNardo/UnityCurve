

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
	public new MultiTargetCamera camera;
	public LineRenderer attackRenderer;
	public LineRenderer decayRenderer;
	public LineRenderer sustainRenderer;
	public LineRenderer releaseRenderer;
	public ADSR y;
	public float xScale = 1.0f;

	private List<Vector3> attackPoints;
	private List<Vector3> decayPoints;
	private List<Vector3> sustainPoints;
	private List<Vector3> releasePoints;
	private int rendererX;
	private int rendererIndex;

	/***************************************/
	/*              PROPERTIES             */
	/***************************************/


	/***************************************/
	/*               METHODS               */
	/***************************************/
	private void Start() {
		Clear();
	}

	private void FixedUpdate() {
		// Update points based on state as long as the ADSR variable is not at default
		if (!Mathf.Approximately((float)y.Value, (float)y.defaultValue)) {
			switch (y.State) {
				case ADSR_STATE.ATTACK:
					AddPoint(attackRenderer);
					break;
				case ADSR_STATE.DECAY:
					AddPoint(decayRenderer);
					break;
				case ADSR_STATE.SUSTAIN:
					AddPoint(sustainRenderer);
					break;
				case ADSR_STATE.RELEASE:
					AddPoint(releaseRenderer);
					break;
			}
		}
	}

	public void Clear() {
		// Clear camera targets
		camera.ClearTargets();

		// Reset index and X axis variables
		rendererX = 0;
		rendererIndex = 0;

		// Clear renderers
		attackRenderer.positionCount = 0;
		attackRenderer.SetPositions(new Vector3[0]);

		decayRenderer.positionCount = 0;
		decayRenderer.SetPositions(new Vector3[0]);

		sustainRenderer.positionCount = 0;
		sustainRenderer.SetPositions(new Vector3[0]);

		releaseRenderer.positionCount = 0;
		releaseRenderer.SetPositions(new Vector3[0]);
	}

	private void AddPoint(LineRenderer renderer) {
		// Resets index if we switch to a new renderer
		ResetIndexIfNecessary(renderer);

		// Add point to renderer
		// NOTE: Y is normalized because attackTarget is the peak of every graph.
		renderer.positionCount = rendererIndex + 1;
		renderer.SetPosition(rendererIndex, new Vector3(rendererX * xScale, (float)(y.Value / y.attackTarget), 0));

		// Add point to camera targets
		//camera.AddTarget(renderer.GetPosition(rendererIndex));

		// Increment variables
		rendererX++;
		rendererIndex++;

		// Renormalize graph
		NormalizeGraph();
	}

	/// <summary>
	/// Resets rendererIndex when we switch to a new renderer with no vertices.
	/// </summary>
	/// <param name="renderer">The potential renderer we switched to</param>
	private void ResetIndexIfNecessary(LineRenderer renderer) {
		if (renderer.positionCount == 0) {
			rendererIndex = 0;
		}
	}

	private void NormalizeGraph() {
		// Create percentage values from 
		float totalPoints = 
			attackRenderer.positionCount +
			decayRenderer.positionCount +
			sustainRenderer.positionCount +
			releaseRenderer.positionCount;
		float normalizer = 1 / totalPoints;
		int i;

		for (i = 0; i < attackRenderer.positionCount; i++) {
			attackRenderer.SetPosition(i, new Vector3(i * normalizer, attackRenderer.GetPosition(i).y, 0));
		}
		for (int j = 0; j < decayRenderer.positionCount; i++, j++) {
			decayRenderer.SetPosition(j, new Vector3(i * normalizer, decayRenderer.GetPosition(j).y, 0));
		}
		for (int k = 0; k < sustainRenderer.positionCount; i++, k++) {
			sustainRenderer.SetPosition(k, new Vector3(i * normalizer, sustainRenderer.GetPosition(k).y, 0));
		}
		for (int l = 0; l < releaseRenderer.positionCount; i++, l++) {
			releaseRenderer.SetPosition(l, new Vector3(i * normalizer, releaseRenderer.GetPosition(l).y, 0));
		}
		
		// FOR DEBUGGING, TODO: REMOVE
		if (attackRenderer.positionCount > 1) {
			Debug.Log("A:0:" + attackRenderer.GetPosition(0));
			Debug.Log("A:!:" + attackRenderer.GetPosition(attackRenderer.positionCount - 1));
		}
		if (decayRenderer.positionCount > 1) {
			Debug.Log("D:0:" + decayRenderer.GetPosition(0));
			Debug.Log("D:!:" + decayRenderer.GetPosition(decayRenderer.positionCount - 1));
		}
		if (sustainRenderer.positionCount > 1) {
			Debug.Log("S:0:" + sustainRenderer.GetPosition(0));
			Debug.Log("S:!:" + sustainRenderer.GetPosition(sustainRenderer.positionCount - 1));
		}
		if (releaseRenderer.positionCount > 1) {
			Debug.Log("R:0:" + releaseRenderer.GetPosition(0));
			Debug.Log("R:!:" + releaseRenderer.GetPosition(releaseRenderer.positionCount - 1));
		}
	}

	/***************************************/
	/*              COROUTINES             */
	/***************************************/
}
