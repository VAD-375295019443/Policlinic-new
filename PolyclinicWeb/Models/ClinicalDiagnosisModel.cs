using PolyclinicWeb.Classes;
using System.Collections.Generic;

namespace PolyclinicWeb.Models
{
    public class ClinicalDiagnosisModel
    {
        public List<ClinicalDiagnosis> ClinicalDiagnosesRegistration = new List<ClinicalDiagnosis>();
        public List<ClinicalDiagnosis> ClinicalDiagnosesDeregistration = new List<ClinicalDiagnosis>();
    }
}