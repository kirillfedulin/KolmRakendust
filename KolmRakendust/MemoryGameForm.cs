using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ThreeAppsWinForms.MemoryGameApp;

public class MemoryGameForm : Form
{
    private const int GridSize = 4; // 4x4 = 16 kaardid = 8 paar
    private static readonly string[] Symbols = { "+", "!", "=", "?", "%", "1", "2", "3" };

    private readonly List<MemoryCard> _cards = new();
    private readonly List<MemoryCard> _selectedCards = new();
    private readonly Panel _boardPanel;
    private readonly Label _statusLabel;
    private readonly Button _btnRestart;
    private readonly System.Windows.Forms.Timer _lockTimer;

    private int _moves;
    private int _matchedPairs;

    public MemoryGameForm()
    {
        Text = "Sarnane pildiotsija mäng";
        ClientSize = new Size(400, 480);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = Theme.Background;
        Font = Theme.Regular();

        _statusLabel = new Label
        {
            Dock = DockStyle.Top,
            Height = 40,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = Theme.Bold(10.5F),
            ForeColor = Theme.Ink,
            BackColor = Theme.Surface
        };

        _btnRestart = new Button
        {
            Text = "Proovi uuesti",
            Dock = DockStyle.Bottom,
            Height = 44,
            Font = Theme.Bold(10.5F)
        };
        Theme.StyleButton(_btnRestart, Theme.Ink, Color.FromArgb(48, 62, 76), Color.White);
        _btnRestart.Click += (s, e) => SetupBoard();

        _boardPanel = new Panel { Dock = DockStyle.Fill, BackColor = Theme.Background };

        Controls.Add(_boardPanel);
        Controls.Add(_statusLabel);
        Controls.Add(_btnRestart);

        _lockTimer = new System.Windows.Forms.Timer { Interval = 700 };
        _lockTimer.Tick += LockTimer_Tick;

        BuildGrid();
        SetupBoard();
    }

    private void BuildGrid()
    {
        const int cellSize = 80;
        const int gap = 6;

        // Размер области под поле: 400 x (480 - 40 - 44) = 400 x 396.
        // Считаем отступы так, чтобы сетка стояла ровно по центру.
        const int panelWidth = 400;
        const int panelHeight = 396;
        const int gridSpan = GridSize * cellSize + (GridSize - 1) * gap;
        const int marginX = (panelWidth - gridSpan) / 2;
        const int marginY = (panelHeight - gridSpan) / 2;

        for (int row = 0; row < GridSize; row++)
        {
            for (int col = 0; col < GridSize; col++)
            {
                var button = new Button
                {
                    Left = marginX + col * (cellSize + gap),
                    Top = marginY + row * (cellSize + gap),
                    Width = cellSize,
                    Height = cellSize,
                    Font = Theme.Bold(22F),
                    TabStop = false
                };
                Theme.StyleButton(button, Theme.Accent, Theme.AccentHover, Color.White);
                button.FlatAppearance.BorderSize = 2;
                button.Click += CardButton_Click;
                _boardPanel.Controls.Add(button);

                // Sumbol будет назначен в SetupBoard(); здесь заглушка.
                _cards.Add(new MemoryCard(string.Empty, button));
            }
        }
    }

    private void SetupBoard()
    {
        _lockTimer.Stop();
        _selectedCards.Clear();
        _moves = 0;
        _matchedPairs = 0;
        UpdateStatus();

        var deck = Symbols.Concat(Symbols).OrderBy(_ => Guid.NewGuid()).ToList();

        for (int i = 0; i < _cards.Count; i++)
        {
            var button = _cards[i].UiButton;
            var newCard = new MemoryCard(deck[i], button);
            _cards[i] = newCard;
            newCard.Hide();
            button.Enabled = true;
        }
    }

    private void CardButton_Click(object? sender, EventArgs e)
    {
        if (_lockTimer.Enabled) return; // игнор клика пока показ несовпавшую пару

        var clickedButton = (Button)sender!;
        var card = _cards.First(c => c.UiButton == clickedButton);

        if (card.IsFlipped || card.IsMatched) return;

        card.Flip();
        _selectedCards.Add(card);

        if (_selectedCards.Count < 2) return;

        _moves++;
        UpdateStatus();

        if (_selectedCards[0].Symbol == _selectedCards[1].Symbol)
        {
            foreach (var c in _selectedCards) c.MarkMatched();
            _selectedCards.Clear();
            _matchedPairs++;
            UpdateStatus();

            if (_matchedPairs == Symbols.Length)
            {
                MessageBox.Show(this, $"Palju õnne! Oled {_moves} käiguga kõik paarid kokku kogunud.",
                    "Võit", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        else
        {
            _lockTimer.Start();
        }
    }

    private void LockTimer_Tick(object? sender, EventArgs e)
    {
        _lockTimer.Stop();
        foreach (var c in _selectedCards) c.Hide();
        _selectedCards.Clear();
    }

    private void UpdateStatus() =>
        _statusLabel.Text = $"Käigud: {_moves}     Paarid leitud: {_matchedPairs} / {Symbols.Length}";

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _lockTimer.Stop();
        _lockTimer.Dispose();
        base.OnFormClosed(e);
    }
}
