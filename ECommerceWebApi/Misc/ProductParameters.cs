using System;

namespace ProductApi.Misc
{
    public class ProductParameters
    {
        const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get => _pageSize;

            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public decimal MinCost { get; set; } = 0;
        public decimal MaxCost { get; set; } = 922337203685477;
        
        internal bool ValidCost => MaxCost >= MinCost;

        public string TypeName { get; set; }
    }
}
