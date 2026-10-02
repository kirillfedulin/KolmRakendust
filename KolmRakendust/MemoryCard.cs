using System.Drawing;
using System.Windows.Forms;

namespace ThreeAppsWinForms.MemoryGameApp;

public class MemoryCard
{
    public string Symbol { get; }
    public bool IsMatched { get; private set; }
    public bool IsFlipped { get; private set; }
    public Button UiButton { get; }

    public MemoryCard(string symbol, Button uiButton)
    {
        Symbol = symbol;
        UiButton = uiButton;
    }

    // Открытая карточка: белая, символ тёмный, бирюзовая рамка
    public void Flip()
    {
        IsFlipped = true;
        UiButton.Text = Symbol;
        Apply(Theme.Surface, Theme.Surface, Theme.Ink, Theme.Accent);
    }

    // Закрытая карточка ("рубашка"): сплошной бирюзовый цвет
    public void Hide()
    {
        IsFlipped = false;
        UiButton.Text = string.Empty;
        UiButton.Cursor = Cursors.Hand;
        Apply(Theme.Accent, Theme.AccentHover, Color.White, Theme.Accent);
    }

    // Найденная пара: зелёная. Кнопку НЕ отключаем (Enabled = false),
    // потому что у отключённой кнопки Windows красит текст в серый.
    // Повторный клик и так игнорируется проверкой IsMatched.
    public void MarkMatched()
    {
        IsMatched = true;
        UiButton.Cursor = Cursors.Default;
        Apply(Theme.Success, Theme.Success, Color.White, Theme.Success);
    }

    private void Apply(Color back, Color hover, Color fore, Color border)
    {
        UiButton.BackColor = back;
        UiButton.ForeColor = fore;
        UiButton.FlatAppearance.MouseOverBackColor = hover;
        UiButton.FlatAppearance.MouseDownBackColor = hover;
        UiButton.FlatAppearance.BorderColor = border;
    }
}
