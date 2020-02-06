using Accord;
using Accord.Statistics.Distributions.Univariate;
using MathNet.Numerics.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ClassificationErrorPossibility
{
    internal class ClassificationErrorDemo : IErrorPossibilityDemo
    {
        public NormalDistribution NormalDistributionClass1 { get; private set; }
        public NormalDistribution NormalDistributionClass2 { get; private set; }
        public double Mean1 { get; }
        public double Variance1 { get; }
        public double Mean2 { get; }
        public double Variance2 { get; }
        public double Chance1 { get; } //class 1 asignment chance
        public double Chance2 { get; }
        public double Class1ErrorAssignmentChance { get; private set; }
        public double Class1ErrorNotAssignedChance { get; private set; }

        //NormalDistribution of class 1 and 2 parametrs are entered
        public ClassificationErrorDemo(double mean1, double variance1, double mean2, double variance2,
            double chance1,
            double chance2)
        {
            NormalDistributionClass1 = new NormalDistribution(mean1, Math.Sqrt(variance1));
            NormalDistributionClass2 = new NormalDistribution(mean2, Math.Sqrt(variance2));
            Mean1 = mean1;
            Variance1 = variance1;
            Mean2 = mean2;
            Variance2 = variance2;
            Chance1 = chance1;
            Chance2 = chance2;
        }

        public void FindClassificationErrorFullPossibility()
        {
            double step = 0.00001;

            Class1ErrorAssignmentChance = 0;
            Class1ErrorNotAssignedChance = 0;

            DoubleRange range1 = NormalDistributionClass1.GetRange(0.99);
            DoubleRange range2 = NormalDistributionClass2.GetRange(0.99);
            double begin = range2.Min;
            double end = FindDistributionEqualityPoint();

            double f1(double x) => Chance2 / Math.Sqrt(Variance2 * 2 * Math.PI)
                * Math.Exp(Math.Pow((x - Mean2), 2) / (-2 * Variance2));
            Class1ErrorAssignmentChance = DoubleExponentialTransformation.Integrate(f1, begin, end, 1e-5);

            begin = end;
            end = range1.Max;
            double f2(double x) => Chance1 / Math.Sqrt(Variance1 * 2 * Math.PI)
                * Math.Exp(Math.Pow((x - Mean1), 2) / (-2 * Variance1));
            Class1ErrorNotAssignedChance = DoubleExponentialTransformation.Integrate(f2, begin, end, 1e-5);

            //double x = Math.Min(range1.Min, range2.Min);
            //double max = Math.Max(range2.Max, range1.Max);

            //bool equalityPointPassed = false;
            //while (x < max)
            //{
            //    double dens1 = NormalDistributionClass1.ProbabilityDensityFunction(x);
            //    double dens2 = NormalDistributionClass2.ProbabilityDensityFunction(x);

            //    if (!equalityPointPassed)
            //        Class1ErrorAssignmentChance += dens2 * chance2;
            //    else
            //        Class1ErrorNotAssignedChance += dens1 * chance1;

            //    if (Math.Abs(dens1 - dens2) < 0.00003)
            //        equalityPointPassed = true;

            //    x += step;
            //}
        }

        public double FindDistributionEqualityPoint(double step = 0.0001, double accuracy = 0.0001)
        {
            DoubleRange range1 = NormalDistributionClass1.GetRange(0.99);
            DoubleRange range2 = NormalDistributionClass2.GetRange(0.99);

            double x = Math.Min(range1.Min, range2.Min);
            double max = Math.Max(range2.Max, range1.Max);

            while (x < max)
            {
                double dens1 = NormalDistributionClass1.ProbabilityDensityFunction(x) * Chance1;
                double dens2 = NormalDistributionClass2.ProbabilityDensityFunction(x) * Chance2;

                if (Math.Abs(dens1 - dens2) < accuracy)
                    return x;

                x += step;
            }

            throw new InvalidOperationException();
        }
    }
}