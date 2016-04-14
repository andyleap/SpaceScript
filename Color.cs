using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLCD.SpaceScript.Types
{
    class Color : IType, IFields
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Color(byte R, byte G, byte B, byte A)
        {
            this.R = R;
            this.G = G;
            this.B = B;
            this.A = A;
        }

        public override string ToString()
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", R, G, B, A);
        }

        public override bool Equals(object obj)
        {
            var colorobj = obj as Color;
            if (colorobj != null)
            {
                return R == colorobj.R && G == colorobj.G && B == colorobj.B && A == colorobj.A;
            }
            return base.Equals(obj);
        }

		public IType GetField(Scope scope, State state, string name)
		{
			if(name == "R")
			{
				return new Integer(R);
			}
			if (name == "G")
			{
				return new Integer(G);
			}
			if (name == "B")
			{
				return new Integer(B);
			}
			if (name == "A")
			{
				return new Integer(A);
			}
			throw new Exception("type Color has no field '" + name + "'");
		}

		public void SetField(Scope scope, State state, string name, IType value)
		{
			if(name == "R")
			{
				R = (byte)value.Cast<Integer>().Value;
				return;
			}
			if (name == "G")
			{
				G = (byte)value.Cast<Integer>().Value;
				return;
			}
			if (name == "B")
			{
				B = (byte)value.Cast<Integer>().Value;
				return;
			}
			if (name == "A")
			{
				A = (byte)value.Cast<Integer>().Value;
				return;
			}

			throw new Exception("type Color has no field '" + name + "'");
		}
	}
}
