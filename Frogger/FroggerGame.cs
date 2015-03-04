﻿/*
1. Include libraries (System, System.Collections.Generic...) DONE
2. Create the main struct for the objects (struct Object...) DONE
3. Start class FroggerGame and add main constants (HEIGHT, WIDTH, LIFES...)
4. Create methods PrintObject(int x, int y, char c, int length, ConsoleColor color) and 
 * PrintText(int x, int y, string str, ConsoleColor color = ConsoleColor.Gray) DONE
5. Display Welcome Screen Method - DisplayWelcomeMessage()
6. Main Variables - lives, speed, symbols, colors...
7. Initialization Method(s) - CreateObject(params[]) or (CreateFrog(params[]) and CreateCar(params[]))
8. Main Game Loop - while(true) { ... }
10. Check KeyPress Method - CheckKeyPress()
11. Move Cars Method - MoveCars()
12. Check For Collision Method - CheckForCollision()
13. Check If Game Is over MEthod - CheckIfGameIsOver()
14. Refresh Screen - Console.Clear();
15. RestartGame() if hit or PrintObject(frog) and PrintObject(cars)
15. Print Info Screen Method - PrintInfoScreen()
16. Control Game Speed - Thread.Sleep(350 - (int)speed);
 */

using System;
using System.Linq;
using System.IO;
using System.Activities;
using System.Activities.Statements;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Frogger;
using System.Text.RegularExpressions;
using System.Speech.Synthesis;


class FroggerGame
{
    struct Frog
    {
        public int x;
        public int y;
        public char bodySymbol;
        public ConsoleColor color;
    }

    static Random randomGenerator = new Random();
    static SpeechSynthesizer voice = new SpeechSynthesizer();

    static int gameScore = 0;
    static int gameWidth = 40;
    static int gameHeight = 25;
    static int gameSpeed = 300;
    static int gameLevel = 1;

    static Frog mrFrog = new Frog();
    static int mrFrogLives = 3;

    static List<Car> cars = new List<Car>();
    static ConsoleColor[] carColors = { ConsoleColor.Red, ConsoleColor.DarkMagenta, ConsoleColor.Blue, ConsoleColor.DarkCyan, ConsoleColor.White };
    static bool collisionFlag = false;

