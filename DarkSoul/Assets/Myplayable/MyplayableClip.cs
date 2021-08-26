using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class MyplayableClip : PlayableAsset, ITimelineClipAsset
{
    public MyplayableBehaviour template = new MyplayableBehaviour ();
    public ExposedReference<ActorManager> am;

    public ClipCaps clipCaps
    {
        get { return ClipCaps.Blending; }
    }

    public override Playable CreatePlayable (PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<MyplayableBehaviour>.Create (graph, template);
        MyplayableBehaviour clone = playable.GetBehaviour ();


        clone.am = am.Resolve (graph.GetResolver ());
        return playable;
    }
}
