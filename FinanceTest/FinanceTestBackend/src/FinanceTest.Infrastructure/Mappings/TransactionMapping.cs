using FinanceTest.Domain.Entities.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTest.Infrastructure.Mappings;

public class TransactionMapping : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");
        
        builder.HasKey(t => t.Id);
        
        builder.Property(t => t.Type)
            .IsRequired()
            .HasConversion<int>(); 
        
        builder.Property(t => t.Date)
            .IsRequired();
        
        builder.Property(t => t.Value)
            .IsRequired()
            .HasPrecision(18, 2);
        
        builder.Property(t => t.Cpf)
            .IsRequired()
            .HasMaxLength(11)
            .IsFixedLength();
        
        builder.Property(t => t.Card)
            .IsRequired()
            .HasMaxLength(12)
            .IsFixedLength();
        
        builder.Property(t => t.StoreOwner)
            .IsRequired()
            .HasMaxLength(14);
        
        builder.Property(t => t.StoreName)
            .IsRequired()
            .HasMaxLength(19);
        
        builder.HasIndex(t => t.StoreName);
    }
}