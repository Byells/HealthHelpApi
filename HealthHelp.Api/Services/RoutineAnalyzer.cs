using HealthHelp.Api.Data;
using HealthHelp.Api.Dtos;
using HealthHelp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthHelp.Api.Services
{
    
    public class ScoringConfig
    {
        public double SleepWeight { get; set; } = 0.30; 
        public double ActivityWeight { get; set; } = 0.20;
        public double WorkWeight { get; set; } = 0.25; 
        public double LeisureWeight { get; set; } = 0.25;

        public double IdealMinSleep { get; set; } = 7.0;
        public double IdealMaxSleep { get; set; } = 9.0;
    }

    
    public class DomainResult
    {
        public string Domain { get; set; } = string.Empty;
        public double Score { get; set; }
        public List<string> Recommendations { get; set; } = new List<string>();
    }

  
    public class RoutineFeatures
    {
        public double AvgSleepHours { get; set; }
        public double AvgWorkHours { get; set; }
        public double AvgLeisureHours { get; set; }
        public double AvgExerciseHours { get; set; }
        public double DaysLogged { get; set; }
    }

    
    public interface IRoutineAnalyzer
    {
        Task<AnalysisDto> AnalyzeForUserAsync(string userId);
    }

   
    public class RoutineAnalyzer : IRoutineAnalyzer
    {
        private readonly ApplicationDbContext _context;
        private readonly ScoringConfig _config;

        public RoutineAnalyzer(ApplicationDbContext context, ScoringConfig config)
        {
            _context = context;
            _config = config;
        }

        public async Task<AnalysisDto> AnalyzeForUserAsync(string userId)
        {
            
            var entries = await _context.RoutineEntries
                .Where(e => e.ApplicationUserId == userId)
                .OrderByDescending(e => e.EntryDate)
                .Take(9)
                .ToListAsync();

            if (!entries.Any())
            {
                return new AnalysisDto
                {
                    Score = 0,
                    Category = "Sem dados",
                    Summary = "Ainda não temos dados suficientes para uma análise precisa.",
                    Tips = new List<string> { "Registre sua rotina (sono, trabalho, lazer) para receber insights." }
                };
            }

            var features = ExtractFeatures(entries);

           
            var sleepRes = EvaluateSleep(features.AvgSleepHours);
            var activityRes = EvaluateActivity(features.AvgExerciseHours);
            var workRes = EvaluateWork(features.AvgWorkHours);
            var leisureRes = EvaluateLeisure(features.AvgLeisureHours);

           
            double weightSum = _config.SleepWeight + _config.ActivityWeight + _config.WorkWeight + _config.LeisureWeight;
            
            double finalScore = (
                sleepRes.Score * _config.SleepWeight +
                activityRes.Score * _config.ActivityWeight +
                workRes.Score * _config.WorkWeight +
                leisureRes.Score * _config.LeisureWeight
            ) / weightSum;

           
            var allTips = new List<string>();
            allTips.AddRange(sleepRes.Recommendations);
            allTips.AddRange(workRes.Recommendations);
            allTips.AddRange(activityRes.Recommendations);
            allTips.AddRange(leisureRes.Recommendations);

            return new AnalysisDto
            {
                Score = Math.Round(finalScore * 10, 1),
                Category = ScoreToCategory(finalScore),
                Summary = GenerateSummary(sleepRes, activityRes, workRes, leisureRes),
                Tips = allTips.Take(5).ToList(), 
                DomainScores = new Dictionary<string, double>
                {
                    { "Sono", Math.Round(sleepRes.Score * 10, 1) },
                    { "Atividade", Math.Round(activityRes.Score * 10, 1) },
                    { "Trabalho", Math.Round(workRes.Score * 10, 1) },
                    { "Lazer", Math.Round(leisureRes.Score * 10, 1) }
                }
            };
        }

        private RoutineFeatures ExtractFeatures(List<RoutineEntry> entries)
        {
            var daysCount = entries.Select(e => e.EntryDate).Distinct().Count();
            if (daysCount == 0) daysCount = 1;

            return new RoutineFeatures
            {
                AvgSleepHours = CalculateAvg(entries, "Sono", daysCount),
                AvgWorkHours = CalculateAvg(entries, "Trabalho", daysCount),
                AvgLeisureHours = CalculateAvg(entries, "Lazer", daysCount),
                AvgExerciseHours = CalculateAvg(entries, "Exercício", daysCount),
                DaysLogged = daysCount
            };
        }

        private double CalculateAvg(List<RoutineEntry> entries, string category, int days)
        {
            var total = entries
                .Where(e => e.Category.Contains(category, StringComparison.OrdinalIgnoreCase))
                .Sum(e => e.Hours);
            return (double)total / days;
        }



        private DomainResult EvaluateSleep(double hours)
        {
            var res = new DomainResult { Domain = "Sono" };
            if (hours >= 7 && hours <= 9) { res.Score = 10; res.Recommendations.Add("Seu sono está perfeito. Mantenha a regularidade."); }
            else if (hours < 6) { res.Score = 4; res.Recommendations.Add("Alerta: Sono insuficiente prejudica a cognição. Tente dormir 1h mais cedo."); }
            else if (hours > 9) { res.Score = 6; res.Recommendations.Add("Excesso de sono pode indicar fadiga ou desregulação."); }
            else { res.Score = 7.5; res.Recommendations.Add("Seu sono está razoável, mas pode melhorar."); }
            return res;
        }

        private DomainResult EvaluateActivity(double hours)
        {
            var res = new DomainResult { Domain = "Atividade" };
            
            if (hours >= 0.5) { res.Score = 10; res.Recommendations.Add("Ótimo nível de atividade física!"); }
            else if (hours > 0) { res.Score = 6; res.Recommendations.Add("Você se exercita, mas tente aumentar a frequência."); }
            else { res.Score = 2; res.Recommendations.Add("O sedentarismo é um risco. Tente uma caminhada de 15min."); }
            return res;
        }

        private DomainResult EvaluateWork(double hours)
        {
            var res = new DomainResult { Domain = "Trabalho" };
            if (hours > 10) { res.Score = 3; res.Recommendations.Add("Cuidado com o Burnout. Sua carga horária está excessiva."); }
            else if (hours > 8) { res.Score = 7; res.Recommendations.Add("Tente fazer pausas ativas durante o expediente."); }
            else { res.Score = 10; res.Recommendations.Add("Carga de trabalho equilibrada."); }
            return res;
        }

        private DomainResult EvaluateLeisure(double hours)
        {
            var res = new DomainResult { Domain = "Lazer" };
            if (hours < 1) { res.Score = 4; res.Recommendations.Add("Você precisa de tempo para si. Reserve 1h para hobbies."); }
            else { res.Score = 10; res.Recommendations.Add("Bom equilíbrio entre dever e lazer."); }
            return res;
        }

        private string ScoreToCategory(double score)
        {
            if (score >= 8.5) return "Excelente";
            if (score >= 7.0) return "Bom";
            if (score >= 5.0) return "Precisa Melhorar";
            return "Crítico";
        }

        private string GenerateSummary(DomainResult sleep, DomainResult activity, DomainResult work, DomainResult leisure)
        {
            if (sleep.Score < 5) return "Foco principal: Melhorar a qualidade do sono.";
            if (work.Score < 5) return "Atenção: Sobrecarga de trabalho detectada.";
            if (activity.Score < 5) return "Atenção: Risco de sedentarismo.";
            return "Sua rotina está bem equilibrada! Continue assim.";
        }
    }
}