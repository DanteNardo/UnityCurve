

/***********************************************/
/*                   INCLUDES                  */
/***********************************************/
using UnityEngine;
using UnityEngine.UI;

/***********************************************/
/*                     CLASS                   */
/***********************************************/
namespace UnityCurve.UI {

	/// <summary>
	/// Reference: https://www.youtube.com/watch?v=--LB7URk60A
	/// -----------------------------------------
	/// This grid renderer draws the cells of a
	/// grid on a Canvas based on the inspector
	/// grid size.
	/// </summary>
	public class UIGridRenderer : Graphic {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/

		/// <summary>
		/// The size of the grid. Determines the 
		/// number of horizontal and vertical 
		/// grid cells.
		/// </summary>
		public Vector2Int gridSize = new Vector2Int(1, 1);

		/// <summary>
		/// The thickness of the rendered lines
		/// created by DrawCell.
		/// </summary>
		public float thickness = 10.0f;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/

		/// <summary>
		/// A property used to optimize space. 
		/// Contains rectTransformWidth.
		/// </summary>
		private float Width { get; set; }

		/// <summary>
		/// A property used to optimize space. 
		/// Contains rectTransformHeight.
		/// </summary>
		private float Height { get; set; }

		/// <summary>
		/// A property used to optimize space. 
		/// Contains rectTransformWidth modified
		/// by gridSize.
		/// </summary>
		private float UnitWidth { get; set; }

		/// <summary>
		/// A property used to optimize space. 
		/// Contains rectTransformHeight modified
		/// by gridSize.
		/// </summary>
		private float UnitHeight { get; set; }

		/***************************************/
		/*               METHODS               */
		/***************************************/

		/// <summary>
		/// Populates this Graphic's mesh with cells.
		/// </summary>
		/// <param name="vh">A reference to the Unity VertexHelper object.</param>
		protected override void OnPopulateMesh(VertexHelper vh) {
			// Initialize VertexHelper and variables
			vh.Clear();
			Width = rectTransform.rect.width;
			Height = rectTransform.rect.height;

			UnitWidth = Width / (float)gridSize.x;
			UnitHeight = Height / (float)gridSize.y;

			// Iterate and create grid cells
			int count = 0;
			for (int y = 0; y < gridSize.y; y++) {
				for (int x = 0; x < gridSize.x; x++) {
					DrawCell(x, y, count, vh);
					count++;
				}
			}
		}

		/// <summary>
		/// Creates the vertices and triangles necessary to render a single grid cell.
		/// </summary>
		/// <param name="x">The x position in the UI.</param>
		/// <param name="y">The y position in the UI.</param>
		/// <param name="cellCount">The number of current cells created (offsets this cell's position).</param>
		/// <param name="vh">A reference to the Unity VertexHelper object.</param>
		private void DrawCell(int x, int y, int cellCount, VertexHelper vh) {
			// Calculate X Position
			float xPos = UnitWidth * x;
			float yPos = UnitHeight * y;

			// Initialize vertex object
			UIVertex vertex = UIVertex.simpleVert;
			vertex.color = color;

			// Create outer rectangle
			vertex.position = new Vector3(xPos, yPos);
			vh.AddVert(vertex);
			vertex.position = new Vector3(xPos, yPos + UnitHeight);
			vh.AddVert(vertex);
			vertex.position = new Vector3(xPos + UnitWidth, yPos + UnitHeight);
			vh.AddVert(vertex);
			vertex.position = new Vector3(xPos + UnitWidth, yPos);
			vh.AddVert(vertex);

			// This is the distance from the outer rectangle to the inner rectangle
			float distance = Mathf.Sqrt((thickness * thickness) / 2f);

			// Create inner rectangle
			vertex.position = new Vector3(xPos + distance, yPos + distance);
			vh.AddVert(vertex);
			vertex.position = new Vector3(xPos + distance, yPos + (UnitHeight - distance));
			vh.AddVert(vertex);
			vertex.position = new Vector3(xPos + (UnitWidth - distance), yPos + (UnitHeight - distance));
			vh.AddVert(vertex);
			vertex.position = new Vector3(xPos + (UnitWidth - distance), yPos + distance);
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