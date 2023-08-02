using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public int gold;
    public int gems;

    public int atkLvl;
    public int msLvl;
    public int jumpLvl;
    public int defLvl;
    public int hpLvl;

    public int jumpBuffs;
    public int speedBuffs;
    public int secondChances;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

}
