using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Editor
{
    public class UnderlineText: TextDecorator 
    {
        public UnderlineText(IText text) : base(text) { }
        public override string Format() => $"<u{base.Format()}/u>";//$"\x1b[4m{base.Format()}\x1b[0m"
    }
}
