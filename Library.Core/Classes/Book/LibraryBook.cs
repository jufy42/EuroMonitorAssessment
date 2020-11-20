using System;

namespace Library.Core
{
    public class LibraryBook
    {
        public Guid BookID { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public double PurchasePrice { get; set; }
        public bool? IsSubscribed { get; set; }
    }
}
