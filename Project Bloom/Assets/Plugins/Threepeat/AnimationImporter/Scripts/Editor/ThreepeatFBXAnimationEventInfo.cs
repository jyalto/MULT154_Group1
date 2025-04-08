using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace ThreepeatEditor
{
#if ENABLE_THREEPEAT_DEV_MODE
    [CreateAssetMenu(fileName = "FBXAnimationInfo", menuName = "Threepeat/FBXAnimationEventInfo")]
#endif
    public class ThreepeatFBXAnimationEventInfo : ScriptableObject
    {
        public string guiName = "";

        [Multiline(2)]
        public string description = "";

        public bool addAnimationEvents = true;
        public bool addPerAnimationSettings = false;
        public bool clearRootMotionNode = false;
        public bool disableAnimationCompression = false;

        public List<FBXInfo> fbxPrefabs = new List<FBXInfo>();

        /*
        [MenuItem("Assets/Get Anim Prop List")]
        private static void GetAnimPropList()
        {
            foreach (UnityEngine.Object obj in Selection.objects)
            {
                AnimationClip animationClip = obj as AnimationClip;

                if (animationClip != null)
                {
                    if (animationClip.hasRootCurves)
                    {
                        EditorCurveBinding[] curves = AnimationUtility.GetCurveBindings(animationClip);

                        for (int i = 0; i < curves.Length; ++i)
                        {
                            Debug.Log($"[{animationClip.name}]: {curves[i].path} : {curves[i].propertyName}");
                        }
                    }
                }
            }
        }*/

        /*
        [MenuItem("Assets/Check Start and End Pose")]
        private static void CheckStartEndPose()
        {
            foreach (UnityEngine.Object obj in Selection.objects)
            {
                AnimationClip animationClip = obj as AnimationClip;

                if (animationClip != null)
                {
                    if (animationClip.hasRootCurves)
                    {
                        EditorCurveBinding[] curves = AnimationUtility.GetCurveBindings(animationClip);

                        for (int i = 0; i < curves.Length; ++i)
                        {
                            if (curves[i].propertyName.Contains("LocalRotation"))
                            {
                                AnimationCurve tmpCurve = AnimationUtility.GetEditorCurve(animationClip, curves[i]);
                                // process root curve?
                                AnimationCurve curve = RootMotionTesting.ProcessRootCurve(tmpCurve);
                                float start = curve.Evaluate(0);
                                float end = curve.Evaluate(1);
                                if (Mathf.Abs(start - end) < 0.1f)
                                {
                                    //Debug.Log($"[{animationClip.name}]: {curves[i].path} : {curves[i].propertyName} - SAME");
                                }
                                else
                                {
                                    Debug.Log($"[{animationClip.name}]: {curves[i].path} : {curves[i].propertyName} - DIFF");
                                }
                            }
                        }
                    }
                }
            }
        }

        [MenuItem("Assets/Run Anim Detection")]
        private static void AnimDetection()
        {
            //string[] a = Selection.assetGUIDs.OfType<string>().Select(o => o.ToString()).ToArray();
            // Do something with you variable
            //Debug.LogFormat("Active object GUID: {0}", Selection.activeObject.GetType());
            RootMotionTesting rmt = RootMotionTesting.CreateInstance<RootMotionTesting>();
            foreach (UnityEngine.Object obj in Selection.objects)
            {
                rmt.clip = obj as AnimationClip;
                rmt.ExtractRM();
                //rmt.ComputeYRot();
                Quaternion start = new Quaternion(rmt.rotX.Evaluate(0f), rmt.rotY.Evaluate(0f), rmt.rotZ.Evaluate(0f), rmt.rotW.Evaluate(0f));
                Quaternion end = new Quaternion(rmt.rotX.Evaluate(1), rmt.rotY.Evaluate(1), rmt.rotZ.Evaluate(1), rmt.rotW.Evaluate(1));
                bool straightLine = Mathf.Abs(end.y - start.y) < 0.1f;
                Debug.Log($"[{rmt.clip.name}]: Angle Change: {end.x - start.x}, {end.y - start.y}, {end.z - start.z} -> {(straightLine ? "straight" : "turning")}");
            }
        }

        [MenuItem("Assets/Run Anim Detection", true)]
        private static bool AnimDetectionValidator()
        {
            // Do something with you variable
            return (Selection.activeObject != null) && (Selection.activeObject.GetType() == typeof(AnimationClip));
        }*/


        [ContextMenu("Populate this SO with selected model prefabs")]
        public void PopulateThisObjectWithSelection()
        {
            foreach (UnityEngine.Object obj in Selection.objects)
            {
                var assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
                Debug.LogFormat("{0}", assetPath);

                ModelImporter importer = (ModelImporter)AssetImporter.GetAtPath(assetPath);

                ThreepeatFBXAnimationEventInfo.FBXInfo theInfo = ReadAnimationEventsFromFBXPrefab(obj, ref importer);
                theInfo.fbxOriginalPath = assetPath;
                theInfo.fbxGuid = AssetDatabase.AssetPathToGUID(assetPath);

                /*if (importer.motionNodeName.Length > 0)
                {
                    Debug.LogFormat("Clearing RMNode for {0}", importer.name);
                    importer.motionNodeName = "";
                }*/
                theInfo.RMNodeIsNone = importer.motionNodeName.Length <= 0;

                this.fbxPrefabs.Add(theInfo);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }

        internal bool HasModificationAlreadyBeenApplied()
        {
            foreach (FBXInfo info in fbxPrefabs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(info.fbxGuid);

                if (string.IsNullOrEmpty(assetPath))
                {
                    Debug.LogErrorFormat("Can't find asset by guid.  Original import path is {0}", info.fbxOriginalPath);
                    return false;
                }

                ModelImporter importer = (ModelImporter)AssetImporter.GetAtPath(assetPath);

                if (importer == null)
                {
                    return false;
                }

                if (clearRootMotionNode && (importer.motionNodeName.Length > 0))
                {
                    return false;
                }

                ModelImporterClipAnimation[] importerAnimationsToUse = importer.clipAnimations;

                if (importer.clipAnimations.Length <= 0)
                {
                    importerAnimationsToUse = importer.defaultClipAnimations;
                }

                foreach (ModelImporterClipAnimation clip in importerAnimationsToUse) //.defaultClipAnimations)
                {

                    // try and find clip in info.clips
                    StringAnimEvent saclip = FindClip(info.clips, clip.name);

                    if (saclip == null)
                    {
                        // no events for this clip
                        continue;
                    }

                    if (saclip.clipEvents.Count > clip.events.Length)
                    {
                        return false;
                    }
                }
            }
            return true;
        }


        [ContextMenu("Automatically add all Events to FBX Prefabs")]
        public void ApplyEvents()
        {
            foreach (FBXInfo info in fbxPrefabs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(info.fbxGuid);

                if (string.IsNullOrEmpty(assetPath))
                {
                    Debug.LogErrorFormat("Can't find asset by guid.  Original import path is {0}", info.fbxOriginalPath);
                    return;
                }

                ModelImporter importer = (ModelImporter)AssetImporter.GetAtPath(assetPath);

                bool changesMade = false;
                List<ModelImporterClipAnimation> outClips = new List<ModelImporterClipAnimation>();

                if (clearRootMotionNode)
                {
                    importer.motionNodeName = "";
                    changesMade = true;
                }

                if (disableAnimationCompression)
                {
                    importer.animationCompression = ModelImporterAnimationCompression.Off;
                    changesMade = true;
                }

                ModelImporterClipAnimation[] importerAnimationsToUse = importer.clipAnimations;

                if (importer.clipAnimations.Length <= 0)
                {
                    importerAnimationsToUse = importer.defaultClipAnimations;
                }


                foreach (ModelImporterClipAnimation clip in importerAnimationsToUse) //.defaultClipAnimations)
                {

                    // try and find clip in info.clips
                    StringAnimEvent saclip = FindClip(info.clips, clip.name);

                    if (saclip == null)
                    {
                        outClips.Add(clip);
                        // no events for this clip
                        continue;
                    }

                    if (addPerAnimationSettings)
                    {
                        clip.keepOriginalOrientation = saclip.keepOriginalOrientation;
                        clip.keepOriginalPositionXZ = saclip.keepOriginalPositionXZ;
                        clip.keepOriginalPositionY = saclip.keepOriginalPositionY;
                        clip.lockRootHeightY = saclip.lockRootHeightY;
                        clip.lockRootPositionXZ = saclip.lockRootPositionXZ;
                        clip.lockRootRotation = saclip.lockRootRotation;
                        changesMade = true;
                    }


                    // check if events are already in the destination clip, and if not, add them (and set changesMade to true)
                    List<AnimationEvent> newEvents = new List<AnimationEvent>(clip.events);
                    bool eventsAlreadyInList = true;

                    foreach (SerializableAnimationEvent sae in saclip.clipEvents)
                    {
                        bool evInList = false;
                        int evInListNeedsMod = -1;
                        for (int ii=0; ii < newEvents.Count; ii++)
                        {
                            if (sae.Equals(newEvents[ii]))
                            {
                                evInList = true;
                                break;
                            }
                            else if ((sae.time == newEvents[ii].time) && (sae.functionName == newEvents[ii].functionName) && (sae.intParameter == newEvents[ii].intParameter))
                            {
                                evInListNeedsMod = ii;
                                break;
                            }
                        }

                        if (evInList)
                        {
                            // do nothing;
                        }
                        else if (evInListNeedsMod >= 0)
                        {
                            eventsAlreadyInList = false;
                            changesMade = true;
                            newEvents[evInListNeedsMod] = sae.ToAnimationEvent();
                        }
                        else
                        {
                            eventsAlreadyInList = false;
                            changesMade = true;
                            // add this event
                            newEvents.Add(sae.ToAnimationEvent());
                        }
                    }

                    // sort new events by time:
                    newEvents = newEvents.OrderBy(item => item.time).ToList();

                    if (!eventsAlreadyInList)
                    {
                        clip.events = newEvents.ToArray();
                    }

                    outClips.Add(clip);
                }

                if (changesMade)
                {
                    importer.clipAnimations = outClips.ToArray();
                }
                AssetDatabase.WriteImportSettingsIfDirty(assetPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private StringAnimEvent FindClip(List<StringAnimEvent> clips, string clipName)
        {
            foreach (StringAnimEvent sae in clips)
            {
                if (sae.clipName.Equals(clipName))
                {
                    return sae;
                }
            }

            return null;
        }

        private void ReadAnimationEventsFromFBXPrefab()
        {
            ThreepeatFBXAnimationEventInfo infos = new ThreepeatFBXAnimationEventInfo();
            foreach (UnityEngine.Object obj in Selection.objects)
            {
                var assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
                Debug.LogFormat("{0}", assetPath);

                ModelImporter importer = (ModelImporter)AssetImporter.GetAtPath(assetPath);
                ThreepeatFBXAnimationEventInfo.FBXInfo theInfo = ReadAnimationEventsFromFBXPrefab(obj, ref importer);
                theInfo.fbxOriginalPath = assetPath;
                theInfo.fbxGuid = AssetDatabase.AssetPathToGUID(assetPath);
                infos.fbxPrefabs.Add(theInfo);
            }

            var savePath = EditorUtility.SaveFilePanelInProject("Save FBXAnimationEventInfo", "FBXAnimationEventInfo", "asset", "asset");

            if (string.IsNullOrEmpty(savePath))
            {
                // do nothing, they canceled the save dialog
            }
#if UNITY_2021_3_OR_NEWER
            else if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(savePath, AssetPathToGUIDOptions.OnlyExistingAssets)))
#else
            else if (!string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(savePath)))
#endif
            {
                EditorUtility.DisplayDialog("File Exists!", "That file already exists, please delete it or pick a different save path.", "OK");
#if UNITY_2021_3_OR_NEWER
                Debug.LogFormat("Can't save to {0}, {1}", savePath, string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(savePath, AssetPathToGUIDOptions.OnlyExistingAssets)));
#else
                Debug.LogFormat("Can't save to {0}, {1}", savePath, string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(savePath)));
#endif
            }
            else
            {
                Debug.LogFormat("Saving to {0}", savePath);
                AssetDatabase.CreateAsset(infos, savePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        public static ThreepeatFBXAnimationEventInfo.FBXInfo ReadAnimationEventsFromFBXPrefab(UnityEngine.Object prefab, ref ModelImporter importer)
        {
            //importer.animationType = ModelImporterAnimationType.Human;
            //importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;

            List<ModelImporterClipAnimation> theClips = new List<ModelImporterClipAnimation>();

            ThreepeatFBXAnimationEventInfo.FBXInfo info = new ThreepeatFBXAnimationEventInfo.FBXInfo();
            info.clips = new List<ThreepeatFBXAnimationEventInfo.StringAnimEvent>();

            ModelImporterClipAnimation[] importerAnimationsToUse = importer.clipAnimations;

            if (importer.clipAnimations.Length <= 0)
            {
                importerAnimationsToUse = importer.defaultClipAnimations;
            }

            foreach (ModelImporterClipAnimation clip in importerAnimationsToUse) //.defaultClipAnimations)
            {
                ThreepeatFBXAnimationEventInfo.StringAnimEvent events = new ThreepeatFBXAnimationEventInfo.StringAnimEvent();
                events.clipName = clip.name;

                events.keepOriginalOrientation = clip.keepOriginalOrientation;
                events.keepOriginalPositionXZ = clip.keepOriginalPositionXZ;
                events.keepOriginalPositionY = clip.keepOriginalPositionY;
                events.lockRootHeightY = clip.lockRootHeightY;
                events.lockRootPositionXZ = clip.lockRootPositionXZ;
                events.lockRootRotation = clip.lockRootRotation;

                //Debug.Log($"{clip.takeName}/{clip.name}");
                foreach (AnimationEvent evt in clip.events)
                {
                    Debug.Log($"EVENT[{clip.takeName}/{clip.name}] - {evt.functionName}");
                    events.clipEvents.Add(new ThreepeatFBXAnimationEventInfo.SerializableAnimationEvent(evt));
                }
                /*if (events.clipEvents.Count > 0)
                {*/
                info.clips.Add(events);
                //}
            }

            return info;
        }


        [Serializable]
        public class SerializableAnimationEvent
        {
            public string functionName;
            public int intParameter = 0;
            public float floatParameter = 0f;
            public string stringParameter;
            public float time;

            public SerializableAnimationEvent(AnimationEvent evt)
            {
                this.functionName = evt.functionName;
                this.intParameter = evt.intParameter;
                this.floatParameter = evt.floatParameter;
                this.stringParameter = evt.stringParameter;
                this.time = evt.time;
            }

            public bool Equals(AnimationEvent evt)
            {

                return functionName.Equals(evt.functionName) && (intParameter == evt.intParameter) && (time == evt.time) && stringParameter.Equals(evt.stringParameter);
                    
            }

            internal AnimationEvent ToAnimationEvent()
            {
                AnimationEvent evt = new AnimationEvent();

                evt.functionName = functionName;
                evt.intParameter = intParameter;
                evt.floatParameter = floatParameter;
                evt.stringParameter = stringParameter;
                evt.time = time;
                return evt;
            }
        }

        [Serializable]
        public class StringAnimEvent
        {
            public string clipName;
            public List<SerializableAnimationEvent> clipEvents = new List<SerializableAnimationEvent>();
            
            public bool keepOriginalOrientation = true;
            public bool keepOriginalPositionXZ = false;
            public bool keepOriginalPositionY = true;
            public bool lockRootHeightY = false;
            public bool lockRootPositionXZ = false;
            public bool lockRootRotation = false;
        }

        [Serializable]
        public struct FBXInfo
        {
            public string fbxGuid;
            public string fbxOriginalPath;
            public List<StringAnimEvent> clips;
            public bool RMNodeIsNone;
        }
    }
}