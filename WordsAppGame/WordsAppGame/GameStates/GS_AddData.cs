#region Config Preprosesor
#define USE_LEAP
#define USE_KEAYBOARD
#endregion
#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using WordsAppGame.Core;
using Leap;
using WordsAppGame.Control;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

#endregion
namespace WordsAppGame.GameStates
{
    public class GS_AddData : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        #region GS_GameMenu Property
        SpriteFont spriteFont;
        List<GameObject> objectMenu;
        GameObject cursorGame;
        SingleListener leapListener;
        Texture2D backImage;
        Game1 GAME;
        TimeSpan elapsedTime = TimeSpan.Zero;
        #endregion

        float xObj;                             //get x pos for leap
        float yObj;                             //get x pos for leap

        Vector2 _boardPosition;                 //where to position the board
        const int _tileSize = 12;               //how wide/tall the tiles are

        readonly bool[,] _board = new bool[28, 28];
        Vector2 leapDownPosition;             //where the mouse was clicked down
        //initialize matrix for preview
        double[] objectVector;
        
        //denote random number from 1 to 26 to denote A to Z
        Random rnd = new Random();
        int label;
        string labelAlphabet="";
        int countAdd = 0;

        public GS_AddData(Game1 game, SpriteBatch Batch, SpriteFont Font, Texture2D bgImage,
            List<GameObject> objMenu, GameObject cursor,
            SingleListener listener)
            : base(game)
        {
            #region GS_GameMenu Construct
            spriteFont = Font;
            spriteBatch = Batch;
            backImage = bgImage;
            objectMenu = objMenu;
            cursorGame = cursor;
            leapListener = listener;
            GAME = game;
            #endregion
            _boardPosition = new Vector2(game.GraphicsDevice.Viewport.Width / 2 - (27 * 12) / 2, game.GraphicsDevice.Viewport.Height / 2 - (27 * 12) / 2);
            objectVector = new double[_board.Length];
        }

        public override void Initialize()
        {
            label = rnd.Next(1, 26);
            labelAlphabet = "'" + IntToLetters(label) + "'";
            base.Initialize();
        }

        private void SaveData(double[] objectVector)
        {
            //generate random number for label
            var sourcePath = @"D:\dataHuruf.csv";
            var delimiter = ",";
            var firstLineContainsHeaders = true;
            var tempPath = @"D:\data.csv";
            var lineNumber = 0;

            var splitExpression = new Regex(@"(" + delimiter + @")(?=(?:[^""]|""[^""]*"")*$)");

            using (var writer = new StreamWriter(tempPath))
            using (var reader = new StreamReader(sourcePath))
            {
                string line = null;
                string[] headers = null;
                if (firstLineContainsHeaders)
                {
                    line = reader.ReadLine();
                    lineNumber++;

                    if (string.IsNullOrEmpty(line)) return; // file is empty;

                    headers = splitExpression.Split(line).Where(s => s != delimiter).ToArray();

                    writer.WriteLine(line); // write the original header to the temp file.
                }

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;

                    var columns = splitExpression.Split(line).Where(s => s != delimiter).ToArray();

                    // if there are no headers, do a simple sanity check to make sure you always have the same number of columns in a line
                    if (headers == null) headers = new string[columns.Length];

                    if (columns.Length != headers.Length) throw new InvalidOperationException(string.Format("Line {0} is missing one or more columns.", lineNumber));

                    // TODO: search and replace in columns
                    // example: replace 'v' in the first column with '\/': if (columns[0].Contains("v")) columns[0] = columns[0].Replace("v", @"\/");

                    writer.WriteLine(string.Join(delimiter, columns));
                }

            }

