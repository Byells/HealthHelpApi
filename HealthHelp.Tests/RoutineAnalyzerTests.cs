using HealthHelp.Api.Data;
using HealthHelp.Api.Models;
using HealthHelp.Api.Services;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HealthHelp.Tests
{
    public class RoutineAnalyzerTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task AnalyzeForUser_UsuarioComBurnout_DeveRetornarNotaBaixa()
        {
            var context = GetInMemoryDbContext();
            var config = new ScoringConfig();
            var service = new RoutineAnalyzer(context, config);
            var userId = "user_teste_burnout";

            for (int i = 0; i < 3; i++)
            {
                var date = DateOnly.FromDateTime(DateTime.Now.AddDays(-i));
                context.RoutineEntries.AddRange(
                    new RoutineEntry { ApplicationUserId = userId, Category = "Trabalho", Hours = 12, EntryDate = date },
                    new RoutineEntry { ApplicationUserId = userId, Category = "Sono", Hours = 4, EntryDate = date }
                );
            }
            await context.SaveChangesAsync();

            var result = await service.AnalyzeForUserAsync(userId);

            Assert.True(result.Score < 60, $"A nota geral foi {result.Score}, esperava-se < 60");
            
            Assert.Contains("Trabalho", result.DomainScores.Keys);
            Assert.True(result.DomainScores["Trabalho"] < 50, $"O score de trabalho foi {result.DomainScores["Trabalho"]}, esperava-se < 50");
            
            Assert.Contains(result.Tips, tip => tip.Contains("Burnout") || tip.Contains("Carga"));
        }

        [Fact]
        public async Task AnalyzeForUser_UsuarioAtleta_DeveRetornarNotaAlta()
        {
            var context = GetInMemoryDbContext();
            var service = new RoutineAnalyzer(context, new ScoringConfig());
            var userId = "user_teste_atleta";

            var date = DateOnly.FromDateTime(DateTime.Now);
            context.RoutineEntries.AddRange(
                new RoutineEntry { ApplicationUserId = userId, Category = "Sono", Hours = 8, EntryDate = date },
                new RoutineEntry { ApplicationUserId = userId, Category = "Trabalho", Hours = 8, EntryDate = date },
                new RoutineEntry { ApplicationUserId = userId, Category = "Exercício", Hours = 1, EntryDate = date }
            );
            await context.SaveChangesAsync();

            var result = await service.AnalyzeForUserAsync(userId);

            Assert.True(result.Score > 80, $"A nota foi {result.Score}, esperava-se > 80");
            Assert.Equal("Excelente", result.Category);
        }
    }
}