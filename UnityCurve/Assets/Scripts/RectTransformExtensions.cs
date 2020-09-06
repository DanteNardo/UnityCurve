

/*******************************************/
/*                  INCLUDES               */
/*******************************************/
using UnityEngine;

/*******************************************/
/*                   CLASS                 */
/*******************************************/
namespace UnityCurve {

	/// <summary>
	/// A utility class that makes it easier to
	/// set internal values for a RectTransform
	/// like you're modifying it the inspector.
	/// </summary>
	public static class RectTransformExtensions {

		/***************************************/
		/*               METHODS               */
		/***************************************/

		/// <summary>
		/// Sets the "left" value of a UI element since the RectTransform variables are confusing.
		/// </summary>
		/// <param name="rt">The rect transform to modify.</param>
		/// <param name="left">The inspector "left" value we want.</param>
		public static void SetLeft(this RectTransform rt, float left) {
			rt.offsetMin = new Vector2(left, rt.offsetMin.y);
		}

		/// <summary>
		/// Sets the "right" value of a UI element since the RectTransform variables are confusing.
		/// </summary>
		/// <param name="rt">The rect transform to modify.</param>
		/// <param name="right">The inspector "right" value we want.</param>
		public static void SetRight(this RectTransform rt, float right) {
			rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
		}

		/// <summary>
		/// Sets the "top" value of a UI element since the RectTransform variables are confusing.
		/// </summary>
		/// <param name="rt">The rect transform to modify.</param>
		/// <param name="top">The inspector "top" value we want.</param>
		public static void SetTop(this RectTransform rt, float top) {
			rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
		}

		/// <summary>
		/// Sets the "bottom" value of a UI element since the RectTransform variables are confusing.
		/// </summary>
		/// <param name="rt">The rect transform to modify.</param>
		/// <param name="bottom">The inspector "bottom" value we want.</param>
		public static void SetBottom(this RectTransform rt, float bottom) {
			rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
		}
	}
}