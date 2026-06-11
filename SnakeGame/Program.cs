using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SnakeGame
{
    enum SnakeMoves
    {
        UP, DOWN, RIGHT, LEFT
    }

    class Test
    {
        public static void run()
        {
            Console.WriteLine(new int[] { 1, 2, 3 }.SequenceEqual(new int[] { 1, 2, 3 }));
        }
    }

    class Program
    {
        private static int speed = 10;//1-100
        private static int refreshRate = 10;
        static void Main(string[] args)
        {
            Snake snake = new Snake('#');
            Treats treats = new Treats('*','@','?');
            GameUi ui = new GameUi(snake,treats);
            Mechanics mech = new Mechanics(snake, treats, ui);
            mech.newApple();

            int count = 0;
            bool test = false;
            //Game loop
            if(!test){
                while (true)
                {
                    //ConsoleKeyInfo key = Console.ReadKey(true);
                    bool keyTyped = true;
                    //else { keyTyped = false; }
                    if (Console.KeyAvailable)
                    {
                        ConsoleKeyInfo key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.UpArrow && snake.HDir != SnakeMoves.DOWN) { snake.HDir = SnakeMoves.UP; }
                        else if (key.Key == ConsoleKey.DownArrow && snake.HDir != SnakeMoves.UP) { snake.HDir = SnakeMoves.DOWN; }
                        else if (key.Key == ConsoleKey.LeftArrow && snake.HDir != SnakeMoves.RIGHT) { snake.HDir = SnakeMoves.LEFT; }
                        else if (key.Key == ConsoleKey.RightArrow && snake.HDir != SnakeMoves.LEFT) { snake.HDir = SnakeMoves.RIGHT; }
                    }
                    if (count == speed)
                    {
                        count = 0;
                        snake.move();
                    }
                    if (mech.collided())
                    {
                        ui.gameOver();
                    }
                    else
                    {
                        ui.printUI();
                    }
                    if(mech.eatenApple())
                    {
                        mech.eatApple();
                        mech.newApple();
                    }
                    count++;
                    Thread.Sleep(refreshRate);
                    ui.clearConsole();
                }
            }
            else
            {
                Test.run();
            }
        }
    }
    class Snake
    {
        private char bodyChar;

        private LinkedList<int[]> body;

        private int bodyLength = 3;
        /// <summary>
        /// Current direction of the snake
        /// </summary>
        private SnakeMoves hDir = SnakeMoves.DOWN;
        public Snake(char bodyChar)
        {
            this.bodyChar = bodyChar;
            body = new LinkedList<int[]>();
            body.AddLast(new int[]{ 0, 2 });
            body.AddLast(new int[] { 0, 1 });
            body.AddLast(new int[] { 0, 0 });
            
        }
        public char BodyChar { set { this.bodyChar = value; } get { return bodyChar; } }
        public SnakeMoves HDir { set { this.hDir = value; } get => hDir; }


        public int[] getHPoint()
        {
            return body.First.Value;
        }

        public int BodyLength { set { 
                switch (getTailDirrection())
                {
                    case SnakeMoves.DOWN:
                        body.AddLast(new int[] { body.Last.Value[0], body.Last.Value[1] + (value-this.bodyLength) });
                        break;
                    case SnakeMoves.UP:
                        body.AddLast(new int[] { body.Last.Value[0], body.Last.Value[1] - (value - this.bodyLength) });
                        break;
                    case SnakeMoves.RIGHT:
                        body.AddLast(new int[] { body.Last.Value[0] + (value - this.bodyLength), body.Last.Value[1] });
                        break;
                    case SnakeMoves.LEFT:
                        body.AddLast(new int[] { body.Last.Value[0] - (value - this.bodyLength), body.Last.Value[1] });
                        break;
                }
                this.bodyLength = value;
            } get { return bodyLength; } }


        public void move()
        {
            switch (HDir)
            {
                case SnakeMoves.UP:
                    body.AddFirst(new int[] { body.First.Value[0], body.First.Value[1] - 1 });
                    break;
                case SnakeMoves.DOWN:
                    body.AddFirst(new int[] { body.First.Value[0], body.First.Value[1] + 1 });
                    break;
                case SnakeMoves.RIGHT:
                    body.AddFirst(new int[] { body.First.Value[0] + 1, body.First.Value[1] });
                    break;
                case SnakeMoves.LEFT:
                    body.AddFirst(new int[] { body.First.Value[0] - 1, body.First.Value[1] });
                    break;
            }
            body.RemoveLast();
        }

        public ArrayList getBodyPartsInRow(int row)
        {
            ArrayList indexes = new ArrayList();
            LinkedListNode<int[]> bp = body.First;
            while (true)
            {
                if (bp.Value[1] == row)
                {
                    indexes.Add(bp.Value[0]);
                }
                bp = bp.Next;
                if (bp == null)
                {
                    break;
                }
            }
            return indexes.Count > 0 ? indexes : null;
        }

        public void grow(int parts)
        {
            BodyLength+=parts;
        }

        private SnakeMoves getTailDirrection()
        {
            int x_ = body.Last.Value[0] - body.Last.Previous.Value[0];
            int y_ = body.Last.Value[1] - body.Last.Previous.Value[1];
            if (x_ == 0 && y_ == 1) { return SnakeMoves.DOWN; }
            else if (x_ == 0 && y_ == -1) { return SnakeMoves.UP; }
            else if (x_ == 1 && y_ == 0) { return SnakeMoves.RIGHT; }
            else { return SnakeMoves.LEFT; }
        }

        public bool isBodyCoord(int[] coord)
        {
            foreach (int[] c in body)
            {
                if (c.SequenceEqual(coord))
                {
                    return true;
                }
            }
            return false;
        }

    }

    class Treats
    {
        private int[] appleCoord;
        private char apple;
        private char bigApple;
        private char bomb;


        public Treats(char apple,char bigApple, char bomb)
        {
            this.apple = apple;
            this.bigApple = bigApple;
            this.bomb = bomb;
        }

        public char Apple
        {
            get { return apple; }
            set { apple = value; }
        }

        public char BigApple
        {
            get { return bigApple; }
            set { bigApple = value; }
        }

        public char Bomb
        {
            get { return bomb; }
            set { bomb = value; }
        }

        public int[] AppleCoord
        {
            get { return appleCoord; }
            set { appleCoord = value; }
        }

        public int[] getAppleCoords(Snake snake,int width,int height)
        {
            Random random = new Random();
            int[] c = null;
            do
            {
                c = new int[] { random.Next(width), random.Next(height) };
            }while (snake.isBodyCoord(c));
            return c;
        }

    }

    class Mechanics
    {
        Snake snake;
        Treats treats;
        GameUi ui;

        public Mechanics(Snake snake, Treats treats, GameUi ui)
        {
            this.ui = ui;
            this.snake = snake;
            this.treats = treats;
        }

        public bool collided()
        {
            return (0 < snake.getHPoint()[1] && snake.getHPoint()[1] < ui.Width && 0 < snake.getHPoint()[0] && snake.getHPoint()[0] < ui.Height)
                && !snake.isBodyCoord(snake.getHPoint());
        }

        public bool eatenApple()
        {
            return snake.getHPoint().SequenceEqual(treats.AppleCoord);
        }

        public void eatApple()
        {
            snake.grow(1);
            newApple();
        }

        public void newApple()
        {
            treats.AppleCoord = treats.getAppleCoords(snake, ui.Width, ui.Height);
        }
    }

    class GameUi
    {
        private int border = 100;
        private int width = 40;
        private int height = 20;
        private Treats treats;
        private Snake snake;
        public GameUi(Snake snake,Treats treats)
        {
            this.snake = snake;
            this.treats = treats;
        }
        public void printUI()
        {
            Console.WriteLine($"{new string(' ', width / 2 - 5)}SNAKE GAME");
            Console.WriteLine(new string('=', width));
            for (int i = 0; i < height; i++)
            {
                char[] row = $"{new string(' ', width - 1)}".ToCharArray();
                ArrayList bp = snake.getBodyPartsInRow(i);
                if (bp != null)
                {
                    foreach (var p in bp)
                    {
                        row[(int)p] = snake.BodyChar;
                    }
                }
                if (treats.AppleCoord[1] == i)
                {
                    row[treats.AppleCoord[0]] = treats.Apple;
                }
                Console.WriteLine($"|{new string(row)}|");   
            }
            Console.WriteLine(new string('=', width));
        }

        public void gameOver()
        {
            Console.WriteLine($"{new string(' ', width / 2 - 5)}SNAKE GAME\n\n\n\n");
            Console.WriteLine($"{new string(' ', width / 2 - 5)}Game Over!!!");
        }

        public void clearConsole()
        {
            Console.Clear();
        }

        public int Width { get { return width; } }
        public int Height { get { return height; } }
        public int Border { get { return border; } }
    }
}