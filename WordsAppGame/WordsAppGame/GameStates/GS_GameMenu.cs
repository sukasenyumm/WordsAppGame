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
    public class GS_GameMenu : DrawableGameComponent
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

        public GS_GameMenu(Game1 game, SpriteBatch Batch, SpriteFont Font, Texture2D bgImage, 
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
          
        }

        public override void Initialize()
        {
            
            base.Initialize();
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
                        xObj = f.g_X * StateMachine.Instance.resWidth;
                        yObj = f.g_Y * StateMachine.Instance.resHeight;
                        cursorGame.Position.X = (int)xObj;
                        cursorGame.Position.Y = (int)yObj;

                        if (cursorGame.BoundingBox.Intersects(objectMenu[0].BoundingBox))
                        {
                            objectMenu[0].color = Color.Chocolate;
                            cursorGame.color = Color.Violet;
                            elapsedTime += gameTime.ElapsedGameTime;
                            if (elapsedTime > TimeSpan.FromSeconds(1.2))
                            {
                                GAME.changeState(StateMachine.ScreenState.MENU_ADDDATA);
                                elapsedTime = TimeSpan.Zero;
                            }
                        }
                        else if (cursorGame.BoundingBox.Intersects(objectMenu[1].BoundingBox))
                        {
                            objectMenu[1].color = Color.Chocolate;
                            cursorGame.color = Color.Violet;
                            elapsedTime += gameTime.ElapsedGameTime;
                            if (elapsedTime > TimeSpan.FromSeconds(1.2))
                            {
                                GAME.changeState(StateMachine.ScreenState.EXIT_GAME);
                                elapsedTime = TimeSpan.Zero;
                            }
                        }
                        else if (cursorGame.BoundingBox.Intersects(objectMenu[2].BoundingBox))
                        {
                            objectMenu[2].color = Color.Chocolate;
                            cursorGame.color = Color.Violet;
                            elapsedTime += gameTime.ElapsedGameTime;
                            if (elapsedTime > TimeSpan.FromSeconds(1.2))
                            {
                                GAME.changeState(StateMachine.ScreenState.MENU_RM);
                                elapsedTime = TimeSpan.Zero;
                            }
                        }
                        else if (cursorGame.BoundingBox.Intersects(objectMenu[3].BoundingBox))
                        {
                            objectMenu[3].color = Color.Chocolate;
                            cursorGame.color = Color.Violet;
                            elapsedTime += gameTime.ElapsedGameTime;
                            if (elapsedTime > TimeSpan.FromSeconds(1.2))
                            {
                                GAME.changeState(StateMachine.ScreenState.MENU_RH);
                                elapsedTime = TimeSpan.Zero;
                            }
                        }
                        else if (cursorGame.BoundingBox.Intersects(objectMenu[4].BoundingBox))
                        {
                            objectMenu[4].color = Color.Chocolate;
                            cursorGame.color = Color.Violet;
                            elapsedTime += gameTime.ElapsedGameTime;
                            if (elapsedTime > TimeSpan.FromSeconds(1.2))
                            {
                                GAME.changeState(StateMachine.ScreenState.MENU_RM200);
                                elapsedTime = TimeSpan.Zero;
                            }
                        }
                        else
                        {
                            objectMenu[0].color = Color.White;
                            objectMenu[1].color = Color.White;
                            objectMenu[2].color = Color.White;
                            objectMenu[3].color = Color.White;
                            objectMenu[4].color = Color.White;
                            cursorGame.color = Color.White;
                            elapsedTime = TimeSpan.Zero;
                        }
                    }
                }
            }
#endif
        }

        public override void Draw(GameTime gameTime)
        {
            #region Draw GS_GameMenu
            string strTimeWait = string.Format("{0}%", (Math.Floor(elapsedTime.TotalSeconds * 10 / 12 * 100)).ToString());
            spriteBatch.Begin();
            if (backImage != null)
                spriteBatch.Draw(backImage, new Vector2(spriteBatch.GraphicsDevice.Viewport.Width / 2 - backImage.Width / 2, spriteBatch.GraphicsDevice.Viewport.Height / 5 - backImage.Height / 2), Color.White);
            for (int i = 0; i < objectMenu.Count; i++)
            {
                objectMenu[i].Draw(spriteBatch);
            }
            cursorGame.Draw(spriteBatch);
            if (spriteFont != null)
                spriteBatch.DrawString(spriteFont, strTimeWait, new Vector2(cursorGame.Position.X + cursorGame.BoundingBox.Width / 2, cursorGame.Position.Y + cursorGame.BoundingBox.Height / 2), Color.Black);

            spriteBatch.End();
            #endregion
        }
    }
}
