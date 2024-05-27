namespace LibApp.Converters
{
    public static class ConvertersStore
    {
        public static NumberUpDownValueConverter NumberUpDownValueConverter { get; } = new();
        public static BookIdToIsFavouriteConverter BookIdToIsFavouriteConverter { get; } = new();
        public static BookIdToIsInCartConverter BookIdToIsInCartConverter { get; } = new();
        public static InputStrToStrArrayConverter InputStrToStrArrayConverter { get; } = new();
        public static BookIdToBookConverter BookIdToBookConverter { get; } = new();
        public static BookIdToCartCountConverter BookIdToCartCountConverter { get; } = new();
        public static OrderStatusToStringConverter OrderStatusToStringConverter { get; } = new();
        public static EnumerableToIsEmptyConverter EnumerableToIsEmptyConverter { get; } = new();
        public static StringIsEmptyConverter StringIsEmptyConverter { get; } = new();
    }
}
