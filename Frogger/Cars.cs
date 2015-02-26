using System;


namespace Frogger
{
    class Car
    {
        public int x;
        public int y;
        public int width;
        public int direction;
        public char bodySymbol;
        public ConsoleColor color;


        public void MoveCar()
        {
            this.x += this.direction;
        }
    }
}
