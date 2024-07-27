using System.Collections.Generic;

namespace LearnOnline.Models
{
    public class InvoiceViewModel
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public List<Cart> Items { get; set; }
        
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public int UserId { get; set; }
    }
}
