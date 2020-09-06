

/***********************************************/
/*                   INCLUDES                  */
/***********************************************/
using System.Collections.Generic;
using UnityEngine;

/***********************************************/
/*                     CLASS                   */
/***********************************************/
namespace UnityCurve.UI {

	/// <summary>
	/// This script controls the instantiation
	/// and destruction of the UnityCurve fields.
	/// </summary>
	public class UIGraphFields : MonoBehaviour {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/

		/// <summary>
		/// A reference to the UIGraph
		/// that needs access to the UnityCurve
		/// graph fields.
		/// </summary>
		public UIGraph graph;

		/// <summary>
		/// A reference to the prefab that is
		/// used to instantiate new curve fields.
		/// </summary>
		public GameObject unityCurveFieldPrefab;

		/// <summary>
		/// Constant variable for UI spacing
		/// between curve fields.
		/// </summary>
		private const int SPACING = 10;

		/// <summary>
		/// Constant variable that represents
		/// the height of the UI curve fields.
		/// </summary>
		private const int HEIGHT = 20;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/

		/// <summary>
		/// A private list that contains all of
		/// the currently active UI curve fields.
		/// </summary>
		private List<UIGraphField> Fields { get; set; }

		/***************************************/
		/*               METHODS               */
		/***************************************/

		/// <summary>
		/// Instantiates the graph fields list.
		/// </summary>
		private void Awake() {
			Fields = new List<UIGraphField>();
		}

		/// <summary>
		/// Clears the dynamic graph fields.
		/// </summary>
		private void OnDestroy() {
			Clear();
		}

		/// <summary>
		/// Destroys the field gameobjects and clears the fields list.
		/// </summary>
		public void Clear() {
			foreach (var field in Fields) {
				Destroy(field.gameObject);
			}
			Fields.Clear();
		}

		/// <summary>
		/// Gets the curve field associated with the parameter curve.
		/// Instantiates a curve field if necessary.
		/// </summary>
		/// <param name="curve">The curve we want a field for.</param>
		/// <param name="neededCurves">The amount of curves we need.</param>
		/// <returns></returns>
		public UIGraphField GetCurveField(Curve curve, int neededCurves) {
			if (NeedToInstantiate(neededCurves)) {
				return InstantiateCurveField(curve);
			}
			else return Fields[neededCurves - 1];
		}

		/// <summary>
		/// Instantiates a curve field UI object and adds it to the list.
		/// </summary>
		/// <param name="curve">The curve attached to this curve field.</param>
		/// <returns>The instantated gameobject's UIGraphField component.</returns>
		private UIGraphField InstantiateCurveField(Curve curve) {
			// Instantiate and get components
			var field = Instantiate(unityCurveFieldPrefab);
			var fieldComponent = field.GetComponent<UIGraphField>();
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

		/// <summary>
		/// A nice wrapper function to check if this script needs to instantiate more graph fields.
		/// </summary>
		/// <param name="neededCurves">The amount of curves the graph needs this frame.</param>
		/// <returns>True if this script needs to instante more fields, else false.</returns>
		private bool NeedToInstantiate(int neededCurves) {
			return neededCurves > Fields.Count;
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}