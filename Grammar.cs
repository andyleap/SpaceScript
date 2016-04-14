using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptLCD.SpaceScript.Parser;

namespace ScriptLCD.SpaceScript
{
    class Grammar
    {
        public static Parser<string> whitespace =
            from ws in Parse.Set(" \n\t").Mult(1).String()
			from comment in Parse.And(Parse.Literal("//"), Parse.NotSet("\n").Mult().String(), Parse.Literal("\n")).Optional()
			select ws;

		public static Parser<string> optionalWhitespace =
			from ws in Parse.Set(" \n\t").Mult().String()
			from comment in Parse.And(Parse.Literal("//"), Parse.NotSet("\n").Mult().String(), Parse.Literal("\n")).Optional()
			select ws;

        public static Parser<string> identifier =
            from leadingWhitespace in optionalWhitespace
            from leading in Parse.Set("a-zA-Z")
            from rest in Parse.Set("a-zA-Z0-9_").Mult().String()
            from trailingWhitespace in optionalWhitespace
            select leading.ToString() + rest;

        public static Parser<RValue> ConstantBoolTrue =
            from True in Parse.Literal("true")
            select new Constant(new Types.Bool(true));
        public static Parser<RValue> ConstantBoolFalse =
            from True in Parse.Literal("false")
            select new Constant(new Types.Bool(false));
        public static Parser<RValue> ConstantBool =
            Parse.Or(ConstantBoolTrue, ConstantBoolFalse);

        public static Parser<RValue> ConstantInt =
            from number in Parse.Set("0-9").Mult(1).String()
            select new Constant(new Types.Integer(int.Parse(number)));

        public static Parser<RValue> ConstantFloat =
            from number in Parse.Set("0-9").Mult(1).String()
            from period in Parse.Literal(".")
            from fractional in Parse.Set("0-9").Mult(1).String()
            select new Constant(new Types.Float(float.Parse(number + "." + fractional)));

        public static Parser<RValue> ConstantColor =
            from pound in Parse.Literal("#")
            from R in Parse.Set("0-9a-fA-F").Mult(2, 2).String()
            from G in Parse.Set("0-9a-fA-F").Mult(2, 2).String()
            from B in Parse.Set("0-9a-fA-F").Mult(2, 2).String()
            from A in Parse.Set("0-9a-fA-F").Mult(2, 2).String().Optional("FF")
            select new Constant(new Types.Color(Convert.ToByte(R, 16), Convert.ToByte(G, 16), Convert.ToByte(B, 16), Convert.ToByte(A, 16)));

		public static Parser<RValue> ConstantString =
			from openquote in Parse.Literal("\"")
			from content in Parse.Or(Parse.NotSet("\\\""), Parse.Literal("\\\"").Select(s => '"')).Mult().String()
			from closequote in Parse.Literal("\"")
			select new Constant(new Types.String(content));

        public static Parser<RValue> Constant =
            Parse.Or(ConstantBool, ConstantFloat, ConstantInt, ConstantColor, ConstantString);

        public static Parser<RValue> Variable =
            from name in identifier
            select new Variable(name);

        public static Parser<GroupAccess> GroupAccess =
            from OpenAngle in Parse.Literal("*")
            from Name in Parse.NotSet("*").Mult().String()
            from CloseAngle in Parse.Literal("*")
            select new GroupAccess(Name);

        public static Parser<string> Argument =
            from ws1 in optionalWhitespace
            from arg in identifier
            from ws2 in optionalWhitespace
            from comma in Parse.Literal(",")
            from ws3 in optionalWhitespace
            select arg;

        public static Parser<List<string>> ArgumentList =
             from first in Argument.Mult()
             from final in identifier
             from ws in optionalWhitespace
             select first.Concat(final);

