using Newtonsoft.Json;

namespace DeliveryApp
{
    public class OrderService
    {
        private readonly string _logPath;
        private readonly string _outputPath;

        public OrderService(string logPath, string outputPath)
        {
            _logPath = logPath;
            _outputPath = outputPath;
        }

        public void Log(string message)
        {
            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
            File.AppendAllText(_logPath, logMessage + Environment.NewLine);
        }

        public void AddOrder(Order order, string filePath)
        {
            var orders = LoadOrders(filePath);
            orders.Add(order);
            File.WriteAllText(filePath, JsonConvert.SerializeObject(orders, Formatting.Indented));
            Log($"Добавлен заказ: {order.OrderId}");
        }

        public List<Order> LoadOrders(string filePath)
        {
            try
            {
                var jsonData = File.ReadAllText(filePath);
                var orders = JsonConvert.DeserializeObject<List<Order>>(jsonData);

                if (orders == null)
                {
                    throw new Exception("Ошибка при загрузке данных из файла. Файл пуст или поврежден.");
                }

                return orders;
            }
            catch (Exception ex)
            {
                Log($"Ошибка загрузки заказов: {ex.Message}");
                return new List<Order>();
            }
        }

        public List<Order> FilterOrders(List<Order> orders, string district, DateTime firstDeliveryTime)
        {
            var result = orders
                .Where(order => order.District == district &&
                    order.DeliveryDateTime >= firstDeliveryTime &&
                    order.DeliveryDateTime <= firstDeliveryTime.AddMinutes(30))
                .ToList();

            Console.WriteLine($"Найдено заказов после фильтрации: {result.Count}");
            foreach (var order in result)
            {
                Console.WriteLine($"OrderNumber: {order.OrderId}, DeliveryTime: {order.DeliveryDateTime}");
            }

            return result;
        }

        public void SaveOrders(List<Order> orders)
        {
            try
            {
                var jsonData = JsonConvert.SerializeObject(orders, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(_outputPath, jsonData);
                Log("Результат фильтрации успешно сохранен.");
            }
            catch (Exception ex)
            {
                Log($"Ошибка сохранения результатов: {ex.Message}");
            }
        }
    }
}
