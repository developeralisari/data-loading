namespace data_loading.Models
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }

        // This should be virtual for lazy loading to work
        public virtual ICollection<Book> Books { get; set; }
    }
}
