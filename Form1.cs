using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DataPlotter;

namespace FunctionPlotter_Req1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string function = textBox1.Text.Replace(" ", "");

            if (function.Length == 0)
            {
                textBox2.Text = "Please provide a non empty function!";
            }
            else if(Regex.IsMatch(function, "^[+|*|^|/]"))
            {
                textBox2.Text = "Function can't start with an operator other than negative sign!";
            }
            else if (Regex.IsMatch(function, "[+|*|^|/|-]$"))
            {
                textBox2.Text = "Function can't end with an operator!";
            }
            else if (Regex.IsMatch(function, "[+|*|^|/|-]{2,}"))
            {
                textBox2.Text = "Function can't contain two or more consecutive operators!";
            }
            else if (Regex.IsMatch(function, "x{2,}"))
            {
                textBox2.Text = "Function can't contain two or more consecutive 'x's!";
            }
            else if (Regex.IsMatch(function, "[0-9]+x") || Regex.IsMatch(function, "x[0-9]+"))
            {
                textBox2.Text = "Please put an operator in a number and x";
            }
            else if (Regex.IsMatch(function, @"^[x 0-9 + * ^ / -]+$"))
            {
                textBox2.Text = "Valid!";

                //plot

                var t = new GraphPlot(function, (int)numericUpDown1.Value, (int)numericUpDown2.Value, 400);
                t.ShowDialog();
            }
            else
                textBox2.Text = "These are the only valid characters: from 0 to 9, (+-*^/), and x";
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
