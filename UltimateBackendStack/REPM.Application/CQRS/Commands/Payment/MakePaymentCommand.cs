using Ardalis.GuardClauses;
using AutoMapper;
using MediatR;
using REPM.Application.DTOs;
using REPM.Domain.Entities;
using REPM.Domain.ValueObjects;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Commands;

public record MakePaymentCommand(PaymentForCreateDto Payment) : IRequest<Guid>;

public class MakePaymentCommandHandler : IRequestHandler<MakePaymentCommand, Guid>
{
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IMapper _mapper;
    public async Task<Guid> Handle(MakePaymentCommand request, CancellationToken cancellationToken)
    {
        Guard.Against.Null(request, nameof(request));
        Guard.Against.Null(request.Payment, nameof(request.Payment));
        
        var payment = new Payment(request.Payment.leaseId, _mapper.Map<Money>(request.Payment.amount), request.Payment.date);
        
        _paymentRepository.Insert(payment);
        await _paymentRepository.SaveChangesAsync(cancellationToken);
        
        return payment.Id;
    }
}