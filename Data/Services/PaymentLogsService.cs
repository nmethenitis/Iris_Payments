using IrisPayments.Data;
using IrisPayments.Data.Interfaces;
using IrisPayments.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace IrisPayments.Data.Services;
    public class PaymentLogsService : IPaymentLogs {

        private readonly IrisPaymentsContext _context;
        private readonly ILogger<PaymentLogsService> _logger;
        public PaymentLogsService(IrisPaymentsContext context, ILogger<PaymentLogsService> logger) {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(PaymentLogs paymentLogs) {
            try {
                _context.PaymentLogs.Add(paymentLogs);
                _context.SaveChangesAsync();
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public void Add(PaymentLogs paymentLogs) {
            try {
                _context.PaymentLogs.Add(paymentLogs);
                _context.SaveChanges();
            } catch (Exception ex) {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<List<PaymentLogs>> GetAll(string code) => await _context.PaymentLogs.Where(x => x.PaymentCode == code).ToListAsync();

        public async Task<bool> Exists(string code) => await _context.PaymentLogs.AnyAsync(p => p.PaymentCode == code);

        public async Task<bool> Exists(string RID, string code) => await _context.PaymentLogs.AnyAsync(p => p.PaymentCode == code && p.ReferenceId == RID);

        public async Task<bool> ExistsPaid(string code) => await _context.PaymentLogs.AnyAsync(p => p.PaymentCode == code && p.IsPaid);
    }