using IrisPayments.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace IrisPayments.Data;
public class IrisPaymentsContext : DbContext {
    public IrisPaymentsContext(DbContextOptions<IrisPaymentsContext> options)
        : base(options) {
    }

    public DbSet<PaymentCode> PaymentCode { get; set; } = default!;
    public DbSet<PaymentLogs> PaymentLogs { get; set; } = default!;
}