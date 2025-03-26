
using ProducerRestAPI.Helpers;

namespace ProducerRestAPI.Services
{
    public interface IDataService
    {
        Task<bool> SendData(string data);
    }

    public class DataService : IDataService
    {
        public async Task<bool> SendData(string data)
        {
            RabbitMqHelper rabbitMqHelper = new();

            return await rabbitMqHelper.Send(data, "test");
        }
    }
}
