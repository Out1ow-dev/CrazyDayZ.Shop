using Microsoft.AspNetCore.Mvc.Rendering;

namespace CrazyDayZ.Shop.Pages.AdminPanel
{
    public static class AdminManageNavPages
    {

        public static string Index => "Index";

        public static string Users => "UserManage";

        public static string CategoriesManage => "CategoriesManage";

        public static string ProductManage => "ProductManage";

        public static string ServerManage => "ServerManage";

        public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

        public static string UsersNavClass(ViewContext viewContext) => PageNavClass(viewContext, Users);

        public static string CategoriesManageNavClass(ViewContext viewContext) => PageNavClass(viewContext, CategoriesManage);

        public static string ProductsManageNavClass(ViewContext viewContext) => PageNavClass(viewContext, ProductManage);

        public static string ServerManageNavClass(ViewContext viewContext) => PageNavClass(viewContext, ServerManage);



        public static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
