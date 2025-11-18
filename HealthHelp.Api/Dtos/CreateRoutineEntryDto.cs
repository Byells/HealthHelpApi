using System.ComponentModel.DataAnnotations;

namespace HealthHelp.Api.Dtos
{
    public class CreateRoutineEntryDto
    {
        [Required(ErrorMessage = "Categoria é obrigatória")]
        [AllowedValues("Sono", "Trabalho", "Lazer", "Exercício", ErrorMessage = "Categoria inválida. Use apenas: 'Sono', 'Trabalho', 'Lazer' ou 'Exercício'.")]
        public string Category { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Horas é obrigatório")]
        [Range(0.1, 24, ErrorMessage = "Valor de horas deve ser entre 0.1 e 24")]
        public decimal Hours { get; set; }

        [Required(ErrorMessage = "Data é obrigatória")]
        public DateOnly EntryDate { get; set; }
    }
}