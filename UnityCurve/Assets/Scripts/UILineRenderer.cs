

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

	/***************************************/
	/*              PROPERTIES             */
	/***************************************/
	public List<UIColorPoint> ColorPoints { get; private set; } = new List<UIColorPoint>();
	private Vector2Int GridSize { get; set; }
	private float Width { get; set; }
	private float Height { get; set; }
	private float UnitWidth { get; set; }
	private float UnitHeight { get; set; }
	private float Angle { get; set; }

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
		Width = rectTransform.rect.width;
		Height = rectTransform.rect.height;
		UnitWidth = Width / GridSize.x;
		UnitHeight = Height / GridSize.y;
		Angle = 0;

		// Cannot create a line unless we have at least two points
		if (points.Count < 2) {
			return;
		}

		// Create vertices for every point in the line
		for (int i = 0; i < points.Count; i++) {
			if (i < points.Count - 1) {
				Angle = GetAngle(points[i], points[i + 1]) + 45f;
			}
			DrawVerticesForPoint(points[i], i, vh, Angle);
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
	private void DrawVerticesForPoint(Vector2 point, int index, VertexHelper vh, float angle) {
		// Initialize vertex object
		UIVertex vertex = UIVertex.simpleVert;
		vertex.color = GetColorAtVertex(index);

		// Create two vertices so that our line has the desired thickness
		// NOTE: A rotation is applied via Quaternion in order to prevent flat sections of the line from appearing
		vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(-thickness / 2, 0);
		vertex.position += new Vector3(UnitWidth * point.x, UnitHeight * point.y);
		vh.AddVert(vertex);
		vertex.position = Quaternion.Euler(0, 0, angle) * new Vector3(thickness / 2, 0);
		vertex.position += new Vector3(UnitWidth * point.x, UnitHeight * point.y);
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
	/// Gets the color the vertex at the given index should be.
	/// Value depends on current ColorPoints array.
	/// </summary>
	/// <param name="index">The index to get a color for.</param>
	/// <returns>The color at this vertex on the line.</returns>
	private Color GetColorAtVertex(int index) {
		// If no points, then default white
		if (ColorPoints.Count == 0) {
			return Color.white;
		}

		// If one point, just return its value
		if (ColorPoints.Count == 1) {
			return ColorPoints[0].color;
		}

		// Iterate until we find an index > than the current index.
		// In that case, use the previous color.
		for (int i = 1; i < ColorPoints.Count; i++) {
			if (ColorPoints[i].index > index) {
				return ColorPoints[i - 1].color;
			}
		}

		// If we get to this point, use the last index
		return ColorPoints[ColorPoints.Count - 1].color;
	}

	/// <summary>
	/// Sets this line renderer's grid size to the reference grid's size.
	/// </summary>
	private void UpdateGridSizeIfChanged() {
		if (referenceGrid != null) {
			if (GridSize != referenceGrid.gridSize) {
				GridSize = referenceGrid.gridSize;
				SetVerticesDirty();
			}
		}
	}

	/***************************************/
	/*              COROUTINES             */
	/***************************************/
}


/*******************************************/
/*                   STRUCT                */
/*******************************************/
public struct UIColorPoint {

	/***************************************/
	/*              PROPERTIES             */
	/***************************************/
	public Color color;
	public int index;

	/***************************************/
	/*               METHODS               */
	/***************************************/
	public UIColorPoint(Color color, int index) {
		this.color = color;
		this.index = index;
	}
}