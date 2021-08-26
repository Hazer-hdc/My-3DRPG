using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class MyplayableBehaviour : PlayableBehaviour
{
    public ActorManager am;
    public float myFloat;

    //PlayableDirector pd;

    public override void OnGraphStart(Playable playable)
    {
        ////timeline结束时，清空导演的剧本
        //pd = (PlayableDirector)playable.GetGraph().GetResolver();
    }

    public override void OnGraphStop(Playable playable)
    {
        //if (pd != null)
        //{
        //    pd.playableAsset = null;
        //}
    }


    public override void PrepareFrame(Playable playable,FrameData info)
    {
        am.LockUnLockActorController(true);
    }

    public override void OnBehaviourPause(Playable playable,FrameData info)
    {
        am.LockUnLockActorController(false);
    }
}