        public static Parser<RValue> FunctionDec =
            from ws1 in optionalWhitespace
            from funcword in Parse.Literal("function")
            from ws2 in whitespace
            from funcName in identifier
            from argStart in Parse.Literal("(")
            from args in ArgumentList.Optional()
            from argEnd in Parse.Literal(")")
            from body in Parse.Ref(() => Expr)
            select new Assign(new Variable(funcName), new Closure(new Types.UserFunction(body, args)), true);

        public static Parser<RValue> LambdaDec =
            from ws1 in optionalWhitespace
            from argStart in Parse.Literal("(")
            from args in ArgumentList.Optional()
            from argEnd in Parse.Literal(")")
            from ws2 in optionalWhitespace
            from lambdaOp in Parse.Literal("=>")
            from ws3 in optionalWhitespace
            from body in Parse.Ref(() => Expr)
            select new Closure(new Types.UserFunction(body, args));

        public static Parser<RValue> Negate =
            from negate in Parse.Literal("-")
            from Base in Parse.Ref(() => Base)
            select new Negate(Base);

		public static Parser<RValue> BoolNegate =
			from negate in Parse.Literal("!")
			from Base in Parse.Ref(() => Base)
			select new BoolNegate(Base);

		public static Parser<RValue> ParenExpr =
            from openParen in Parse.Literal("(")
            from expr in Parse.Ref(() => Expr)
            from closeParen in Parse.Literal(")")
            select expr;

        public static Parser<RValue> Base =
            from target in Parse.Or<RValue>(Parse.Ref(() => ExprBlock), GroupAccess, Parse.Ref(() => If), Parse.Ref(() => While), Parse.Ref(() => ForEach), Constant, FunctionDec, LambdaDec, Variable, Negate, BoolNegate, ParenExpr)
            select target;

        public static Parser<Func<RValue, RValue>> IndexAccess =
            from openBracket in Parse.Literal("[")
            from index in Parse.Ref(() => Expr)
            from closeBracket in Parse.Literal("]")
            select (Func<RValue, RValue>)(t => new IndexAccess(t, index));

        public static Parser<Func<RValue, RValue>> FieldAccess =
            from period in Parse.Literal(".")
            from field in Parse.Set("a-zA-Z0-9_").Mult().String()
            select (Func<RValue, RValue>)(t => new FieldAccess(t, field));

        public static Parser<Func<RValue, RValue>> MethodCall =
            from openParen in Parse.Literal("(")
            from parameters in ParameterList.Optional()
            from closeParen in Parse.Literal(")")
            select (Func<RValue, RValue>)(t => new MethodCall(t, parameters));

        public static Parser<Func<RValue, RValue>> Assign =
            from ws1 in optionalWhitespace
            from equal in Parse.Literal("=")
            from ws2 in optionalWhitespace
            from expr in Parse.Ref(() => Expr)
            select (Func<RValue, RValue>)(t => new Assign(t, expr, false));

        public static Parser<Func<RValue, RValue>> DecAssign =
            from ws1 in optionalWhitespace
            from equal in Parse.Literal(":=")
            from ws2 in optionalWhitespace
            from expr in Parse.Ref(() => Expr)
            select (Func<RValue, RValue>)(t => new Assign(t, expr, true));

        public static Parser<Func<RValue, RValue>> Mutator =
            from mutator in Parse.Or(MethodCall, FieldAccess, IndexAccess, Assign, DecAssign)
            select mutator;

        public static Parser<RValue> Operand =
            from leadingWhitespace in optionalWhitespace
            from Base in Base
            from mutators in Mutator.Mult()
            from trailingwhitespace in optionalWhitespace
            select Expression.Apply(Base, mutators);

        public static Parser<Func<RValue, RValue>> FactorRest =
            from ws1 in optionalWhitespace
            from op in Parse.Set("/*")
            from ws2 in optionalWhitespace
            from operand in Operand
            select (Func<RValue, RValue>)(t => new MathOp(t, op, operand));

