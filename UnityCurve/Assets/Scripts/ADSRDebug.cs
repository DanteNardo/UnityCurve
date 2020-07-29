

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
	public LineRenderer lineRenderer;
	public ADSR y;
	private int index;
	private double lastY;

	/***************************************/
	/*              PROPERTIES             */
	/***************************************/


	/***************************************/
	/*               METHODS               */
	/***************************************/
	private void FixedUpdate() {
		if (!Mathf.Approximately((float)y.Value, (float)y.defaultValue)) {
			lineRenderer.positionCount = index + 1;
			lineRenderer.SetPosition(index, new Vector3(index * 0.1f, (float)y.Value, 0));
			index++;
		}
	}

	private void OnEnable() {
		index = 0;
		lineRenderer.SetPositions(new Vector3[0]);
	}

	/***************************************/
	/*              COROUTINES             */
	/***************************************/
}
