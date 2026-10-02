using System;
using System.Collections.Generic;

namespace ThreeAppsWinForms.MathGameApp;

public class MathQuestion
{
    private static readonly Random Rng = new();

    public int FirstOperand { get; private set; }
    public int SecondOperand { get; private set; }
    public char Operator { get; private set; }
    public int CorrectAnswer { get; private set; }

    public MathQuestion(int maxNumber, bool includeMultiplication, bool includeDivision, bool includeSubtraction)
    {
        Generate(maxNumber, includeMultiplication, includeDivision, includeSubtraction);
    }

    public void Generate(int maxNumber, bool includeMultiplication, bool includeDivision, bool includeSubtraction)
    {
        var operators = new List<char> { '+' };
        if (includeSubtraction) operators.Add('-');
        if (includeMultiplication) operators.Add('*');
        if (includeDivision) operators.Add('/');

        Operator = operators[Rng.Next(operators.Count)];

        switch (Operator)
        {
            case '+':
                FirstOperand = Rng.Next(0, maxNumber + 1);
                SecondOperand = Rng.Next(0, maxNumber + 1);
                CorrectAnswer = FirstOperand + SecondOperand;
                break;

            case '-':
                FirstOperand = Rng.Next(0, maxNumber + 1);
                SecondOperand = Rng.Next(0, FirstOperand + 1); // избегаем отрицательного результата
                CorrectAnswer = FirstOperand - SecondOperand;
                break;

            case '*':
                int multMax = Math.Max(2, maxNumber / 2);
                FirstOperand = Rng.Next(0, multMax + 1);
                SecondOperand = Rng.Next(0, multMax + 1);
                CorrectAnswer = FirstOperand * SecondOperand;
                break;

            case '/':
                int divMax = Math.Max(2, maxNumber / 2);
                SecondOperand = Rng.Next(1, divMax + 1);       // делитель не равен нулю
                CorrectAnswer = Rng.Next(0, divMax + 1);       // частное
                FirstOperand = SecondOperand * CorrectAnswer;  // деление без остатка
                break;
        }
    }

    public bool CheckAnswer(int userAnswer) => userAnswer == CorrectAnswer;

    public override string ToString() => $"{FirstOperand} {Operator} {SecondOperand} = ?";
}
