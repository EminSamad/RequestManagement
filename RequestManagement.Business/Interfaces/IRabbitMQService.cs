namespace RequestManagement.Business.Interfaces;

public interface IRabbitMQService
{
    void PublishMessageAsync(string queueName, string message);
    void ConsumeMessageAsync(string queueName, Func<string, Task> onMessage);
}