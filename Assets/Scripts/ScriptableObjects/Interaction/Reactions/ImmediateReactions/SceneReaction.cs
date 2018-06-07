// The SceneReaction is used to change between scenes.
// Though there is a delay while the scene fades out,
// this is done with the SceneController class and so
// this is just a Reaction not a DelayedReaction.
using UnityEngine;

public class SceneReaction : Reaction
{
    public string sceneName;                    // The name of the scene to be loaded.
    //public string startingPointInLoadedScene;   // The name of the StartingPosition in the newly loaded scene.

    private SceneController sceneController;    // Reference to the SceneController to actually do the loading and unloading of scenes.


    protected override void SpecificInit()
    {
        //startingPointInLoadedScene = sceneName;

        sceneController = FindObjectOfType<SceneController> ();
    }

    public void ChangeScene()
    {
        ImmediateReaction();
    }


    protected override void ImmediateReaction()
    {
        Debug.Log("Scene reaction");
        // Start the scene loading process.
        sceneController.FadeAndLoadScene (this);
    }
}