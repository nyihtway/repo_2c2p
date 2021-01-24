using _2C2P.DEMO.Api.Models;
using _2C2P.DEMO.Infrastructure.Interfaces;
using _2C2P.DEMO.Infrastructure.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using _2C2P.DEMO.Domain.Models;

namespace _2C2P.DEMO.Api.Features.Transactions.Queries
{
    public class GetTransactionsByStatusQuery : IRequest<Response<List<TransactionDTO>>>
    {
        public string _status;
        public GetTransactionsByStatusQuery(string status)
        {
            _status = status ?? throw new ArgumentNullException(nameof(status));
        }

        public class GetTransactionsByStatusQueryHandler : IRequestHandler<GetTransactionsByStatusQuery, Response<List<TransactionDTO>>>
        {
            private readonly ITransactionRepository _transactionRepository;
            private readonly IMapper _mapper;

            public GetTransactionsByStatusQueryHandler(ITransactionRepository transactionRepository, IMapper mapper)
            {
                _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<Response<List<TransactionDTO>>> Handle(GetTransactionsByStatusQuery request, CancellationToken cancellationToken)
            {
                var result = new List<TransactionDTO>();
                IEnumerable<Transaction> transactions = await _transactionRepository.GetTransactionsByStatus(request._status);

                result = _mapper.Map<List<TransactionDTO>>(transactions.ToList());

                return new OkResponse<List<TransactionDTO>>(result);
            }
        }
    }
}
