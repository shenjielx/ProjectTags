using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectTags.Models
{
    /// <summary>
    /// 任务
    /// </summary>
    public class Tasks : BaseObj
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        public long ProjectID { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        [MaxLength(320)]
        public string Name { get; set; }
        /// <summary>
        /// 任务简介
        /// </summary>
        [MaxLength(2000)]
        public string Desc { get; set; }

        /// <summary>
        /// 项目信息
        /// </summary>
        [ForeignKey("ProjectID")]
        public virtual Projects Project { get; set; }
    }
}