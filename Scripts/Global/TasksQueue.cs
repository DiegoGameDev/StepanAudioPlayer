using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace StepanPlayer.Global
{
    public static class TasksQueue
    {
        public static ConcurrentQueue<Action> Queue = new ConcurrentQueue<Action>();
    }
}
