

///*******************************************/
///*                  INCLUDES               */
///*******************************************/
//using UnityEngine;
//using UnityEditor;
//using UnityEditorInternal;

///*******************************************/
///*                   CLASS                 */
///*******************************************/
//[CustomEditor(typeof(CurveController))]
//[CanEditMultipleObjects]
//public class CurveEditor: Editor {

//	/***************************************/
//	/*               MEMBERS               */
//	/***************************************/
//	private ReorderableList list;

//	/***************************************/
//	/*              PROPERTIES             */
//	/***************************************/


//	/***************************************/
//	/*               METHODS               */
//	/***************************************/
//	private void OnEnable() {
//		//list = new ReorderableList(serializedObject,
//		//		serializedObject.FindProperty("curves"),
//		//		true, true, true, true);
//		var property = serializedObject.FindProperty("curves");
//		list = ReorderableListUtility.CreateAutoLayout(
//		   property
//		);
//	}

//	public override void OnInspectorGUI() {
//		base.OnInspectorGUI();
//		serializedObject.Update();
//		ReorderableListUtility.DoLayoutListWithFoldout(list);
//		serializedObject.ApplyModifiedProperties();
//	}

//	/***************************************/
//	/*              COROUTINES             */
//	/***************************************/
//}
