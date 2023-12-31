using DeliveryServiceApi.Data;
using DeliveryServiceApi.Data.Models;
using DeliveryServiceApi.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net;

namespace DeliveryServiceApi.IntegrationTests
{
    [TestFixture]
    public class DeliveryControllerTests
    {
        [Test]
        public async Task CheckStatus_SendRequest_ShouldReturnOk()
        {
            WebApplicationFactory<Startup> webHost = new WebApplicationFactory<Startup>().WithWebHostBuilder(_ => { });
            HttpClient httpClient = webHost.CreateClient();

            HttpResponseMessage responseMessage = await httpClient.GetAsync("api/delivery/checkStatus");
            Assert.AreEqual(HttpStatusCode.OK, responseMessage.StatusCode);
        }

        [Test]
        public async Task SendOrder_FreeCourierAvailable_ShouldReturnNotFound()
        {
            WebApplicationFactory<Startup> webHost = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var orderService = services.SingleOrDefault(d => d.ServiceType == typeof(IOrderService));
                    services.Remove(orderService);
                    var mockService = new Mock<IOrderService>();
                    mockService.Setup(_ => _.IsFreeCourierAvailable()).Returns(() => false);
                    services.AddTransient(_ => mockService.Object);
                });
            });

            HttpClient httpClient = webHost.CreateClient();

            HttpResponseMessage responseMessage = await httpClient.PostAsync("api/delivery/sendOrder", null);

            Assert.AreEqual(HttpStatusCode.NotFound, responseMessage.StatusCode);
        }

        [Test]
        public async Task GetOrdersCount_sendRequest_ShouldReturnActualOrdersCount()
        {
            WebApplicationFactory<Startup> webHost = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    var dbContextDescriptor = services.SingleOrDefault(d =>
                    d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                    services.Remove(dbContextDescriptor);
                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseInMemoryDatabase("delivery_db");
                    });
                });
            });

            ApplicationDbContext dbContext = webHost.Services.CreateScope().ServiceProvider.GetService<ApplicationDbContext>();
            List<Order> orders = new List<Order>() { new Order(), new Order(), new Order() };
            await dbContext.Orders.AddRangeAsync(orders);
            await dbContext.SaveChangesAsync();
            HttpClient httpClient = webHost.CreateClient();
            HttpResponseMessage response = await httpClient.GetAsync("api/delivery/ordersCount");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            int ordersCount = int.Parse(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(ordersCount, orders.Count);
        }
    }
}
