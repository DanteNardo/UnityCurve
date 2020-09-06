

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
		private Curve fieldCurve;

		/***************************************/
		/*              PROPERTIES             */
		/***************************************/
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
		private void Awake() {
			fieldColor = GetComponentInChildren<Image>();
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