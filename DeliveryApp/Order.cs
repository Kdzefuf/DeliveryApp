﻿namespace DeliveryApp
{

    public class Order
    {
        public string OrderId { get; set; }
        public double Weight { get; set; }
        public string District { get; set; }
        public DateTime DeliveryDateTime { get; set; }
    }
}