            File.Delete(sourcePath);
            File.Move(tempPath, sourcePath);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0},", label);

            for (int i = 0; i < 783; i++)
                sb.AppendFormat("{0},", objectVector[i]);

            sb.AppendFormat("{0}", objectVector[783]);
            // flush all rows once time.
            File.AppendAllText(sourcePath, sb.ToString(), Encoding.UTF8);

        }

        public override void Update(GameTime gameTime)
        {
            
#if USE_LEAP
            if (leapListener != null)
            {
                foreach (FingerPointStorage f in leapListener.fingerPoint)
                {
                    if (f.isActive)
                    {
                        xObj = _boardPosition.X + f.g_X * 27 * 12; //graphics.PreferredBackBufferWidth;
                        yObj = _boardPosition.Y + f.g_Y * 27 * 12; // graphics.PreferredBackBufferHeight;

                        cursorGame.Position.X = (int)xObj;
                        cursorGame.Position.Y = (int)yObj;

                        //find out which square the mouse is over
                        Vector3 tile = new Vector3(GetSquareFromCurrentMousePosition(), 0);
                        leapDownPosition = new Vector2((int)xObj, (int)yObj);
                        if (f.g_circle >= 3 && f.g_circle < 3.4 && countAdd<10)
                        {
                            SaveData(objectVector);
                            label = rnd.Next(1, 26);
                            labelAlphabet = "'" + IntToLetters(label) + "'";
                            labelAlphabet += "\nData Saved...Try Draw Again";
                            countAdd++;
                            if (countAdd == 10)
                            {
                                labelAlphabet += "\n          Limited for 10 letters";
                            }
                        }
                        

                        if (f.g_Tap < 0 && f.numHand != true)
                        {
                            f.g_Tap = 0;
                            GAME.changeState(StateMachine.ScreenState.GAME_MENU);
                            countAdd = 0;
                        }
                        if (f.numHand == true)
                        {

                            for (int i = 0; i < 28; i++)
                                for (int j = 0; j < 28; j++)
                                    _board[i, j] = false;

                            for (int i = 0; i < objectVector.Length; ++i)
                                objectVector[i] = 0.0d;
                        }
                        if (f.g_Z < 130)
                        {
                            if (f.g_Z != 0)
                            {

                                //if the mousebutton was released inside the board
                                if (IsMouseInsideBoard())
                                {
                                    //batas horisontal dari x dan y = 0 sampai x kurang dari panjang dan y = 0
                                    if (((int)tile.X >= 0 && ((int)tile.X < 27) && (int)tile.Y == 0))
                                    {
                                        _board[(int)tile.X, (int)tile.Y] = true;
                                        _board[(int)tile.X + 1, (int)tile.Y] = true;
                                        _board[(int)tile.X + 1, (int)tile.Y + 1] = true;
                                        _board[(int)tile.X, (int)tile.Y + 1] = true;
                                    }
                                    else if ((int)tile.X == 27 && (int)tile.Y == 0)
                                    {
                                        if (_board[(int)tile.X - 1, (int)tile.Y] != true && _board[(int)tile.X, (int)tile.Y + 1] != true)
                                        {
                                            _board[(int)tile.X, (int)tile.Y] = true;
                                            _board[(int)tile.X, (int)tile.Y + 1] = true;
                                            _board[(int)tile.X - 1, (int)tile.Y] = true;
                                            _board[(int)tile.X - 1, (int)tile.Y + 1] = true;
                                        }
                                    }
                                    //
                                    //batas horisontal dari x = 0 dan y = lebar sampai x kurang dari panjang dan y = lebar
                                    else if (((int)tile.X >= 0 && (int)tile.X < 27) && (int)tile.Y == 27)
                                    {
                                        _board[(int)tile.X, (int)tile.Y] = true;
                                        _board[(int)tile.X + 1, (int)tile.Y] = true;
                                        _board[(int)tile.X, (int)tile.Y - 1] = true;
                                        _board[(int)tile.X + 1, (int)tile.Y - 1] = true;
                                    }
                                    else if ((int)tile.X == 27 && (int)tile.Y == 27)
                                    {
                                        if (_board[(int)tile.X - 1, (int)tile.Y] != true && _board[(int)tile.X, (int)tile.Y - 1] != true)
                                        {
                                            _board[(int)tile.X, (int)tile.Y] = true;
                                            _board[(int)tile.X - 1, (int)tile.Y] = true;
                                            _board[(int)tile.X - 1, (int)tile.Y - 1] = true;
                                            _board[(int)tile.X, (int)tile.Y - 1] = true;
                                        }
                                    }
                                    //
                                    //batas horisontal dari x dan y = 0 sampai x kurang dari panjang dan y = 0
                                    else if (((int)tile.Y >= 0 && ((int)tile.Y < 27) && (int)tile.X == 0))
                                    {
                                        _board[(int)tile.X, (int)tile.Y] = true;
                                        _board[(int)tile.X + 1, (int)tile.Y] = true;
                                        _board[(int)tile.X + 1, (int)tile.Y + 1] = true;
                                        _board[(int)tile.X, (int)tile.Y + 1] = true;
                                    }
                                    else if ((int)tile.Y == 27 && (int)tile.X == 0)
                                    {
                                        if (_board[(int)tile.X, (int)tile.Y - 1] != true && _board[(int)tile.X + 1, (int)tile.Y] != true)
                                        {
                                            _board[(int)tile.X, (int)tile.Y] = true;
                                            _board[(int)tile.X + 1, (int)tile.Y] = true;
                                            _board[(int)tile.X, (int)tile.Y - 1] = true;
                                            _board[(int)tile.X + 1, (int)tile.Y - 1] = true;
                                        }
                                    }
                                    //batas vertical dari x = panjang dan y = 0 sampai y kurang dari lebar dan x = panjang
                                    else if (((int)tile.Y >= 0 && (int)tile.Y < 27) && (int)tile.X == 27)
                                    {
                                        _board[(int)tile.X, (int)tile.Y] = true;
                                        _board[(int)tile.X - 1, (int)tile.Y] = true;
                                        _board[(int)tile.X, (int)tile.Y + 1] = true;
                                        _board[(int)tile.X - 1, (int)tile.Y + 1] = true;
                                    }
                                    else
                                    {
                                        _board[(int)tile.X, (int)tile.Y] = true;
                                        _board[(int)tile.X + 1, (int)tile.Y] = true;
                                        _board[(int)tile.X, (int)tile.Y + 1] = true;
                                        _board[(int)tile.X - 1, (int)tile.Y] = true;
                                        _board[(int)tile.X, (int)tile.Y - 1] = true;
                                        _board[(int)tile.X + 1, (int)tile.Y + 1] = true;
                                        _board[(int)tile.X - 1, (int)tile.Y - 1] = true;
                                        _board[(int)tile.X + 1, (int)tile.Y - 1] = true;
                                        _board[(int)tile.X - 1, (int)tile.Y + 1] = true;
                                    }

                                    //Console.WriteLine("{0},{1}", (int)tile.X, (int)tile.Y);
                                    int count = 0;
                                    for (int i = 0; i < 28; i++)
                                    {
                                        for (int j = 0; j < 28; j++)
                                        {

                                            if (tile.X == j && tile.Y == i && j != 27)
                                            {
                                                if (count <= 755 && (count <= 27 || count == 28 || count == 56
                                                    || count == 84 || count == 112 || count == 140 || count == 168
                                                    || count == 196 || count == 224 || count == 252 || count == 280
                                                    || count == 308 || count == 336 || count == 364 || count == 392
                                                    || count == 420 || count == 448 || count == 476 || count == 504
                                                    || count == 532 || count == 560 || count == 588 || count == 616
                                                    || count == 644 || count == 672 || count == 700 || count == 728))
                                                {
                                                    objectVector[count] = 1.0d;
                                                    objectVector[count + 1] = 1.0d;
                                                    objectVector[count + 28] = 1.0d;
                                                    objectVector[count + 29] = 1.0d;
                                                    //Console.WriteLine("count {0}" + count);
                                                }
                                                else if (count > 755 && count <= 783)
                                                {
                                                    //barisan bawah
                                                    objectVector[count] = 1.0d;
                                                    objectVector[count + 1] = 1.0d;
                                                }
                                                else
                                                {
                                                    // 6 piece
                                                    objectVector[count - 27] = 1.0d;
                                                    objectVector[count - 28] = 1.0d;
                                                    objectVector[count - 29] = 1.0d;
                                                    objectVector[count] = 1.0d;
                                                    objectVector[count + 1] = 1.0d;
                                                    objectVector[count - 1] = 1.0d;
                                                    objectVector[count + 27] = 1.0d;
                                                    objectVector[count + 28] = 1.0d;
                                                    objectVector[count + 29] = 1.0d;
                                                }
                                                count = 1;
                                            }
                                            else if (tile.X == j && tile.Y == i && j == 27)
                                            {
                                                if (count <= 755)
                                                {
                                                    //untuk tepi pojok kanan
                                                    objectVector[count] = 1.0d;
                                                    objectVector[count - 1] = 1.0d;
                                                    objectVector[count + 27] = 1.0d;
                                                    objectVector[count + 28] = 1.0d;
                                                    // Console.WriteLine("count {0}" + count);
                                                }
                                                else if (count == 783)
                                                {
                                                    //ujung bawah
                                                    objectVector[count] = 1.0d;
                                                    objectVector[count - 1] = 1.0d;
                                                }
                                                else
                                                {
                                                    //lainnya
                                                    objectVector[count] = 1.0d;
                                                    objectVector[count - 1] = 1.0d;
                                                }
                                            }
                                            else
                                            {
                                                count++;
                                            }
                                        }
                                    }
                                }                               
                            }
                        }
                    }
                }
            }
#endif
        }

        public static string IntToLetters(int value)
        {
            string result = string.Empty;
            while (--value >= 0)
            {
                result = (char)('A' + value % 26) + result;
                value /= 26;
            }
            return result;
        }

        private void DrawText()
        {
            string title = "        Draw this label= " + labelAlphabet;
            spriteBatch.DrawString(spriteFont, title, new Vector2(_boardPosition.X - 30, _boardPosition.Y + (27 * 12) + 50), Color.Black);
           
        }

        public override void Draw(GameTime gameTime)
        {
            #region Draw GS_GameMenu
            spriteBatch.Begin();
            if (backImage != null)
                spriteBatch.Draw(backImage, new Vector2(spriteBatch.GraphicsDevice.Viewport.Width / 2 - backImage.Width / 2, spriteBatch.GraphicsDevice.Viewport.Height / 5 - backImage.Height / 2), Color.White);

            cursorGame.Draw(spriteBatch);

            DrawBoard();            //draw the board

            if (spriteFont != null)
                DrawText();
            spriteBatch.End();
            #endregion
        }

        // Draws the game board
        private void DrawBoard()
        {
            float opacity = .5f;                                      //how opaque/transparent to draw the square
            Color colorToUse = Color.White;                     //background color to use
            Rectangle squareToDrawPosition = new Rectangle();   //the square to draw (local variable to avoid creating a new variable per square)

            //for all columns
            for (int x = 0; x < _board.GetLength(0); x++)
            {
                //for all rows
                for (int y = 0; y < _board.GetLength(1); y++)
                {

                    //figure out where to draw the square
                    squareToDrawPosition = new Rectangle((int)(x * _tileSize + _boardPosition.X), (int)(y * _tileSize + _boardPosition.Y), _tileSize, _tileSize);

                    if ((x + y) % 2 == 0)
                    {
                        opacity = .33f;
                    }
                    else
                    {
                        //otherwise it is one tenth opaque
                        opacity = .1f;
                    }

                    if (IsMouseInsideBoard() && IsMouseOnTile(x, y))
                    {
                        colorToUse = Color.Red;
                        opacity = .5f;
                    }
                    else
                    {
                        colorToUse = Color.White;
                    }


                    //if the square has a tile - draw it
                    if (_board[x, y])
                    {
                        spriteBatch.Draw(objectMenu[0].getTexture(), squareToDrawPosition, Color.DarkRed);
                        colorToUse = Color.Yellow;
                    }

                    //draw the white square at the given position, offset by the x- and y-offset, in the opacity desired
                    spriteBatch.Draw(objectMenu[0].getTexture(), squareToDrawPosition, colorToUse * opacity);

                }

            }
        }


        bool IsMouseInsideBoard()
        {
            if (xObj >= _boardPosition.X && xObj <= _boardPosition.X + _board.GetLength(0) * _tileSize && yObj >= _boardPosition.Y && yObj <= _boardPosition.Y + _board.GetLength(1) * _tileSize)
            {
                return true;
            }
            else
            { return false; }
        }

        Vector2 GetSquareFromCurrentMousePosition()
        {
            //adjust for the boards offset (_boardPosition) and do an integerdivision
            return new Vector2((int)(xObj - _boardPosition.X) / _tileSize, (int)(yObj - _boardPosition.Y) / _tileSize);
        }

        // Checks to see whether a given coordinate is within the board
        private bool IsMouseOnTile(int x, int y)
        {
            //do an integerdivision (whole-number) of the coordinates relative to the board offset with the tilesize in mind
            return (int)(xObj - _boardPosition.X) / _tileSize == x && (int)(yObj - _boardPosition.Y) / _tileSize == y;
        }
    }
}
