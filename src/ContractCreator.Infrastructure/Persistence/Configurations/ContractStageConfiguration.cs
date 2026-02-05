using ContractCreator.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractCreator.Infrastructure.Persistence.Configurations
{
    public class ContractStageConfiguration : IEntityTypeConfiguration<ContractStage>
    {
        public void Configure(EntityTypeBuilder<ContractStage> builder)
        {
            builder.ToTable("ContractStages", schema: "public");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.TypeIds)
                   .HasColumnType("integer[]")
                   .IsRequired();

            builder.Property(e => e.Name).HasMaxLength(80).IsRequired();
            builder.Property(e => e.Description).HasMaxLength(160).IsRequired();

            builder.HasData(
                new ContractStage { Id = 1, TypeIds = new[] { 1, 2 }, Name = "Черновик", Description = "Первоначальный статус при создании документа." },
                new ContractStage { Id = 2, TypeIds = new[] { 1, 2 }, Name = "Согласование", Description = "Согласование внутри предприятия и с контрагентом." },
                new ContractStage { Id = 3, TypeIds = new[] { 1 }, Name = "Подача заявок", Description = "Заказчик разместил контракт для торгов." },
                new ContractStage { Id = 4, TypeIds = new[] { 1 }, Name = "Торги", Description = "Проходят торги." },
                new ContractStage { Id = 5, TypeIds = new[] { 1 }, Name = "Торги проиграны", Description = "Предложение предприятия не выиграло на торгах." },
                new ContractStage { Id = 6, TypeIds = new[] { 1, 2 }, Name = "Заключение", Description = "Для договора: текст согласован сторонами, договор на подписи. Для госконтракта: торги выиграны, идет процесс заключения контракта." },
                new ContractStage { Id = 7, TypeIds = new[] { 1, 2 }, Name = "Заключен", Description = "Обе стороны подписали документ." },
                new ContractStage { Id = 8, TypeIds = new[] { 1, 2 }, Name = "На исполнении", Description = "Находится на исполнении." },
                new ContractStage { Id = 9, TypeIds = new[] { 1, 2 }, Name = "Исполнен", Description = "Исполнен." },
                new ContractStage { Id = 10, TypeIds = new[] { 1, 2 }, Name = "Оплачен", Description = "Все финансовые и прочие расчеты по документу завершены." },
                new ContractStage { Id = 11, TypeIds = new[] { 2 }, Name = "Расторжение", Description = "Договор на стадии расторжения." },
                new ContractStage { Id = 12, TypeIds = new[] { 2 }, Name = "Расторгнут", Description = "Договор расторгнут." }
            );
        }
    }
}
