using IrisPayments.Data.Interfaces;
using IrisPayments.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace IrisPayments.Data.Services;
public class PaymentCodeService : IPaymentCode {

    private readonly IrisPaymentsContext _context;
    private readonly ILogger<PaymentCodeService> _logger;
    public PaymentCodeService(IrisPaymentsContext context, ILogger<PaymentCodeService> logger) {
        _context = context;
        _logger = logger;
    }
    public async Task<Guid> Add(PaymentCode paymentCode) {
        try {
            _context.PaymentCode.Add(paymentCode);
            await _context.SaveChangesAsync();
        } catch(Exception ex) {
            _logger.LogError(ex, ex.Message);
            throw;
        }
        return paymentCode.Id;
    }

    public async Task<bool> Update(PaymentCode paymentCode) {
        _context.Attach(paymentCode).State = EntityState.Modified;
        try {
            await _context.SaveChangesAsync();
            return true;
        } catch(DbUpdateConcurrencyException e) {
            if(!await Exists(paymentCode.Id)) {
                return false;
            } else {
                _logger.LogError(e, e.Message);
                throw;
            }
        } catch(Exception ex) {
            _logger.LogError(ex, ex.Message);
            throw;
        }
    }

    public async Task<bool> Delete(Guid? id) {
        var paymentIdentity = await _context.PaymentCode.FindAsync(id);
        if(paymentIdentity == null) {
            return false;
        }
        _context.PaymentCode.Remove(paymentIdentity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<PaymentCode> Get(Guid? id) => await _context.PaymentCode.FindAsync(id);

    public async Task<PaymentCode> Get(string code) => await _context.PaymentCode.Where(x => x.Code == code).FirstOrDefaultAsync();

    public async Task<PaymentCode> GetByOrderID(long? orderId) => await _context.PaymentCode.Where(x => x.OrderID == orderId).FirstOrDefaultAsync();

    public async Task<PaymentCode> GetByCustomerID(long? customerId) => await _context.PaymentCode.Where(x => x.CustomerID == customerId).FirstOrDefaultAsync();

    public async Task<List<PaymentCode>> GetAll() => await _context.PaymentCode.ToListAsync();

    public async Task<bool> Exists(Guid id) => await _context.PaymentCode?.AnyAsync(x => x.Id == id);
}