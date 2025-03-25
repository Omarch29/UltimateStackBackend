using Ardalis.GuardClauses;
using AutoMapper;
using MediatR;
using REPM.Application.DTOs;
using REPM.Domain.Entities;
using REPM.Infrastructure.Interfaces;

namespace REPM.Application.CQRS.Commands;

public record UpdatePaymentCommand(PaymentForUpdateDto PaymentForUpdateDto) : IRequest<bool>;

public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand, bool>
{
    private readonly IRepository<Payment> _paymentRepository;
    private readonly IMapper _mapper;

    public UpdatePaymentCommandHandler(IRepository<Payment> paymentRepository, IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<bool> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
    {
       Guard.Against.Null(request, nameof(request));
       Guard.Against.Null(request.PaymentForUpdateDto, nameof(request.PaymentForUpdateDto));
       
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentForUpdateDto.Id, cancellationToken);
        if (payment == null) throw new NotFoundException(nameof(Payment), request.PaymentForUpdateDto.Id.ToString());

        _mapper.Map(request.PaymentForUpdateDto, payment);
        return await _paymentRepository.SaveChangesAsync(cancellationToken);
    }
}