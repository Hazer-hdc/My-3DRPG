using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

//每一个挂载了这个脚本的obj，都要求一个PlayableDirector组件，没有的话就会自动创建
[RequireComponent(typeof(PlayableDirector))]
public class DirectorManager : Manager
{
    PlayableDirector pd;

    //各个剧本资源Timeline
    [Header("====== Timeline Asset  =========")]
    public TimelineAsset frontStab;

    //某个剧本中的具体绑定的obj
    [Header("======  Asset Setting =========")]
    public ActorManager attackerAm;
    public ActorManager victimAm;

    // Start is called before the first frame update
    void Start()
    {
        pd = GetComponent<PlayableDirector>();
        pd.playOnAwake = false;
    }

    public void Play(string timelineName, ActorManager original, ActorManager target)
    {
        //Debug.Log("play");
        if (timelineName == "frontStab")
        {
            PlayFrontStab(original, target);
        }
        else if(timelineName == "openBox")
        {
            Debug.Log("try to play openbox");
        }
    }

    public void PlayFrontStab(ActorManager original, ActorManager target)
    {
        //如果剧本正在播放，就不允许打断重来
        if (pd.state == PlayState.Playing)
            return;

        //配置剧本,两种方法
        //pd.PlayableAsset = frontStab;
        pd.playableAsset = Instantiate(frontStab);

        //取得这个剧本
        TimelineAsset timeline = (TimelineAsset)pd.playableAsset;

        //取得所有轨道
        foreach (var track in timeline.GetOutputTracks())
        {
            if (track.name == "Attacker's ActorManager")
            {
                //改变这个轨道绑定的目标
                pd.SetGenericBinding(track, original);

                //取得这个轨道里面所有的clip，来修改clip的参数
                foreach (var clip in track.GetClips())
                {
                    MyplayableClip myclip = (MyplayableClip)clip.asset;
                    MyplayableBehaviour mybehav = myclip.template;

                    //这里要手动帮exposedName初始化，因为unity官方没有把它初始化
                    //这里初始化exposedName是为了让不同clip的myGameObject的exposeName有独一无二的标识。
                    //以免在后面利用exposedName来设置clip的参数时，因为exposedName相同而导致所有的clip的参数都一样。
                    myclip.am.exposedName = System.Guid.NewGuid().ToString();

                    //设置这个clip的参数为attackerAm，
                    //要利用exposedName，必须先将exposedname初始化。
                    pd.SetReferenceValue(myclip.am.exposedName, attackerAm);
                }
            }
            else if (track.name == "Victim's ActorManager")
            {
                pd.SetGenericBinding(track, target);

                //取得这个轨道里面所有的clip，来修改clip的参数
                foreach (var clip in track.GetClips())
                {
                    MyplayableClip myclip = (MyplayableClip)clip.asset;
                    MyplayableBehaviour mybehav = myclip.template;

                    //这里要手动帮exposedName初始化，因为unity官方没有把它初始化
                    //这里初始化exposedName是为了让不同clip的myGameObject的exposeName有独一无二的标识。
                    //以免在后面利用exposedName来设置clip的参数时，因为exposedName相同而导致所有的clip的参数都一样。
                    myclip.am.exposedName = System.Guid.NewGuid().ToString();

                    //设置这个clip的参数为victimAm
                    pd.SetReferenceValue(myclip.am.exposedName, victimAm);
                }
            }
            else if (track.name == "Attacker Animation")
            {
                //改变这个轨道绑定的目标
                pd.SetGenericBinding(track, original.GetActorController().GetAnimator());
            }
            else if (track.name == "Victim Animation")
            {
                pd.SetGenericBinding(track, target.GetActorController().GetAnimator());
            }
        }

        pd.Evaluate();

        //播放Timeline
        pd.Play();
    }

}
