using System.Collections.Generic;

namespace Library.Core
{
    public class BookList
    {
        public List<LibraryBook> Books { get; set; }
        public int ItemsPerPage { get; set; }
        public int PageNo { get; set; }
        public int NoPages { get; set; }
        public string Search { get;set; }
    }
}
