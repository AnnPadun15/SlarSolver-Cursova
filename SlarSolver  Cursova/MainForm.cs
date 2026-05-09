using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace SlarSolver
{
    public class MainForm : Form
    {
        private int n = 2;
        private const double MinAbsValue = 1e-4;
        private const double MaxAbsValue = 1000;
        private TextBox[,] aBoxes;
        private TextBox[] bBoxes;

        private NumericUpDown numSize;
        private ComboBox cmbMethod;
        private Panel matrixPanel;
        private TextBox txtResult;
        private TextBox txtSteps;
        private Chart chart;
        private Label lblComplexity;

        private double[,] lastMatrix;
        private double[] lastSolution;
        private string lastMethod = "";

        public MainForm()
        {
            Text = "Курсова робота — розв’язання СЛАР точними методами";
            Size = new Size(1250, 760);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 10);
            BackColor = Color.FromArgb(245, 247, 250);

            BuildInterface();
            CreateMatrixInputs();
        }

        private void BuildInterface()
        {
            Label title = new Label
            {
                Text = "Розв’язання систем лінійних алгебраїчних рівнянь",
                Location = new Point(25, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 17, FontStyle.Bold),
                ForeColor = Color.FromArgb(35, 55, 90)
            };
            Controls.Add(title);

            GroupBox settingsBox = new GroupBox
            {
                Text = "Параметри обчислення",
                Location = new Point(25, 60),
                Size = new Size(1180, 90),
                BackColor = Color.White
            };
            Controls.Add(settingsBox);

            Label lblSize = new Label
            {
                Text = "Розмірність СЛАР:",
                Location = new Point(20, 38),
                AutoSize = true
            };
            settingsBox.Controls.Add(lblSize);

            numSize = new NumericUpDown
            {
                Location = new Point(155, 35),
                Minimum = 2,
                Maximum = 8,
                Value = 2,
                Width = 70
            };
            settingsBox.Controls.Add(numSize);

            Button btnCreate = new Button
            {
                Text = "Створити матрицю",
                Location = new Point(245, 31),
                Size = new Size(155, 35),
                BackColor = Color.FromArgb(220, 235, 255)
            };
            btnCreate.Click += BtnCreate_Click;
            settingsBox.Controls.Add(btnCreate);

            Label lblMethod = new Label
            {
                Text = "Метод:",
                Location = new Point(430, 38),
                AutoSize = true
            };
            settingsBox.Controls.Add(lblMethod);

            cmbMethod = new ComboBox
            {
                Location = new Point(490, 34),
                Width = 260,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbMethod.Items.Add("Класичний метод Гауса");
            cmbMethod.Items.Add("Метод Жордана-Гауса");
            cmbMethod.Items.Add("Матричний метод");
            cmbMethod.SelectedIndex = 0;
            settingsBox.Controls.Add(cmbMethod);

            Button btnSolve = new Button
            {
                Text = "Обчислити",
                Location = new Point(780, 31),
                Size = new Size(120, 35),
                BackColor = Color.FromArgb(160, 230, 170)
            };
            btnSolve.Click += BtnSolve_Click;
            settingsBox.Controls.Add(btnSolve);

            Button btnClear = new Button
            {
                Text = "Очистити",
                Location = new Point(915, 31),
                Size = new Size(110, 35),
                BackColor = Color.FromArgb(245, 225, 180)
            };
            btnClear.Click += BtnClear_Click;
            settingsBox.Controls.Add(btnClear);

            Button btnSave = new Button
            {
                Text = "Зберегти у файл",
                Location = new Point(1040, 31),
                Size = new Size(125, 35),
                BackColor = Color.FromArgb(225, 225, 245)
            };
            btnSave.Click += BtnSave_Click;
            settingsBox.Controls.Add(btnSave);

            GroupBox matrixBox = new GroupBox
            {
                Text = "Введення коефіцієнтів системи",
                Location = new Point(25, 160),
                Size = new Size(1180, 200),
                BackColor = Color.White
            };
            Controls.Add(matrixBox);

            matrixPanel = new Panel
            {
                Location = new Point(15, 25),
                Size = new Size(1150, 160),
                AutoScroll = true
            };
            matrixBox.Controls.Add(matrixPanel);

            GroupBox resultBox = new GroupBox
            {
                Text = "Результат",
                Location = new Point(25, 370),
                Size = new Size(370, 325),
                BackColor = Color.White
            };
            Controls.Add(resultBox);

            txtResult = new TextBox
            {
                Location = new Point(15, 25),
                Size = new Size(340, 230),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical
            };
            resultBox.Controls.Add(txtResult);

            lblComplexity = new Label
            {
                Text = "Практична складність: -",
                Location = new Point(15, 265),
                Size = new Size(340, 45),
                ForeColor = Color.FromArgb(80, 80, 80)
            };
            resultBox.Controls.Add(lblComplexity);

            GroupBox stepsBox = new GroupBox
            {
                Text = "Покроковий хід розв’язання",
                Location = new Point(410, 370),
                Size = new Size(390, 325),
                BackColor = Color.White
            };
            Controls.Add(stepsBox);

            txtSteps = new TextBox
            {
                Location = new Point(15, 25),
                Size = new Size(360, 285),
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                WordWrap = false
            };
            stepsBox.Controls.Add(txtSteps);

            GroupBox graphBox = new GroupBox
            {
                Text = "Графічне відображення для системи 2×2",
                Location = new Point(815, 370),
                Size = new Size(390, 325),
                BackColor = Color.White
            };
            Controls.Add(graphBox);

            chart = new Chart
            {
                Location = new Point(15, 25),
                Size = new Size(360, 285)
            };

            chart.ChartAreas.Add(new ChartArea("Area"));
            chart.Legends.Add(new Legend("Legend"));
            graphBox.Controls.Add(chart);
        }

        private void CreateMatrixInputs()
        {
            matrixPanel.Controls.Clear();
            aBoxes = new TextBox[n, n];
            bBoxes = new TextBox[n];

            int startX = 20;
            int startY = 20;
            int boxW = 55;
            int boxH = 28;
            int step = 110;

            for (int i = 0; i < n; i++)
            {
                int y = startY + i * 38;

                Label rowLabel = new Label
                {
                    Text = $"R{i + 1}:",
                    Location = new Point(startX, y + 4),
                    AutoSize = true,
                    ForeColor = Color.FromArgb(35, 55, 90),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };
                matrixPanel.Controls.Add(rowLabel);

                for (int j = 0; j < n; j++)
                {
                    int boxX = startX + 60 + j * step;

                    aBoxes[i, j] = new TextBox
                    {
                        Location = new Point(boxX, y),
                        Size = new Size(boxW, boxH),
                        Text = "0",
                        TextAlign = HorizontalAlignment.Center
                    };
                    aBoxes[i, j].TextChanged += TextBox_TextChanged;
                    matrixPanel.Controls.Add(aBoxes[i, j]);

                    Label xLabel = new Label
                    {
                        Text = $"x{j + 1}",
                        Location = new Point(boxX + boxW + 8, y + 5),
                        AutoSize = true
                    };
                    matrixPanel.Controls.Add(xLabel);

                    if (j < n - 1)
                    {
                        Label plus = new Label
                        {
                            Text = "+",
                            Location = new Point(boxX + boxW + 30, y + 5),
                            AutoSize = true
                        };
                        matrixPanel.Controls.Add(plus);
                    }
                }

                Label equal = new Label
                {
                    Text = "=",
                    Location = new Point(startX + 60 + n * step - 20, y + 5),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };
                matrixPanel.Controls.Add(equal);

                bBoxes[i] = new TextBox
                {
                    Location = new Point(equal.Right + 15, y),
                    Size = new Size(boxW, boxH),
                    Text = "0",
                    TextAlign = HorizontalAlignment.Center
                };
                bBoxes[i].TextChanged += TextBox_TextChanged;
                matrixPanel.Controls.Add(bBoxes[i]);
            }

            Label hint = new Label
            {
                Text = "Дозволено вводити цілі та дробові числа. Дробову частину можна писати через кому або крапку.",
                Location = new Point(20, startY + n * 38 + 12),
                AutoSize = true,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 9, FontStyle.Italic)
            };
            matrixPanel.Controls.Add(hint);
        }

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            n = (int)numSize.Value;
            CreateMatrixInputs();
            ClearOutput();
        }

        private void BtnSolve_Click(object sender, EventArgs e)
        {
            try
            {
                double[,] matrix = ReadMatrix();

                EquationSystem equationSystem = new EquationSystem(n);

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        equationSystem.SetCoefficient(i, j, matrix[i, j]);
                    }

                    equationSystem.SetConstant(i, matrix[i, n]);
                }

                lastMatrix = equationSystem.GetMatrixCopy();

                double det = Determinant(GetA(lastMatrix));

                if (Math.Abs(det) < 1e-10)
                    throw new InvalidOperationException("Матриця коефіцієнтів є виродженою. Система не має єдиного розв’язку або має безліч розв’язків.");

                StringBuilder steps = new StringBuilder();
                ISolver solver = SolverFactory.GetSolver(cmbMethod.SelectedIndex);

                steps.AppendLine($"Обраний метод: {solver.Name}");
                steps.AppendLine($"Розмірність системи: {n}×{n}");
                steps.AppendLine($"Визначник матриці коефіцієнтів det(A) = {det:F3}");
                steps.AppendLine();

                double[] solution = equationSystem.Solve(solver, steps);

                lastSolution = solution;
                lastMethod = solver.Name;

                txtResult.Text = $"Метод: {solver.Name}\r\n\r\n";
                txtResult.Text += "Розв’язок системи:\r\n";

                for (int i = 0; i < solution.Length; i++)
                    txtResult.Text += $"x{i + 1} = {solution[i]:F3}\r\n";

                txtResult.Text += "\r\nПеревірка:\r\n";
                txtResult.Text += CheckSolution(lastMatrix, solution);

                txtSteps.Text = steps.ToString();
                lblComplexity.Text = GetComplexityText(cmbMethod.SelectedIndex, n);

                if (n == 2)
                    DrawGraph(solution);
                else
                    chart.Series.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ClearOutput();
            }
        }

        private double[,] ReadMatrix()
        {
            double[,] matrix = new double[n, n + 1];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (!ValidateNumber(aBoxes[i, j].Text, out double value))
                        throw new InvalidOperationException($"Некоректне значення коефіцієнта A[{i + 1},{j + 1}].");

                    if (Math.Abs(value) > MaxAbsValue)
                        throw new InvalidOperationException($"Значення коефіцієнта A[{i + 1},{j + 1}] занадто велике. Введіть число в межах від -1000000 до 1000000.");

                    if (value != 0 && Math.Abs(value) < MinAbsValue)
                        throw new InvalidOperationException($"Значення коефіцієнта A[{i + 1},{j + 1}] занадто мале. Введіть число більше за 1e-10 або 0.");

                    matrix[i, j] = value;
                }

                if (!ValidateNumber(bBoxes[i].Text, out double b))
                    throw new InvalidOperationException($"Некоректне значення вільного члена b[{i + 1}].");

                if (Math.Abs(b) > MaxAbsValue)
                    throw new InvalidOperationException($"Значення вільного члена b[{i + 1}] занадто велике. Введіть число в межах від -1000000 до 1000000.");

                if (b != 0 && Math.Abs(b) < MinAbsValue)
                    throw new InvalidOperationException($"Значення вільного члена b[{i + 1}] занадто мале. Введіть число більше за 1e-10 або 0.");

                matrix[i, n] = b;
            }

            return matrix;
        }

        private bool ValidateNumber(string text, out double value)
        {
            value = 0;

            if (string.IsNullOrWhiteSpace(text))
                return false;

            text = text.Replace(",", ".");

            return double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox box = sender as TextBox;
            if (box == null) return;

            string text = box.Text;
            if (string.IsNullOrEmpty(text)) return;

            if (!Regex.IsMatch(text, @"^-?\d*[,.]?\d*$"))
            {
                int pos = Math.Max(0, box.SelectionStart - 1);
                box.Text = text.Remove(pos, 1);
                box.SelectionStart = Math.Min(pos, box.Text.Length);
            }
        }

        private string CheckSolution(double[,] matrix, double[] x)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < n; i++)
            {
                double left = 0;

                for (int j = 0; j < n; j++)
                    left += matrix[i, j] * x[j];

                double right = matrix[i, n];
                double error = Math.Abs(left - right);

                sb.AppendLine($"Рівняння {i + 1}: {left:F3} ≈ {right:F3}; похибка = {error:E2}");
            }

            return sb.ToString();
        }

        private string GetComplexityText(int methodIndex, int size)
        {
            int approxOperations;

            if (methodIndex == 0)
                approxOperations = size * size * size / 3;
            else if (methodIndex == 1)
                approxOperations = size * size * size;
            else
                approxOperations = 2 * size * size * size;


            string method;

            if (methodIndex == 0)
            {
                method = "O(n³), класичний метод Гауса";
            }
            else if (methodIndex == 1)
            {
                method = "O(n³), метод Жордана-Гауса";
            }
            else
            {
                method = "O(n³), матричний метод";
            }

            return $"Практична складність: {method}\r\nОрієнтовна кількість основних операцій для n={size}: {approxOperations}";
        }

        private double[,] GetA(double[,] matrix)
        {
            double[,] A = new double[n, n];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    A[i, j] = matrix[i, j];

            return A;
        }

        private double Determinant(double[,] A)
        {
            int size = A.GetLength(0);
            double[,] a = new double[size, size];

            Array.Copy(A, a, A.Length);

            double det = 1;

            for (int k = 0; k < size; k++)
            {
                int maxRow = k;

                for (int i = k + 1; i < size; i++)
                {
                    if (Math.Abs(a[i, k]) > Math.Abs(a[maxRow, k]))
                        maxRow = i;
                }

                if (Math.Abs(a[maxRow, k]) < 1e-10)
                    return 0;

                if (maxRow != k)
                {
                    SwapRows(a, k, maxRow);
                    det *= -1;
                }

                det *= a[k, k];

                for (int i = k + 1; i < size; i++)
                {
                    double factor = a[i, k] / a[k, k];

                    for (int j = k; j < size; j++)
                        a[i, j] -= factor * a[k, j];
                }
            }

            return det;
        }

        private void SwapRows(double[,] a, int r1, int r2)
        {
            int cols = a.GetLength(1);

            for (int j = 0; j < cols; j++)
            {
                double temp = a[r1, j];
                a[r1, j] = a[r2, j];
                a[r2, j] = temp;
            }
        }

        private void DrawGraph(double[] solution)
        {
            chart.Series.Clear();

            chart.ChartAreas[0].AxisX.Title = "x1";
            chart.ChartAreas[0].AxisY.Title = "x2";
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.LightGray;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.LightGray;

            double x0 = solution[0];
            double y0 = solution[1];
            double range = Math.Max(Math.Max(Math.Abs(x0), Math.Abs(y0)) + 5, 5);

            chart.ChartAreas[0].AxisX.Minimum = x0 - range;
            chart.ChartAreas[0].AxisX.Maximum = x0 + range;
            chart.ChartAreas[0].AxisY.Minimum = y0 - range;
            chart.ChartAreas[0].AxisY.Maximum = y0 + range;

            for (int i = 0; i < n; i++)
            {
                Series s = new Series($"Рівняння {i + 1}")
                {
                    ChartType = SeriesChartType.Line,
                    BorderWidth = 3
                };

                double a1 = lastMatrix[i, 0];
                double a2 = lastMatrix[i, 1];
                double b = lastMatrix[i, 2];

                if (Math.Abs(a2) > 1e-10)
                {
                    for (int p = 0; p <= 100; p++)
                    {
                        double x = x0 - range + 2 * range * p / 100;
                        double y = (b - a1 * x) / a2;
                        s.Points.AddXY(x, y);
                    }
                }
                else if (Math.Abs(a1) > 1e-10)
                {
                    double x = b / a1;
                    s.Points.AddXY(x, y0 - range);
                    s.Points.AddXY(x, y0 + range);
                }

                chart.Series.Add(s);
            }

            Series point = new Series("Розв’язок")
            {
                ChartType = SeriesChartType.Point,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 11,
                Color = Color.Red
            };

            point.Points.AddXY(x0, y0);
            point.Points[0].Label = $"({x0:F2}; {y0:F2})";

            chart.Series.Add(point);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtResult.Text))
            {
                MessageBox.Show("Спочатку виконайте обчислення.", "Увага", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt",
                Title = "Зберегти результат",
                FileName = "Slar_Result.txt"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(dialog.FileName, false, Encoding.UTF8))
                {
                    writer.WriteLine("Розв’язання системи лінійних алгебраїчних рівнянь");
                    writer.WriteLine($"Дата і час: {DateTime.Now:dd.MM.yyyy HH:mm:ss}");
                    writer.WriteLine($"Метод: {lastMethod}");
                    writer.WriteLine();

                    writer.WriteLine("Результат:");
                    writer.WriteLine(txtResult.Text);

                    writer.WriteLine();
                    writer.WriteLine("Покроковий хід розв’язання:");
                    writer.WriteLine(txtSteps.Text);

                    writer.WriteLine();
                    writer.WriteLine(lblComplexity.Text);
                }

                MessageBox.Show("Результати успішно збережено.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            CreateMatrixInputs();
            ClearOutput();
        }

        private void ClearOutput()
        {
            txtResult.Clear();
            txtSteps.Clear();
            chart.Series.Clear();
            lblComplexity.Text = "Практична складність: -";
            lastSolution = null;
            lastMatrix = null;
            lastMethod = "";
        }
    }
}
