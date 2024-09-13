using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Order.Service
{
    public record CheckStockResponse(bool status);
    public class StockService(HttpClient client)
    {
        public async Task<bool> CheckStock(int productId, int quantity)
        {
            var fallbackPolicy = Policy<HttpResponseMessage>.Handle<Exception>().FallbackAsync(new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(JsonSerializer.Serialize(new CheckStockResponse(true)))
            });

            var response = await fallbackPolicy.ExecuteAsync(() => client.GetAsync($"api/stock/{productId}/{quantity}"));

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var content = await response.Content.ReadFromJsonAsync<CheckStockResponse>();

            return content!.status;
        }
    }
}
