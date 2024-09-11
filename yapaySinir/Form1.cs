using Accord.Math;
using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace yapaySinir
{
    public partial class Form1 : Form
    {
        private const int rowNumber = 7;
        private const int columnNumber = 5;
        private double[,] w1;
        private double[,] w2;
        private double[,] inputs;
        private double[,] outputs;
        private double[] hiddenLayer;
        private double[] outputLayer;
        private double learningRate = 0.1;
        private int iterationNumber = 20;
        double totalError = 0;
        private bool hideLines = false;
        private string[] letters = { "A", "B", "C", "D", "E" };


        public Form1()
        {
            InitializeComponent();
            InitializeDataGridView();
            InitializeNetwork();

            numericUpDown1.Minimum = 0.01m;
            numericUpDown1.Maximum = 1m;
            numericUpDown1.DecimalPlaces = 2;
            numericUpDown1.Increment = 0.01m;

            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < columnNumber; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = 0;
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                    dataGridView1.Rows[i].Cells[j].Style.ForeColor = Color.White;
                }
            }


        }


        private void InitializeNetwork()
        {
            int inputSize = rowNumber * columnNumber;
            int hiddenLayerSize = 10;
            int outputSize = letters.Length;


            w1 = InitializeWeights(inputSize, hiddenLayerSize);
            w2 = InitializeWeights(hiddenLayerSize, outputSize);


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


        private double[] Flatten(double[][] matrix)
        {
            return matrix.SelectMany(x => x).ToArray();
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


        private double[] FeedForward(double[] input, double[,] weights)
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


        private double GeriBesleme(double[] input, double[] output, double[] target, double[] hiddenLayer)
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
            double totalError = outputErrors.Sum(x => Math.Pow(x, 2)) / 2;
            return totalError;
        }


        private void Egit(double hataOraniInput)
        {

            for (int iter = 0; iter < iterationNumber; iter++)
            {
                totalError = 0;

                for (int i = 0; i < inputs.GetLength(0); i++)
                {

                    hiddenLayer = FeedForward(inputs.GetRow(i), w1);
                    outputLayer = FeedForward(hiddenLayer, w2);


                    totalError += GeriBesleme(inputs.GetRow(i), outputLayer, outputs.GetRow(i), hiddenLayer);
                }

                label6.Text = $"Error Rateı: {totalError}";
            }


            for (int i = 0; i < inputs.GetLength(0); i++)
            {
                var input = inputs.GetRow(i);
                var output = FeedForward(input, w1);
                output = FeedForward(output, w2);

                int maxIndex = GetMaxIndex(output);
                switch (i)
                {
                    case 0:
                        label1.Text = $"{letters[0]} Output: {output[maxIndex]:F2}";
                        break;
                    case 1:
                        label2.Text = $"{letters[1]} Output: {output[maxIndex]:F2}";
                        break;
                    case 2:
                        label3.Text = $"{letters[2]} Output: {output[maxIndex]:F2}";
                        break;
                    case 3:
                        label4.Text = $"{letters[3]} Output: {output[maxIndex]:F2}";
                        break;
                    case 4:
                        label5.Text = $"{letters[4]} Output: {output[maxIndex]:F2}";
                        break;
                }
            }

        }


        private void Define()
        {

            double[] input = new double[columnNumber * rowNumber];
            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < columnNumber; j++)
                {
                    input[i * columnNumber + j] = dataGridView1.Rows[i].Cells[j].Style.BackColor == Color.Black ? 1 : 0;
                }
            }

            var output = FeedForward(input, w1);
            output = FeedForward(output, w2);

            int maxIndex = GetMaxIndex(output);
            MessageBox.Show($"Guessed letter: {letters[maxIndex]}");
        }


        private void InitializeDataGridView()
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView1.RowHeadersVisible = false;

            dataGridView1.ColumnCount = columnNumber;
            for (int i = 0; i < columnNumber; i++)
            {
                dataGridView1.Columns[i].Width = dataGridView1.Width / columnNumber;
            }

            dataGridView1.RowCount = rowNumber;
            for (int i = 0; i < rowNumber; i++)
            {
                dataGridView1.Rows[i].Height = dataGridView1.Height / rowNumber;
            }

        }


        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.White;

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                dataGridView1.CurrentCell = null;
            }

            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
            {
                int value = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = (value == 0) ? 1 : 0;
            }

            if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor == Color.Black)
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.White;
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.White;

            }
            else
            {
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor = Color.Black;
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Black;
            }

        }


        private void Reset()
        {

            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < columnNumber; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = 0;
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                    dataGridView1.Rows[i].Cells[j].Style.ForeColor = Color.White;
                }
            }

            label1.Text = $"{letters[0]} Output = ";
            label2.Text = $"{letters[1]} Output = ";
            label3.Text = $"{letters[2]} Output = ";
            label4.Text = $"{letters[3]} Output = ";
            label5.Text = $"{letters[4]} Output = ";
            label6.Text = "Error Rateı: ";

            totalError = 0;
            numericUpDown1.Value = 0.01m;
            InitializeNetwork();

            button2.Enabled = true;
        }


        private void CizgiDegistirme()
        {
            if (hideLines)
            {
                dataGridView1.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.Single;
                hideLines = false;
                button5.Text = "Remove Lines";
            }
            else
            {
                dataGridView1.AdvancedCellBorderStyle.All = DataGridViewAdvancedCellBorderStyle.None;
                hideLines = true;
                button5.Text = "Bring Back Lines";
            }
        }


        private void Save()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
                saveFileDialog.DefaultExt = "txt";
                saveFileDialog.AddExtension = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    using (StreamWriter file = new StreamWriter(filePath))
                    {
                        file.WriteLine("w1");
                        for (int i = 0; i < w1.GetLength(0); i++)
                        {
                            for (int j = 0; j < w1.GetLength(1); j++)
                            {
                                file.Write(w1[i, j] + " ");
                            }
                            file.WriteLine();
                        }

                        file.WriteLine("w2");
                        for (int i = 0; i < w2.GetLength(0); i++)
                        {
                            for (int j = 0; j < w2.GetLength(1); j++)
                            {
                                file.Write(w2[i, j] + " ");
                            }
                            file.WriteLine();
                        }

                        file.WriteLine(label6.Text);
                        file.WriteLine(label1.Text);
                        file.WriteLine(label2.Text);
                        file.WriteLine(label3.Text);
                        file.WriteLine(label4.Text);
                        file.WriteLine(label5.Text);
                        file.WriteLine(button2.Enabled);
                    }
                }
            }
        }



        private void Upload(string filePath)
        {
            using (StreamReader file = new StreamReader(filePath))
            {
                string satır;
                bool formatUygun = false;
                while ((satır = file.ReadLine()) != null)
                {
                    if (satır == "w1")
                    {
                        for (int i = 0; i < w1.GetLength(0); i++)
                        {
                            string[] values = file.ReadLine().Split(' ');
                            for (int j = 0; j < w1.GetLength(1); j++)
                            {
                                w1[i, j] = double.Parse(values[j]);
                            }
                        }
                        formatUygun = true;
                    }
                    else if (satır == "w2")
                    {
                        for (int i = 0; i < w2.GetLength(0); i++)
                        {
                            string[] values = file.ReadLine().Split(' ');
                            for (int j = 0; j < w2.GetLength(1); j++)
                            {
                                w2[i, j] = double.Parse(values[j]);
                            }
                        }
                        formatUygun = true;
                    }
                    else if (satır.StartsWith("Error Rateı: "))
                    {
                        label6.Text = satır;
                        formatUygun = true;
                    }
                    else if (satır.StartsWith("A Output: "))
                    {
                        label1.Text = satır;
                        formatUygun = true;
                    }
                    else if (satır.StartsWith("B Output: "))
                    {
                        label2.Text = satır;
                        formatUygun = true;
                    }
                    else if (satır.StartsWith("C Output: "))
                    {
                        label3.Text = satır;
                        formatUygun = true;
                    }
                    else if (satır.StartsWith("D Output: "))
                    {
                        label4.Text = satır;
                        formatUygun = true;
                    }
                    else if (satır.StartsWith("E Output: "))
                    {
                        label5.Text = satır;
                        formatUygun = true;
                    }
                    else if (satır == "True" || satır == "False")
                    {
                        button2.Enabled = bool.Parse(satır);
                        formatUygun = true;
                    }
                }
                if (!formatUygun)
                {
                    MessageBox.Show("File format is not suitable.");
                }
                else
                {
                    MessageBox.Show("Weights uploaded successfully.");
                }
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < rowNumber; i++)
            {
                for (int j = 0; j < columnNumber; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = 0;
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                    dataGridView1.Rows[i].Cells[j].Style.ForeColor = Color.White;
                }
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            if (numericUpDown1.Value <= 0)
            {
                MessageBox.Show("Please enter an error threshold value.");
                return;
            }

            Egit((double)numericUpDown1.Value);

            if (totalError <= (double)numericUpDown1.Value)
            {
                MessageBox.Show($"Education Completed.");
                button2.Enabled = false;
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            Define();
        }


        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            double hataOraniInput = (double)numericUpDown1.Value;
        }


        private void button4_Click(object sender, EventArgs e)
        {
            Reset();
        }


        private void button5_Click(object sender, EventArgs e)
        {
            CizgiDegistirme();
        }


        private void button6_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "Text Files | *.txt";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    
                    MessageBox.Show("Weights saved successfully.");
                }
            }
        }


        private void button7_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Text Files | *.txt";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Upload(dialog.FileName);
                }
            }
        }


        private double[][][] educationDataSet = new double[][][]
       {
            new double[][]
            {
                new double[] {0,0,1,0,0},
                new double[] {0,1,0,1,0},
                new double[] {1,0,0,0,1},
                new double[] {1,0,0,0,1},
                new double[] {1,1,1,1,1},
                new double[] {1,0,0,0,1},
                new double[] {1,0,0,0,1}
            },
            new double[][]
            {
                new double[] {1,1,1,1,0},
                new double[] {1,0,0,0,1},
                new double[] {1,0,0,0,1},
                new double[] {1,1,1,1,0},
                new double[] {1,0,0,0,1},
                new double[] {1,0,0,0,1},
                new double[] {1,1,1,1,0}
            },
            new double[][]
            {
                new double[] {0,0,1,1,1},
                new double[] {0,1,0,0,0},
                new double[] {1,0,0,0,0},
                new double[] {1,0,0,0,0},
                new double[] {1,0,0,0,0},
                new double[] {0,1,0,0,0},
                new double[] {0,0,1,1,1}
            },
            new double[][]
            {
                new double[] {1,1,1,0,0},
                new double[] {1,0,0,1,0},
                new double[] {1,0,0,0,1},
                new double[] {1,0,0,0,1},
                new double[] {1,0,0,0,1},
                new double[] {1,0,0,1,0},
                new double[] {1,1,1,0,0}
            },
            new double[][]
            {
                new double[] {1,1,1,1,1},
                new double[] {1,0,0,0,0},
                new double[] {1,0,0,0,0},
                new double[] {1,1,1,1,1},
                new double[] {1,0,0,0,0},
                new double[] {1,0,0,0,0},
                new double[] {1,1,1,1,1}
            }
       };

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
