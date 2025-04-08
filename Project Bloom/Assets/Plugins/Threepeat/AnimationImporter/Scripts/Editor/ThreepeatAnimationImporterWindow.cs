using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ThreepeatEditor
{
    public class ThreepeatAnimationImporterWindow : UnityEditor.EditorWindow
    {
        static ThreepeatAnimationImporterWindow instance;
        static ThreepeatAnimationImporterSettings configInstance;
        public class ClipInfo
        {
            public bool isTurn = false;
            public float turnAngleCW = 0f;
            public float startSpeed = 0f;
            public float endSpeed = 0f;

            public bool loopable = false;
            public bool ybakeEligible = false;
        }

        public class PoseInfo
        {
            public Vector3 pos;
            public Quaternion rot;

            public PoseInfo(Transform trans)
            {
                pos = trans.position;
                rot = trans.rotation;
            }

            public PoseInfo(Vector3 tpos, Quaternion trot)
            {
                pos = tpos;
                rot = trot;
            }
        }

        //==============================================================================================
        //                                                                      MEMBER VARS
        //==============================================================================================

        private Vector2 scrollMain = Vector2.zero;
        private GUIStyle SectionHeaderStyleThin = null;

        public bool SectionImportedAnims_Expanded = false;
        public bool SectionFoundAnims_Expanded = false;
        public bool SectionManualImport_Expanded = true;

        static public GameObject sceneObject;
        static bool sceneObjectIsTemporary = false;
        public AnimationClip clipToTest;
        public bool debugMode = false;


        /*
        public Avatar avatar;
        public bool removeTPoseAnimIfPresent = true;
        public bool bakeYForMinimalYMotion = true;
        public float bakeY_maxPositiveYChange = 0.3f;
        public float bakeY_maxNegativeYChange = -0.5f;*/




        [MenuItem("Tools/Threepeat/Animation Importer")]
        static void CreateAnimationImporterWindow()
        {
            GetWindowInstance();
        }

        public static ThreepeatAnimationImporterWindow GetWindowInstance()
        {
            if (instance != null)
            {
                instance.CheckConfig();
                return instance;
            }

            instance = EditorWindow.GetWindow<ThreepeatAnimationImporterWindow>("Animation Importer");
            instance.CheckConfig();
            return instance;
        }

        public static ThreepeatAnimationImporterSettings GetConfig()
        {
            if (configInstance != null)
            {
                return configInstance;
            }

            string path = AssetDatabase.GUIDToAssetPath("7c21b60631414b44893559ee8dbf6f32");
            configInstance = AssetDatabase.LoadAssetAtPath<ThreepeatAnimationImporterSettings>(path);
            return configInstance;
        }

        public void CheckConfig()
        {
            GetConfig();
        }

        /*[MenuItem("Assets/Autodetect Animation Info")]
        public static void AutodetectAnimationInfo()
        {
            foreach (UnityEngine.Object obj in Selection.objects)
            {
                AnimationClip animationClip = obj as AnimationClip;
                ThreepeatAnimationImporterWindow window = GetWindowInstance();
                if (animationClip != null)
                {
                    window.GetClipInfo(window.sceneObject, animationClip);
                }
            }
        }*/

        //==============================================================================================
        //                                                                      ASSET CONTEXT MENU ITEMS
        //==============================================================================================


        [MenuItem("Assets/Threepeat Animation Importer/FULL: Auto-Humanoid and Create Avatar", false, 500)]
        public static void AutoHumanoid()
        {
            //ThreepeatAnimationImporterWindow window = GetWindowInstance();
            ThreepeatAnimationImporterSettings animconfig = GetConfig();
            Avatar backupAv = animconfig.avatar;

            animconfig.avatar = null;


            AutoconfigureAnimations();

            animconfig.avatar = backupAv;

            /*foreach (UnityEngine.Object obj in Selection.objects)
            {
                GameObject go = obj as GameObject;

                if (go != null)
                {
                    window.ConfigureModelPrefab_Stage1(go);
                    window.ConfigureModelPrefab_Stage2(go);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();*/
        }

        [MenuItem("Assets/Threepeat Animation Importer/FULL: Auto-Humanoid and Use Other Avatar", false, 501)]
        public static void AutoconfigureAnimationsMenuSelect()
        {
            ThreepeatAnimationImporterSettings animconfig = GetConfig();

            Rect buttonRect = new Rect(100, 100, 200, 100);


            //if (Event.current.type == EventType.Repaint) buttonRect = GUILayoutUtility.GetLastRect();
            if (animconfig.avatar == null)
            {
                PopupWindow.Show(buttonRect, new AvatarPopup());
            }
            else if (EditorUtility.DisplayDialog(
                            "Use previous avatar?",
                            "Do you want to use the Avatar from your last run or pick a new one?",
                            "Use Previous Avatar",
                            "Pick New Avatar"))
            {
                AutoconfigureAnimations();
            }
            else
            {
                PopupWindow.Show(buttonRect, new AvatarPopup());
            }

        }


        [MenuItem("Assets/Threepeat Animation Importer/QUICK: Auto-Humanoid and Create Avatar", false, 514)]
        public static void AutoHumanoidQuick()
        {
            //ThreepeatAnimationImporterWindow window = GetWindowInstance();
            ThreepeatAnimationImporterSettings animconfig = GetConfig();
            Avatar backupAv = animconfig.avatar;

            animconfig.avatar = null;

            AutoconfigureAnimations(false);

            animconfig.avatar = backupAv;

        }

        [MenuItem("Assets/Threepeat Animation Importer/QUICK: Auto-Humanoid and Use Other Avatar", false, 515)]
        public static void AutoconfigureAnimationsMenuSelectQuick()
        {
            ThreepeatAnimationImporterSettings animconfig = GetConfig();

            Rect buttonRect = new Rect(100, 100, 200, 100);


            //if (Event.current.type == EventType.Repaint) buttonRect = GUILayoutUtility.GetLastRect();
            if (animconfig.avatar == null)
            {
                PopupWindow.Show(buttonRect, new AvatarPopup());
            }
            else if (EditorUtility.DisplayDialog(
                            "Use previous avatar?",
                            "Do you want to use the Avatar from your last run or pick a new one?",
                            "Use Previous Avatar",
                            "Pick New Avatar"))
            {
                AutoconfigureAnimations(false);
            }
            else
            {
                PopupWindow.Show(buttonRect, new AvatarPopup(false));
            }

        }

        [MenuItem("Assets/Threepeat Animation Importer/   >>> Import with Full Detection >>>", false, 499)]
        public static void FullDetect() { }


        [MenuItem("Assets/Threepeat Animation Importer/   >>> Import with Full Detection >>>", true, 499)]
        public static bool FullDetectValidate()
        {
            return false;
        }

        [MenuItem("Assets/Threepeat Animation Importer/FULL: Auto-Humanoid and Create Avatar", true, 500)]
        public static bool AutoHumanoidValidate()
        {
            bool isgood = true;
            foreach (UnityEngine.Object obj in Selection.objects)
            {
                var assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
                ModelImporter importer = (ModelImporter)AssetImporter.GetAtPath(assetPath);

                if (importer == null)
                {
                    isgood = false;
                    break;
                }
            }

            return isgood;   
        }

        [MenuItem("Assets/Threepeat Animation Importer/FULL: Auto-Humanoid and Use Other Avatar", true, 501)]
        public static bool AutoHumanoidValidate2()
        {
            return AutoHumanoidValidate();
        }

        [MenuItem("Assets/Threepeat Animation Importer/   >>> Quick Import (no detection) >>>", false, 512)]
        public static void NoDetect() { }


        [MenuItem("Assets/Threepeat Animation Importer/   >>> Quick Import (no detection) >>>", true, 512)]
        public static bool NoDetectValidate()
        {
            return false;
        }


        [MenuItem("Assets/Threepeat Animation Importer/QUICK: Auto-Humanoid and Create Avatar", true, 514)]
        public static bool AutoHumanoidValidate3()
        {
            return AutoHumanoidValidate();
        }

        [MenuItem("Assets/Threepeat Animation Importer/QUICK: Auto-Humanoid and Use Other Avatar", true, 515)]
        public static bool AutoHumanoidValidate4()
        {
            return AutoHumanoidValidate();
        }


        public enum QuickActionsEnum
        {
            AddMirrors,
            FixNames,
            SetYRotBake,
            SetYBake,
            SetXZBake,
            YRotOriginal,
            YRotBodyOrientation,
            YOriginal,
            XZOriginal
        }

        private static void QuickActions(List<QuickActionsEnum> actions)
        {
            ThreepeatAnimationImporterSettings animconfig = GetConfig();
            foreach (UnityEngine.Object obj in Selection.objects)
            {
                var assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
                ModelImporter importer = (ModelImporter)AssetImporter.GetAtPath(assetPath);

                if (importer == null)
                {
                    Debug.LogError($"Selected item is not a model prefab: {obj.name}");
                    return;
                }

                List<ModelImporterClipAnimation> outClips = new List<ModelImporterClipAnimation>();

                foreach (ModelImporterClipAnimation clip in importer.clipAnimations)
                {
                    if (actions.Contains(QuickActionsEnum.FixNames))
                    {
                        if (clip.name.Equals("Unreal Take") || clip.name.Equals("Take 001") || clip.name.Equals("mixamo.com"))
                        {
                            clip.name = obj.name;
                        }
                    }

                    if (actions.Contains(QuickActionsEnum.SetYRotBake))
                    {
                        clip.lockRootRotation = true;
                    }

                    if (actions.Contains(QuickActionsEnum.YRotOriginal))
                    {
                        clip.keepOriginalOrientation = true;
                    }
                    else if (actions.Contains(QuickActionsEnum.YRotBodyOrientation))
                    {
                        clip.keepOriginalOrientation = false;
                    }

                    if (actions.Contains(QuickActionsEnum.YOriginal))
                    {
                        clip.keepOriginalPositionY = true;
                    }

                    if (actions.Contains(QuickActionsEnum.XZOriginal))
                    {
                        clip.keepOriginalPositionXZ = true;
                    }


                    if (actions.Contains(QuickActionsEnum.SetYBake))
                    {
                        clip.lockRootHeightY = true;
                    }

                    if (actions.Contains(QuickActionsEnum.SetXZBake))
                    {
                        clip.lockRootPositionXZ = true;
                    }


                    outClips.Add(clip);
                    //Debug.Log($"{clip.name} - loop( {clip.loopTime} ), bakeYRot( {clip.lockRootRotation} ), bakeY( {clip.lockRootHeightY} ), mirror( {clip.mirror} )");
                    if (actions.Contains(QuickActionsEnum.AddMirrors))
                    {

                        ModelImporterClipAnimation clipMirror = new ModelImporterClipAnimation();
                        CopyMICA(clip, clipMirror);
                        clipMirror.mirror = true;
                        clipMirror.name = animconfig.mirrorPrefix + clipMirror.name + animconfig.mirrorPostfix;
                        outClips.Add(clipMirror);
                    }

                }

                importer.clipAnimations = outClips.ToArray();
                AssetDatabase.WriteImportSettingsIfDirty(assetPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }


        [MenuItem("Assets/Threepeat Animation Importer/Quick Actions/Add Mirrors", false, 300)]
        public static void AddMirrors()
        {
            List<QuickActionsEnum> actions = new List<QuickActionsEnum>();
            actions.Add(QuickActionsEnum.AddMirrors);
            QuickActions(actions);
        }


        [MenuItem("Assets/Threepeat Animation Importer/Quick Actions/Add Mirrors", true, 300)]
        public static bool AddMirrorValidate()
        {
            return AutoHumanoidValidate();
        }

        [MenuItem("Assets/Threepeat Animation Importer/Quick Actions/Fix Names", false, 300)]
        public static void FixNames()
        {
            List<QuickActionsEnum> actions = new List<QuickActionsEnum>();
            actions.Add(QuickActionsEnum.FixNames);
            QuickActions(actions);
        }

        [MenuItem("Assets/Threepeat Animation Importer/Quick Actions/Fix Names", true, 300)]
        public static bool FixNameValidate()
        {
            return AutoHumanoidValidate();
        }


        [MenuItem("Assets/Threepeat Animation Importer/Quick Actions/Bake Y Rotation", false, 320)]
        public static void SetYRotBake()
        {
            List<QuickActionsEnum> actions = new List<QuickActionsEnum>();
            actions.Add(QuickActionsEnum.SetYRotBake);
            QuickActions(actions);
        }

        [MenuItem("Assets/Threepeat Animation Importer/Quick Actions/Bake Y Rotation", true, 320)]
        public static bool SetYRotBakeValidate()
        {
            return AutoHumanoidValidate();
        }

        [MenuItem("Assets/Threepeat Animation Importer/Quick Actions/Bake Y Position", false, 320)]
        public static void SetYBake()
        {
            List<QuickActionsEnum> actions = new List<QuickActionsEnum>();
            actions.Add(QuickActionsEnum.SetYBake);
            QuickActions(actions);
        }

        [MenuItem("Assets/Threepeat Animation Importer/Quick Actions/Bake Y Position", true, 320)]
        public static bool SetYBakeValidate()
        {
            return AutoHumanoidValidate();
        }

        [MenuItem("Assets/Threepeat Animation Importer/Quick Actions/Bake XZ Position", false, 320)]
        public static void SetYZBake()
        {
            List<QuickActionsEnum> actions = new List<QuickActionsEnum>();
            actions.Add(QuickActionsEnum.SetXZBake);
            QuickActions(actions);
        }

        [MenuItem("Assets/Threepeat Animation Importer/Quick Actions/Bake XZ Position", true, 320)]
        public static bool SetXZBakeValidate()
        {
            return AutoHumanoidValidate();
        }


        [MenuItem("Assets/Threepeat Animation Importer/Quick Actions/Y Rotation - Original", false, 340)]
        public static void YRotOriginal()
        {
            List<QuickActionsEnum> actions = new List<QuickActionsEnum>();
            actions.Add(QuickActionsEnum.YRotOriginal);
            QuickActions(actions);
        }

        [MenuItem("Assets/Threepeat Animation Importer/Quick Actions/Y Rotation - Original", true, 340)]
        public static bool YRotOriginalValidate()
        {
            return AutoHumanoidValidate();
        }

        [MenuItem("Assets/Threepeat Animation Importer/Quick Actions/Y Rotation - Body Orientation", false, 340)]
        public static void YRotBodyOrientationOriginal()
        {
            List<QuickActionsEnum> actions = new List<QuickActionsEnum>();
            actions.Add(QuickActionsEnum.YRotBodyOrientation);
            QuickActions(actions);
        }

        [MenuItem("Assets/Threepeat Animation Importer/Quick Actions/Y Rotation - Body Orientation", true, 340)]
        public static bool YRotBodyOrientationValidate()
        {
            return AutoHumanoidValidate();
        }

        [MenuItem("Assets/Threepeat Animation Importer/Quick Actions/Y Position - Original", false, 340)]
        public static void YOriginal()
        {
            List<QuickActionsEnum> actions = new List<QuickActionsEnum>();
            actions.Add(QuickActionsEnum.YOriginal);
            QuickActions(actions);
        }

        [MenuItem("Assets/Threepeat Animation Importer/Quick Actions/Y Position - Original", true, 340)]
        public static bool YOriginalValidate()
        {
            return AutoHumanoidValidate();
        }

        [MenuItem("Assets/Threepeat Animation Importer/Quick Actions/XZ Position - Original", false, 340)]
        public static void XZOriginal()
        {
            List<QuickActionsEnum> actions = new List<QuickActionsEnum>();
            actions.Add(QuickActionsEnum.XZOriginal);
            QuickActions(actions);
        }

        [MenuItem("Assets/Threepeat Animation Importer/Quick Actions/XZ Position - Original", true, 340)]
        public static bool XZOriginalValidate()
        {
            return AutoHumanoidValidate();
        }

        /*
        [MenuItem("Assets/Threepeat Animation Importer/Clear Previous Avatar", false, 512)]
        public static void ClearPreviousAvatar()
        {
            ThreepeatAnimationImporterSettings animconfig = GetConfig();
            animconfig.avatar = null;
        }*/

        [MenuItem("Assets/Threepeat Animation Importer/Configure Importer Settings", false, 532)]
        public static void ConfigureImporter()
        {
            ThreepeatAnimationImporterWindow window = GetWindowInstance();
            window.Focus();
        }

        public static void AutoconfigureAnimations(bool doDetection = true)
        {
            ThreepeatAnimationImporterSettings animconfig = GetConfig();

            foreach (UnityEngine.Object obj in Selection.objects)
            {
                AnimationClip animationClip = obj as AnimationClip;
                if (animationClip != null)
                {
                    //window.ConfigureAnimation(window.sceneObject, animationClip);
                    continue;
                }

                GameObject go = obj as GameObject;

                if (go != null)
                {
                    ConfigureModelPrefab_Stage1(go);
                }
                else
                {
                    Debug.LogError($"Selected item is not an animationclip or model prefab: {obj.name}");
                }

            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (doDetection)
            {
                foreach (UnityEngine.Object obj in Selection.objects)
                {
                    AnimationClip animationClip = obj as AnimationClip;
                    if (animationClip != null)
                    {
                        //window.ConfigureAnimation(window.sceneObject, animationClip);
                        continue;
                    }

                    GameObject go = obj as GameObject;

                    if (go != null)
                    {
                        ConfigureModelPrefab_Stage2(go);
                    }
                    else
                    {
                        Debug.LogError($"Selected item is not an animationclip or model prefab: {obj.name}");
                    }

                }
            }

            if (sceneObjectIsTemporary)
            {
                GameObject.DestroyImmediate(sceneObject);
                sceneObject = null;
            }


            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }

//==============================================================================================
//                                                                      OnGUI
//==============================================================================================

        private void OnGUI()
        {

            SetupStyles();

            EditorGUILayout.Space();

            EditorGUILayout.Space();

            scrollMain = EditorGUILayout.BeginScrollView(scrollMain);

            /*SectionImportedAnims_Expanded = EditorGUILayout.BeginFoldoutHeaderGroup(SectionImportedAnims_Expanded, "Imported Animations", SectionHeaderStyleThin);

            GUI.enabled = false;
            EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
            GUI.enabled = true;

            EditorGUILayout.EndFoldoutHeaderGroup();

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            SectionFoundAnims_Expanded = EditorGUILayout.BeginFoldoutHeaderGroup(SectionFoundAnims_Expanded, "Found Animations (importable)", SectionHeaderStyleThin);
            EditorGUILayout.EndFoldoutHeaderGroup();*/

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            SectionManualImport_Expanded = EditorGUILayout.BeginFoldoutHeaderGroup(SectionManualImport_Expanded, "Automatic Import Settings", SectionHeaderStyleThin);

            ThreepeatAnimationImporterSettings animconfig = GetConfig();

            if (SectionManualImport_Expanded)
            {
                //sceneObject = EditorGUILayout.ObjectField("SceneObject", sceneObject, typeof(GameObject), true) as GameObject;
                //clipToTest = EditorGUILayout.ObjectField("Animation Clip", clipToTest, typeof(AnimationClip), false) as AnimationClip;
                float originalValue = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 300;
                animconfig.avatar = EditorGUILayout.ObjectField("Avatar to use", animconfig.avatar, typeof(Avatar), false) as Avatar;
                animconfig.removeTPoseAnimIfPresent = EditorGUILayout.Toggle(new GUIContent("Remove T-Pose Anim", "This is needed for ActorCore animations"), animconfig.removeTPoseAnimIfPresent);
                animconfig.bakeYForMinimalYMotionClips = EditorGUILayout.Toggle(new GUIContent("Bake Y when Y Motion is within tresholds", "If true, bake Y when clip's Y motion variation is within thresholds (set below)"), animconfig.bakeYForMinimalYMotionClips);
                if (animconfig.bakeYForMinimalYMotionClips)
                {
                    animconfig.bakeY_maxPositiveYChange = EditorGUILayout.FloatField("Bake Y - Maximum Positive Deviation", animconfig.bakeY_maxPositiveYChange);
                    animconfig.bakeY_maxNegativeYChange = EditorGUILayout.FloatField("Bake Y - Maximum Negative Deviation", animconfig.bakeY_maxNegativeYChange);
                }
                animconfig.automaticallyAddMirrorsOfAllAnimations = EditorGUILayout.Toggle("Automatically add Mirrors for all Animations", animconfig.automaticallyAddMirrorsOfAllAnimations);
                /*if (animconfig.automaticallyAddMirrorsOfAllAnimations)
                {*/
                    animconfig.mirrorPrefix = EditorGUILayout.TextField(new GUIContent("Mirror Prefix", "Prefix to apply to the beginning of all mirrored animations"), animconfig.mirrorPrefix);
                    animconfig.mirrorPostfix = EditorGUILayout.TextField(new GUIContent("Mirror Postfix", "Prefix to apply to the end of all mirrored animations"), animconfig.mirrorPostfix);
                //}
                debugMode = EditorGUILayout.BeginToggleGroup("debugMode", debugMode);
                EditorGUILayout.EndToggleGroup();

                EditorGUIUtility.labelWidth = originalValue;
            }

            /*if ((sceneObject != null) && clipToTest != null)
            {
                if (GUILayout.Button("test animation"))
                {
                    GetClipInfo(sceneObject, clipToTest);
                }
            }*/

            EditorGUILayout.EndFoldoutHeaderGroup();


            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space();

            /*if (GUILayout.Button("Scan Project for Importable Animations"))
            {
                string[] guids = AssetDatabase.FindAssets("t:Avatar");
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    Avatar avatar = AssetDatabase.LoadAssetAtPath<Avatar>(path);
                    if (avatar != null)
                    {
                        Debug.Log("Avatar found: " + avatar.name + " at " + path);
                    }
                }
                //string[] filters = { "Asssets", "asset" };
                //EditorUtility.OpenFilePanelWithFilters("Select Avatar", "Assets/Plugins/Threepeat/AnimationImporter/Avatars", filters);
                Rect buttonRect = new Rect(100, 100, 200, 100);
                if (Event.current.type == EventType.Repaint) buttonRect = GUILayoutUtility.GetLastRect();
                PopupWindow.Show(buttonRect, new AvatarPopup());
            }*/

            /*if (GUILayout.Button("test clip"))
            {

                foreach (UnityEngine.Object obj in Selection.objects)
                {
                    var assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
                    ModelImporter importer = (ModelImporter)AssetImporter.GetAtPath(assetPath);

                    if (importer == null)
                    {
                        Debug.LogError($"Selected item is not a model prefab: {obj.name}");
                        return;
                    }

                    foreach (ModelImporterClipAnimation clip in importer.clipAnimations)
                    {
                        Debug.Log($"{clip.name} - loop( {clip.loopTime} ), bakeYRot( {clip.lockRootRotation} ), bakeY( {clip.lockRootHeightY} ), mirror( {clip.mirror} )");
                    }
                }
            }

            if (GUILayout.Button("add mirror"))
            {

                foreach (UnityEngine.Object obj in Selection.objects)
                {
                    var assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
                    ModelImporter importer = (ModelImporter)AssetImporter.GetAtPath(assetPath);

                    if (importer == null)
                    {
                        Debug.LogError($"Selected item is not a model prefab: {obj.name}");
                        return;
                    }

                    List<ModelImporterClipAnimation> outClips = new List<ModelImporterClipAnimation>();

                    foreach (ModelImporterClipAnimation clip in importer.clipAnimations)
                    {
                        outClips.Add(clip);
                        //Debug.Log($"{clip.name} - loop( {clip.loopTime} ), bakeYRot( {clip.lockRootRotation} ), bakeY( {clip.lockRootHeightY} ), mirror( {clip.mirror} )");
                        ModelImporterClipAnimation clipMirror = new ModelImporterClipAnimation();
                        CopyMICA(clip, clipMirror);
                        clipMirror.mirror = true;
                        clipMirror.name = animconfig.mirrorPrefix + clipMirror.name + animconfig.mirrorPostfix;
                        outClips.Add(clipMirror);

                    }

                    importer.clipAnimations = outClips.ToArray();
                    AssetDatabase.WriteImportSettingsIfDirty(assetPath);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }*/

        }

        public class AvatarPopup : PopupWindowContent
        {
            //private GUIStyle leftAlignedButtonStyle;

            public bool doDetection = true;

            public class AvatarInfo
            {
                public Avatar avatar;
                public string path;
            }

            public List<AvatarInfo> infos = new List<AvatarInfo>();

            public AvatarPopup(bool doDetect = true)
            {
                doDetection = doDetect;
            }

            public override Vector2 GetWindowSize()
            {
                return new Vector2(800, 600);
            }

            Vector2 scrollPos = Vector2.zero;

            public override void OnGUI(Rect rect)
            {
                GUIStyle leftAlignedButtonStyle = new GUIStyle(GUI.skin.button);
                leftAlignedButtonStyle.alignment = TextAnchor.MiddleLeft;
                EditorGUILayout.Space();
                GUILayout.Label("Select Avatar: ", EditorStyles.boldLabel);
                EditorGUILayout.Space();
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                foreach (AvatarInfo info in infos.OrderBy(info => info.avatar.name))
                {
                    
                    if (GUILayout.Button(new GUIContent(info.avatar.name, info.path), leftAlignedButtonStyle)) 
                    {
                        editorWindow.Close();
                        ThreepeatAnimationImporterSettings animconfig = GetConfig();
                        animconfig.avatar = info.avatar;
                        AutoconfigureAnimations(doDetection);

                    }
                }
                EditorGUILayout.EndScrollView();
            }

            public override void OnOpen()
            {
                
                //Debug.Log("Popup opened: " + this);
                string[] guids = AssetDatabase.FindAssets("t:Avatar");
                infos.Clear();
                foreach (string guid in guids)
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    Avatar avatar = AssetDatabase.LoadAssetAtPath<Avatar>(path);
                    if (avatar != null)
                    {
                        //Debug.Log("Avatar found: " + avatar.name + " at " + path);
                    }
                    AvatarInfo info = new AvatarInfo();
                    info.avatar = avatar;
                    info.path = path;
                    infos.Add(info);
                }
            }

            public override void OnClose()
            {
                //Debug.Log("Popup closed: " + this);
            }
        }

        

        public static void ConfigureModelPrefab_Stage1(GameObject go)
        {
            ThreepeatAnimationImporterSettings animconfig = GetConfig();
            if (sceneObject == null)
            {
                sceneObjectIsTemporary = true;
                string robotPath = AssetDatabase.GUIDToAssetPath("512ad92bdf2e60744bbf5bc8b7af1c30");
                GameObject robot = AssetDatabase.LoadAssetAtPath<GameObject>(robotPath);
                sceneObject = GameObject.Instantiate(robot);
            }

            var assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(go);
            ModelImporter importer = (ModelImporter)AssetImporter.GetAtPath(assetPath);

            if (importer == null)
            {
                Debug.LogError($"Selected item is not a model prefab: {go.name}");
                return;
            }

            importer.animationType = ModelImporterAnimationType.Human;

            if (animconfig.avatar != null)
            {
                importer.avatarSetup = ModelImporterAvatarSetup.CopyFromOther;
                importer.sourceAvatar = animconfig.avatar;
            }
            else
            {
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            }

            List<ModelImporterClipAnimation> theClips = new();

            Dictionary<string, TakeInfo> takes = new();

            foreach (TakeInfo take in importer.importedTakeInfos)
            {
                /*Debug.LogFormat("take name {0}, duration {1:2} numFrames {2}",
                    take.name,
                    take.stopTime - take.startTime,
                    Mathf.RoundToInt((take.stopTime - take.startTime) * take.sampleRate));*/

                takes.Add(take.name, take);
            }

            foreach (ModelImporterClipAnimation clip in importer.defaultClipAnimations)
            {
                bool useClip = true; // this is redundant since we're just continuing when we run into a bad clip
                                     //Debug.LogFormat("clip name {0}, takeName {1}", clip.name, clip.takeName);
                if (animconfig.removeTPoseAnimIfPresent)
                {
                    if (clip.name.Contains("T-Pose"))
                    {
                        Debug.Log("Found T-Pose, ignoring");
                        useClip = false;
                        continue;
                    }

                    // set clip to end of take (if frame starts at 0)
                    if ((clip.firstFrame == 0) && takes.ContainsKey(clip.takeName))
                    {
                        TakeInfo take = takes[clip.takeName];
                        clip.lastFrame = Mathf.RoundToInt((take.stopTime - take.startTime) * take.sampleRate);
                    }

                }
                if (clip.name.Equals("Unreal Take") || clip.name.Equals("Take 001") || clip.name.Equals("mixamo.com"))
                {
                    clip.name = go.name;
                }

                /*if (doYBakeForGameCreator)
                {
                    clip.lockRootHeightY = true;
                    clip.keepOriginalPositionY = true;
                    clip.heightOffset = 0.1f;
                }*/

                //if (changeBakeSettings && !doTheOldWayReOriginal)
                {
                    clip.keepOriginalOrientation = true;
                    //clip.keepOriginalPositionXZ = true;
                    clip.keepOriginalPositionY = true;
                }


                if (useClip)
                {
                    theClips.Add(clip);
                }
            }

            importer.clipAnimations = theClips.ToArray();


            AssetDatabase.WriteImportSettingsIfDirty(assetPath);
        }

        public static void ConfigureModelPrefab_Stage2(GameObject go)
        {
            ThreepeatAnimationImporterSettings animconfig = GetConfig();

            var assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(go);
            ModelImporter importer = (ModelImporter)AssetImporter.GetAtPath(assetPath);

            importer.animationType = ModelImporterAnimationType.Human;

            if (animconfig.avatar != null)
            {
                importer.avatarSetup = ModelImporterAvatarSetup.CopyFromOther;
                importer.sourceAvatar = animconfig.avatar;
            }
            else
            {
                importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            }



            UnityEngine.Object[] objects = AssetDatabase.LoadAllAssetsAtPath(assetPath);

            Dictionary<string, ClipInfo> clipInfos = new Dictionary<string, ClipInfo>();

            foreach (var obj in objects)
            {
                if (obj is AnimationClip)
                {
                    AnimationClip clip = obj as AnimationClip;
                    //Debug.Log("Imported Animation Clip: " + clip.name);

                    clipInfos[clip.name] = GetClipInfo(sceneObject, clip);
                }
            }

            if (importer == null)
            {
                Debug.LogError($"Selected item is not a model prefab: {go.name}");
                return;
            }

            List<ModelImporterClipAnimation> theClips = new();

            Dictionary<string, TakeInfo> takes = new();

            foreach (TakeInfo take in importer.importedTakeInfos)
            {
                /*Debug.LogFormat("take name {0}, duration {1:2} numFrames {2}",
                    take.name,
                    take.stopTime - take.startTime,
                    Mathf.RoundToInt((take.stopTime - take.startTime) * take.sampleRate));*/

                takes.Add(take.name, take);
            }

            foreach (ModelImporterClipAnimation clip in importer.defaultClipAnimations)
            {
                bool useClip = true; // this is redundant since we're just continuing when we run into a bad clip
                                     //Debug.LogFormat("clip name {0}, takeName {1}", clip.name, clip.takeName);
                if (animconfig.removeTPoseAnimIfPresent)
                {
                    if (clip.name.Contains("T-Pose"))
                    {
                        Debug.Log("Found T-Pose, ignoring");
                        useClip = false;
                        continue;
                    }

                    // set clip to end of take (if frame starts at 0)
                    if ((clip.firstFrame == 0) && takes.ContainsKey(clip.takeName))
                    {
                        TakeInfo take = takes[clip.takeName];
                        clip.lastFrame = Mathf.RoundToInt((take.stopTime - take.startTime) * take.sampleRate);
                    }

                }
                if (clip.name.Equals("Unreal Take") || clip.name.Equals("Take 001") || clip.name.Equals("mixamo.com"))
                {
                    clip.name = go.name;
                }




                /*if (doYBakeForGameCreator)
                {
                    clip.lockRootHeightY = true;
                    clip.keepOriginalPositionY = true;
                    clip.heightOffset = 0.1f;
                }*/

                //if (changeBakeSettings && !doTheOldWayReOriginal)
                {
                    clip.keepOriginalOrientation = true;
                    //clip.keepOriginalPositionXZ = true;
                    clip.keepOriginalPositionY = true;
                }

                if (clipInfos.ContainsKey(clip.name))
                {
                    ClipInfo info = clipInfos[clip.name];

                    if (info.loopable)
                    {
                        clip.loopTime = true;
                        clip.wrapMode = WrapMode.Loop;
                    }

                    if (!info.isTurn)
                    {
                        clip.lockRootRotation = true;
                    }

                    if (animconfig.bakeYForMinimalYMotionClips && info.ybakeEligible)
                    {
                        clip.lockRootHeightY = true;
                    }
                }
                else
                {
                    Debug.LogError($"clip not found( {clip.name} )!");
                }
            

                if (useClip)
                {
                    theClips.Add(clip);
                }

                if (animconfig.automaticallyAddMirrorsOfAllAnimations)
                {
                    ModelImporterClipAnimation clipMirror = new ModelImporterClipAnimation();
                    CopyMICA(clip, clipMirror);
                    clipMirror.mirror = true;
                    clipMirror.name = animconfig.mirrorPrefix + clipMirror.name + animconfig.mirrorPostfix;
                    theClips.Add(clipMirror);
                }
            }

            importer.clipAnimations = theClips.ToArray();


            AssetDatabase.WriteImportSettingsIfDirty(assetPath);
        }

        public static void CopyMICA(ModelImporterClipAnimation src, ModelImporterClipAnimation dest)
        {
            dest.additiveReferencePoseFrame = src.additiveReferencePoseFrame;
            dest.curves = src.curves;
            dest.cycleOffset = src.cycleOffset;
            dest.events = src.events;
            dest.firstFrame = src.firstFrame;
            dest.hasAdditiveReferencePose = src.hasAdditiveReferencePose;
            dest.heightFromFeet = src.heightFromFeet;
            dest.heightOffset = src.heightOffset;
            dest.keepOriginalOrientation = src.keepOriginalOrientation;
            dest.keepOriginalPositionXZ = src.keepOriginalPositionXZ;
            dest.keepOriginalPositionY = src.keepOriginalPositionY;
            dest.lastFrame = src.lastFrame;
            dest.lockRootHeightY = src.lockRootHeightY;
            dest.lockRootPositionXZ = src.lockRootPositionXZ;
            dest.lockRootRotation = src.lockRootRotation;
            dest.loop = src.loop;
            dest.loopPose = src.loopPose;
            dest.loopTime = src.loopTime;
            dest.maskSource = src.maskSource;
            dest.maskType = src.maskType;
            dest.mirror = src.mirror;
            dest.name = src.name;
            dest.rotationOffset = src.rotationOffset;
            dest.takeName = src.takeName;
            dest.wrapMode = src.wrapMode;
        }


        public static HumanBodyBones[] BONES_TO_CHECK =
        {
            HumanBodyBones.Neck,
            HumanBodyBones.Head,
            HumanBodyBones.Hips,
            HumanBodyBones.LeftHand,
            HumanBodyBones.LeftFoot,
            HumanBodyBones.RightHand,
            HumanBodyBones.RightFoot
        };

        public static ClipInfo GetClipInfo(GameObject sceneObj, AnimationClip clip)
        {
            ThreepeatAnimationImporterSettings animconfig = GetConfig();

            ClipInfo info = new ClipInfo();
            AnimationMode.StartAnimationMode();

            AnimationMode.BeginSampling();

            AnimationMode.SampleAnimationClip(
                sceneObj,
                clip,
                0
            );

            Animator animator = sceneObj.GetComponent<Animator>();


            bool ybakeEligible = animconfig.bakeYForMinimalYMotionClips;

            float maxYDiffFromStart = -100f;
            float minYDiffFromStart = 100f;


            // Root is stored in HumanBodyBones.LastBone
            Dictionary<HumanBodyBones, PoseInfo> startPose = GetPose("CLIP_START", sceneObj, animator);

            float startY = sceneObj.transform.position.y;

            List<Dictionary<HumanBodyBones, PoseInfo>> middlePoses = new List<Dictionary<HumanBodyBones, PoseInfo>>();

            float ydiff;

            for (float ii = 0.1f; ii < clip.length; ii += 0.1f)
            {
                AnimationMode.SampleAnimationClip(
                    sceneObj,
                    clip,
                    ii
                );
                Dictionary<HumanBodyBones, PoseInfo> pose = GetPose(ii.ToString(), sceneObj, animator);
                if (ybakeEligible)
                {
                    ydiff = (sceneObj.transform.position.y - startY);
                    if (ydiff > maxYDiffFromStart)
                    {
                        maxYDiffFromStart = ydiff;
                    }
                    if (ydiff < minYDiffFromStart)
                    {
                        minYDiffFromStart = ydiff;
                    }

                    if ((ydiff > animconfig.bakeY_maxPositiveYChange) || (ydiff < animconfig.bakeY_maxNegativeYChange))
                    {
                        ybakeEligible = false;
                    }

                }
                middlePoses.Add(pose);
            }

            if (middlePoses.Count > 0)
            {
                info.startSpeed = (middlePoses[0][HumanBodyBones.LastBone].pos - startPose[HumanBodyBones.LastBone].pos).magnitude / 0.1f;
            }

            AnimationMode.SampleAnimationClip(
                sceneObj,
                clip,
                clip.length
            );

            if (ybakeEligible)
            {
                ydiff = sceneObj.transform.position.y - startY;

                if ((ydiff > animconfig.bakeY_maxPositiveYChange) || (ydiff < animconfig.bakeY_maxNegativeYChange))
                {
                    ybakeEligible = false;
                }
            }

            Dictionary<HumanBodyBones, PoseInfo> endPose = GetPose("CLIP_END", sceneObj, animator);

            AnimationMode.SampleAnimationClip(
                sceneObj,
                clip,
                clip.length - 0.1f
            );

            Dictionary<HumanBodyBones, PoseInfo> preEndPose = GetPose("CLIP_PRE-END", sceneObj, animator);

            info.endSpeed = (preEndPose[HumanBodyBones.LastBone].pos - endPose[HumanBodyBones.LastBone].pos).magnitude/0.1f;


            AnimationMode.EndSampling();

            info.turnAngleCW = GetTurnAngle(startPose, endPose);
            info.isTurn = Mathf.Abs(info.turnAngleCW) > 0.1f;
            info.loopable = IsLoopable(startPose, endPose);
            info.ybakeEligible = ybakeEligible;

            if (animconfig.debugMode)
            {
                Debug.Log($"[ {clip.name} ]: turnAng( {info.turnAngleCW} ), isTurn( {info.isTurn} ), loopable( {info.loopable} ), startSpeed( {info.startSpeed} ), endSpeed( {info.endSpeed} )");
            }
            AnimationMode.StopAnimationMode();
            return info;
        }

        public static float GetTurnAngle(Dictionary<HumanBodyBones, PoseInfo> startPose, Dictionary<HumanBodyBones, PoseInfo> endPose)
        {
            return Quaternion.Angle(startPose[HumanBodyBones.LastBone].rot, endPose[HumanBodyBones.LastBone].rot);
        }

        public static bool IsLoopable(Dictionary<HumanBodyBones, PoseInfo> startPose, Dictionary<HumanBodyBones, PoseInfo> endPose)
        {
            ThreepeatAnimationImporterSettings animconfig = GetConfig();

            bool isLoopable = true;

            foreach (HumanBodyBones bone in BONES_TO_CHECK)
            {
                if (bone == HumanBodyBones.LastBone)
                {
                    continue;
                }
                PoseInfo pose1 = startPose[bone];
                PoseInfo pose2 = endPose[bone];
                if (((pose1.pos - pose2.pos).magnitude > 0.05f) || (Mathf.Abs(Quaternion.Angle(pose1.rot, pose2.rot)) > 0.1f)) 
                {
                    if (animconfig.debugMode)
                    {
                        Debug.Log($"BONE[{bone}] failed: pos( {pose1.pos} | {pose2.pos} ), rot({pose1.rot} | {pose2.rot}) ");
                    }
                    isLoopable = false;
                    break;
                }
            }

            return isLoopable;
        }


        // Root is stored in HumanBodyBones.LastBone
        private static Dictionary<HumanBodyBones, PoseInfo> GetPose(string label, GameObject sceneObject, Animator animator)
        {
            Dictionary<HumanBodyBones, PoseInfo> poses = new Dictionary<HumanBodyBones, PoseInfo>();


            PoseInfo rootPose = new PoseInfo(sceneObject.transform);
            poses.Add(HumanBodyBones.LastBone, rootPose);

            foreach (HumanBodyBones bone in BONES_TO_CHECK)
            {
                Transform boneTransform = animator.GetBoneTransform(bone);
                poses.Add(bone, new PoseInfo(boneTransform.localPosition, boneTransform.localRotation));
            }

            //Debug.Log($"{label}: transform pos( {sceneObject.transform.position} ), fwd( {sceneObject.transform.forward}");

            return poses;
        }

        private void SetupStyles()
        {
            SectionHeaderStyleThin = new GUIStyle(EditorStyles.foldoutHeader)
            {
                margin = new RectOffset(0, 0, 20, 20),
                padding = new RectOffset(50, 20, 60, 60),
                fontSize = 15,
                fontStyle = FontStyle.Bold,

            };
            Texture2D blackTex = MakeTex(32, 32, new Color(0.3f, 0.33f, 0.3f));

            SectionHeaderStyleThin.normal.background = blackTex;
            SectionHeaderStyleThin.normal.textColor = Color.white;
            SectionHeaderStyleThin.border.top = 25;
            SectionHeaderStyleThin.border.bottom = 25;

        }

        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];

            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();

            return result;
        }
    }
}