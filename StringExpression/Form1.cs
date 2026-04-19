using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExpressionParser;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Append text to the expression textbox
        /// </summary>
        private void AppendToExpression(string text)
        {
            txtExpression.Text += text;
            txtExpression.Focus();
            txtExpression.SelectionStart = txtExpression.Text.Length;
        }

        /// <summary>
        /// Handle number button clicks (0-9 and .)
        /// </summary>
        private void btnNumber_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                AppendToExpression(btn.Text);
            }
        }

        /// <summary>
        /// Handle operator button clicks (+, -, *, /, (, ))
        /// </summary>
        private void btnOperator_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                AppendToExpression(btn.Text);
            }
        }

        /// <summary>
        /// Handle function button clicks (sin, cos, tan, sqrt, log, etc.)
        /// </summary>
        private void btnFunction_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                // For pow function, we need comma separator
                if (btn.Text == "pow")
                {
                    AppendToExpression("pow(,)");
                    // Move cursor back to position before the closing paren and comma
                    txtExpression.SelectionStart = txtExpression.Text.Length - 2;
                }
                else
                {
                    AppendToExpression(btn.Text + "()");
                    // Move cursor back to position before the closing paren
                    txtExpression.SelectionStart = txtExpression.Text.Length - 1;
                }
            }
        }

        /// <summary>
        /// Handle constant button clicks (π, e)
        /// </summary>
        private void btnConstant_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                if (btn.Text == "π")
                {
                    AppendToExpression("3.14159265358979");
                }
                else if (btn.Text == "e")
                {
                    AppendToExpression("2.71828182845905");
                }
            }
        }

        /// <summary>
        /// Calculate the expression
        /// </summary>
        private void btnCalculate_Click(object sender, EventArgs e)
        {
            string expression = txtExpression.Text.Trim();
            
            if (string.IsNullOrEmpty(expression))
            {
                lblResult.Text = "结果: (空表达式)";
                lblResult.ForeColor = Color.Red;
                return;
            }

            try
            {
                var expr = ExpressionBuilder.Instance.ParseToExpr(expression);
                string result = expr.CalcValue();
                lblResult.Text = "结果: " + result;
                lblResult.ForeColor = Color.Green;
            }
            catch (ParserException ex)
            {
                lblResult.Text = "错误: " + ex.Message;
                lblResult.ForeColor = Color.Red;
            }
            catch (Exception ex)
            {
                lblResult.Text = "错误: " + ex.Message;
                lblResult.ForeColor = Color.Red;
            }
        }

        /// <summary>
        /// Clear the expression textbox
        /// </summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtExpression.Text = "";
            lblResult.Text = "结果: ";
            lblResult.ForeColor = Color.Black;
            txtExpression.Focus();
        }

        /// <summary>
        /// Allow keyboard input in the textbox
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Allow Enter key to calculate
            if (keyData == Keys.Enter)
            {
                btnCalculate_Click(null, null);
                return true;
            }
            // Allow Escape key to clear
            if (keyData == Keys.Escape)
            {
                btnClear_Click(null, null);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}