using System.Collections.Generic;
using Shop_site_parser.Model;

namespace Shop_site_parser.Interfaces
{
    interface IAvitoDB
    {
        AvitoDBModel GetByItemID(int product_id);
        int WriteItem(AvitoDBModel _newItem);
        List<AvitoDBModel> GetDataList();
        int UpdateItem(AvitoDBModel _updateItem);
    }
}
