

/***********************************************/
/*                   INCLUDES                  */
/***********************************************/
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/***********************************************/
/*                     CLASS                   */
/***********************************************/
namespace UnityCurve {
	public class UnityCurveGraphField : MonoBehaviour {

		/***************************************/
		/*               MEMBERS               */
		/***************************************/
		public Image fieldColor;
		public TMP_Text fieldDurationText;
		public TMP_Text fieldTotalTimeText;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/
		public Curve FieldCurve { get; set; }

		/***************************************/
		/*               METHODS               */
		/***************************************/
		private void Awake() {
			fieldColor = GetComponentInChildren<Image>();
		}

		public void SetColor(Color color) {
			fieldColor.color = color;
		}

		public void SetDuration(string text) {
			fieldDurationText.text = text;
		}

		public void SetTotalTime(string text) {
			fieldTotalTimeText.text = text;
		}

		/***************************************/
		/*              COROUTINES             */
		/***************************************/
	}
}