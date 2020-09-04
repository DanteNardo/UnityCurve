

/***********************************************/
/*                   INCLUDES                  */
/***********************************************/
using System.Collections.Generic;
using UnityEngine;

/***********************************************/
/*                     CLASS                   */
/***********************************************/
namespace UnityCurve {
	/// <summary>
	/// 
	/// </summary>
	public class UnityCurveGraphFields : MonoBehaviour {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/
		public UnityCurveGraph graph;
		public GameObject unityCurveFieldPrefab;

		private const int SPACING = 10;
		private const int HEIGHT = 20;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/
		private List<UnityCurveGraphField> Fields { get; set; }

		/***************************************/
		/*               METHODS               */
		/***************************************/
		private void Awake() {
			Fields = new List<UnityCurveGraphField>();
		}

		private void OnDestroy() {
			Clear();
		}

		private void Update() {
			UpdateGraphFields();
		}

		public void Clear() {
			foreach (var field in Fields) {
				Destroy(field.gameObject);
			}
			Fields.Clear();
		}

		private void UpdateGraphFields() {
			//// Not enough points for a field
			//if (graph.Line.Points.Count < 2) {
			//	return;
			//}
			
			//// EXPLAIN! EXPLAAAAAAIN!
			//int curveCount = 1;

			//// Add first curve field if it doesn't already exist
			//if (curveCount > Fields.Count) {
			//	InstantiateCurveField(graph.Line.Points[0].CurveAtPoint);
			//}

			//// Iterate and create more curve fields if necessary
			//for (int i = 1; i < graph.Line.Points.Count; i++) {
			//	if (CurveChangedInLine(i)) {
			//		curveCount++;
			//		if (NeedToInstantiate(curveCount)) {
			//			InstantiateCurveField(graph.Line.Points[i].CurveAtPoint);
			//		}
			//	}
			//}
		}

		public UnityCurveGraphField GetCurveField(Curve curve, int neededCurves) {
			if (NeedToInstantiate(neededCurves)) {
				return InstantiateCurveField(curve);
			}
			else return Fields[neededCurves - 1];
		}

		private UnityCurveGraphField InstantiateCurveField(Curve curve) {
			// Instantiate and get components
			var field = Instantiate(unityCurveFieldPrefab);
			var fieldComponent = field.GetComponent<UnityCurveGraphField>();
			var rectComponent = field.GetComponent<RectTransform>();

			// Update CurveField position, size, etc.
			field.transform.SetParent(transform);
			field.transform.localScale = new Vector3(1, 1, 1);
			field.transform.localPosition = new Vector3(0, 0, 0);
			RectTransformExtensions.SetLeft(rectComponent, 0);
			RectTransformExtensions.SetRight(rectComponent, 0);
			RectTransformExtensions.SetTop(rectComponent, SPACING + (HEIGHT * Fields.Count-1));
			rectComponent.sizeDelta = new Vector2(rectComponent.sizeDelta.x, HEIGHT);

			// Set up field curve and return
			fieldComponent.FieldCurve = curve;
			Fields.Add(fieldComponent);
			return fieldComponent;
		}

		private bool NeedToInstantiate(int neededCurves) {
			return neededCurves > Fields.Count;
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}