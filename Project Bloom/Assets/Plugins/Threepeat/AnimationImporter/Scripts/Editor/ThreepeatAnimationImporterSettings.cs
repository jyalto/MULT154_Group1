using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ThreepeatEditor
{
#if ENABLE_THREEPEAT_DEV_MODE
    [CreateAssetMenu(fileName = "ThreepeatAnimationImporterSettings", menuName = "Threepeat/Anim Importer Settings SO")]
#endif
    public class ThreepeatAnimationImporterSettings : ScriptableObject
    {

        public Avatar avatar = null;

        public bool removeTPoseAnimIfPresent = true;
        public bool bakeYForMinimalYMotionClips = true;
        public float bakeY_maxPositiveYChange = 0.3f;
        public float bakeY_maxNegativeYChange = -0.5f;
        public bool automaticallyAddMirrorsOfAllAnimations = false;
        public string mirrorPrefix = "";
        public string mirrorPostfix = "-mirror";
        public bool debugMode = false;
    }
}