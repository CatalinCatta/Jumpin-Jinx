using UnityEngine;
using TMPro;
using System.Collections;
    
public class PlayerStatus : MonoBehaviour
{
    [SerializeField] private GameObject display;

    private int _coins;
    private int _hp = 500;

    public bool FreezedFromDamage;
    
    public void AddCoin()
    {
        _coins++;
        display.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = _coins.ToString();
    }

    public void GetDamage(Vector3 direction)
    {
        _hp--;
        display.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = _hp.ToString();
        StartCoroutine(MoveToDirection(direction.x));
        
        if (_hp == 0)
            Die();
    }
    
    private IEnumerator MoveToDirection(float direction)
    {
        FreezedFromDamage = true;
     
        var oldPosition = transform.position; 
        var targetPosition = new Vector3(oldPosition.x * 2.5f - direction * 1.5f, oldPosition.y + 1, -5);
        var elapsedTime = 0f;

        transform.GetComponent<SpriteRenderer>().color = Color.red;
        
        while (elapsedTime < 1f)
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, elapsedTime);
            elapsedTime += Time.deltaTime * 5;
            yield return null;
        }
        
        transform.GetComponent<SpriteRenderer>().color = Color.white;
        transform.position = targetPosition;

        FreezedFromDamage = false;
    }
    
    public void Die()
    {
        _hp = 0;
        display.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = _hp.ToString();
        Destroy(gameObject);
    }
}
