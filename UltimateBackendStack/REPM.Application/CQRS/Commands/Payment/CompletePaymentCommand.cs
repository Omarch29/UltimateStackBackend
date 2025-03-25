using Ardalis.GuardClauses;
using MediatR;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Commands;

public record CompletePaymentCommand(Guid PaymentId) : IRequest<bool>;

public class CompletePaymentCommandHandler : IRequestHandler<CompletePaymentCommand, bool>
{
    private readonly IRepository<Payment> _paymentRepository;

    public CompletePaymentCommandHandler(IRepository<Payment> paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<bool> Handle(CompletePaymentCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.PaymentId, nameof(request.PaymentId));

        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment == null) throw new NotFoundException(nameof(Payment), request.PaymentId.ToString());

        payment.MarkAsCompleted();
        return await _paymentRepository.SaveChangesAsync(cancellationToken);
    }
}