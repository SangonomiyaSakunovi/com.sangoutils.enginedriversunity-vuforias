using SangoUtils.Bases_Unity.RecognizableObjects;
using SangoUtils.Engines_Unity;
using System.Collections.Generic;

namespace SangoUtils.EngineDrivers_Unity.Vuforias.Recognizables
{
    public class RecognizableObjectRegister_Vuforia
    {
        public void Regist()
        {
            RegistMarkerRecognizableObject();
        }

        private void RegistMarkerRecognizableObject()
        {
            ICollection<MarkerRecognizableObjectPack> packs = RecognizableObjectSession.Instance.GetMarkerRecognizableObjectPacks();
            foreach(var pack in packs)
            {

            }
        }
    }
}
