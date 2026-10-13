using MyEntity;

namespace MyUI
{
    /// <summary>
    /// 当前登录用户上下文
    /// </summary>
    public static class SessionContext
    {
        public static UsersEntity CurrentUser { get; set; }

        public static string DisplayName
        {
            get
            {
                if (CurrentUser == null) return string.Empty;
                return string.IsNullOrEmpty(CurrentUser.RealName)
                    ? CurrentUser.UserName
                    : CurrentUser.RealName;
            }
        }

        public static bool IsAdmin
        {
            get { return CurrentUser != null && CurrentUser.Role == "Admin"; }
        }
    }
}
