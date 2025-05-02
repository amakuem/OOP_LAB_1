using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2.Editor
{
    public abstract class TextDecorator: IText
    {
        protected IText _text;
        public TextDecorator(IText text) => _text = text;
        public virtual string Format() => _text.Format();
    }
}