        public static Parser<RValue> Factor =
            from ws1 in optionalWhitespace
            from operand in Operand
            from factorRest in FactorRest.Mult()
            from ws2 in optionalWhitespace
            select Expression.Apply(operand, factorRest);

        public static Parser<Func<RValue, RValue>> TermRest =
            from ws1 in optionalWhitespace
            from op in Parse.Set("-+")
            from ws2 in optionalWhitespace
            from factor in Factor
            select (Func<RValue, RValue>)(t => new MathOp(t, op, factor));

        public static Parser<RValue> Term =
            from ws1 in optionalWhitespace
            from factor in Factor
            from termRest in TermRest.Mult()
            from ws2 in optionalWhitespace
            select Expression.Apply(factor, termRest);

        public static Parser<Func<RValue, RValue>> PreExprRest =
            from ws1 in optionalWhitespace
            from op in Parse.Or(Parse.Literal("=="), Parse.Literal("!="), Parse.Literal(">="), Parse.Literal("<="), Parse.Literal(">"), Parse.Literal("<"))
            from ws2 in optionalWhitespace
            from term in Term
            select (Func<RValue, RValue>)(t => new CompOp(t, op, term));

        public static Parser<RValue> PreExpr =
            from ws1 in optionalWhitespace
            from term in Term
            from exprRest in PreExprRest.Mult()
            from ws2 in optionalWhitespace
            select Expression.Apply(term, exprRest);

		public static Parser<Func<RValue, RValue>> ExprRest =
			from ws1 in optionalWhitespace
			from op in Parse.Or(Parse.Literal("&&"), Parse.Literal("||"))
			from ws2 in optionalWhitespace
			from term in PreExpr
			select (Func<RValue, RValue>)(t => new BoolOp(t, op, term));

		public static Parser<RValue> Expr =
			from ws1 in optionalWhitespace
			from term in PreExpr
			from exprRest in ExprRest.Mult()
			from ws2 in optionalWhitespace
			select Expression.Apply(term, exprRest);

		public static Parser<RValue> Parameter =
            from exp in Expr
            from ws1 in Parse.Set(" ").Mult()
            from comma in Parse.Literal(",")
            from ws2 in Parse.Set(" ").Mult()
            select exp as RValue;

        public static Parser<List<RValue>> ParameterList =
            from paramList in Parameter.Mult()
            from final in Expr
            select paramList.Concat(final);

        public static Parser<RValue> TerminatedExpr =
            from exp in Expr
            from end in Parse.Literal(";")
            from ws in optionalWhitespace
            select exp;

        public static Parser<RValue> ExpressionList =
            from exp in Parse.Or(TerminatedExpr, Parse.Ref(() => If), Parse.Ref(() => While), Parse.Ref(() => ForEach), Parse.Ref(() => FunctionDec)).Mult()
            select new ExprBlock(exp);

        public static Parser<RValue> ExprBlock =
            from ws1 in optionalWhitespace
            from OpenBrace in Parse.Literal("{")
            from ws2 in optionalWhitespace
            from ExprBlock in ExpressionList
            from ws3 in optionalWhitespace
            from CloseBrace in Parse.Literal("}")
            from ws4 in optionalWhitespace
            select ExprBlock;

        public static Parser<RValue> If =
            from ws1 in optionalWhitespace
            from If in Parse.Literal("if")
            from cond in Expr
            from truebranch in Expr
            from falsebranch in Parse.Literal("else").Then(s => Expr).Optional()
            select new If(cond, truebranch, falsebranch);

        public static Parser<RValue> While =
            from ws1 in optionalWhitespace
            from If in Parse.Literal("while")
            from cond in Expr
            from loop in Expr
            select new While(cond, loop);

		public static Parser<RValue> ForEach =
			from ws1 in optionalWhitespace
			from ForEach in Parse.Literal("foreach")
			from item in Variable
			from In in Parse.Literal("in")
			from list in Expr
			from loop in Expr
			select new ForEach(item as Variable, list, loop);

	}
}
