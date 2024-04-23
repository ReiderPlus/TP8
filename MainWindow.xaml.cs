using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IOPath = System.IO.Path;

namespace TP8
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void rbNoFilter_Checked(object sender, RoutedEventArgs e)
        {
            UpdateCounterControlsAvailability(true, true);
        }

        private void rbStartsWithLetter_Checked(object sender, RoutedEventArgs e)
        {
            UpdateCounterControlsAvailability(false, true);
        }

        private void rbStartsWithDigit_Checked(object sender, RoutedEventArgs e)
        {
            UpdateCounterControlsAvailability(true, false);
        }

        private void rbStartsWithAlphaNumeric_Checked(object sender, RoutedEventArgs e)
        {
            UpdateCounterControlsAvailability(false, false);
        }

        private void UpdateCounterControlsAvailability(bool disableLetterCounter, bool disableDigitCounter)
        {
            rbStartsWithLetter.IsEnabled = !disableLetterCounter;
            rbStartsWithDigit.IsEnabled = !disableDigitCounter;
            filteredCharsTextBox.IsEnabled = disableLetterCounter;
            filteredDigitsTextBox.IsEnabled = disableDigitCounter;
        }

        private void ReadMatrixFromFile_Click(object sender, RoutedEventArgs e)
        {
            string filePath = IOPath.Combine(AppDomain.CurrentDomain.BaseDirectory, "123.txt");

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                originalMatrixTextBox.Text = string.Join(Environment.NewLine, lines);

                List<string> filteredValues = ProcessMatrix(lines);

                resultTextBox.Text = string.Join(Environment.NewLine, filteredValues);

                int filteredCharsCount = filteredValues.Sum(s => s.Count(char.IsLetter));
                int filteredDigitsCount = filteredValues.Sum(s => s.Count(char.IsDigit));

                filteredCharsTextBox.Text = $"Количество символов: {filteredCharsCount}";
                filteredDigitsTextBox.Text = $"Количество цифр: {filteredDigitsCount}";

                transformedMatrixTextBox.Text = string.Join(Environment.NewLine, TransformMatrix(lines, filteredValues));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading matrix from file: {ex.Message}");
            }
        }

        private List<string> ProcessMatrix(string[] lines)
        {
            List<string> filteredValues = new List<string>();

            for (int i = 0; i < lines.Length; i++)
            {
                string[] rowValues = lines[i].Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < rowValues.Length; j++)
                {
                    string value = rowValues[j];

                    if (rbStartsWithLetter.IsChecked == true && !char.IsLetter(value[0]))
                    {
                        continue;
                    }
                    else if (rbStartsWithDigit.IsChecked == true && !char.IsDigit(value[0]))
                    {
                        continue;
                    }

                    string transformedValue = TransformFirstLetter(value);

                    filteredValues.Add($"Row: {i + 1}, Column: {j + 1}, Old Value: {value}, Transformed Value: {transformedValue}");
                }
            }

            return filteredValues;
        }

        private string TransformFirstLetter(string value)
        {
            if (rbFirstToUpper.IsChecked == true)
            {
                return char.ToUpper(value[0]) + value.Substring(1);
            }
            else if (rbFirstToLower.IsChecked == true)
            {
                return char.ToLower(value[0]) + value.Substring(1);
            }
            else
            {
                return value;
            }
        }

        private string TransformMatrix(string[] lines, List<string> filteredValues)
        {
            string[] transformedMatrix = lines.ToArray();

            foreach (string filteredValue in filteredValues)
            {
                string[] parts = filteredValue.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
                int rowIndex = int.Parse(parts[0].Split(':')[1].Trim()) - 1;
                int colIndex = int.Parse(parts[1].Split(':')[1].Trim()) - 1;
                string transformedValue = parts[3].Split(':')[1].Trim();
                transformedMatrix[rowIndex] = transformedValue;
            }

            return string.Join(Environment.NewLine, transformedMatrix);
        }
    }
}