    static void Main()
    {
        voice.SelectVoiceByHints(VoiceGender.Female, VoiceAge.Adult);
        Task.Run(() =>
        {
            voice.SpeakAsync("Welcome to Frogger!");
            while (true)
            {
                PlaySound();
            }
        });
        SetGameDimensions();
        InitializeMrFrog();
        Menu();
    }
    static void PrintBorders()
    {
        //char[,] borders = new char[Console.WindowWidth - 1, Console.WindowHeight];
        for (int col = 0; col < Console.WindowWidth - 1; col++)
        {
            for (int row = 0; row < Console.WindowHeight; row++)
            {
                if (row < 2)
                {
                    Print(row, col, '#');
                }
                else if (row == gameHeight - 1 || row == gameHeight)
                {
                    Print(row, col, '#');
                }
                else
                {
                    if (col == 0)
                    {
                        Print(row, 0, '#');
                    }
                    else if (col == 1)
                    {
                        Print(row, 1, '#');
                    }
                    else if (col == gameWidth)
                    {
                        Print(row, gameWidth, '#');
                    }
                    else if (col == gameWidth - 1)
                    {
                        Print(row, gameWidth - 1, '#');
                    }
                }
            }
        }
    }
    static void Print(int row, int col, object data)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.SetCursorPosition(col, row);
        Console.Write(data);
    }
    public static void NewGame()
    {
        while (true)
        {
            Console.Clear();
            MoveEnemyCars();
            MoveAndDrawMrFrog();
            //PrintBorders();

            CreateEnemies();
            Lives();

            PrintingString(44, 4, "Lives left: " + mrFrogLives);
            PrintingString(44, 6, "Score: " + gameScore);
            PrintingString(44, 8, "Level: " + gameLevel);

            Thread.Sleep(gameSpeed);

            collisionFlag = false;
        }
    }
    static void Menu()
    {
        Console.Clear();
        PrintingString(10, 6, "New Game", ConsoleColor.Yellow);
        PrintingString(10, 8, "Scores");
        PrintingString(10, 10, "Game Rules");

        Console.CursorVisible = false;
        ConsoleKeyInfo key = Console.ReadKey();

        if (key.Key == ConsoleKey.Enter)
        {  //start game
            Console.Clear();
            NewGame();
        }
        else
        {
            if (key.Key == ConsoleKey.DownArrow)
            {
                PrintingString(10, 6, "New Game", ConsoleColor.White);
                PrintingString(10, 8, "Scores", ConsoleColor.Yellow);
                PrintingString(10, 10, "Game Rules", ConsoleColor.White);
                Console.SetCursorPosition(10, 8);
                key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    Console.Clear();
                    PrintScores();
                }
                else
                {
                    if (key.Key == ConsoleKey.DownArrow)
                    {
                        PrintingString(10, 6, "New Game", ConsoleColor.White);
                        PrintingString(10, 8, "Scores", ConsoleColor.White);
                        PrintingString(10, 10, "Game Rules", ConsoleColor.Yellow);

                        Console.WriteLine();
                        key = Console.ReadKey();
                        if (key.Key == ConsoleKey.Enter)
                        {
                            Console.Clear();
                            Rules();
                        }
                        else
                        {
                            Console.Clear();
                            Menu();
                        }
                    }
                    else
                    {
                        Console.Clear();
                        Menu();
                    }
                }
            }
            else
            {
                Console.Clear();
                Menu();
            }
        }
    }
    static void Rules()
    {
        voice.SpeakAsync(@"Hey, this is Frogger. You are the little smile at the bottom of the screen.
You should redirect the frog to his home.
But you should be prepared for all the cars crossing the road.
You can move in all directions.");
        Console.Clear();
        Console.WriteLine(@"
 
Hey, this is Fogger.
You are the little smile at the bottom of the screen.
You shoud redirect the frog(smile) to his home.
But you shoud be prepared for all
the cars crossing the road.
You can move in all directions.

        ");
        Console.WriteLine("\tPress N for new game\n\n\tPress M for Main Manu");
        ConsoleKeyInfo key = Console.ReadKey();
        if (key.Key == ConsoleKey.N)
        {
            NewGame();
        }
        else
        {
            if (key.Key == ConsoleKey.M)
            {
                Console.Clear();
                Menu();
            }
            else
            {
                Rules();
            }
        }
    }
    static void LevelUp()
    {
        if (mrFrog.y == 1)
        {
            gameSpeed -= 50;
            if (gameSpeed < 100)
            {
                gameSpeed = 100;
            }
            gameScore += 50;
            cars.Clear();
            InitializeMrFrog();
            gameLevel++;
        }
    }
    static void PrintingString(int x, int y, string str, ConsoleColor color = ConsoleColor.White)
    {
        Console.SetCursorPosition(x, y);
        Console.ForegroundColor = color;
        Console.Write(str);
    }
    static void InitializeMrFrog()
    {
        mrFrog.x = gameWidth / 2;
        mrFrog.y = gameHeight - 2;
        mrFrog.bodySymbol = (char)2;
        mrFrog.color = ConsoleColor.Green;
    }
    static void MoveAndDrawMrFrog()
    {
        //see if we have pressed a key
        if (Console.KeyAvailable)
        {
            ConsoleKeyInfo pressedKey = Console.ReadKey(true);//save the pressed key
            while (Console.KeyAvailable) Console.ReadKey(true);// we remove buffer keys from before

            if (pressedKey.Key == ConsoleKey.LeftArrow)//move Left >>>
            {
                if (mrFrog.x > 2)//so we dont get out of the bounderies of our gameScreen
                    mrFrog.x--;
            }
            if (pressedKey.Key == ConsoleKey.RightArrow)//move Right <<<
            {
                if (mrFrog.x < gameWidth - 2)
                    mrFrog.x++;
            }
            if (pressedKey.Key == ConsoleKey.UpArrow)//move Up ^^^
            {
                if (mrFrog.y != 0)
                {
                    mrFrog.y--;
                    gameScore++;
                }
            }
            if (pressedKey.Key == ConsoleKey.DownArrow)//move Down vvv
            {
                if (mrFrog.y < gameHeight - 2)
                {
                    mrFrog.y++;
                    gameScore--;
                }
            }
        }
        LevelUp();
        //draw mrFrog on the Screen
        PrintAtPosition(mrFrog.x, mrFrog.y, mrFrog.bodySymbol, mrFrog.color);
    }
    static void CreateEnemies()
    {
        Car newEnemyCar = new Car();
        newEnemyCar.y = randomGenerator.Next(2, gameHeight - 2);
        if (newEnemyCar.y % 2 == 1)
        {
            newEnemyCar.x = 2;
            newEnemyCar.direction = 1;
        }
        else //if (newEnemyCar.y % 2 == 0)
        {
            newEnemyCar.x = gameWidth - 5;
            newEnemyCar.direction = -1;
        }
        newEnemyCar.width = randomGenerator.Next(1, 5);
        newEnemyCar.color = carColors[randomGenerator.Next(0, carColors.Length)];
        newEnemyCar.bodySymbol = '=';
        cars.Add(newEnemyCar);

        foreach (Car currentCar in cars)
        {
            PrintAtPosition(currentCar.x,
                currentCar.y,
                currentCar.bodySymbol,
                currentCar.color,
                currentCar.width);
        }
    }
    static void Lives()
    {
        for (int i = 0; i < cars.Count; i++)
        {
            if ((mrFrog.x >= cars[i].x && mrFrog.x <= cars[i].x + cars[i].width - 1) && cars[i].y == mrFrog.y)
            {
                //set that we have been hit 
                collisionFlag = true;
                voice.SpeakAsync("Ouch!");
                mrFrogLives--;
                //remove 1 live from total
                if (mrFrogLives != 0)
                {
                    PrintAtPosition(mrFrog.x,
                    mrFrog.y,
                    'X',
                  ConsoleColor.Red);
                }
                else
                {
                    GameOver();
                }
            }
        }
        if (collisionFlag)
        {
            Console.Beep();
            InitializeMrFrog();
        }
    }
    static void MoveEnemyCars()
    {
        for (int i = 0; i < cars.Count; i++)
        {
            cars[i].MoveCar();

            if (cars[i].x >= gameWidth - 5 || cars[i].x <= 1)
            {
                cars.Remove(cars[i]);
                --i;
            }
        }
    }
    static void PrintAtPosition(int x, int y, char symbol, ConsoleColor color, int elementBodyWidth = 1)
    {
        Console.SetCursorPosition(x, y);
        Console.ForegroundColor = color;
        for (int len = 0; len < elementBodyWidth; len++)
        {
            Console.Write(symbol);
        }
    }
    static void SetGameDimensions()
    {
        Console.CursorVisible = false;
        Console.WindowWidth = gameWidth + 20;
        Console.BufferWidth = Console.WindowWidth;
        Console.WindowHeight = gameHeight + 1;
        Console.BufferHeight = gameHeight + 1;
    }
    static void PrintScores()
    {
        Console.WriteLine("\tTop 10 scores");

        TextReader scoreReader = new StreamReader("../../Scores.txt");
        string line = scoreReader.ReadLine();
        Dictionary<string, int> scores = new Dictionary<string, int>();

        while (line != null)
        {
            string[] currentScorer = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (!scores.ContainsKey(currentScorer[0]))
            {
                scores.Add(currentScorer[0], int.Parse(currentScorer[1]));
            }
            else
            {
                scores[currentScorer[0]] = int.Parse(currentScorer[1]);
            }
            line = scoreReader.ReadLine();
        }
        int scorePlace = 1;
        foreach (var item in scores.OrderByDescending(key => key.Value).Select(x => string.Format("{0} - {1}", x.Key, x.Value)))
        {
            Console.WriteLine(scorePlace + ". " + item);
            scorePlace++;
            if (scorePlace > 10)
            {
                break;
            }
        }
        scoreReader.Close();

        Console.WriteLine("\n\nPress N for new game\nPress M for Main Manu");
        ConsoleKeyInfo pressedKey = Console.ReadKey();
        if (pressedKey.Key == ConsoleKey.N)
        {
            NewGame();
        }
        else
        {
            if (pressedKey.Key == ConsoleKey.M)
            {
                Console.Clear();
                Menu();
            }
            else
            {
                Console.Clear();
                PrintScores();
            }
        }
    }
    static void GameOver()
    {
        voice.SpeakAsync("Game over!");
        Console.Clear();
        string fileName = @"..\..\frogGameOver.txt";
        Console.ForegroundColor = ConsoleColor.White;
        StreamReader streamReader = new StreamReader(fileName);

        using (streamReader)
        {
            string fileContents = streamReader.ReadToEnd();
            Console.WriteLine(fileContents);
        }

        Console.WriteLine("Score:" + gameScore);
        string playerName = string.Empty;
        while (true)
        {
            try
            {
                Console.WriteLine("What is your name?");
                playerName = Console.ReadLine();
                if (playerName.IndexOf(' ') >= 0)
                {
                    throw new ArgumentException("Name should not contain any spaces. Try again.");
                }
                else if (playerName.Length <= 0)
                {
                    throw new ArgumentException("Name should not be empty. Try again.");
                }
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        var scoreWriter = new StreamWriter("../../Scores.txt", true);
        scoreWriter.WriteLine(playerName + " " + gameScore);
        scoreWriter.Close();
        Console.WriteLine("\nPress Enter To Go Back To The Menu...");

        gameScore = 0;
        gameLevel = 1;
        InitializeMrFrog();
        cars.Clear();
        mrFrogLives = 3;
        gameSpeed = 300;

        ConsoleKeyInfo key = Console.ReadKey();

        if (key.Key == ConsoleKey.Enter)
        {
            Menu();
        }
    }
    static void PlaySound()
    {
        //Super Mario Theme Song
        Console.Beep(659, 125);
        Console.Beep(659, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(167);
        Console.Beep(523, 125);
        Console.Beep(659, 125);
        Thread.Sleep(125);
        Console.Beep(784, 125);
        Thread.Sleep(375);
        Console.Beep(392, 125);
        Thread.Sleep(375);
        Console.Beep(523, 125);
        Thread.Sleep(250);
        Console.Beep(392, 125);
        Thread.Sleep(250);
        Console.Beep(330, 125);
        Thread.Sleep(250);
        Console.Beep(440, 125);
        Thread.Sleep(125);
        Console.Beep(494, 125);
        Thread.Sleep(125);
        Console.Beep(466, 125);
        Thread.Sleep(42);
        Console.Beep(440, 125);
        Thread.Sleep(125);
        Console.Beep(392, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(125);
        Console.Beep(784, 125);
        Thread.Sleep(125);
        Console.Beep(880, 125);
        Thread.Sleep(125);
        Console.Beep(698, 125);
        Console.Beep(784, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(125);
        Console.Beep(523, 125);
        Thread.Sleep(125);
        Console.Beep(587, 125);
        Console.Beep(494, 125);
        Thread.Sleep(125);
        Console.Beep(523, 125);
        Thread.Sleep(250);
        Console.Beep(392, 125);
        Thread.Sleep(250);
        Console.Beep(330, 125);
        Thread.Sleep(250);
        Console.Beep(440, 125);
        Thread.Sleep(125);
        Console.Beep(494, 125);
        Thread.Sleep(125);
        Console.Beep(466, 125);
        Thread.Sleep(42);
        Console.Beep(440, 125);
        Thread.Sleep(125);
        Console.Beep(392, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(125);
        Console.Beep(784, 125);
        Thread.Sleep(125);
        Console.Beep(880, 125);
        Thread.Sleep(125);
        Console.Beep(698, 125);
        Console.Beep(784, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(125);
        Console.Beep(523, 125);
        Thread.Sleep(125);
        Console.Beep(587, 125);
        Console.Beep(494, 125);
        Thread.Sleep(375);
        Console.Beep(784, 125);
        Console.Beep(740, 125);
        Console.Beep(698, 125);
        Thread.Sleep(42);
        Console.Beep(622, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(167);
        Console.Beep(415, 125);
        Console.Beep(440, 125);
        Console.Beep(523, 125);
        Thread.Sleep(125);
        Console.Beep(440, 125);
        Console.Beep(523, 125);
        Console.Beep(587, 125);
        Thread.Sleep(250);
        Console.Beep(784, 125);
        Console.Beep(740, 125);
        Console.Beep(698, 125);
        Thread.Sleep(42);
        Console.Beep(622, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(167);
        Console.Beep(698, 125);
        Thread.Sleep(125);
        Console.Beep(698, 125);
        Console.Beep(698, 125);
        Thread.Sleep(625);
        Console.Beep(784, 125);
        Console.Beep(740, 125);
        Console.Beep(698, 125);
        Thread.Sleep(42);
        Console.Beep(622, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(167);
        Console.Beep(415, 125);
        Console.Beep(440, 125);
        Console.Beep(523, 125);
        Thread.Sleep(125);
        Console.Beep(440, 125);
        Console.Beep(523, 125);
        Console.Beep(587, 125);
        Thread.Sleep(250);
        Console.Beep(622, 125);
        Thread.Sleep(250);
        Console.Beep(587, 125);
        Thread.Sleep(250);
        Console.Beep(523, 125);
        Thread.Sleep(1125);
        Console.Beep(784, 125);
        Console.Beep(740, 125);
        Console.Beep(698, 125);
        Thread.Sleep(42);
        Console.Beep(622, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(167);
        Console.Beep(415, 125);
        Console.Beep(440, 125);
        Console.Beep(523, 125);
        Thread.Sleep(125);
        Console.Beep(440, 125);
        Console.Beep(523, 125);
        Console.Beep(587, 125);
        Thread.Sleep(250);
        Console.Beep(784, 125);
        Console.Beep(740, 125);
        Console.Beep(698, 125);
        Thread.Sleep(42);
        Console.Beep(622, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(167);
        Console.Beep(698, 125);
        Thread.Sleep(125);
        Console.Beep(698, 125);
        Console.Beep(698, 125);
        Thread.Sleep(625);
        Console.Beep(784, 125);
        Console.Beep(740, 125);
        Console.Beep(698, 125);
        Thread.Sleep(42);
        Console.Beep(622, 125);
        Thread.Sleep(125);
        Console.Beep(659, 125);
        Thread.Sleep(167);
        Console.Beep(415, 125);
        Console.Beep(440, 125);
        Console.Beep(523, 125);
        Thread.Sleep(125);
        Console.Beep(440, 125);
        Console.Beep(523, 125);
        Console.Beep(587, 125);
        Thread.Sleep(250);
        Console.Beep(622, 125);
        Thread.Sleep(250);
        Console.Beep(587, 125);
        Thread.Sleep(250);
        Console.Beep(523, 125);
        Thread.Sleep(625);
    }
}
