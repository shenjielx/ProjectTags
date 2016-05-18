using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectTags.Models
{
    public class Users
    {
        /// <summary>
        ///  主键
        /// </summary>
        [Key]
        public long ID { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }
        /// <summary>
        /// 最后操作IP
        /// </summary>
        [MaxLength(32)]
        public string WebClientIP { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        [MaxLength(320)]
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [MaxLength(320)]
        public string Password { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [MaxLength(320)]
        public string Email { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [MaxLength(32)]
        public string Mobile { get; set; }
        /// <summary>
        /// 真实姓名
        /// </summary>
        [MaxLength(150)]
        public string Name { get; set; }
        /// <summary>
        /// 性别（3-保密，1-男，2-女）
        /// </summary>
        public UsersGender Gender { get; set; }

        /// <summary>
        /// 等级（0-普通用户，1-管理员，2-超级管理员）
        /// </summary>
        public int Rank { get; set; }
    }

    #region 用户性别
    /// <summary>
    /// 用户性别
    /// </summary>
    public enum UsersGender
    {
        DEFAULT = 0,
        /// <summary>
        /// 男
        /// </summary>
        MALE = 1,
        /// <summary>
        /// 女
        /// </summary>
        FEMALE = 2,
        /// <summary>
        /// 保密
        /// </summary>
        SECRET = 3,
    }
    #endregion

    #region 用户等级
    /// <summary>
    /// 用户等级
    /// </summary>
    public enum UsersRank
    {
        /// <summary>
        /// 普通用户
        /// </summary>
        DEFAULT = 0,
        /// <summary>
        /// 管理员
        /// </summary>
        ADMIN = 1,
        /// <summary>
        /// 超级管理员
        /// </summary>
        SUPER = 2,
    }
    #endregion

}