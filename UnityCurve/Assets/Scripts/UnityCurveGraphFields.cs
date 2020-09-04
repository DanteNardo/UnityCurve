

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
			foreach (var field in Fields) {
				Destroy(field.gameObject);
			}
		}

		private void Update() {
			UpdateGraphFields();
		}

		private void UpdateGraphFields() {
			// Not enough points for a field
			if (graph.Line.Points.Count < 2) {
				return;
			}
			
			// EXPLAIN! EXPLAAAAAAIN!
			int curveCount = 0;

			// Add first curve field if it doesn't already exist
			if (curveCount == Fields.Count) {
				InstantiateCurveField(graph.Line.Points[0].CurveAtPoint);
				curveCount++;
			}

			// Iterate and create more curve fields if necessary
			for (int i = 1; i < graph.Line.Points.Count; i++) {
				if (CurveChangedInLine(i)) {
					if (NeedToInstantiate(curveCount)) {
						InstantiateCurveField(graph.Line.Points[i].CurveAtPoint);
						curveCount++;
					}
				}
			}
		}

		private void InstantiateCurveField(Curve curve) {
			var field = Instantiate(unityCurveFieldPrefab);
			var fieldComponent = field.GetComponent<UnityCurveGraphField>();
			field.transform.SetParent(transform);
			fieldComponent.FieldCurve = curve;
			Fields.Add(fieldComponent);
		}

		private bool CurveChangedInLine(int i) {
			return graph.Line.Points[i].CurveAtPoint != graph.Line.Points[i - 1].CurveAtPoint;
		}

		private bool NeedToInstantiate(int neededCurves) {
			return neededCurves > Fields.Count;
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}