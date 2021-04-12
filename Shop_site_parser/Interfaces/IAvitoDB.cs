using System.Collections.Generic;
using Shop_site_parser.Model;

namespace Shop_site_parser.Interfaces
{
    interface IAvitoDB
    {
        int WriteItem(AvitoDBModel _newItem);
        List<AvitoDBModel> GetDataList();
    }
}
