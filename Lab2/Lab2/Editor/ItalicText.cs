using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Editor
{
    public class ItalicText: TextDecorator
    {
        public ItalicText(IText text) : base(text) { }
        public override string Format() => $"<i{base.Format()}/i>";//$"\x1b[3m{base.Format()}\x1b[0m"
    }
}
