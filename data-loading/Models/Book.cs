namespace data_loading.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }

        // This should be virtual for lazy loading to work
        public virtual Author Author { get; set; }
    }
}
