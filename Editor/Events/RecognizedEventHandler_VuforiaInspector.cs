using SangoUtils.Engines_Unity.Utilities;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SangoUtils.EngineDrivers_Unity.Vuforias.Recognizables
{
    [CustomEditor(typeof(RecognizedEventHandler_Vuforia)), CanEditMultipleObjects]
    public class RecognizedEventHandler_VuforiaInspector : Editor
    {
        const string k_OverloadWarning = "Some functions were overloaded in MonoBehaviour components and may not work as intended if used with Animation Events!";
        const string k_NoFunction = "No function";
        const string k_FunctionLabel0 = "On Recognized: ";
        const string k_FunctionLabel1 = "On Lost: ";
        const string k_MethodIsNotValid = "Method is not valid";

        SerializedProperty m_Method0;
        SerializedProperty m_MethodParameterType0;
        SerializedProperty m_IntArg0;
        SerializedProperty m_FloatArg0;
        SerializedProperty m_StringArg0;
        SerializedProperty m_ObjectArg0;

        SerializedProperty m_Method1;
        SerializedProperty m_MethodParameterType1;
        SerializedProperty m_IntArg1;
        SerializedProperty m_FloatArg1;
        SerializedProperty m_StringArg1;
        SerializedProperty m_ObjectArg1;

        private void OnEnable()
        {
            m_Method0 = serializedObject.FindProperty("method0");
            m_MethodParameterType0 = serializedObject.FindProperty("methodParameterType0");
            m_IntArg0 = serializedObject.FindProperty("Int0");
            m_FloatArg0 = serializedObject.FindProperty("Float0");
            m_StringArg0 = serializedObject.FindProperty("String0");
            m_ObjectArg0 = serializedObject.FindProperty("Object0");

            m_Method1 = serializedObject.FindProperty("method1");
            m_MethodParameterType1 = serializedObject.FindProperty("methodParameterType1");
            m_IntArg1 = serializedObject.FindProperty("Int1");
            m_FloatArg1 = serializedObject.FindProperty("Float1");
            m_StringArg1 = serializedObject.FindProperty("String1");
            m_ObjectArg1 = serializedObject.FindProperty("Object1");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var message = target as Component;
            var obj = message.gameObject;

            using (var changeScope = new EditorGUI.ChangeCheckScope())
            {
                DrawMethodAndParameters0(obj);
                DrawMethodAndParameters1(obj);
                if (changeScope.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void DrawMethodAndParameters0(GameObject obj)
        {
            var collectedMethods = MethodsUtils_Unity.CollectMethods_BasicParam(obj).ToList();
            var dropdown = collectedMethods.Select(m => m.ToString()).ToList();
            dropdown.Add(k_NoFunction);

            var selectedMethodID = collectedMethods.FindIndex(m => m.name == m_Method0.stringValue);
            if (selectedMethodID == -1)
            {
                selectedMethodID = collectedMethods.Count;
            }

            var previousMixedValue = EditorGUI.showMixedValue;
            if (m_Method0.hasMultipleDifferentValues)
            {
                EditorGUI.showMixedValue = true;
            }
            selectedMethodID = EditorGUILayout.Popup(k_FunctionLabel0, selectedMethodID, dropdown.ToArray());
            EditorGUI.showMixedValue = previousMixedValue;

            if(selectedMethodID < collectedMethods.Count)
            {
                var method = collectedMethods.ElementAt(selectedMethodID);
                m_Method0.stringValue = method.name;
                DrawParameters0(method);
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

        private void DrawParameters0(MethodDesc_BasicParam_1 method)
        {
            m_MethodParameterType0.enumValueIndex = (int)method.type;
            switch (method.type)
            {
                case MethodParameterType.Int:
                    EditorGUILayout.PropertyField(m_IntArg0);
                    break;
                case MethodParameterType.Float:
                    EditorGUILayout.PropertyField(m_FloatArg0);
                    break;
                case MethodParameterType.String:
                    EditorGUILayout.PropertyField(m_StringArg0);
                    break;
                case MethodParameterType.Object:
                    EditorGUILayout.PropertyField(m_ObjectArg0);
                    break;
                case MethodParameterType.None:
                default: break;
            }
        }

        private void DrawMethodAndParameters1(GameObject obj)
        {
            var collectedMethods = MethodsUtils_Unity.CollectMethods_BasicParam(obj).ToList();
            var dropdown = collectedMethods.Select(m => m.ToString()).ToList();
            dropdown.Add(k_NoFunction);

            var selectedMethodID = collectedMethods.FindIndex(m => m.name == m_Method1.stringValue);
            if (selectedMethodID == -1)
            {
                selectedMethodID = collectedMethods.Count;
            }

            var previousMixedValue = EditorGUI.showMixedValue;
            if (m_Method1.hasMultipleDifferentValues)
            {
                EditorGUI.showMixedValue = true;
            }
            selectedMethodID = EditorGUILayout.Popup(k_FunctionLabel1, selectedMethodID, dropdown.ToArray());
            EditorGUI.showMixedValue = previousMixedValue;

            if (selectedMethodID < collectedMethods.Count)
            {
                var method = collectedMethods.ElementAt(selectedMethodID);
                m_Method1.stringValue = method.name;
                DrawParameters1(method);
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

        private void DrawParameters1(MethodDesc_BasicParam_1 method)
        {
            m_MethodParameterType1.enumValueIndex = (int)method.type;
            switch (method.type)
            {
                case MethodParameterType.Int:
                    EditorGUILayout.PropertyField(m_IntArg1);
                    break;
                case MethodParameterType.Float:
                    EditorGUILayout.PropertyField(m_FloatArg1);
                    break;
                case MethodParameterType.String:
                    EditorGUILayout.PropertyField(m_StringArg1);
                    break;
                case MethodParameterType.Object:
                    EditorGUILayout.PropertyField(m_ObjectArg1);
                    break;
                case MethodParameterType.None:
                default: break;
            }
        }
    }
}

