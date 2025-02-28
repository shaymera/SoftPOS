using System;
using Microsoft.EntityFrameworkCore;
using PaymentApi.Models;

namespace PaymentApi.Context;

public class ApplicationDbContext : DbContext
{
    public required DbSet<Transaction> Transactions { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>(transac =>
        {
            transac.HasKey(t => t.TransactionId);
            transac.Property(t => t.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            transac.Property(t => t.State)
                .HasConversion<int>()
                .HasDefaultValue(TransactionState.Pending);
            transac.Property(t => t.CardNumber)
                .HasMaxLength(19);
        });
    }
}
