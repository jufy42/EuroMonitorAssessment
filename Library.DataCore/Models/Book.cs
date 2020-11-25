using System;
using System.Collections.Generic;

namespace Library.DataCore
{
    public class Book
    {
        public Guid BookID { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public double PurchasePrice { get; set; }
        public bool Active { get; set; }
        public string ImageName { get; set; }
        public string Author { get; set; }

        public ICollection<UserBook> UserBooks { get; set; }
    }
}
