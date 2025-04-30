using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace yapaySinir
{
    public class FileManager
    {
        public void SaveWeights(string filePath, double[,] w1, double[,] w2, string[] labels, bool buttonEnabled)
        {
            using (StreamWriter file = new StreamWriter(filePath))
            {
                file.WriteLine("w1");
                for (int i = 0; i < w1.GetLength(0); i++)
                {
                    for (int j = 0; j < w1.GetLength(1); j++)
                    {
                        file.Write(w1[i, j].ToString(CultureInfo.InvariantCulture) + " ");
                    }
                    file.WriteLine();
                }

                file.WriteLine("w2");
                for (int i = 0; i < w2.GetLength(0); i++)
                {
                    for (int j = 0; j < w2.GetLength(1); j++)
                    {
                        file.Write(w2[i, j].ToString(CultureInfo.InvariantCulture) + " ");
                    }
                    file.WriteLine();
                }

                foreach (var label in labels)
                {
                    file.WriteLine(label);
                }
                file.WriteLine(buttonEnabled);
            }
        }

        public bool LoadWeights(string filePath, double[,] w1, double[,] w2, string[] labels, out bool buttonEnabled, out string errorMessage)
        {
            buttonEnabled = false;
            errorMessage = string.Empty;
            bool formatValid = false;

            try
            {
                using (StreamReader file = new StreamReader(filePath))
                {
                    string line;
                    int labelIndex = 0;
                    int lineNumber = 0;
                    bool errorRateProcessed = false;

                    while ((line = file.ReadLine()) != null)
                    {
                        lineNumber++;
                        line = line.Trim();
                        if (string.IsNullOrEmpty(line))
                            continue;

                        if (line == "w1")
                        {
                            for (int i = 0; i < w1.GetLength(0); i++)
                            {
                                line = file.ReadLine()?.Trim();
                                lineNumber++;
                                if (string.IsNullOrEmpty(line))
                                {
                                    errorMessage = $"Line {lineNumber}: Missing data for w1 row {i + 1}.";
                                    return false;
                                }

                                string[] values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                if (values.Length != w1.GetLength(1))
                                {
                                    errorMessage = $"Line {lineNumber}: Expected {w1.GetLength(1)} values for w1, found {values.Length}.";
                                    return false;
                                }

                                for (int j = 0; j < w1.GetLength(1); j++)
                                {
                                    if (!double.TryParse(values[j], NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                                    {
                                        errorMessage = $"Line {lineNumber}: Invalid number format in w1 at position {j + 1}.";
                                        return false;
                                    }
                                    w1[i, j] = value;
                                }
                            }
                            formatValid = true;
                        }
                        else if (line == "w2")
                        {
                            for (int i = 0; i < w2.GetLength(0); i++)
                            {
                                line = file.ReadLine()?.Trim();
                                lineNumber++;
                                if (string.IsNullOrEmpty(line))
                                {
                                    errorMessage = $"Line {lineNumber}: Missing data for w2 row {i + 1}.";
                                    return false;
                                }

                                string[] values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                                if (values.Length != w2.GetLength(1))
                                {
                                    errorMessage = $"Line {lineNumber}: Expected {w2.GetLength(1)} values for w2, found {values.Length}.";
                                    return false;
                                }

                                for (int j = 0; j < w2.GetLength(1); j++)
                                {
                                    if (!double.TryParse(values[j], NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                                    {
                                        errorMessage = $"Line {lineNumber}: Invalid number format in w2 at position {j + 1}.";
                                        return false;
                                    }
                                    w2[i, j] = value;
                                }
                            }
                            formatValid = true;
                        }
                        else if (line.StartsWith("Error Rate: "))
                        {
                            if (errorRateProcessed)
                            {
                                continue;
                            }
                            if (labelIndex < labels.Length)
                            {
                                labels[labelIndex++] = line;
                                errorRateProcessed = true;
                                formatValid = true;
                            }
                        }
                        else if (line.StartsWith("A Output: ") || line.StartsWith("B Output: ") ||
                                 line.StartsWith("C Output: ") || line.StartsWith("D Output: ") ||
                                 line.StartsWith("E Output: "))
                        {
                            if (labelIndex < labels.Length)
                            {
                                labels[labelIndex++] = line;
                                formatValid = true;
                            }
                        }
                        else if (line == "True" || line == "False")
                        {
                            buttonEnabled = bool.Parse(line);
                            formatValid = true;
                        }
                        else
                        {
                            errorMessage = $"Line {lineNumber}: Unexpected content: '{line}'.";
                            return false;
                        }
                    }

                    if (labelIndex < labels.Length)
                    {
                        errorMessage = $"Expected {labels.Length} labels, found {labelIndex}.";
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Error reading file: {ex.Message}";
                return false;
            }

            if (!formatValid)
            {
                errorMessage = "File is empty or contains no valid data.";
            }

            return formatValid;
        }
    }
}