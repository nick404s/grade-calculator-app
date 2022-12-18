/*
 * School: Palomar College
 * Course: CSIT 180
 * Project: C# Project 6
 * Student: Nikolay Skryabin 
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace GradeCalculatorApp
{
    public partial class Form1 : Form
    {
        // variables to store student's data
        private string name;
        private string className;
        private string semester;
        private string pictureFileName;
        private double[] scores;

        // a variable to store a file path
        private string filePath;

        public Form1()
        {
            InitializeComponent();

            // initialize the variables
            name = string.Empty;
            className = string.Empty;
            semester = string.Empty;
            pictureFileName = string.Empty;
            filePath = String.Empty;
            scores = new double[8];

            // set the Palomar College label text
            PalomarLabel.Text = "P\nA\nL\nO\nM\nA\nR\n\nC\nO\nL\nL\nE\nG\nE";
        }

        /// <summary>
        /// Exits the App from the menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Opens file dialog from the menu and processes the selected file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // set initial directory to open for choosing the student files
            openFileDialog1.InitialDirectory = Application.CommonAppDataPath + @"\Static\";

            // disable multiselecting files
            openFileDialog1.Multiselect = false;

            // select only text files
            openFileDialog1.Filter = "Text files|*.txt";

            // restore each time
            openFileDialog1.RestoreDirectory = true;

            // open the window
            DialogResult dialog = openFileDialog1.ShowDialog();

            // check if a file was selected
            if (dialog == DialogResult.OK)
            {
                // clear the grade
                FinalGradeLabel.Text = string.Empty;

                // enable the calculate button
                CalculateButton.Enabled = true;

                // process data from the selected file
                ProcessFileData();

                // set the data in the UI
                SetStudentInfo();
            }
        }

        /// <summary>
        /// Processes the selected file data.
        /// Throws an Exception if the processed data type is not correct.
        /// </summary>
        private void ProcessFileData()
        {
            // get an array with a student data
            string[] data = ReadData().Split(',');

            // initialize the variables with the data 
            name = data[0];
            className = data[1];
            semester = data[2];
            pictureFileName = data[3];

            // initialize index for processing scores
            int index = 0;

            try
            {
                for (int i = 0; i < data.Length; i++)
                {
                    // check for elements at indexes after 3
                    if (i > 3)
                    {
                        // initialize the scores array
                        scores[index++] = Convert.ToDouble(data[i]);
                    }
                }
            }
            catch (Exception exception)
            {
                // Display the error message
                DisplayError(exception);
            }
        }

        /// <summary>
        /// Reads data from the selected file. 
        /// Throws an Exception if the file is not found.
        /// </summary>
        /// <returns>a string with data</returns>
        private string ReadData()
        {
            // a string to store a student data from file
            string line = string.Empty;

            try
            {
                // get the file path
                filePath = openFileDialog1.FileName;

                // create the stream reader and pass the path
                StreamReader reader = new StreamReader(filePath);

                // read the file till end
                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();
                }

                reader.Close();
            }
            catch (Exception exception)
            {
                // Display the error message
                DisplayError(exception);
            }
            return line;
        }

        /// <summary>
        /// Sets a student info and image.
        /// Throws an Exception if the image is not found.
        /// </summary>
        private void SetStudentInfo()
        {
            try
            {
                // set the labels, picture, scores
                NameLabel.Text = name;
                ClassLabel.Text = className;
                SemesterLabel.Text = semester;
                StudentPictureBox.Image = Image.FromFile($@"Images\{pictureFileName}");
                SetScores();
            }
            catch (Exception exception)
            {
                // Display the error message
                DisplayError(exception);
            }
        }

        /// <summary>
        /// Sets student's scores.
        /// </summary>
        private void SetScores()
        {
            // clear the text box
            ScoresRichTextBox.Text = String.Empty;

            // loop to add scores to the textbox
            for (int i = 0; i < scores.Length; i++)
            {
                // check projects
                if (i < scores.Length - 2)
                {
                    ScoresRichTextBox.AppendText($"{$"Project{i + 1}",-15} {scores[i]}\n");
                }
                // check midterm
                if (i == scores.Length - 2)
                {
                    ScoresRichTextBox.AppendText($"{"Midterm",-15}{scores[i]}\n");
                }
                // check final
                if (i == scores.Length - 1)
                {
                    ScoresRichTextBox.AppendText($"{"Final",-15}    {scores[i]}");
                }
            }
        }

        /// <summary>
        /// Calculates student's grade.
        /// Throws an Exception if the scores are not present.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CalculateButton_Click(object sender, EventArgs e)
        {
            try
            {
                FinalGradeLabel.Text = GetGradeAndColor().ElementAt(0).Key;
                FinalGradeLabel.ForeColor = GetGradeAndColor().ElementAt(0).Value;
            }
            catch (Exception exception)
            {
                // Display the error message
                DisplayError(exception);

            }
        }

        /// <summary>
        /// Returns the color and letter grade depending on 
        /// a student score.
        /// </summary>
        /// <returns>dictionary with the grade and color</returns>
        private Dictionary<string, Color> GetGradeAndColor()
        {
            // get points 
            double points = GetPoints();

            // check the points and return a Dictionary
            // with the letter and color 
            if (points > 89)
            {
                return new Dictionary<string, Color>() 
                {
                    { "A", Color.Green }
                };
            }
            else if (points > 79)
            {
                return new Dictionary<string, Color>()
                {
                    { "B", Color.Green }
                };
            }
            else if (points > 69)
            {
                return new Dictionary<string, Color>()
                {
                    { "C", Color.Green }
                };
            }
            else if (points > 59)
            {
                return new Dictionary<string, Color>()
                {
                    { "D", Color.Orange }
                };
            }
            else
            {
                return new Dictionary<string, Color>()
                {
                    { "F", Color.Red }
                };
            }
        }

        /// <summary>
        /// Calculate student's score.
        /// </summary>
        /// <returns>double student score</returns>
        private double GetPoints()
        {
            // a vaiable to store the total score
            double totalScore = 0;

            // check if there is any scores in the array
            if (scores.Length > 0)
            {
                foreach (var score in scores)
                {
                    // calculate the total score
                    totalScore += score;
                }
                // calculate and return the points
                return totalScore / scores.Length;
            }
            return 0;
        }

        /// <summary>
        /// Displays an exception in the MessageBox.
        /// </summary>
        /// <param name="exception">the current exception</param>
        private void DisplayError(Exception exception)
        {
            // Display the error message and exit
            MessageBox.Show($"An error has occured!\n" +
                $"The error was: {exception.Message}");
        }
    }
}
