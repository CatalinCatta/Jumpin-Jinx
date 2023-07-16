using System.Collections;
using UnityEngine;

public class EndLvl : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
            return;
        
        var playerStatus = collision.transform.GetComponent<PlayerStatus>();
        
        playerStatus.FreezedFromDamage = true;
        transform.GetComponent<Animator>().enabled = true;
        StartCoroutine(AnimationFinished(playerStatus));
    }

    private IEnumerator AnimationFinished(PlayerStatus player)
    {
        yield return new WaitForSeconds(0.6f);

        player.Win();
    }
    
}
