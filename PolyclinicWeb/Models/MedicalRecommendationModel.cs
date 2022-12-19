using PolyclinicWeb.Classes;
using System.Collections.Generic;

namespace PolyclinicWeb.Models
{
    public class MedicalRecommendationModel
    {
        public List<MedicalRecommendation> MedicationRecommendationsRelease = new List<MedicalRecommendation>();
        public List<MedicalRecommendation> MedicationRecommendations = new List<MedicalRecommendation>();
    }
}
