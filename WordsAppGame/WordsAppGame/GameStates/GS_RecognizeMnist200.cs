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
#endregion
namespace WordsAppGame.GameStates
{
    public class GS_RecognizeMnist200 : DrawableGameComponent
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

        readonly bool[,] _board = new bool[20, 20];
        Vector2 leapDownPosition;             //where the mouse was clicked down
        //initialize matrix for preview
        double[] objectVector;

        Recognition recog;
        string recognizeRes;

        public GS_RecognizeMnist200(Game1 game, SpriteBatch Batch, SpriteFont Font, Texture2D bgImage,
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
            _boardPosition = new Vector2(game.GraphicsDevice.Viewport.Width / 2 - (19 * 12) / 2, game.GraphicsDevice.Viewport.Height / 2 - (19 * 12) / 2);
            objectVector = new double[_board.Length];
        }

        public override void Initialize()
        {
            recog = new Recognition(@"D:\FIX\B3",true);
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            recog.setRecognize(objectVector,true);
            recognizeRes = recog.getRecognize();
#if USE_LEAP
            if (leapListener != null)
            {
                foreach (FingerPointStorage f in leapListener.fingerPoint)
                {
                    if (f.isActive)
                    {
                        xObj = _boardPosition.X + f.g_X * 19 * 12; //graphics.PreferredBackBufferWidth;
                        yObj = _boardPosition.Y + f.g_Y * 19 * 12; // graphics.PreferredBackBufferHeight;

                        cursorGame.Position.X = (int)xObj;
                        cursorGame.Position.Y = (int)yObj;

                        //find out which square the mouse is over
                        Vector3 tile = new Vector3(GetSquareFromCurrentMousePosition(), 0);
                        leapDownPosition = new Vector2((int)xObj, (int)yObj);
                        if (f.g_Tap < 0 && f.numHand != true)
                        {
                            f.g_Tap = 0;
                            GAME.changeState(StateMachine.ScreenState.GAME_MENU);
                        }
                        if (f.numHand == true)
                        {

                            for (int i = 0; i < 20; i++)
                                for (int j = 0; j < 20; j++)
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
                                    if (((int)tile.X >= 0 && ((int)tile.X < 19) && (int)tile.Y == 0))
                                    {
                                        _board[(int)tile.X, (int)tile.Y] = true;
                                        _board[(int)tile.X + 1, (int)tile.Y] = true;
                                        _board[(int)tile.X + 1, (int)tile.Y + 1] = true;
                                        _board[(int)tile.X, (int)tile.Y + 1] = true;
                                    }
                                    else if ((int)tile.X == 19 && (int)tile.Y == 0)
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
                                    else if (((int)tile.X >= 0 && (int)tile.X < 19) && (int)tile.Y == 19)
                                    {
                                        _board[(int)tile.X, (int)tile.Y] = true;
                                        _board[(int)tile.X + 1, (int)tile.Y] = true;
                                        _board[(int)tile.X, (int)tile.Y - 1] = true;
                                        _board[(int)tile.X + 1, (int)tile.Y - 1] = true;
                                    }
                                    else if ((int)tile.X == 19 && (int)tile.Y == 19)
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
                                    else if (((int)tile.Y >= 0 && ((int)tile.Y < 19) && (int)tile.X == 0))
                                    {
                                        _board[(int)tile.X, (int)tile.Y] = true;
                                        _board[(int)tile.X + 1, (int)tile.Y] = true;
                                        _board[(int)tile.X + 1, (int)tile.Y + 1] = true;
                                        _board[(int)tile.X, (int)tile.Y + 1] = true;
                                    }
                                    else if ((int)tile.Y == 19 && (int)tile.X == 0)
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
                                    else if (((int)tile.Y >= 0 && (int)tile.Y < 19) && (int)tile.X == 19)
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
                                    for (int i = 0; i < 20; i++)
                                    {
                                        for (int j = 0; j < 20; j++)
                                        {

                                            if (tile.X == j && tile.Y == i && j != 19)
                                            {
                                                if (count <= 379 && (count <= 19 || count == 20 || count == 40
                                                    || count == 60 || count == 80 || count == 100 || count == 120
                                                    || count == 140 || count == 160 || count == 180 || count == 200
                                                    || count == 220 || count == 240 || count == 260 || count == 280
                                                    || count == 300 || count == 320 || count == 340 || count == 360))
                                                {
                                                    objectVector[count] = 1.0d;
                                                    objectVector[count + 1] = 1.0d;
                                                    objectVector[count + 20] = 1.0d;
                                                    objectVector[count + 21] = 1.0d;
                                                    //Console.WriteLine("count {0}" + count);
                                                }
                                                else if (count > 379 && count <= 399)
                                                {
                                                    //barisan bawah
                                                    objectVector[count] = 1.0d;
                                                    objectVector[count + 1] = 1.0d;
                                                }
                                                else
                                                {
                                                    // 6 piece
                                                    objectVector[count - 19] = 1.0d;
                                                    objectVector[count - 20] = 1.0d;
                                                    objectVector[count - 21] = 1.0d;
                                                    objectVector[count] = 1.0d;
                                                    objectVector[count + 1] = 1.0d;
                                                    objectVector[count - 1] = 1.0d;
                                                    objectVector[count + 19] = 1.0d;
                                                    objectVector[count + 20] = 1.0d;
                                                    objectVector[count + 21] = 1.0d;
                                                }
                                                count = 1;
                                            }
                                            else if (tile.X == j && tile.Y == i && j == 19)
                                            {
                                                if (count <= 379)
                                                {
                                                    //untuk tepi pojok kanan
                                                    objectVector[count] = 1.0d;
                                                    objectVector[count - 1] = 1.0d;
                                                    objectVector[count + 19] = 1.0d;
                                                    objectVector[count + 20] = 1.0d;
                                                    // Console.WriteLine("count {0}" + count);
                                                }
                                                else if (count == 399)
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
                                recog.setRecognize(objectVector,true);
                                recognizeRes = recog.getRecognize();
                            }
                        }
                    }
                }
            }
#endif
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
                spriteBatch.DrawString(spriteFont, "This is a digit = '" + recognizeRes + "'", new Vector2(_boardPosition.X, _boardPosition.Y + (19 * 12) + 50), Color.Black);
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
