#region Using Statement
using System;
#endregion

#region Singleton ScreenConfig
namespace WordsAppGame.Core
{
    public sealed class StateMachine
    {
        private static StateMachine instance = null;
        private static readonly object padlock = new Object();

        private StateMachine() { }
        public static StateMachine Instance
        {
            get{
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new StateMachine();
                    }
                    return instance;
                }
            }
        }
        public enum ScreenState
        { 
            GAME_MENU,
            MENU_RH,
            MENU_RM,
            MENU_RM200,
            MENU_ADDDATA,
            EXIT_GAME
        }
        public ScreenState state { get; set; }
        public int resWidth { get; set; }
        public int resHeight { get; set; }
    }
}
#endregion