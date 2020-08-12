

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

	/***************************************/
	/*              PROPERTIES             */
	/***************************************/


	/***************************************/
	/*               METHODS               */
	/***************************************/
	private void Update() {
		UpdateGridSizeIfChanged();
	}

	protected override void OnPopulateMesh(VertexHelper vh) {
		vh.Clear();

		width = rectTransform.rect.width;
		height = rectTransform.rect.height;

		unitWidth = width / gridSize.x;
		unitHeight = height / gridSize.y;

		if (points.Count < 2) {
			return;
		}

		for (int i = 0; i < points.Count; i++) {
			Vector2 point = points[i];
			DrawVerticesForPoint(point, vh);
		}

		for (int j = 0; j < points.Count-1; j++) {
			int index = j * 2;
			vh.AddTriangle(index + 0, index + 1, index + 3);
			vh.AddTriangle(index + 3, index + 2, index + 0);
		}
	}

	private void DrawVerticesForPoint(Vector2 point, VertexHelper vh) {
		// Initialize vertex object
		UIVertex vertex = UIVertex.simpleVert;
		vertex.color = color;

		// 
		vertex.position = new Vector3(-thickness / 2, 0);
		vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
		vh.AddVert(vertex);

		// 
		vertex.position = new Vector3(thickness / 2, 0);
		vertex.position += new Vector3(unitWidth * point.x, unitHeight * point.y);
		vh.AddVert(vertex);
	}

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
