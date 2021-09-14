using System.Text.RegularExpressions;
using ProductApi.Models;

namespace ProductApi.Misc
{
    public static class ProductExtensions
    {
        public static void UpdateSku(this Product product)
        {
            product.Sku = $"{product.Manufacturer.Abbreviation}-{product.Type.Code}-{product.Subtype.Code}-{Regex.Replace(product.Name, @"[aeyuoiAEYUOI'-_?!#().,`~\s]", string.Empty)}".ToUpper();
        }
    }
}
