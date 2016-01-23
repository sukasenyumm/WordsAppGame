using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using WordsAppGame.Core;
using WordsAppGame.GameStates;

using Leap;
using WordsAppGame.Control;

namespace WordsAppGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameObject menuRM;
        GameObject menuRM200;
        GameObject menuRH;
        GameObject menuADD;
        GameObject menuExit;
        GameObject menuBox;

        GS_GameMenu gs_StartScreen;
        GS_RecognizeHuruf gs_RecognizeHuruf;
        GS_RecognizeMnist gs_RecognizeMnist;
        GS_RecognizeMnist200 gs_RecognizeMnist200;
        GS_AddData gs_AddData;

        Texture2D textureObj,textureBox, textureAngka, textureAngka200, textureAdd, textureHuruf, textureExit;
        SpriteFont textureFont;
        GameObject finger;

        List<GameObject> gameMenu = new List<GameObject>();
        List<GameObject> recognizeMenu = new List<GameObject>();

        Controller leapController;
        SingleListener listener;
        StateMachine currentState;
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferMultiSampling = true;
            //graphics.IsFullScreen = true;
            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1024;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here    
            currentState = StateMachine.Instance;
            currentState.state = StateMachine.ScreenState.GAME_MENU;
            StateMachine.Instance.resWidth = graphics.GraphicsDevice.Viewport.Width;
            StateMachine.Instance.resHeight = graphics.GraphicsDevice.Viewport.Height;
            leapController = new Controller();
            listener = new SingleListener();
            leapController.EnableGesture(Gesture.GestureType.TYPE_CIRCLE);
            leapController.EnableGesture(Gesture.GestureType.TYPESCREENTAP);
            leapController.AddListener(listener);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.    
            spriteBatch = new SpriteBatch(GraphicsDevice);
            textureObj = Content.Load<Texture2D>("pointer");
            textureAngka = Content.Load<Texture2D>("Btn_angka");
            textureAngka200 = Content.Load<Texture2D>("Btn_angka200");
            textureAdd = Content.Load<Texture2D>("Btn_add");
            textureHuruf = Content.Load<Texture2D>("Btn_huruf");
            textureExit = Content.Load<Texture2D>("Btn_exit");
            textureBox = Content.Load<Texture2D>("circle");

            textureFont = Content.Load<SpriteFont>("font");

            finger = new GameObject(textureObj, Vector2.Zero);
            finger.color = Color.Snow;
            menuADD = new GameObject(textureAdd, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - textureAdd.Width / 2, graphics.GraphicsDevice.Viewport.Height / 2 - textureAdd.Height / 2 - 100));
            menuADD.color = Color.White;
            menuRH = new GameObject(textureHuruf, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - textureHuruf.Width / 2, menuADD.Position.Y + menuADD.BoundingBox.Height - 50 + textureHuruf.Height / 2));
            menuRH.color = Color.White;
            menuExit = new GameObject(textureExit, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - textureExit.Width / 2, menuRH.Position.Y + menuRH.BoundingBox.Height + textureExit.Height / 2));
            menuExit.color = Color.White;
            menuRM = new GameObject(textureAngka, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 - textureAngka.Width / 2 - textureAngka.Width - 50, menuADD.Position.Y + menuADD.BoundingBox.Height - 50 + textureAngka.Height / 2));
            menuRM.color = Color.White;
            menuRM200 = new GameObject(textureAngka200, new Vector2(graphics.GraphicsDevice.Viewport.Width / 2 + textureAngka200.Width / 2 + 50, menuADD.Position.Y + menuADD.BoundingBox.Height - 50 + textureAngka200.Height / 2));
            menuRM200.color = Color.White;

            menuBox = new GameObject(textureBox, new Vector2(0,0));
            menuExit.color = Color.White;

            gameMenu.Add(menuADD);
            gameMenu.Add(menuExit);
            gameMenu.Add(menuRM); 
            gameMenu.Add(menuRH);
            gameMenu.Add(menuRM200);

            recognizeMenu.Add(menuBox);

            gs_StartScreen = new GS_GameMenu(this, spriteBatch, textureFont, Content.Load<Texture2D>("title"), gameMenu, finger, listener);
            gs_RecognizeHuruf = new GS_RecognizeHuruf(this, spriteBatch, textureFont, Content.Load<Texture2D>("titlehuruf"), recognizeMenu, finger, listener);
            gs_RecognizeMnist = new GS_RecognizeMnist(this, spriteBatch, textureFont, Content.Load<Texture2D>("titleangka"), recognizeMenu, finger, listener);
            gs_RecognizeMnist200 = new GS_RecognizeMnist200(this, spriteBatch, textureFont, Content.Load<Texture2D>("titleangka200"), recognizeMenu, finger, listener);
            gs_AddData = new GS_AddData(this, spriteBatch, textureFont, Content.Load<Texture2D>("titleadd"), recognizeMenu, finger, listener);
            

            changeState(StateMachine.ScreenState.GAME_MENU);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Azure);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public void changeState(StateMachine.ScreenState state)
        {
            switch (state)
            {
                case StateMachine.ScreenState.GAME_MENU:
                    if(currentState.state == StateMachine.ScreenState.MENU_RH)
                        Components.Remove(gs_RecognizeHuruf);
                    if (currentState.state == StateMachine.ScreenState.MENU_RM)
                        Components.Remove(gs_RecognizeMnist);
                    if (currentState.state == StateMachine.ScreenState.MENU_RM200)
                        Components.Remove(gs_RecognizeMnist200);
                    if (currentState.state == StateMachine.ScreenState.MENU_ADDDATA)
                        Components.Remove(gs_AddData);
                    Components.Clear();   
                    currentState.state = StateMachine.ScreenState.GAME_MENU;
                    Components.Add(gs_StartScreen);
                    break;
                case StateMachine.ScreenState.MENU_ADDDATA:
                    if(currentState.state == StateMachine.ScreenState.GAME_MENU)
                        Components.Remove(gs_StartScreen);
                    Components.Clear();   
                    Components.Add(gs_AddData);
                    currentState.state = StateMachine.ScreenState.MENU_ADDDATA;
                    break;
                case StateMachine.ScreenState.MENU_RM:
                    if (currentState.state == StateMachine.ScreenState.GAME_MENU)
                        Components.Remove(gs_StartScreen);
                    Components.Clear();
                    Components.Add(gs_RecognizeMnist);
                    currentState.state = StateMachine.ScreenState.MENU_RM;
                    break;
                case StateMachine.ScreenState.MENU_RH:
                    if (currentState.state == StateMachine.ScreenState.GAME_MENU)
                        Components.Remove(gs_StartScreen);
                    Components.Clear();
                    Components.Add(gs_RecognizeHuruf);
                    currentState.state = StateMachine.ScreenState.MENU_RH;
                    break;
                case StateMachine.ScreenState.MENU_RM200:
                    if (currentState.state == StateMachine.ScreenState.GAME_MENU)
                        Components.Remove(gs_StartScreen);
                    Components.Clear();
                    Components.Add(gs_RecognizeMnist200);
                    currentState.state = StateMachine.ScreenState.MENU_RM200;
                    break;
                case StateMachine.ScreenState.EXIT_GAME:
                    Exit();
                    break;
            }
        }
    }
}
