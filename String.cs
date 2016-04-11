using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLCD.SpaceScript.Types
{
    class String : IType, IFields
    {
        public string Value;

        public String(string Value)
        {
            this.Value = Value;
        }

        public override string ToString()
        {
            return Value;
        }

        public override IType Add(IType i2)
        {
            return new String(this.Value + i2.ToString());
        }

        public override bool Equals(object obj)
        {
            var strobj = obj as String;
            if (strobj != null)
            {
                return Value == strobj.Value;
            }
            return base.Equals(obj);
        }

		public IType GetField(Scope scope, State state, string name)
		{
			if (name == "Contains")
			{
				return new NativeFunction(args =>
				{
					if (args.Count != 1 || !(args[0] is String))
					{
						throw new Exception("String.Contains expects a single string argument");
					}
					if (Value.Contains((args[0] as String).Value))
					{
						return new Bool(true);
					}
					return new Bool(false);
				});
			}
			throw new Exception(string.Format("Field {0} does not exist on type String", name));
		}

		public void SetField(Scope scope, State state, string name, IType value)
		{
			throw new Exception(string.Format("Field {0} does not exist on type String", name));
		}
	}
}
