namespace DeliveryBro.Areas.store.DTO
{
    public class DishMonthlyChartDTO
    {
        public int? Month { get;set; }
        public List<DishEChartsDTO>? Dish { get; set; }
        public int? Number { get; set; }
    }
}
