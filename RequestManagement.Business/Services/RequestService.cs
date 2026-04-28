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
    private readonly IEmailService _emailService;

    public RequestService(IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _unitOfWork = unitOfWork;
        _emailService = emailService;
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
                Priority = r.Priority.ToString(),
                DueDate = r.DueDate,
                Status = r.Status.ToString(),
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
                Priority = r.Priority.ToString(),
                DueDate = r.DueDate,
                Status = r.Status.ToString(),
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

        await _unitOfWork.SaveChangesAsync();

        // Email notification
        var users = await _unitOfWork.Users.GetAllAsync();
        var requester = users.FirstOrDefault(u => u.Id == userId);

        if (requester != null)
            await _emailService.SendEmailAsync(
                requester.Email,
                "Request Created",
                $"<h3>Your request '{request.Title}' has been created successfully!</h3>"
            );
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

        var users = await _unitOfWork.Users.GetAllAsync();
        var requester = users.FirstOrDefault(u => u.Id == request.RequesterId);

        if (requester != null)
            await _emailService.SendEmailAsync(
                requester.Email,
                "Response Received",
                $"<h3>You have received a response for your request '{request.Title}'!</h3>"
            );
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

        var users = await _unitOfWork.Users.GetAllAsync();
        var executor = users.FirstOrDefault(u => u.Id == executorId);

        if (executor != null)
            await _emailService.SendEmailAsync(
                executor.Email,
                "New Request Assigned",
                $"<h3>A new request '{request.Title}' has been assigned to you!</h3>"
            );
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

        var users = await _unitOfWork.Users.GetAllAsync();
        var requester = users.FirstOrDefault(u => u.Id == request.RequesterId);

        if (requester != null)
            await _emailService.SendEmailAsync(
                requester.Email,
                "Request Completed",
                $"<h3>Your request '{request.Title}' has been completed!</h3>"
            );
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

        var users = await _unitOfWork.Users.GetAllAsync();
        var executor = users.FirstOrDefault(u => u.Id == request.ExecutorId);

        if (executor != null)
            await _emailService.SendEmailAsync(
                executor.Email,
                "Request Approved",
                $"<h3>Your work on request '{request.Title}' has been approved!</h3>"
            );
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

        var users = await _unitOfWork.Users.GetAllAsync();
        var executor = users.FirstOrDefault(u => u.Id == request.ExecutorId);

        if (executor != null)
            await _emailService.SendEmailAsync(
                executor.Email,
                "Request Declined",
                $"<h3>Your work on request '{request.Title}' has been declined!</h3>"
            );
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

        var users = await _unitOfWork.Users.GetAllAsync();
        var requester = users.FirstOrDefault(u => u.Id == request.RequesterId);

        if (requester != null)
            await _emailService.SendEmailAsync(
                requester.Email,
                "Request Rejected",
                $"<h3>Your request '{request.Title}' has been rejected!</h3>"
            );
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

        if (filter.OrderByDateAsc)
            query = query.OrderBy(r => r.DueDate);
        else
            query = query.OrderByDescending(r => r.DueDate);

        query = query
        .Skip((filter.PageNumber - 1) * filter.PageSize)
        .Take(filter.PageSize);

        return query.Select(r => new RequestDto
        {
            Id = r.Id,
            Title = r.Title,
            Description = r.Description,
            Priority = r.Priority.ToString(),
            DueDate = r.DueDate,
            Status = r.Status.ToString(),
            FilePath = r.FilePath,
            CategoryId = r.CategoryId,
            RequesterId = r.RequesterId,
            ExecutorId = r.ExecutorId
        });
    }
}