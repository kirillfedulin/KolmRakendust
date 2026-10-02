using System;
using System.Drawing;
using System.Windows.Forms;

namespace ThreeAppsWinForms.MathGameApp;

public class MathGameForm : Form
{
    private const int TimeLimit = 30;     // секунд на один пример
    private const int WarningTime = 5;    // когда остаётся столько секунд — всё краснеет

    private MathQuestion _question;
    private int _score;
    private int _attempts;
    private int _timeLeft;
    private readonly System.Windows.Forms.Timer _timer;

    private readonly Label _questionLabel;
    private readonly TextBox _answerTextBox;
    private readonly Button _btnSubmit;
    private readonly Label _feedbackLabel;
    private readonly Label _scoreLabel;
    private readonly Label _timerLabel;
    private readonly ComboBox _difficultyCombo;
    private readonly Panel _barTrack;   // серая "дорожка" полоски времени
    private readonly Panel _barFill;    // цветная заполненная часть

    public MathGameForm()
    {
        Text = "Matemaatiline äraarvamismäng";
        ClientSize = new Size(420, 350);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        BackColor = Theme.Background;
        Font = Theme.Regular();

        var difficultyLabel = new Label
        {
            Text = "Keerukus:",
            Left = 20,
            Top = 20,
            Width = 80,
            Height = 24,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Theme.Muted
        };
        _difficultyCombo = new ComboBox
        {
            Left = 100,
            Top = 20,
            Width = 300,
            DropDownStyle = ComboBoxStyle.DropDownList,
            FlatStyle = FlatStyle.Flat
        };
        _difficultyCombo.Items.AddRange(new object[]
        {
            "Lihtne (+/-, kuni 10)",
            "Keskmine (+/-/*, kuni 50)",
            "Raske (+/-/*//, kuni 100)"
        });
        _difficultyCombo.SelectedIndex = 0;
        _difficultyCombo.SelectedIndexChanged += (s, e) => NewQuestion();

        // Белая "карточка" с примером
        _questionLabel = new Label
        {
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = Theme.Bold(26F),
            ForeColor = Theme.Ink,
            BackColor = Theme.Surface,
            TextAlign = ContentAlignment.MiddleCenter
        };
        var questionCard = new Panel
        {
            Left = 20,
            Top = 60,
            Width = 380,
            Height = 84,
            BackColor = Theme.Surface,
            BorderStyle = BorderStyle.FixedSingle
        };
        questionCard.Controls.Add(_questionLabel);

        _answerTextBox = new TextBox
        {
            Left = 110,
            Top = 160,
            Width = 200,
            Font = Theme.Regular(15F),
            TextAlign = HorizontalAlignment.Center
        };
        _answerTextBox.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // чтобы Windows не пищал при Enter
                SubmitAnswer();
            }
        };

        _btnSubmit = new Button
        {
            Text = "Vasta",
            Left = 110,
            Top = 204,
            Width = 200,
            Height = 40,
            Font = Theme.Bold(11F)
        };
        Theme.StyleButton(_btnSubmit, Theme.Accent, Theme.AccentHover, Color.White);
        _btnSubmit.Click += (s, e) => SubmitAnswer();

        _feedbackLabel = new Label
        {
            Left = 20,
            Top = 254,
            Width = 380,
            Height = 28,
            AutoSize = false,
            Font = Theme.Bold(11F),
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Theme.Success
        };

        // Полоска оставшегося времени
        _barTrack = new Panel { Left = 20, Top = 292, Width = 380, Height = 8, BackColor = Theme.Border };
        _barFill = new Panel { Left = 0, Top = 0, Width = 380, Height = 8, BackColor = Theme.Accent };
        _barTrack.Controls.Add(_barFill);

        _scoreLabel = new Label
        {
            Left = 20,
            Top = 312,
            Width = 190,
            Height = 24,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleLeft,
            ForeColor = Theme.Ink,
            Text = "Kontrolli: 0 / 0"
        };
        _timerLabel = new Label
        {
            Left = 210,
            Top = 312,
            Width = 190,
            Height = 24,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleRight,
            ForeColor = Theme.Ink,
            Text = $"Aega: {TimeLimit}"
        };

        Controls.Add(difficultyLabel);
        Controls.Add(_difficultyCombo);
        Controls.Add(questionCard);
        Controls.Add(_answerTextBox);
        Controls.Add(_btnSubmit);
        Controls.Add(_feedbackLabel);
        Controls.Add(_barTrack);
        Controls.Add(_scoreLabel);
        Controls.Add(_timerLabel);

        _timer = new System.Windows.Forms.Timer { Interval = 1000 };
        _timer.Tick += Timer_Tick;

        _question = CreateQuestionForDifficulty();
        NewQuestion();
    }

    private int GetMaxNumber() => _difficultyCombo.SelectedIndex switch
    {
        0 => 10,
        1 => 50,
        _ => 100
    };

    private MathQuestion CreateQuestionForDifficulty()
    {
        int level = _difficultyCombo.SelectedIndex;
        return new MathQuestion(
            GetMaxNumber(),
            includeMultiplication: level >= 1,
            includeDivision: level == 2,
            includeSubtraction: true);
    }

    private void NewQuestion()
    {
        _question = CreateQuestionForDifficulty();
        _questionLabel.Text = _question.ToString();
        _answerTextBox.Text = string.Empty;
        _answerTextBox.Focus();

        _timeLeft = TimeLimit;
        UpdateTimerVisuals();
        _timer.Stop();
        _timer.Start();
    }

    // Обновляет надпись времени, длину и цвет полоски
    private void UpdateTimerVisuals()
    {
        bool warning = _timeLeft <= WarningTime;
        _timerLabel.Text = $"Aega: {_timeLeft}";
        _timerLabel.ForeColor = warning ? Theme.Danger : Theme.Ink;
        _barFill.BackColor = warning ? Theme.Danger : Theme.Accent;
        _barFill.Width = Math.Max(0, _barTrack.Width * _timeLeft / TimeLimit);
    }

    private void ShowFeedback(string text, Color color)
    {
        _feedbackLabel.ForeColor = color;
        _feedbackLabel.Text = text;
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        _timeLeft--;
        UpdateTimerVisuals();
        if (_timeLeft <= 0)
        {
            _timer.Stop();
            _attempts++;
            int correct = _question.CorrectAnswer;
            UpdateScoreLabel();
            NewQuestion();
            // Сообщение показываем ПОСЛЕ NewQuestion, иначе оно сразу стёрлось бы
            ShowFeedback($"Aeg on läbi! Õige vastus: {correct}", Theme.Danger);
        }
    }

    private void SubmitAnswer()
    {
        if (!int.TryParse(_answerTextBox.Text, out int userAnswer))
        {
            ShowFeedback("Sisestage täisarv.", Theme.Danger);
            return;
        }

        _attempts++;
        string message;
        Color color;
        if (_question.CheckAnswer(userAnswer))
        {
            _score++;
            message = "Õige!";
            color = Theme.Success;
        }
        else
        {
            message = $"Vale. Õige vastus: {_question.CorrectAnswer}";
            color = Theme.Danger;
        }

        UpdateScoreLabel();
        NewQuestion();
        ShowFeedback(message, color);
    }

    private void UpdateScoreLabel() => _scoreLabel.Text = $"Kontrolli: {_score} / {_attempts}";

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _timer.Stop();
        _timer.Dispose();
        base.OnFormClosed(e);
    }
}
