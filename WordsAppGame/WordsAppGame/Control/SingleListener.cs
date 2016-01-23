using Leap;
using System.Collections.Generic;
using System;
using WordsAppGame.Core;

namespace WordsAppGame.Control
{
    public class SingleListener : Listener
    {
        public override void OnInit(Controller cntrlr)
        {
            //Console.WriteLine("Initialized");
        }

        public override void OnConnect(Controller cntrlr)
        {
            //Console.WriteLine("Connected");
        }

        public override void OnDisconnect(Controller cntrlr)
        {
            //Console.WriteLine("Disconnected");
        }

        public override void OnExit(Controller cntrlr)
        {
            //Console.WriteLine("Exited");
        }

        private long currentTime;
        private long previousTime;
        private long timeChange;
        const int FramePause = 10000;
        public List<FingerPointStorage> fingerPoint = new List<FingerPointStorage>();

        public override void OnFrame(Controller cntrlr)
        {
            // Get the current frame.
            Frame currentFrame = cntrlr.Frame();

            currentTime = currentFrame.Timestamp;
            timeChange = currentTime - previousTime;
            if (timeChange > FramePause)
            {

                //pointable jari
                if (!currentFrame.Hands.IsEmpty)
                {
                    // Get the first finger in the list of fingers
                    Pointable finger = currentFrame.Pointables[0];
                    // Get the closest screen intercepting a ray projecting from the finger
                    Screen screen = cntrlr.LocatedScreens.ClosestScreenHit(finger);

                    if (screen != null && screen.IsValid)
                    {
                        // Get the velocity of the finger tip
                        //var tipVelocity = (int)finger.TipVelocity.Magnitude;
                        Hand hand = currentFrame.Hands.Frontmost;
                        // Use tipVelocity to reduce jitters when attempting to hold
                        // the cursor steady
                        if (finger.TipVelocity.Magnitude > 25)
                        {
                            float xScreenIntersect = (float)screen.Intersect(finger, true).x;
                            float yScreenIntersect = (float)(1 - screen.Intersect(finger, true).y);
                            float zScreenIntersect = screen.DistanceToPoint(finger.TipPosition);

                            if (xScreenIntersect.ToString() != "NaN")
                            {
                            
                                if (fingerPoint.Count <= 0)
                                {
                                    fingerPoint.Add(new FingerPointStorage(xScreenIntersect, yScreenIntersect, zScreenIntersect, false, 0, 0));
                                }
                                else
                                {

                                    ////////////////////gesture
                                    if (currentFrame.Pointables.Count > 2)
                                    {
                                        if (currentFrame.Gestures().Count > 0)
                                        {
                                            //debugbox1.Text = "Gesture" + frame.Gestures()[0].ToString();
                                            int numGestures = currentFrame.Gestures().Count;
                                            if (numGestures > 0)
                                            {
                                                for (int i = 0; i < numGestures; i++)
                                                {
                                                    if (currentFrame.Gestures()[i].Type == Leap.Gesture.GestureType.TYPESCREENTAP)
                                                    {
                                                        ScreenTapGesture tap = new ScreenTapGesture(currentFrame.Gestures()[i]);
                                                        fingerPoint[0].g_Tap = tap.Position.z;
                                                    }
                                                    else if (currentFrame.Gestures()[i].Type == Leap.Gesture.GestureType.TYPECIRCLE)
                                                    {
                                                        CircleGesture circle = new CircleGesture(currentFrame.Gestures()[i]);
                                                        fingerPoint[0].g_circle = circle.Progress;

                                                    }
                                                }
                                            }
                                        }
                                    }
                                    ///////////////////////////////////
                                    
                                    fingerPoint[0].g_X = xScreenIntersect;
                                    fingerPoint[0].g_Y = yScreenIntersect;
                                    fingerPoint[0].g_Z = zScreenIntersect;
                                    fingerPoint[0].isActive = true;
                                    if (hand.IsLeft)
                                        fingerPoint[0].numHand = true;
                                    else
                                        fingerPoint[0].numHand = false;
                                    //Console.WriteLine("leap x-axis: {0},y-axis: {1},z-axis: {2}", fingerPoint[0].g_X, fingerPoint[0].g_Y, fingerPoint[0].g_Z);
                                }
                            }
                        }
                    }
                }
                previousTime = currentTime;
            }
        }
    }
}
