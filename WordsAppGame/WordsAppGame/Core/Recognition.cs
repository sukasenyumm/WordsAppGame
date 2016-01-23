using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibDL;

namespace WordsAppGame.Core
{
    public class Recognition
    {
        public string outputLabel;
        private DBN dbn;
        private NeuralNetwork ann;

        public Recognition(string fileName,bool isDBN)
        {
            if (isDBN)
                dbn = DBN.Load(fileName);
            else
                ann = NeuralNetwork.Load(fileName);
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
        public void setRecognize(double[] dataTest,bool isAngka)
        {
           
            if (dbn != null || ann !=null)
            {
                if (dataTest.ToList().IndexOf(dataTest.Max()) != 0)
                {
                    double[] outTest = (dbn==null)?ann.Compute(dataTest):dbn.Compute(dataTest);
                   
                    if (isAngka)
                    {
                        outputLabel = (10 - outTest.ToList().IndexOf(outTest.Max())).ToString();
                        if (outputLabel == "10")
                            outputLabel = "0";
                    }
                    else
                    {
                        outputLabel = IntToLetters((26 - outTest.ToList().IndexOf(outTest.Max())));
                    }
                }
                else
                    outputLabel = "-";
            }
            else
            {
                outputLabel = "network error";
            }
        }
        public string getRecognize()
        {
            return outputLabel;
        }
    }
}
