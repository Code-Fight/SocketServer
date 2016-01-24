using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Comm
{
    public class SocketInfo
    {
        /// <summary>
        /// 客户端名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 操作
        /// </summary>
        public SocketOrder Order { get; set; }

        /// <summary>
        /// 目标服务器
        /// </summary>
        public string AimName { get; set; }

        /// <summary>
        /// 值或信息
        /// </summary>
        public string DataInfo { get; set; }
    }

    /// <summary>
    /// 命令
    /// </summary>
    public enum SocketOrder
    {
        /// <summary>
        /// 登录
        /// </summary>
        Login = 20001,
        /// <summary>
        /// 下线
        /// </summary>
        Logout = 20002,
        /// <summary>
        /// 服务器下线
        /// </summary>
        ServerLogout = 20003,

        /// <summary>
        /// 开始注册
        /// </summary>
        StartReg = 10001,
        /// <summary>
        /// 注册成功
        /// </summary>
        RegSuccess = 10002,
        /// <summary>
        /// 开始出乘考勤
        /// </summary>
        StartCckq = 10003,
        /// <summary>
        /// 出乘考勤成功
        /// </summary>
        CckqSuccess = 10004,
        /// <summary>
        /// 开始退乘考勤
        /// </summary>
        StartTckq = 10005,
        /// <summary>
        /// 退乘考勤成功
        /// </summary>
        TckqSuccess = 10006,
        /// <summary>
        /// 下载开始
        /// </summary>
        StartDown = 10007,
        /// <summary>
        /// 下载完成
        /// </summary>
        DownSuccess = 10008
    }
}
