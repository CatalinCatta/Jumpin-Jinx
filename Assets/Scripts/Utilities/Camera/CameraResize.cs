using System;
using UnityEngine;

/// <summary>
/// Change camera minAndMaxParticleSizes and position at the beginning of scene based on player's screen resolution.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraResize : MonoBehaviour
{
    private float _currentResolution;

    private void Update()
    {
        if (WasResolutionChanged()) ChangeResolution();
    }

    private bool WasResolutionChanged()
    {
        // var resolution = Screen.currentResolution;
        var newResolution = (float)Screen.width / Screen.height;
        var wasChanged = Math.Abs(newResolution - _currentResolution) < .05f;
        _currentResolution = newResolution;
        
        return wasChanged;
    }

    private void ChangeResolution()
    {
        
        float size, yPosition;

        switch (_currentResolution)
        {
            case >= 1.7f:
                size = 11;
                yPosition = 0;
                break;

            case >= 1.6f:
                size = 12;
                yPosition = 1;
                break;

            case >= 1.5f:
                size = 13;
                yPosition = 2;
                break;

            case >= 1.4f:
                size = 14;
                yPosition = 2;
                break;

            case >= 1.3f:
                size = 15;
                yPosition = 2;
                break;

            case >= 1.2f:
                size = 16;
                yPosition = 2;
                break;

            case >= 1.1f:
                size = 17.5f;
                yPosition = 2;
                break;

            case >= 1f:
                size = 19;
                yPosition = 3;
                break;

            default:
                size = 19 + 1.5f * (_currentResolution - 1f) * 10;
                yPosition = 3;
                break;
        }

        GetComponent<Camera>().orthographicSize = size;
        transform.position = new Vector3(0, yPosition, -15);
    }
    
}