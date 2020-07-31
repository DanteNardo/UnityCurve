

/*******************************************/
/*                  INCLUDES               */
/*******************************************/
using System.Collections.Generic;
using UnityEngine;

/*******************************************/
/*                   CLASS                 */
/*******************************************/
public class MultiTargetCamera : MonoBehaviour {

	/***************************************/
	/*               MEMBERS               */
	/***************************************/
	public Vector3 offset;
	public float smoothTime = 0.5f;
	public float zoomSpeed = 10f;
	public float bufferPercent = 10f;

	private new Camera camera;
	private Bounds bounds;
	private Vector3 targetPosition;
	private Vector3 velocity;
	private float buffer;

	/***************************************/
	/*              PROPERTIES             */
	/***************************************/
	public List<Vector3> Targets { get; private set; }

	/***************************************/
	/*               METHODS               */
	/***************************************/
	private void Awake() {
		camera = GetComponent<Camera>();
		bounds = new Bounds();
		Targets = new List<Vector3>();
	}

	private void LateUpdate() {
		// Check if there are targets
		if (Targets.Count == 0) {
			return;
		}

		// Move the camera and update zoom to contain targets
		MoveCamera();
		ZoomCamera();
		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

		// Update buffer
		buffer = bufferPercent * CameraWidth();
	}

	public void AddTarget(Vector3 target) {
		Targets.Add(target);
		CalculateBounds();
	}

	public void ClearTargets() {
		Targets.Clear();
		CalculateBounds();
	}

	private void CalculateBounds() {
		if (Targets.Count >= 1) {
			bounds = new Bounds(Targets[0], Vector3.zero);
			foreach (var target in Targets) {
				bounds.Encapsulate(target);
			}
		}
	}

	private void MoveCamera() {
		Vector3 center = GetCentroid() + offset;
		targetPosition = new Vector3(center.x, center.y, transform.position.z);
	}

	private void ZoomCamera() {
		Vector3 bufferMin = bounds.min - new Vector3(buffer, buffer, 0);
		Vector3 bufferMax = bounds.max + new Vector3(buffer, buffer, 0);
		if (OutOfBounds(bufferMin) || OutOfBounds(bufferMax)) {
			targetPosition = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z - zoomSpeed);
		}
	}

	private bool OutOfBounds(Vector3 position) {
		Vector3 screenPoint = camera.WorldToViewportPoint(position);
		return
			screenPoint.z < 0 ||
			screenPoint.x < 0 ||
			screenPoint.x > 1 ||
			screenPoint.y < 0 ||
			screenPoint.y > 1;
	}

	private Vector3 GetCentroid() {
		// Check if there is only one target
		if (Targets.Count == 1) {
			return Targets[0];
		}

		return bounds.center;
	}

	private float CameraWidth() {
		Vector3 min = camera.ViewportToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
		Vector3 max = camera.ViewportToWorldPoint(new Vector3(1, 1, camera.nearClipPlane));
		return max.x - min.x;
	}

	/***************************************/
	/*              COROUTINES             */
	/***************************************/
}
