using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQDemo.common
{
    public class Global
    {
        public const int s_QQServerId = 960;
        public const int s_QQMessageTopic = s_QQServerId << 20;
        public const int s_QQDataTopic = (s_QQServerId << 20) | 0x00002;
        public const int s_QQContentTopic = (s_QQServerId << 20) | 0x00003;
        public const int s_QQMessageKey = 1;
        public const int s_QQContentKey = 1;
        public const int s_QQNameKey = 3;

        public const int timeDiff = 8 * 60 * 60 * 1000; 
    }
}
