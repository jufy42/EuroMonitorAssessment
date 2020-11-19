using System;

namespace Library.DataCore
{
    public class Book
    {
        public Guid BookID { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public double PurchasePrice { get; set; }
    }
}
