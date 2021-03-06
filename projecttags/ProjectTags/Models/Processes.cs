﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectTags.Models
{
    /// <summary>
    /// 项目流程
    /// </summary>
    public class Processes : BaseObj
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        public long TaskID { get; set; }
        /// <summary>
        /// 状态ID
        /// </summary>
        public long StateID { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string Remark { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public long? UserID { get; set; }

        /// <summary>
        /// 任务信息
        /// </summary>
        [ForeignKey("TaskID")]
        public virtual Tasks Task { get; set; }
        /// <summary>
        /// 状态信息
        /// </summary>
        [ForeignKey("StateID")]
        public virtual Status State { get; set; }
        /// <summary>
        /// 用户信息
        /// </summary>
        [ForeignKey("UserID")]
        public virtual Users User { get; set; }
    }
}