using Accord;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClassificationErrorPossibility
{
    public partial class ErrorGraphicForm : Form
    {
        private Bitmap bitmap;
        private Graphics graphics;
        private Pen pen = new Pen(Color.Black);
        private Brush brush = new SolidBrush(Color.Black);

        private const double mean1 = 11; // 7 9 15 30 check, example of case when offered error estimating method is incorrect (probably)
        private const double mean2 = 22;
        private const double variance1 = 24;
        private const double variance2 = 30;
        private const double chance1 = 0.6; //class1AssignmentChance
        private const double chance2 = 1 - chance1; //class2AssignmentChance
        private const int classCount = 2;
        private const double visiblePartOfFirstDistribution = 0.999;
        private const int hatchStep = 10;

        private ClassificationErrorDemo classificationErrorDemo;

        public ErrorGraphicForm()
        {
            InitializeComponent();

            classificationErrorDemo = new ClassificationErrorDemo(mean1, variance1, mean2, variance2,
                chance1,
                chance2);
        }

        private void ErrorGraphicForm_Load(object sender, EventArgs e)
        {
            bitmap = new Bitmap(pb.Width, pb.Height);
            graphics = Graphics.FromImage(bitmap);

            DrawGraphics();
            classificationErrorDemo.FindClassificationErrorFullPossibility();

            Form testDialog = new Form
            {
                Size = new Size(0, 0),
                AutoSize = true,
                FormBorderStyle = FormBorderStyle.FixedSingle,
                ControlBox = false,
            };
            Label answer = new Label
            {
                Text = String.Format("Error assignment to class 1 chance: {0}{1}Error not assigned to class 1 chance:{2}",
                    classificationErrorDemo.Class1ErrorAssignmentChance,
                    Environment.NewLine,
                    classificationErrorDemo.Class1ErrorNotAssignedChance),
                AutoSize = true,
                Font = new Font("Consolas", 20),
                Parent = testDialog
            };
            testDialog.Show();

            pb.Image = bitmap;
            pb.Invalidate();
        }

        private void DrawGraphics()
        {
            double maxDens = EvaluateMaxDens();

            DoubleRange range = classificationErrorDemo.
                NormalDistributionClass1.
                GetRange(visiblePartOfFirstDistribution);
            double xScale = range.Length / pb.Width;

            PointF[] dots1 = new PointF[pb.Width];
            PointF[] dots2 = new PointF[pb.Width];

            for (int i = 0; i < dots1.Length; i++)
            {
                double dens1 = classificationErrorDemo.
                    NormalDistributionClass1.
                    ProbabilityDensityFunction(range.Min + i * xScale)
                    * chance1;

                double dens2 = classificationErrorDemo.
                        NormalDistributionClass2.
                        ProbabilityDensityFunction(range.Min + i * xScale)
                        * chance2;

                dots1[i] = new PointF(i, pb.Height * (float)(1 - dens1 / maxDens));

                dots2[i] = new PointF(i, pb.Height * (float)(1 - dens2 / maxDens));

                if (i % hatchStep == 0)
                {
                    pen.Color = dens1 > dens2 ? Color.Orange : Color.Red;

                    PointF start = new PointF(i, pb.Height - 0);
                    PointF end = new PointF(i, Math.Max(dots1[i].Y, dots2[i].Y));

                    graphics.DrawLine(pen, start, end);
                }
            }

            void DrawSeparationLine()
            {
                double x = (classificationErrorDemo.FindDistributionEqualityPoint() - range.Min) / xScale; //separationLineX classificationErrorDemo.FindDistributionEqualityPoint() -

                Pen dashPen = new Pen(Color.Black, 1f)
                {
                    DashStyle = System.Drawing.Drawing2D.DashStyle.DashDotDot
                };

                graphics.DrawLine(dashPen,
                    new PointF((float)x, pb.Height),
                    new PointF((float)x, 0));
            }

            DrawSeparationLine();

            graphics.DrawCurve(new Pen(Color.Red, 2f), dots1);
            graphics.DrawCurve(new Pen(Color.Orange, 2f), dots2);

            DrawSeparationLine();
        }

        private double EvaluateMaxDens()
        {
            double mode1 = classificationErrorDemo.NormalDistributionClass1.Mode;
            double mostAscendValue1 = classificationErrorDemo.
                NormalDistributionClass1.
                ProbabilityDensityFunction(mode1);

            double mode2 = classificationErrorDemo.NormalDistributionClass2.Mode;
            double mostAscendValue2 = classificationErrorDemo.
                NormalDistributionClass2.
                ProbabilityDensityFunction(mode2);

            double maxDens = Math.Max(mostAscendValue1, mostAscendValue2);
            maxDens = mostAscendValue1 * chance1;

            return maxDens;
        }
    }
}