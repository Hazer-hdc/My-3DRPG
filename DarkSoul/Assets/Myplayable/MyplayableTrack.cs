using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackColor(0f, 0.4716981f, 0.9433962f)]
[TrackClipType(typeof(MyplayableClip))]
[TrackBindingType(typeof(ActorManager))]
public class MyplayableTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        return ScriptPlayable<MyplayableMixerBehaviour>.Create (graph, inputCount);
    }
}
