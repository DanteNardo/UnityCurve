

/*******************************************/
/*                  INCLUDES               */
/*******************************************/
using UnityEngine;

/*******************************************/
/*                   CLASS                 */
/*******************************************/
public class ADSRDebug : MonoBehaviour {

	/***************************************/
	/*               MEMBERS               */
	/***************************************/
	public LineRenderer attackRenderer;
	public LineRenderer decayRenderer;
	public LineRenderer sustainRenderer;
	public LineRenderer releaseRenderer;
	public ADSR y;

	private int rendererX;
	private int rendererIndex;

	/***************************************/
	/*              PROPERTIES             */
	/***************************************/


	/***************************************/
	/*               METHODS               */
	/***************************************/
	private void FixedUpdate() {
		if (!Mathf.Approximately((float)y.Value, (float)y.defaultValue)) {

			// Add new point to correct renderer
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

	private void OnEnable() {
		Clear();
	}

	public void Clear() {
		rendererX = 0;
		rendererIndex = 0;

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
		renderer.positionCount = rendererIndex + 1;
		renderer.SetPosition(rendererIndex, new Vector3(rendererX * 0.1f, (float)y.Value, 0));

		// Increment variables
		rendererX++;
		rendererIndex++;
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

	/***************************************/
	/*              COROUTINES             */
	/***************************************/
}
