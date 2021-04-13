using SQLite;

namespace Shop_site_parser.Model
{
    class ProductModel
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }
        public string link { get; set; }
        [Unique]
        public int product_id { get; set; }
        public bool actual { get; set; }

    }
}
