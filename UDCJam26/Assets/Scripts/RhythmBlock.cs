using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmBlock : MonoBehaviour
{
    public void Despawn()
    {
        StartCoroutine(StartDespawnTimer());
    }

    private IEnumerator StartDespawnTimer()
    {
        yield return new WaitForSeconds(1f);
        this.gameObject.SetActive(false);
    }
}
