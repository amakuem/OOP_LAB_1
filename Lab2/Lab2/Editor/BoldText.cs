using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Editor
{
    public class BoldText: TextDecorator
    {
        public BoldText(IText text) : base(text) { }
        public override string Format() => $"<b{base.Format()}/b>";//$"\x1b[1m{base.Format()}\x1b[0m"
    }
}
