﻿using ViandasDelSur.Models.Enums;

namespace ViandasDelSur.Models
{
    public class SaleData
    {
        public int Id { get; set; }
        public string productName { get; set; }
        public DateTime validDate { get; set; }
        public DateTime day { get; set; }
        public string category { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
        public PaymentMethod paymentMethod { get; set; }
    }
}
