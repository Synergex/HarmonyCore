using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HarmonyCoreCodeGenGUI
{
    class NotificationMessageAction<T>
    {
        public object target;
        public Action<T> callback;
        public NotificationMessageAction(object target, Action<T> callback)
        {
            this.callback = callback;
            this.target = target;
        }
    }
}
