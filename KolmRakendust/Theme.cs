using System.Drawing;
using System.Windows.Forms;

namespace ThreeAppsWinForms;

/// <summary>
/// Общая "палитра" и вспомогательные методы оформления для всех окон.
/// Меняете цвет здесь — он меняется во всём приложении.
/// </summary>
public static class Theme
{
    public static readonly Color Background = Color.FromArgb(240, 244, 246);
    public static readonly Color Surface = Color.White;
    public static readonly Color Ink = Color.FromArgb(28, 38, 48);         // тёмный текст и шапки
    public static readonly Color Muted = Color.FromArgb(104, 117, 130);    // второстепенный текст
    public static readonly Color Border = Color.FromArgb(214, 221, 227);
    public static readonly Color Accent = Color.FromArgb(0, 121, 128);     // основной акцент (бирюзовый)
    public static readonly Color AccentHover = Color.FromArgb(18, 148, 156);
    public static readonly Color AccentSoft = Color.FromArgb(226, 244, 245);
    public static readonly Color Success = Color.FromArgb(46, 139, 87);
    public static readonly Color Danger = Color.FromArgb(190, 60, 60);

    public static Font Regular(float size = 10F) => new("Segoe UI", size);
    public static Font Bold(float size = 10F) => new("Segoe UI", size, FontStyle.Bold);

    /// <summary>Делает кнопку плоской, со сменой цвета при наведении и нажатии.</summary>
    public static void StyleButton(Button button, Color back, Color hover, Color fore)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.BackColor = back;
        button.ForeColor = fore;
        button.Cursor = Cursors.Hand;
        button.UseVisualStyleBackColor = false;
        button.FlatAppearance.BorderSize = 0;
        button.FlatAppearance.MouseOverBackColor = hover;
        button.FlatAppearance.MouseDownBackColor = hover;
    }
}
