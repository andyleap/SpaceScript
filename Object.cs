using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLCD.SpaceScript.Types
{
    class Object : IType, IFields, IEnumerable<KeyValuePair<string, IType>>
    {
        Dictionary<string, IType> field = new Dictionary<string, IType>();

        public IEnumerator<KeyValuePair<string, IType>> GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, IType>>)field).GetEnumerator();
        }

        public IType GetField(Scope scope, State state, string name)
        {
            if(!field.ContainsKey(name))
            {
                return null;
            }
            return field[name];
        }

        public void SetField(Scope scope, State state, string name, IType value)
        {
            if(!field.ContainsKey(name))
            {
                field.Add(name, value);
            }
            else
            {
                field[name] = value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, IType>>)field).GetEnumerator();
        }

        public void Add(string key, IType value)
        {
            field.Add(key, value);
        }
    }
}
