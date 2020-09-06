

/***********************************************/
/*                   INCLUDES                  */
/***********************************************/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/***********************************************/
/*                     CLASS                   */
/***********************************************/
namespace UnityCurve.UI {

	/// <summary>
	/// A script attached to a dynamically 
	/// created UI object that contains a color,
	/// duration text, and total time text that
	/// represents a single Curve along a full
	/// UnityCurve. This script updates all
	/// of the associated data of that UI object.
	/// </summary>
	public class UIGraphField : MonoBehaviour {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/

		/// <summary>
		/// The color marker that correlates this
		/// CurveField with the curve on the 
		/// actual UnityCurve graph.
		/// </summary>
		public Image fieldColor;

		/// <summary>
		/// The UI text element that contains 
		/// the Curve's live duration or final 
		/// duration if inactive.
		/// </summary>
		public TMP_Text fieldDurationText;

		/// <summary>
		/// The UI text element that contains 
		/// the last known total time value
		/// from the UnityCurve while this 
		/// specific curve was active. AKA the
		/// time at which the curve was last
		/// updated.
		/// </summary>
		public TMP_Text fieldTotalTimeText;

		/// <summary>
		/// The Curve that this GraphField lists
		/// data for including color, duration,
		/// and the time at which it was last
		/// updated by the UnityCurve.
		/// </summary>
		private Curve fieldCurve;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/

		/// <summary>
		/// A getter and setter for this field's
		/// curve that also updates the internal
		/// values when it can implicitly.
		/// </summary>
		public Curve FieldCurve {
			get { return fieldCurve; }
			set {
				fieldCurve = value;
				fieldColor.color = fieldCurve.curveColor;
			}
		}

		/***************************************/
		/*               METHODS               */
		/***************************************/

		/// <summary>
		/// An explicit setter for this curve's field UI display.
		/// </summary>
		/// <param name="text">The duration as a string.</param>
		public void SetDuration(string text) {
			fieldDurationText.text = text;
		}

		/// <summary>
		/// An explicit setter for this curve's field UI display.
		/// </summary>
		/// <param name="text">The total time as a string.</param>
		public void SetTotalTime(string text) {
			fieldTotalTimeText.text = text;
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}