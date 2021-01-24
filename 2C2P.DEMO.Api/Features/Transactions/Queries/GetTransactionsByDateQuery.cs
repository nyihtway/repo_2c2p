using _2C2P.DEMO.Api.Models;
using _2C2P.DEMO.Domain.Models;
using _2C2P.DEMO.Infrastructure.Interfaces;
using _2C2P.DEMO.Infrastructure.Responses;
using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace _2C2P.DEMO.Api.Features.Transactions.Queries
{
    public class GetTransactionsByDateQuery : IRequest<Response<List<TransactionDTO>>>
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public GetTransactionsByDateQuery()
        {

        }

        public class GetTransactionsByDateQueryHandler : IRequestHandler<GetTransactionsByDateQuery, Response<List<TransactionDTO>>>
        {
            private readonly ITransactionRepository _transactionRepository;
            private readonly IMapper _mapper;

            public GetTransactionsByDateQueryHandler(ITransactionRepository transactionRepository, IMapper mapper)
            {
                _transactionRepository = transactionRepository ?? throw new ArgumentNullException(nameof(transactionRepository));
                _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            }

            public async Task<Response<List<TransactionDTO>>> Handle(GetTransactionsByDateQuery request, CancellationToken cancellationToken)
            {
                var result = new List<TransactionDTO>();
                IEnumerable<Transaction> transactions = await _transactionRepository.GetTransactionsByStartDateEndDate(request.StartDate, request.EndDate);

                result = _mapper.Map<List<TransactionDTO>>(transactions.ToList());

                return new OkResponse<List<TransactionDTO>>(result);
            }
        }
    }
}
