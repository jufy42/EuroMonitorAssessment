using System;

namespace Library.DataCore
{
    public class UserBook
    {
        public Guid UserID { get; set; }
        public User User { get; set; }
        public Guid BookID { get; set; }
        public Book Book { get; set; }
    }
}
