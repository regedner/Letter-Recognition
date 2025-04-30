using Accord.Math;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace yapaySinir
{
    public partial class Form1 : Form
    {
        private readonly NeuralNetwork neuralNetwork;
        private readonly DataProcessor dataProcessor;
        private readonly FileManager fileManager;
        private readonly NetworkConfig config;
        private bool hideLines = false;

        private readonly double[][][] educationDataSet = new double[][][]
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

        public Form1()
        {
            InitializeComponent();
            config = new NetworkConfig();
            dataProcessor = new DataProcessor(educationDataSet, config.Letters);
            neuralNetwork = new NeuralNetwork(
                inputSize: config.RowNumber * config.ColumnNumber,
                hiddenSize: 10,
                outputSize: config.Letters.Length,
                learningRate: config.LearningRate,
                iterationNumber: config.IterationNumber
            );
            fileManager = new FileManager();
            InitializeDataGridView();

            numericUpDown1.Minimum = 0.01m;
            numericUpDown1.Maximum = 1m;
            numericUpDown1.DecimalPlaces = 2;
            numericUpDown1.Increment = 0.01m;

            for (int i = 0; i < config.RowNumber; i++)
            {
                for (int j = 0; j < config.ColumnNumber; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = 0;
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                    dataGridView1.Rows[i].Cells[j].Style.ForeColor = Color.White;
                }
            }
        }

        private void InitializeDataGridView()
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.EditMode = DataGridViewEditMode.EditProgrammatically;
            dataGridView1.RowHeadersVisible = false;

            dataGridView1.ColumnCount = config.ColumnNumber;
            for (int i = 0; i < config.ColumnNumber; i++)
            {
                dataGridView1.Columns[i].Width = dataGridView1.Width / config.ColumnNumber;
            }

            dataGridView1.RowCount = config.RowNumber;
            for (int i = 0; i < config.RowNumber; i++)
            {
                dataGridView1.Rows[i].Height = dataGridView1.Height / config.RowNumber;
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

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < config.RowNumber; i++)
            {
                for (int j = 0; j < config.ColumnNumber; j++)
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

            var (totalError, predictedOutputs) = neuralNetwork.Train(dataProcessor.GetInputs(), dataProcessor.GetOutputs());
            label6.Text = $"Error Rate: {totalError}";

            for (int i = 0; i < predictedOutputs.Length; i++)
            {
                int maxIndex = neuralNetwork.Predict(dataProcessor.GetInputs().GetRow(i));
                string outputText = $"{config.Letters[i]} Output: {predictedOutputs[i][maxIndex]:F2}";
                switch (i)
                {
                    case 0: label1.Text = outputText; break;
                    case 1: label2.Text = outputText; break;
                    case 2: label3.Text = outputText; break;
                    case 3: label4.Text = outputText; break;
                    case 4: label5.Text = outputText; break;
                }
            }

            if (totalError <= (double)numericUpDown1.Value)
            {
                MessageBox.Show("Education Completed.");
                button2.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double[] input = new double[config.ColumnNumber * config.RowNumber];
            for (int i = 0; i < config.RowNumber; i++)
            {
                for (int j = 0; j < config.ColumnNumber; j++)
                {
                    input[i * config.ColumnNumber + j] = dataGridView1.Rows[i].Cells[j].Style.BackColor == Color.Black ? 1 : 0;
                }
            }

            int predictedIndex = neuralNetwork.Predict(input);
            MessageBox.Show($"Guessed letter: {config.Letters[predictedIndex]}");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < config.RowNumber; i++)
            {
                for (int j = 0; j < config.ColumnNumber; j++)
                {
                    dataGridView1.Rows[i].Cells[j].Value = 0;
                    dataGridView1.Rows[i].Cells[j].Style.BackColor = Color.White;
                    dataGridView1.Rows[i].Cells[j].Style.ForeColor = Color.White;
                }
            }

            label1.Text = $"{config.Letters[0]} Output = ";
            label2.Text = $"{config.Letters[1]} Output = ";
            label3.Text = $"{config.Letters[2]} Output = ";
            label4.Text = $"{config.Letters[3]} Output = ";
            label5.Text = $"{config.Letters[4]} Output = ";
            label6.Text = "Error Rate: ";

            numericUpDown1.Value = 0.01m;
            neuralNetwork.Reset(
                inputSize: config.RowNumber * config.ColumnNumber,
                hiddenSize: 10,
                outputSize: config.Letters.Length
            );
            button2.Enabled = true;
        }

        private void button5_Click(object sender, EventArgs e)
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

        private void button6_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "Text Files | *.txt";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string[] labels = { label6.Text, label1.Text, label2.Text, label3.Text, label4.Text, label5.Text };
                    fileManager.SaveWeights(dialog.FileName, neuralNetwork.GetW1(), neuralNetwork.GetW2(), labels, button2.Enabled);
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
                    string[] labels = new string[6];
                    bool buttonEnabled;
                    string errorMessage;
                    bool success = fileManager.LoadWeights(dialog.FileName, neuralNetwork.GetW1(), neuralNetwork.GetW2(), labels, out buttonEnabled, out errorMessage);
                    if (success)
                    {
                        label6.Text = labels[0];
                        label1.Text = labels[1];
                        label2.Text = labels[2];
                        label3.Text = labels[3];
                        label4.Text = labels[4];
                        label5.Text = labels[5];
                        button2.Enabled = buttonEnabled;
                        MessageBox.Show("Weights uploaded successfully.");
                    }
                    else
                    {
                        MessageBox.Show($"File format is not suitable: {errorMessage}");
                    }
                }
            }
        }
    }
}