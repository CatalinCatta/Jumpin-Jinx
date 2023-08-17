using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

public static class Utils
{
    public static readonly (int x, int y)[] DirectionsModifier =
    {
        ( 0,  1),          // Up
        ( 0, -1),          // Down
        (-1,  0),          // Left
        ( 1,  0)           // Right
    };
    
    private static readonly System.Random Random = new();

    public static int RandomPickNumberExcludingZero(int max) =>
        Random.Next(1, max);

    public static int RandomPickNumberBetween(int min, int max) =>
        Random.Next(min, max);

    public static string DoubleToString(double number, bool withDecimals) => number switch
    {
        >= 100_000_000_000_000 => (number / 1_000_000_000_000).ToString("F0") + " t",

        >= 10_000_000_000_000 => (number / 1_000_000_000_000).ToString("F1") + " t",

        >= 1_000_000_000_000 => (number / 1_000_000_000_000).ToString("F2") + " t",

        >= 100_000_000_000 => (number / 1_000_000_000).ToString("F0") + " b",

        >= 10_000_000_000 => (number / 1_000_000_000).ToString("F1") + " b",

        >= 1_000_000_000 => (number / 1_000_000_000).ToString("F2") + " b",

        >= 100_000_000 => (number / 1_000_000).ToString("F0") + " m",

        >= 10_000_000 => (number / 1_000_000).ToString("F1") + " m",

        >= 1_000_000 => (number / 1_000_000).ToString("F2") + " m",

        >= 100_000 => (number / 1_000).ToString("F0") + " k",

        >= 10_000 => (number / 1_000).ToString("F1") + " k",

        >= 1_000 => (number / 1_000).ToString("F2") + " k",

        >= 100 => number.ToString("F0") + " ",

        >= 10 => number.ToString(withDecimals ? "F1" : "F0") + " ",

        0 => "0 ",

        _ => number.ToString(withDecimals ? "F2" : "F0") + " "

    };

    public static IEnumerator PlaySoundOnDeath(GameObject actor)
    {
        var audioSource = actor.GetComponent<AudioSource>();

        audioSource.volume =
            SettingsManager.Instance.soundEffectVolume * SettingsManager.Instance.generalVolume;
        audioSource.Play();
        actor.GetComponent<Collider2D>().isTrigger = true;
        actor.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);

        yield return new WaitForSeconds(1f);

        Object.Destroy(actor);
    }

    public static (int, int) ResolutionIndexToTuple(int rez) => rez switch
    {
        0 => (640, 360),
        
        1 => (640, 480),
        
        2 => (800, 600),
        
        3 => (1024, 768),
        
        4 => (1280, 720),
        
        5 => (1280, 800),
        
        6 => (1280, 1024),
        
        7 => (1360, 768),
        
        8 => (1366, 768),
        
        9 => (1440, 900),
        
        10 => (1536, 864),
        
        11 => (1600, 900),
        
        12 => (1600, 1200),
        
        13 => (1680, 1050),
        
        14 => (1920, 1080),
        
        15 => (1920, 1200),
        
        16 => (2048, 1152),
        
        17 => (2048, 1536),
        
        18 => (2560, 1080),
        
        19 => (2560, 1440),
        
        20 => (2560, 1600),
        
        21 => (3840, 2160),
        
        _ => throw new NotSupportedException("This resolution is not supported yet.")
        
    };

    public static int ResolutionTupleToIndex(Resolution rez) => (rez.width, rez.height) switch
    {
        (640, 360) => 0,
        
        (640, 480) => 1,
        
        (800, 600) => 2,
        
        (1024, 768) => 3,
        
        (1280, 720) => 4,
        
        (1280, 800) => 5,
        
        (1280, 1024) => 6,
        
        (1360, 768) => 7,
        
        (1366, 768) => 8,
        
        (1440, 900) => 9,
        
        (1536, 864) => 10,
        
        (1600, 900) => 11,
        
        (1600, 1200) => 12,
        
        (1680, 1050) => 13,
        
        (1920, 1080) => 14,
        
        (1920, 1200) => 15,
        
        (2048, 1152) => 16,
        
        (2048, 1536) => 17,
        
        (2560, 1080) => 18,
        
        (2560, 1440) => 19,
        
        (2560, 1600) => 20,
        
        (3840, 2160) => 21,
        
        _ => 0
        
    };

    public static Vector2 GetColliderSize(Collision2D collision)
    {
        var otherCollider = collision.collider;

        var boxCollider2D = otherCollider.GetComponent<BoxCollider2D>();
        if (boxCollider2D != null)
        {
            var size = boxCollider2D.size;
            return new Vector2(size.x, size.y);
        }

        var circleCollider2D = otherCollider.GetComponent<CircleCollider2D>();
        if (circleCollider2D != null)
        {
            var radius = circleCollider2D.radius;
            return new Vector2(radius, radius);
        }

        var capsuleCollider2D = otherCollider.GetComponent<CapsuleCollider2D>();
        if (capsuleCollider2D != null)
        {
            var size = capsuleCollider2D.size;
            return new Vector2(size.x, size.y);
        }

        throw new ArgumentException("This Collider is not supported yet!");
    }
}
