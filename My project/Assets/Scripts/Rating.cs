using UnityEngine;
using System.Collections.Generic;

public enum RatingMode
{
    Default,        // Level 1 & 2: flip = lose rating, no flip = gain rating
    AlwaysIncrease, // Level 3: both outcomes increase rating
    Inverted        // Level 4 & 6: flip = gain rating, no flip = lose rating
}

/// <summary>
/// Draws a scrolling rating graph directly onto the RatingDisplay cube as a texture.
/// Attach to the "Rating" parent object. Other scripts call ChangeRating() or SetRating().
/// </summary>
public class Rating : MonoBehaviour
{
    public static RatingMode CurrentMode = RatingMode.Default;
    public Renderer ratingDisplayRenderer;
    public Color backgroundColor = new Color(0f, 0f, 0f, 0.9f);
    public Color lineColor = Color.magenta;
    [Range(1, 8)]
    public int lineThickness = 3;

    [Header("UV Orientation")]
    [Tooltip("Flip horizontally to match cube's front face UVs.")]
    public bool flipX = true;
    [Tooltip("Flip vertically to match cube's front face UVs.")]
    public bool flipY = true;

    [Header("Rating Settings")]
    [Range(0f, 1f)]
    public float startingRating = 0.2f;

    public float ratingLerpSpeed = 1f;
    public float secondsToFillGraph = 5f;
    public float pointInterval = 0.05f;

    [Header("Shader")]
    [Tooltip("Assign an Unlit/Texture shader (or URP Unlit) here so it works in builds.")]
    public Shader graphShader;

    [Header("Texture Resolution")]
    public int texWidth = 256;
    public int texHeight = 128;


    private float _currentRating;
    private float _targetRating;
    private float _timer;
    private float _graphX;
    private bool _scrolling;
    private List<float> _ratingHistory = new List<float>();
    private Texture2D _tex;
    private Color[] _clearPixels;
    private bool _dirty = true;

    void Start()
    {
        _currentRating = startingRating;
        _targetRating = startingRating;

        _tex = new Texture2D(texWidth, texHeight, TextureFormat.RGBA32, false);
        _tex.filterMode = FilterMode.Point;
        _tex.wrapMode = TextureWrapMode.Clamp;

        _clearPixels = new Color[texWidth * texHeight];
        for (int i = 0; i < _clearPixels.Length; i++)
            _clearPixels[i] = backgroundColor;

        if (ratingDisplayRenderer != null)
        {
            if (graphShader == null)
                graphShader = Shader.Find("Unlit/Texture");
            if (graphShader == null)
                graphShader = Shader.Find("Universal Render Pipeline/Unlit");

            Material mat = new Material(graphShader);
            mat.mainTexture = _tex;
            ratingDisplayRenderer.material = mat;
        }
        else
        {
            Debug.LogError("[Rating] ratingDisplayRenderer not assigned!");
        }

        _ratingHistory.Add(_currentRating);
        RedrawTexture();
    }

    void Update()
    {
        _currentRating = Mathf.MoveTowards(_currentRating, _targetRating, ratingLerpSpeed * Time.deltaTime);

        _timer += Time.deltaTime;
        float stepTime = Mathf.Max(pointInterval, 0.01f);
        if (_timer >= stepTime)
        {
            _timer -= stepTime;

            float dx = stepTime / Mathf.Max(secondsToFillGraph, 0.1f);
            _graphX += dx;

            if (_graphX >= 1f)
            {
                _scrolling = true;
                _graphX = 1f;
            }

            _ratingHistory.Add(_currentRating);

            if (_scrolling && _ratingHistory.Count > texWidth)
                _ratingHistory.RemoveAt(0);

            _dirty = true;
        }

        if (_dirty)
        {
            RedrawTexture();
            _dirty = false;
        }
    }

    private void RedrawTexture()
    {
        _tex.SetPixels(_clearPixels);

        if (_ratingHistory.Count < 2)
        {
            _tex.Apply();
            return;
        }

        int count = _ratingHistory.Count;
        float pixelsPerPoint;

        if (_scrolling)
        {
            pixelsPerPoint = (float)(texWidth - 1) / (count - 1);
        }
        else
        {
            float usedWidth = _graphX * (texWidth - 1);
            pixelsPerPoint = count > 1 ? usedWidth / (count - 1) : 1f;
        }

        for (int i = 0; i < count - 1; i++)
        {
            int x0 = Mathf.RoundToInt(i * pixelsPerPoint);
            int x1 = Mathf.RoundToInt((i + 1) * pixelsPerPoint);
            int y0 = Mathf.RoundToInt(_ratingHistory[i] * (texHeight - 1));
            int y1 = Mathf.RoundToInt(_ratingHistory[i + 1] * (texHeight - 1));

            // Flip to match cube UV orientation
            if (flipX)
            {
                x0 = (texWidth - 1) - x0;
                x1 = (texWidth - 1) - x1;
            }
            if (flipY)
            {
                y0 = (texHeight - 1) - y0;
                y1 = (texHeight - 1) - y1;
            }

            x0 = Mathf.Clamp(x0, 0, texWidth - 1);
            x1 = Mathf.Clamp(x1, 0, texWidth - 1);
            y0 = Mathf.Clamp(y0, 0, texHeight - 1);
            y1 = Mathf.Clamp(y1, 0, texHeight - 1);

            DrawLine(x0, y0, x1, y1);
        }

        _tex.Apply();
    }

    private void DrawLine(int x0, int y0, int x1, int y1)
    {
        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;
        int half = lineThickness / 2;

        while (true)
        {
            for (int tx = -half; tx <= half; tx++)
            {
                for (int ty = -half; ty <= half; ty++)
                {
                    int px = x0 + tx;
                    int py = y0 + ty;
                    if (px >= 0 && px < texWidth && py >= 0 && py < texHeight)
                        _tex.SetPixel(px, py, lineColor);
                }
            }

            if (x0 == x1 && y0 == y1) break;

            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x0 += sx; }
            if (e2 < dx)  { err += dx; y0 += sy; }
        }
    }
    public void ChangeRating(float amount)
    {
        _targetRating = Mathf.Clamp01(_targetRating + amount);
        Debug.Log($"[Rating] ChangeRating({amount:F2}) -> target={_targetRating:F2} current={_currentRating:F2}");
    }

    public void SetRating(float value)
    {
        _targetRating = Mathf.Clamp01(value);
    }
}