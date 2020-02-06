using Accord.Statistics.Distributions.Univariate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ClassificationErrorPossibility
{
    internal interface IErrorPossibilityDemo
    {
        NormalDistribution NormalDistributionClass1 { get; }
        NormalDistribution NormalDistributionClass2 { get; }

        double Class1ErrorAssignmentChance { get; }
        double Class1ErrorNotAssignedChance { get; }

        double FindDistributionEqualityPoint(double step, double accuracy);

        void FindClassificationErrorFullPossibility();
    }
}