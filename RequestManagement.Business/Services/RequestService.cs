using RequestManagement.Core.DTOs.Request;
using RequestManagement.Business.Interfaces;
using RequestManagement.Core.Entities;
using RequestManagement.Core.Enums;
using RequestManagement.Core.Exceptions;
using RequestManagement.Data.Repositories.Interfaces;

namespace RequestManagement.Business.Services;

public class RequestService : IRequestService
{
    private readonly IUnitOfWork _unitOfWork;

    public RequestService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<RequestDto>> GetMyRequestsAsync(int userId)
    {
        var requests = await _unitOfWork.Requests.GetAllAsync();

        return requests
            .Where(r => r.RequesterId == userId)
            .Select(r => new RequestDto
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                Priority = r.Priority,
                DueDate = r.DueDate,
                Status = r.Status,
                FilePath = r.FilePath,
                CategoryId = r.CategoryId,
                RequesterId = r.RequesterId,
                ExecutorId = r.ExecutorId
            });
    }

    public async Task<IEnumerable<RequestDto>> GetRequestsToMeAsync(int executorId)
    {
        var requests = await _unitOfWork.Requests.GetAllAsync();

        return requests
            .Where(r => r.ExecutorId == executorId)
            .Select(r => new RequestDto
            {
                Id = r.Id,
                Title = r.Title,
                Description = r.Description,
                Priority = r.Priority,
                DueDate = r.DueDate,
                Status = r.Status,
                FilePath = r.FilePath,
                CategoryId = r.CategoryId,
                RequesterId = r.RequesterId,
                ExecutorId = r.ExecutorId
            });
    }

    public async Task CreateRequestAsync(CreateRequestDto dto, int userId, string? filePath = null)
    {
        var request = new Request
        {
            Title = dto.Title,
            Description = dto.Description,
            Priority = dto.Priority,
            DueDate = dto.DueDate,
            CategoryId = dto.CategoryId,
            RequesterId = userId,
            Status = RequestStatus.Initial,
            FilePath = filePath,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Requests.AddAsync(request);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RespondToRequestAsync(ResponseRequestDto dto, int executorId)
    {
        var requests = await _unitOfWork.Requests.GetAllAsync();
        var request = requests.FirstOrDefault(r => r.Id == dto.RequestId);

        if (request == null)
            throw new NotFoundException("Request not found");

        if (request.ExecutorId != executorId)
            throw new ForbiddenException("You are not the executor of this request");

        request.ResponseText = dto.ResponseText;
        request.Status = RequestStatus.Completed;
        request.ModifiedBy = executorId;
        request.ModifiedAt = DateTime.UtcNow;

        await _unitOfWork.Requests.UpdateAsync(request);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ChangeStatusToInProgressAsync(int requestId, int executorId)
    {
        var request = await _unitOfWork.Requests.GetByIdAsync(requestId);

        if (request == null)
            throw new NotFoundException("Request not found");

        if (request.Status != RequestStatus.Initial)
            throw new BadRequestException("Request is not in Initial status");

        request.ExecutorId = executorId;
        request.Status = RequestStatus.InProgress;
        request.ModifiedBy = executorId;
        request.ModifiedAt = DateTime.UtcNow;

        await _unitOfWork.Requests.UpdateAsync(request);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task CompleteRequestAsync(int requestId, int executorId)
    {
        var request = await _unitOfWork.Requests.GetByIdAsync(requestId);

        if (request == null)
            throw new NotFoundException("Request not found");

        if (request.ExecutorId != executorId)
            throw new ForbiddenException("You are not the executor of this request");

        if (request.Status != RequestStatus.InProgress)
            throw new BadRequestException("Request is not in InProgress status");

        request.Status = RequestStatus.Completed;
        request.ModifiedBy = executorId;
        request.ModifiedAt = DateTime.UtcNow;

        await _unitOfWork.Requests.UpdateAsync(request);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ApproveRequestAsync(int requestId, int userId)
    {
        var request = await _unitOfWork.Requests.GetByIdAsync(requestId);

        if (request == null)
            throw new NotFoundException("Request not found");

        if (request.RequesterId != userId)
            throw new ForbiddenException("You are not the requester of this request");

        if (request.Status != RequestStatus.Completed)
            throw new BadRequestException("Request is not in Completed status");

        request.Status = RequestStatus.Approved;
        request.ModifiedBy = userId;
        request.ModifiedAt = DateTime.UtcNow;

        await _unitOfWork.Requests.UpdateAsync(request);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeclineRequestAsync(int requestId, int userId)
    {
        var request = await _unitOfWork.Requests.GetByIdAsync(requestId);

        if (request == null)
            throw new NotFoundException("Request not found");

        if (request.RequesterId != userId)
            throw new ForbiddenException("You are not the requester of this request");

        if (request.Status != RequestStatus.Completed)
            throw new BadRequestException("Request is not in Completed status");

        request.Status = RequestStatus.Declined;
        request.ModifiedBy = userId;
        request.ModifiedAt = DateTime.UtcNow;

        await _unitOfWork.Requests.UpdateAsync(request);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task RejectRequestAsync(int requestId, int userId)
    {
        var request = await _unitOfWork.Requests.GetByIdAsync(requestId);

        if (request == null)
            throw new NotFoundException("Request not found");

        if (request.Status != RequestStatus.Initial)
            throw new BadRequestException("Request is not in Initial status");

        request.Status = RequestStatus.Rejected;
        request.ModifiedBy = userId;
        request.ModifiedAt = DateTime.UtcNow;

        await _unitOfWork.Requests.UpdateAsync(request);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<RequestDto>> GetFilteredRequestsAsync(RequestFilterDto filter)
    {
        var requests = await _unitOfWork.Requests.GetAllAsync();

        var query = requests.AsQueryable();

        if (!string.IsNullOrEmpty(filter.SearchText))
            query = query.Where(r => r.Title.Contains(filter.SearchText) ||
                                     r.Description.Contains(filter.SearchText));

        if (filter.CategoryId.HasValue)
            query = query.Where(r => r.CategoryId == filter.CategoryId);

        if (filter.Priority.HasValue)
            query = query.Where(r => r.Priority == filter.Priority.Value);

        if (filter.Status.HasValue)
            query = query.Where(r => r.Status == filter.Status.Value);
        return query.Select(r => new RequestDto
        {
            Id = r.Id,
            Title = r.Title,
            Description = r.Description,
            Priority = r.Priority,
            DueDate = r.DueDate,
            Status = r.Status,
            FilePath = r.FilePath,
            CategoryId = r.CategoryId,
            RequesterId = r.RequesterId,
            ExecutorId = r.ExecutorId
        });
    }
}