using Accord.Math;
using System;
using System.Linq;

namespace yapaySinir
{
    public class NeuralNetwork
    {
        private double[,] w1;
        private double[,] w2;
        private double[] hiddenLayer;
        private double[] outputLayer;
        private readonly double learningRate;
        private readonly int iterationNumber;
        private double totalError;

        public NeuralNetwork(int inputSize, int hiddenSize, int outputSize, double learningRate, int iterationNumber)
        {
            this.learningRate = learningRate;
            this.iterationNumber = iterationNumber;
            w1 = InitializeWeights(inputSize, hiddenSize);
            w2 = InitializeWeights(hiddenSize, outputSize);
            hiddenLayer = new double[hiddenSize];
            outputLayer = new double[outputSize];
        }

        private double[,] InitializeWeights(int rows, int cols)
        {
            var rand = new Random();
            var weights = new double[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    weights[i, j] = rand.NextDouble() * 2 - 1;
                }
            }
            return weights;
        }

        private double Sigmoid(double x)
        {
            return 1.0 / (1.0 + Math.Exp(-x));
        }

        private int GetMaxIndex(double[] array)
        {
            int maxIndex = 0;
            double maxValue = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i] > maxValue)
                {
                    maxIndex = i;
                    maxValue = array[i];
                }
            }
            return maxIndex;
        }

        public double[] FeedForward(double[] input, double[,] weights)
        {
            int inputSize = input.Length;
            int outputSize = weights.GetLength(1);
            var output = new double[outputSize];
            for (int j = 0; j < outputSize; j++)
            {
                double sum = 0;
                for (int i = 0; i < inputSize; i++)
                {
                    sum += input[i] * weights[i, j];
                }
                output[j] = Sigmoid(sum);
            }
            return output;
        }

        private double Backpropagate(double[] input, double[] output, double[] target, double[] hiddenLayer)
        {
            int inputSize = input.Length;
            int outputSize = output.Length;
            double[] outputErrors = new double[outputSize];
            for (int i = 0; i < outputSize; i++)
            {
                outputErrors[i] = (target[i] - output[i]) * output[i] * (1 - output[i]);
            }

            double[] hiddenErrors = new double[hiddenLayer.Length];
            for (int i = 0; i < hiddenLayer.Length; i++)
            {
                double sum = 0;
                for (int j = 0; j < outputSize; j++)
                {
                    sum += outputErrors[j] * w2[i, j];
                }
                hiddenErrors[i] = sum * hiddenLayer[i] * (1 - hiddenLayer[i]);
            }

            for (int i = 0; i < inputSize; i++)
            {
                for (int j = 0; j < hiddenLayer.Length; j++)
                {
                    w1[i, j] += learningRate * hiddenErrors[j] * input[i];
                }
            }

            for (int i = 0; i < hiddenLayer.Length; i++)
            {
                for (int j = 0; j < outputSize; j++)
                {
                    w2[i, j] += learningRate * outputErrors[j] * hiddenLayer[i];
                }
            }

            return outputErrors.Sum(x => Math.Pow(x, 2)) / 2;
        }

        public (double TotalError, double[][] Outputs) Train(double[,] inputs, double[,] outputs)
        {
            double[][] predictedOutputs = new double[inputs.GetLength(0)][];
            totalError = 0;

            for (int iter = 0; iter < iterationNumber; iter++)
            {
                totalError = 0;
                for (int i = 0; i < inputs.GetLength(0); i++)
                {
                    hiddenLayer = FeedForward(inputs.GetRow(i), w1);
                    outputLayer = FeedForward(hiddenLayer, w2);
                    totalError += Backpropagate(inputs.GetRow(i), outputLayer, outputs.GetRow(i), hiddenLayer);
                    predictedOutputs[i] = outputLayer;
                }
            }

            return (totalError, predictedOutputs);
        }

        public int Predict(double[] input)
        {
            var hidden = FeedForward(input, w1);
            var output = FeedForward(hidden, w2);
            return GetMaxIndex(output);
        }

        public void Reset(int inputSize, int hiddenSize, int outputSize)
        {
            w1 = InitializeWeights(inputSize, hiddenSize);
            w2 = InitializeWeights(hiddenSize, outputSize);
            hiddenLayer = new double[hiddenSize];
            outputLayer = new double[outputSize];
            totalError = 0;
        }

        public double[,] GetW1() => w1;
        public double[,] GetW2() => w2;
    }
}