using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.HighDefinition;

public class OnDemandShadowMapUpdate : MonoBehaviour
{
    // Update on camera move variables
    [HideInInspector]
    public bool updateOnCameraMove;
    [HideInInspector]
    public Camera cameraToTrack;
    
    // Enums for the shadow map to refresh and the counter mode
    [HideInInspector]
    public ShadowMapToRefresh shadowMapToRefresh = ShadowMapToRefresh.EntireShadowMap;
    [HideInInspector]
    public CounterMode counterMode = CounterMode.Frames;

    // Unity GameObjects and components
    [HideInInspector]
    public Camera mainCamera;
    private HDAdditionalLightData hdLight;

    // Full shadow map refresh counter variables
    [HideInInspector]
    public int fullShadowMapRefreshWaitFrames;
    [HideInInspector]
    public float fullShadowMapRefreshWaitSeconds;
    private int framesCounterFullShadowMap;
    private float secondsCounterFullShadowMap;

    // Cascades refresh counter variables
    [HideInInspector]
    public int[] cascadesRefreshWaitFrames = new int[NumberOfCascades];
    [HideInInspector]
    public float[] cascadesRefreshWaitSeconds = new float[NumberOfCascades];
    private int[] framesCounterCascades = new int[NumberOfCascades];
    private float[] secondsCounterCascades = new float[NumberOfCascades];
    private const int NumberOfCascades = 4;

    // Subshadows refresh counter variables
    [HideInInspector]
    public int[] subshadowsRefreshWaitFrames = new int[NumberOfSubshadows];
    [HideInInspector]
    public float[] subshadowsRefreshWaitSeconds = new float[NumberOfSubshadows];
    private int[] framesCounterSubshadows = new int[NumberOfSubshadows];
    private float[] secondsCounterSubshadows = new float[NumberOfSubshadows];
    private const int NumberOfSubshadows = 6;
    
