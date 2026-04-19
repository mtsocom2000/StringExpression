namespace WindowsFormsApp1
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.txtExpression = new System.Windows.Forms.TextBox();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblResult = new System.Windows.Forms.Label();
            this.panelNumbers = new System.Windows.Forms.Panel();
            this.panelOperators = new System.Windows.Forms.Panel();
            this.panelFunctions = new System.Windows.Forms.Panel();
            this.btn0 = new System.Windows.Forms.Button();
            this.btn1 = new System.Windows.Forms.Button();
            this.btn2 = new System.Windows.Forms.Button();
            this.btn3 = new System.Windows.Forms.Button();
            this.btn4 = new System.Windows.Forms.Button();
            this.btn5 = new System.Windows.Forms.Button();
            this.btn6 = new System.Windows.Forms.Button();
            this.btn7 = new System.Windows.Forms.Button();
            this.btn8 = new System.Windows.Forms.Button();
            this.btn9 = new System.Windows.Forms.Button();
            this.btnDot = new System.Windows.Forms.Button();
            this.btnPlus = new System.Windows.Forms.Button();
            this.btnMinus = new System.Windows.Forms.Button();
            this.btnMultiply = new System.Windows.Forms.Button();
            this.btnDivide = new System.Windows.Forms.Button();
            this.btnLeftParen = new System.Windows.Forms.Button();
            this.btnRightParen = new System.Windows.Forms.Button();
            this.btnSin = new System.Windows.Forms.Button();
            this.btnCos = new System.Windows.Forms.Button();
            this.btnTan = new System.Windows.Forms.Button();
            this.btnSqrt = new System.Windows.Forms.Button();
            this.btnLog = new System.Windows.Forms.Button();
            this.btnLog10 = new System.Windows.Forms.Button();
            this.btnLn = new System.Windows.Forms.Button();
            this.btnAbs = new System.Windows.Forms.Button();
            this.btnPow = new System.Windows.Forms.Button();
            this.btnExp = new System.Windows.Forms.Button();
            this.btnAsin = new System.Windows.Forms.Button();
            this.btnAcos = new System.Windows.Forms.Button();
            this.btnAtan = new System.Windows.Forms.Button();
            this.btnPi = new System.Windows.Forms.Button();
            this.btnE = new System.Windows.Forms.Button();
            this.lblFunctions = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // txtExpression
            //
            this.txtExpression.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtExpression.Location = new System.Drawing.Point(12, 12);
            this.txtExpression.Name = "txtExpression";
            this.txtExpression.Size = new System.Drawing.Size(560, 30);
            this.txtExpression.TabIndex = 0;
            //
            // btnCalculate
            //
            this.btnCalculate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnCalculate.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCalculate.ForeColor = System.Drawing.Color.White;
            this.btnCalculate.Location = new System.Drawing.Point(478, 48);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(94, 40);
            this.btnCalculate.TabIndex = 1;
            this.btnCalculate.Text = "计算";
            this.btnCalculate.UseVisualStyleBackColor = false;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            //
            // btnClear
            //
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnClear.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Location = new System.Drawing.Point(378, 48);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(94, 40);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "清空";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            //
            // lblResult
            //
            this.lblResult.BackColor = System.Drawing.Color.LightGray;
            this.lblResult.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblResult.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblResult.Location = new System.Drawing.Point(12, 48);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(360, 40);
            this.lblResult.TabIndex = 3;
            this.lblResult.Text = "结果: ";
            this.lblResult.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // panelNumbers
            //
            this.panelNumbers.Controls.Add(this.btnDot);
            this.panelNumbers.Controls.Add(this.btn0);
            this.panelNumbers.Controls.Add(this.btn9);
            this.panelNumbers.Controls.Add(this.btn8);
            this.panelNumbers.Controls.Add(this.btn7);
            this.panelNumbers.Controls.Add(this.btn6);
            this.panelNumbers.Controls.Add(this.btn5);
            this.panelNumbers.Controls.Add(this.btn4);
            this.panelNumbers.Controls.Add(this.btn3);
            this.panelNumbers.Controls.Add(this.btn2);
            this.panelNumbers.Controls.Add(this.btn1);
            this.panelNumbers.Location = new System.Drawing.Point(12, 95);
            this.panelNumbers.Name = "panelNumbers";
            this.panelNumbers.Size = new System.Drawing.Size(180, 180);
            this.panelNumbers.TabIndex = 4;
            //
            // btn0
            //
            this.btn0.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn0.Location = new System.Drawing.Point(60, 150);
            this.btn0.Name = "btn0";
            this.btn0.Size = new System.Drawing.Size(55, 25);
            this.btn0.TabIndex = 10;
            this.btn0.Text = "0";
            this.btn0.UseVisualStyleBackColor = true;
            this.btn0.Click += new System.EventHandler(this.btnNumber_Click);
            //
            // btn1
            //
            this.btn1.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn1.Location = new System.Drawing.Point(0, 0);
            this.btn1.Name = "btn1";
            this.btn1.Size = new System.Drawing.Size(55, 25);
            this.btn1.TabIndex = 1;
            this.btn1.Text = "1";
            this.btn1.UseVisualStyleBackColor = true;
            this.btn1.Click += new System.EventHandler(this.btnNumber_Click);
            //
            // btn2
            //
            this.btn2.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn2.Location = new System.Drawing.Point(60, 0);
            this.btn2.Name = "btn2";
            this.btn2.Size = new System.Drawing.Size(55, 25);
            this.btn2.TabIndex = 2;
            this.btn2.Text = "2";
            this.btn2.UseVisualStyleBackColor = true;
            this.btn2.Click += new System.EventHandler(this.btnNumber_Click);
            //
            // btn3
            //
            this.btn3.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn3.Location = new System.Drawing.Point(120, 0);
            this.btn3.Name = "btn3";
            this.btn3.Size = new System.Drawing.Size(55, 25);
            this.btn3.TabIndex = 3;
            this.btn3.Text = "3";
            this.btn3.UseVisualStyleBackColor = true;
            this.btn3.Click += new System.EventHandler(this.btnNumber_Click);
            //
            // btn4
            //
            this.btn4.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn4.Location = new System.Drawing.Point(0, 30);
            this.btn4.Name = "btn4";
            this.btn4.Size = new System.Drawing.Size(55, 25);
            this.btn4.TabIndex = 4;
            this.btn4.Text = "4";
            this.btn4.UseVisualStyleBackColor = true;
            this.btn4.Click += new System.EventHandler(this.btnNumber_Click);
            //
            // btn5
            //
            this.btn5.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn5.Location = new System.Drawing.Point(60, 30);
            this.btn5.Name = "btn5";
            this.btn5.Size = new System.Drawing.Size(55, 25);
            this.btn5.TabIndex = 5;
            this.btn5.Text = "5";
            this.btn5.UseVisualStyleBackColor = true;
            this.btn5.Click += new System.EventHandler(this.btnNumber_Click);
            //
            // btn6
            //
            this.btn6.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn6.Location = new System.Drawing.Point(120, 30);
            this.btn6.Name = "btn6";
            this.btn6.Size = new System.Drawing.Size(55, 25);
            this.btn6.TabIndex = 6;
            this.btn6.Text = "6";
            this.btn6.UseVisualStyleBackColor = true;
            this.btn6.Click += new System.EventHandler(this.btnNumber_Click);
            //
            // btn7
            //
            this.btn7.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn7.Location = new System.Drawing.Point(0, 60);
            this.btn7.Name = "btn7";
            this.btn7.Size = new System.Drawing.Size(55, 25);
            this.btn7.TabIndex = 7;
            this.btn7.Text = "7";
            this.btn7.UseVisualStyleBackColor = true;
            this.btn7.Click += new System.EventHandler(this.btnNumber_Click);
            //
            // btn8
            //
            this.btn8.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn8.Location = new System.Drawing.Point(60, 60);
            this.btn8.Name = "btn8";
            this.btn8.Size = new System.Drawing.Size(55, 25);
            this.btn8.TabIndex = 8;
            this.btn8.Text = "8";
            this.btn8.UseVisualStyleBackColor = true;
            this.btn8.Click += new System.EventHandler(this.btnNumber_Click);
            //
            // btn9
            //
            this.btn9.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btn9.Location = new System.Drawing.Point(120, 60);
            this.btn9.Name = "btn9";
            this.btn9.Size = new System.Drawing.Size(55, 25);
            this.btn9.TabIndex = 9;
            this.btn9.Text = "9";
            this.btn9.UseVisualStyleBackColor = true;
            this.btn9.Click += new System.EventHandler(this.btnNumber_Click);
            //
            // btnDot
            //
            this.btnDot.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDot.Location = new System.Drawing.Point(120, 150);
            this.btnDot.Name = "btnDot";
            this.btnDot.Size = new System.Drawing.Size(55, 25);
            this.btnDot.TabIndex = 11;
            this.btnDot.Text = ".";
            this.btnDot.UseVisualStyleBackColor = true;
            this.btnDot.Click += new System.EventHandler(this.btnNumber_Click);
            //
            // panelOperators
            //
            this.panelOperators.Controls.Add(this.btnRightParen);
            this.panelOperators.Controls.Add(this.btnLeftParen);
            this.panelOperators.Controls.Add(this.btnDivide);
            this.panelOperators.Controls.Add(this.btnMultiply);
            this.panelOperators.Controls.Add(this.btnMinus);
            this.panelOperators.Controls.Add(this.btnPlus);
            this.panelOperators.Location = new System.Drawing.Point(200, 95);
            this.panelOperators.Name = "panelOperators";
            this.panelOperators.Size = new System.Drawing.Size(180, 180);
            this.panelOperators.TabIndex = 5;
            //
            // btnPlus
            //
            this.btnPlus.BackColor = System.Drawing.Color.LightBlue;
            this.btnPlus.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPlus.Location = new System.Drawing.Point(0, 0);
            this.btnPlus.Name = "btnPlus";
            this.btnPlus.Size = new System.Drawing.Size(55, 25);
            this.btnPlus.TabIndex = 12;
            this.btnPlus.Text = "+";
            this.btnPlus.UseVisualStyleBackColor = false;
            this.btnPlus.Click += new System.EventHandler(this.btnOperator_Click);
            //
            // btnMinus
            //
            this.btnMinus.BackColor = System.Drawing.Color.LightBlue;
            this.btnMinus.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnMinus.Location = new System.Drawing.Point(60, 0);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(55, 25);
            this.btnMinus.TabIndex = 13;
            this.btnMinus.Text = "-";
            this.btnMinus.UseVisualStyleBackColor = false;
            this.btnMinus.Click += new System.EventHandler(this.btnOperator_Click);
            //
            // btnMultiply
            //
            this.btnMultiply.BackColor = System.Drawing.Color.LightBlue;
            this.btnMultiply.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnMultiply.Location = new System.Drawing.Point(120, 0);
            this.btnMultiply.Name = "btnMultiply";
            this.btnMultiply.Size = new System.Drawing.Size(55, 25);
            this.btnMultiply.TabIndex = 14;
            this.btnMultiply.Text = "*";
            this.btnMultiply.UseVisualStyleBackColor = false;
            this.btnMultiply.Click += new System.EventHandler(this.btnOperator_Click);
            //
            // btnDivide
            //
            this.btnDivide.BackColor = System.Drawing.Color.LightBlue;
            this.btnDivide.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnDivide.Location = new System.Drawing.Point(0, 30);
            this.btnDivide.Name = "btnDivide";
            this.btnDivide.Size = new System.Drawing.Size(55, 25);
            this.btnDivide.TabIndex = 15;
            this.btnDivide.Text = "/";
            this.btnDivide.UseVisualStyleBackColor = false;
            this.btnDivide.Click += new System.EventHandler(this.btnOperator_Click);
            //
            // btnLeftParen
            //
            this.btnLeftParen.BackColor = System.Drawing.Color.LightYellow;
            this.btnLeftParen.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnLeftParen.Location = new System.Drawing.Point(60, 30);
            this.btnLeftParen.Name = "btnLeftParen";
            this.btnLeftParen.Size = new System.Drawing.Size(55, 25);
            this.btnLeftParen.TabIndex = 16;
            this.btnLeftParen.Text = "(";
            this.btnLeftParen.UseVisualStyleBackColor = false;
            this.btnLeftParen.Click += new System.EventHandler(this.btnOperator_Click);
            //
            // btnRightParen
            //
            this.btnRightParen.BackColor = System.Drawing.Color.LightYellow;
            this.btnRightParen.Font = new System.Drawing.Font("Microsoft YaHei UI", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnRightParen.Location = new System.Drawing.Point(120, 30);
            this.btnRightParen.Name = "btnRightParen";
            this.btnRightParen.Size = new System.Drawing.Size(55, 25);
            this.btnRightParen.TabIndex = 17;
            this.btnRightParen.Text = ")";
            this.btnRightParen.UseVisualStyleBackColor = false;
            this.btnRightParen.Click += new System.EventHandler(this.btnOperator_Click);
            //
            // panelFunctions
            //
            this.panelFunctions.Controls.Add(this.btnE);
            this.panelFunctions.Controls.Add(this.btnPi);
            this.panelFunctions.Controls.Add(this.btnAtan);
            this.panelFunctions.Controls.Add(this.btnAcos);
            this.panelFunctions.Controls.Add(this.btnAsin);
            this.panelFunctions.Controls.Add(this.btnExp);
            this.panelFunctions.Controls.Add(this.btnPow);
            this.panelFunctions.Controls.Add(this.btnAbs);
            this.panelFunctions.Controls.Add(this.btnLn);
            this.panelFunctions.Controls.Add(this.btnLog10);
            this.panelFunctions.Controls.Add(this.btnLog);
            this.panelFunctions.Controls.Add(this.btnSqrt);
            this.panelFunctions.Controls.Add(this.btnTan);
            this.panelFunctions.Controls.Add(this.btnCos);
            this.panelFunctions.Controls.Add(this.btnSin);
            this.panelFunctions.Location = new System.Drawing.Point(12, 280);
            this.panelFunctions.Name = "panelFunctions";
            this.panelFunctions.Size = new System.Drawing.Size(560, 140);
            this.panelFunctions.TabIndex = 6;
            //
            // btnSin
            //
            this.btnSin.BackColor = System.Drawing.Color.LightGreen;
            this.btnSin.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSin.Location = new System.Drawing.Point(0, 0);
            this.btnSin.Name = "btnSin";
            this.btnSin.Size = new System.Drawing.Size(65, 25);
            this.btnSin.TabIndex = 18;
            this.btnSin.Text = "sin";
            this.btnSin.UseVisualStyleBackColor = false;
            this.btnSin.Click += new System.EventHandler(this.btnFunction_Click);
            //
            // btnCos
            //
            this.btnCos.BackColor = System.Drawing.Color.LightGreen;
            this.btnCos.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCos.Location = new System.Drawing.Point(70, 0);
            this.btnCos.Name = "btnCos";
            this.btnCos.Size = new System.Drawing.Size(65, 25);
            this.btnCos.TabIndex = 19;
            this.btnCos.Text = "cos";
            this.btnCos.UseVisualStyleBackColor = false;
            this.btnCos.Click += new System.EventHandler(this.btnFunction_Click);
            //
            // btnTan
            //
            this.btnTan.BackColor = System.Drawing.Color.LightGreen;
            this.btnTan.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnTan.Location = new System.Drawing.Point(140, 0);
            this.btnTan.Name = "btnTan";
            this.btnTan.Size = new System.Drawing.Size(65, 25);
            this.btnTan.TabIndex = 20;
            this.btnTan.Text = "tan";
            this.btnTan.UseVisualStyleBackColor = false;
            this.btnTan.Click += new System.EventHandler(this.btnFunction_Click);
            //
            // btnSqrt
            //
            this.btnSqrt.BackColor = System.Drawing.Color.LightSalmon;
            this.btnSqrt.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnSqrt.Location = new System.Drawing.Point(210, 0);
            this.btnSqrt.Name = "btnSqrt";
            this.btnSqrt.Size = new System.Drawing.Size(65, 25);
            this.btnSqrt.TabIndex = 21;
            this.btnSqrt.Text = "sqrt";
            this.btnSqrt.UseVisualStyleBackColor = false;
            this.btnSqrt.Click += new System.EventHandler(this.btnFunction_Click);
            //
            // btnLog
            //
            this.btnLog.BackColor = System.Drawing.Color.LightSalmon;
            this.btnLog.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnLog.Location = new System.Drawing.Point(280, 0);
            this.btnLog.Name = "btnLog";
            this.btnLog.Size = new System.Drawing.Size(65, 25);
            this.btnLog.TabIndex = 22;
            this.btnLog.Text = "log";
            this.btnLog.UseVisualStyleBackColor = false;
            this.btnLog.Click += new System.EventHandler(this.btnFunction_Click);
            //
            // btnLog10
            //
            this.btnLog10.BackColor = System.Drawing.Color.LightSalmon;
            this.btnLog10.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnLog10.Location = new System.Drawing.Point(350, 0);
            this.btnLog10.Name = "btnLog10";
            this.btnLog10.Size = new System.Drawing.Size(65, 25);
            this.btnLog10.TabIndex = 23;
            this.btnLog10.Text = "log10";
            this.btnLog10.UseVisualStyleBackColor = false;
            this.btnLog10.Click += new System.EventHandler(this.btnFunction_Click);
            //
            // btnLn
            //
            this.btnLn.BackColor = System.Drawing.Color.LightSalmon;
            this.btnLn.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnLn.Location = new System.Drawing.Point(420, 0);
            this.btnLn.Name = "btnLn";
            this.btnLn.Size = new System.Drawing.Size(65, 25);
            this.btnLn.TabIndex = 24;
            this.btnLn.Text = "ln";
            this.btnLn.UseVisualStyleBackColor = false;
            this.btnLn.Click += new System.EventHandler(this.btnFunction_Click);
            //
            // btnAbs
            //
            this.btnAbs.BackColor = System.Drawing.Color.LightCoral;
            this.btnAbs.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAbs.Location = new System.Drawing.Point(495, 0);
            this.btnAbs.Name = "btnAbs";
            this.btnAbs.Size = new System.Drawing.Size(60, 25);
            this.btnAbs.TabIndex = 25;
            this.btnAbs.Text = "abs";
            this.btnAbs.UseVisualStyleBackColor = false;
            this.btnAbs.Click += new System.EventHandler(this.btnFunction_Click);
            //
            // btnPow
            //
            this.btnPow.BackColor = System.Drawing.Color.Plum;
            this.btnPow.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPow.Location = new System.Drawing.Point(0, 30);
            this.btnPow.Name = "btnPow";
            this.btnPow.Size = new System.Drawing.Size(65, 25);
            this.btnPow.TabIndex = 26;
            this.btnPow.Text = "pow";
            this.btnPow.UseVisualStyleBackColor = false;
            this.btnPow.Click += new System.EventHandler(this.btnFunction_Click);
            //
            // btnExp
            //
            this.btnExp.BackColor = System.Drawing.Color.Plum;
            this.btnExp.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnExp.Location = new System.Drawing.Point(70, 30);
            this.btnExp.Name = "btnExp";
            this.btnExp.Size = new System.Drawing.Size(65, 25);
            this.btnExp.TabIndex = 27;
            this.btnExp.Text = "exp";
            this.btnExp.UseVisualStyleBackColor = false;
            this.btnExp.Click += new System.EventHandler(this.btnFunction_Click);
            //
            // btnAsin
            //
            this.btnAsin.BackColor = System.Drawing.Color.LightGreen;
            this.btnAsin.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAsin.Location = new System.Drawing.Point(140, 30);
            this.btnAsin.Name = "btnAsin";
            this.btnAsin.Size = new System.Drawing.Size(65, 25);
            this.btnAsin.TabIndex = 28;
            this.btnAsin.Text = "asin";
            this.btnAsin.UseVisualStyleBackColor = false;
            this.btnAsin.Click += new System.EventHandler(this.btnFunction_Click);
            //
            // btnAcos
            //
            this.btnAcos.BackColor = System.Drawing.Color.LightGreen;
            this.btnAcos.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAcos.Location = new System.Drawing.Point(210, 30);
            this.btnAcos.Name = "btnAcos";
            this.btnAcos.Size = new System.Drawing.Size(65, 25);
            this.btnAcos.TabIndex = 29;
            this.btnAcos.Text = "acos";
            this.btnAcos.UseVisualStyleBackColor = false;
            this.btnAcos.Click += new System.EventHandler(this.btnFunction_Click);
            //
            // btnAtan
            //
            this.btnAtan.BackColor = System.Drawing.Color.LightGreen;
            this.btnAtan.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnAtan.Location = new System.Drawing.Point(280, 30);
            this.btnAtan.Name = "btnAtan";
            this.btnAtan.Size = new System.Drawing.Size(65, 25);
            this.btnAtan.TabIndex = 30;
            this.btnAtan.Text = "atan";
            this.btnAtan.UseVisualStyleBackColor = false;
            this.btnAtan.Click += new System.EventHandler(this.btnFunction_Click);
            //
            // btnPi
            //
            this.btnPi.BackColor = System.Drawing.Color.Khaki;
            this.btnPi.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPi.Location = new System.Drawing.Point(350, 30);
            this.btnPi.Name = "btnPi";
            this.btnPi.Size = new System.Drawing.Size(65, 25);
            this.btnPi.TabIndex = 31;
            this.btnPi.Text = "π";
            this.btnPi.UseVisualStyleBackColor = false;
            this.btnPi.Click += new System.EventHandler(this.btnConstant_Click);
            //
            // btnE
            //
            this.btnE.BackColor = System.Drawing.Color.Khaki;
            this.btnE.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnE.Location = new System.Drawing.Point(420, 30);
            this.btnE.Name = "btnE";
            this.btnE.Size = new System.Drawing.Size(65, 25);
            this.btnE.TabIndex = 32;
            this.btnE.Text = "e";
            this.btnE.UseVisualStyleBackColor = false;
            this.btnE.Click += new System.EventHandler(this.btnConstant_Click);
            //
            // lblFunctions
            //
            this.lblFunctions.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblFunctions.Location = new System.Drawing.Point(12, 260);
            this.lblFunctions.Name = "lblFunctions";
            this.lblFunctions.Size = new System.Drawing.Size(100, 20);
            this.lblFunctions.TabIndex = 7;
            this.lblFunctions.Text = "函数面板:";
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 431);
            this.Controls.Add(this.lblFunctions);
            this.Controls.Add(this.panelFunctions);
            this.Controls.Add(this.panelOperators);
            this.Controls.Add(this.panelNumbers);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnCalculate);
            this.Controls.Add(this.txtExpression);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "表达式计算器";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtExpression;
        private System.Windows.Forms.Button btnCalculate;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Panel panelNumbers;
        private System.Windows.Forms.Panel panelOperators;
        private System.Windows.Forms.Panel panelFunctions;
        private System.Windows.Forms.Button btn0;
        private System.Windows.Forms.Button btn1;
        private System.Windows.Forms.Button btn2;
        private System.Windows.Forms.Button btn3;
        private System.Windows.Forms.Button btn4;
        private System.Windows.Forms.Button btn5;
        private System.Windows.Forms.Button btn6;
        private System.Windows.Forms.Button btn7;
        private System.Windows.Forms.Button btn8;
        private System.Windows.Forms.Button btn9;
        private System.Windows.Forms.Button btnDot;
        private System.Windows.Forms.Button btnPlus;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.Button btnMultiply;
        private System.Windows.Forms.Button btnDivide;
        private System.Windows.Forms.Button btnLeftParen;
        private System.Windows.Forms.Button btnRightParen;
        private System.Windows.Forms.Button btnSin;
        private System.Windows.Forms.Button btnCos;
        private System.Windows.Forms.Button btnTan;
        private System.Windows.Forms.Button btnSqrt;
        private System.Windows.Forms.Button btnLog;
        private System.Windows.Forms.Button btnLog10;
        private System.Windows.Forms.Button btnLn;
        private System.Windows.Forms.Button btnAbs;
        private System.Windows.Forms.Button btnPow;
        private System.Windows.Forms.Button btnExp;
        private System.Windows.Forms.Button btnAsin;
        private System.Windows.Forms.Button btnAcos;
        private System.Windows.Forms.Button btnAtan;
        private System.Windows.Forms.Button btnPi;
        private System.Windows.Forms.Button btnE;
        private System.Windows.Forms.Label lblFunctions;
    }
}