using Evangelion01.Contracts.Models;
using System.Collections.Generic;

namespace Evangelion01.BackendApp.Functions.Predictions
{
    public class PredictionDto
    {
        public Dictionary<GradeSubject, double> Grades { get; set; }
        public string Predicted { get; set; }
    }
}
