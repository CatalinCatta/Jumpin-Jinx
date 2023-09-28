using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

/// <summary>
/// Utility class for various helper functions.
/// </summary>
public static class Utility
{
    private static readonly Random Random = new();

    /// <summary>
    /// List of tuple for each direction.
    /// </summary>
    public static readonly (int x, int y)[] DirectionModifiers =
    {
        (0, 1), // Up
        (0, -1), // Down
        (-1, 0), // Left
        (1, 0) // Right
    };

    /// <summary>
    /// Picks a random number between 1 and max, excluding zero.
    /// </summary>
    /// <param name="max">The maximum value (exclusive).</param>
    /// <returns>A randomly selected number.</returns>
    public static int GetRandomNumberExcludingZero(int max) =>
        Random.Next(1, max);

    /// <summary>
    /// Picks a random number between min and max.
    /// </summary>
    /// <param name="min">The minimum value (inclusive).</param>
    /// <param name="max">The maximum value (exclusive).</param>
    /// <returns>A randomly selected number.</returns>
    public static int GetRandomNumberBetween(int min, int max) =>
        Random.Next(min, max);

    /// <summary>
    /// Converts a double to a string format with appropriate unit symbols.
    /// </summary>
    /// <param name="number">The number to convert.</param>
    /// <param name="withDecimals">Whether to include decimal places.</param>
    /// <returns>The formatted string representation of the number.</returns>
    public static string FormatDoubleWithUnits(double number, bool withDecimals) => number switch
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

    /// <summary>
    /// Plays a sound on death and handles object cleanup.
    /// </summary>
    /// <param name="actor">The GameObject representing the actor.</param>
    /// <returns>An IEnumerator for coroutine handling.</returns>
    public static IEnumerator PlayDeathSoundAndCleanup(GameObject actor)
    {
        var settingsManager = SettingsManager.Instance;

        if (actor.TryGetComponent<AudioSource>(out var audioSource))
        {
            audioSource.volume = settingsManager.SoundEffectVolume * settingsManager.GeneralVolume;
            audioSource.Play();   
        }
        if (actor.TryGetComponent<Collider2D>(out var collider)) collider.isTrigger = true;
        if (actor.TryGetComponent<SpriteRenderer>(out var sprite)) sprite.color = new Color(0, 0, 0, 0);

        yield return new WaitForSeconds(1f);

        Object.Destroy(actor);
    }

    /// <summary>
    /// Converts a resolution index to a resolution tuple.
    /// </summary>
    /// <param name="rez">The resolution index.</param>
    /// <returns>The resolution tuple (width, height).</returns>
    /// <exception cref="NotSupportedException">Thrown when the resolution index is not supported yet.</exception>
    public static (int, int) ConvertResolutionIndexToTuple(int rez) => rez switch
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

    /// <summary>
    /// Converts a resolution tuple to a resolution index.
    /// </summary>
    /// <param name="rez">The resolution tuple (width, height).</param>
    /// <returns>The resolution index.</returns>
    public static int ConvertResolutionTupleToIndex(Resolution rez) => (rez.width, rez.height) switch
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

    /// <summary>
    /// Retrieves the minAndMaxParticleSizes of the collider from a collision.
    /// </summary>
    /// <param name="collision">The collision information.</param>
    /// <returns>The collider minAndMaxParticleSizes as a Vector2.</returns>
    /// <exception cref="NotSupportedException">Thrown when the collider type is not supported.</exception>
    public static Vector2 RetrieveColliderSize(Collision2D collision)
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

        throw new NotSupportedException("This Collider is not supported yet!");
    }
}