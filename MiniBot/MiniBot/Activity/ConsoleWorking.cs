using MiniBot.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MiniBot.Activity.Sources;

namespace MiniBot.Activity
{
    partial class AssistantBot : IBot
    {
        private int ChoosePosition()
        {
            int result = -1;
            (int x, int y) position;
            position = Console.GetCursorPosition();
            int sX = position.x;
            int sY = position.y;

            int temp = _choices.Count - 1;
            Console.SetCursorPosition(position.x, --position.y);
            Console.CursorVisible = false;
            HighLight(_choices[_choices.Count - 1], ConsoleColor.DarkMagenta);

            while (true)
            {
                var cki = Console.ReadKey(true);
                if (temp >= 0 && temp < _choices.Count)
                {
                    HighLight(_choices[temp], ConsoleColor.White);
                }

                if (cki.Key == ConsoleKey.DownArrow && sY - position.y - 1 <= _choices.Count && sY - position.y - 1 > 0)
                {
                    if (_choices[temp + 1] != null && Equals(_choices[temp + 1], Indent))
                        position.y++;
                    Console.SetCursorPosition(position.x, position.y + 1);
                }
                else if (cki.Key == ConsoleKey.UpArrow && sY - position.y + 1 <= _choices.Count && sY - position.y + 1 > 0)
                {
                    if (_choices[temp - 1] != null && Equals(_choices[temp - 1], Indent))
                        position.y--;
                    Console.SetCursorPosition(position.x, position.y - 1);
                }
                else if (cki.Key == ConsoleKey.Enter)
                {
                    if (!Equals(_choices[temp], Indent))
                    {
                        result = temp;
                        Console.SetCursorPosition(sX, sY);
                        break;
                    }
                }
                else if (cki.Key == ConsoleKey.Escape)
                {
                    Console.CursorVisible = true;
                    Console.SetCursorPosition(sX, sY);
                    return -1;
                }

                position = Console.GetCursorPosition();
                temp = _choices.Count - sY + position.y;

                if (temp >= 0 && temp < _choices.Count)
                {
                    HighLight(_choices[temp], ConsoleColor.DarkMagenta);
                }
            }

            Console.CursorVisible = true;
            return result;
        }

        private void HighLight(string msg, ConsoleColor cc)
        {
            Console.ForegroundColor = cc;
            Console.Write("\r" + msg);
            Console.ResetColor();
        }

        private void AddChoice(string answer)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(answer);
            Console.ResetColor();
            _choices.Add(answer);
        }

        private void MakeChoice()
        {
            int index = ChoosePosition();
            while (index < 0)
            {
                index = ChoosePosition();
            }
            _buffer = _choices[index];

            WriteBotName(false);
            if (State == BotState.ShowMenu && index < _listID.Count)
                _currentID = _listID[index];
            else if (State == BotState.ShowBasket && index < _basket.Count)
                _currentID = _basket[index].id;
            _listID.Clear();

            Console.WriteLine(_choices[index].Substring(Indent.Length));

            _choices.Clear();
        }
    }
}
