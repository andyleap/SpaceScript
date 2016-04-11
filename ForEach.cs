using ScriptLCD.SpaceScript.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptLCD.SpaceScript
{
	class ForEach : RValue
	{
		Variable Item;
		RValue List;
		RValue Loop;

		public ForEach(Variable item, RValue list, RValue loop)
		{
			Item = item;
			Item.Declare = true;
			List = list;
			Loop = loop;
		}

		public IEnumerable<Node> Resolve(Scope scope, State state, Action<IType> Result)
		{
			IIndex listVal = null;
			int len = 0;
			yield return (sc, st) => List.Resolve(sc, st, r => {

				if (!(r is IIndex))
				{
					throw new Exception(string.Format("{0} is not indexable", listVal));
				}
				listVal = r as IIndex;
				len = (listVal.GetLength(sc, st) as Integer).Value;
			});
			IType val = null;
			for(int l1 = 0; l1 < len; l1++)
			{
				scope.Push();
				try {
					yield return (sc, st) => Item.Set(sc, st, listVal.GetIndex(sc, st, l1));
					yield return (sc, st) => Loop.Resolve(sc, st, r => { val = r; });
				}
				finally
				{
					scope.Pop();
				}
				
			}
			
			Result(val);
		}
	}
}
