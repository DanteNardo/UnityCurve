

/***********************************************/
/*                  INCLUDES                   */
/***********************************************/
using UnityEngine;
using UnityEngine.UI;

/***********************************************/
/*                   CLASS                     */
/***********************************************/
namespace UnityCurve.UI {
	/// <summary>
	/// Reference: https://www.youtube.com/watch?v=--LB7URk60A
	/// </summary>
	public class UIGridRenderer : Graphic {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/
		public Vector2Int gridSize = new Vector2Int(1, 1);
		public float thickness = 10.0f;

		private float width;
		private float height;
		private float cellWidth;
		private float cellHeight;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/


		/***************************************/
		/*               METHODS               */
		/***************************************/
		protected override void OnPopulateMesh(VertexHelper vh) {
			// Initialize VertexHelper and variables
			vh.Clear();
			width = rectTransform.rect.width;
			height = rectTransform.rect.height;

			cellWidth = width / (float)gridSize.x;
			cellHeight = height / (float)gridSize.y;

			// Iterate and create grid cells
			int count = 0;
			for (int y = 0; y < gridSize.y; y++) {
				for (int x = 0; x < gridSize.x; x++) {
					DrawCell(x, y, count, vh);
					count++;
				}
			}
		}

		private void DrawCell(int x, int y, int cellCount, VertexHelper vh) {
			// Calculate X Position
			float xPos = cellWidth * x;
			float yPos = cellHeight * y;

			// Initialize vertex object
			UIVertex vertex = UIVertex.simpleVert;
			vertex.color = color;

			// Create outer rectangle
			vertex.position = new Vector3(xPos, yPos);
			vh.AddVert(vertex);
			vertex.position = new Vector3(xPos, yPos + cellHeight);
			vh.AddVert(vertex);
			vertex.position = new Vector3(xPos + cellWidth, yPos + cellHeight);
			vh.AddVert(vertex);
			vertex.position = new Vector3(xPos + cellWidth, yPos);
			vh.AddVert(vertex);

			// This is the distance from the outer rectangle to the inner rectangle
			float distance = Mathf.Sqrt((thickness * thickness) / 2f);

			// Create inner rectangle
			vertex.position = new Vector3(xPos + distance, yPos + distance);
			vh.AddVert(vertex);
			vertex.position = new Vector3(xPos + distance, yPos + (cellHeight - distance));
			vh.AddVert(vertex);
			vertex.position = new Vector3(xPos + (cellWidth - distance), yPos + (cellHeight - distance));
			vh.AddVert(vertex);
			vertex.position = new Vector3(xPos + (cellWidth - distance), yPos + distance);
			vh.AddVert(vertex);

			// Vertex Offset
			// This makes sure we don't overwrite any previously added triangles
			int offset = cellCount * 8;

			// Create triangles for outer and inner cell rectangles
			vh.AddTriangle(0 + offset, 1 + offset, 5 + offset);
			vh.AddTriangle(5 + offset, 4 + offset, 0 + offset);

			vh.AddTriangle(1 + offset, 2 + offset, 6 + offset);
			vh.AddTriangle(6 + offset, 5 + offset, 1 + offset);

			vh.AddTriangle(2 + offset, 3 + offset, 7 + offset);
			vh.AddTriangle(7 + offset, 6 + offset, 2 + offset);

			vh.AddTriangle(3 + offset, 0 + offset, 4 + offset);
			vh.AddTriangle(4 + offset, 7 + offset, 3 + offset);
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}