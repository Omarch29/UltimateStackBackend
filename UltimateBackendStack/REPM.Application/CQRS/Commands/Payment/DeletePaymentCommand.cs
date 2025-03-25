using Ardalis.GuardClauses;
using MediatR;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Commands;

public record DeletePaymentCommand(Guid PaymentId) : IRequest<bool>;

public class DeletePaymentCommandHandler : IRequestHandler<DeletePaymentCommand, bool>
{
    private readonly IRepository<Payment> _paymentRepository;

    public DeletePaymentCommandHandler(IRepository<Payment> paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<bool> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.PaymentId, nameof(request.PaymentId));

        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment == null) throw new NotFoundException(nameof(Payment), request.PaymentId.ToString());

        _paymentRepository.Delete(payment);
        return await _paymentRepository.SaveChangesAsync(cancellationToken);
    }
}