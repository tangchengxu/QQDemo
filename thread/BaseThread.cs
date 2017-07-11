using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;

namespace QQDemo.thread
{
    public abstract class BaseThread
    {
        protected abstract void ThreadEntrance();

        // 开启线程
        public void Start()
        {
            if (mIsRuning)
            {
                return;
            }

            new Thread(new ThreadStart(this.ThreadEntrance)).Start();
            mIsRuning = true;
        }

        // 停止线程
        public void Stop()
        {
            mIsStop = true;
        }

        // 是否需要停止线程
        public bool IsNeedStop()
        {
            return mIsStop;
        }

        protected bool mIsRuning = false;
        protected bool mIsStop = false;
    }
}
