using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectTags.Models
{
    /// <summary>
    /// 项目
    /// </summary>
    public class Projects : BaseObj
    {
        /// <summary>
        /// 项目名称
        /// </summary>
        [MaxLength(150)]
        public string Name { get; set; }
        /// <summary>
        /// 项目简介
        /// </summary>
        [MaxLength(500)]
        public string Desc { get; set; }
    }
}