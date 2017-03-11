using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logging.Server.Entitys
{
    public class LogVM
    {
        public long Start { get; set; }

        public long End { get; set; }

        public long Cursor { get; set; }

        public List<LogEntity> List { get; set; }
    }
}
