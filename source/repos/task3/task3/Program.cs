using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
/*
*             реализовать скрипт (в виде jar-файла или исполняемой сборки),
который реализует *обобщенную* игру камень-ножницы-бумага
*/

namespace task3
{
    class Program
    {
        class ValidationCondition
        {
            public string ErrorMessage { get; set; }
            public bool IsCorrect { get; set; }
            public ValidationCondition(bool condition, string errorMessage)
            {
                ErrorMessage = errorMessage;
                IsCorrect = condition;
            }
            public ValidationCondition()
            {
                ErrorMessage = "";
                IsCorrect = false;
            }
        }
        static void Main(string[] moves)
        {
            var inputStringValidationConditions = new List<ValidationCondition>()
            {
                new ValidationCondition(moves.Length % 2 != 0, "Error: An even number of moves. Odd quantity required. Example: Rock Paper Scissors"),
                new ValidationCondition(moves.Length >= 3, "Error: The number of moves is less than three. More required. Example: Rock Paper Scissors"),
                new ValidationCondition(moves.Distinct().Count() == moves.Length, "Error: The moves are repeated. Unique values required. Example: Rock Paper Scissors Lizard")
            };
            if (inputStringValidationConditions.All(condition => condition.IsCorrect == true))
            {
                int KeyLength = 16;
                var BytesData = new byte[KeyLength];
                RandomNumberGenerator.Fill(BytesData);
                var hmac = new HMACSHA256(BytesData);
                int computerMove = RandomNumberGenerator.GetInt32(moves.Length), userMove;
                var hash = hmac.ComputeHash(BitConverter.GetBytes(computerMove));
                Console.WriteLine($"HMAC: {BitConverter.ToString(hash).Replace("-", "")}");

                ValidationCondition inputUserMoveValidationCondition;
                while (true)
                {
                    Console.WriteLine("Make a move: ");
                    DisplayMenu(moves);
                    Console.Write(">> ");
                    userMove = int.Parse(Console.ReadLine());
                    inputUserMoveValidationCondition = new ValidationCondition(userMove > 0 && userMove <= moves.Length, $"Error: You entered a number out of the range of available values. You must enter a number from 1 to {moves.Length}");
                    userMove--;
                    if (inputUserMoveValidationCondition.IsCorrect) { Console.WriteLine($"Your move: {moves[userMove]}"); break; }
                    else { Console.WriteLine(inputUserMoveValidationCondition.ErrorMessage); continue; }
                };
                Console.WriteLine($"Computer move: {moves[computerMove]}" +
                    $"\n{determineWinner(moves, userMove, computerMove)}" +
                    $"\nHMAC key: {BitConverter.ToString(hmac.Key).Replace("-", "")}");
            }
            else
            {
                var unfulfilledConditions = inputStringValidationConditions.Select(condition => condition).Where(condition => condition.IsCorrect == false);
                foreach (ValidationCondition unCondition in unfulfilledConditions) Console.WriteLine(unCondition.ErrorMessage);
            }
        }

        public static void DisplayMenu(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {args[i]}");
            }
        }
        public static string determineWinner(string[] moves, int userMove, int computerMove)
        {
            int averageAmountOfMoves = moves.Length / 2;

            int lastWinner = computerMove == moves.Length - 1 ? 0 : ++computerMove;
            var winners = new List<string>(GetRequiredHalf(averageAmountOfMoves, moves, ref lastWinner));

            int lastLoser = lastWinner;
            var losers = new List<string>(GetRequiredHalf(averageAmountOfMoves, moves, ref lastLoser));

            if (winners.Contains(moves[userMove])) return "You win!";
            if (losers.Contains(moves[userMove])) return "Sorry, you lost";
            else return "Tie";
        }
        public static List<string> GetRequiredHalf(int requiredAmount, string[] strings, ref int reqString)
        {
            var list = new List<string>();
            for (int i = 0; i < requiredAmount; i++)
            {
                list.Add(strings[reqString]);
                if (reqString == strings.Length - 1) reqString = 0;
                else reqString++;
            }
            return list;
        }
    }
}
