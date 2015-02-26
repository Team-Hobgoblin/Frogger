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
using System.Activities;
using System.Activities.Statements;
using System.Threading;
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

    static Frog mrFrog = new Frog();
    static int mrFrogLives = 3; // because F(frog) is the 6th letter in Engl

    static List<Car> cars = new List<Car>();
    static bool collisionFlag = false;



    static void Main()
    {
        SetGameDimensions();

        //initialize mrFrog 
        initializeMrFrog();

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
            if (collisionFlag)
            {
                //display X /colision/
                PrintAtPosition(mrFrog.x,
                    mrFrog.y,
                    'X',
                    ConsoleColor.Red);
                GameOver();
            }
            PrintingString(41, 4, "Lives: " + mrFrogLives);
            // slow down the program - so we can see what is happening 
            PrintingString(41, 6, "Score: " + gameScore);

            Thread.Sleep(gameSpeed);


            collisionFlag = false;

        }
    }

    static void ScoreUpgrade()
    {

        if (true)
        {
            
        }

    }
    static void LevelUp()
    {
        if (mrFrog.y == 0)
        {
            
            gameSpeed -= 50;
            if (gameSpeed < 100)
            {
                gameSpeed = 100;
            }
            cars.Clear();
            initializeMrFrog();
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


        //Sidewalks are lanes that there are no cars
        //lane 0 is Top Sidewalk | lane gameHeight - 1 is Bot Sidewalk | everything else is the road
        newEnemyCar.y = randomGenerator.Next(1, gameHeight - 2);
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

            if ((mrFrog.x >= cars[i].x && mrFrog.x <= cars[i].x + cars[i].width) && cars[i].y == mrFrog.y)
            {
                //set that we have been hit 
                collisionFlag = true;
                if (gameScore % 40 == 0)
                {
                    mrFrogLives++;
                }
                //remove 1 live from total
                if (mrFrogLives != 0)
                {
                    mrFrogLives--;
                    //hit = true;
                }
                else
                {
                    //GOTO : Activate gameOver !
                }
            }
            if (collisionFlag)
            {
                Console.Beep();
                initializeMrFrog();
                //Printing(dwarf.posX, dwarf.posY, "X", ConsoleColor.Red);
            }
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
        Console.WindowHeight = gameHeight;
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

    public static void GameOver()
    {
        string[] gameOver = new string[]
            {
            @"   _________    _____   ____     _______  __ ___________ ",
            @"  / ___\__  \  /     \_/ __ \   /  _ \  \/ // __ \_  __ \",
            @" / /_/  > __ \|  Y Y  \  ___/  (  <_> )   /\  ___/|  | \/",
            @" \___  (____  /__|_|  /\___  >  \____/ \_/  \___  >__|   ",
            @"/_____/     \/      \/     \/                   \/       "
            };

        Console.Clear();
        PrintStringArray(gameOver);
        Console.WriteLine("\n\n\n");
        Console.ReadKey(true);


    }


}
