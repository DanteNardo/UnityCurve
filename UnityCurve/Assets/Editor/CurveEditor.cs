

///*******************************************/
///*                  INCLUDES               */
///*******************************************/
//using UnityEngine;
//using UnityEditor;
//using UnityEditorInternal;

///*******************************************/
///*                   CLASS                 */
///*******************************************/
//namespace UnityCurve {
//	/// <summary>
//	/// 
//	/// </summary>
//	[CustomEditor(typeof(Curve))]
//	[CanEditMultipleObjects]
//	public class CurveEditor : Editor {

//		/***************************************/
//		/*               MEMBERS               */
//		/***************************************/
//		private ReorderableList list;

//		/***************************************/
//		/*              PROPERTIES             */
//		/***************************************/


//		/***************************************/
//		/*               METHODS               */
//		/***************************************/
//		private void OnEnable() {
//			var property = serializedObject.FindProperty("curves");
//			list = ReorderableListUtility.CreateAutoLayout(property);
//		}

//		public override void OnInspectorGUI() {
//			base.OnInspectorGUI();
//			serializedObject.Update();
//			ReorderableListUtility.DoLayoutListWithFoldout(list);
//			serializedObject.ApplyModifiedProperties();
//		}

//		/***************************************/
//		/*              COROUTINES             */
//		/***************************************/
//	}
//}