using RequestManagement.Core.DTOs.Request;

namespace RequestManagement.Business.Interfaces;

public interface IRequestService
{
    Task<IEnumerable<RequestDto>> GetMyRequestsAsync(int userId);
    Task<IEnumerable<RequestDto>> GetRequestsToMeAsync(int executorId);
    Task<IEnumerable<RequestDto>> GetFilteredRequestsAsync(RequestFilterDto filter);
    Task CreateRequestAsync(CreateRequestDto dto, int userId, string? filePath = null);
    Task RespondToRequestAsync(ResponseRequestDto dto, int executorId);
    Task ChangeStatusToInProgressAsync(int requestId, int executorId);
    Task CompleteRequestAsync(int requestId, int executorId);
    Task ApproveRequestAsync(int requestId, int userId);
    Task DeclineRequestAsync(int requestId, int userId);
    Task RejectRequestAsync(int requestId, int userId);
}