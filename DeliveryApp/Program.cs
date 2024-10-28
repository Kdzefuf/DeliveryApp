using Newtonsoft.Json;

namespace DeliveryApp
{
    class Program
    {
        private static string configPath = "config/app.json";
        private static string fullPath = System.IO.Path.GetFullPath(configPath);
        private static Dictionary<string, string> config = LoadConfiguration(fullPath);

        private static string ordersFilePath = config["OrdersFile"];
        private static string logPath = config["DeliveryLog"];
        private static string outputPath = config["DeliveryOrder"];

        static void Main(string[] args)
        {
            if (config == null)
            {
                Console.WriteLine("Ошибка загрузки конфигурации.");
                return;
            }

            var orderService = new OrderService(logPath, outputPath);

            while (true)
            {
                PrintAppText(orderService);
                if (Console.ReadLine()?.ToLower() != "да")
                {
                    break;
                }
            }
        }

        static void PrintAppText(OrderService orderService)
        {
            Console.WriteLine("Вы хотите добавить заказ? (да/нет): ");

            if (Console.ReadLine()?.ToLower() == "да")
            {
                var newOrder = GetOrderInput();
                orderService.AddOrder(newOrder, ordersFilePath);
                Console.WriteLine("Заказ успешно добавлен.");
            }

            while (true)
            {
                Console.WriteLine("Вы хотите провести фильтрацию? (да/нет): ");
                if (Console.ReadLine()?.ToLower() != "да")
                {
                    break;
                }
                FilterAndSaveOrders(orderService, ordersFilePath);
            }

            Console.WriteLine("\nХотите добавить еще один заказ? (да/нет): ");
        }

        static Dictionary<string, string> LoadConfiguration(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"Файл конфигурации '{path}' не найден.");
                return null;
            }

            try
            {
                var jsonData = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка чтения конфигурационного файла: {ex.Message}");
                return null;
            }
        }

        static Order GetOrderInput()
        {
            Console.WriteLine("\nСоздание нового заказа");

            var orderId = Guid.NewGuid().ToString();

            Console.Write("Введите вес заказа в килограммах: ");
            double orderWeight;
            while (!double.TryParse(Console.ReadLine(), out orderWeight))
            {
                Console.Write("Неверный формат. Введите вес заказа (число): ");
            }

            Console.Write("Введите район доставки: ");
            string district = Console.ReadLine();

            DateTime deliveryDateTime;
            while (true)
            {
                Console.Write("Введите время доставки (yyyy-MM-dd HH:mm:ss): ");
                if (DateTime.TryParse(Console.ReadLine(), out deliveryDateTime))
                    break;
                Console.WriteLine("Неверный формат даты. Повторите ввод.");
            }

            return new Order
            {
                OrderId = orderId,
                Weight = orderWeight,
                District = district,
                DeliveryDateTime = deliveryDateTime
            };
        }

        static void FilterAndSaveOrders(OrderService orderService, string ordersFilePath)
        {
            Console.WriteLine("Введите район для фильтрации: ");
            string filterDistrict = Console.ReadLine();

            DateTime firstDeliveryTime;
            while (true)
            {
                Console.WriteLine("Введите время первой доставки в формате yyyy-MM-dd HH:mm:ss: ");
                if (DateTime.TryParse(Console.ReadLine(), out firstDeliveryTime))
                    break;
                Console.WriteLine("Неверный формат даты. Пожалуйста, повторите ввод.");
            }

            try
            {
                var orders = orderService.LoadOrders(ordersFilePath);
                var filteredOrders = orderService.FilterOrders(orders, filterDistrict, firstDeliveryTime);
                orderService.SaveOrders(filteredOrders);
                Console.WriteLine("Фильтрация завершена. Результаты сохранены.");
            }
            catch (Exception ex)
            {
                orderService.Log($"Ошибка выполнения: {ex.Message}");
                Console.WriteLine("Произошла ошибка. Подробности в лог-файле.");
            }
        }
    }
}
