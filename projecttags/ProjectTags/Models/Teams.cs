using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectTags.Models
{
    /// <summary>
    /// 成员信息
    /// </summary>
    public class Teams : BaseObj
    {
        /// <summary>
        /// 项目ID
        /// </summary>
        public long ProjectID { get; set; }
        /// <summary>
        /// 成员ID
        /// </summary>
        public long UserID { get; set; }

        /// <summary>
        /// 任务信息
        /// </summary>
        [ForeignKey("ProjectID")]
        public virtual Projects Project { get; set; }
        /// <summary>
        /// 状态信息
        /// </summary>
        [ForeignKey("UserID")]
        public virtual Users User { get; set; }
    }
}