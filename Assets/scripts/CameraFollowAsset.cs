using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class CameraFollowAsset : PlayableAsset
{
    public ExposedReference<GameObject> targetObject;
    public float shakeIntensity;
    public float followSpeed = 5f;
    public float yOffset = 2f;
    public float zOffset = 5f;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<CameraFollow>.Create(graph);

        var behaviour = playable.GetBehaviour();
        behaviour.targetObject = targetObject.Resolve(graph.GetResolver());
        behaviour.shakeIntensity = shakeIntensity;
        behaviour.followSpeed = followSpeed;
        behaviour.yOffset = yOffset;
        behaviour.zOffset = zOffset;

        return playable;
    }
}