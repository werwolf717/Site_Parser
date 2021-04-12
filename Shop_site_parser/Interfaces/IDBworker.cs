namespace Shop_site_parser.Interfaces
{
    interface IDBworker
    {
        int RemoveItem(int _id);
        void CloseDBConnection();
    }
}
