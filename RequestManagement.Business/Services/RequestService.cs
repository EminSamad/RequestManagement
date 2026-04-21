using RequestManagement.Core.DTOs.Request;
using RequestManagement.Business.Interfaces;

namespace RequestManagement.Business.Services;

public class RequestService : IRequestService
{
    public Task<IEnumerable<RequestDto>> GetMyRequestsAsync(int userId)
        => throw new NotImplementedException();

    public Task<IEnumerable<RequestDto>> GetRequestsToMeAsync(int executorId)
        => throw new NotImplementedException();

    public Task CreateRequestAsync(CreateRequestDto dto, int userId)
        => throw new NotImplementedException();

    public Task RespondToRequestAsync(ResponseRequestDto dto, int executorId)
        => throw new NotImplementedException();

    public Task ChangeStatusToInProgressAsync(int requestId, int executorId)
        => throw new NotImplementedException();

    public Task CompleteRequestAsync(int requestId, int executorId)
        => throw new NotImplementedException();

    public Task ApproveRequestAsync(int requestId, int userId)
        => throw new NotImplementedException();

    public Task DeclineRequestAsync(int requestId, int userId)
        => throw new NotImplementedException();

    public Task RejectRequestAsync(int requestId, int userId)
        => throw new NotImplementedException();
}