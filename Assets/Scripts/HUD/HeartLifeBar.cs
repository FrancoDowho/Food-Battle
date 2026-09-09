using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;

public class HeartLifeBar : NetworkBehaviour
{
    [HideInInspector] public GameObject myChar;
    [SerializeField] GameObject mask;
    [SerializeField] Vector3 offset;
    Animator _anim;
    // Start is called before the first frame update
    void Start()
    {
        _anim = gameObject.GetComponent<Animator>();
    }

    public override void Spawned()
    {
        base.Spawned();
       


    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]

    public void RPC_UpdateHeartBar(float progress)
    {
        if (myChar == null)
            return;
        
        transform.position = myChar.gameObject.transform.position + offset;
        _anim.SetTrigger("appears");

        StartCoroutine(BarSoftProgress(progress));
    }

    IEnumerator BarSoftProgress(float destiny)
    {
        yield return new WaitForSeconds(1);
        float speed = 0.025f;
        WaitForSeconds wait = new WaitForSeconds(0.03f);
        while (mask.transform.localScale.y >= destiny)
        {
            mask.transform.localScale = new Vector3(mask.transform.localScale.x, mask.transform.localScale.y - speed, mask.transform.localScale.z);
            yield return wait;
        }

        mask.transform.localScale = new Vector3(mask.transform.localScale.x, destiny, mask.transform.localScale.z);
        yield return new WaitForSeconds(4);
        transform.position = new Vector2(0, 1000);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
