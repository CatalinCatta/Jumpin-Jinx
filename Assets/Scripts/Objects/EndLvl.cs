using System.Collections;
using UnityEngine;

public class EndLvl : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.gameObject.CompareTag("Player"))
            return;
        
        var playerStatus = col.transform.GetComponent<PlayerStatus>();
        var playerRb = col.transform.GetComponent<Rigidbody2D>();

        playerRb.isKinematic = true;
        playerRb.velocity = Vector2.zero;
        playerRb.angularVelocity = 0f;
        
        playerStatus.freezeFromDamage = true;
        transform.GetComponent<Animator>().enabled = true;
        StartCoroutine(AnimationFinished(playerStatus));
    }

    private IEnumerator AnimationFinished(PlayerStatus player)
    {
        transform.GetComponent<AudioSource>().Play();
        
        yield return new WaitForSeconds(0.6f);
        
        player.Win();
    }
    
}
