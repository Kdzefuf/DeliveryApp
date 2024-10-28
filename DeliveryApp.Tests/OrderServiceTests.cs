using Newtonsoft.Json;

namespace DeliveryApp.Tests
{
    public class OrderServiceTests
    {
        private static string configPath = "app.json";
        private static string fullPath = Path.GetFullPath(configPath);
        private static Dictionary<string, string> config = LoadConfiguration(fullPath);

        private string OrdersFilePath = config["TestOrdersFile"];
        private string LogFilePath = config["TestDeliveryLog"];
        private string OutputFilePath = config["TestDeliveryOrder"];

        private OrderService InitializeOrderService()
        {
            return new OrderService(LogFilePath, OutputFilePath);
        }

        [SetUp]
        public void SetUp()
        {
            if (File.Exists(OrdersFilePath)) File.Delete(OrdersFilePath);
            if (File.Exists(LogFilePath)) File.Delete(LogFilePath);
            if (File.Exists(OutputFilePath)) File.Delete(OutputFilePath);
        }

        [Test]
        public void AddOrder_ShouldAddOrderToFile()
        {
            var orderService = InitializeOrderService();
            var order = new Order
            {
                OrderId = Guid.NewGuid().ToString(),
                Weight = 10,
                District = "DistrictA",
                DeliveryDateTime = DateTime.Now
            };

            orderService.AddOrder(order, OrdersFilePath);

            var orders = JsonConvert.DeserializeObject<Order[]>(File.ReadAllText(OrdersFilePath));
            Assert.IsTrue(orders.Any(o => o.OrderId == order.OrderId));
        }

        [Test]
        public void FilterOrders_ShouldReturnFilteredOrders()
        {
            SetUp();
            var orderService = InitializeOrderService();
            var orders = new List<Order>
            {
                new Order {
                    OrderId = Guid.NewGuid().ToString(),
                    District = "DistrictA",
                    DeliveryDateTime = DateTime.Now.AddMinutes(-20)
                },
                new Order {
                    OrderId = Guid.NewGuid().ToString(),
                    District = "DistrictA",
                    DeliveryDateTime = DateTime.Now.AddMinutes(10)
                },
                new Order {
                    OrderId = Guid.NewGuid().ToString(),
                    District = "DistrictB",
                    DeliveryDateTime = DateTime.Now.AddMinutes(20)
                }
            };
            var firstDeliveryTime = DateTime.Now;

            var filteredOrders = orderService.FilterOrders(orders, "DistrictA", firstDeliveryTime);

            Assert.AreEqual(1, filteredOrders.Count);
            Assert.AreEqual(orders[1].OrderId, filteredOrders[0].OrderId);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(OrdersFilePath)) File.Delete(OrdersFilePath);
            if (File.Exists(LogFilePath)) File.Delete(LogFilePath);
            if (File.Exists(OutputFilePath)) File.Delete(OutputFilePath);
        }

        private static Dictionary<string, string> LoadConfiguration(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Файл конфигурации '{path}' не найден.");
            }

            try
            {
                var jsonData = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка загрузки конфигурации", ex);
            }
        }
    }
}