using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SnakeGame
{
    enum SnakeMoves
    {
        UP, DOWN, RIGHT, LEFT
    }

    class Program
    {
        private static int speed = 10;//1-100
        private static int refreshRate = 10;
        private static readonly Random random = new Random();
        static void Main(string[] args)
        {
            Snake snake = new Snake('#');
            GameUi ui = new GameUi(snake);
            int count = 0;
            //Game loop
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
                int appleX = random.Next(0, ui.Width);
                int appleY = random.Next(0, ui.Height);
                if (count == speed)
                {
                    count = 0;
                    snake.move();
                }
                count++;
                ui.printUI();
                Thread.Sleep(refreshRate);
                ui.clearConsole();
            }
        }
    }
    class Snake
    {
        private char bodyChar;

        private LinkedList<int[]> body;

        private int bodyLength = 3;
        /// <summary>
        /// Head point of the snake
        /// </summary>
        private int[] hPoint = { 0, 2 };
        /// <summary>
        /// Current direction of the snake
        /// </summary>
        private SnakeMoves hDir = SnakeMoves.DOWN;
        public Snake(char bodyChar)
        {
            this.bodyChar = bodyChar;
            body = new LinkedList<int[]>();
            body.AddLast(hPoint);
            body.AddLast(new int[] { 0, 1 });
            body.AddLast(new int[] { 0, 0 });
            
        }
        public char BodyChar { set { this.bodyChar = value; } get { return bodyChar; } }

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

        public SnakeMoves HDir { set { this.hDir = value; } get => hDir; }

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

        public int getHX()
        {
            return this.hPoint[0];
        }

        public int getHY()
        {
            return this.hPoint[1];
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


    }
    class GameUi
    {
        private int border = 100;
        private int width = 40;
        private int height = 20;
        private Snake snake;
        public GameUi(Snake snake)
        {
            this.snake = snake;
        }
        public void printUI()
        {
            Console.WriteLine($"{new string(' ', width / 2 - 5)}SNAKE GAME");
            Console.WriteLine(new string('=', width));
            for (int i = 0; i < height; i++)
            {
                ArrayList bp = snake.getBodyPartsInRow(i);
                if (bp != null)
                {
                    char[] row = $"{new string(' ', width - 1)}".ToCharArray();
                    foreach (var p in bp)
                    {
                        row[(int)p] = snake.BodyChar;
                    }
                    Console.WriteLine($"|{new string(row)}|");
                }
                else
                    Console.WriteLine($"|{new string(' ', width - 1)}|");
            }
            Console.WriteLine(new string('=', width));
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