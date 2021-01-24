using _2C2P.DEMO.Domain.Models;
using _2C2P.DEMO.Infrastructure.Interfaces;
using _2C2P.DEMO.Infrastructure.Responses;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace _2C2P.DEMO.Api.Features.Transactions.Commands
{
    public class InsertTransactionCommand : IRequest<Response<Unit>>
    {
        public Transaction _transactinon;
        public InsertTransactionCommand(Transaction transaction)
        {
            _transactinon = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        public class InsertTransactionCommandHandler : IRequestHandler<InsertTransactionCommand, Response<Unit>>
        {
            private readonly ITransactionRepository _transactionRepository;
            public InsertTransactionCommandHandler(ITransactionRepository transactionRepository)
            {
                _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
            }
            public async Task<Response<Unit>> Handle(InsertTransactionCommand request, CancellationToken cancellationToken)
            {
                var trans = request._transactinon;

                trans.Id = Guid.NewGuid();

                await _transactionRepository.Insert(trans);

                return new OkResponse<Unit>(Unit.Value);
            }
        }
    }
}
