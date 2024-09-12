using SangoUtils.Engines_Unity.Utilities;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SangoUtils.EngineDrivers_Unity.Vuforias.Recognizables
{
    [CustomEditor(typeof(MarkerRecognizableObjectMessage_Vuforia)), CanEditMultipleObjects]
    public class MarkerRecognizableObjectMessage_VuforiaInspector : Editor
    {
        const string k_OverloadWarning = "Some functions were overloaded in MonoBehaviour components and may not work as intended if used with Animation Events!";
        const string k_NoFunction = "No function";
        const string k_FunctionLabel = "Function: ";
        const string k_MethodIsNotValid = "Method is not valid";

        SerializedProperty _Method;
        SerializedProperty _MethodParameterType;
        SerializedProperty _IntArg;
        SerializedProperty _FloatArg;
        SerializedProperty _StringArg;
        SerializedProperty _ObjectArg;

        private void OnEnable()
        {
            _Method = serializedObject.FindProperty("method");
            _MethodParameterType = serializedObject.FindProperty("methodParameterType");
            _IntArg = serializedObject.FindProperty("Int");
            _FloatArg = serializedObject.FindProperty("Float");
            _StringArg = serializedObject.FindProperty("String");
            _ObjectArg = serializedObject.FindProperty("Object");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var message = target as Component;
            var obj = message.gameObject;

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                DrawMethodAndParameters(obj);
                if (changeScope.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void DrawMethodAndParameters(GameObject obj)
        {
            var collectedMethods = MethodsUtils_Unity.CollectMethods_BasicParam(obj).ToList();
            var dropdown = collectedMethods.Select(m => m.ToString()).ToList();
            dropdown.Add(k_NoFunction);

            var selectedMethodID = collectedMethods.FindIndex(m => m.name == _Method.stringValue);
            if (selectedMethodID == -1)
            {
                selectedMethodID = collectedMethods.Count;
            }

            var previousMixedValue = EditorGUI.showMixedValue;
            if (_Method.hasMultipleDifferentValues)
            {
                EditorGUI.showMixedValue = true;
            }
            selectedMethodID = EditorGUILayout.Popup(k_FunctionLabel, selectedMethodID, dropdown.ToArray());
            EditorGUI.showMixedValue = previousMixedValue;

            if (selectedMethodID < collectedMethods.Count)
            {
                var method = collectedMethods.ElementAt(selectedMethodID);
                _Method.stringValue = method.name;
                DrawParameters(method);
                if (collectedMethods.Any(m => m.isOverload == true))
                {
                    EditorGUILayout.HelpBox(k_OverloadWarning, MessageType.Warning, true);
                }
            }
            else
            {
                EditorGUILayout.HelpBox(k_MethodIsNotValid, MessageType.Warning, true);
            }
        }

        private void DrawParameters(MethodDesc_BasicParam_1 method)
        {
            _MethodParameterType.enumValueIndex = (int)method.type;
            switch (method.type)
            {
                case MethodParameterType.Int:
                    EditorGUILayout.PropertyField(_IntArg);
                    break;
                case MethodParameterType.Float:
                    EditorGUILayout.PropertyField(_FloatArg);
                    break;
                case MethodParameterType.String:
                    EditorGUILayout.PropertyField(_StringArg);
                    break;
                case MethodParameterType.Object:
                    EditorGUILayout.PropertyField(_ObjectArg);
                    break;
                case MethodParameterType.None:
                default: break;
            }
        }
    }
}
