using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectTags.Models
{
    /// <summary>
    /// 项目开发流程状态
    /// </summary>
    public class Status : BaseObj
    {
        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// 状态名称
        /// </summary>
        [MaxLength(32)]
        public string Name { get; set; }
        /// <summary>
        /// 状态描述
        /// </summary>
        [MaxLength(320)]
        public string Desc { get; set; }
        /// <summary>
        /// 前置ID
        /// </summary>
        public long? PrevID { get; set; }
        /// <summary>
        /// 后置ID
        /// </summary>
        public long? NextID { get; set; }
        /// <summary>
        /// 分组（0-默认，1-研发）
        /// </summary>
        public int Group { get; set; }
    }
}