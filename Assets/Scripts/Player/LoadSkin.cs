using UnityEngine;
using UnityEngine.U2D.Animation;

public class LoadSkin : MonoBehaviour
{
    private void Start()
    {
        var playerManager = PlayerManager.Instance;
        foreach (Transform bodyPart in transform)
            if (bodyPart.TryGetComponent<SpriteResolver>(out var spriteResolver))
                spriteResolver.SetCategoryAndLabel(spriteResolver.GetCategory(),
                    playerManager.CurrentSkin[spriteResolver.GetCategory()]);
    }
}