using System;
using System.Drawing;
using System.Windows.Forms;
using ThreeAppsWinForms.ImageViewerApp;
using ThreeAppsWinForms.MathGameApp;
using ThreeAppsWinForms.MemoryGameApp;

namespace ThreeAppsWinForms;

public class MainMenuForm : Form
{
    private readonly Label _titleLabel;
    private readonly Label _subtitleLabel;
    private readonly Button _btnImageViewer;
    private readonly Button _btnMathGame;
    private readonly Button _btnMemoryGame;

    public MainMenuForm()
    {
        Text = "Kolm rakendust – peamenüü";
        ClientSize = new Size(420, 350);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = Theme.Background;
        Font = Theme.Regular();

        // Тёмная шапка с заголовком и подзаголовком
        var header = new Panel { Dock = DockStyle.Top, Height = 90, BackColor = Theme.Ink };

        _titleLabel = new Label
        {
            Text = "Valige rakendus",
            Font = Theme.Bold(17F),
            ForeColor = Color.White,
            BackColor = Color.Transparent,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Left = 0,
            Top = 14,
            Width = 420,
            Height = 36
        };

        _subtitleLabel = new Label
        {
            Text = "Kolm väikest programmi ühes aknas",
            Font = Theme.Regular(9.5F),
            ForeColor = Color.FromArgb(160, 205, 208),
            BackColor = Color.Transparent,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Left = 0,
            Top = 50,
            Width = 420,
            Height = 24
        };

        header.Controls.Add(_titleLabel);
        header.Controls.Add(_subtitleLabel);

        _btnImageViewer = CreateMenuButton("Pildivaatur", "Avage pilte, suurendage ja pöörake neid", 110);
        _btnMathGame = CreateMenuButton("Matemaatiline äraarvamismäng", "Lahendage näiteid ajalimiidiga", 186);
        _btnMemoryGame = CreateMenuButton("Sarnane pildiotsija mäng", "Leidke kõik kaardipaarid", 262);

        _btnImageViewer.Click += (s, e) => OpenChildForm(new ImageViewerForm());
        _btnMathGame.Click += (s, e) => OpenChildForm(new MathGameForm());
        _btnMemoryGame.Click += (s, e) => OpenChildForm(new MemoryGameForm());

        Controls.Add(_btnMemoryGame);
        Controls.Add(_btnMathGame);
        Controls.Add(_btnImageViewer);
        Controls.Add(header);
    }

    // Кнопка-карточка: белая, с рамкой, жирный заголовок и серое описание под ним.
    // Текст рисуем сами в событии Paint, чтобы у заголовка и описания были разные шрифты.
    private static Button CreateMenuButton(string title, string description, int top)
    {
        var button = new Button
        {
            Left = 30,
            Top = top,
            Width = 360,
            Height = 64,
            AccessibleName = title
        };
        Theme.StyleButton(button, Theme.Surface, Theme.AccentSoft, Theme.Ink);
        button.FlatAppearance.BorderSize = 1;
        button.FlatAppearance.BorderColor = Theme.Border;

        var titleFont = Theme.Bold(11F);
        var descFont = Theme.Regular(9F);

        button.Paint += (s, e) =>
        {
            TextRenderer.DrawText(e.Graphics, title, titleFont,
                new Rectangle(18, 10, button.Width - 36, 24), Theme.Accent,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
            TextRenderer.DrawText(e.Graphics, description, descFont,
                new Rectangle(18, 34, button.Width - 36, 20), Theme.Muted,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
        };

        return button;
    }

    //Открывает дочернюю форму как модальное окно, центрированное на экране
    private static void OpenChildForm(Form form)
    {
        form.StartPosition = FormStartPosition.CenterScreen;
        using (form)
        {
            form.ShowDialog();
        }
    }
}
