using Accord.Math;
using System;
using System.Linq;

namespace yapaySinir
{
    public class DataProcessor
    {
        private readonly double[][][] educationDataSet;
        private readonly string[] letters;
        private double[,] inputs;
        private double[,] outputs;

        public DataProcessor(double[][][] dataSet, string[] letters)
        {
            educationDataSet = dataSet;
            this.letters = letters;
            InitializeInputsAndOutputs();
        }

        private void InitializeInputsAndOutputs()
        {
            int inputSize = educationDataSet[0].Length * educationDataSet[0][0].Length;
            int outputSize = letters.Length;

            inputs = new double[educationDataSet.Length, inputSize];
            outputs = new double[educationDataSet.Length, outputSize];

            for (int i = 0; i < educationDataSet.Length; i++)
            {
                double[] input = Flatten(educationDataSet[i]);
                inputs.SetRow(i, input);

                int targetIndex = Array.IndexOf(letters, letters[i]);
                outputs[i, targetIndex] = 1;
            }
        }

        public double[] Flatten(double[][] matrix)
        {
            return matrix.SelectMany(x => x).ToArray();
        }

        public double[,] GetInputs() => inputs;
        public double[,] GetOutputs() => outputs;
        public string[] GetLetters() => letters;
    }
}