    void Start()
    {
        ResetCounters();

        // Null check for the HDAdditionalLightData component
        hdLight = GetComponent<HDAdditionalLightData>();
        if (hdLight == null)
        {
            Debug.LogError("This script requires an HDAdditionalLightData component to work!");
            return;
        }

        // Cache the main camera and do a null check
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("There's no Camera GameObject with the MainCamera tag in the scene!");
            return;
        }
    }

    private void Update()
    {
        // Check which shadow map to refresh mode is selected
        switch (shadowMapToRefresh)
        {
            case ShadowMapToRefresh.EntireShadowMap:
                UpdateShadowMap();
                break;
            case ShadowMapToRefresh.Cascades:
                UpdateCascades();
                break;
            case ShadowMapToRefresh.Subshadows:
                UpdateSubshadows();
                break;
        }

        // Call this method if Update On Camera Movement is enabled
        if (updateOnCameraMove)
        {
            UpdateOnCameraMove();
        }
    }

    private void ResetCounters()
    {
        // Initialize the refresh wait frames counters for shadow maps
        framesCounterFullShadowMap = 0;
        secondsCounterFullShadowMap = 0;

        // Initialize the refresh wait frames counters for shadow cascades
        for (int i = 0; i < NumberOfCascades; i++)
        {
            framesCounterCascades[i] = 0;
            secondsCounterCascades[i] = 0;
        }
        
        // Initialize the refresh wait frames counters for subshadows
        for (int i = 0; i < NumberOfSubshadows; i++)
        {
            framesCounterSubshadows[i] = 0;
            secondsCounterSubshadows[i] = 0;
        }
    }

    private void UpdateShadowMap()
    {
        // Check which counter mode (frames or seconds) is selected
        switch (counterMode)
        {
            case CounterMode.Frames:
                framesCounterFullShadowMap++;
                if (fullShadowMapRefreshWaitFrames > 0 && framesCounterFullShadowMap >= fullShadowMapRefreshWaitFrames)
                {
                    hdLight.RequestShadowMapRendering();
                    framesCounterFullShadowMap = 0;
                }
                break;
            case CounterMode.Seconds:
                secondsCounterFullShadowMap += Time.deltaTime;
                if (fullShadowMapRefreshWaitSeconds > 0 && secondsCounterFullShadowMap >= fullShadowMapRefreshWaitSeconds)
                {
                    hdLight.RequestShadowMapRendering();
                    secondsCounterFullShadowMap = 0;
                }
                break;
        }
    }

    private void UpdateCascades()
    {
        // Check which counter mode (frames or seconds) is selected
        switch (counterMode)
        {
            case CounterMode.Frames:
                for (int i = 0; i < framesCounterCascades.Length; i++) 
                {
                    framesCounterCascades[i]++;

                    if (cascadesRefreshWaitFrames[i] > 0 && framesCounterCascades[i] >= cascadesRefreshWaitFrames[i])
                    {
                        hdLight.RequestSubShadowMapRendering(i);
                        framesCounterCascades[i] = 0;
                    }
                }
                break;
            case CounterMode.Seconds:
                for (int i = 0; i < secondsCounterCascades.Length; i++)
                {
                    secondsCounterCascades[i] += Time.deltaTime;

                    if (cascadesRefreshWaitSeconds[i] > 0 && secondsCounterCascades[i] >= cascadesRefreshWaitSeconds[i])
                    {
                        hdLight.RequestSubShadowMapRendering(i);
                        secondsCounterCascades[i] = 0;
                    }
                }
                break;
        }
    }

    private void UpdateSubshadows()
    {
        // Check which counter mode (frames or seconds) is selected
        switch (counterMode)
        {
            case CounterMode.Frames:
                for (int i = 0; i < framesCounterSubshadows.Length; i++) 
                {
                    framesCounterSubshadows[i]++;

                    if (subshadowsRefreshWaitFrames[i] > 0 && framesCounterSubshadows[i] >= subshadowsRefreshWaitFrames[i])
                    {
                        hdLight.RequestSubShadowMapRendering(i);
                        framesCounterSubshadows[i] = 0;
                    }
                }
                break;
            case CounterMode.Seconds:
                for (int i = 0; i < secondsCounterSubshadows.Length; i++)
                {
                    secondsCounterSubshadows[i] += Time.deltaTime;

                    if (subshadowsRefreshWaitSeconds[i] > 0 && secondsCounterSubshadows[i] >= subshadowsRefreshWaitSeconds[i])
                    {
                        hdLight.RequestSubShadowMapRendering(i);
                        secondsCounterSubshadows[i] = 0;
                    }
                }
                break;
        }
    }
    
    private void UpdateOnCameraMove()
    {
        if (cameraToTrack != null)
        {
            if (cameraToTrack.transform.hasChanged)
            {
                hdLight.RequestShadowMapRendering();
                cameraToTrack.transform.hasChanged = false;
            }
        }
    }

    [CustomEditor(typeof(OnDemandShadowMapUpdate))]
    public class ShadowMapRefreshEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            OnDemandShadowMapUpdate myScript = (OnDemandShadowMapUpdate)target;

            // Update on camera move UI toggle
            myScript.updateOnCameraMove = EditorGUILayout.Toggle("Update On Camera Movement", myScript.updateOnCameraMove);

            if (myScript.updateOnCameraMove)
            {
                DrawCameraField("Camera To Track", ref myScript.cameraToTrack);
                                
                // If the camera to track is null, display a help box and a assign the MainCamera automatically
                if (myScript.cameraToTrack == null)
                {
                    EditorGUILayout.HelpBox("The MainCamera GameObject will be automatically assigned if none is specified.", MessageType.Info);
                    myScript.cameraToTrack = myScript.mainCamera;
                }
            }
            
            // Counter mode UI
            myScript.counterMode = (CounterMode)EditorGUILayout.EnumPopup("Counter Mode", myScript.counterMode);

            // Shadow map to refresh UI
            myScript.shadowMapToRefresh = (ShadowMapToRefresh)EditorGUILayout.EnumPopup("Shadow Map To Update", myScript.shadowMapToRefresh);

            // Draw the appropriate UI based on the shadow map to refresh mode
            switch (myScript.shadowMapToRefresh)
            {
                case ShadowMapToRefresh.EntireShadowMap:
                    if (myScript.counterMode == CounterMode.Frames)
                    {
                        DrawShadowMapGUIIntField("Update Interval", ref myScript.fullShadowMapRefreshWaitFrames);
                    }
                    else if (myScript.counterMode == CounterMode.Seconds)
                    {
                        DrawShadowMapGUIFloatField("Update Interval", ref myScript.fullShadowMapRefreshWaitSeconds);
                    }
                    break;
                case ShadowMapToRefresh.Cascades:
                    if (myScript.counterMode == CounterMode.Frames)
                    {
                        DrawShadowMapGUIIntField("Cascade 1 Update Interval", ref myScript.cascadesRefreshWaitFrames[0]);
                        DrawShadowMapGUIIntField("Cascade 2 Update Interval", ref myScript.cascadesRefreshWaitFrames[1]);
                        DrawShadowMapGUIIntField("Cascade 3 Update Interval", ref myScript.cascadesRefreshWaitFrames[2]);
                        DrawShadowMapGUIIntField("Cascade 4 Update Interval", ref myScript.cascadesRefreshWaitFrames[3]);
                    }
                    else if (myScript.counterMode == CounterMode.Seconds)
                    {
                        DrawShadowMapGUIFloatField("Cascade 1 Update Interval", ref myScript.cascadesRefreshWaitSeconds[0]);
                        DrawShadowMapGUIFloatField("Cascade 2 Update Interval", ref myScript.cascadesRefreshWaitSeconds[1]);
                        DrawShadowMapGUIFloatField("Cascade 3 Update Interval", ref myScript.cascadesRefreshWaitSeconds[2]);
                        DrawShadowMapGUIFloatField("Cascade 4 Update Interval", ref myScript.cascadesRefreshWaitSeconds[3]);
                    }
                    break;
                case ShadowMapToRefresh.Subshadows:
                    if (myScript.counterMode == CounterMode.Frames)
                    {
                        DrawShadowMapGUIIntField("Subshadow 1 Update Interval", ref myScript.subshadowsRefreshWaitFrames[0]);
                        DrawShadowMapGUIIntField("Subshadow 2 Update Interval", ref myScript.subshadowsRefreshWaitFrames[1]);
                        DrawShadowMapGUIIntField("Subshadow 3 Update Interval", ref myScript.subshadowsRefreshWaitFrames[2]);
                        DrawShadowMapGUIIntField("Subshadow 4 Update Interval", ref myScript.subshadowsRefreshWaitFrames[3]);
                        DrawShadowMapGUIIntField("Subshadow 5 Update Interval", ref myScript.subshadowsRefreshWaitFrames[4]);
                        DrawShadowMapGUIIntField("Subshadow 6 Update Interval", ref myScript.subshadowsRefreshWaitFrames[5]);
                    }
                    else if (myScript.counterMode == CounterMode.Seconds)
                    {
                        DrawShadowMapGUIFloatField("Subshadow 1 Update Interval", ref myScript.subshadowsRefreshWaitSeconds[0]);
                        DrawShadowMapGUIFloatField("Subshadow 2 Update Interval", ref myScript.subshadowsRefreshWaitSeconds[1]);
                        DrawShadowMapGUIFloatField("Subshadow 3 Update Interval", ref myScript.subshadowsRefreshWaitSeconds[2]);
                        DrawShadowMapGUIFloatField("Subshadow 4 Update Interval", ref myScript.subshadowsRefreshWaitSeconds[3]);
                        DrawShadowMapGUIFloatField("Subshadow 5 Update Interval", ref myScript.subshadowsRefreshWaitSeconds[4]);
                        DrawShadowMapGUIFloatField("Subshadow 6 Update Interval", ref myScript.subshadowsRefreshWaitSeconds[5]);
                    }
                    break;  
            }
        }

        // Helper method for drawing int fields
        private void DrawShadowMapGUIIntField(string label, ref int refreshWaitFrames)
        {
            OnDemandShadowMapUpdate myScript = (OnDemandShadowMapUpdate)target;

            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            refreshWaitFrames = Mathf.Max(0, EditorGUILayout.IntField(label, refreshWaitFrames));
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("Frames", GUILayout.MaxWidth(43));
            EditorGUILayout.EndHorizontal();
        }
        
        // Helper method for drawing float fields
        private void DrawShadowMapGUIFloatField(string label, ref float refreshWaitSeconds)
        {
            OnDemandShadowMapUpdate myScript = (OnDemandShadowMapUpdate)target;

            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            refreshWaitSeconds = Mathf.Max(0, EditorGUILayout.FloatField(label, refreshWaitSeconds));
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("Seconds", GUILayout.MaxWidth(52));
            EditorGUILayout.EndHorizontal();
        }

        // Draw a Camera GameObject field
        private void DrawCameraField(string label, ref Camera gameObject)
        {
            OnDemandShadowMapUpdate myScript = (OnDemandShadowMapUpdate)target;

            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            myScript.cameraToTrack = (Camera)EditorGUILayout.ObjectField(label, gameObject, typeof(Camera), true);
            EditorGUI.indentLevel--;
            EditorGUILayout.EndHorizontal();
        }
    }
}

public enum ShadowMapToRefresh
{
    EntireShadowMap,
    Cascades,
    Subshadows
}

public enum CounterMode
{
    Frames,
    Seconds
}