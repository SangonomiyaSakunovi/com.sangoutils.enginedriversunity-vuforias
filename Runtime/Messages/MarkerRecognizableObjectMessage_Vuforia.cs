using SangoUtils.Engines_Unity.Utilities;
using UnityEngine;

namespace SangoUtils.EngineDrivers_Unity.Vuforias.Recognizables
{
    public class MarkerRecognizableObjectMessage_Vuforia : MonoBehaviour
    {
        public string method;

        public MethodParameterType methodParameterType;
        public int Int;
        public string String;
        public float Float;
        public UnityEngine.Object Object;

        public PropertyName id { get; }
    }
}
