using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class TestDirector : MonoBehaviour
{
    public PlayableDirector pd;

    public Animator attacker;
    public Animator victim;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            //pd.playableAsset.outputs取得的是这个timeline的轨道数组。
            foreach (var track in pd.playableAsset.outputs)
            {
                if(track.streamName == "Attacker Animation")
                {
                    //改变这个轨道绑定的目标
                    pd.SetGenericBinding(track.sourceObject, attacker);
                }
                else if (track.streamName == "Victim Animation")
                {
                    pd.SetGenericBinding(track.sourceObject, victim);
                }
            }
            //pd.time = 0;
            //pd.Stop();
            //pd.Evaluate();
            pd.Play();
        }
    }
}
