using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ThreeAppsWinForms.ImageViewerApp;

public class ImageManager
{
    private readonly List<string> _imagePaths = new();
    private int _currentIndex = -1;
    private float _zoomFactor = 1.0f;
    private int _rotationAngle;

    public bool HasImages => _imagePaths.Count > 0;
    public int CurrentIndex => _currentIndex;
    public int Count => _imagePaths.Count;
    public float ZoomFactor => _zoomFactor;
    public string? CurrentPath => HasImages && _currentIndex >= 0 ? _imagePaths[_currentIndex] : null;

    public void AddImage(string path)
    {
        _imagePaths.Add(path);
        _currentIndex = _imagePaths.Count - 1;
        ResetTransform();
    }

    public void Next()
    {
        if (!HasImages) return;
        _currentIndex = (_currentIndex + 1) % _imagePaths.Count;
        ResetTransform();
    }

    public void Previous()
    {
        if (!HasImages) return;
        _currentIndex = (_currentIndex - 1 + _imagePaths.Count) % _imagePaths.Count;
        ResetTransform();
    }

    public void ZoomIn() => _zoomFactor = Math.Min(_zoomFactor + 0.1f, 5f);

    public void ZoomOut() => _zoomFactor = Math.Max(_zoomFactor - 0.1f, 0.1f);

    public void Rotate90() => _rotationAngle = (_rotationAngle + 90) % 360;

    private void ResetTransform()
    {
        _zoomFactor = 1.0f;
        _rotationAngle = 0;
    }

    // Загружает текущее изображение с диска и применяет масштаб/поворот,
    // возвращая готовый для отображения Bitmap.
 
    public Image? GetTransformedImage()
    {
        if (CurrentPath is null) return null;

        using var original = Image.FromFile(CurrentPath);

        int newWidth = Math.Max(1, (int)(original.Width * _zoomFactor));
        int newHeight = Math.Max(1, (int)(original.Height * _zoomFactor));

        var zoomed = new Bitmap(newWidth, newHeight);
        using (var g = Graphics.FromImage(zoomed))
        {
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(original, 0, 0, newWidth, newHeight);
        }

        if (_rotationAngle == 0) return zoomed;

        var rotated = new Bitmap(zoomed);
        rotated.RotateFlip(_rotationAngle switch
        {
            90 => RotateFlipType.Rotate90FlipNone,
            180 => RotateFlipType.Rotate180FlipNone,
            270 => RotateFlipType.Rotate270FlipNone,
            _ => RotateFlipType.RotateNoneFlipNone
        });
        zoomed.Dispose();
        return rotated;
    }
}
