

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

	private List<Vector3> attackPoints;
	private List<Vector3> decayPoints;
	private List<Vector3> sustainPoints;
	private List<Vector3> releasePoints;
	private float maxYHeight;

	/***************************************/
	/*              PROPERTIES             */
	/***************************************/


	/***************************************/
	/*               METHODS               */
	/***************************************/
	private void Start() {
		Clear();
		maxYHeight = (float)(y.attackTarget - y.defaultValue);
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

		// Clear renderers and reset to default
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
		// Add point to renderer
		// NOTE: Y is normalized because attackTarget is the peak of every graph.
		float normalY = (float)((y.Value - y.defaultValue) / maxYHeight);
		renderer.positionCount += 1;
		renderer.SetPosition(renderer.positionCount - 1, new Vector3(0, normalY, 0));

		// Add point to camera targets
		//camera.AddTarget(renderer.GetPosition(rendererIndex));

		// Renormalize graph
		NormalizeGraph();
	}

	private void NormalizeGraph() {
		// Create percentage values from 
		float totalPoints = 
			attackRenderer.positionCount +
			decayRenderer.positionCount +
			sustainRenderer.positionCount +
			releaseRenderer.positionCount;
		float normalizer = 1 / totalPoints;
		float x;

		// Normalize all values in the graph
		for (int i = 0; i < attackRenderer.positionCount; i++) {
			x = i * normalizer;
			attackRenderer.SetPosition(i, new Vector3(x, attackRenderer.GetPosition(i).y, 0));
		}
		for (int j = 0; j < decayRenderer.positionCount; j++) {
			x = (attackRenderer.positionCount + j) * normalizer;
			decayRenderer.SetPosition(j, new Vector3(x, decayRenderer.GetPosition(j).y, 0));
		}
		for (int k = 0; k < sustainRenderer.positionCount; k++) {
			x = (attackRenderer.positionCount + decayRenderer.positionCount + k) * normalizer;
			sustainRenderer.SetPosition(k, new Vector3(x, sustainRenderer.GetPosition(k).y, 0));
		}
		for (int l = 0; l < releaseRenderer.positionCount; l++) {
			x = (attackRenderer.positionCount + decayRenderer.positionCount + sustainRenderer.positionCount + l) * normalizer;
			releaseRenderer.SetPosition(l, new Vector3(x, releaseRenderer.GetPosition(l).y, 0));
		}

		//for (i = 0; i < attackRenderer.positionCount; i++) {
		//	Vector3 position = new Vector3(
		//		((i * normalizer) + transform.position.x) * transform.localScale.x,
		//		(attackRenderer.GetPosition(i).y + transform.position.y) * transform.localScale.y,
		//		transform.position.z * transform.localScale.z
		//	);
		//	attackRenderer.SetPosition(i, position);
		//}
		//for (int j = 0; j < decayRenderer.positionCount; i++, j++) {
		//	Vector3 position = new Vector3(
		//		((i * normalizer) + transform.position.x) * transform.localScale.x,
		//		(decayRenderer.GetPosition(j).y + transform.position.y) * transform.localScale.y,
		//		transform.position.z * transform.localScale.z
		//	);
		//	decayRenderer.SetPosition(j, position);
		//}
		//for (int k = 0; k < sustainRenderer.positionCount; i++, k++) {
		//	Vector3 position = new Vector3(
		//		((i * normalizer) + transform.position.x) * transform.localScale.x,
		//		(sustainRenderer.GetPosition(k).y + transform.position.y) * transform.localScale.y,
		//		transform.position.z * transform.localScale.z
		//	);
		//	sustainRenderer.SetPosition(k, position);
		//}
		//for (int l = 0; l < releaseRenderer.positionCount; i++, l++) {
		//	Vector3 position = new Vector3(
		//		((i * normalizer) + transform.position.x) * transform.localScale.x,
		//		(releaseRenderer.GetPosition(l).y + transform.position.y) * transform.localScale.y,
		//		transform.position.z * transform.localScale.z
		//	);
		//	releaseRenderer.SetPosition(l, position);
		//}

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
