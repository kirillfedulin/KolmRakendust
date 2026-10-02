using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ThreeAppsWinForms.ImageViewerApp;

public class ImageViewerForm : Form
{
    // Тёмная тема просмотрщика: картинки на тёмном фоне смотрятся лучше
    private static readonly Color BarColor = Color.FromArgb(30, 34, 40);
    private static readonly Color ToolButtonColor = Color.FromArgb(52, 58, 68);
    private static readonly Color ToolButtonHover = Color.FromArgb(72, 80, 94);

    private readonly ImageManager _manager = new();
    private readonly ToolTip _toolTip = new();

    private readonly PictureBox _pictureBox;
    private readonly Button _btnOpen;
    private readonly Button _btnPrevious;
    private readonly Button _btnNext;
    private readonly Button _btnZoomIn;
    private readonly Button _btnZoomOut;
    private readonly Button _btnRotate;
    private readonly Label _statusLabel;

    public ImageViewerForm()
    {
        Text = "Pildivaatur";
        ClientSize = new Size(800, 600);
        MinimumSize = new Size(720, 400); // меньше нельзя: иначе кнопки не поместятся
        KeyPreview = true;                // форма сама ловит горячие клавиши
        BackColor = BarColor;

        _pictureBox = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.CenterImage,
            BackColor = Color.FromArgb(18, 20, 24)
        };

        // Пустое состояние: подсказка вместо чёрного прямоугольника
        var hintFont = Theme.Regular(13F);
        _pictureBox.Paint += (s, e) =>
        {
            if (_pictureBox.Image != null) return;
            TextRenderer.DrawText(e.Graphics, "Avage pilt nupuga „Ava...“",
                hintFont, _pictureBox.ClientRectangle, Color.FromArgb(130, 140, 152),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        };

        _statusLabel = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 28,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(10, 0, 0, 0),
            BackColor = BarColor,
            ForeColor = Color.FromArgb(190, 198, 208),
            Text = "Pilte ei laaditud"
        };

        var toolPanel = new Panel { Dock = DockStyle.Top, Height = 52, BackColor = BarColor };

        // Раньше кнопки "Назад" и "Вперёд" перекрывали друг друга — теперь у каждой своё место
        _btnOpen = CreateToolButton("Ava...", 12, 100, accent: true);
        _btnPrevious = CreateToolButton("< Tagasi", 140, 90);
        _btnNext = CreateToolButton("Edasi >", 236, 90);
        _btnZoomIn = CreateToolButton("Suurenda (+)", 350, 112);
        _btnZoomOut = CreateToolButton("Vähenda (-)", 468, 112);
        _btnRotate = CreateToolButton("Pööra 90°", 586, 100);

        toolPanel.Controls.Add(_btnOpen);
        toolPanel.Controls.Add(_btnPrevious);
        toolPanel.Controls.Add(_btnNext);
        toolPanel.Controls.Add(_btnZoomIn);
        toolPanel.Controls.Add(_btnZoomOut);
        toolPanel.Controls.Add(_btnRotate);

        _toolTip.SetToolTip(_btnOpen, "Ava pildid (Ctrl+O)");
        _toolTip.SetToolTip(_btnPrevious, "Eelmine pilt (←)");
        _toolTip.SetToolTip(_btnNext, "Järgmine pilt (→)");
        _toolTip.SetToolTip(_btnZoomIn, "Suurenda (+)");
        _toolTip.SetToolTip(_btnZoomOut, "Vähenda (-)");
        _toolTip.SetToolTip(_btnRotate, "Pööra 90° (R)");

        _btnOpen.Click += BtnOpen_Click;
        _btnPrevious.Click += (s, e) => { _manager.Previous(); RefreshImage(); };
        _btnNext.Click += (s, e) => { _manager.Next(); RefreshImage(); };
        _btnZoomIn.Click += (s, e) => { _manager.ZoomIn(); RefreshImage(); };
        _btnZoomOut.Click += (s, e) => { _manager.ZoomOut(); RefreshImage(); };
        _btnRotate.Click += (s, e) => { _manager.Rotate90(); RefreshImage(); };

        KeyDown += ImageViewerForm_KeyDown;

        Controls.Add(_pictureBox);
        Controls.Add(toolPanel);
        Controls.Add(_statusLabel);

        UpdateButtons();
    }

    private static Button CreateToolButton(string text, int left, int width, bool accent = false)
    {
        var button = new Button
        {
            Text = text,
            Left = left,
            Top = 9,
            Width = width,
            Height = 34,
            Font = Theme.Regular(9.5F),
            TabStop = false
        };

        if (accent)
            Theme.StyleButton(button, Theme.Accent, Theme.AccentHover, Color.White);
        else
            Theme.StyleButton(button, ToolButtonColor, ToolButtonHover, Color.White);

        return button;
    }

    // Горячие клавиши: ← → переключают, + - масштаб, R поворот, Ctrl+O открыть
    private void ImageViewerForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.O)
        {
            e.SuppressKeyPress = true;
            BtnOpen_Click(this, EventArgs.Empty);
            return;
        }

        if (!_manager.HasImages) return;

        switch (e.KeyCode)
        {
            case Keys.Left: _manager.Previous(); break;
            case Keys.Right: _manager.Next(); break;
            case Keys.Add:
            case Keys.Oemplus: _manager.ZoomIn(); break;
            case Keys.Subtract:
            case Keys.OemMinus: _manager.ZoomOut(); break;
            case Keys.R: _manager.Rotate90(); break;
            default: return;
        }

        e.SuppressKeyPress = true;
        RefreshImage();
    }

    private void BtnOpen_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Pildid (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|Koik failid (*.*)|*.*",
            Multiselect = true,
            Title = "Valige pilt"
        };

        if (dialog.ShowDialog(this) != DialogResult.OK) return;

        foreach (var file in dialog.FileNames)
        {
            _manager.AddImage(file);
        }

        RefreshImage();
    }

    // Кнопки без смысла (нет картинок / картинка одна) делаем неактивными
    private void UpdateButtons()
    {
        bool hasImages = _manager.HasImages;
        bool manyImages = _manager.Count > 1;

        _btnPrevious.Enabled = manyImages;
        _btnNext.Enabled = manyImages;
        _btnZoomIn.Enabled = hasImages;
        _btnZoomOut.Enabled = hasImages;
        _btnRotate.Enabled = hasImages;
    }

    private void RefreshImage()
    {
        var previous = _pictureBox.Image;
        _pictureBox.Image = _manager.GetTransformedImage();
        previous?.Dispose();

        UpdateButtons();

        _statusLabel.Text = _manager.HasImages
            ? $"Pilt {_manager.CurrentIndex + 1} alates {_manager.Count}  |  {Path.GetFileName(_manager.CurrentPath)}  |  skaala {_manager.ZoomFactor:P0}"
            : "Pilte ei laaditud";
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _pictureBox.Image?.Dispose();
        _toolTip.Dispose();
        base.OnFormClosed(e);
    }
}
