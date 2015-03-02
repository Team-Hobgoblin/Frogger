/*
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

    static int gameScore = 0;
    static int gameCurrentLevel = 0;
    static int gameWidth = 40;
    static int gameHeight = 25;
    static int gameSpeed = 300;
    static int gameLevel = 1;

    static Frog mrFrog = new Frog();
    static int mrFrogLives = 3; // because F(frog) is the 6th letter in Engl

    static List<Car> cars = new List<Car>();
    static bool collisionFlag = false;

    static void Main()
    {
        SetGameDimensions();
        //PrintBorders();
        //initialize mrFrog 
        initializeMrFrog();

        Menu();
    }
   // static void PrintBorders()
   // {
   //     for (int col = 0; col < gameWidth; col++)
   //     {
   //         Print(0, col, '-');
   //         Print(gameHeight - 1, col, '_');
   //     }
   //     for (int row = 0; row < gameHeight; row++)
   //     {
   //         Print(row, 0, '|');
   //         Print(row, 25, '|');
   //     }
   // }

    public static void NewGame()
    {
        Task.Run(() =>
        {
            while (true)
            {
                PlaySound();
            }
        });
        while (true)
        {
            //Move enemy Cars 
            MoveEnemyCars();

            // clear the console - so we dont see the PAST !
            Console.Clear();
            // create mr.Frog 
            MoveAndDrawMrFrog();

            // create enemies for mr.Frog - cars/obstacles
            //TODO : make so that when we start we have verywhere cars, not to wait them to be created
            //TODO : some cars move from left to right | some from right to left
            //TODO : make that the 1st simbol shows thedirection in which they are moving
            // '==>' or '<====' or '>' or '<' or '<===' hope you get it
            CreateEnemies();

            //display Colision
            //PS: there is a problem >> we detect colision with the cars only if we hit the the first index of the car
            //this means that if we have a 4 elementBody car(====) and hit its last element (the most right) 
            // we will not have a collision !!!! only if we hit its first ->>> the element with index 0 (the most left)
            //this can be solved by List.Contains
            //
            Lives();

            PrintingString(41, 4, "Lives left: " + mrFrogLives);
            // slow down the program - so we can see what is happening 
            PrintingString(41, 6, "Score: " + gameScore);

            PrintingString(41, 8, "Level: " + gameLevel);

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
                    Scores();
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
                        else { Main(); }
                    }
                    else { Main(); }
                }
            }
            else  {   Main(); }
        }
    }

    static void Rules()
    {
        Console.Clear();
        Console.WriteLine(@"
 
Hey, this is Fogger.
You are the little smile at the bottom of the screen.
You shoud redirect the frog(smile) to his home.
But you shoud be prepared for all the cars crossing the road.
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

        if (mrFrog.y == 3)
        {
            gameSpeed -= 50;
            if (gameSpeed < 100)
            {
                gameSpeed = 100;
            }
            cars.Clear();
            initializeMrFrog();
            gameLevel++;

        }

    }

    static void PrintingString(int x, int y, string str, ConsoleColor color = ConsoleColor.White)
    {
        Console.SetCursorPosition(x, y);
        Console.ForegroundColor = color;
        Console.Write(str);
    }

    static void initializeMrFrog()
    {
        mrFrog.x = gameWidth / 2;
        mrFrog.y = gameHeight - 1;
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
                if (mrFrog.x != 0)//so we dont get out of the bounderies of our gameScreen
                    mrFrog.x--;
            }
            if (pressedKey.Key == ConsoleKey.RightArrow)//move Right <<<
            {
                if (mrFrog.x < gameWidth - 1)
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
                if (mrFrog.y < gameHeight - 1)
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


        if (newEnemyCar.y % 2 == 0)
        {
            newEnemyCar.direction = randomGenerator.Next(0, 2);
        }
        else if (newEnemyCar.y % 2 == 1)
        {
            newEnemyCar.direction = randomGenerator.Next(2, 0);
        }
        //Sidewalks are lanes that there are no cars
        //lane 0 is Top Sidewalk | lane gameHeight - 1 is Bot Sidewalk | everything else is the road
        newEnemyCar.y = randomGenerator.Next(3, gameHeight - 4);
        if (newEnemyCar.y % 2 == 1)
        {
            newEnemyCar.x = 5;
            newEnemyCar.direction = 1;
        }
        else //if (newEnemyCar.y % 2 == 0)
        {
            newEnemyCar.x = gameWidth - 6;
            newEnemyCar.direction = -1;
        }
        newEnemyCar.width = randomGenerator.Next(1, 5);
        newEnemyCar.color = ConsoleColor.Yellow;
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
            if ((mrFrog.x >= cars[i].x && mrFrog.x <= cars[i].x + cars[i].width) && cars[i].y == mrFrog.y)
            {
                //set that we have been hit 
                collisionFlag = true;
                //remove 1 live from total
                if (mrFrogLives != 0)
                {
                    mrFrogLives--;
                    //hit = true;
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
            if (collisionFlag)
            {

                Console.Beep();
                initializeMrFrog();
                //Printing(dwarf.posX, dwarf.posY, "X", ConsoleColor.Red);
            }
        }
    }

    static void MoveEnemyCars()
    {
        for (int i = 0; i < cars.Count; i++)
        {
            cars[i].MoveCar();


            //maybe Colision detection has to be solved something like the lines below???
            //if (currentCar.y == mrFrog.y) cars.Contains(mrFrog.x);
            //{ 
            //    collisionFlag = true;
            //}

            //check for Colision 

           
            if (cars[i].x >= gameWidth - 5 || cars[i].x <= 5)
            {
                cars.Remove(cars[i]);
                --i;
            }
        }
        //Rly i have no clue why I have to create a new list
        //then create a oldCar and currentCar and set currentCar variables to oldCar
        //and then add it to the new LIST 
        //and how by doing this I solve the problem  .... 
        //at http://telerikacademy.com/Courses/LectureResources/Video/5433/Just-Cars-%D0%9D%D0%B8%D0%BA%D0%B8-21-%D0%BD%D0%BE%D0%B5%D0%BC%D0%B2%D1%80%D0%B8-2012
        //min 60:41 they explain and do it like just like this
        //I guess here C# acts like a magic wand - pure magic
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

        Console.WindowWidth = gameWidth + 15;
        Console.WindowHeight = gameHeight + 2;
        Console.BufferHeight = Console.WindowHeight;
        Console.BufferWidth = Console.WindowWidth;
    }

    static void PrintStringArray(string[] newString)
    {
        foreach (var text in newString)
        {
            int whiteSpaces = (text.Length) / 2;
            Console.WriteLine(text.PadLeft(whiteSpaces), 'a');

        }

    }

    static void Scores()
    {
        Console.WriteLine("\tTop 10 scores");
       // File.ReadLines("highscore.txt").Select(line => int.Parse(line)).OrderByDescending(score => score).Take(10);
    //    TextWriter textWriter = new StreamWriter();
    //    //here we have to figure a way to add an actual score
    //    //....
    //    textWriter.Close();

    //    TextReader textReader = new StreamReader();
        
        //for (int i = 0; i < length; i++)
        //    {
			 
        //    }
    //    textReader.ReadLine();
    //    Console.WriteLine(textReader);
    }

     static void GameOver()
     {
        string fileName = @"..\..\frogGameOver.txt";
        StreamReader streamReader = new StreamReader(fileName);

        using (streamReader)
        {
            string fileContents = streamReader.ReadToEnd();
            Console.WriteLine(fileContents);
        }
        
        Console.WriteLine("\t\tScore:" + gameScore);
        Console.WriteLine("\n\tPress any key");
        Console.ReadKey(true);
        Menu();
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
