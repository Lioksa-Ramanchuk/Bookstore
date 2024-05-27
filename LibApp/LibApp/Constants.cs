using System.Globalization;
using System.Text.RegularExpressions;

namespace LibApp
{
    public static partial class Constants
    {
        public const string ProcessingOrderStatusName = "processing";
        public const string ShippedOrderStatusName = "shipped";
        public const string DeliveredOrderStatusName = "delivered";
        public const string CanceledOrderStatusName = "canceled";
        public const string ReturnedOrderStatusName = "returned";

        public static Regex LoginRgx { get; } = LoginRegex();
        public static Regex PasswordRgx { get; } = PasswordRegex();
        public static Regex EmailRgx { get; } = EmailRegex();
        public static int MinPasswordLength { get; } = 4;
        public static int MaxLoginLength { get; } = 255;
        public static int MaxPasswordLength { get; } = 255;
        public static string DefaultAdministratorLogin { get; } = "admin";
        public static string DefaultAdministratorPassword { get; } = "1111";
        public static string AdministratorRoleName { get; } = "Administrator";
        public static string UserRoleName { get; } = "User";
        public static CultureInfo CurrentCulture { get; } = new CultureInfo("be-BY");
        public static short MaxBookYear { get; } = 9999;
        public static short MaxBookPages { get; } = 9999;
        public static decimal MaxBookPrice { get; } = 999_999_999.99m;
        public static int MaxBookCount { get; } = 999_999_999;
        public static int MaxBookIsbnLength { get; } = 20;
        public static int MaxBookNameLength { get; } = 255;
        public static int MaxBookAuthorLength { get; } = 255;
        public static int MaxBookPublisherLength { get; } = 255;
        public static int MaxBookDescriptionLength { get; } = 1000;
        public static int MaxBookTagsLength { get; } = 511;
        public static string BookDefaultImagePath { get; } = "../../../images/undefined_img.png";
        public static int MaxBooksNumberOnPageInCatalogue { get; } = 6;
        public static int MaxBooksNumberOnPageInFavourite { get; } = 6;
        public static int MaxBooksNumberOnPageInCart { get; } = 6;
        public static int MaxOrdersNumberOnPageInOrdersList { get; } = 4;
        public static int MaxBooksNumberOnPageInOrder { get; } = 6;
        public static int MaxUserNameLength { get; } = 255;
        public static int MaxEmailLength { get; } = 255;
        public static int MaxAddressLength { get; } = 255;
        public static int MaxCommentLength { get; } = 511;
        public static int MaxNotEnoughBookIdsDisplayedNumber { get; } = 3;
        public static int MaxNotEnoughBooksDisplayedNumber { get; } = 3;
        public static int MaxGeneralTextBoxInputLength { get; } = 1000;

        [GeneratedRegex(@"^[a-zA-Z0-9_]{1,255}$")]
        private static partial Regex LoginRegex();
        [GeneratedRegex(@"^[a-zA-Z0-9_!@#$%^&*-]{4,255}$")]
        private static partial Regex PasswordRegex();
        [GeneratedRegex(@"^(?=.{1,255}$)[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")]
        private static partial Regex EmailRegex();
    }
}