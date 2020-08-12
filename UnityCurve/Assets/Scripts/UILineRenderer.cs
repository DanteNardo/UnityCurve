

/*******************************************/
/*                  INCLUDES               */
/*******************************************/
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*******************************************/
/*                   CLASS                 */
/*******************************************/
public class UILineRenderer : Graphic {

	/***************************************/
	/*               MEMBERS               */
	/***************************************/
	public UIGridRenderer referenceGrid;
	public List<Vector2> points;
	public float thickness = 10.0f;

	private Vector2Int gridSize;
	private float width;
	private float height;
	private float unitWidth;
	private float unitHeight;
	private float angle;

	/***************************************/
	/*              PROPERTIES             */
	/***************************************/


	/***************************************/
	/*               METHODS               */
	/***************************************/
	/// <summary>
	/// If our reference grid object's size changes, update this entire line renderer.
	/// </summary>
	private void Update() {
		UpdateGridSizeIfChanged();
	}

	/// <summary>
	/// Constructs a line in the UI given a set of points.
	/// </summary>
	/// <param name="vh">The vertexhelper utility for UI Graphic classes</param>
	protected override void OnPopulateMesh(VertexHelper vh) {
		vh.Clear();

		// Prepare important variables for line sizing
		width = rectTransform.rect.width;
		height = rectTransform.rect.height;
		unitWidth = width / gridSize.x;
		unitHeight = height / gridSize.y;
		angle = 0;

		// Cannot create a line unless we have at least two points
		if (points.Count < 2) {
			return;
		}

		// Create vertices for every point in the line
		for (int i = 0; i < points.Count; i++) {
			if (i < points.Count - 1) {
				angle = GetAngle(points[i], points[i + 1]) + 45f;
			}
			DrawVerticesForPoint(points[i], vh, angle);
		}

		// Create triangles from the line vertices
		for (int j = 0; j < points.Count-1; j++) {
			int index = j * 2;
			vh.AddTriangle(index + 0, index + 1, index + 3);
			vh.AddTriangle(index + 3, index + 2, index + 0);
		}
	}

	/// <summary>
	/// Adds two vertices that represent the given point to the vertex helper.
	/// </summary>
	/// <param name="point">The point we want to add to the line.</param>
	/// <param name="vh">The vertex helper utility that renders the line.</param>
	/// <param name="angle">The angle between this point and the next point. Prevents flat sections of the line.</param>
	private void DrawVerticesForPoint(Vector2 point, VertexHelper vh, float angle) {
		// Initialize vertex object
		UIVertex vertex = UIVertex.simpleVert;
		vertex.color = color;

		// Create two vertices so that our line has the desired thickness
		// NOTE: A rotation is applied via Quaternion in order to prevent flat sections of the line from appearing
		vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
		vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
		vh.AddVert(vertex);
		vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
		vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
		vh.AddVert(vertex);
	}

	/// <summary>
	/// Gets the angle between two points on the line.
	/// </summary>
	/// <param name="p">The first point</param>
	/// <param name="q">The second point</param>
	/// <returns>The angle in radians between two points.</returns>
	private float GetAngle(Vector2 p, Vector2 q) {
		return Mathf.Atan2(q.y - p.y, q.x - p.x) * Mathf.Rad2Deg;
	}

	/// <summary>
	/// Sets this line renderer's grid size to the reference grid's size.
	/// </summary>
	private void UpdateGridSizeIfChanged() {
		if (referenceGrid != null) {
			if (gridSize != referenceGrid.gridSize) {
				gridSize = referenceGrid.gridSize;
				SetVerticesDirty();
			}
		}
	}

	/***************************************/
	/*              COROUTINES             */
	/***************************************/
}
