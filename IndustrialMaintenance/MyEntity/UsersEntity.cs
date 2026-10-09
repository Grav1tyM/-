using System;

namespace MyEntity
{
    /// <summary>
    /// 用户实体，对应表 Users
    /// </summary>
    public class UsersEntity
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string RealName { get; set; }
        public string Role { get; set; }
        public string Phone { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
