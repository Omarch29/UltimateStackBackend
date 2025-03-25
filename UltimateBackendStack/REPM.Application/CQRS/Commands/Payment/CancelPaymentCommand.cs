using Ardalis.GuardClauses;
using MediatR;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Commands;

public record CancelPaymentCommand(Guid PaymentId) : IRequest<bool>;

public class CancelPaymentCommandHandler : IRequestHandler<CancelPaymentCommand, bool>
{
    private readonly IRepository<Payment> _paymentRepository;

    public CancelPaymentCommandHandler(IRepository<Payment> paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<bool> Handle(CancelPaymentCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.PaymentId, nameof(request.PaymentId));

        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment == null) throw new NotFoundException(nameof(Payment), request.PaymentId.ToString());

        payment.Cancel();
        return await _paymentRepository.SaveChangesAsync(cancellationToken);
    }
}