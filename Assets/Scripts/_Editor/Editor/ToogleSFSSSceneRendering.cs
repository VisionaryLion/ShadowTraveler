using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class ToogleSFSSSceneRendering {

    const float sfRendereUpdateRate = 30; //secs.
    static SceneView view;
    static SFRenderer sfRenderer;

    static ToogleSFSSSceneRendering()
    {
        view = SceneView.lastActiveSceneView;
        EditorApplication.update += CheckSceneStatus;
        sfRenderer = GameObject.FindObjectOfType<SFRenderer>();
    }

    //Runs every time the editor update
    static void CheckSceneStatus()
    {
        if (sfRenderer == null)
        {
            //Only look for a SFRenderer at a predefined frequency, to avoid overhead
            if(EditorApplication.timeSinceStartup % sfRendereUpdateRate < 0.1f)
                sfRenderer = GameObject.FindObjectOfType<SFRenderer>();
            return;
        }

        if (view == null)
        {
            view = SceneView.lastActiveSceneView;
            //Still no scene view
            if (view == null)
                return;
        }
        //Update the rendere in scene bool according to the sceneview toolbar.
        //Warning: As SFRenderer doesn't support multiple sceneviews with different settings, so does this script.
        sfRenderer._renderInSceneView = view.m_SceneLighting;
    }
}
