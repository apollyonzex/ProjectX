

using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Foundation.Tables
{



    public interface IExprTree
    {
        int accept(IExprTreeVisitor visitor);
    }

    public interface IExprTreeVisitor
    {
        int visit_value(IExprTreeValue value);
        int visit_ident(IExprTreeIdent ident);
        int visit_call(IExprTreeIdent ident, IExprTree[] args);
        int visit_struct(IExprTreeIdent ident, (string name, IExprTree value)[] fields);
        int visit_block(IExprTreeBlock block);
        int visit_unary(IExprTreeUnary unary);
        int visit_binary(IExprTreeBinary binary);
        int visit_assign(IExprTreeAssign assign);
        int visit_if(IExprTree cond, IExprTree then);
        int visit_if_else(IExprTree cond, IExprTree then, IExprTree else_then);
        int visit_let(string name, IExprTree value);
    }

    public interface IExprTreeValue : IExprTree
    {
        int accept(IExprTreeValueVisitor visitor);
        bool try_assign(System.Type ty, ref object obj);
    }

    public interface IExprTreeValueVisitor
    {
        int visit_empty();
        int visit_number(IExprTreeNumber number);
        int visit_string(string value);
        int visit_bool(bool value);
    }

    public interface IExprTreeNumber : IExprTreeValue
    {
        int accept(IExprTreeNumberVisitor visitor);
        int as_int { get; }
        long as_long { get; }
        float as_float { get; }
        double as_double { get; }
    }

    public interface IExprTreeNumberVisitor
    {
        int visit_int(int value);
        int visit_long(long value);
        int visit_float(float value);
        int visit_double(double value);
    }

    public interface IExprTreeIdent : IExprTree
    {
        string to_string();
        int accept(IExprTreeIdentVisitor visitor);
    }

    public interface IExprTreeIdentVisitor
    {
        int visit_ident(string ident);
        int visit_ident_path(string[] idents);
    }

    public interface IExprTreeBlock : IExprTree
    {
        IExprTree[] items { get; }
        bool is_block_value { get; }
    }

    public interface IExprTreeUnary : IExprTree
    {
        int accept(IExprTreeUnaryVisitor visitor);
    }

    public interface IExprTreeUnaryVisitor
    {
        int visit_neg(IExprTree value);
        int visit_not(IExprTree value);
    }

    public interface IExprTreeBinary : IExprTree
    {
        int accept(IExprTreeBinaryVisitor visitor);
    }

    public interface IExprTreeBinaryVisitor
    {
        int visit_mul(IExprTree left, IExprTree right);
        int visit_div(IExprTree left, IExprTree right);
        int visit_rem(IExprTree left, IExprTree right);
        int visit_add(IExprTree left, IExprTree right);
        int visit_sub(IExprTree left, IExprTree right);
        int visit_shl(IExprTree left, IExprTree right);
        int visit_shr(IExprTree left, IExprTree right);
        int visit_bit_and(IExprTree left, IExprTree right);
        int visit_bit_xor(IExprTree left, IExprTree right);
        int visit_bit_or(IExprTree left, IExprTree right);
        int visit_eq(IExprTree left, IExprTree right);
        int visit_not_eq(IExprTree left, IExprTree right);
        int visit_less(IExprTree left, IExprTree right);
        int visit_less_eq(IExprTree left, IExprTree right);
        int visit_greater(IExprTree left, IExprTree right);
        int visit_greater_eq(IExprTree left, IExprTree right);
        int visit_and(IExprTree left, IExprTree right);
        int visit_or(IExprTree left, IExprTree right);
    }

    public interface IExprTreeAssign : IExprTree
    {
        int accept(IExprTreeAssignVisitor visitor);
    }

    public interface IExprTreeAssignVisitor
    {
        int visit_assign(IExprTree left, IExprTree right);
        int visit_add_assign(IExprTree left, IExprTree right);
        int visit_sub_assign(IExprTree left, IExprTree right);
        int visit_mul_assign(IExprTree left, IExprTree right);
        int visit_div_assign(IExprTree left, IExprTree right);
        int visit_rem_assign(IExprTree left, IExprTree right);
        int visit_bit_and_assign(IExprTree left, IExprTree right);
        int visit_bit_xor_assign(IExprTree left, IExprTree right);
        int visit_bit_or_assign(IExprTree left, IExprTree right);
        int visit_shl_assign(IExprTree left, IExprTree right);
        int visit_shr_assign(IExprTree left, IExprTree right);
    }

    public struct ExprTreeInt : IExprTreeNumber
    {
        public int value;

        public ExprTreeInt(int value)
        {
            this.value = value;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_value(this);
        }

        public int accept(IExprTreeValueVisitor visitor)
        {
            return visitor.visit_number(this);
        }

        public int accept(IExprTreeNumberVisitor visitor)
        {
            return visitor.visit_int(value);
        }

        public bool try_assign(System.Type ty, ref object obj)
        {
            if (ty.IsAssignableFrom(typeof(int)))
            {
                obj = value;
                return true;
            }
            if (ty.IsAssignableFrom(typeof(long)))
            {
                obj = (long)value;
                return true;
            }
            if (ty.IsAssignableFrom(typeof(float)))
            {
                obj = (float)value;
                return true;
            }
            if (ty.IsAssignableFrom(typeof(double)))
            {
                obj = (double)value;
                return true;
            }
            return false;
        }

        public int as_int => value;
        public long as_long => value;
        public float as_float => value;
        public double as_double => value;
    }

    public struct ExprTreeLong : IExprTreeNumber
    {
        public long value;

        public ExprTreeLong(long value)
        {
            this.value = value;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_value(this);
        }

        public int accept(IExprTreeValueVisitor visitor)
        {
            return visitor.visit_number(this);
        }

        public int accept(IExprTreeNumberVisitor visitor)
        {
            return visitor.visit_long(value);
        }

        public bool try_assign(System.Type ty, ref object obj)
        {
            if (ty.IsAssignableFrom(typeof(long)))
            {
                obj = value;
                return true;
            }
            if (ty.IsAssignableFrom(typeof(int)))
            {
                obj = (int)value;
                return true;
            }
            if (ty.IsAssignableFrom(typeof(float)))
            {
                obj = (float)value;
                return true;
            }
            if (ty.IsAssignableFrom(typeof(double)))
            {
                obj = (double)value;
                return true;
            }
            return false;
        }

        public int as_int => (int)value;
        public long as_long => value;
        public float as_float => value;
        public double as_double => value;
    }

    public struct ExprTreeFloat : IExprTreeNumber
    {
        public float value;

        public ExprTreeFloat(float value)
        {
            this.value = value;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_value(this);
        }

        public int accept(IExprTreeValueVisitor visitor)
        {
            return visitor.visit_number(this);
        }

        public int accept(IExprTreeNumberVisitor visitor)
        {
            return visitor.visit_float(value);
        }

        public bool try_assign(System.Type ty, ref object obj)
        {
            if (ty.IsAssignableFrom(typeof(float)))
            {
                obj = value;
                return true;
            }
            if (ty.IsAssignableFrom(typeof(double)))
            {
                obj = (double)value;
                return true;
            }
            if (ty.IsAssignableFrom(typeof(int)))
            {
                obj = (int)value;
                return true;
            }
            if (ty.IsAssignableFrom(typeof(long)))
            {
                obj = (long)value;
                return true;
            }
            return false;
        }


        public int as_int => (int)value;
        public long as_long => (long)value;
        public float as_float => value;
        public double as_double => value;
    }

    public struct ExprTreeDouble : IExprTreeNumber
    {
        public double value;

        public ExprTreeDouble(double value)
        {
            this.value = value;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_value(this);
        }

        public int accept(IExprTreeValueVisitor visitor)
        {
            return visitor.visit_number(this);
        }

        public int accept(IExprTreeNumberVisitor visitor)
        {
            return visitor.visit_double(value);
        }

        public bool try_assign(System.Type ty, ref object obj)
        {
            if (ty.IsAssignableFrom(typeof(double)))
            {
                obj = value;
                return true;
            }
            if (ty.IsAssignableFrom(typeof(float)))
            {
                obj = value;
                return true;
            }
            if (ty.IsAssignableFrom(typeof(int)))
            {
                obj = (int)value;
                return true;
            }
            if (ty.IsAssignableFrom(typeof(long)))
            {
                obj = (long)value;
                return true;
            }
            return false;
        }

        public int as_int => (int)value;
        public long as_long => (long)value;
        public float as_float => (float)value;
        public double as_double => value;
    }

    public struct ExprTreeBool : IExprTreeValue
    {
        public bool value;

        public ExprTreeBool(bool value)
        {
            this.value = value;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_value(this);
        }

        public int accept(IExprTreeValueVisitor visitor)
        {
            return visitor.visit_bool(value);
        }

        public bool try_assign(System.Type ty, ref object obj)
        {
            if (ty.IsAssignableFrom(typeof(bool)))
            {
                obj = value;
                return true;
            }
            return false;
        }
    }

    public struct ExprTreeString : IExprTreeValue
    {
        public string value;

        public ExprTreeString(string value)
        {
            this.value = value;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_value(this);
        }

        public int accept(IExprTreeValueVisitor visitor)
        {
            return visitor.visit_string(value);
        }

        public bool try_assign(System.Type ty, ref object obj)
        {
            if (ty.IsAssignableFrom(typeof(string)))
            {
                obj = value;
                return true;
            }
            return false;
        }
    }

    public struct ExprTreeEmpty : IExprTreeValue
    {
        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_value(this);
        }

        public int accept(IExprTreeValueVisitor visitor)
        {
            return visitor.visit_empty();
        }
        public bool try_assign(System.Type ty, ref object obj)
        {
            if (ty.IsValueType)
            {
                obj = System.Activator.CreateInstance(ty);
            }
            else
            {
                obj = null;
            }
            return true;
        }
    }

    public struct ExprTreeIdent : IExprTreeIdent
    {
        public string ident;

        public ExprTreeIdent(string ident)
        {
            this.ident = ident;
        }

        public string to_string()
        {
            return ident;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_ident(this);
        }

        public int accept(IExprTreeIdentVisitor visitor)
        {
            return visitor.visit_ident(ident);
        }
    }

    public struct ExprTreeIdentPath : IExprTreeIdent
    {
        public string[] idents;

        public ExprTreeIdentPath(string[] idents)
        {
            this.idents = idents;
        }

        public string to_string()
        {
            var sb = new System.Text.StringBuilder();
            var iter = idents.GetEnumerator();
            if (iter.MoveNext())
            {
                sb.Append(iter.Current);
                while (iter.MoveNext())
                {
                    sb.Append(".");
                    sb.Append(iter.Current);
                }
            }
            return sb.ToString();
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_ident(this);
        }

        public int accept(IExprTreeIdentVisitor visitor)
        {
            return visitor.visit_ident_path(idents);
        }
    }

    public struct ExprTreeCall : IExprTree
    {
        public IExprTreeIdent ident;
        public IExprTree[] args;

        public ExprTreeCall(IExprTreeIdent ident, IExprTree[] args)
        {
            this.ident = ident;
            this.args = args;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_call(ident, args);
        }
    }

    public struct ExprTreeStruct : IExprTree
    {
        public IExprTreeIdent ident;
        public (string name, IExprTree value)[] fields;

        public ExprTreeStruct(IExprTreeIdent ident, (string, IExprTree)[] fields)
        {
            this.ident = ident;
            this.fields = fields;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_struct(ident, fields);
        }
    }

    public struct ExprTreeBlock : IExprTreeBlock
    {
        public IExprTree[] items;

        public ExprTreeBlock(IExprTree[] items)
        {
            this.items = items;
        }

        IExprTree[] IExprTreeBlock.items => items;
        bool IExprTreeBlock.is_block_value => false;

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_block(this);
        }
    }

    public struct ExprTreeBlockValue : IExprTreeBlock
    {
        public IExprTree[] items;

        public ExprTreeBlockValue(IExprTree[] items)
        {
            this.items = items;
        }

        IExprTree[] IExprTreeBlock.items => items;
        bool IExprTreeBlock.is_block_value => true;

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_block(this);
        }
    }

    public struct ExprTreeNeg : IExprTreeUnary
    {
        public IExprTree value;

        public ExprTreeNeg(IExprTree value)
        {
            this.value = value;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_unary(this);
        }

        public int accept(IExprTreeUnaryVisitor visitor)
        {
            return visitor.visit_neg(value);
        }
    }

    public struct ExprTreeNot : IExprTreeUnary
    {
        public IExprTree value;

        public ExprTreeNot(IExprTree value)
        {
            this.value = value;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_unary(this);
        }

        public int accept(IExprTreeUnaryVisitor visitor)
        {
            return visitor.visit_not(value);
        }
    }

    public struct ExprTreeAdd : IExprTreeBinary
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeAdd(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_binary(this);
        }

        public int accept(IExprTreeBinaryVisitor visitor)
        {
            return visitor.visit_add(left, right);
        }
    }

    public struct ExprTreeSub : IExprTreeBinary
    {
        public IExprTree left;
        public IExprTree right;
        public ExprTreeSub(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_binary(this);
        }

        public int accept(IExprTreeBinaryVisitor visitor)
        {
            return visitor.visit_sub(left, right);
        }
    }

    public struct ExprTreeMul : IExprTreeBinary
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeMul(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_binary(this);
        }

        public int accept(IExprTreeBinaryVisitor visitor)
        {
            return visitor.visit_mul(left, right);
        }
    }

    public struct ExprTreeDiv : IExprTreeBinary
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeDiv(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_binary(this);
        }

        public int accept(IExprTreeBinaryVisitor visitor)
        {
            return visitor.visit_div(left, right);
        }
    }

    public struct ExprTreeRem : IExprTreeBinary
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeRem(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_binary(this);
        }

        public int accept(IExprTreeBinaryVisitor visitor)
        {
            return visitor.visit_rem(left, right);
        }
    }

    public struct ExprTreeShl : IExprTreeBinary
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeShl(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_binary(this);
        }

        public int accept(IExprTreeBinaryVisitor visitor)
        {
            return visitor.visit_shl(left, right);
        }
    }

    public struct ExprTreeShr : IExprTreeBinary
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeShr(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_binary(this);
        }

        public int accept(IExprTreeBinaryVisitor visitor)
        {
            return visitor.visit_shr(left, right);
        }
    }

    public struct ExprTreeBitAnd : IExprTreeBinary
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeBitAnd(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_binary(this);
        }

        public int accept(IExprTreeBinaryVisitor visitor)
        {
            return visitor.visit_bit_and(left, right);
        }
    }

    public struct ExprTreeBitOr : IExprTreeBinary
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeBitOr(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_binary(this);
        }

        public int accept(IExprTreeBinaryVisitor visitor)
        {
            return visitor.visit_bit_or(left, right);
        }
    }

    public struct ExprTreeBitXor : IExprTreeBinary
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeBitXor(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_binary(this);
        }

        public int accept(IExprTreeBinaryVisitor visitor)
        {
            return visitor.visit_bit_xor(left, right);
        }
    }

    public struct ExprTreeEq : IExprTreeBinary
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeEq(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_binary(this);
        }

        public int accept(IExprTreeBinaryVisitor visitor)
        {
            return visitor.visit_eq(left, right);
        }
    }

    public struct ExprTreeNotEq : IExprTreeBinary
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeNotEq(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_binary(this);
        }

        public int accept(IExprTreeBinaryVisitor visitor)
        {
            return visitor.visit_not_eq(left, right);
        }
    }

    public struct ExprTreeLess : IExprTreeBinary
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeLess(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_binary(this);
        }

        public int accept(IExprTreeBinaryVisitor visitor)
        {
            return visitor.visit_less(left, right);
        }
    }

    public struct ExprTreeGreater : IExprTreeBinary
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeGreater(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_binary(this);
        }

        public int accept(IExprTreeBinaryVisitor visitor)
        {
            return visitor.visit_greater(left, right);
        }
    }

    public struct ExprTreeLessEq : IExprTreeBinary
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeLessEq(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_binary(this);
        }

        public int accept(IExprTreeBinaryVisitor visitor)
        {
            return visitor.visit_less_eq(left, right);
        }
    }

    public struct ExprTreeGreaterEq : IExprTreeBinary
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeGreaterEq(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_binary(this);
        }

        public int accept(IExprTreeBinaryVisitor visitor)
        {
            return visitor.visit_greater_eq(left, right);
        }
    }

    public struct ExprTreeAnd : IExprTreeBinary
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeAnd(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_binary(this);
        }

        public int accept(IExprTreeBinaryVisitor visitor)
        {
            return visitor.visit_and(left, right);
        }
    }

    public struct ExprTreeOr : IExprTreeBinary
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeOr(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_binary(this);
        }

        public int accept(IExprTreeBinaryVisitor visitor)
        {
            return visitor.visit_or(left, right);
        }
    }

    public struct ExprTreeAssign : IExprTreeAssign
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeAssign(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }
        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_assign(this);
        }

        public int accept(IExprTreeAssignVisitor visitor)
        {
            return visitor.visit_assign(left, right);
        }
    }

    public struct ExprTreeAddAssign : IExprTreeAssign
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeAddAssign(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_assign(this);
        }

        public int accept(IExprTreeAssignVisitor visitor)
        {
            return visitor.visit_add_assign(left, right);
        }
    }

    public struct ExprTreeSubAssign : IExprTreeAssign
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeSubAssign(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_assign(this);
        }

        public int accept(IExprTreeAssignVisitor visitor)
        {
            return visitor.visit_sub_assign(left, right);
        }
    }

    public struct ExprTreeMulAssign : IExprTreeAssign
    {
        public IExprTree left;
        public IExprTree right;

        public ExprTreeMulAssign(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_assign(this);
        }

        public int accept(IExprTreeAssignVisitor visitor)
        {
            return visitor.visit_mul_assign(left, right);
        }
    }

    public struct ExprTreeDivAssign : IExprTreeAssign
    {
        public IExprTree left;
        public IExprTree right;
        public ExprTreeDivAssign(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_assign(this);
        }

        public int accept(IExprTreeAssignVisitor visitor)
        {
            return visitor.visit_div_assign(left, right);
        }
    }

    public struct ExprTreeRemAssign : IExprTreeAssign
    {
        public IExprTree left;
        public IExprTree right;
        public ExprTreeRemAssign(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }
        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_assign(this);
        }

        public int accept(IExprTreeAssignVisitor visitor)
        {
            return visitor.visit_rem_assign(left, right);
        }
    }

    public struct ExprTreeBitAndAssign : IExprTreeAssign
    {
        public IExprTree left;
        public IExprTree right;
        public ExprTreeBitAndAssign(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }
        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_assign(this);
        }

        public int accept(IExprTreeAssignVisitor visitor)
        {
            return visitor.visit_bit_and_assign(left, right);
        }
    }

    public struct ExprTreeBitOrAssign : IExprTreeAssign
    {
        public IExprTree left;
        public IExprTree right;
        public ExprTreeBitOrAssign(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }
        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_assign(this);
        }

        public int accept(IExprTreeAssignVisitor visitor)
        {
            return visitor.visit_bit_or_assign(left, right);
        }
    }

    public struct ExprTreeBitXorAssign : IExprTreeAssign
    {
        public IExprTree left;
        public IExprTree right;
        public ExprTreeBitXorAssign(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }
        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_assign(this);
        }

        public int accept(IExprTreeAssignVisitor visitor)
        {
            return visitor.visit_bit_xor_assign(left, right);
        }
    }

    public struct ExprTreeShlAssign : IExprTreeAssign
    {
        public IExprTree left;
        public IExprTree right;
        public ExprTreeShlAssign(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }
        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_assign(this);
        }

        public int accept(IExprTreeAssignVisitor visitor)
        {
            return visitor.visit_shl_assign(left, right);
        }
    }

    public struct ExprTreeShrAssign : IExprTreeAssign
    {
        public IExprTree left;
        public IExprTree right;
        public ExprTreeShrAssign(IExprTree left, IExprTree right)
        {
            this.left = left;
            this.right = right;
        }
        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_assign(this);
        }

        public int accept(IExprTreeAssignVisitor visitor)
        {
            return visitor.visit_shr_assign(left, right);
        }
    }

    public struct ExprTreeIf : IExprTree
    {
        public IExprTree cond;
        public IExprTree then;

        public ExprTreeIf(IExprTree cond, IExprTree then)
        {
            this.cond = cond;
            this.then = then;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_if(cond, then);
        }
    }

    public struct ExprTreeIfElse : IExprTree
    {
        public IExprTree cond;
        public IExprTree then;
        public IExprTree else_then;

        public ExprTreeIfElse(IExprTree cond, IExprTree then, IExprTree else_then)
        {
            this.cond = cond;
            this.then = then;
            this.else_then = else_then;
        }
        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_if_else(cond, then, else_then);
        }
    }

    public struct ExprTreeLet : IExprTree
    {
        public string name;
        public IExprTree value;

        public ExprTreeLet(string name, IExprTree value)
        {
            this.name = name;
            this.value = value;
        }

        public int accept(IExprTreeVisitor visitor)
        {
            return visitor.visit_let(name, value);
        }
    }

    public static class ExprTree
    {
        public static IExprTree load(BinaryReader r, string[] ss)
        {
            switch (r.ReadByte())
            {
                case 0:
                    return new ExprTreeEmpty();
                case 1:
                    return new ExprTreeBool(true);
                case 2:
                    return new ExprTreeBool(false);
                case 3:
                    return new ExprTreeInt((int)CompressedInt.decompress_int(r));
                case 4:
                    return new ExprTreeLong(CompressedInt.decompress_int(r));
                case 5:
                    return new ExprTreeFloat(r.ReadSingle());
                case 6:
                    return new ExprTreeDouble(r.ReadDouble());
                case 7:
                    return new ExprTreeString(FieldReader.read_indexed_string(r, ss));
                case 8:
                    return new ExprTreeIdent(FieldReader.read_indexed_string(r, ss));
                case 9:
                    {
                        var len = (int)CompressedInt.decompress_uint(r);
                        var idents = new string[len];
                        for (int i = 0; i < len; ++i)
                        {
                            idents[i] = FieldReader.read_indexed_string(r, ss);
                        }
                        return new ExprTreeIdentPath(idents);
                    }
                case 10:
                    {
                        var ident = new ExprTreeIdent(FieldReader.read_indexed_string(r, ss));
                        var len = (int)CompressedInt.decompress_uint(r);
                        var args = new IExprTree[len];
                        for (int i = 0; i < len; ++i)
                        {
                            args[i] = load(r, ss);
                        }
                        return new ExprTreeCall(ident, args);
                    }
                case 11:
                    {
                        var len = (int)CompressedInt.decompress_uint(r);
                        var idents = new string[len];
                        for (int i = 0; i < len; ++i)
                        {
                            idents[i] = FieldReader.read_indexed_string(r, ss);
                        }
                        len = (int)CompressedInt.decompress_uint(r);
                        var args = new IExprTree[len];
                        for (int i = 0; i < len; ++i)
                        {
                            args[i] = load(r, ss);
                        }
                        return new ExprTreeCall(new ExprTreeIdentPath(idents), args);
                    }
                case 12:
                    {
                        var ident = new ExprTreeIdent(FieldReader.read_indexed_string(r, ss));
                        var len = (int)CompressedInt.decompress_uint(r);
                        var fields = new (string, IExprTree)[len];
                        for (int i = 0; i < len; ++i)
                        {
                            fields[i] = (FieldReader.read_indexed_string(r, ss), load(r, ss));
                        }
                        return new ExprTreeStruct(ident, fields);
                    }
                case 13:
                    {
                        var len = (int)CompressedInt.decompress_uint(r);
                        var idents = new string[len];
                        for (int i = 0; i < len; ++i)
                        {
                            idents[i] = FieldReader.read_indexed_string(r, ss);
                        }
                        len = (int)CompressedInt.decompress_uint(r);
                        var fields = new (string, IExprTree)[len];
                        for (int i = 0; i < len; ++i)
                        {
                            fields[i] = (FieldReader.read_indexed_string(r, ss), load(r, ss));
                        }
                        return new ExprTreeStruct(new ExprTreeIdentPath(idents), fields);
                    }
                case 14:
                    {
                        var len = (int)CompressedInt.decompress_uint(r);
                        var items = new IExprTree[len];
                        for (int i = 0; i < len; ++i)
                        {
                            items[i] = load(r, ss);
                        }
                        return new ExprTreeBlock(items);
                    }
                case 15:
                    {
                        var len = (int)CompressedInt.decompress_uint(r);
                        var items = new IExprTree[len];
                        for (int i = 0; i < len; ++i)
                        {
                            items[i] = load(r, ss);
                        }
                        return new ExprTreeBlockValue(items);
                    }
                case 16:
                    return new ExprTreeNeg(load(r, ss));
                case 17:
                    return new ExprTreeNot(load(r, ss));
                case 18:
                    return new ExprTreeMul(load(r, ss), load(r, ss));
                case 19:
                    return new ExprTreeDiv(load(r, ss), load(r, ss));
                case 20:
                    return new ExprTreeRem(load(r, ss), load(r, ss));
                case 21:
                    return new ExprTreeAdd(load(r, ss), load(r, ss));
                case 22:
                    return new ExprTreeSub(load(r, ss), load(r, ss));
                case 23:
                    return new ExprTreeShl(load(r, ss), load(r, ss));
                case 24:
                    return new ExprTreeShr(load(r, ss), load(r, ss));
                case 25:
                    return new ExprTreeBitAnd(load(r, ss), load(r, ss));
                case 26:
                    return new ExprTreeBitXor(load(r, ss), load(r, ss));
                case 27:
                    return new ExprTreeBitOr(load(r, ss), load(r, ss));
                case 28:
                    return new ExprTreeEq(load(r, ss), load(r, ss));
                case 29:
                    return new ExprTreeNotEq(load(r, ss), load(r, ss));
                case 30:
                    return new ExprTreeLess(load(r, ss), load(r, ss));
                case 31:
                    return new ExprTreeLessEq(load(r, ss), load(r, ss));
                case 32:
                    return new ExprTreeGreater(load(r, ss), load(r, ss));
                case 33:
                    return new ExprTreeGreaterEq(load(r, ss), load(r, ss));
                case 34:
                    return new ExprTreeAnd(load(r, ss), load(r, ss));
                case 35:
                    return new ExprTreeOr(load(r, ss), load(r, ss));
                case 36:
                    return new ExprTreeAssign(load(r, ss), load(r, ss));
                case 37:
                    return new ExprTreeAddAssign(load(r, ss), load(r, ss));
                case 38:
                    return new ExprTreeSubAssign(load(r, ss), load(r, ss));
                case 39:
                    return new ExprTreeMulAssign(load(r, ss), load(r, ss));
                case 40:
                    return new ExprTreeDivAssign(load(r, ss), load(r, ss));
                case 41:
                    return new ExprTreeRemAssign(load(r, ss), load(r, ss));
                case 42:
                    return new ExprTreeBitAndAssign(load(r, ss), load(r, ss));
                case 43:
                    return new ExprTreeBitXorAssign(load(r, ss), load(r, ss));
                case 44:
                    return new ExprTreeBitOrAssign(load(r, ss), load(r, ss));
                case 45:
                    return new ExprTreeShlAssign(load(r, ss), load(r, ss));
                case 46:
                    return new ExprTreeShrAssign(load(r, ss), load(r, ss));
                case 47:
                    return new ExprTreeIf(load(r, ss), load(r, ss));
                case 48:
                    return new ExprTreeIfElse(load(r, ss), load(r, ss), load(r, ss));
                case 49:
                    return new ExprTreeLet(FieldReader.read_indexed_string(r, ss), load(r, ss));
                default:
                    throw new InvalidDataException();
            }
        }

        public static void skip_by_read(BinaryReader r)
        {
            switch (r.ReadByte())
            {
                case 0:
                case 1:
                case 2:
                    return;
                case 3:
                case 4:
                    CompressedInt.decompress_int(r);
                    return;
                case 5:
                    r.ReadSingle();
                    return;
                case 6:
                    r.ReadDouble();
                    return;
                case 7:
                case 8:
                    CompressedInt.decompress_uint(r);
                    return;
                case 9:
                    {
                        var len = (int)CompressedInt.decompress_uint(r);
                        for (int i = 0; i < len; ++i)
                        {
                            CompressedInt.decompress_uint(r);
                        }
                        return;
                    }
                case 10:
                    {
                        CompressedInt.decompress_uint(r);
                        var len = (int)CompressedInt.decompress_uint(r);
                        for (int i = 0; i < len; ++i)
                        {
                            skip_by_read(r);
                        }
                        return;
                    }
                case 11:
                    {
                        var len = (int)CompressedInt.decompress_uint(r);
                        for (int i = 0; i < len; ++i)
                        {
                            CompressedInt.decompress_uint(r);
                        }
                        len = (int)CompressedInt.decompress_uint(r);
                        for (int i = 0; i < len; ++i)
                        {
                            skip_by_read(r);
                        }
                        return;
                    }
                case 12:
                    {
                        CompressedInt.decompress_uint(r);
                        var len = (int)CompressedInt.decompress_uint(r);
                        for (int i = 0; i < len; ++i)
                        {
                            CompressedInt.decompress_uint(r);
                            skip_by_read(r);
                        }
                        return;
                    }
                case 13:
                    {
                        var len = (int)CompressedInt.decompress_uint(r);
                        for (int i = 0; i < len; ++i)
                        {
                            CompressedInt.decompress_uint(r);
                        }
                        len = (int)CompressedInt.decompress_uint(r);
                        for (int i = 0; i < len; ++i)
                        {
                            CompressedInt.decompress_uint(r);
                            skip_by_read(r);
                        }
                        return;
                    }
                case 14:
                case 15:
                    {
                        var len = (int)CompressedInt.decompress_uint(r);
                        for (int i = 0; i < len; ++i)
                        {
                            skip_by_read(r);
                        }
                        return;
                    }
                case 16:
                case 17:
                    skip_by_read(r);
                    return;
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                case 30:
                case 31:
                case 32:
                case 33:
                case 34:
                case 35:
                case 36:
                case 37:
                case 38:
                case 39:
                case 40:
                case 41:
                case 42:
                case 43:
                case 44:
                case 45:
                case 46:
                case 47:
                    skip_by_read(r);
                    skip_by_read(r);
                    return;
                case 48:
                    skip_by_read(r);
                    skip_by_read(r);
                    skip_by_read(r);
                    return;
                case 49:
                    CompressedInt.decompress_uint(r);
                    skip_by_read(r);
                    return;
                default:
                    throw new InvalidDataException();
            }
        }
    }


    public class ExprTreeConverter
    {

        public ExprTreeConverter(string class_prefix, string class_suffix)
        {
            m_call_or_struct_class_prefix = m_others_class_prefix = class_prefix ?? string.Empty;
            m_call_or_struct_class_suffix = m_others_class_suffix = class_suffix ?? string.Empty;
            m_visitor = new Visitor(this);
        }

        public ExprTreeConverter(string call_or_struct_class_prefix, string call_or_struct_class_suffix, string others_class_prefix, string others_class_suffix)
        {
            m_call_or_struct_class_prefix = call_or_struct_class_prefix ?? string.Empty;
            m_call_or_struct_class_suffix = call_or_struct_class_suffix ?? string.Empty;
            m_others_class_prefix = others_class_prefix ?? string.Empty;
            m_others_class_suffix = others_class_suffix ?? string.Empty;
            m_visitor = new Visitor(this);
        }

        public bool convert(IExprTree value, out object obj, out string err_msg)
        {
            var r = value.accept(m_visitor);
            if (r != 0)
            {
                foreach (ref var arg in ArraySlice.create(m_args, 0, m_argc))
                {
                    arg = null;
                }
                m_argc = 0;
                err_msg = m_err_msg;
                m_err_msg = null;
                obj = null;
                return false;
            }
            ref var ret = ref m_args[0];
            obj = ret;
            ret = null;
            m_argc = 0;
            err_msg = null;
            return true;
        }

        private string m_call_or_struct_class_prefix;
        private string m_call_or_struct_class_suffix;
        private string m_others_class_prefix;
        private string m_others_class_suffix;
        private object[] m_args = new object[32];
        private int m_argc = 0;
        private string m_err_msg;
        private Visitor m_visitor;
        private static Dictionary<string, System.Type> s_catched_types = new Dictionary<string, System.Type>();
        private static System.Type[] s_default_ctor_types = new System.Type[] { typeof(ArraySlice<object>) };

        private bool find_call_or_struct_class(string name, out System.Type type)
        {
            return find_class($"{m_call_or_struct_class_prefix}{name}{m_call_or_struct_class_suffix}", out type);
        }

        private bool find_others_class(string name, out System.Type type)
        {
            return find_class($"{m_others_class_prefix}{name}{m_others_class_suffix}", out type);
        }

        private static bool find_class(string name, out System.Type type)
        {
            lock (s_catched_types)
            {
                if (s_catched_types.TryGetValue(name, out type))
                {
                    return true;
                }
                type = System.Type.GetType(name, false);
                if (type != null)
                {
                    s_catched_types.Add(name, type);
                    return true;
                }
                return false;
            }
        }

        private void push_arg(object arg)
        {
            if (m_argc == m_args.Length)
            {
                var args = new object[m_argc * 2];
                m_args.CopyTo(args, 0);
                m_args = args;
            }
            m_args[m_argc] = arg;
            m_argc += 1;
        }

        private bool convert(System.Type ty, ArraySlice<object> args, out object obj)
        {
            var ctor = ty.GetConstructor(s_default_ctor_types);
            if (ctor != null)
            {
                obj = ctor.Invoke(new object[] { args });
                return true;
            }

            var ps = new List<object>();
            foreach (var ci in ty.GetConstructors())
            {
                var pia = ci.GetParameters();
                object p = null;
                if (pia.Length == 0)
                {
                    if (args.length == 0)
                    {
                        obj = ci.Invoke(null);
                        return true;
                    }
                }
                else
                {
                    ps.Clear();
                    ctor = ci;
                    var last = pia.Length - 1;
                    for (int i = 0; i < last; ++i)
                    {
                        if (i == args.length)
                        {
                            ctor = null;
                            break;
                        }
                        if (try_assign_object(pia[i].ParameterType, args[i], ref p))
                        {
                            ps.Add(p);
                        }
                        else
                        {
                            ctor = null;
                            break;
                        }
                    }
                    if (ctor == null)
                    {
                        continue;
                    }
                    var pi = pia[last];
                    if (pi.GetCustomAttribute<System.ParamArrayAttribute>() != null)
                    {
                        var et = pi.ParameterType.GetElementType();
                        var array = System.Array.CreateInstance(et, args.length - last);
                        for (int i = 0; i < array.Length; ++i)
                        {
                            if (try_assign_object(et, args[last + i], ref p))
                            {
                                ps.Add(p);
                            }
                            else
                            {
                                ctor = null;
                                break;
                            }
                        }
                        if (ctor != null)
                        {
                            ps.Add(array);
                            obj = ctor.Invoke(ps.ToArray());
                            return true;
                        }
                    }
                    if (ctor == null)
                    {
                        continue;
                    }
                    if (last + 1 != args.length)
                    {
                        continue;
                    }
                    var lasta = args[last];
                    if (try_assign_object(pi.ParameterType, args[last], ref p))
                    {
                        ps.Add(p);
                    }
                    else
                    {
                        continue;
                    }
                    obj = ctor.Invoke(ps.ToArray());
                    return true;
                }
            }
            obj = null;
            return false;
        }

        private bool convert_others(string name, ArraySlice<object> args, out object obj, out string err_msg)
        {
            if (!find_others_class(name, out var ty))
            {
                obj = null;
                err_msg = $"`{name}` does not exist";
                return false;
            }
            if (!convert(ty, args, out obj))
            {
                err_msg = $"`{name}` no matched ctor";
                return false;
            }
            err_msg = null;
            return true;
        }

        private bool convert_call(string name, ArraySlice<object> args, out object obj, out string err_msg)
        {
            if (!find_call_or_struct_class(name, out var ty))
            {
                obj = null;
                err_msg = $"`{name}` does not exist";
                return false;
            }
            if (!convert(ty, args, out obj))
            {
                err_msg = $"`{name}` no matched ctor";
                return false;
            }
            err_msg = null;
            return true;
        }

        private static bool try_assign_object(System.Type ty, object src, ref object dst)
        {
            if (ty.IsAssignableFrom(src.GetType()))
            {
                dst = src;
                return true;
            }
            if (src is IExprTreeValue val)
            {
                return val.try_assign(ty, ref dst);
            }
            if (src is IExprTreeIdent ident && ty.IsAssignableFrom(typeof(string)))
            {
                dst = ident.to_string();
                return true;
            }
            return false;
        }

        private void append_err_msg(string err_msg)
        {
            m_err_msg = err_msg;
            /*
            if (m_err_msg_builder == null) {
                m_err_msg_builder = new System.Text.StringBuilder(err_msg);
            } else {
                m_err_msg_builder.AppendLine();
                m_err_msg_builder.Append(err_msg);
            }
            */
        }

        private class Visitor : IExprTreeVisitor, IExprTreeUnaryVisitor, IExprTreeBinaryVisitor, IExprTreeAssignVisitor
        {

            public readonly ExprTreeConverter self;

            public Visitor(ExprTreeConverter self)
            {
                this.self = self;
            }

            private int convert_others(string name, IExprTree left, IExprTree right)
            {
                int offset = self.m_argc;
                var r = left.accept(this);
                if (r != 0)
                {
                    return r;
                }
                r = right.accept(this);
                if (r != 0)
                {
                    return r;
                }
                var args = ArraySlice.create(self.m_args, offset, 2);
                if (self.convert_others(name, args, out var obj, out var err_msg))
                {
                    foreach (ref var a in args)
                    {
                        a = null;
                    }
                    self.m_argc -= 2;
                    self.push_arg(obj);
                    return 0;
                }
                self.append_err_msg(err_msg);
                return 1;
            }

            private int convert_others(string name, IExprTree value)
            {
                int offset = self.m_argc;
                var r = value.accept(this);
                if (r != 0)
                {
                    return r;
                }
                var args = ArraySlice.create(self.m_args, offset, 1);
                if (self.convert_others(name, args, out var obj, out var err_msg))
                {
                    foreach (ref var a in args)
                    {
                        a = null;
                    }
                    self.m_argc -= 1;
                    self.push_arg(obj);
                    return 0;
                }
                self.append_err_msg(err_msg);
                return 1;
            }

            public int visit_add(IExprTree left, IExprTree right)
            {
                return convert_others("Add", left, right);
            }

            public int visit_add_assign(IExprTree left, IExprTree right)
            {
                return convert_others("AddAssign", left, right);
            }

            public int visit_and(IExprTree left, IExprTree right)
            {
                return convert_others("And", left, right);
            }

            public int visit_assign(IExprTreeAssign assign)
            {
                return assign.accept(this);
            }

            public int visit_assign(IExprTree left, IExprTree right)
            {
                return convert_others("Assign", left, right);
            }

            public int visit_binary(IExprTreeBinary binary)
            {
                return binary.accept(this);
            }

            public int visit_bit_and(IExprTree left, IExprTree right)
            {
                return convert_others("BitAdd", left, right);
            }

            public int visit_bit_and_assign(IExprTree left, IExprTree right)
            {
                return convert_others("AddBitAndAssign", left, right);
            }

            public int visit_bit_or(IExprTree left, IExprTree right)
            {
                return convert_others("BitOr", left, right);
            }

            public int visit_bit_or_assign(IExprTree left, IExprTree right)
            {
                return convert_others("BitOrAssign", left, right);
            }

            public int visit_bit_xor(IExprTree left, IExprTree right)
            {
                return convert_others("BitXor", left, right);
            }

            public int visit_bit_xor_assign(IExprTree left, IExprTree right)
            {
                return convert_others("BitXorAssign", left, right);
            }

            public int visit_block(IExprTreeBlock block)
            {
                var items = block.items;
                int offset = self.m_argc;
                foreach (var item in items)
                {
                    var r = item.accept(this);
                    if (r != 0)
                    {
                        return r;
                    }
                }
                var args = ArraySlice.create(self.m_args, offset, items.Length);
                if (self.convert_others(block.is_block_value ? "BlockValue" : "Block", args, out var obj, out var err_msg))
                {
                    foreach (ref var a in args)
                    {
                        a = null;
                    }
                    self.m_argc -= items.Length;
                    self.push_arg(obj);
                    return 0;
                }
                self.append_err_msg(err_msg);
                return 1;
            }

            public int visit_call(IExprTreeIdent ident, IExprTree[] args)
            {
                var offset = self.m_argc;
                foreach (var arg in args)
                {
                    var r = arg.accept(this);
                    if (r != 0)
                    {
                        return r;
                    }
                }
                var _args = ArraySlice.create(self.m_args, offset, args.Length);
                if (self.convert_call(ident.to_string(), _args, out var obj, out var err_msg))
                {
                    foreach (ref var a in _args)
                    {
                        a = null;
                    }
                    self.m_argc -= args.Length;
                    self.push_arg(obj);
                    return 0;
                }
                self.append_err_msg(err_msg);
                return 1;
            }

            public int visit_div(IExprTree left, IExprTree right)
            {
                return convert_others("Div", left, right);
            }

            public int visit_div_assign(IExprTree left, IExprTree right)
            {
                return convert_others("DivAssign", left, right);
            }

            public int visit_eq(IExprTree left, IExprTree right)
            {
                return convert_others("Eq", left, right);
            }

            public int visit_greater(IExprTree left, IExprTree right)
            {
                return convert_others("Greater", left, right);
            }

            public int visit_greater_eq(IExprTree left, IExprTree right)
            {
                return convert_others("GreaterEq", left, right);
            }

            public int visit_ident(IExprTreeIdent ident)
            {
                self.push_arg(ident);
                return 0;
            }

            public int visit_if(IExprTree cond, IExprTree then)
            {
                return convert_others("If", cond, then);
            }

            public int visit_if_else(IExprTree cond, IExprTree then, IExprTree else_then)
            {
                int offset = self.m_argc;
                var r = cond.accept(this);
                if (r != 0)
                {
                    return r;
                }
                r = then.accept(this);
                if (r != 0)
                {
                    return r;
                }
                r = else_then.accept(this);
                if (r != 0)
                {
                    return r;
                }
                var args = ArraySlice.create(self.m_args, offset, 3);
                if (self.convert_others("IfElse", args, out var obj, out var err_msg))
                {
                    foreach (ref var a in args)
                    {
                        a = null;
                    }
                    self.m_argc -= 3;
                    self.push_arg(obj);
                    return 0;
                }
                self.append_err_msg(err_msg);
                return 1;
            }

            public int visit_let(string name, IExprTree value)
            {
                int offset = self.m_argc;
                self.push_arg(name);
                var r = value.accept(this);
                if (r != 0)
                {
                    return r;
                }
                var args = ArraySlice.create(self.m_args, offset, 2);
                if (self.convert_others("Let", args, out var obj, out var err_msg))
                {
                    foreach (ref var a in args)
                    {
                        a = null;
                    }
                    self.m_argc -= 2;
                    self.push_arg(obj);
                    return 0;
                }
                self.append_err_msg(err_msg);
                return 1;
            }

            public int visit_less(IExprTree left, IExprTree right)
            {
                return convert_others("Less", left, right);
            }

            public int visit_less_eq(IExprTree left, IExprTree right)
            {
                return convert_others("LessEq", left, right);
            }

            public int visit_mul(IExprTree left, IExprTree right)
            {
                return convert_others("Less", left, right);
            }

            public int visit_mul_assign(IExprTree left, IExprTree right)
            {
                return convert_others("MulAssign", left, right);
            }

            public int visit_neg(IExprTree value)
            {
                return convert_others("Neg", value);
            }

            public int visit_not(IExprTree value)
            {
                return convert_others("Not", value);
            }

            public int visit_not_eq(IExprTree left, IExprTree right)
            {
                return convert_others("NotEq", left, right);
            }

            public int visit_or(IExprTree left, IExprTree right)
            {
                return convert_others("Or", left, right);
            }

            public int visit_rem(IExprTree left, IExprTree right)
            {
                return convert_others("Rem", left, right);
            }

            public int visit_rem_assign(IExprTree left, IExprTree right)
            {
                return convert_others("RemAssign", left, right);
            }

            public int visit_shl(IExprTree left, IExprTree right)
            {
                return convert_others("Shl", left, right);
            }

            public int visit_shl_assign(IExprTree left, IExprTree right)
            {
                return convert_others("ShlAssign", left, right);
            }

            public int visit_shr(IExprTree left, IExprTree right)
            {
                return convert_others("Shr", left, right);
            }

            public int visit_shr_assign(IExprTree left, IExprTree right)
            {
                return convert_others("ShrAssign", left, right);
            }

            public int visit_struct(IExprTreeIdent ident, (string name, IExprTree value)[] fields)
            {
                var name = ident.to_string();
                if (!self.find_call_or_struct_class(name, out var ty))
                {
                    self.append_err_msg($"`{name}` does not exist");
                    return 1;
                }
                var ctor = ty.GetConstructor(System.Type.EmptyTypes);
                if (ctor == null)
                {
                    self.append_err_msg($"`{name}` no matched ctor");
                    return 1;
                }
                var offset = self.m_argc;
                var len = fields.Length;
                foreach (var field in fields)
                {
                    var r = field.value.accept(this);
                    if (r != 0)
                    {
                        return r;
                    }
                }
                var args = ArraySlice.create(self.m_args, offset, len);
                var obj = ctor.Invoke(null);
                object p = null;
                for (int i = 0; i < len; ++i)
                {
                    ref var arg = ref args[i];
                    var pi = ty.GetProperty(fields[i].name);
                    if (pi != null && pi.CanWrite && try_assign_object(pi.PropertyType, arg, ref p))
                    {
                        pi.SetValue(obj, p);
                    }
                    arg = null;
                }
                self.m_argc -= len;
                self.push_arg(obj);
                return 0;
            }

            public int visit_sub(IExprTree left, IExprTree right)
            {
                return convert_others("Sub", left, right);
            }

            public int visit_sub_assign(IExprTree left, IExprTree right)
            {
                return convert_others("SubAssign", left, right);
            }

            public int visit_unary(IExprTreeUnary unary)
            {
                return unary.accept(this);
            }

            public int visit_value(IExprTreeValue value)
            {
                self.push_arg(value);
                return 0;
            }
        }
    }
}
